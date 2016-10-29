using UnityEngine;

[AddComponentMenu("LuxVertigo/GameController")]
public class GameController : MonoBehaviour {
    
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float scrollSpeed = 10;

    public bool invertX, invertY;

    WorldWrapper wrapper;

    float pitch = 0.0f;
    float yaw = 0.0f;

    void Start() {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.y;
        yaw = angles.x;
        wrapper = WorldWrapper.singleton;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update() {
        // Keyboard Controls
        pitch = Input.GetAxis("Horizontal") * xSpeed * 0.02f;
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") != 0) {
            wrapper.Zoom(Input.GetAxis("Vertical"));
            yaw = 0;
        } else
            yaw = Input.GetAxis("Vertical") * ySpeed * 0.02f;

        // Mouse Controls
        if (Input.GetMouseButton(0) && Mouse.hover == null && Mouse.holding == null) {
            pitch = Input.GetAxis("Mouse X") * xSpeed * 0.02f * -1;
            
            yaw = Input.GetAxis("Mouse Y") * ySpeed * 0.02f * -1;

            Cursor.visible = false;

        } else Cursor.visible = true;
        
        if (Input.GetMouseButtonUp(1) && Mouse.hover == null)
            Mouse.BreakLink();

        if (Input.GetMouseButton(2) && Input.GetAxis("Mouse Y") != 0)
            wrapper.Zoom(Input.GetAxis("Mouse Y"));
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            wrapper.Zoom(Input.GetAxis("Mouse ScrollWheel") * scrollSpeed);

        // Apply Rotation
        if (invertY) pitch *= -1;
        if (!invertX) yaw *= -1;
        wrapper.Rotate(new Vector3(yaw, pitch, 0));
    }
}