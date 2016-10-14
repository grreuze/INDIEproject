using UnityEngine;

public class Star : MonoBehaviour {

    [SerializeField]
    float scaleFactor;
    [SerializeField]
    Material normalMat, hoverMat = null;
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
        transform.localScale = Vector3.one * Vector3.Distance(Vector3.zero, transform.position) * scaleFactor;

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
