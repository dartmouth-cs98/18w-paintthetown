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
    public Camera povCam;
    public Camera setCam;
    public float zoomSpeed = .5f; // speed to zoom in or out at
    private double distance = 300.00; // height in Wrld3d api distance terms
    public double uMinLng;
    public double uMinLat;
    public double uMaxLng;
    public double uMaxLat;
    public string[] parsingString;
    public Material highlightMaterialRed;
    public Material highlightMaterialBlue;
    public ArrayList poiList;
    public GameObject prefab;
    private float time = 1f;
    public GameObject toBeDestroyedMarker;
    public GameObject[] listOfToBeDestroyed;
    public LatLong centerMapLatLong;
    public double centerMapDistance;
    public bool mapCentered;

    // particle system stuff
    public ParticleSystem pLauncher;

    HashSet<List<string>> oldBuildings = new HashSet<List<string>>();
    HashSet<List<string>> newBuildings = new HashSet<List<string>>();

    IEnumerator Start()
    {
        mapCentered = false;
        Input.gyro.enabled = true;

        poiList = new ArrayList();
        poiList.Add("71a5f824a0dc35526a4b13078541adee");
        highlightMaterialRed.color = Color.red;
        highlightMaterialBlue.color = Color.blue;

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

        InvokeRepeating("updateMap", 2.0f, time);

        var currentLatLong = LatLong.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);
        //Api.Instance.CameraApi.AnimateTo(currentLatLong, distance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
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

        //print("WWW " + www.text);
        parsingString = Regex.Split(www.text, @"[,:{}]+");

        // for(int y = 3; y < parsingString.Length; y++){
        //   print(parsingString[y]);
        // }

        for(int x = 3; x < parsingString.Length - 14; x = x + 14){

          double lnge;
          double topAlt;
          string stringLnge;
          string stringTopAlt;
          string id;
          string stringLat = parsingString[x + 7];
          double lat = Convert.ToDouble(parsingString[x + 7]) + 0.0000000000001;

          if(parsingString[x].Trim('"') == "team"){

            stringLnge = parsingString[x + 9];
            lnge = Convert.ToDouble(parsingString[x + 9]) + 0.0000000000001;

            stringTopAlt = parsingString[x + 3];
            topAlt = Convert.ToDouble(parsingString[x + 3]);

            id = parsingString[x + 11].Trim('"');

            double alt = Convert.ToDouble(parsingString[x + 5]);
            var buildingLocation = LatLongAltitude.FromDegrees(lnge, lat, alt);

            if(parsingString[x + 1].Trim('"') == "red"){
              Api.Instance.BuildingsApi.HighlightBuildingAtLocation(buildingLocation, highlightMaterialRed, OnHighlightReceived);

            } else if(parsingString[x + 1].Trim('"') == "blue"){
              Api.Instance.BuildingsApi.HighlightBuildingAtLocation(buildingLocation, highlightMaterialBlue, OnHighlightReceived);
            }
          }else{
            stringLnge = parsingString[x + 5];
            lnge = Convert.ToDouble(parsingString[x + 5]) + 0.0000000000001;

            stringTopAlt = parsingString[x + 11];
            topAlt = Convert.ToDouble(parsingString[x + 11]);

            id = parsingString[x + 3].Trim('"');
          }

          foreach(string idNum in poiList){
            if (idNum.Equals(id)){
              //print("YAY");
              var v1  = new List<string>();
              v1.Add(id);
              v1.Add(stringLat);
              v1.Add(stringLnge);
              v1.Add(stringTopAlt);
              //id, stringLat, stringLnge, stringTopAlt
              newBuildings.Add(v1);
            }
          }
        }

        var toLoad = new HashSet<List<string>>(newBuildings);
        var toDestroy = new HashSet<List<string>>(oldBuildings);

        toDestroy.ExceptWith(newBuildings);

        toLoad.ExceptWith(oldBuildings);

        foreach (List<string> placement in toDestroy){

          var boxLocation = LatLongAltitude.FromDegrees(Convert.ToDouble(placement[2]), Convert.ToDouble(placement[1]), Convert.ToDouble(placement[3]) + 10);
          StartCoroutine(destroy(placement[0], boxLocation));
        }

        foreach (List<string> placement in toLoad){
          var boxLocation = LatLongAltitude.FromDegrees(Convert.ToDouble(placement[2]), Convert.ToDouble(placement[1]), Convert.ToDouble(placement[3]) + 10);
          print(boxLocation.GetLatitude());
          print(placement[0]);
          StartCoroutine(MakeBox(placement[0], boxLocation));
        }

        oldBuildings = newBuildings;

      }
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {
          //it did work
        } else{
          //it didn't work
        }
    }

    IEnumerator MakeBox(string id, LatLongAltitude latLongAlt){
      var viewpoint = Wrld.Api.Instance.CameraApi.GeographicToViewportPoint(latLongAlt);

      var worldpoint = setCam.ViewportToWorldPoint(viewpoint);


      GameObject cloneMarker = Instantiate(prefab, worldpoint, Quaternion.Euler(45, 0, 0)) as GameObject;;

      cloneMarker.name = id;

      yield return null;
    }

    IEnumerator destroy(string id, LatLongAltitude latLongAlt){
      toBeDestroyedMarker = GameObject.Find(id);
      Destroy(toBeDestroyedMarker);
      yield return null;
    }

    public void centerCam(){
        print("'ELLO'");
        Api.Instance.CameraApi.AnimateTo(centerMapLatLong, centerMapDistance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
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

        var currentLatLong = LatLong.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);
        var currentLocation = LatLongAltitude.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude, 0);

        Api.Instance.CameraApi.GeographicToWorldPoint(currentLocation,setCam);

        Api.Instance.StreamResourcesForCamera(setCam);
        Api.Instance.Update();

        povCam.transform.position = new Vector3(setCam.transform.position.x, 160, setCam.transform.position.z);

        pLauncher.transform.SetPositionAndRotation(setCam.transform.position, setCam.transform.rotation);
        

        centerMapLatLong = currentLatLong;
        centerMapDistance = distance;

        if(!mapCentered && currentLatLong.GetLatitude() != 0.0f && currentLatLong.GetLongitude() != 0.0f){
          print("GOVNA");
          mapCentered = true;
          Api.Instance.CameraApi.AnimateTo(centerMapLatLong, centerMapDistance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
        }

        //print("---------------------------------");
        //print(setCam.transform.position);
        //print(povCam.transform.position);

        povCam.transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, -Input.gyro.rotationRateUnbiased.z);

	}
}
