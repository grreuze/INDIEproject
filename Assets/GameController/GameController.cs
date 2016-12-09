using UnityEngine;
using System.Collections;

[AddComponentMenu("LuxVertigo/GameController")]
public class GameController : MonoBehaviour {

    #region Properties

    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float scrollSpeed = 10;

    public bool invertX, invertY;

    public float screenCenterMargin = 20;
    public float screenBorderMargin = 10;
    public float zoomSpeed, zoomStopSpeed;
    
    WorldWrapper wrapper;

    float pitch = 0.0f;
    float yaw = 0.0f;
    float zoomValue;
    Vector2 momentum;

    bool isDragging;
    WorldSpaceCursor cursor;

    #endregion

    #region MonoBehaviour Methods

    void Start() {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.y;
        yaw = angles.x;
        wrapper = WorldWrapper.singleton;

        cursor = FindObjectOfType<WorldSpaceCursor>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnApplicationFocus() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update() {
        KeyboardControls();
        MouseControls();

        // Apply Rotation
        if (invertY) pitch *= -1;
        if (!invertX) yaw *= -1;
        wrapper.Rotate(new Vector3(yaw, pitch, 0));
    }

    #endregion

    #region Input Controls

    void KeyboardControls() {
        if (Input.GetAxis("Horizontal") != 0)
            pitch = Input.GetAxis("Horizontal") * xSpeed * 0.02f;

        if (Input.GetAxis("Vertical") != 0)
            wrapper.Zoom(Input.GetAxis("Vertical"));
    }

    void MouseControls() {
        LeftClickControls();
        ScrollWheelControls();
        RightClickControls();
        MousePositionZoom();
    }

    bool noCursor;
    void LeftClickControls() {
        if (Input.GetMouseButtonUp(0) && noCursor) {
            StartCoroutine(Momentum());
            isDragging = false;
        }

        if (Input.GetMouseButtonDown(0) && Mouse.hover == null && Mouse.holding == null) {
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging && Mouse.hover == null && Mouse.holding == null) {
            StopAllCoroutines();

            pitch = Input.GetAxis("Mouse X") * xSpeed * 0.02f * -1;
            yaw = Input.GetAxis("Mouse Y") * ySpeed * 0.02f * -1;

            momentum.x = Input.GetAxis("Mouse X");
            momentum.y = Input.GetAxis("Mouse Y");
            noCursor = true;
            cursor.gameObject.SetActive(false);

        } else {
            noCursor = false;
            cursor.gameObject.SetActive(true);
        }
    }

    void MousePositionZoom() {
        if (!isDragging && (Input.mousePosition.x > (Screen.width / 2) - screenCenterMargin && Input.mousePosition.x < (Screen.width / 2) + screenCenterMargin
            && Input.mousePosition.y > (Screen.height / 2) - screenCenterMargin && Input.mousePosition.y < (Screen.height / 2) + screenCenterMargin)) {
            zoomValue += zoomValue >= 1 ? 0 : Time.deltaTime * zoomSpeed;
        } else if (zoomValue > 0) {
            zoomValue -= Time.deltaTime * zoomStopSpeed;
            if (zoomValue < 0) zoomValue = 0;
        }

        if (!isDragging && (Input.mousePosition.x < screenBorderMargin || Input.mousePosition.y < screenBorderMargin ||
            Input.mousePosition.x > Screen.width - screenBorderMargin || Input.mousePosition.y > Screen.height - screenBorderMargin)) {
            zoomValue -= zoomValue <= -1 ? 0 : Time.deltaTime * zoomSpeed;
        } else if (zoomValue < 0) {
            zoomValue += Time.deltaTime * zoomStopSpeed;
            if (zoomValue > 0) zoomValue = 0;
        }
        wrapper.Zoom(zoomValue);
    }

    /// <summary>
    /// Controls using the mouse scroll wheel and mouse middle click. Used to Zoom.
    /// </summary>
    void ScrollWheelControls() {
        if (Input.GetMouseButton(2) && Input.GetAxis("Mouse Y") != 0)
            wrapper.Zoom(Input.GetAxis("Mouse Y"));

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            wrapper.Zoom(Input.GetAxis("Mouse ScrollWheel") * scrollSpeed);
    }

    /// <summary>
    /// Controls using the mouse right click. Used to break links.
    /// </summary>
    void RightClickControls() {
        if (Input.GetMouseButtonDown(1) && Mouse.hover == null)
            Mouse.breakLinkMode = true;

        if (Input.GetMouseButtonUp(1)) {
            if (Mouse.hover == null) Mouse.BreakLink();
            Mouse.breakLinkMode = false;
        }
    }

    #endregion

    IEnumerator Momentum() {
        float duration = 4;

        for (float i = 0; i < duration; i += Time.deltaTime) {

            float t = i / duration;
            pitch = Mathf.Lerp(-momentum.x, 0, t);
            yaw   = Mathf.Lerp(-momentum.y, 0, t);

            yield return null;
        }
        pitch = yaw = 0;
    }
}