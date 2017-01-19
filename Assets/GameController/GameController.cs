using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("LuxVertigo/GameController")]
public class GameController : MonoBehaviour {

    #region Properties

    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float scrollSpeed = 10;

    public bool invertX, invertY;

    public float screenCenterMargin = 20;
    public float screenBorderMargin = 10;
    public float zoomMaxSpeed, zoomIncrementation, zoomDecrementation;
    public string[] partition = new string[] { }; //{ "Mi", "Mi", "Fa" ,"Sol", "Sol" ,"Fa" ,"Mi" ,"Re", "Do", "Do" ,"Re", "Mi", "Mi" ,"Re" ,"Re", "Mi", "Mi" ,"Fa" ,"Sol", "Sol", "Fa" ,"Mi", "Re" ,"Do", "Do" ,"Re", "Mi" ,"Re" ,"Do" };
    // Ode à la joie : "Mi", "Mi", "Fa" ,"Sol", "Sol" ,"Fa" ,"Mi" ,"Re", "Do", "Do" ,"Re", "Mi", "Mi" ,"Re" ,"Re", "Mi", "Mi" ,"Fa" ,"Sol", "Sol", "Fa" ,"Mi", "Re" ,"Do", "Do" ,"Re", "Mi" ,"Re" ,"Do" 

     

    [SerializeField]
    AnimationCurve zoomExponent;

    public bool gameStarted = false;
    private Star previousStar;
    AudioSource mainAudio;
    
    WorldWrapper wrapper;

    float pitch = 0.0f;
    float yaw = 0.0f;
    float zoomValue;
    Vector2 momentum;

    bool isDragging;
    WorldSpaceCursor cursor;

    bool PartitionAdded = false;

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

        mainAudio = Camera.main.GetComponent<AudioSource>();
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

        if(Input.GetKey(KeyCode.P) && PartitionAdded == false)
        {
            PartitionAdded = true;
            AddPartition();
        }

    }

    #endregion

    #region Input Controls

    void KeyboardControls() {
        if (Input.GetAxis("Horizontal") != 0) {
            StopAllCoroutines();
            pitch = Input.GetAxis("Horizontal") * xSpeed * 0.02f;
        }

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
            && Input.mousePosition.y > (Screen.height / 2) - screenCenterMargin && Input.mousePosition.y < (Screen.height / 2) + screenCenterMargin))
        {
            zoomValue += zoomValue >= zoomMaxSpeed ? 0 : Time.deltaTime * zoomIncrementation;
            PlayZoomSound();
        }
        else if (zoomValue > 0)
        {
            zoomValue -= Time.deltaTime * zoomDecrementation;
            if (zoomValue < 0) zoomValue = 0;
        }

        if (!isDragging && (Input.mousePosition.x < screenBorderMargin || Input.mousePosition.y < screenBorderMargin ||
            Input.mousePosition.x > Screen.width - screenBorderMargin || Input.mousePosition.y > Screen.height - screenBorderMargin))
        {
            zoomValue -= zoomValue <= -zoomMaxSpeed ? 0 : Time.deltaTime * zoomIncrementation;
            PlayZoomSound();
        }
        else if (zoomValue < 0)
        {
            zoomValue += Time.deltaTime * zoomDecrementation;
            if (zoomValue > 0) zoomValue = 0;
        }
        mainAudio.volume = Mathf.Clamp(Mathf.Abs(zoomValue), 0.2f,1f);
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

    void PlayZoomSound()
    {
        if(!mainAudio.isPlaying && gameStarted){
            SoundManager.singleton.Play(SoundManager.singleton.startSound, 1f, mainAudio);
        }
    }

    /*                          We may need this if we don't want ambiant sound
    void StopZoomSound()
    {
        if (mainAudio.isPlaying && mainAudio.volume == 0)
        {
            mainAudio.Stop();
        }
    }
    */
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

    void AddPartition()
    {
        Chroma tempChroma = new Chroma();
        int i = 0;
        Star[] tempStarList = GameObject.Find("WorldInstance [0]").GetComponent<WorldInstance>().stars;
        foreach (Star star in tempStarList)
        {
            Debug.Log("Star : " + star);
            star.existence = Existence.unique;
            if(i < partition.Length)tempChroma = NoteToChroma(partition[i]);
            star.chroma.r = tempChroma.r;
            star.chroma.g = tempChroma.g;
            star.chroma.b = tempChroma.b;
            star.ApplyChroma();
            if (previousStar != null && i < partition.Length) star.AutoLink(previousStar.id, 0, 0);
            previousStar = star;
            i++;
        }
    }

    Chroma NoteToChroma(string Note)
    {
        Chroma chroma;
        switch (Note)
        {
            case "Do":
                chroma.r = 2;
                chroma.g = 0;
                chroma.b = 1;
                return chroma;
            case "Re":
                chroma.r = 0;
                chroma.g = 0;
                chroma.b = 0;
                return chroma;
            case "Mi":
                chroma.r = 1;
                chroma.g = 0;
                chroma.b = 0;
                return chroma;
            case "Fa":
                chroma.r = 2;
                chroma.g = 1;
                chroma.b = 0;
                return chroma;
            case "Sol":
                chroma.r = 1;
                chroma.g = 1;
                chroma.b = 0;
                return chroma;
            case "La":
                chroma.r = 1;
                chroma.g = 2;
                chroma.b = 0;
                return chroma;
            case "Si":
                chroma.r = 0;
                chroma.g = 1;
                chroma.b = 0;
                return chroma;
            case "Fa1":
                chroma.r = 1;
                chroma.g = 0;
                chroma.b = 2;
                return chroma;
            case "Sol1":
                chroma.r = 0;
                chroma.g = 0;
                chroma.b = 1;
                return chroma;
            case "La1":
                chroma.r = 0;
                chroma.g = 1;
                chroma.b = 1;
                return chroma;
            case "Si1": //rose
                chroma.r = 3;
                chroma.g = 1;
                chroma.b = 2;
                return chroma;
            default:
                chroma.r = 0;
                chroma.g = 0;
                chroma.b = 0;
                return chroma;
        }
    }
}