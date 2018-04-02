using System.Collections.Generic;
using System.Collections;
using Wrld;
using Wrld.Resources.Buildings;
using Wrld.Space;
using UnityEngine;
using UnityEngine.Networking;

// based on example code from https://wrld3d.com/unity/latest/docs/examples/picking-buildings/

public class HighlightBuildingOnClick : MonoBehaviour
{
    public Material highlightMaterial;
    private Vector3 mouseDownPosition;
    public string token;
    public string getBuildingIDURL; //https://paint-the-town.herokuapp.com/api/buildings/info?id=<buildingid>&fields[]=team
    public LatLong location;
    public string baseAlt;
    public string topAlt;
    public string id;
    public string captureBuildingID;

    void OnEnable()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) && Vector3.Distance(mouseDownPosition, Input.mousePosition) < 5.0f)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var viewportPoint = Camera.main.WorldToViewportPoint(hit.point);
                var latLongAlt = Api.Instance.CameraApi.ViewportToGeographicPoint(viewportPoint, Camera.main);

                //given selected building, start to get data from server
                Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), passToGetID);

                Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), OnBuildingRecieved);
                Api.Instance.BuildingsApi.HighlightBuildingAtLocation(latLongAlt, highlightMaterial, OnHighlightReceived);
            }
        }
    }

    void passToGetID(bool success, Building b)
    {
      //location = "" + b.Centroid;
      baseAlt = "" + b.BaseAltitude;
      topAlt = "" + b.TopAltitude;
      id = b.BuildingId;
      startGetBuildingColor(b.BuildingId);
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {
            StartCoroutine(ClearHighlight(highlight));
        }
    }

    void OnBuildingRecieved(bool success, Building b)
    {
        if(success)
        {
            print(b.BuildingId);
        }
    }

    //https://paint-the-town.herokuapp.com/api/buildings/updateTeam
    //building - building ID
    //team - teamID

    //function to get building data
    IEnumerator getBuildingColor()
    {
      Hashtable headers = new Hashtable();
      print("You're retrieving information about a building");
  		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
  		WWW www = new WWW(getBuildingIDURL, null, headers);
  		yield return www;

        if(www.text == "null"){
          //the building has never been clicked before
          print(www.error);
          StartCoroutine("createBuilding");
        }else{
            //the building has been found and it's data is returned
            //TODO: update data on server and color building
            print("hehehe");
            print(www.text);
            StartCoroutine("captureBuilding");
        }


  		// user data we can use for this scene

  		// subReturnStrings = returnData.Split(',');
  		// foreach(var item in subReturnStrings) {
  		// 		print(item.ToString());
  		// }
    }

    IEnumerator captureBuilding()
    {
      print("You're capturing a building");

      WWWForm captureform = new WWWForm();

      print("team ID: " + PlayerPrefs.GetString("teamID", "no teamID"));
      captureform.AddField("building", captureBuildingID);
      captureform.AddField("team", PlayerPrefs.GetString("teamID", "no teamID"));

      Hashtable headers = new Hashtable();
      headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));

      WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings/updateTeam", captureform.data, headers);
      yield return www;
      if (www.error != null)
      {
        print("Error downloading: " + www.error);
      }
      else
      {
        print(www.text);
        print("building captured!");
      }

    }


    //function to create building data on the server
    IEnumerator createBuilding()
    {
      print ("You're making a building");

      WWWForm signupform = new WWWForm();

      signupform.AddField("name", "I am a name");
      //print(location);
      string lat = "" + location.GetLatitude();
      string longi = "" + location.GetLongitude();
      string[] array = new string[2];
      array[0] = lat;
      array[1] = longi;
      signupform.AddField("centroid[]", array[0]);
      signupform.AddField("centroid[]", array[1]);
      signupform.AddField("baseAltitude", baseAlt);
      signupform.AddField("topAltitude", topAlt);

      Hashtable headers = new Hashtable();
      headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
      WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings", signupform.data, headers);
      yield return www;
      //var signup = UnityWebRequest.Post("https://paint-the-town.herokuapp.com/api/buildings", signupform);
      //yield return signup.SendWebRequest();

  		if (www.error != null)
  		{
  			print("Error downloading: ");
  		}
  		else
  		{
        print(www.text);
  			print("building signed up!");
        StartCoroutine("captureBuilding");
      }
    }

    //starter fuction to retrieve building data
    public void startGetBuildingColor(string buildingID)
    {
      captureBuildingID = buildingID;
      getBuildingIDURL = "https://paint-the-town.herokuapp.com/api/buildings/info?id=" + buildingID + "&fields[]=team";
      StartCoroutine("getBuildingColor");
    }

    IEnumerator ClearHighlight(Highlight highlight)
    {
        yield return new WaitForSeconds(3.0f);
        Api.Instance.BuildingsApi.ClearHighlight(highlight);
    }

    // set the desired material for the highlight
    public void OnRedClick()
    {
        //highlightMaterial = Resources.Load("RedMaterial") as Material;
        highlightMaterial.color = Color.red;
    }

    public void OnBlueClick()
    {
        //highlightMaterial = Resources.Load("BlueMaterial") as Material;
        highlightMaterial.color = Color.blue;
    }
}
