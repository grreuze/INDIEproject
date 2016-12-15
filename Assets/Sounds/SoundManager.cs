﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager singleton;
    public AudioClip[] starSound;


    void Awake()
    {
        singleton = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Play(AudioClip audioClip, float volume, AudioSource audioSource)
    {
        audioSource.PlayOneShot(audioClip, volume);
    }

}