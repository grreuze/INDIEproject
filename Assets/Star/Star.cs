using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Star : MonoBehaviour {

    #region Properties
    
    public enum Color { White, Red, Cyan, Purple, Yellow }

    public Material mat;
    public bool anchored;
    public int id;
    public Color currentColor;

    /// <summary>
    /// The star that is currently held by the player, if any.
    /// </summary>
    public static Star heldStar;
    /// <summary>
    /// The star that is currently linked by the player, if any.
    /// </summary>
    public static Star linking;

    public WorldInstance worldInstance;

    [SerializeField]
    float scaleFactor, minScale = 0.1f, maxScale = 1;
    [SerializeField]
    Material hoverMat = null;
    [SerializeField]
    Link link;

    Renderer rend;
    Vector3 parentScale;
    Transform worldTransform;
    Star[] clones;
    List<Star> linked = new List<Star>();
    List<Link> links = new List<Link>();
    bool hovered;

    /// <summary>
    /// Returns whether or not the player is holding this star.
    /// </summary>
    bool isHeld {
        get { return hovered && Input.GetMouseButton(0); }
    }

    static WorldWrapper wrapper;
    
    #endregion

    #region DefaultMethods

    void Start() {
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = mat;
        if (!wrapper) wrapper = WorldWrapper.singleton;
        GetWorldInstance();
        GetAllClones();
    }

    void OnMouseEnter() {
        if (heldStar == null) {
            hovered = true;
            rend.sharedMaterial = hoverMat;
        }
    }

    void OnMouseExit() {
        if (!isHeld) StopHold();
    }

    void LateUpdate() {
        Rescale();
        CheckHeld();
        CheckLink();

        if (Mathf.Abs(transform.localPosition.x) > 1000)
            transform.localPosition /= 10000;
        if (Mathf.Abs(transform.localPosition.x) < 0.001)
            transform.localPosition *= 10000;

        //if (Mathf.Abs(transform.localPosition.x) > worldInstance.radius)
          //  print(name + " #" + worldInstance.id + " is too far");
    }

    #endregion

    #region StarMethods

    void GetWorldInstance() {
        worldTransform = transform.parent;
        worldInstance = worldTransform.GetComponent<WorldInstance>();
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

    void Rescale() {
        parentScale = GetParentScale();
        Vector3 scale = Vector3.one * Vector3.Distance(Vector3.zero, transform.position) * scaleFactor;
        scale.x *= parentScale.x;
        scale.y *= parentScale.y;
        scale.z *= parentScale.z;

        scale.x = Mathf.Clamp(scale.x, minScale * parentScale.x, maxScale * parentScale.x);
        scale.y = Mathf.Clamp(scale.y, minScale * parentScale.y, maxScale * parentScale.y);
        scale.z = Mathf.Clamp(scale.z, minScale * parentScale.z, maxScale * parentScale.z);

        transform.localScale = scale;
    }

    void CheckHeld() {
        if (isHeld) {
            if (heldStar != this) {
                heldStar = this;
                transform.parent = null;
                AnchorClones();
            }
            MoveClones();
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        } else if (heldStar == this) StopHold();
    }

    void StopHold() {
        hovered = false;
        rend.sharedMaterial = mat;
        if (heldStar == this) {
            heldStar = null;
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
            if (links.Count > 0) CheckLinksCrossInstance();
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
        int newid = InverseInstanceID(diff);
        transform.parent = wrapper.worldInstances[newid].transform;
        GetWorldInstance();
    }

    #endregion

    #region LinkMethods

    void CheckLink() {
        if (Input.GetMouseButtonDown(1)) {
            if (hovered) {
                if (linking == null)
                    linking = this;

                else if (linking != this) {
                    linked.Add(linking);
                    int diff = linking.worldInstance.id - worldInstance.id;
                    BuildLink(linking, diff);
                    LinkClones(linking.id, diff);
                    CheckLinksCrossInstance();
                    linking = null;
                } else
                    linking = null;
            }
        }
    }

    /// <summary>
    /// Links to the specified star in the instance set at the specified distance.
    /// </summary>
    /// <param name="starID"> The ID of the star to link to. </param>
    /// <param name="diff"> The difference between the current instance and the desired one. </param>
    public void LinkTo(int starID, int diff) {
        int worldID = InverseInstanceID(diff);
        Star target = wrapper.worldInstances[worldID].stars[starID];
        linked.Add(target);
        BuildLink(target, diff);
    }

    void BuildLink(Star target, int diff) {
        Link newlink = Instantiate(link);
        newlink.transform.parent = transform;
        newlink.transform.position = transform.position;
        newlink.target = target;
        links.Add(newlink);
    }

    void CheckLinksCrossInstance() {
        foreach(Link link in links) {
            int diff = link.target.worldInstance.id - worldInstance.id;
            print(link.target.worldInstance.id + " - " + worldInstance.id + " = " + diff);


            link.isCrossing = IsCrossing(diff);
        }
        SetCrossInstanceClones();
    }

    Link.CrossInstance IsCrossing(int diff) {
        Link.CrossInstance isCrossing = 0;
        if (diff > 0)
            isCrossing = Link.CrossInstance.Outward;
        else if (diff < 0)
            isCrossing = Link.CrossInstance.Inward;
        return isCrossing;
    }

    #endregion

    #region CloneMethods

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

    void LinkClones(int starid, int diff) {
        foreach (Star clone in clones) {
            clone.LinkTo(starid, diff);
        }
    }

    void SetCrossInstanceClones() {
        foreach (Star clone in clones) {
            int i = 0;
            foreach (Link link in clone.links) {
                link.isCrossing = links[i].isCrossing;
                print(clone.name + "[" + clone.worldInstance.id + "] " + "Set link #" + i + " to " + link.isCrossing);
                i++;
            }
        }
    }

    void GetAllClones() {
        clones = new Star[wrapper.worldInstances.Count - 1];

        for (int i=0, j=0; i < clones.Length + 1; i++, j++) {
            if (i == worldInstance.id) {
                j--;
                continue;
            }
            Star clone = wrapper.worldInstances[i].stars[id];
            clones[j] = clone;
        }
    }

    #endregion

}