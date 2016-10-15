using UnityEngine;

[AddComponentMenu("Camera-Control/Orbit")]
public class CameraOrbit : MonoBehaviour {

    public Transform target;
    public Transform worldInstance;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    float pitch = 0.0f;
    float yaw = 0.0f;
    WorldWrapper wrapper;

    void Start() {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.y;
        yaw = angles.x;
        wrapper = WorldWrapper.singleton;
    }

    void LateUpdate() {
        if (target) {
            pitch = Input.GetAxis("Horizontal") * xSpeed * 0.02f;

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") != 0)
                wrapper.Zoom();
            else
                yaw = Input.GetAxis("Vertical") * ySpeed * 0.02f;
            
            wrapper.Rotate(new Vector3(yaw, pitch, 0));
        }
    }
}