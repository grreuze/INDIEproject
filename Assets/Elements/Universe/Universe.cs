using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{

    bool doOnce = true;
    GameObject miniUniverse;
    [SerializeField]float speed;

    // Use this for initialization
    void Start()
    {
        miniUniverse = new GameObject();
        miniUniverse.name = "MiniUniverse";
        miniUniverse.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Initialisation();
    }


    void Initialisation()
    {
        if (WorldWrapper.singleton.worldInstances.Count == 4 && doOnce)
        {
            Camera.main.GetComponent<GameController>().zoomSpeed = 0;
            foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances)
            {
                Winstance.transform.parent = miniUniverse.transform;
            }
            miniUniverse.transform.localScale = new Vector3(0.00005f, 0.00005f, 0.00005f);
            doOnce = false;
        }
    }

    void OnMouseDown()
    {
        StartCoroutine("StartGame");
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(GameObject.Find("GameTitle"));
    }


    IEnumerator StartGame()
    {

        while (miniUniverse.transform.localScale.x < 0.95)
        {
            miniUniverse.transform.localScale = Vector3.Lerp(miniUniverse.transform.localScale, Vector3.one, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        Camera.main.GetComponent<GameController>().zoomSpeed = 0.5f;
        foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances)
        {
            Winstance.transform.parent = null;
        }
        Destroy(gameObject);
        yield return new WaitForEndOfFrame();

    }

}

