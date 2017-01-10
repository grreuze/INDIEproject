using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Link : MonoBehaviour {

    #region Properties

    #region Public Properties

    public GameObject stringParticle;

    [Header("Line Renderer Properties"), SerializeField, Tooltip("The start and end width of the line renderer")]
    float _width = 0.5f;
    float width {
        get { return _width / transform.lossyScale.x; }
    }
    [SerializeField]
    float segmentLength = 0.4f;

    public Material mat;
    [SerializeField]
    Material hoverMat;

    [Header("Link Properties")]
    public Element origin;
    public Element target;

    [SerializeField]
    int _originLoop = 0, _targetLoop = 0;

    public int loopDifference;
    int instancesLoopDifference = 0, lastInstancesLoopDifference = 0; // loop difference between visible instances

    public int originLoop {
        get { return repeatable && target.isActive ? originWorldLoop : _originLoop; }
        set {
            if (doneAnimated) {
                instancesLoopDifference = originWorldLoop - targetWorldLoop;
                if (originMetaPos == MetaPosition.Internal) instancesLoopDifference++;
                else if (originMetaPos == MetaPosition.External) instancesLoopDifference--;
                
                if (_originLoop != value || instancesLoopDifference != lastInstancesLoopDifference) {

                    int baseLoop = instancesLoopDifference == 0 && targetMetaPos == MetaPosition.InRange ? originWorldLoop : value - loopDifference;

                    if (instancesLoopDifference != lastInstancesLoopDifference) {
                        name += " target-" + instancesLoopDifference + "+" + lastInstancesLoopDifference;
                        print("Target Loop: " + _targetLoop + " => " + (baseLoop - loopDifference) + " - " + instancesLoopDifference + "+" + lastInstancesLoopDifference);
                    }
                    print("Target Loop changed from origin, from " + _targetLoop + " to " + (baseLoop - loopDifference - instancesLoopDifference + lastInstancesLoopDifference) + " Origin Loop: " + _originLoop + " => " + value);
                    _targetLoop = baseLoop - loopDifference - instancesLoopDifference + lastInstancesLoopDifference;
                }
            }
            _originLoop = value;
            loopDifference = _originLoop - _targetLoop;
            //print("From Origin: Origin Loop is now " + _originLoop + " Target Loop is " + _targetLoop + " Difference is " + loopDifference);
            lastInstancesLoopDifference = instancesLoopDifference;
        }
    }
    public int targetLoop {
        get { return repeatable && target.isActive ? originWorldLoop - loopDifference : _targetLoop; }
        set {
            if (doneAnimated) {
                instancesLoopDifference = originWorldLoop - targetWorldLoop;
                if (targetMetaPos == MetaPosition.External) instancesLoopDifference++;
                else if (targetMetaPos == MetaPosition.Internal) instancesLoopDifference--;
                
                if (_targetLoop != value || instancesLoopDifference != lastInstancesLoopDifference) {

                    int baseLoop = instancesLoopDifference == 0 && targetMetaPos == MetaPosition.InRange ? originWorldLoop : value + loopDifference;

                    if (instancesLoopDifference != lastInstancesLoopDifference) {
                        name += " origin+" + instancesLoopDifference + "-" + lastInstancesLoopDifference;
                        print("Origin Loop: " + _originLoop + " => " + baseLoop + " + " + instancesLoopDifference + "-" + lastInstancesLoopDifference);
                    }
                    print("Origin Loop changed from target, from " + _originLoop + " to " + (baseLoop + instancesLoopDifference - lastInstancesLoopDifference) + " Target Loop: " + _targetLoop + " => " + value);
                    _originLoop = baseLoop + instancesLoopDifference - lastInstancesLoopDifference;
                }
            }

            _targetLoop = value;
            loopDifference = _originLoop - _targetLoop;
            //print("From Target: Origin Loop is now " + _originLoop + " Target Loop is " + _targetLoop + " Difference is " + loopDifference);
            lastInstancesLoopDifference = instancesLoopDifference;
        }
    }

    public List<Prism> prismToOrigin;
    public List<Prism> prismToTarget;
    public bool connected;
    public Existence existence;
    public MetaPosition originMetaPos, targetMetaPos;
    [SerializeField]
    bool adjustWidth;

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
        get { return Mathf.Min(Vector3.Distance(originPosition, targetPosition) / transform.lossyScale.x, 10000); }
    }

    #endregion

    #region Animation Properties

    [Header("Animation"), SerializeField]
    float waveHeight = 1.5f;
    [SerializeField]
    float waveLength = 0.3f, waveDuration = 0.2f, waveSpeed = 60;
    bool doneAnimated;

    #endregion

    #region Private Properties

    LineRenderer line;
    BoxCollider col;
    bool destroyed, repeatable;

    float startWidth {
        get {
            return originMetaPos == MetaPosition.Internal ? 0 : _width * origin.transform.lossyScale.x;
        }
    }
    float endWidth {
        get {
            return targetMetaPos == MetaPosition.Internal ? 0 : _width * target.transform.lossyScale.x;
        }
    }

    WorldWrapper wrapper;
    WorldInstance worldInstance {
        get { return origin.worldInstance; }
    }

    int originWorldLoop {
        get { return GetWorldLoop(origin); }
    }
    int targetWorldLoop {
        get { return GetWorldLoop(target); }
    }

    int GetWorldLoop(Element element) {
        WorldInstance instance = element.worldInstance;
        if (element.isHeld)
            instance = wrapper.currentInstance;
        else if (element.anchored) {
            int instanceID = wrapper.currentInstance.id - element.anchorInstanceOffset;
            if (instanceID >= wrapper.numberOfInstances) instanceID -= wrapper.numberOfInstances;
            else if (instanceID < 0) instanceID += wrapper.numberOfInstances;
            instance = wrapper.worldInstances[instanceID];
        }
        return instance.loop;
    }

    Vector3 originPosition {
        get {
            return GetMetaPosition(origin.transform.position, ref originMetaPos, originLoop, originWorldLoop);
        }
    }
    Vector3 targetPosition {
        get {
            return GetMetaPosition(target.transform.position, ref targetMetaPos, targetLoop, targetWorldLoop);
        }
    }

    float screenDepth;

    ParticleSystem stringParticlePs;
    float cursorSpeedF {
        get { return cursorSpeed.speed; }
    }

    protected AudioSource MySound;
    SoundManager soundManager;

    CursorSpeedScript cursorSpeed;

    /// <summary>
    /// Whether or not this link has collisions activated. If false, sets itself to 'Ignore Raycast' layer.
    /// </summary>
    bool collisions {
        set {
            gameObject.layer = value ? 0 : 2;
            if (!value) col.size = Vector3.one * 0.1f;
        } get {
            return gameObject.layer == 0;
        }
    }

    #endregion

    #endregion

    #region MonoBehaviour Functions

    void Start() {
        wrapper = WorldWrapper.singleton;
        line = GetComponent<LineRenderer>();
        line.sharedMaterial = mat;
        origin = transform.parent.GetComponent<Element>();
        col = GetComponent<BoxCollider>();
        if (adjustWidth) SetWidth(startWidth, startWidth);
        else SetWidth(width, width);
        SetStartColor();
        transform.localPosition = Vector3.zero;
        screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
        gameObject.layer = 2;
        cursorSpeed = GameObject.Find("CursorSpeed").GetComponent<CursorSpeedScript>();
        MySound = GetComponent<AudioSource>();
        soundManager = SoundManager.singleton;
    }

    void LateUpdate() { // We should change it so that links only update when necessary
        if (destroyed) return;

        SetStartPostion(originPosition);
        
        collisions = Mouse.breakLinkMode || Mouse.isHoldingPrism; // Only have collisions if we're breaking links or holding a prism
        
        if (connected) {
            transform.position = originPosition; // sometimes infinity
            if (!doneAnimated) Animate();
            SetEndPosition(targetPosition);
            if (collisions) SetCollider(); // Only set collider if we have collisions
            if (adjustWidth) SetWidth(startWidth, endWidth);

        } else {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth));
            SetEndPosition(mouseWorldPos);
        }
    }

    void OnMouseEnter() {

        if (Mouse.holding != null && Mouse.holding != origin && Mouse.holding != target && waveHeight <= 0.1f)
            PlayMusic();

        if (Mouse.holding && Mouse.holding != target && Mouse.holding != origin &&
            Mouse.holding.GetComponent<Prism>()) {
            line.sharedMaterial = hoverMat;
            Mouse.hover = this;
        }

        if (Mouse.breakLinkMode && !destroyed && isVisible)
            BreakLink();
    }

    void OnMouseExit() {

        line.sharedMaterial = mat;
        if (Mouse.hover == this)
            Mouse.hover = null;
    }


    #endregion

    #region Properties Setter Functions

    public void Connect(Element target, bool isAclone = false) {
        this.target = target;
        targetLoop = target.worldInstance.loop;
        connected = true;
        target.targeted.Add(this);
        origin.links.Add(this);
        if (Mouse.link == this) Mouse.link = null;
        if (Mouse.linking == origin) Mouse.linking = null;
        if(origin.GetComponent<Prism>() && target.GetComponent<Prism>())
           CircuitManager.instance.CheckCircuit(target);
        instancesLoopDifference = originWorldLoop - targetWorldLoop;
        SetExistence();
    }

    void SetExistence() {
        Existence start = origin.existence;
        Existence end = target.existence;

        if (start == Existence.unique || end == Existence.unique) {
            existence = Existence.unique;
        } else {
            if (start == Existence.cloned && end == Existence.cloned) {
                existence = Existence.cloned;
            } else if (start == Existence.substracted || end == Existence.substracted) {
                existence = Existence.substracted;
            }
            repeatable = true;
            loopDifference = originLoop - targetLoop;
        }
    }

    void SetCollider() {
        transform.LookAt(targetPosition); // sometimes infinity
        col.center = Vector3.forward * (length / 2); // sometimes infinity
        if (originPosition.z >= 25 && targetPosition.z >= 25) {//not possible to strike the links which are too far away
            col.size = new Vector3(0, 0, length);
        }
        else if (cursorSpeedF <= 1f) {
            col.size = new Vector3(width, width, length);
        }
        else {
            //float colWidth = width + (0.1f * Mathf.Sqrt(Mathf.Max(0, cursorSpeedF - 1f)));
            float colWidth = Mathf.Clamp( Mathf.Sqrt(cursorSpeedF), width, width * 3 );
            col.size = new Vector3(colWidth, colWidth, length);
        }
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

        ParticleSystem.ColorOverLifetimeModule lifetimeColor = ps.colorOverLifetime;
        Gradient psGrad = new Gradient();
        psGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(GetComponent<LineRenderer>().startColor, 0.0f), new GradientColorKey(GetComponent<LineRenderer>().endColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
        lifetimeColor.color = psGrad;
    }

    public void SetStartColor() {
        line.startColor = origin.chroma.color;
    }

    public void SetEndColor() {
        line.endColor = target.chroma.color;
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

    #endregion

    #region Miscellanous Functions

    void Animate() {
        SetEndColor();
        int vertices = Mathf.Max(2, (int)(length / segmentLength));
        line.numPositions = vertices;

        for (int currentVertice = 1; currentVertice < vertices; currentVertice++) {
            float step = (float)currentVertice / (float)vertices;
            Vector3 pos = Vector3.Lerp(originPosition, targetPosition, step);
            pos += transform.up * Mathf.Sin(Time.time * waveSpeed + currentVertice * waveLength) * waveHeight * (1 - Mathf.Abs(step - 0.5f) * 2);
            line.SetPosition(currentVertice, pos);
        }
        waveHeight -= Time.deltaTime / waveDuration;
        if (waveHeight <= 0) {
            doneAnimated = true;
            line.numPositions = 2;
        }
    }

    //Links as musical strings - works only when holding a prism
    void PlayMusic() {
        //Particles
        stringParticlePs = stringParticle.GetComponent<ParticleSystem>();
        var stringParticlePsMain = stringParticlePs.main;
        var stringParticlePsColor = stringParticlePs.colorOverLifetime;
        stringParticlePsMain.startSpeed = 3f * cursorSpeedF;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(GetComponent<LineRenderer>().startColor, 0.0f), new GradientColorKey(GetComponent<LineRenderer>().endColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
        stringParticlePsColor.color = grad;
        Instantiate(stringParticle, Mouse.holding.GetComponent<Transform>().position, Quaternion.identity);

        //wave
        doneAnimated = false;
        waveHeight = (cursorSpeedF / 5f) + 0.5f;
        waveHeight = Mathf.Clamp(waveHeight, 0f, 3f);
        waveLength = (1 / 70f) * length;
        waveSpeed = 10f + (cursorSpeedF * 2f);
        waveDuration = 0.5f;

        //Sound
        //Sound intensity should change according to the speed at which the string is struck
        soundManager.Play(soundManager.stringSound, Mathf.Clamp(0.2f + (cursorSpeedF / 10f), 0f, 2f), MySound);
        //Sound should be high or low depending on the string's size
        MySound.pitch = 1 / (length * 0.1f);
        MySound.pitch = Mathf.Clamp(MySound.pitch, 0.3f, 1.7f);
        //The color of the string (link) should also impact the sound (change in "instrument")
    }

    #endregion

    #region Destroy Functions

    public void BreakLink(bool destroyClones = true) {
        destroyed = true;

        origin.links.Remove(this);
        target.targeted.Remove(this);
        
        if (repeatable && destroyClones)
            origin.DestroyCloneLink(target.id);
        
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>() ?? null;
        SetParticleSystem(ps);
        ps.Play();

        line.enabled = false;
        target = null;
        origin = null;
        foreach (Prism prism in prismToOrigin)
            prism.attachedLink = null;
        foreach (Prism prism in prismToTarget)
            prism.attachedLink = null;
        Invoke("DestroyLink", ps.main.startLifetime.constant); //Destroy the link when the Particle System is over
    }

    void DestroyLink() {
        // We should also remove the loops containing this link
        Destroy(gameObject); // We should probably do object pooling for links
    }

    #endregion

}

public enum MetaPosition { InRange, External, Internal }