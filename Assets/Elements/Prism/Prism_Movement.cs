using System.Collections;
using UnityEngine;

public class Prism_Movement : MonoBehaviour {

	private Vector3 originalPosition;

	public float duration = 0.5f;

	IEnumerator bringPrismTowardsCenterOfPath(Vector3 centerOfPath)
	{
		originalPosition = transform.position;

		for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
		{
			float t = elapsed / duration;

			transform.position = Vector3.Lerp(originalPosition, centerOfPath, t);

			yield return null;
		}
        GetComponent<Prism>().DestroyAllLinks();	
		Destroy (this.gameObject);
	}
}
