using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Link : MonoBehaviour {

    [SerializeField, Tooltip("The start and end width of the line renderer")]
    float _width = 0.2f;
    float width {
        get { return _width / transform.lossyScale.x; }
    }

    public Star parent;
    public Star target;
    public int targetLoop;
    public int originLoop;
    public bool connected;

    /// <summary>
    /// Returns whether or not the Link is currently visible by the player.
    /// </summary>
    public bool isVisible {
        get { return (origin != end) || (origin == end && end == MetaPosition.InRange); }
    }
    enum MetaPosition { InRange, External, Internal }
    MetaPosition origin, end;
    LineRenderer line;
    BoxCollider col;
    bool destroyed;
    /// <summary>
    /// The length of the link.
    /// </summary>
    float length {
        get { return Vector3.Distance(originPosition, targetPosition); }
    }

    WorldInstance worldInstance {
        get { return parent.worldInstance; }
    }

    Vector3 originPosition {
        get { return GetMetaPosition(transform.position, ref origin, originLoop, worldInstance.loop); }
    }

    Vector3 targetPosition {
        get { return GetMetaPosition(target.transform.position, ref end, targetLoop, target.worldInstance.loop); }
    }

    void Start() {
        line = GetComponent<LineRenderer>();
        parent = transform.parent.GetComponent<Star>();
        col = GetComponent<BoxCollider>();
        line.SetWidth(width, width);
        transform.localPosition = Vector3.zero;
    }

    void LateUpdate() { // We should change it so that links only update when necessary
        line.SetPosition(0, originPosition);
        if (connected) {
            line.SetPosition(1, targetPosition);

            transform.LookAt(targetPosition);
            col.center = Vector3.forward * (length / 2);
            col.size = new Vector3(width, width, length);
            // We need to take local scale into account when changing the size of the collider

        } else {
            float screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth)));
        }
    }

    void OnMouseEnter() {
        if (Mouse.breakLinkMode && !destroyed) {
            destroyed = true;
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            ps.transform.localEulerAngles = 90 * Vector3.up;
            ps.transform.localPosition = Vector3.forward * (length / 2);

            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rate = 70 * length;

            ParticleSystem.ShapeModule shape = ps.shape;
            shape.radius = length / 2;

            ps.Play();
            line.enabled = false;
            Invoke("DestroyLink", ps.startLifetime);
        }
    }

    void DestroyLink() {
        parent.links.Remove(this);
        target.targeted.Remove(this);
        // We should also remove the loops containing this link
        Destroy(gameObject); // We should probably do object pooling for links
    }

    /// <summary>
    /// Returns the position of a point given the loop it is in and the current world loop.
    /// </summary>
    /// <param name="point"> The world position of the desired point. </param>
    /// <param name="loop"> The loop of the desired point. </param>
    /// <param name="worldLoop"> The loop of the world instance containing the desired point. </param>
    /// <returns> The actual world position the point is at. </returns>
    Vector3 GetMetaPosition(Vector3 point, ref MetaPosition pos, int loop, int worldLoop) {
        if (worldLoop > loop) {
            pos = MetaPosition.External;
            return point.normalized * 1000;
        } else if (worldLoop < loop) {
            pos = MetaPosition.Internal;
            return Vector3.zero;
        } else {
            pos = MetaPosition.InRange;
            return point;
        }
    }
}
