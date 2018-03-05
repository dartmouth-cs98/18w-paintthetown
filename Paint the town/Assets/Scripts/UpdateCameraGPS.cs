using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrld;
using Wrld.Space;

// based on tutorials at https://docs.unity3d.com/ScriptReference/LocationService.Start.html and https://wrld3d.com/unity/latest/docs/examples/moving-the-camera/



public class UpdateCameraGPS : MonoBehaviour {

    public bool isUnityRemote;
    public Camera setCam;

    IEnumerator Start()
    {
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
            // print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        // Input.location.Stop();

        // Set the camera for the Wrld3d map
        Api.Instance.CameraApi.SetControlledCamera(setCam);
    }

    // Update is called once per frame
    void Update () {

        //print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

        // take the lastData and put it into the camera api from wrld3d
        var currentLocation = LatLong.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);
        //print(Input.compass.trueHeading);
        Api.Instance.CameraApi.AnimateTo(currentLocation, distanceFromInterest: 300, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
        Api.Instance.StreamResourcesForCamera(setCam);
        Api.Instance.Update();
	}
}
