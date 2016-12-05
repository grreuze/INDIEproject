using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{

    bool doOnce = true;
    GameObject miniUniverse;
    [SerializeField]float speed;
    GameController gc;
    ParticleSystem ps;
    
    void Start() {
        miniUniverse = new GameObject();
        miniUniverse.name = "MiniUniverse";
        miniUniverse.transform.parent = this.transform;
        gc = Camera.main.GetComponent<GameController>();
        ps = GetComponent<ParticleSystem>();
    }
    
    void Update() {
        Initialisation();
    }

    float zoomSpeed, zoomSpeedModifier = 10;
    void Initialisation() {
        if (WorldWrapper.singleton.worldInstances.Count == 4 && doOnce) {
            zoomSpeed = gc.zoomSpeed;
            gc.zoomSpeed = 0;
            foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances) {
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
    
    IEnumerator StartGame() {

        ps.Clear();
        ps.Stop();

        gc.zoomSpeed = zoomSpeed * zoomSpeedModifier;

        while (miniUniverse.transform.localScale.x < 0.999) {

            miniUniverse.transform.localScale = Vector3.Lerp(miniUniverse.transform.localScale, Vector3.one, Time.deltaTime * speed);
            gc.zoomSpeed = Mathf.Lerp(zoomSpeed * zoomSpeedModifier, zoomSpeed, Time.deltaTime * speed);

            yield return new WaitForEndOfFrame();
        }

        miniUniverse.transform.localScale = Vector3.one;
        gc.zoomSpeed = zoomSpeed;

        foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances) {
            Winstance.transform.parent = null;
        }
        Destroy(gameObject);
        yield return new WaitForEndOfFrame();

    }

}

