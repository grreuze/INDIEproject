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
    public Element parent;
    public Element target;
    public int targetLoop;
    public int originLoop;
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
        get { return (parentMetaPos != targetMetaPos) || (parentMetaPos == targetMetaPos && targetMetaPos == MetaPosition.InRange); }
    }

    /// <summary>
    /// The length of the link. Distance between originPosition and targetPosition.
    /// </summary>
    public float length {
        get { return Vector3.Distance(originPosition, targetPosition) / transform.lossyScale.x; }
    }

    public List<Prism> prismToOrigin;
    public List<Prism> prismToTarget;

    enum MetaPosition { InRange, External, Internal }
    MetaPosition parentMetaPos, targetMetaPos;
    LineRenderer line;
    BoxCollider col;
    bool destroyed, animated;

    float startWidth {
        get {
            return parentMetaPos == MetaPosition.Internal ? 0 : parent.transform.lossyScale.x * width * transform.localScale.x;
        }
    }

    float endWidth {
        get {
            return targetMetaPos == MetaPosition.Internal ? 0 : target.transform.lossyScale.x * width * transform.localScale.x;
        }
    }

    WorldInstance worldInstance {
        get { return parent.worldInstance; }
    }

    Vector3 originPosition {
        get {
            int worldLoop = parent.isHeld ? WorldWrapper.singleton.currentInstance.loop : worldInstance.loop;
            return GetMetaPosition(parent.transform.position, ref parentMetaPos, originLoop, worldLoop);
        }
    }

    Vector3 targetPosition {
        get {
            int worldLoop = target.isHeld ? WorldWrapper.singleton.currentInstance.loop : target.worldInstance.loop;
            return GetMetaPosition(target.transform.position, ref targetMetaPos, targetLoop, worldLoop);
        }
    }

    float screenDepth;

    #endregion

    #region MonoBehaviour Functions

    void Start() {
        line = GetComponent<LineRenderer>();
        line.sharedMaterial = mat;
        parent = transform.parent.GetComponent<Element>();
        col = GetComponent<BoxCollider>();
        SetWidth(width, width);
        transform.localPosition = Vector3.zero;
        screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
    }

    void LateUpdate() { // We should change it so that links only update when necessary
        SetOriginPosition(originPosition);

        if (connected) {
            transform.position = originPosition; // sometimes infinity

            if (!animated) {
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
            
            SetTargetPosition(targetPosition);
            SetCollider();

            if (adjustWidth) SetWidth(startWidth, endWidth);

        } else {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth));
            SetTargetPosition(mouseWorldPos);
        }
    }

    void OnMouseEnter() {
        if (Mouse.holding && Mouse.holding != target && Mouse.holding != parent &&
            Mouse.holding.GetComponent<Prism>()) {
            line.sharedMaterial = hoverMat;
            Mouse.hover = this;
        }

        if (Mouse.breakLinkMode && !destroyed && isVisible) {
            destroyed = true;
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            ps.transform.localEulerAngles = 90 * Vector3.up;
            ps.transform.localPosition = Vector3.forward * (length / 2);
            
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = 70 * length;

            ParticleSystem.ShapeModule shape = ps.shape;
            shape.radius = length / 2;

            ps.Play();
            line.enabled = false;
            Invoke("DestroyLink", ps.main.startLifetime.constant);
        }
    }

    void OnMouseExit() {
        line.sharedMaterial = mat;
        if (Mouse.hover == this)
            Mouse.hover = null;
    }

    #endregion

    void SetCollider() {
        transform.LookAt(targetPosition); // sometimes infinity
        col.center = Vector3.forward * (length / 2); // sometimes infinity
        col.size = new Vector3(width, width, length);
    }

    void SetOriginPosition(Vector3 pos) {
        line.SetPosition(0, pos);
    }

    void SetTargetPosition(Vector3 pos) {
        line.SetPosition(line.numPositions - 1, pos);
    }

    void SetWidth(float start, float end) {
        line.startWidth = start;
        line.endWidth = end;
    }

    public void DestroyLink() {
        parent.links.Remove(this);
        target.targeted.Remove(this);
        // We should also remove the loops containing this link
        Destroy(gameObject); // We should probably do object pooling for links
    }

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
