using UnityEngine;
using System.Collections;

public class Star : Element {

    #region Properties
    
    [Header("Star Properties")]
    public bool anchored;

    StarParticles particles;

    #endregion

    public void Awake() {
        particles = FindObjectOfType<StarParticles>();
    }

    public override void StartHold() {
        base.StartHold();
        particles.Play();
        StartCoroutine("CheckShake");
    }

    public override void StopHold() {
        particles.Stop();
        base.StopHold();
        StopCoroutine("CheckShake");
    }
    
    void BreakStar() {
        StopHold();
        DestroyAllLinks();

        if (existence == Existence.cloned)
            existence = Existence.substracted;
        else {
            particles.transform.parent = transform.root;
            Destroy(gameObject);
        }

        substractedFrom.Add(worldInstance.loop);
        CreatePrisms();
    }

    void CreatePrisms() {
        if (chroma.isPure) chroma *= Chroma.MAX; //If only one color, give 3 prisms instead of one

        for (int i = 0; i < chroma.r; i++)
            CreatePrism(Chroma.red);
        for (int i = 0; i < chroma.g; i++)
            CreatePrism(Chroma.green);
        for (int i = 0; i < chroma.b; i++)
            CreatePrism(Chroma.blue);
    }

    void CreatePrism(Chroma value) {
        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        Prism prism = (Prism)Instantiate(PrefabManager.prism, randomPosition, Quaternion.identity);
        prism.transform.parent = WorldWrapper.singleton.currentInstance.transform;
        prism.chroma = value;
        prism.transform.LookAt(transform);
    }

    IEnumerator CheckShake() {
        bool isShaking = false;
        int shakeRequired = 6; // Number of mouse x direction change required
        int delay = 2; // Delay allowed to shake, in seconds
        float shakeTime = Time.time + delay;
        bool previousMove = false;
        bool currentMove = false;
        float minAmplitude = 0.3f;
        
        while (isShaking == false && Time.time < shakeTime) {

            float mouseX = Input.GetAxis("Mouse X");
            currentMove = Mathf.Sign(mouseX) > 0;
            
            if (Mathf.Abs(mouseX) > minAmplitude) {
                if (previousMove != currentMove) {
                    previousMove = currentMove;
                    shakeRequired--;
                }
                if (shakeRequired == 0)
                    isShaking = true;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (isShaking) {
            BreakStar();
        } else ResetCheckShake();
    }

    void ResetCheckShake() {
        StopCoroutine("CheckShake");
        StartCoroutine("CheckShake");
    }

}