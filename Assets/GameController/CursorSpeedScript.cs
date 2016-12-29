using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSpeedScript : MonoBehaviour {

    public float speed;
    Vector3 currVel;

    void Start()
    {
        StartCoroutine(CalcVelocity());
    }

    IEnumerator CalcVelocity()
    {
        Vector3 prevPos;
        while (Application.isPlaying)
        {
            // Position at frame start
            prevPos = transform.position;
            // Wait till it the end of the frame
            yield return new WaitForEndOfFrame();
            currVel = (prevPos - transform.position) / Time.deltaTime;
            speed = currVel.magnitude;
        }
    }

    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        speed = currVel.magnitude;
    }
}
