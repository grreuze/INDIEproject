using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Link : MonoBehaviour {

    #region Properties
    
    [Header("Line Renderer Properties"), SerializeField, Tooltip("The start and end width of the line renderer")]
    float _width = 0.5f;
    float width {
        get { return _width / transform.lossyScale.x; }
    }
    [SerializeField]
    float segmentLength = 0.4f;

    [SerializeField]
    Material mat;
    [SerializeField]
    Material hoverMat;

    [Header("Link Properties")]
    public Element origin;
    public Element target;
    public int originLoop;
    public int targetLoop;
    public List<Prism> prismToOrigin;
    public List<Prism> prismToTarget;
    public bool connected;
    [SerializeField]
    bool adjustWidth;
    
    [Header("Animation"), SerializeField]
    float waveHeight = 1.5f;
    [SerializeField]
    float waveLength = 0.3f, waveDuration = 0.2f, waveSpeed = 60;

    /// <summary>
    /// Returns whether or not the Link is currently visible by the player.
    /// </summary>
    public bool isVisible {
        get { return (originMetaPos != targetMetaPos) || (originMetaPos == targetMetaPos && targetMetaPos == MetaPosition.InRange); }
    }

    /// <summary>
    /// The length of the link. Distance between originPosition and targetPosition.
    /// </summary>
    public float length {
        get { return Vector3.Distance(originPosition, targetPosition) / transform.lossyScale.x; }
    }

    enum MetaPosition { InRange, External, Internal }
    MetaPosition originMetaPos, targetMetaPos;
    Existence existence;
    LineRenderer line;
    BoxCollider col;
    bool destroyed, animated;

    float startWidth {
        get {
            return originMetaPos == MetaPosition.Internal ? 0 : origin.transform.lossyScale.x * width * transform.localScale.x;
        }
    }

    float endWidth {
        get {
            return targetMetaPos == MetaPosition.Internal ? 0 : target.transform.lossyScale.x * width * transform.localScale.x;
        }
    }

    WorldInstance worldInstance {
        get { return origin.worldInstance; }
    }

    Vector3 originPosition {
        get {
            bool moving = origin.isHeld || origin.anchored;
            int worldLoop = moving ? WorldWrapper.singleton.currentInstance.loop : worldInstance.loop;
            return GetMetaPosition(origin.transform.position, ref originMetaPos, originLoop, worldLoop);
        }
    }

    Vector3 targetPosition {
        get {
            bool moving = target.isHeld || target.anchored;
            int worldLoop = moving ? WorldWrapper.singleton.currentInstance.loop : target.worldInstance.loop;
            return GetMetaPosition(target.transform.position, ref targetMetaPos, targetLoop, worldLoop);
        }
    }

    float screenDepth;

    #endregion

    #region MonoBehaviour Functions

    void Start() {
        line = GetComponent<LineRenderer>();
        line.sharedMaterial = mat;
        origin = transform.parent.GetComponent<Element>();
        col = GetComponent<BoxCollider>();
        SetWidth(width, width);
        SetStartColor();
        transform.localPosition = Vector3.zero;
        screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
        gameObject.layer = 2;
    }

    void LateUpdate() { // We should change it so that links only update when necessary
        SetStartPostion(originPosition);



        gameObject.layer = Mouse.breakLinkMode || Mouse.isHoldingPrism ? 0 : 2;

        if (connected) {
            transform.position = originPosition; // sometimes infinity

            if (!animated) {
                SetEndColor();
                int vertices = (int)(length / segmentLength);
                line.numPositions = vertices;

                for (int currentVertice = 1; currentVertice < vertices; currentVertice++) {
                    float step = (float)currentVertice / (float)vertices;
                    Vector3 pos = Vector3.Lerp(originPosition, targetPosition, step);
                    pos += transform.up * Mathf.Sin(Time.time * waveSpeed + currentVertice * waveLength) * waveHeight * (1 - Mathf.Abs(step - 0.5f)*2);
                    line.SetPosition(currentVertice, pos);
                }
                waveHeight -= Time.deltaTime/waveDuration;
                if (waveHeight <= 0) {
                    animated = true;
                    line.numPositions = 2;
                }
            }
            
            SetEndPosition(targetPosition);
            SetCollider();

            if (adjustWidth) SetWidth(startWidth, endWidth);

        } else {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth));
            SetEndPosition(mouseWorldPos);
        }
    }

    void OnMouseEnter() {
        if (Mouse.holding && Mouse.holding != target && Mouse.holding != origin &&
            Mouse.holding.GetComponent<Prism>()) {
            line.sharedMaterial = hoverMat;
            Mouse.hover = this;
        }

        if (Mouse.breakLinkMode && !destroyed && isVisible) {
            BreakLink();
        }
    }

    void OnMouseExit() {
        line.sharedMaterial = mat;
        if (Mouse.hover == this)
            Mouse.hover = null;
    }

    #endregion

    #region Properties Setter Functions

    void SetCollider() {
        transform.LookAt(targetPosition); // sometimes infinity
        col.center = Vector3.forward * (length / 2); // sometimes infinity
        col.size = new Vector3(width, width, length);
    }

    void SetStartPostion(Vector3 pos) {
        line.SetPosition(0, pos);
    }

    void SetEndPosition(Vector3 pos) {
        line.SetPosition(line.numPositions - 1, pos);
    }

    void SetWidth(float start, float end) {
        line.startWidth = start;
        line.endWidth = end;
    }

    void SetParticleSystem(ParticleSystem ps) {
        ps.transform.localEulerAngles = 90 * Vector3.up;
        ps.transform.localPosition = Vector3.forward * (length / 2);

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 70 * length;

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.radius = length / 2;

        ParticleSystem.MainModule main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Custom;
        main.customSimulationSpace = origin.worldInstance.transform;
    }

    public void SetStartColor() {
        line.startColor = origin.chroma.color;
    }

    public void SetEndColor() {
        line.endColor = target.chroma.color;
    }

    #endregion

    #region Destroy Functions

    public void BreakLink() {
        destroyed = true;

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        SetParticleSystem(ps);
        ps.Play();

        line.enabled = false;
        foreach (Prism prism in prismToOrigin)
            prism.attachedLink = null;
        foreach (Prism prism in prismToTarget)
            prism.attachedLink = null;
        origin.links.Remove(this);
        target.targeted.Remove(this);
        Invoke("DestroyLink", ps.main.startLifetime.constant);
    }

    void DestroyLink() {
        // We should also remove the loops containing this link
        Destroy(gameObject); // We should probably do object pooling for links
    }

    #endregion

    /// <summary>
    /// Returns the position of a point given the loop it is in and the current world loop.
    /// </summary>
    /// <param name="point"> The world position of the desired point. </param>
    /// <param name="loop"> The loop of the desired point. </param>
    /// <param name="worldLoop"> The loop of the world instance containing the desired point. </param>
    /// <returns> The actual world position the point is at. </returns>
    Vector3 GetMetaPosition(Vector3 point, ref MetaPosition pos, int loop, int worldLoop) {
        if (worldLoop > loop) {
            pos = MetaPosition.External;
            return point.normalized * 1000; // Float approximation, not the best method
        } else if (worldLoop < loop) {
            pos = MetaPosition.Internal;
            return Vector3.zero;
        } else {
            pos = MetaPosition.InRange;
            return point;
        }
    }
}
