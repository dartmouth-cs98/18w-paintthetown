﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_controller : MonoBehaviour {

    // sound stuff
    AudioSource pAudioSource;

    // Use this for initialization
    void Start () {
        pAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Fire1"))
        {

            // play the sound
            pAudioSource.Play();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            print("Got into the stop!");

            pAudioSource.Stop();
        }
	}
}
