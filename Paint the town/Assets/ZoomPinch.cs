using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based on tutorial at https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom

public class ZoomPinch : MonoBehaviour {

    public float zoomSpeed = .5f; // speed to zoom in or out at 

	// start removed, nothing to initialize
	
	// Update is called once per frame
    // we check for two touches, then to make sure the user is moving those points futher apart or closer together
	void Update () {
		
        // if there are two touches
        if (Input.touchCount == 2)
        {
            // store them 
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // find the positions of those touches in the previous frame
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // find magnitude of the distance between the touches, both current and in the previous frame
            float touchDistanceMag = (touch0.position - touch1.position).magnitude;
            float prevTouchDistanceMag = (touch0PrevPos - touch1PrevPos).magnitude;

            // find the difference in magnitude between the two distances
            float distMagnitudeDiff = prevTouchDistanceMag - touchDistanceMag;


        }
    }
}
