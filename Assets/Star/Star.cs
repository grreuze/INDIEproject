using UnityEngine;

[ExecuteInEditMode]
public class Star : MonoBehaviour {

    #region Properties

    [SerializeField]
    float scaleFactor, minScale = 0.1f, maxScale = 1;
    [SerializeField]
    Material normalMat, hoverMat = null;
    Renderer rend;
    Vector3 parentScale;
    Transform worldInstance;
    bool hovered;

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

    void Awake() {
        rend = GetComponent<Renderer>();
        worldInstance = transform.parent;
    }

    void OnMouseEnter() {
        if (heldStar == null)
            hovered = true;
        rend.material = hoverMat;
    }

    void OnMouseExit() {
        if (!isHeld) StopHold();
        rend.material = normalMat;
    }

    void Update() {
        Rescale();
        CheckHeld();
    }

    #endregion

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
            heldStar = this;
            transform.parent = null;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        } else if (heldStar == this) StopHold();
    }

    void StopHold() {
        hovered = false;
        heldStar = null;
        transform.parent = worldInstance;
    }

}
