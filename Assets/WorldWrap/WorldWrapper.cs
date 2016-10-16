using UnityEngine;
using System.Collections.Generic;

public class WorldWrapper : MonoBehaviour {

    [SerializeField]
    WorldInstance ogWorldInstance;
    public List<WorldInstance> worldInstances;

    public float scaleFactor = 10;
    public float scaleSpeed = 0.05f;

    float minScale, maxScale;

    public static WorldWrapper singleton;
    int currentID;
    public WorldInstance currentInstance {
        get { return worldInstances[currentID]; }
    }

    void Awake() {
        singleton = this;
        maxScale = scaleFactor * scaleFactor;
        minScale = 1 / maxScale;
    }

    public void Generate() {
        ogWorldInstance.GetComponent<WorldInstance>().enabled = false;

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[0].transform.localScale = ogWorldInstance.transform.localScale / (scaleFactor * scaleFactor);

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[1].transform.localScale = ogWorldInstance.transform.localScale / scaleFactor;

        worldInstances.Add(ogWorldInstance);
        currentID = 2;

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[3].transform.localScale = ogWorldInstance.transform.localScale * scaleFactor;
    }

    public void Zoom() {
        for (int i = 0; i < worldInstances.Count; i++) {
            worldInstances[i].transform.localScale += Input.GetAxis("Vertical") * worldInstances[i].transform.localScale * scaleSpeed;

            if (worldInstances[i].transform.localScale.x > maxScale) {
                worldInstances[i].transform.localScale = Vector3.one * minScale;
                worldInstances.Insert(0, worldInstances[i]);
                worldInstances.RemoveAt(i + 1);

            } else if (worldInstances[i].transform.localScale.x < minScale) {
                worldInstances[i].transform.localScale = Vector3.one * maxScale;
                worldInstances.Insert(worldInstances.Count, worldInstances[i]);
                worldInstances.RemoveAt(i);

            }
        }
    }

    public void Rotate(Vector3 rotation) {
        for (int i = 0; i < worldInstances.Count; i++) {
            worldInstances[i].transform.Rotate(rotation, Space.World);
        }
    }

    /// <summary>
    /// Returns the current index of the specified world instance.
    /// </summary>
    /// <param name="world"> The world instance whose index to return. </param>
    /// <returns> The index of the specified world instance. </returns>
    public int ID(WorldInstance world) {
        return worldInstances.IndexOf(world);
    }
}
