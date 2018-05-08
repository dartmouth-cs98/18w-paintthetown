using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Wrld;
using Wrld.Space;
using Wrld.Resources.Buildings;

using UnityEngine;
using UnityEngine.Networking;

// based on tutorials at https://docs.unity3d.com/ScriptReference/LocationService.Start.html and https://wrld3d.com/unity/latest/docs/examples/moving-the-camera/
// based on tutorial at https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom

public class UpdateCameraGPS : MonoBehaviour {

    public Camera povCam;
    public Camera setCam;

    public float zoomSpeed = 1.0f; // speed to zoom in or out at
    private double distance = 300.00; // height in Wrld3d api distance terms
    public bool isUnityRemote;
    public bool mapCentered;

    IEnumerator Start()
    {
        mapCentered = false;

        // wait for the unity remote to connect, if applicable
        if (isUnityRemote)
        {
            print("waiting for remote!");
            yield return new WaitForSeconds(5);
        }

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser) {
            print("location is not enabled!");
            yield break;
        }

        // Start service before querying location
        Input.location.Start((float)6.0, (float)6.0);
        Input.compass.enabled = true;

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Set the camera for the Wrld3d map
        Api.Instance.CameraApi.SetControlledCamera(setCam);
    }

void Update () {
        if (Input.touchCount == 2 && setCam.enabled)
        {
            print("Pinch gesture detected!");

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

        LatLong currentLatLong = LatLong.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);
        LatLongAltitude currentLatLongAlt = LatLongAltitude.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude, 0);

        Api.Instance.CameraApi.GeographicToWorldPoint(currentLatLongAlt,setCam);
        Api.Instance.StreamResourcesForCamera(setCam);
        Api.Instance.Update();

        // Runs on first time user GPS is received
        if (!mapCentered && currentLatLong.GetLatitude() != 0.0f && currentLatLong.GetLongitude() != 0.0f)
        {
            mapCentered = true;
            Api.Instance.CameraApi.AnimateTo(currentLatLong, distance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
        }

        print("x: " + setCam.transform.position.x + "  y: " + setCam.transform.position.y + "  z: " + setCam.transform.position.z );
	}
}
