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
    public ArrayList poiList = new ArrayList(new string[] { "dbf69cccfd7b8c096e5b150e0140b0ae" });
    private Boolean isPoi;
    private string buildingDistanceMessage = "You must be closer to the building in order to paint it!";
    private string sameBuildingColorMessage = "That building is already owned by your team!";

    int index = 0;
    int characterIndex = 0;

    void Start()
    {
      textArea.enabled = false;
      image.enabled = false;

      if(PlayerPrefs.GetString("color", "no color") == "red")
      {
        highlightMaterial.color = Color.red;
        print("THE GAME COLOR IS RED");
      } else if(PlayerPrefs.GetString("color", "no color") == "blue"){
        highlightMaterial.color = Color.blue;
        print("THE GAME COLOR IS BLUE");
      } else {
        print("Error: could not find player color");
      }
    }

    void OnEnable()
    {

    }

    void Update()
    {
      if(Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Space)){
        if(image.enabled == true && textArea.enabled == true){
          if (index == strings.Length - 1){
            image.enabled = false;
            textArea.enabled = false;
            index = 0;
            characterIndex = 0;
          }else if (index < strings.Length){
            index++;
            characterIndex = 0;
          } else if(characterIndex < strings[index].Length){
            characterIndex = strings[index].Length;
          }
        }
      }

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
                latLongAlt = Api.Instance.CameraApi.ViewportToGeographicPoint(viewportPoint, Camera.main);
                double captureDistance = .0015;
                if(((Input.location.lastData.latitude - latLongAlt.GetLatitude()) < captureDistance && -captureDistance < (Input.location.lastData.latitude - latLongAlt.GetLatitude())) && ((Input.location.lastData.longitude - latLongAlt.GetLongitude()) < captureDistance && -captureDistance < (Input.location.lastData.longitude - latLongAlt.GetLongitude()))){
                  //given selected building, start to get data from server

                  location = latLongAlt;

                  Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), passToGetID);

                  Api.Instance.BuildingsApi.HighlightBuildingAtLocation(latLongAlt, highlightMaterial, OnHighlightReceived);

                } else if(image.enabled == false){

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
        print("MEMEMEMEMEMEMEMEMEMEMEM");
        image.enabled = true;
        textArea.enabled = true;
        index = 0;
        characterIndex = 0;
        strings[0] = buildingDistanceMessage;
        StartCoroutine("displayTimer");
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
      print("You're retrieving information about a building");
  		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
  		WWW www = new WWW(getBuildingIDURL, null, headers);
  		yield return www;

        print("HERE HERE HERE " + www.text);
        print(PlayerPrefs.GetString("teamID", "no teamID"));
        if(www.text == "null"){
          //the building has never been clicked before
          print(www.error);
        }else{
            string[] subStrings = Regex.Split(www.text, @"[,:{}]+");
            bool Flag = false;
            for (int i = 0; i < subStrings.Length; i++){
              if(subStrings[i].Trim('"') == "team"){
                if(subStrings[i + 1].Trim('"') == PlayerPrefs.GetString("teamID", "no teamID")){
                  Flag = true;
                }
              }
            }
            if(Flag == true){
              image.enabled = true;
              textArea.enabled = true;
              index = 0;
              characterIndex = 0;
              strings[0] = sameBuildingColorMessage;
              StartCoroutine("displayTimer");
            }else{
              StartCoroutine("captureBuilding");
            }
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
      print("You're capturing a building");

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

    IEnumerator displayTimer(){

      while(true){

        if((Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Space)) && index == (strings.Length)){
          print("Hello???");
          break;
        }
        yield return new WaitForSeconds(speed);
        if(characterIndex > strings[index].Length){
          continue;
        }
        textArea.text = strings[index].Substring(0, characterIndex);
        characterIndex++;
        //print("text area: " + textArea.text);
      }
    }
}
