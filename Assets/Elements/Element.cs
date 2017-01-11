using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class Element : MonoBehaviour {

    #region Public Properties

    [Header("Element Properties")]

    public WorldInstance worldInstance;
    public Existence existence;

    public Chroma chroma;
	public Color outlineColorWhenChromaIsWhite;
	public Color hoverColor; 

    public List<Link> links = new List<Link>();
    public List<Link> targeted = new List<Link>();
    
    public int id;
    
	public Material mat;

    public bool isActive {
        get { return rend.enabled; }
    }
    
    public List<int> substractedFrom = new List<int>();
    
    [Header("Scale"), SerializeField]
    Vector3 defaultScale = Vector3.one;
    [SerializeField]
    float scaleFactor = 0.2f;
    [SerializeField]
    MinMax scale = new MinMax(0, 0.8f);
    
    /// <summary>
    /// Returns whether or not the player is holding this element.
    /// </summary>
    public bool isHeld {
        get { return hovered && Input.GetMouseButton(0); }
    }

    public bool anchored;
    public int anchorInstanceOffset;
    
    public float vertexOffset {
        get { return rend.material.GetFloat("_VertexOffset"); }
        set { rend.material.SetFloat("_VertexOffset", value); }
    }
    #endregion

    #region Private Properties

    bool hovered;

    static WorldWrapper wrapper;
    Transform worldTransform;
    public AudioSource MySound;
    SoundManager soundManager;

    /// <summary>
    /// If the element is unique, the loop in which it exists.
    /// </summary>
    int existenceLoop;

    Collider col;
    Renderer rend;

    Star[] clones;
    
    Color outlineColor {
        get {
            return rend.material.GetColor("_Outline_Color");
        }
        set {;
            rend.material.SetColor("_Outline_Color", value);
        }
    }

    #endregion

    #region MonoBehaviours Methods

    void Start() {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        MySound = GetComponent<AudioSource>();
        soundManager = SoundManager.singleton;
        rend.sharedMaterial = mat;
        outlineColor = chroma.color == Color.white ? outlineColorWhenChromaIsWhite : chroma.color;
        
        if (!wrapper) wrapper = WorldWrapper.singleton;
        GetWorldInstance();
        if (existence == Existence.unique)
            existenceLoop = worldInstance.loop;
        else GetAllClones();
    }

    void Update() {
        CheckHeld();
        if (!isHeld) Rescale(); // We should only rescale if zooming / dezooming / anchored
        CheckLink();

        if (anchored) UpdateLinks();
        
        if (existence != Existence.unique && orbiting && walker) // if we're the original orbiting
            MoveClones();

        if (existence == Existence.unique) {
            SetActive(existenceLoop == worldInstance.loop || isHeld);
            if (trail) trail.enabled = existenceLoop == worldInstance.loop;
        }
        else
        if (existence == Existence.substracted)
            SetActive(!substractedFrom.Contains(worldInstance.loop) || isHeld);
    }

    void OnMouseEnter() {
        if (!isHeld && !Input.GetMouseButton(0) && Mouse.holding == null) {
            hovered = true;
            Mouse.hover = this;
			rend.material.SetColor("_Color", hoverColor);
			rend.material.SetFloat ("_Atmospheric_Opacity_or_Opaque", 1);
        }
    }
    
    void OnMouseExit() {
        if (!isHeld) StopHover();
    }

    #endregion

    #region Render Methods

    void SetActive(bool value) {
        col.enabled = value;
        rend.enabled = value;
    }

    public void ApplyChroma() {
        Recolor();
        //if (GetComponent<Star>()) Debug.Log(chroma);
        if (existence != Existence.unique) RecolorClones();
    }

    void Recolor() {
        chroma.ReBalance();
        rend.sharedMaterial = mat;
        outlineColor = chroma.color == Color.white ? outlineColorWhenChromaIsWhite : chroma.color;
        RecolorLinks();
        if (orbiting) RecolorTrail();
    }

    void RecolorLinks() {
        foreach (Link link in links) {// Links I am the origin of
            if (link == null) continue;
            if (link.prismToTarget.Count > 0) link.prismToTarget[0].UpdateTargetColor();
            link.SetStartColor();
        }
        foreach (Link link in targeted) {// Links I am the target of
            if (link == null) continue;
            if (link.prismToOrigin.Count > 0) link.prismToOrigin[0].UpdateTargetColor();
            link.SetEndColor();
        }
    }

    void RecolorTrail() {
        trail.startColor = trail.endColor = chroma.color;
    }

    public void VertexPing() {
        StartCoroutine(_VertexPing());
        PlayMySound();
    }
    
    IEnumerator _VertexPing() {
        float duration = 0.2f;
        float max = 1;
        
        for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime) {
            float t = elapsed / duration;
            vertexOffset = Mathf.Lerp(max, 0, t);
            yield return null;
        }
        vertexOffset = 0;
    }

    #endregion

    #region Holding Methods

    void CheckHeld() {
        if (isHeld) {
            if (Mouse.holding != this) StartHold();
            MoveToMousePosition();
        } else if (Mouse.holding == this) {
            StopHold();
        }
    }

    void StopHover() {
        hovered = false;
        Mouse.hover = null;
        Recolor();
    }

    public virtual void StartHold() {
        if (orbiting) {
            StopOrbit();
            if (existence != Existence.unique)
                StopOrbitClones();
        }

        Mouse.holding = this;
        transform.parent = null;
        gameObject.layer = 2;
        if (existence != Existence.unique) AnchorClones();
    }

    public virtual void MoveToMousePosition() {
        if (existence != Existence.unique) MoveClones();
        float screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth));
        UpdateLinks();
    }

    public virtual void StopHold() {
        StopHover();
        Mouse.holding = null;
        ChangeInstance();
        if (existence != Existence.unique) ReleaseClones();
        gameObject.layer = 0;
    }

    #endregion

    #region WorldInstance Methods

    /// <summary>
    /// Gets the parent World Instance
    /// </summary>
    void GetWorldInstance() {
        worldTransform = transform.parent;
        worldInstance = worldTransform.GetComponent<WorldInstance>();
        if (existence == Existence.unique)
            existenceLoop = worldInstance.loop;
        else
            worldInstance.stars[id] = GetComponent<Star>() ?? worldInstance.stars[id];
    }

    /// <summary>
    /// Sets the element's instance to the one at the specified relative distance.
    /// </summary>
    /// <param name="diff"> The difference between the current instance and the desired one. </param>
    void SetNewInstance(int diff) {
        int newid = InverseInstanceID(diff);
        transform.localScale = Vector3.zero;
        transform.parent = wrapper.worldInstances[newid].transform;
        GetWorldInstance();
        UpdateLinks();
    }
    
    void ChangeInstance() {
        if (existence == Existence.unique)
            existenceLoop = wrapper.currentInstance.loop;
        int diff = wrapper.currentInstance.id - worldInstance.id;
        if (diff == 0)
            transform.parent = worldTransform;
        else {
            SetNewInstance(diff);
            if (existence != Existence.unique) {
                SetNewCloneInstance(diff);
                if (anchorInstanceOffset == 0)
                    MoveClones();
            }
        }
    }

    /// <summary>
    /// Returns the ID of the World Instance set at the specified relative distance.
    /// </summary>
    /// <param name="diff"> The difference between the current instance ID and the desired one. </param>
    /// <returns> The ID of the desired World Instance. </returns>
    int InverseInstanceID(int diff) {
        int instances = wrapper.worldInstances.Count;
        int newid = worldInstance.id + diff;
        if (newid < 0) newid += instances;
        else if (newid > instances - 1)
            newid -= instances;
        return newid;
    }

    #endregion

    #region Link Methods
    
    void CheckLink() {
        if (hovered && !isHeld) {
            if (Input.GetMouseButtonDown(1)) {
                CreateLink(this);
            }
            if (Input.GetMouseButtonUp(1)) {
                if (Mouse.linking && Mouse.linking != this && !Mouse.linking.IsLinkedTo(this)) {
                    if (Mouse.linking.GetType() == GetType()) {
                        if (existence != Existence.unique && Mouse.link.origin.existence != Existence.unique) {
                            int instanceDifference = Mouse.link.origin.worldInstance.id - worldInstance.id;
                            int loopDifference = Mouse.link.origin.worldInstance.loop - Mouse.link.originLoop; // number of times we've crossed boundaries
                            Mouse.link.origin.LinkClones(id, instanceDifference, loopDifference);
                        }
                        Mouse.link.Connect(this);
                    } else { // connecting a Star to a Prism, ORBIT
                        Link link = Mouse.link;
                        link.Connect(this);
                        Prism prism = GetComponent<Prism>() ?? link.origin.GetComponent<Prism>();
                        Star star = GetComponent<Star>() ?? link.origin.GetComponent<Star>();
                        
                        star.chroma += prism.chroma;
                        star.ApplyChroma();

                        star.Orbit(prism, link);
                    }
                } else {
                    Mouse.BreakLink();
                }
            }
        }
    }
    
    bool IsLinkedTo(Element element) {
        foreach (Link link in links)
            if (link.target == element) return true;
        foreach (Link link in targeted)
            if (link.origin == element) return true;
        return false;
    }

    void CreateLink(Element origin) {
        Mouse.linking = origin;
        Mouse.link = Instantiate(PrefabManager.link);
        Mouse.link.transform.parent = transform;
        Mouse.link.transform.position = transform.position;
        Mouse.link.originLoop = worldInstance.loop;
        Mouse.link.connected = false;
    }
    
    void LinkTo(Element target, int targetLoop) {
        Link newLink = Instantiate(PrefabManager.link);

        newLink.transform.parent = transform;
        newLink.origin = this;
        newLink.transform.position = transform.position;
        newLink.originLoop = worldInstance.loop;
        links.Add(newLink);

        newLink.Connect(target, true);
        newLink.targetLoop = targetLoop;
    }

    /// <summary>
    /// Automatically links this element to another using relative positions.
    /// </summary>
    /// <param name="targetID"> The ID of the element we're linking to </param>
    /// <param name="instanceDiff"> The number of instances between this element and the targeted element </param>
    /// <param name="loopDiff"> The number of loops between this element and the targeted element </param>
    public void AutoLink(int targetID, int instanceDiff, int loopDiff) {

        int numberOfInstances = wrapper.numberOfInstances;
        int maxInstance = numberOfInstances - 1;

        int diff = -instanceDiff; // Check if we're crossing inner or outer bounds
        if (diff > (maxInstance - worldInstance.id) || diff < (-worldInstance.id)) 
            loopDiff -= (diff / maxInstance) + (int)Mathf.Sign(diff);

        int targetInstanceID = worldInstance.id - instanceDiff;
        if (targetInstanceID >= numberOfInstances) targetInstanceID -= numberOfInstances;
        else if (targetInstanceID < 0) targetInstanceID += numberOfInstances;

        WorldInstance instance = wrapper.worldInstances[targetInstanceID];
        int targetLoop = worldInstance.loop + loopDiff - (worldInstance.loop - instance.loop);
        Element target = instance.stars[targetID];
        LinkTo(target, targetLoop);
    }

    int loopModifier = 0;
    public void UpdateLinks() {
        if (targeted.Count + links.Count == 0) return;
        int myLoop = wrapper.currentInstance.loop;
        MetaPosition modifier = MetaPosition.InRange;

        if (anchored) {
            int numberOfInstances = wrapper.numberOfInstances;
            int instanceID = worldInstance.id;
            
            if (Mouse.holding) {
                Mouse.holding.AnchorClones(); // ReAnchor clones just to be sure we get the right anchorInstanceOffset
                instanceID = wrapper.currentInstance.id - anchorInstanceOffset;
            } else {
                print("Stopped Holding.");
                loopModifier = 0;
            }

            if (instanceID >= numberOfInstances) {
                loopModifier = -(instanceID / numberOfInstances); // outer bounds
                modifier = MetaPosition.External;
                if (!name.EndsWith(" outer")) { //Debug
                    print("Outer modifier: " + loopModifier);
                    name += " outer";
                }
                instanceID -= numberOfInstances;
            } else if (instanceID < 0) {
                loopModifier = (-instanceID / numberOfInstances) + 1; // inner bounds
                modifier = MetaPosition.Internal;
                if (!name.EndsWith(" inner")) { //Debug
                    print("Inner modifier: " + loopModifier);
                    name += " inner";
                }
                instanceID += numberOfInstances;
            } else if (loopModifier != 0) {
                loopModifier = -loopModifier;
                modifier = MetaPosition.InRange;
                if (!name.EndsWith(" back")) { //Debug
                    print("Reverse modifier: " + loopModifier + " myLoop is " + wrapper.worldInstances[instanceID].loop + loopModifier);
                    name += " back";
                }
            }

            myLoop = wrapper.worldInstances[instanceID].loop + loopModifier;
        } else loopModifier = 0;

        if (links.Count > 0) UpdateOriginPoints(myLoop, loopModifier, modifier);
        if (targeted.Count > 0) UpdateTargetPoints(myLoop, loopModifier, modifier);

        if (loopModifier != 0 && modifier == MetaPosition.InRange) // If we reversed the previous modifier, get back to 0
            loopModifier = 0; // Prevents reverting reverse modifiers forever
    }

    void UpdateOriginPoints(int myLoop, int loopDiff, MetaPosition metaPos) {
        foreach (Link link in links) {
            if (link.isVisible && (link.origin && myLoop != link.originLoop || loopDiff != 0)) {
                if (loopDiff != 0) link.originMetaPos = metaPos;
                link.originLoop = myLoop;
            }
        }
    }

    void UpdateTargetPoints(int myLoop, int loopDiff, MetaPosition metaPos) {
        foreach (Link link in targeted) {
            if (link.isVisible && (link.target && myLoop != link.targetLoop || loopDiff != 0)) {
                if (loopDiff != 0) link.targetMetaPos = metaPos;
                link.targetLoop = myLoop;
            }
        }
    }

    public void DestroyAllLinks() {
        while (links.Count > 0)
            links[0].BreakLink();
        while (targeted.Count > 0)
            targeted[0].BreakLink();
    }

    #endregion

    #region Scale Methods

    /// <summary>
    /// Rescales an element depending on its parent's scale.
    /// </summary>
    void Rescale() {
        Vector3 parentScale = GetParentScale();
        Vector3 localScale = defaultScale * Vector3.Distance(Vector3.zero, transform.position) * scaleFactor;
        localScale.x *= parentScale.x;
        localScale.y *= parentScale.y;
        localScale.z *= parentScale.z;

        localScale.x = Mathf.Clamp(localScale.x, scale.min * parentScale.x, scale.max * parentScale.x);
        localScale.y = Mathf.Clamp(localScale.y, scale.min * parentScale.y, scale.max * parentScale.y);
        localScale.z = Mathf.Clamp(localScale.z, scale.min * parentScale.z, scale.max * parentScale.z);

        transform.localScale = localScale; //sometimes infinity
        FixPosition();
    }

    void FixPosition() { // Ugly fix
        if (Mathf.Abs(transform.localPosition.x) > 1000)
            transform.localPosition /= 10000;
        if (Mathf.Abs(transform.localPosition.x) < 0.001)
            transform.localPosition *= 10000;
    }

    Vector3 GetParentScale() {
        Vector3 sf = Vector3.one;
        Transform parent = transform.parent ?? null;

        while (parent) {
            sf.x *= 1 / parent.localScale.x;
            sf.y *= 1 / parent.localScale.y;
            sf.z *= 1 / parent.localScale.z;

            parent = parent.parent ?? null;
        }
        return sf;
    }

    #endregion

    #region CloneMethods

    void GetAllClones() {
        clones = new Star[wrapper.worldInstances.Count - 1];

        for (int i = 0, j = 0; i < clones.Length + 1; i++, j++) {
            if (i == worldInstance.id) {
                j--;
                continue;
            }
            Star clone = wrapper.worldInstances[i].stars[id];
            clones[j] = clone;
        }
    }

    void AnchorClones() {
        if (existence == Existence.unique) return;
        anchored = false;
        anchorInstanceOffset = 0;
        foreach (Star clone in clones) {
            clone.anchored = true;
            clone.anchorInstanceOffset = worldInstance.id - clone.worldInstance.id;
        }
    }

    void MoveClones() {
        foreach (Star clone in clones)
            clone.transform.localPosition = worldTransform.InverseTransformPoint(transform.position);
    }

    void ReleaseClones() {
        foreach (Star clone in clones)
            clone.anchored = false;
    }

    void LinkClones(int targetID, int instanceDiff, int loopDiff) {
        foreach(Star clone in clones) 
            clone.AutoLink(targetID, instanceDiff, loopDiff);
    }

    public void DestroyCloneLink(int targetID) {
        foreach(Star clone in clones) {
            foreach(Link link in clone.links) {
                if (link.target && link.target.id == targetID) {
                    link.BreakLink(false);
                    break;
                }
            }
        }
    }

    void RecolorClones() {
        foreach (Star clone in clones) {
            clone.chroma = chroma;
            clone.Recolor();
        }
    }

    void SetNewCloneInstance(int diff) {
        foreach (Star clone in clones)
            clone.SetNewInstance(diff);
    }

    void OrbitClones() {
        foreach (Star clone in clones) {
            clone.orbiting = true;
            clone.AddLocalTrail();
        }
    }

    void StopOrbitClones(bool breakClones = true) {
        foreach(Star clone in clones)
            clone.StopOrbit(breakClones);
    }

    #endregion

    #region Sound Methods

    public void PlayMySound()
    {
        float volume = 1f;
        int gamme = 5; // can't be < 3 or > 8   Higher = High frequency
        if(chroma.r == chroma.b && chroma.b == chroma.g) // white
        {
            soundManager.Play(soundManager.starSound[gamme], volume, MySound);
        }

        if (chroma.b > chroma.r && chroma.g > chroma.b) //violet - blue
        {
            soundManager.Play(soundManager.starSound[gamme + 5], volume, MySound);
        } 

        else if (chroma.b > chroma.r && chroma.b > chroma.g) // blue
        {
            soundManager.Play(soundManager.starSound[gamme + 4], volume, MySound);
        }
        else if (chroma.b == chroma.g && chroma.g > chroma.r) // cyan
        {
            soundManager.Play(soundManager.starSound[gamme + 3], volume, MySound);
        }

        else if (chroma.r == chroma.b && chroma.b > chroma.g) // rose
        {
            soundManager.Play(soundManager.starSound[gamme + 2], volume, MySound);
        }

        else if (chroma.r > chroma.b && chroma.b > chroma.g) //magenta
        {
            soundManager.Play(soundManager.starSound[gamme + 1], volume, MySound);
        } 

        //MID (white in logic)

        else if (chroma.r > chroma.b && chroma.r > chroma.g) // red
        {
            soundManager.Play(soundManager.starSound[gamme - 1], volume, MySound);
        }

        if (chroma.r > chroma.g && chroma.g > chroma.b) //orange
        {
            soundManager.Play(soundManager.starSound[gamme - 2], volume, MySound);
        }

        else if (chroma.r == chroma.g && chroma.g > chroma.b) // jaune
        {
            soundManager.Play(soundManager.starSound[gamme - 3], volume, MySound);
        }

        else if (chroma.g > chroma.b && chroma.b > chroma.r) //Jaune - vert
        {
            soundManager.Play(soundManager.starSound[gamme - 4], volume, MySound);
        } 

        else if (chroma.g > chroma.b && chroma.g > chroma.r) // green
        {
            soundManager.Play(soundManager.starSound[gamme - 5], volume, MySound);
        }

    }

    #endregion

    #region Orbit Methods

    bool orbiting;
    BezierSpline spline;
    SplineWalker walker;
    LocalTrailRenderer trail;
    void Orbit(Prism prism, Link link) {
        int diff = worldInstance.id - prism.worldInstance.id;
        if (diff != 0) prism.SetNewInstance(diff);

        if (existence != Existence.unique)
            StopOrbitClones(false);

        DoSpline(prism);
        DoWalker();
        AddLocalTrail();
        
        link.BreakLink();
        Destroy(prism.gameObject);
        
        orbiting = true;
        if (existence != Existence.unique)
            OrbitClones();
    }

    void DoSpline(Prism prism) {
        if (!spline)
            spline = worldInstance.gameObject.AddComponent<BezierSpline>(); // we should put the splines elsewhere
        spline.Reset();
        spline.AddCurve();
        spline.Loop = true;

        spline.points[0] = spline.points[6] = transform.localPosition; // set start and end of loop here

        spline.points[3] = -transform.localPosition; // set opposite point relative to center

        spline.points[5] = prism.transform.localPosition; // set the prism's tangent point
        spline.points[2] = -spline.points[5]; // set the equivalent for the opposite point

        spline.SetControlPointMode(5, BezierControlPointMode.Mirrored); // Set mirrored for start tangent
        spline.SetControlPointMode(2, BezierControlPointMode.Mirrored); // Set mirrored for opposite tangent
    }

    void DoWalker() {
        if (walker)
            Destroy(walker);

        walker = gameObject.AddComponent<SplineWalker>();

        walker.mode = SplineWalkerMode.Loop;
        walker.duration = 5; // time the star takes to orbit
        walker.spline = spline;
    }

    void AddLocalTrail() {
        if (!trail)
            trail = gameObject.AddComponent<LocalTrailRenderer>();
        trail.material = PrefabManager.link.mat;
        trail.startColor = trail.endColor = chroma.color;

        trail.width = PrefabManager.link._width;
        trail.adjustWidth = PrefabManager.link.adjustWidth;
        trail.startWidth = 0;
        trail.endWidth = PrefabManager.link._width;
    }

    void StopOrbit(bool breakTrail = true) {
        orbiting = false;
        if (walker) Destroy(walker);
        if (spline) Destroy(spline);
        if (trail && breakTrail) Destroy(trail);
    }

    #endregion
}

public enum Existence {
    cloned, unique, substracted
}