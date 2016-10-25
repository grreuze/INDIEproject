using UnityEngine;

public class Link : MonoBehaviour {

    LineRenderer line;
    public Transform targetPoint;

    void Start() {
        line = GetComponent<LineRenderer>();
    }

    void Update() {
        line.SetPosition(0, transform.parent.position);
        line.SetPosition(1, targetPoint.position);
    }

}
