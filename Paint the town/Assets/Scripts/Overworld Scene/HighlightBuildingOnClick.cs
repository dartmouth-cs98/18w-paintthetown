using System.Collections.Generic;
using System.Collections;
using Wrld;
using Wrld.Resources.Buildings;
using Wrld.Space;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


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
    public Text textArea;
    public string[] strings;
    public float speed = 0.001f;
    public Image image;
    public LatLongAltitude latLongAlt;
    public ArrayList poiList = new ArrayList(new string[] { "71a5f824a0dc35526a4b13078541adee" });
    private Boolean isPoi;
    public Boolean stopFlag;
    private string buildingDistanceMessage = "You must be closer to the building in order to paint it!";
    private string sameBuildingColorMessage = "That building is already owned by your team!";
    public Camera mainCam;
    public Camera povCam;

    private ShowTextBox myTB;
    int index = 0;
    int characterIndex = 0;

    void Start()
    {
      stopFlag = false;
      textArea.enabled = false;
      image.enabled = false;
      myTB = GetComponent<ShowTextBox>();


    }

    void OnEnable()
    {

    }

    void Update()
    {


        if (Input.GetMouseButtonDown(0)) { mouseDownPosition = Input.mousePosition; }

        if (Input.GetMouseButtonUp(0) && Vector3.Distance(mouseDownPosition, Input.mousePosition) < 5.0f && mainCam.enabled == false)
        {

            var ray = povCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var viewportPoint = povCam.WorldToViewportPoint(hit.point);
                latLongAlt = Api.Instance.CameraApi.ViewportToGeographicPoint(viewportPoint, povCam);
                double captureDistance = .0015;
                if(((Input.location.lastData.latitude - latLongAlt.GetLatitude()) < captureDistance && -captureDistance < (Input.location.lastData.latitude - latLongAlt.GetLatitude())) && ((Input.location.lastData.longitude - latLongAlt.GetLongitude()) < captureDistance && -captureDistance < (Input.location.lastData.longitude - latLongAlt.GetLongitude()))){

                    location = latLongAlt;

                    Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), passToGetID);
                }
                else if(image.enabled == false){
                    Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), checkBuildingExist);
                }
            }
        }
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {

        }
    }

    void checkBuildingExist(bool success, Building b){
      if(success){
        myTB.show(OverworldGlobals.ERROR_BUILDING_TOO_FAR);
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
        StartCoroutine("checkPoi");

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
  		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
  		WWW www = new WWW(getBuildingIDURL, null, headers);
  		yield return www;

        print(PlayerPrefs.GetString("teamID", "no teamID"));
        if(www.text == "null"){
          //the building has never been clicked before
          print(www.error);
        }else{
            string[] subStrings = Regex.Split(www.text, @"[,:{}]+");
            StartCoroutine("captureBuilding");
        }
    }

    //starter fuction to retrieve building data
    public void startGetBuildingColor(string buildingID)
    {
      getBuildingIDURL = "https://paint-the-town.herokuapp.com/api/buildings/info?id=" + buildingID + "&fields[]=team";
      StartCoroutine("getBuildingColor");
    }

    // check if an id is that of a POI, asynchronously so threads don't lock
    IEnumerator checkPoi()
    {
        foreach(string idNum in poiList){
            if (idNum.Equals(id)){
                print("POI found!");
                isPoi = true;
                // open the testModelScene
                SceneManager.LoadScene("testModelScene");
            }else{
                print("POI not found!");
                isPoi = false;
            }
        }

        return null;
    }

    IEnumerator captureBuilding()
    {
      //print("You're capturing a building");

      WWWForm captureform = new WWWForm();

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

        string[] parsingString = Regex.Split(www.text, @"[,:{}]+");

        for(int x =0; x < parsingString.Length; x ++){

          if(parsingString[x].Trim('"') == "paintLeft"){

            PlayerPrefs.SetString("Energy", parsingString[x+1].Trim('"'));
            print(PlayerPrefs.GetString("Energy", "nooooo"));
          }
        }
      }
    }

    IEnumerator createBuilding()
    {
      //print ("You're making a building");

      WWWForm signupform = new WWWForm();

      signupform.AddField("name", "I am a name");
      //print(location);
      string lat = "" + location.GetLatitude();
      string longi = "" + location.GetLongitude();
      List<string> array = new List<string>();
      array.Add(lat);
      array.Add(longi);
      //print(array[0]);
      //print(array[1]);
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
        //print(www.text);
  			//print("building signed up!");
        StartCoroutine("captureBuilding");
      }
    }

    IEnumerator ClearHighlight(Highlight highlight)
    {
        yield return new WaitForSeconds(3.0f);
        Api.Instance.BuildingsApi.ClearHighlight(highlight);
    }

    public void stopCheckingMapClicks(){
      stopFlag = !stopFlag;
    }
}
