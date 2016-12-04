using UnityEngine;
using System.Collections;

public class SpinBackground : MonoBehaviour {

	public float spinSpeed;
	private Vector3 spinSpeedPrivate;

	
	// Update is called once per frame
	void Update () {
		spinSpeedPrivate = new Vector3 (0, spinSpeed, 0);
		transform.Rotate (spinSpeedPrivate * Time.deltaTime);
	}
}
