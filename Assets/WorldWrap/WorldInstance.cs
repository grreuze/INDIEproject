﻿using UnityEngine;

public class WorldInstance : MonoBehaviour {

    public int numberOfStars;
    public float radius = 10;
    [SerializeField]
    Star starPrefab;
    public Star[] stars;

    [SerializeField]
    Material[] starMaterials;

    public int id {
        get { return WorldWrapper.singleton.ID(this); }
    }

    void Start() {
        stars = new Star[numberOfStars];
        for (int i = 0; i < numberOfStars; i++) {
            Vector3 pos = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
            Star newStar = (Star)Instantiate(starPrefab, pos, Quaternion.identity);
            newStar.transform.parent = transform;
            newStar.name = "Star " + i;
            newStar.id = i;
            newStar.currentColor = (Star.Color) Random.Range(0, starMaterials.Length);
            newStar.mat = starMaterials[(int)newStar.currentColor];
            stars[i] = newStar;
        }
        WorldWrapper.singleton.Generate();
    }

}
