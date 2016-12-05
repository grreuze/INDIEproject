using UnityEngine;

public class StarParticles : MonoBehaviour {

    ParticleSystem ps;
	ParticleSystem.MainModule psMain;
	ParticleSystem.TrailModule particleTrail;

    void Awake() {
        ps = GetComponent<ParticleSystem>();
        ps.Stop();
		particleTrail = ps.trails;
		psMain = ps.main;
            }

    public void Play() {
        ps.Play();
        transform.parent = Mouse.holding.transform;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

	public void Update()
	{
		int psCount = ps.particleCount;
		float trailOpacity = psCount / 70f;
		particleTrail.colorOverTrail = new Color (trailOpacity, trailOpacity, trailOpacity);
		psMain.startSpeedMultiplier = psCount / 10f;
	}



    public void Stop() {
        ps.Stop();
    }
}
