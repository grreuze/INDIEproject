using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Element : MonoBehaviour {

    #region Public Properties

    [Header("Element Properties")]

    public WorldInstance worldInstance;
    public Existence existence;

    public Chroma chroma;

    public List<Link> links = new List<Link>();
    public List<Link> targeted = new List<Link>();

    public int id;

    public Material mat, hoverMat;

    public bool isActive {
        get { return rend.enabled; }
    }

    #endregion

    #region Private Properties

    bool hovered;
    /// <summary>
    /// Returns whether or not the player is holding this element.
    /// </summary>
    bool isHeld {
        get { return hovered && Input.GetMouseButton(0); }
    }

    static WorldWrapper wrapper;
    Transform worldTransform;

    [Header("Scale"), SerializeField]
    Vector3 defaultScale = Vector3.one;
    [SerializeField]
    float scaleFactor = 0.2f;
    [SerializeField]
    MinMax scale = new MinMax(0, 0.8f);

    int loop;
    List<int> substractedFrom = new List<int>();

    Collider col;
    Renderer rend;

    Star[] clones;

    #endregion

    #region MonoBehaviours Methods

    void Start() {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = mat;

        rend.material.SetColor("_EmissionColor", chroma.color);

        if (!wrapper) wrapper = WorldWrapper.singleton;
        GetWorldInstance();
        if (existence == Existence.cloned)
            GetAllClones();
        if (existence == Existence.unique)
            loop = worldInstance.loop;
    }

    void Update() {
        Rescale(); // We should only rescale if zooming / dezooming / anchored
        CheckHeld();
        CheckLink();

        //Temporary breaking method
        if (hovered && GetComponent<Star>() && Input.GetMouseButtonDown(2)) {
            DestroyAllLinks();

            if (existence == Existence.cloned)
                existence = Existence.substracted;
            else Destroy(gameObject);

            substractedFrom.Add(worldInstance.loop);

            if (chroma.isPure) chroma *= Chroma.MAX; //If only one color, give 3 prisms instead of one

            for (int i = 0; i < chroma.r; i++) {
                Prism redPrism = (Prism)Instantiate(PrefabManager.prism, 
                    transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), 
                    Quaternion.identity);
                redPrism.transform.parent = wrapper.currentInstance.transform;
                redPrism.chroma = Chroma.red;
                redPrism.transform.LookAt(transform);
            }
            for (int i = 0; i < chroma.g; i++) {
                Prism greenPrism = (Prism)Instantiate(PrefabManager.prism,
                    transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)),
                    Quaternion.identity);
                greenPrism.transform.parent = wrapper.currentInstance.transform;
                greenPrism.chroma = Chroma.green;
                greenPrism.transform.LookAt(transform);
            }
            for (int i = 0; i < chroma.b; i++) {
                Prism bluePrism = (Prism)Instantiate(PrefabManager.prism,
                    transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)),
                    Quaternion.identity);
                bluePrism.transform.parent = wrapper.currentInstance.transform;
                bluePrism.chroma = Chroma.blue;
                bluePrism.transform.LookAt(transform);
            }
        }


        if (existence == Existence.unique)
            SetActive(loop == worldInstance.loop);
        else
        if (existence == Existence.substracted)
            SetActive(!substractedFrom.Contains(worldInstance.loop));
    }

    void OnMouseEnter() {
        if (!isHeld && !Input.GetMouseButton(0) && Mouse.holding == null) {
            hovered = true;
            Mouse.hover = this;
            rend.sharedMaterial = hoverMat;
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
        if (existence == Existence.cloned) RecolorClones();
    }

    void Recolor() {
        chroma.ReBalance();
        rend.sharedMaterial = mat;
        rend.material.SetColor("_EmissionColor", chroma.color);
    }

    #endregion

    #region Holding Methods

    void CheckHeld() {
        if (isHeld) {
            if (Mouse.holding != this) StartHold();
            MoveToMousePosition();
        } else if (Mouse.holding == this) StopHold();
    }

    void StopHover() {
        hovered = false;
        Mouse.hover = null;
        Recolor();
    }

    public virtual void StartHold() {
        Mouse.holding = this;
        transform.parent = null;
        gameObject.layer = 2;
        if (existence == Existence.cloned) AnchorClones();
        if (GetComponent<Star>()) StartCoroutine("CheckShake");
    }

    public virtual void MoveToMousePosition() {
        if (existence == Existence.cloned) MoveClones();
        float screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth));
        UpdateLinks();
    }

    public virtual void StopHold() {
        StopHover();
        Mouse.holding = null;
        ChangeInstance();
        gameObject.layer = 0;
        if (existence == Existence.cloned) ReleaseClones();
        if (GetComponent<Star>()) StopCoroutine("CheckShake");
    }

    #endregion

    #region WorldInstance Methods

    /// <summary>
    /// Gets the parent World Instance
    /// </summary>
    void GetWorldInstance() {
        worldTransform = transform.parent;
        worldInstance = worldTransform.GetComponent<WorldInstance>();
    }

    /// <summary>
    /// Sets the element's instance to the one at the specified relative distance.
    /// </summary>
    /// <param name="diff"> The difference between the current instance and the desired one. </param>
    void SetNewInstance(int diff) {
        if (worldInstance.id == wrapper.numberOfInstances - 1) {
            transform.localScale = Vector3.one * 100;
        }
        int newid = InverseInstanceID(diff);
        transform.parent = wrapper.worldInstances[newid].transform;
        GetWorldInstance();
    }
    
    void ChangeInstance() {
        if (existence == Existence.unique)
            loop = worldInstance.loop;

        int diff = wrapper.currentInstance.id - worldInstance.id;
        if (diff == 0)
            transform.parent = worldTransform;
        else {
            SetNewInstance(diff);
            if (existence == Existence.cloned)
                SetNewCloneInstance(diff);
            UpdateLinks();
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
        if (hovered) {
            if (Input.GetMouseButtonDown(1)) {
                CreateLink(this);
            }
            if (Input.GetMouseButtonUp(1)) {
                if (Mouse.linking && Mouse.linking.GetType() == GetType() && Mouse.linking != this && !Mouse.linking.IsLinkedTo(this)) {
                    ConnectLink(this);
                    Mouse.linking = null;
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
            if (link.parent == element) return true;
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

    void ConnectLink(Element target) {
        Link newLink = Mouse.link;
        newLink.target = target;
        newLink.targetLoop = target.worldInstance.loop;
        newLink.connected = true;
        target.targeted.Add(newLink);
        newLink.parent.links.Add(newLink);
        Mouse.link = null;
        CircuitManager.CheckCircuit(target);
    }

    void UpdateLinks() {
        if (links.Count > 0) UpdateOriginPoints();
        if (targeted.Count > 0) UpdateTargetPoints();
    }

    void UpdateOriginPoints() {
        foreach (Link link in links)
            link.originLoop = link.isVisible ? wrapper.currentInstance.loop : link.originLoop;
    }

    void UpdateTargetPoints() {
        foreach (Link link in targeted)
            link.targetLoop = link.isVisible ? wrapper.currentInstance.loop : link.targetLoop;
    }

    public void DestroyAllLinks() {
        while (links.Count > 0)
            links[0].DestroyLink();
        while (targeted.Count > 0)
            targeted[0].DestroyLink();
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

        transform.localScale = localScale;
        FixPosition();
    }

    void FixPosition() { //Not sure if necessary anymore
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
        foreach (Star clone in clones)
            clone.anchored = true;
    }

    void MoveClones() {
        foreach (Star clone in clones)
            clone.transform.localPosition = worldTransform.InverseTransformPoint(transform.position);
    }

    void ReleaseClones() {
        foreach (Star clone in clones)
            clone.anchored = false;
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

    #endregion

    #region ShakeMethods

    IEnumerator CheckShake()
    {
        bool isShaking = false; 
        int shakeRequired = 6; // Number of mouse x direction change required
        int delay = 5; // Delay allowed to shake
        float shakeTime = Time.time + delay;
        bool previousMove = false;
        bool currentMove = false;

        Debug.Log("Start shake");

        while(isShaking == false && Time.time < shakeTime)
        {
            if (Mathf.Sign(Input.GetAxis("Mouse X")) > 0) currentMove = true;
            else currentMove = false;
            if(previousMove != currentMove)
            {
                previousMove = currentMove;
                shakeRequired--;
            }
            if (shakeRequired == 0) isShaking = true;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (isShaking) Debug.Log("Shaaaake"); //Where you destroy the star
        else StopStarCheckShake();
        yield return null;
    }

    void StopStarCheckShake()
    {
        StopCoroutine("CheckShake");
        StartCoroutine("CheckShake");
    }

    #endregion

}

public enum Existence {
    cloned, unique, substracted
}