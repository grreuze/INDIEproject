using UnityEngine;

public class StarParticles : MonoBehaviour {

    ParticleSystem ps;

    void Awake() {
        ps = GetComponent<ParticleSystem>();
        ps.Stop();
    }

    public void Play() {
        ps.Play();
        transform.parent = Mouse.holding.transform;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public void Stop() {
        ps.Stop();
    }
}
