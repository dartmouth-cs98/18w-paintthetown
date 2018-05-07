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

    public bool isUnityRemote;
    public Camera povCam;
    public Camera setCam;
    public float zoomSpeed = 1.0f; // speed to zoom in or out at
    private double distance = 300.00; // height in Wrld3d api distance terms

    public LatLong centerMapLatLong;
    public double centerMapDistance;
    public bool mapCentered;

    private const float lowPassFilterFactor = 0.2f;
    private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);
    private readonly Quaternion landscapeRight = Quaternion.Euler(0, 0, 90);
    private readonly Quaternion landscapeLeft = Quaternion.Euler(0, 0, -90);
    private readonly Quaternion upsideDown = Quaternion.Euler(0, 0, 180);
    private Quaternion cameraBase = Quaternion.identity;
    private Quaternion calibration = Quaternion.identity;
    private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);
    private Quaternion baseOrientationRotationFix = Quaternion.identity;
    private Quaternion referenceRotation = Quaternion.identity;

    public int myGlobalInt;

    private LatLongAltitude lastLocationOfCamera;

    // particle system stuff
    public ParticleSystem pLauncher;

    IEnumerator Start()
    {
        mapCentered = false;
        Input.gyro.enabled = true;
        ResetBaseOrientation();
        UpdateCalibration(true);
        UpdateCameraBaseRotation(true);
        RecalculateReferenceRotation();
        myGlobalInt = 0;

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

    public void centerCam(){
        Api.Instance.CameraApi.AnimateTo(centerMapLatLong, centerMapDistance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
    }

	  private void UpdateCalibration(bool onlyHorizontal)
    {
        if (onlyHorizontal)
        {
            var fw = (Input.gyro.attitude) * (-Vector3.forward);
            fw.z = 0;
            if (fw == Vector3.zero)
            {
                calibration = Quaternion.identity;
            }
            else
            {
                calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
            }
        }
        else
        {
            calibration = Input.gyro.attitude;
        }
    }

    private void UpdateCameraBaseRotation(bool onlyHorizontal)
    {
        if (onlyHorizontal)
        {
            var fw = transform.forward;
            fw.y = 0;
            if (fw == Vector3.zero)
            {
                cameraBase = Quaternion.identity;
            }
            else
            {
                cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
            }
        }
        else
        {
            cameraBase = transform.rotation;
        }
    }

    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    private Quaternion GetRotFix()
    {
		if (Screen.orientation == ScreenOrientation.Portrait)
			return Quaternion.identity;

		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
			return landscapeLeft;

		if (Screen.orientation == ScreenOrientation.LandscapeRight)
			return landscapeRight;

		if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
			return upsideDown;
		return Quaternion.identity;
    }

    private void ResetBaseOrientation()
    {
        baseOrientationRotationFix = GetRotFix();
        baseOrientation = baseOrientationRotationFix * baseIdentity;
    }

    private void RecalculateReferenceRotation()
    {
        referenceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
    }


void Update () {

        // handle pinch to zoom
        // if there are two touches
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

        if(myGlobalInt > 600){
          myGlobalInt = 0;
        }

        var currentLatLong = LatLong.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);
        var currentLocation = LatLongAltitude.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude, myGlobalInt);

        Api.Instance.CameraApi.GeographicToWorldPoint(currentLocation,setCam);
        Api.Instance.StreamResourcesForCamera(setCam);
        Api.Instance.Update();

        centerMapLatLong = currentLatLong;
        centerMapDistance = distance;

        // Runs on first time user GPS is received
        if (!mapCentered && currentLatLong.GetLatitude() != 0.0f && currentLatLong.GetLongitude() != 0.0f)
        {
            mapCentered = true;
            Api.Instance.CameraApi.AnimateTo(centerMapLatLong, centerMapDistance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
            lastLocationOfCamera = LatLongAltitude.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude, Input.location.lastData.altitude);
        }

        // Create temp Vector 3, dynamically change its height
        RaycastHit hit;
        Vector3 tempPOVposition = new Vector3(setCam.transform.position.x, 400, setCam.transform.position.z);
        if (Physics.Raycast(tempPOVposition,Vector3.down,out hit, 600))
        {
            tempPOVposition.y = hit.point.y + 15f;
        }

        // Set POV cam to the temp Vector 3 created above
        pLauncher.transform.SetPositionAndRotation(setCam.transform.position, setCam.transform.rotation);
        povCam.transform.position = tempPOVposition;
        povCam.transform.rotation = Quaternion.Slerp(povCam.transform.rotation,cameraBase * (ConvertRotation(referenceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor);

        print("x: " + setCam.transform.position.x + "  y: " + setCam.transform.position.y + "  z: " + setCam.transform.position.z + "  g: " + myGlobalInt);
        myGlobalInt++;
	}
}
