using UnityEngine;

public class Link : MonoBehaviour {
    
    public Star target;
    public int targetLoop;
    public int originLoop;

    Star parent;
    LineRenderer line;
    WorldWrapper wrapper;

    WorldInstance worldInstance {
        get { return parent.worldInstance; }
    }

    Vector3 originPosition {
        get { return GetMetaPosition(transform.position, originLoop, worldInstance.loop); }
    }

    Vector3 targetPosition {
        get { return GetMetaPosition(target.transform.position, targetLoop, target.worldInstance.loop); }
    }

    void Start() {
        line = GetComponent<LineRenderer>();
        parent = transform.parent.GetComponent<Star>();
        wrapper = WorldWrapper.singleton;
    }

    void Update() {
        line.SetPosition(0, originPosition);
        line.SetPosition(1, targetPosition);
    }

    /// <summary>
    /// Returns the position of a point given the loop it is in and the current world loop.
    /// </summary>
    /// <param name="point"> The world position of the desired point. </param>
    /// <param name="loop"> The loop of the desired point. </param>
    /// <param name="worldLoop"> The loop of the world instance containing the desired point. </param>
    /// <returns> The actual world position the point is at. </returns>
    Vector3 GetMetaPosition(Vector3 point, int loop, int worldLoop) {
        if (!wrapper.repeatLinks && worldLoop > loop)
            return point.normalized * 1000; // Position at the outside of the instanciated worlds
        else if (!wrapper.repeatLinks && worldLoop < loop)
            return Vector3.zero; // Position inside the center of the world
        else
            return point; // Position of the actual instanciated point
    }
}
