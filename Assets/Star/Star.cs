using UnityEngine;

[ExecuteInEditMode]
public class Star : MonoBehaviour {

    #region Properties
    
    public enum Color { White, Red, Cyan, Purple, Yellow }

    [SerializeField]
    float scaleFactor, minScale = 0.1f, maxScale = 1;
    [SerializeField]
    Material hoverMat = null;
    Renderer rend;
    Vector3 parentScale;
    Transform worldTransform;
    WorldInstance worldInstance;
    Star[] clones;
    bool hovered;

    static WorldWrapper wrapper;

    public Material mat;
    public bool anchored;
    public int id;
    public Color currentColor;

    /// <summary>
    /// The star that is currently held by the player, if any.
    /// </summary>
    public static Star heldStar;

    /// <summary>
    /// Returns whether or not the player is holding this star.
    /// </summary>
    bool isHeld {
        get { return hovered && Input.GetMouseButton(0); }
    }
    
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

    void Update() {
        Rescale();
        CheckHeld();
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
        }
    }

    public void SetNewInstance(int diff) {
        int instances = wrapper.worldInstances.Count;
        int newid = worldInstance.id + diff;
        if (newid < 0) newid += instances;
        else if (newid > instances - 1) {
            newid -= instances;
        }
        
        transform.parent = wrapper.worldInstances[newid].transform;
        GetWorldInstance();
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

    void GetAllClones() {
        WorldWrapper wrapper = WorldWrapper.singleton;

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