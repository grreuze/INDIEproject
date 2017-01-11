using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bg_adapt_to_screen : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Camera camera = Camera.main;
        float height;
        float width;
        height = (1 / camera.aspect) * (15f*(16f/9f)); //makes 15 for 16:9 aspect ratio
        //Debug.Log(height);
        width = camera.aspect * (25f*(9f/16f)); //makes 25 for 16:9 aspect ratio
        //Debug.Log(width);
        transform.localScale = new Vector3(height + 2f, 1, width);
    }

}
