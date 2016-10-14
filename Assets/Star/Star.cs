using UnityEngine;

public class Star : MonoBehaviour {

    [SerializeField]
    Material normalMat, hoverMat;
    Renderer rend;
    bool hovered;

    public static Star holding;

    bool held {
        get {
            return hovered && Input.GetMouseButton(0);
        }
    }
    
    void Awake() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        if (held) {
            holding = this;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
    }

    void OnMouseEnter() {
        if (holding == null)
            hovered = true;
        rend.material = hoverMat;
    }

    void OnMouseExit() {
        if (!held) {
            hovered = false;
            holding = null;
        }
        rend.material = normalMat;
    }


}
