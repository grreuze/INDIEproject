using UnityEngine;

public class WorldInstance : MonoBehaviour {

    public int numberOfStars;
    public float radius = 10;
    public Star[] stars;
    public int loop = 0;

    public int id {
        get { return WorldWrapper.singleton.worldInstanceID(this); }
    }

    void Start() {
        stars = new Star[numberOfStars];
        for (int i = 0; i < numberOfStars; i++) {
            Vector3 pos = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
            Star newStar = (Star)Instantiate(PrefabManager.star, pos, Quaternion.identity);
            newStar.transform.parent = transform;
            newStar.name = "Star " + i;
            newStar.id = i;
            newStar.chroma = Chroma.white;

            stars[i] = newStar;
        }
        WorldWrapper.singleton.Generate();
    }

    /// <summary>
    /// Creates a Star at the defined position with the defined Chromatic property.
    /// </summary>
    /// <param name="pos"> The position of the new Star. </param>
    /// <param name="chroma"> The Chromatic property of the new Star. </param>
    public void CreateStar(Vector3 pos, Chroma chroma) {
        Star newStar = (Star)Instantiate(PrefabManager.star, pos, Quaternion.identity);
        newStar.transform.parent = transform;
        newStar.name = "New Star";
        newStar.existence = Existence.unique;
        newStar.chroma = chroma;
        newStar.id = -1;
    }

}
