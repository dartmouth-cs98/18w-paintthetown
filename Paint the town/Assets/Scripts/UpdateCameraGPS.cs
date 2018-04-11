using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrld;
using Wrld.Space;
using System.Text.RegularExpressions;
using Wrld.Resources.Buildings;
using System;
using UnityEngine;
using UnityEngine.Networking;

// based on tutorials at https://docs.unity3d.com/ScriptReference/LocationService.Start.html and https://wrld3d.com/unity/latest/docs/examples/moving-the-camera/
// based on tutorial at https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom

public class UpdateCameraGPS : MonoBehaviour {

    public bool isUnityRemote;
    public Camera setCam;
    public float zoomSpeed = .5f; // speed to zoom in or out at
    private double distance = 300.00; // height in Wrld3d api distance terms
    public double uMinLng;
    public double uMinLat;
    public double uMaxLng;
    public double uMaxLat;
    public string[] parsingString;
    public Material highlightMaterial;

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

        // Set the camera for the Wrld3d map
        Api.Instance.CameraApi.SetControlledCamera(setCam);

        InvokeRepeating("updateMap", 2.0f, 1.0f);


    }

    public void updateMap(){


      double lat = Input.location.lastData.latitude;
      double longe = Input.location.lastData.longitude;

      uMinLng = (longe - .003);
      uMinLat = (lat - .003);
      uMaxLng = (longe + .003);
      uMaxLat = (lat + .003);

      StartCoroutine("sendUpdateBoundingBox");
    }

    public IEnumerator sendUpdateBoundingBox(){

      Hashtable header = new Hashtable();
      header.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));


      //centroidLat
      //team
      WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings?bbox[0]=" + uMinLat + "&bbox[1]=" + uMinLng + "&bbox[2]=" + uMaxLat + "&bbox[3]=" + uMaxLng + "&extraFields[0]=centroidLng&extraFields[1]=centroidLat&extraFields[2]=team&extraFields[3]=baseAltitude&extraFields[4]=topAltitude", null, header);
      yield return www;
      if (www.error != null)
      {
        print("Error downloading: " + www.error);
      } else {

        //SET BUILDING COLORS HERE
        //print("WWW " + www.text);
        parsingString = Regex.Split(www.text, @"[,:{}]+");

        // for(int x = 0; x < parsingString.Length; x ++){
        //   print(parsingString[x]);
        // }

        for(int x = 3; x < parsingString.Length - 14; x = x + 14){
          if(parsingString[x].Trim('"') == "team"){


            //print("BUILDIGN TOPALT " + parsingString[x + 11]);

            double lat = Convert.ToDouble(parsingString[x + 7]) + 0.0000000000001;
            double lnge = Convert.ToDouble(parsingString[x + 5]) + 0.0000000000001;
            double alt = Convert.ToDouble(parsingString[x + 9]);

            // print("BUILDING COLOR: " +parsingString[x + 1]);
            // print("BUILDING ID: " + parsingString[x + 3]);
            // print("BUILDING LNG: " + lnge);
            // print("BUILDING LAT: " + lat);
            // print("BUILDING BASEALT " + alt);

            var buildingLocation = LatLongAltitude.FromDegrees(lnge, lat, alt);

            if(parsingString[x + 1].Trim('"') == "red"){
              highlightMaterial.color = Color.red;
            } else if(parsingString[x + 1].Trim('"') == "blue"){
              highlightMaterial.color = Color.blue;
            }

            Api.Instance.BuildingsApi.HighlightBuildingAtLocation(buildingLocation, highlightMaterial, OnHighlightReceived);
          }
        }
      }
    }

    void test(bool success, Highlight highlight)
    {
        if (success)
        {
            //StartCoroutine(ClearHighlight(highlight));
        } else{
          //
        }
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {
          // print("HELLO?");
            //StartCoroutine(ClearHighlight(highlight));
        } else{
          print("NOOOOOO");
        }
    }

    void Update () {

        // handle pinch to zoom
        // if there are two touches
        if (Input.touchCount == 2)
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

            // if the camera is orthographic
            //if (setCam.orthographic)
            //{
            //    // change the orthographic size based on the change in distance between the touches
            //    setCam.orthographicSize += distMagnitudeDiff * zoomSpeed;

            //    // make sure the orthographic size never goes negative
            //    setCam.orthographicSize = Mathf.Max(setCam.orthographicSize, 0.1f);
            //}

            // otherwise the camera is in perspective mode
            //else
            //{
            //    // change the field of view based on the change in distance between the touches
            //    setCam.fieldOfView += distMagnitudeDiff * zoomSpeed;

            //    // clamp the fov to make sure it's between 0 and 180.
            //    setCam.fieldOfView = Mathf.Clamp(setCam.fieldOfView, 0.1f, 179.9f);
            //}

            distance += distMagnitudeDiff * zoomSpeed;
        }

        // take the lastData and put it into the camera api from wrld3d
        var currentLocation = LatLong.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);

        Api.Instance.CameraApi.AnimateTo(currentLocation, distance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
        Api.Instance.StreamResourcesForCamera(setCam);
        Api.Instance.Update();

	}
}
