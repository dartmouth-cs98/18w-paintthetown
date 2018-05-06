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

    struct BuildingStuff
    {
        public string id;
        public float Lat;
        public float Longe;
        public float alt;
        public float r;
        public float g;
        public float b;

        public override bool Equals( object ob ){
      		if( ob is BuildingStuff ) {
      			BuildingStuff c = (BuildingStuff) ob;
      			return id==c.id && Lat==c.Lat && Longe==c.Longe && alt==c.alt && r==c.r && g==c.g && b==c.b;
      		}
      		else {
      			return false;
      		}
      	}

        // public override int GetHashCode(){
        //   return id.GetHashCode() + Lat.GetHashCode()  + Longe.GetHashCode() + alt.GetHashCode() + r.GetHashCode() + g.GetHashCode() + b.GetHashCode();
        // }

    };

    struct BuildingPOIStuff
    {
      public string id;
      public float Lat;
      public float Longe;
      public float alt;

      public override bool Equals( object ob ){
        if( ob is BuildingPOIStuff ) {
          BuildingPOIStuff c = (BuildingPOIStuff) ob;
          return id==c.id && Lat==c.Lat && Longe==c.Longe && alt==c.alt;
        }
        else {
          return false;
        }
      }

      // public override int GetHashCode(){
      //   return (id.GetHashCode() + Lat.GetHashCode() + Longe.GetHashCode() + alt.GetHashCode());
      // }

    };

    public bool isUnityRemote;
    public Camera povCam;
    public Camera setCam;
    public float zoomSpeed = 1.0f; // speed to zoom in or out at
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
    private float time = 0.5f;
    public GameObject toBeDestroyedMarker;
    public GameObject highlightGameObject;
    public GameObject[] listOfToBeDestroyed;
    public LatLong centerMapLatLong;
    public double centerMapDistance;
    public bool mapCentered;
    public Shader shader1;

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

    private LatLongAltitude lastLocationOfCamera;

    public static Mesh highlightMesh;

    // particle system stuff
    public ParticleSystem pLauncher;

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



    Dictionary<string, float> oldBuildingsD = new Dictionary<string, float>();
    Dictionary<string, float> newBuildingsD = new Dictionary<string, float>();

    HashSet<BuildingPOIStuff> oldBuildings = new HashSet<BuildingPOIStuff>();
    HashSet<BuildingPOIStuff> newBuildings = new HashSet<BuildingPOIStuff>();

    HashSet<BuildingStuff> oldBuildingsColor = new HashSet<BuildingStuff>();
    HashSet<BuildingStuff> newBuildingsColor = new HashSet<BuildingStuff>();

    IEnumerator Start()
    {
        mapCentered = false;
        Input.gyro.enabled = true;
        ResetBaseOrientation();
        UpdateCalibration(true);
        UpdateCameraBaseRotation(true);
        RecalculateReferenceRotation();

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

      uMinLng = (longe - .01);
      uMinLat = (lat - .01);
      uMaxLng = (longe + .01);
      uMaxLat = (lat + .01);

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

      WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings?bbox[0]=" + uMinLat + "&bbox[1]=" + uMinLng + "&bbox[2]=" + uMaxLat + "&bbox[3]=" + uMaxLng + "&extraFields[0]=centroidLng&extraFields[1]=centroidLat&extraFields[2]=team&extraFields[3]=baseAltitude&extraFields[4]=topAltitude&extraFields[5]=rgb&teamOnly=true", null, header);
      yield return www;
      if (www.error != null)
      {
        print("Error downloading: " + www.error);
      } else {

        // print("WWW " + www.text);
        parsingString = Regex.Split(www.text, @"[,:{}]+");
        //
        // for(int y = 3; y < parsingString.Length; y++){
        //   print(parsingString[y]);
        // }

        for(int x = 3; x < parsingString.Length - 18; x = x + 38){
          stringLnge = "";
          team = "";
          lnge = -1;
          stringLat = "";
          lat = -1;
          alt = -1;
          id = "";
          team = "";
          stringTopAlt = "";
          topAlt = -1;
          r = "";
          g = "";
          b = "";
          for(int y = 0; y < 18; y ++){
            //
            // if(parsingString[x + y].Trim('"') == "centroidLng"){
            //   print("centroidlng: " + parsingString[x + y].Trim('"'));
            // }
            // if (parsingString[x + y].Trim('"') == "centroidLat"){
            //   print("centroidlat: " + parsingString[x + y].Trim('"'));
            // }
            // if (parsingString[x + y].Trim('"') == "baseAltitude"){
            //   print("baseAlit: " + parsingString[x + y].Trim('"'));
            // }
            // if (parsingString[x + y].Trim('"') == "id"){
            //   print("id? " +parsingString[x + y + 1].Trim('"'));
            // }
            // if (parsingString[x + y].Trim('"') == "name" && parsingString[x + y + 2].Trim('"') != "color"){
            //   print("name " + parsingString[x + y].Trim('"'));
            // }
            // if (parsingString[x + y].Trim('"') == "topAltitude"){
            //   print("topAlt " + parsingString[x + y].Trim('"'));
            // }
            // if (parsingString[x + y].Trim('"') == "rgb"){
            //   print("rgb " + parsingString[x + y].Trim('"'));
            // }

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
            } else if (parsingString[x + y].Trim('"') == "name" && parsingString[x + y + 2].Trim('"') != "color"){
              team = parsingString[x + y + 1].Trim('"');
            } else if (parsingString[x + y].Trim('"') == "topAltitude"){
              stringTopAlt = parsingString[x + y + 1].Trim('"');
              topAlt = Convert.ToDouble(parsingString[x + y + 1].Trim('"'));
            } else if (parsingString[x + y].Trim('"') == "rgb"){
              r = parsingString[x + y + 1].Trim('[');
              g = parsingString[x + y + 2].Trim('"');
              b = parsingString[x + y + 3].Trim(']');
            }
          }

          if(id == "0e91d94e8cf9315668e31b33706787a5"){
            print("LALALLALALALA");
            print(r);
            print(g);
            print(b);
          }

          // ***********************************
          //BUILDING HIGHLIGHT DESTROY AND LOAD
          // ***********************************

            var value = (float)Convert.ToDouble(r) + ((float)Convert.ToDouble(g) * 10) + ((float)Convert.ToDouble(b) * 25);

            if(id == "0e91d94e8cf9315668e31b33706787a5"){
              print(value);
            }

            if(id != ""){

              var key = id;

              newBuildingsD.Add(key, value);

              if(oldBuildingsD.ContainsKey(key)){

                if(key == "0e91d94e8cf9315668e31b33706787a5"){
                  print("papi");
                  print(oldBuildingsD[key]);
                }

                if(oldBuildingsD[key] != value){
                  print("WHHAAAAAOOOOOO");
                    var boxLocation = LatLongAltitude.FromDegrees(lnge, lat, alt);
                    //create RGB from the list
                    Color color = new Color( (float)Convert.ToDouble(r)/255, (float)Convert.ToDouble(g)/255, (float)Convert.ToDouble(b)/255, 0.7f);
                    StartCoroutine(MakeHighlight(id, boxLocation, color));
                }
              } else{
                print("THIS HAPPENS FIRST");
                var boxLocation = LatLongAltitude.FromDegrees(lnge, lat, alt);
                //create RGB from the list
                Color color = new Color( (float)Convert.ToDouble(r)/255, (float)Convert.ToDouble(g)/255, (float)Convert.ToDouble(b)/255, 0.7f);
                StartCoroutine(MakeHighlight(id, boxLocation, color));
              }
            } else {
              if(value > 0){
                print("OH NOOOOOO");
              }
            }


          // *****************************************
          // add POIs to the new team color hashset
          // *****************************************

          foreach(string idNum in poiList){
            if (idNum.Equals(id)){

              BuildingPOIStuff BuildingPOI = new BuildingPOIStuff();

              BuildingPOI.id = id;
              BuildingPOI.Lat = (float)lat;
              BuildingPOI.Longe = (float)lnge;
              BuildingPOI.alt  = (float)alt;
              newBuildings.Add(BuildingPOI);
            }
          }

        }

        foreach(KeyValuePair<string, float> entry in oldBuildingsD){

            if(newBuildingsD.ContainsKey(entry.Key)){
              if(newBuildingsD[entry.Key] != entry.Value){
                 StartCoroutine(destroyColor(entry.Key));
              }
            } else{
              StartCoroutine(destroyColor(entry.Key));
            }
        }

        oldBuildingsD = new Dictionary<string, float>(newBuildingsD);

        newBuildingsD.Clear();


        // ***********************************
        //BUILDING POI DESTROY AND LOAD
        // ***********************************
        var toLoad = new HashSet<BuildingPOIStuff>(newBuildings);
        var toDestroy = new HashSet<BuildingPOIStuff>(oldBuildings);

        toDestroy.ExceptWith(newBuildings);
        toLoad.ExceptWith(oldBuildings);

        foreach (BuildingPOIStuff placement in toDestroy){
          StartCoroutine(destroy(placement.id));
        }

        foreach (BuildingPOIStuff placement in toLoad){
          var boxLocation = LatLongAltitude.FromDegrees(placement.Longe, placement.Lat, placement.alt + 20);
          StartCoroutine(MakeBox(placement.id, boxLocation));
        }

        oldBuildings = newBuildings;
        newBuildings.Clear();
      }
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success){

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

        var viewpoint = Api.Instance.CameraApi.GeographicToViewportPoint(latLongAlt);
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

      // if(setCam.enabled){
      //     var yourlocation = LatLongAltitude.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude,  Input.location.lastData.altitude);
      //
      //     //var viewportpoint = setCam.WorldToViewportPoint(setCam.transform.position);
      //
      //     // var viewpoint = Api.Instance.CameraApi.GeographicToViewportPoint(yourlocation);
      //     var playerWorldPoint = Api.Instance.CameraApi.GeographicToWorldPoint(yourlocation);
      //
      //     var cameraWorldPoint = setCam.transform.position;
      //
      //     var differenceX = playerWorldPoint.x - cameraWorldPoint.x;
      //     var differenceY = playerWorldPoint.z - cameraWorldPoint.z;
      //
      //     print("CAMERA X: " + cameraWorldPoint.x);
      //     print("CAMERA Y: " + cameraWorldPoint.y);
      //     print("PLAYER X: " + playerWorldPoint.x);
      //     print("PLAYER Y: " + playerWorldPoint.y);
      //
      //
      //     // if (differenceX < -500 || differenceX > 500 || differenceY < -500 || differenceY > 500){
      //     if (differenceX < -500){
      //       var tempWorldPoint = cameraWorldPoint;
      //
      //       tempWorldPoint.x = playerWorldPoint.x-450;
      //
      //       print("GOING TO x: " + Convert.ToString(playerWorldPoint.x-450));
      //
      //       var tempViewportPoint = setCam.WorldToViewportPoint(tempWorldPoint);
      //       var tempGeoPoint = Api.Instance.CameraApi.ViewportToGeographicPoint(tempViewportPoint);
      //
      //       Api.Instance.CameraApi.AnimateTo(tempGeoPoint.GetLatLong(), 0, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0, transitionDuration: 0);
      //     }
      //     else if(differenceX > 500){
      //       var tempWorldPoint = cameraWorldPoint;
      //
      //       tempWorldPoint.x = playerWorldPoint.x + 450;
      //
      //       print("GOING TO x: " + Convert.ToString(playerWorldPoint.x + 450));
      //
      //       var tempViewportPoint = setCam.WorldToViewportPoint(tempWorldPoint);
      //       var tempGeoPoint = Api.Instance.CameraApi.ViewportToGeographicPoint(tempViewportPoint);
      //
      //       Api.Instance.CameraApi.AnimateTo(tempGeoPoint.GetLatLong(), 0, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0, transitionDuration: 0);
      //     }
      //     else if (differenceY < -500){
      //       var tempWorldPoint = cameraWorldPoint;
      //
      //       tempWorldPoint.y = playerWorldPoint.y - 450;
      //
      //       print("GOING TO y: " + Convert.ToString(playerWorldPoint.y - 450));
      //
      //       var tempViewportPoint = setCam.WorldToViewportPoint(tempWorldPoint);
      //       var tempGeoPoint = Api.Instance.CameraApi.ViewportToGeographicPoint(tempViewportPoint);
      //
      //       Api.Instance.CameraApi.AnimateTo(tempGeoPoint.GetLatLong(), 0, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0, transitionDuration: 0);
      //
      //     }
      //     else if (differenceY > 500){
      //       var tempWorldPoint = cameraWorldPoint;
      //
      //       tempWorldPoint.y = playerWorldPoint.y + 450;
      //
      //       print("GOING TO y: " + Convert.ToString(playerWorldPoint.y + 450));
      //
      //       var tempViewportPoint = setCam.WorldToViewportPoint(tempWorldPoint);
      //       var tempGeoPoint = Api.Instance.CameraApi.ViewportToGeographicPoint(tempViewportPoint);
      //
      //       Api.Instance.CameraApi.AnimateTo(tempGeoPoint.GetLatLong(), 0, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0, transitionDuration: 0);
      //
      //     }
      //   }

        // if(((Input.location.lastData.latitude - latLongAlt.GetLatitude()) < captureDistance && -captureDistance < (Input.location.lastData.latitude - latLongAlt.GetLatitude())) && ((Input.location.lastData.longitude - latLongAlt.GetLongitude()) < captureDistance && -captureDistance < (Input.location.lastData.longitude - latLongAlt.GetLongitude()))){

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
        }
        var currentLatLong = LatLong.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);
        var currentLocation = LatLongAltitude.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude, 0);

        Api.Instance.CameraApi.GeographicToWorldPoint(currentLocation,setCam);
        Api.Instance.StreamResourcesForCamera(setCam);
        Api.Instance.Update();

        centerMapLatLong = currentLatLong;
        centerMapDistance = distance;

        if (!mapCentered && currentLatLong.GetLatitude() != 0.0f && currentLatLong.GetLongitude() != 0.0f)
        {
            mapCentered = true;
            Api.Instance.CameraApi.AnimateTo(centerMapLatLong, centerMapDistance, headingDegrees: Input.compass.trueHeading, tiltDegrees: 0);
            lastLocationOfCamera = LatLongAltitude.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude, Input.location.lastData.altitude);
        }

        RaycastHit hit;
        Vector3 tempPOVposition = new Vector3(setCam.transform.position.x, 160, setCam.transform.position.z + 75);

        if (Physics.Raycast(tempPOVposition,Vector3.down,out hit, 300))
        {
            tempPOVposition.y = hit.point.y + 15f;
        }

        pLauncher.transform.SetPositionAndRotation(setCam.transform.position, setCam.transform.rotation);
        povCam.transform.position = tempPOVposition;
        povCam.transform.rotation = Quaternion.Slerp(povCam.transform.rotation,cameraBase * (ConvertRotation(referenceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor); ;
	}
}
