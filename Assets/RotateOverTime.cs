using UnityEngine;

public class RotateOverTime : MonoBehaviour {

    [SerializeField]
    float speed;
    Vector3 rot;
    Transform _transform;

	void Start () {
        _transform = transform;
        rot = _transform.localRotation.eulerAngles;
	}
	
	void Update () {
        rot.z += Time.deltaTime * speed;
        _transform.localRotation = Quaternion.Euler(rot);
	}
}
