using UnityEngine;

[AddComponentMenu("Camera-Control/Orbit")]
public class CameraOrbit : MonoBehaviour {

    public Transform target;
    public Transform worldInstance;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float moveSpeed = 0.5f;
    public float scaleSpeed = 0.9f;

    float pitch = 0.0f;
    float yaw = 0.0f;
    WorldWrapper wrapper;

    void Start() {
        distance = target.position.z - transform.position.z;
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

    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}