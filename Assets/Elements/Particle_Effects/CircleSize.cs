using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSize : MonoBehaviour {

	ParticleSystem ps;
	ParticleSystem.MainModule psMain;
	Star starHeld;
	float circleSize;

	void Awake () {
		ps = this.gameObject.GetComponent<ParticleSystem> ();
		psMain = ps.main;
		if (Mouse.holding is Star)
		{
			starHeld = Mouse.holding as Star;
			circleSize = starHeld.shakesPerformed;
			psMain.startSize = new ParticleSystem.MinMaxCurve(1.25f + (circleSize/1.5f));
			ps.Play ();
		}
	}
		
}
