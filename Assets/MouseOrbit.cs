using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbit : MonoBehaviour {

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float moveSpeed = 0.5f;

    float x = 0.0f;
    float y = 0.0f;
    
    void Start() {
        distance = target.position.z - transform.position.z;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate() {
        if (target) {
            x -= Input.GetAxis("Horizontal") * xSpeed * 0.02f;

            if (Input.GetKey(KeyCode.LeftShift))
                distance = distance - Input.GetAxis("Vertical") * moveSpeed;
            else
                y += Input.GetAxis("Vertical") * ySpeed * 0.02f;

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
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