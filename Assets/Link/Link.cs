using UnityEngine;

public class Link : MonoBehaviour {

    public enum CrossInstance { None, Inward, Outward }

    /// <summary>
    /// Returns whether the link crosses another instance, and in which direction (relative to the center)
    /// </summary>
    public CrossInstance isCrossing;

    public Star target;
    Star parent;
    Transform targetPoint;
    LineRenderer line;

    WorldInstance worldInstance {
        get { return parent.worldInstance; }
    }

    void Start() {
        line = GetComponent<LineRenderer>();
        parent = transform.parent.GetComponent<Star>();
    }

    void Update() {
        line.SetPosition(0, transform.position);

        if (isCrossing == CrossInstance.Outward && target.worldInstance.id < worldInstance.id)
            line.SetPosition(1, transform.position);
        else
        if (isCrossing == CrossInstance.Inward && target.worldInstance.id > worldInstance.id)
            line.SetPosition(1, Vector3.zero);
        else 
            line.SetPosition(1, target.transform.position);
    }
}
