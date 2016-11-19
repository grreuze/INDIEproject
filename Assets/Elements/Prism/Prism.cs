using UnityEngine;

public class Prism : Element {

    #region Properties

    [Header("Prism Properties")]
    public Material mat;
    public int id;

    [SerializeField]
    Material hoverMat = null;

    Renderer rend;

    #endregion

    #region MonoBehaviour Methods

    void Start() {
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = mat;
        Init();
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

    #region Holding Methods

    void CheckHeld() {
        if (isHeld) {
            if (Mouse.holding != this) {
                Mouse.holding = this;
                transform.parent = null;
            }
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
        }
    }

    void ChangeInstance() {
        int diff = wrapper.currentInstance.id - worldInstance.id;
        if (diff == 0)
            transform.parent = worldTransform;
        else {
            SetNewInstance(diff);
            UpdateLinks();
        }
    }
    
    #endregion
    
}
