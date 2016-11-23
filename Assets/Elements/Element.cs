using UnityEngine;
using System.Collections.Generic;

public abstract class Element : MonoBehaviour {

    #region Properties

    [Header("Element Properties")]

    public WorldInstance worldInstance;
    public Chroma chroma;
    [HideInInspector]
    public Transform worldTransform;

    public List<Link> links = new List<Link>();
    public List<Link> targeted = new List<Link>();

    public bool hovered;

    /// <summary>
    /// Returns whether or not the player is holding this element.
    /// </summary>
    public bool isHeld {
        get { return hovered && Input.GetMouseButton(0); }
    }

    public static WorldWrapper wrapper;

    [SerializeField]
    Link link;

    float scaleFactor = 0.2f;
    MinMax scale = new MinMax(0, 0.8f);

    #endregion

    public void Init() {
        if (!wrapper) wrapper = WorldWrapper.singleton;
        GetWorldInstance();
    }

    #region Instance Methods

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
    public virtual void SetNewInstance(int diff) {
        if (worldInstance.id == wrapper.numberOfInstances - 1) {
            transform.localScale = Vector3.one * 100;
        }
        int newid = InverseInstanceID(diff);
        transform.parent = wrapper.worldInstances[newid].transform;
        GetWorldInstance();
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
    
    public void CheckLink() {
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
        Mouse.link = Instantiate(link);
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

    public void UpdateLinks() {
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

    #region Scale Methods

    /// <summary>
    /// Rescales an element depending on its parent's scale.
    /// </summary>
    public void Rescale() {
        Vector3 parentScale = GetParentScale();
        Vector3 localScale = Vector3.one * Vector3.Distance(Vector3.zero, transform.position) * scaleFactor;
        localScale.x *= parentScale.x;
        localScale.y *= parentScale.y;
        localScale.z *= parentScale.z;

        localScale.x = Mathf.Clamp(localScale.x, scale.min * parentScale.x, scale.max * parentScale.x);
        localScale.y = Mathf.Clamp(localScale.y, scale.min * parentScale.y, scale.max * parentScale.y);
        localScale.z = Mathf.Clamp(localScale.z, scale.min * parentScale.z, scale.max * parentScale.z);

        transform.localScale = localScale;
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

}
