﻿using UnityEngine;
using System.Collections.Generic;

public class WorldWrapper : MonoBehaviour {

    [SerializeField]
    WorldInstance ogWorldInstance;
    public List<WorldInstance> worldInstances;
    public int numberOfInstances;
    [Tooltip("The distance between each instance.")]
    public float scaleFactor = 10;
    public float scaleSpeed = 0.05f;
    public static WorldWrapper singleton;
    float minScale, maxScale;
    Universe universe;
    Vignette vignette;

    /// <summary>
    /// The ID of the instance the camera is currently in.
    /// </summary>
    int currentID;
    public WorldInstance currentInstance {
        get {
            // to do: get the instance closest to scale 1
            return worldInstances[currentID];
        }
    }

    void Awake() {
        singleton = this;
        maxScale = scaleFactor * scaleFactor;
        minScale = 1 / maxScale;
        universe = FindObjectOfType<Universe>();
        vignette = Camera.main.GetComponent<Vignette>();
    }

    public void Generate() {
        ogWorldInstance.GetComponent<WorldInstance>().enabled = false; //Prevent other instances to re-instantiate stars

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[0].transform.localScale = ogWorldInstance.transform.localScale / (maxScale);
        worldInstances[0].name = "WorldInstance [0]";

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[1].transform.localScale = ogWorldInstance.transform.localScale / scaleFactor;
        worldInstances[1].name = "WorldInstance [1]";

        worldInstances.Add(ogWorldInstance);
        currentID = 2;
        worldInstances[2].name = "WorldInstance [2]";

        worldInstances.Add(Instantiate(ogWorldInstance));
        worldInstances[3].transform.localScale = ogWorldInstance.transform.localScale * scaleFactor;
        worldInstances[3].name = "WorldInstance [3]";

        numberOfInstances = worldInstances.Count;
        universe.Initialisation();
    }

    /// <summary>
    /// Zooms in the World at the specified value
    /// </summary>
    /// <param name="value"> The value of the Zoom to perform </param>
    public void Zoom(float value) {
        if (Universe.isDone)
            vignette.fallOff = Mathf.Abs(value) / 5;

        for (int i = 0; i < worldInstances.Count; i++) {
            worldInstances[i].transform.localScale += value * worldInstances[i].transform.localScale * scaleSpeed; // sometimes infinity

            worldInstances[i].name = "WorldInstance [" + i + "]";

            if (worldInstances[i].transform.localScale.x > maxScale) {
                worldInstances[i].transform.localScale = Vector3.one * minScale;
                worldInstances[i].loop++;
                worldInstances.Insert(0, worldInstances[i]);
                worldInstances.RemoveAt(i + 1);
                ResetSizesFromSmallest();

            } else if (worldInstances[i].transform.localScale.x < minScale) {
                worldInstances[i].transform.localScale = Vector3.one * maxScale;
                worldInstances[i].loop--;
                worldInstances.Insert(worldInstances.Count, worldInstances[i]);
                worldInstances.RemoveAt(i);
                ResetSizesFromBiggest();

            }
        }
    }

    void ResetSizesFromSmallest() {
        //worldInstances[0].transform.localScale = Vector3.one * minScale;
        worldInstances[1].transform.localScale = Vector3.one / scaleFactor;
        worldInstances[2].transform.localScale = Vector3.one;
        worldInstances[3].transform.localScale = Vector3.one * scaleFactor;
    }

    void ResetSizesFromBiggest() {
        worldInstances[0].transform.localScale = Vector3.one / scaleFactor;
        worldInstances[1].transform.localScale = Vector3.one;
        worldInstances[2].transform.localScale = Vector3.one * scaleFactor;
        //worldInstances[3].transform.localScale = Vector3.one * maxScale;
    }

    public void Rotate(Vector3 rotation) {
        for (int i = 0; i < worldInstances.Count; i++) {
            worldInstances[i].transform.Rotate(rotation, Space.World); // sometimes infinity
        }
    }

    /// <summary>
    /// Returns the current index of the specified world instance.
    /// </summary>
    /// <param name="world"> The world instance whose index to return. </param>
    /// <returns> The index of the specified world instance. </returns>
    public int worldInstanceID(WorldInstance world) {
        return worldInstances.IndexOf(world);
    }
}
