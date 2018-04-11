using System.Collections.Generic;
using System.Collections;
using Wrld;
using Wrld.Resources.Buildings;
using Wrld.Space;
using UnityEngine;
using UnityEngine.Networking;
using System;

// based on example code from https://wrld3d.com/unity/latest/docs/examples/picking-buildings/

public class HighlightBuildingOnClick : MonoBehaviour
{
    public Material highlightMaterial;
    private Vector3 mouseDownPosition;
    public string token;
    public string getBuildingIDURL; //https://paint-the-town.herokuapp.com/api/buildings/info?id=<buildingid>&fields[]=team
    public LatLongAltitude location;
    public string baseAlt;
    public string topAlt;
    public string id;



    void Start()
    {
      print("setting color");
      if(PlayerPrefs.GetString("color", "no color") == "red")
      {
        highlightMaterial.color = Color.red;
      } else if(PlayerPrefs.GetString("color", "no color") == "blue"){
        highlightMaterial.color = Color.blue;
      } else {
        print("Error: could not find player color");
      }
    }

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
                print("BUILDING LAT LONG ALT: " + latLongAlt.GetLatitude() + " " + latLongAlt.GetLongitude() + " " + latLongAlt.GetAltitude());

                PlayerPrefs.SetString("LAT", Convert.ToString(latLongAlt.GetLatitude()));
                PlayerPrefs.SetString("LONG", Convert.ToString(latLongAlt.GetLongitude()));
                PlayerPrefs.SetString("ALT", Convert.ToString(latLongAlt.GetAltitude()));
                PlayerPrefs.Save();
                //given selected building, start to get data from server

                location = latLongAlt;
                Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), passToGetID);

                Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), OnBuildingRecieved);

                Api.Instance.BuildingsApi.HighlightBuildingAtLocation(latLongAlt, highlightMaterial, OnHighlightReceived);
            }
        }
    }


    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {
            //StartCoroutine(ClearHighlight(highlight));
        }
    }

    void OnBuildingRecieved(bool success, Building b)
    {
        if(success)
        {
            print(b.BuildingId);
        }
    }

    void passToGetID(bool success, Building b)
    {
      if(success){
        baseAlt = "" + b.BaseAltitude;
        topAlt = "" + b.TopAltitude;
        id = b.BuildingId;
        print("THIS IS THE BUILDING'S ID: " + id);

        PlayerPrefs.SetString("bid", id);
        PlayerPrefs.Save();

        startGetBuildingColor(id);
      } else {
        print("uh oh");
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

        print("HERE HERE HERE " + www.text);
        if(www.text == "null"){
          //the building has never been clicked before
          print(www.error);
          //StartCoroutine("createBuilding");
        }else{
            StartCoroutine("captureBuilding");
        }
    }

    //starter fuction to retrieve building data
    public void startGetBuildingColor(string buildingID)
    {
      getBuildingIDURL = "https://paint-the-town.herokuapp.com/api/buildings/info?id=" + buildingID + "&fields[]=team";
      StartCoroutine("getBuildingColor");
    }

    IEnumerator captureBuilding()
    {
      print("You're capturing a building");

      WWWForm captureform = new WWWForm();

      print("team ID: " + PlayerPrefs.GetString("teamID", "no teamID"));
      captureform.AddField("building", id);
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
        print("THIS IS COLOR " + PlayerPrefs.GetString("color", "no color"));
      }
    }

    IEnumerator createBuilding()
    {
      print ("You're making a building");

      WWWForm signupform = new WWWForm();

      signupform.AddField("name", "I am a name");
      print(location);
      string lat = "" + location.GetLatitude();
      string longi = "" + location.GetLongitude();
      List<string> array = new List<string>();
      array.Add(lat);
      array.Add(longi);
      print(array[0]);
      print(array[1]);
      //TODO: BUG HERE
      signupform.AddField("id", id);
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

    IEnumerator ClearHighlight(Highlight highlight)
    {
        yield return new WaitForSeconds(3.0f);
        Api.Instance.BuildingsApi.ClearHighlight(highlight);
    }
}
