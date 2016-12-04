using UnityEngine;

public class WorldSpaceCursor : MonoBehaviour {
    [SerializeField]
    float distance = 10;

    TrailRenderer trail, cutter;
    MeshRenderer rend;

    void Awake() {
        trail = GetComponent<TrailRenderer>();
        cutter = GetComponentsInChildren<TrailRenderer>()[1];
        rend = GetComponentInChildren<MeshRenderer>();
    }

	void Update () {
        cutter.enabled = Input.GetMouseButton(1) && !Mouse.linking;
        rend.enabled = trail.enabled = !cutter.enabled;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
    }
}
