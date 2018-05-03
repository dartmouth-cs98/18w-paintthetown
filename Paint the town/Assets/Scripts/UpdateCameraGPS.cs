using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrld;
using Wrld.Space;
using System.Text.RegularExpressions;
using Wrld.Resources.Buildings;
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
    private float time = 10f;
    public GameObject toBeDestroyedMarker;
    public GameObject[] listOfToBeDestroyed;
    public LatLong centerMapLatLong;
    public double centerMapDistance;
    public bool mapCentered;

    // particle system stuff
    public ParticleSystem pLauncherPOV;

    double topAlt;
    string stringLnge;
    double lnge;
    string stringTopAlt;
    string id;
    string stringLat;
    double lat;
    double alt;
    string team;
    string r;
    string g;
    string b;

    HashSet<List<string>> oldBuildings = new HashSet<List<string>>();
    HashSet<List<string>> newBuildings = new HashSet<List<string>>();

    HashSet<List<string>> oldBuildingsColor = new HashSet<List<string>>();
    HashSet<List<string>> newBuildingsColor = new HashSet<List<string>>();

    IEnumerator Start()
    {
        mapCentered = false;
        Input.gyro.enabled = true;

        poiList = new ArrayList();
        poiList.Add("71a5f824a0dc35526a4b13078541adee");

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

      //****************************************
      //request information on buildings within a bounding box
      // uses:
      // * minLat
      // * minLong
      // * maxLat
      // * maxLong
      // retrieves:
      // * centroidLng
      // * centroidLat
      // * baseAltitude
      // * topAltitude
      // * team
      // * ownership
      // * rgb
      //****************************************

      Hashtable header = new Hashtable();
      header.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));

      WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings?bbox[0]=" + uMinLat + "&bbox[1]=" + uMinLng + "&bbox[2]=" + uMaxLat + "&bbox[3]=" + uMaxLng + "&extraFields[0]=centroidLng&extraFields[1]=centroidLat&extraFields[2]=team&extraFields[3]=baseAltitude&extraFields[4]=topAltitude&extraFields[5]=rgb", null, header);
      yield return www;
      if (www.error != null)
      {
        print("Error downloading: " + www.error);
      } else {

        print("WWW " + www.text);
        parsingString = Regex.Split(www.text, @"[,:{}]+");

        // for(int y = 3; y < parsingString.Length; y++){
        //   print(parsingString[y]);
        // }

        for(int x = 3; x < parsingString.Length - 14; x = x + 14){
          team = "";
          for(int y = 0; y < 14; y ++){

            if(parsingString[x + y].Trim('"') == "centroidLng"){
              stringLnge = parsingString[x + y + 1].Trim('"');
              lnge = Convert.ToDouble(parsingString[x + y + 1].Trim('"'));
            } else if (parsingString[x + y].Trim('"') == "centroidLat"){
              stringLat = parsingString[x + y + 1].Trim('"');
              lat = Convert.ToDouble(parsingString[x + y + 1].Trim('"'));
            } else if (parsingString[x + y].Trim('"') == "baseAltitude"){
              alt = Convert.ToDouble(parsingString[x + y + 1].Trim('"'));
            } else if (parsingString[x + y].Trim('"') == "id"){
              id = parsingString[x + y + 1].Trim('"');
            // } else if (parsingString[x + y].Trim('"') == "team" && parsingString[x + y - 4].Trim('"') == "rgb"){
            } else if (parsingString[x + y].Trim('"') == "name" && parsingString[x + y + 2].Trim('"') != "color"){
              team = parsingString[x + y + 1].Trim('"');
            } else if (parsingString[x + y].Trim('"') == "stringTopAlt"){
              stringTopAlt = parsingString[x + y + 1].Trim('"');
              topAlt = Convert.ToDouble(parsingString[x + y + 1].Trim('"'));
            } else if (parsingString[x + y].Trim('"') == "rgb"){
              r = parsingString[x + y + 1].Trim('[');
              g = parsingString[x + y + 2].Trim('"');
              b = parsingString[x + y + 3].Trim(']');
            }
          }

          //****************************************
          //add buildings to the new team color hashset
          // * ID
          // * Location
          // * RGB
          //****************************************

          if(team != ""){
            print("id " + id);
            print("stringlat " + stringLat);
            print("stringLnge " + stringLnge);
            print("alt " + alt);
            print("r " + r);
            print("g " + g);
            print("b " + b);
            print("team: " + team);
            var v0 = new List<string>();
            v0.Add(id);
            v0.Add(stringLat);
            v0.Add(stringLnge);
            v0.Add(Convert.ToString(alt));
            v0.Add(r);
            v0.Add(g);
            v0.Add(b);
            newBuildingsColor.Add(v0);
          }

          // *****************************************
          // add POIs to the new team color hashset
          // *****************************************

          foreach(string idNum in poiList){
            if (idNum.Equals(id)){
              var v1 = new List<string>();
              v1.Add(id);
              v1.Add(stringLat);
              v1.Add(stringLnge);
              v1.Add(stringTopAlt);
              newBuildings.Add(v1);
            }
          }
        }

        // ***********************************
        //BUILDING HIGHLIGHT DESTROY AND LOAD
        // ***********************************
        var toLoadColors = new HashSet<List<string>>(newBuildingsColor);
        var toDestroyColors = new HashSet<List<string>>(oldBuildingsColor);

        toDestroyColors.ExceptWith(newBuildingsColor);
        toLoadColors.ExceptWith(oldBuildingsColor);

        foreach (List<string> placement in toDestroyColors){
          StartCoroutine(destroyColor(placement[0]));
        }

        foreach (List<string> placement in toLoadColors){
          var boxLocation = LatLongAltitude.FromDegrees(Convert.ToDouble(placement[2]), Convert.ToDouble(placement[1]), Convert.ToDouble(placement[3]));
          //create RGB from the list
          Color color = new Color((float)Convert.ToDouble(placement[4]),(float)Convert.ToDouble(placement[5]),(float)Convert.ToDouble(placement[6]));
          StartCoroutine(MakeHighlight(placement[0], boxLocation, color));
        }

        oldBuildingsColor = newBuildingsColor;

        // ***********************************
        //BUILDING POI DESTROY AND LOAD
        // ***********************************
        var toLoad = new HashSet<List<string>>(newBuildings);
        var toDestroy = new HashSet<List<string>>(oldBuildings);

        toDestroy.ExceptWith(newBuildings);
        toLoad.ExceptWith(oldBuildings);

        foreach (List<string> placement in toDestroy){

          var boxLocation = LatLongAltitude.FromDegrees(Convert.ToDouble(placement[2]), Convert.ToDouble(placement[1]), Convert.ToDouble(placement[3]) + 10);
          StartCoroutine(destroy(placement[0]));
        }

        foreach (List<string> placement in toLoad){
          var boxLocation = LatLongAltitude.FromDegrees(Convert.ToDouble(placement[2]), Convert.ToDouble(placement[1]), Convert.ToDouble(placement[3]) + 10);
          StartCoroutine(MakeBox(placement[0], boxLocation));
        }

        oldBuildings = newBuildings;

      }
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {
          //things good happened
        } else{
          //print("i didn't do things");
        }
    }

    IEnumerator MakeHighlight(string id, LatLongAltitude latLongAlt, Color color){
      Material Highlight = new Material(highlightMaterialRed);
      Highlight.color = color;
      Api.Instance.BuildingsApi.HighlightBuildingAtLocation(latLongAlt, Highlight, OnHighlightReceived);
      Highlight.name = "highlight:" + id;
      yield return null;
    }

    IEnumerator destroyColor(string id){
      string material_name = "hightlight:" + id;
      Material toBeDestroyedHighlight = (Material)Resources.Load(material_name, typeof(Material));
      Destroy(toBeDestroyedHighlight);
      yield return null;
    }

    IEnumerator MakeBox(string id, LatLongAltitude latLongAlt){
      var viewpoint = Wrld.Api.Instance.CameraApi.GeographicToViewportPoint(latLongAlt);
      var worldpoint = setCam.ViewportToWorldPoint(viewpoint);
      GameObject cloneMarker = Instantiate(prefab, worldpoint, Quaternion.Euler(45, 0, 0)) as GameObject;;
      cloneMarker.name = id;
      yield return null;
    }

    IEnumerator destroy(string id){
      toBeDestroyedMarker = GameObject.Find(id);
      Destroy(toBeDestroyedMarker);
      yield return null;
    }

    public void centerCam(){
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

        povCam.transform.position = new Vector3(setCam.transform.position.x, 160, setCam.transform.position.z + 40);

        pLauncherPOV.transform.SetPositionAndRotation(povCam.transform.position, povCam.transform.rotation);


        centerMapLatLong = currentLatLong;
        centerMapDistance = distance;

        if(!mapCentered && currentLatLong.GetLatitude() != 0.0f && currentLatLong.GetLongitude() != 0.0f){
          mapCentered = true;
          Api.Instance.CameraApi.AnimateTo(centerMapLatLong, centerMapDistance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
        }
        povCam.transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, -Input.gyro.rotationRateUnbiased.z);
	}
}
