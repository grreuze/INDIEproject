using System.Collections;
using UnityEngine;

public class Universe : MonoBehaviour {
    
    GameObject miniUniverse;
    [SerializeField]
    float duration = 2;
    [SerializeField]
    AnimationCurve animCurve, vignetteCurve = null;
    GameController gc;
    ParticleSystem ps;
    float zoomSpeed, zoomSpeedModifier = 20;
    Vector3 small = Vector3.one * 0.00001f;
    Vignette vignette;

    void Start() {
        miniUniverse = new GameObject();
        miniUniverse.name = "MiniUniverse";
        miniUniverse.transform.parent = this.transform;
        gc = Camera.main.GetComponent<GameController>();
        ps = GetComponent<ParticleSystem>();
        vignette = Camera.main.GetComponent<Vignette>();
    }

    public void Initialisation() {
        zoomSpeed = gc.zoomSpeed;
        gc.zoomSpeed = 0;
        foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances)
            Winstance.transform.parent = miniUniverse.transform;
        miniUniverse.transform.localScale = small;
    }

    void OnMouseDown() {
        StartCoroutine("StartGame");
        Destroy(GameObject.Find("GameTitle"));
    }
    
    IEnumerator StartGame() {
        ps.Clear();
        ps.Stop();
        gc.zoomSpeed = zoomSpeed * zoomSpeedModifier;
        

        for (float elapsed = 0; elapsed < duration; elapsed+=Time.deltaTime) {
            float t = elapsed / duration;

            vignette.color = Color.Lerp(Color.black, Color.white, vignetteCurve.Evaluate(t));
            miniUniverse.transform.localScale = Vector3.Lerp(small, Vector3.one, animCurve.Evaluate(t));
            gc.zoomSpeed = Mathf.Lerp(zoomSpeed * zoomSpeedModifier, zoomSpeed, animCurve.Evaluate(t));

            yield return null;
        }
        miniUniverse.transform.localScale = Vector3.one;
        gc.zoomSpeed = zoomSpeed;

        foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances)
            Winstance.transform.parent = null;

        Destroy(gameObject);
    }

}

