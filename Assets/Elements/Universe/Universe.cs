using System.Collections;
using UnityEngine;

public class Universe : MonoBehaviour {

    public static bool isDone = false;

    GameObject miniUniverse;
    [SerializeField]
    float duration = 2;
    [SerializeField]
    AnimationCurve animCurve, vignetteCurve = null;
    GameController gc;
    ParticleSystem ps;
    float zoomSpeed, zoomSpeedModifier = 10;
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
        zoomSpeed = gc.zoomIncrementation;
        gc.zoomIncrementation = 0;
        foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances)
            Winstance.transform.parent = miniUniverse.transform;
        miniUniverse.transform.localScale = small;
    }

    void OnMouseDown() {
        StartCoroutine("StartGame");
        Destroy(GameObject.Find("GameTitle"));
        SoundManager.singleton.Play(SoundManager.singleton.startSound, 1f, Camera.main.GetComponent<AudioSource>());
        Camera.main.GetComponent<GameController>().gameStarted = true;
    }

    IEnumerator StartGame() {
        ps.Clear();
        ps.Stop();
        gc.zoomIncrementation = zoomSpeed * zoomSpeedModifier;

        isDone = true;
        for (float elapsed = 0; elapsed < duration; elapsed+=Time.deltaTime) {
            float t = elapsed / duration;

            vignette.color = Color.Lerp(Color.black, Color.white, vignetteCurve.Evaluate(t));
            miniUniverse.transform.localScale = Vector3.Lerp(small, Vector3.one, animCurve.Evaluate(t));
            gc.zoomIncrementation = Mathf.Lerp(zoomSpeed * zoomSpeedModifier, zoomSpeed, animCurve.Evaluate(t));

            yield return null;
        }
        miniUniverse.transform.localScale = Vector3.one;
        gc.zoomIncrementation = zoomSpeed;

        foreach (WorldInstance Winstance in WorldWrapper.singleton.worldInstances)
            Winstance.transform.parent = null;

        Destroy(gameObject);
    }

}

