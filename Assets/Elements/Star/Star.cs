using UnityEngine;
using System.Collections.Generic;

public class Star : Element {

    #region Properties
    
    [Header("Star Properties")]
    public Material mat;
    public bool anchored;
    public int id;

    public List<Link> links = new List<Link>();
    public List<Link> targeted = new List<Link>();
    
    [SerializeField]
    Material hoverMat = null;
    [SerializeField]
    Link link;

    Renderer rend;
    Transform worldTransform;
    Star[] clones;
    bool hovered;

    /// <summary>
    /// Returns whether or not the player is holding this star.
    /// </summary>
    bool isHeld {
        get { return hovered && Input.GetMouseButton(0); }
    }

    static WorldWrapper wrapper;
    
    #endregion

    #region MonoBehaviour Methods

    void Start() {
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = mat;
        if (!wrapper) wrapper = WorldWrapper.singleton;
        GetWorldInstance();
        GetAllClones();
    }

    void OnMouseEnter() {
        if (!isHeld && !Input.GetMouseButton(0) && Mouse.holding == null) {
            hovered = true;
            Mouse.hover = this;
            rend.sharedMaterial = hoverMat;
        }
    }

    void OnMouseExit() {
        if (!isHeld) StopHold();
    }

    void Update() {
        Rescale(); // We should only rescale if zooming / dezooming / anchored
        CheckHeld();
        CheckLink();

        if (Mathf.Abs(transform.localPosition.x) > 1000)
            transform.localPosition /= 10000;
        if (Mathf.Abs(transform.localPosition.x) < 0.001)
            transform.localPosition *= 10000;
    }

    #endregion

    #region StarMethods

    /// <summary>
    /// Gets the parent World Instance
    /// </summary>
    void GetWorldInstance() {
        worldTransform = transform.parent;
        worldInstance = worldTransform.GetComponent<WorldInstance>();
        worldInstance.stars[id] = this;
    }

    void CheckHeld() {
        if (isHeld) {
            if (Mouse.holding != this) {
                Mouse.holding = this;
                transform.parent = null;
                AnchorClones();
            }
            MoveClones();
            float screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth));
            UpdateLinks();
        } else if (Mouse.holding == this) StopHold();
    }

    void StopHold() {
        hovered = false;
        Mouse.hover = null;
        rend.sharedMaterial = mat;
        if (Mouse.holding == this) {
            Mouse.holding = null;
            ChangeInstance();
            ReleaseClones();
        }
    }

    void ChangeInstance() {
        int diff = wrapper.currentInstance.id - worldInstance.id;
        if (diff == 0)
            transform.parent = worldTransform;
        else {
            SetNewInstance(diff);
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
    

    /// <summary>
    /// Sets the star's instance to the one at the specified relative distance.
    /// </summary>
    /// <param name="diff"> The difference between the current instance and the desired one. </param>
    public void SetNewInstance(int diff) {
        if  (worldInstance.id == wrapper.numberOfInstances - 1) {
            transform.localScale = Vector3.one * 100;
        }
        int newid = InverseInstanceID(diff);
        transform.parent = wrapper.worldInstances[newid].transform;
        GetWorldInstance();
    }

    #endregion

    #region LinkMethods

    void CheckLink() {
        if (hovered) {
            if (Input.GetMouseButtonDown(1)) {
                CreateLink(this);
            }
            if (Input.GetMouseButtonUp(1)) {
                if (Mouse.linking && Mouse.linking != this && !Mouse.linking.IsLinkedTo(this)) {
                    ConnectLink(this);
                    Mouse.linking = null;
                } else {
                    Mouse.BreakLink();
                }
            }
        }
    }

    void CreateLink(Star origin) {
        Mouse.linking = origin;
        Mouse.link = Instantiate(link);
        Mouse.link.transform.parent = transform;
        Mouse.link.transform.position = transform.position;
        Mouse.link.originLoop = worldInstance.loop;
        Mouse.link.connected = false;
    }

    void ConnectLink(Star target) {
        Link newLink = Mouse.link;
        newLink.target = target;
        newLink.targetLoop = target.worldInstance.loop;
        newLink.connected = true;
        target.targeted.Add(newLink);
        newLink.parent.links.Add(newLink);
        Mouse.link = null;
        CircuitManager.CheckCircuit(target);
    }

    bool IsLinkedTo(Star star) {
        foreach (Link link in links)
            if (link.target == star) return true;
        foreach (Link link in targeted)
            if (link.parent == star) return true;
        return false;
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
        foreach (Star clone in clones) {
            clone.anchored = true;
        }
    }

    void MoveClones() {
        foreach (Star clone in clones) {
            clone.transform.localPosition = worldTransform.InverseTransformPoint(transform.position);
        }
    }

    void ReleaseClones() {
        foreach (Star clone in clones) {
            clone.anchored = false;
        }
    }

    void SetNewCloneInstance(int diff) {
        foreach (Star clone in clones) {
            clone.SetNewInstance(diff);
        }
    }

    #endregion

}