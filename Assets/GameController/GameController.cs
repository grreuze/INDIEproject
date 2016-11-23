using UnityEngine;
using System.Collections;

[AddComponentMenu("LuxVertigo/GameController")]
public class GameController : MonoBehaviour {
    
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float scrollSpeed = 10;

    public bool invertX, invertY;

    public float screenCenterMargin = 20;
    public float screenBorderMargin = 10;
    public float zoomSpeed, zoomStopSpeed;

    public Prism tempPrefabPrism;

    WorldWrapper wrapper;

    float pitch = 0.0f;
    float yaw = 0.0f;
    float zoomValue;
    Vector2 momentum;

    void Start() {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.y;
        yaw = angles.x;
        wrapper = WorldWrapper.singleton;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update() {
        // Keyboard Controls
        if (Input.GetAxis("Horizontal") != 0) {
            pitch = Input.GetAxis("Horizontal") * xSpeed * 0.02f;
        }

        if (Input.GetAxis("Vertical") != 0)
            wrapper.Zoom(Input.GetAxis("Vertical"));

        // Mouse Controls

        if (Input.GetMouseButtonUp(0) && !Cursor.visible)
            StartCoroutine(Momentum());

        if (Input.GetMouseButton(0) && Mouse.hover == null && Mouse.holding == null) {
            StopAllCoroutines();

            pitch = Input.GetAxis("Mouse X") * xSpeed * 0.02f * -1;
            yaw = Input.GetAxis("Mouse Y") * ySpeed * 0.02f * -1;

            momentum.x = Input.GetAxis("Mouse X");
            momentum.y = Input.GetAxis("Mouse Y");

            Cursor.visible = false;

        } else {
            Cursor.visible = true;

            if (Input.mousePosition.x > (Screen.width / 2) - screenCenterMargin && Input.mousePosition.x < (Screen.width / 2) + screenCenterMargin
                && Input.mousePosition.y > (Screen.height / 2) - screenCenterMargin && Input.mousePosition.y < (Screen.height / 2) + screenCenterMargin) {
                zoomValue += zoomValue >= 1 ? 0 : Time.deltaTime * zoomSpeed;
            } else if (zoomValue > 0) {
                zoomValue -= Time.deltaTime * zoomStopSpeed;
                if (zoomValue < 0) zoomValue = 0;
            }

            if (Input.mousePosition.x < screenBorderMargin || Input.mousePosition.y < screenBorderMargin ||
                Input.mousePosition.x > Screen.width - screenBorderMargin || Input.mousePosition.y > Screen.height - screenBorderMargin) {
                zoomValue -= zoomValue <= -1 ? 0 : Time.deltaTime * zoomSpeed;
            } else if (zoomValue < 0) {
                zoomValue += Time.deltaTime * zoomStopSpeed;
                if (zoomValue > 0) zoomValue = 0;
            }
            wrapper.Zoom(zoomValue);
        }


        //Creates a Prism with middle click, TO DELETE
        if(Input.GetMouseButtonUp(2)) {
            Prism newPrism = (Prism)Instantiate(tempPrefabPrism, Input.mousePosition, Quaternion.identity);
            newPrism.transform.parent = wrapper.currentInstance.transform;
        }

        if (Input.GetMouseButton(2) && Input.GetAxis("Mouse Y") != 0)
            wrapper.Zoom(Input.GetAxis("Mouse Y"));
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            wrapper.Zoom(Input.GetAxis("Mouse ScrollWheel") * scrollSpeed);

        if (Input.GetMouseButtonDown(1) && Mouse.hover == null)
            Mouse.breakLinkMode = true;

        if (Input.GetMouseButtonUp(1)) {
            if (Mouse.hover == null) Mouse.BreakLink();
            Mouse.breakLinkMode = false;
        }

        // Apply Rotation
        if (invertY) pitch *= -1;
        if (!invertX) yaw *= -1;
        wrapper.Rotate(new Vector3(yaw, pitch, 0));
    }

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