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
    private Vector3 mouseDownPosition;
    public string token;
    public string getBuildingIDURL; //https://paint-the-town.herokuapp.com/api/buildings/info?id=<buildingid>&fields[]=team
	public string baseAlt;
	public string topAlt;
	public string id;
	public LatLongAltitude location;
    public Text textArea;
    public Image image;
    public LatLongAltitude latLongAlt;
    public ArrayList poiList = new ArrayList(new string[] { "71a5f824a0dc35526a4b13078541adee" });
    public Boolean stopFlag;
	public Camera mainCam;
    public Camera povCam;
	public GameObject PlayerLevel;
    private ShowTextBox myTB;

    void Start()
    {
		// set inital state
      	stopFlag = false;
      	textArea.enabled = false;
      	image.enabled = false;
      	myTB = GetComponent<ShowTextBox>();

		// display player level on start
		PlayerLevel.GetComponent<Text>().text = "Level " + PlayerPrefs.GetString ("Level", "1");
    }


    void Update()
    {
		// get mouse position if down
        if (Input.GetMouseButtonDown(0)) { mouseDownPosition = Input.mousePosition; }

        if (Input.GetMouseButtonUp(0) && Vector3.Distance(mouseDownPosition, Input.mousePosition) < 5.0f && mainCam.enabled == false)
        {
			// make a ray out these positions
            var ray = povCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var viewportPoint = povCam.WorldToViewportPoint(hit.point);
                latLongAlt = Api.Instance.CameraApi.ViewportToGeographicPoint(viewportPoint, povCam);
                double captureDistance = .0015;
                if(((Input.location.lastData.latitude - latLongAlt.GetLatitude()) < captureDistance 
					&& -captureDistance < (Input.location.lastData.latitude - latLongAlt.GetLatitude())) && 
					((Input.location.lastData.longitude - latLongAlt.GetLongitude()) < captureDistance && 
						-captureDistance < (Input.location.lastData.longitude - latLongAlt.GetLongitude())))
				{
                    location = latLongAlt;
                    Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), passToGetID);
                }
                else if(image.enabled == false){
                    Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt.GetLatLong(), checkBuildingExist);
                }
            }
        }
    }
		
	// check if building exists
    void checkBuildingExist(bool success, Building b){
      if(success){
        string[] array = new string[1];
        array[0] = OverworldGlobals.ERROR_BUILDING_TOO_FAR;
        myTB.show(array);
      }
    }

	// get ID and check for POI
    void passToGetID(bool success, Building b)
    {
    	if(success){
	        baseAlt = "" + b.BaseAltitude;
	        topAlt = "" + b.TopAltitude;
	        id = b.BuildingId;
			// check if this building is a POI
	        StartCoroutine("checkPoi");

	        PlayerPrefs.SetString("bid", id);
	        PlayerPrefs.Save();

	        startGetBuildingColor(id);
      	} else {
        	print("uh oh");
      	}
    }

    /*
     * function to get building data
     * 	https://paint-the-town.herokuapp.com/api/buildings/updateTeam
     * building - building ID
	 * team - teamID
     */
    IEnumerator getBuildingColor()
    {
		// send request to get building id
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
                // open the testModelScene
                SceneManager.LoadScene("testModelScene");
            }else{
                print("POI not found!");
            }
        }

        return null;
    }

    IEnumerator captureBuilding()
    {
		// create form to request for updated team data
		WWWForm captureform = new WWWForm ();
	    captureform.AddField("building", id);
	    captureform.AddField("team", PlayerPrefs.GetString("teamID", "no teamID"));

		// send request for update team
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

			// use server response later in challenges scene
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();

			// parse server response to get paintLeft and level
		    string[] parsingString = Regex.Split(www.text, @"[,:{}]+");
		    for(int x =0; x < parsingString.Length; x ++){
		    	if(parsingString[x].Trim('"') == "paintLeft"){
					PlayerPrefs.SetString("Energy", parsingString[x+1].Trim('"'));
					PlayerPrefs.SetString("SendTimerUpdate", "true");
				} else if(parsingString[x].Trim('"') == "level"){
					PlayerPrefs.SetString("Level", parsingString[x+1].Trim('"'));
					PlayerLevel.GetComponent<Text>().text = "Level " + PlayerPrefs.GetString ("Level", "?");
				}
	        }
			PlayerPrefs.Save ();
	    }
    }

    IEnumerator createBuilding()
    {

		// create form to send for server update on buildings
        WWWForm signupform = new WWWForm();
      	signupform.AddField("name", "I am a name");
      	string lat = "" + location.GetLatitude();
      	string longi = "" + location.GetLongitude();
      	List<string> array = new List<string>();
      	array.Add(lat);
      	array.Add(longi);

      	//TODO: BUG HERE
      	signupform.AddField("id", id);
      	signupform.AddField("centroid[]", array[0]);
      	signupform.AddField("centroid[]", array[1]);
      	signupform.AddField("baseAltitude", baseAlt);
      	signupform.AddField("topAltitude", topAlt);

		// send request to server
      	Hashtable headers = new Hashtable();
      	headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
      	WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings", signupform.data, headers);
      	yield return www;

  		if (www.error != null)
  		{
  			print("Error downloading: ");
  		}
  		else
  		{
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
