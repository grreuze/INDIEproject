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
        WorldWrapper wrapper = WorldWrapper.singleton;

        DestroyAllLinks();

        if (existence == Existence.cloned)
            existence = Existence.substracted;
        else Destroy(gameObject);

        substractedFrom.Add(worldInstance.loop);

        if (chroma.isPure) chroma *= Chroma.MAX; //If only one color, give 3 prisms instead of one

        for (int i = 0; i < chroma.r; i++) {
            Prism redPrism = (Prism)Instantiate(PrefabManager.prism,
                transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)),
                Quaternion.identity);
            redPrism.transform.parent = wrapper.currentInstance.transform;
            redPrism.chroma = Chroma.red;
            redPrism.transform.LookAt(transform);
        }
        for (int i = 0; i < chroma.g; i++) {
            Prism greenPrism = (Prism)Instantiate(PrefabManager.prism,
                transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)),
                Quaternion.identity);
            greenPrism.transform.parent = wrapper.currentInstance.transform;
            greenPrism.chroma = Chroma.green;
            greenPrism.transform.LookAt(transform);
        }
        for (int i = 0; i < chroma.b; i++) {
            Prism bluePrism = (Prism)Instantiate(PrefabManager.prism,
                transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)),
                Quaternion.identity);
            bluePrism.transform.parent = wrapper.currentInstance.transform;
            bluePrism.chroma = Chroma.blue;
            bluePrism.transform.LookAt(transform);
        }
    }

    IEnumerator CheckShake() {
        bool isShaking = false;
        int shakeRequired = 10; // Number of mouse x direction change required
        int delay = 1; // Delay allowed to shake, in seconds
        float shakeTime = Time.time + delay;
        bool previousMove = false;
        bool currentMove = false;
        float minAmplitude = 0.3f;
        
        while (isShaking == false && Time.time < shakeTime) {

            float mouseX = Input.GetAxis("Mouse X");
            currentMove = Mathf.Sign(mouseX) > 0 && Mathf.Abs(mouseX) > minAmplitude;
            
            if (previousMove != currentMove) {
                previousMove = currentMove;
                shakeRequired--;
            }
            if (shakeRequired == 0)
                isShaking = true;
            
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