using UnityEngine;
using System.Collections.Generic;

public class WorldWrapper : MonoBehaviour {

    [SerializeField]
    Transform ogWorldInstance;
    public List<Transform> worldInstances;

    public float scaleFactor = 10;
    public float scaleSpeed = 0.05f;

    public float minScale {
        get { return 1 / (scaleFactor * scaleFactor); }
    }
    public float maxScale {
        get { return scaleFactor* scaleFactor; }
    }

    public static WorldWrapper singleton;

    void Awake() {
        singleton = this;
    }

    void Start() {
        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[0].localScale = ogWorldInstance.localScale / (scaleFactor * scaleFactor);

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[1].localScale = ogWorldInstance.localScale / scaleFactor;

        worldInstances.Add(ogWorldInstance);

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[3].localScale = ogWorldInstance.localScale * scaleFactor;
    }

    public void Zoom() {
        for (int i=0; i < worldInstances.Count; i++) {
            worldInstances[i].localScale += Input.GetAxis("Vertical") * worldInstances[i].localScale * scaleSpeed;

            if (worldInstances[i].localScale.x > maxScale) {
                worldInstances[i].localScale = Vector3.one * minScale;
                worldInstances.Insert(0, worldInstances[i]);
                worldInstances.RemoveAt(i + 1);

            } else if (worldInstances[i].localScale.x < minScale) {
                worldInstances[i].localScale = Vector3.one * maxScale;
                worldInstances.Insert(worldInstances.Count, worldInstances[i]);
                worldInstances.RemoveAt(i);

            }
        }
    }
}
