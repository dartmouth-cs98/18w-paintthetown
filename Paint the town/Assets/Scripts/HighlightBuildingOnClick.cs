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
    public string needBuildingID;
    public LatLong location;
    public string baseAlt;
    public string topAlt;
    public string id;

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

    IEnumerator getBuildingColor()
    {
      Hashtable headers = new Hashtable();
      print("hi there");
  		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
  		WWW www = new WWW(getBuildingIDURL, null, headers);
  		yield return www;

        if(www.text == "null"){
          print(www.error);
          StartCoroutine("createBuilding");
        }else{
            print("hehehe");
            print(www.text);
        }
  		// user data we can use for this scene

  		// subReturnStrings = returnData.Split(',');
  		// foreach(var item in subReturnStrings) {
  		// 		print(item.ToString());
  		// }
    }

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
      signupform.AddField("id", id);

      Hashtable headers = new Hashtable();
      headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
      WWW www = new WWW("https://paint-the-town.herokuapp.com/api/buildings", signupform.data, headers);
      yield return www;
    //  var signup = UnityWebRequest.Post("https://paint-the-town.herokuapp.com/api/buildings", signupform);
      //yield return signup.SendWebRequest();

  		if (www.error != null)
  		{
  			print("Error downloading: ");
  		}
  		else
  		{
        print(www.text);
  			print("building signed up!");
      }
    }

    public void startGetBuildingColor(string buildingID)
    {
      needBuildingID = buildingID;
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
