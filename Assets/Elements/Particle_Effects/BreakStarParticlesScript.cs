using System.Collections;
using UnityEngine;

public class BreakStarParticlesScript : MonoBehaviour {

	ParticleSystem ps;

	void Awake () {
		StartCoroutine (DestroyParticleSystem ());
		ps = GetComponent<ParticleSystem>();
		ps.Play();
	}

	IEnumerator DestroyParticleSystem()
	{
		yield return new WaitForSeconds (1);
		Destroy (this.gameObject);
	}
}
