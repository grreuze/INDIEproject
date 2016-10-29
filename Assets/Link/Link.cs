using UnityEngine;

public class Link : MonoBehaviour {

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
    }

    void LateUpdate() {
        line.SetPosition(0, originPosition);
        if (connected)
            line.SetPosition(1, targetPosition);
        else {
            float screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth)));
        }
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
