
using System.Collections;
using Wrld;
using Wrld.Resources.Buildings;
using Wrld.Space;
using UnityEngine;

// based on example code from https://wrld3d.com/unity/latest/docs/examples/picking-buildings/

public class HighlightBuildingOnClick : MonoBehaviour
{
    public Material highlightMaterial;
    private Vector3 mouseDownPosition;
    public string buildingOwnershipURL = "https://paint-the-town.herokuapp.com/api/buildings/getTeam?id=";
    public string buildingID = "";
    public string token;

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
                Api.Instance.BuildingsApi.GetBuildingAtLocation(latLongAlt, GetBuildingCallback);
                Api.Instance.BuildingsApi.HighlightBuildingAtLocation(latLongAlt, highlightMaterial, OnHighlightReceived);

            }
        }
    }

    void GetBuildingCallback(bool success, Building building)
    {
      print("this is your building ID " + building.BuildingId);
      getBuildingIDStart(building.BuildingId);
    }

    void OnHighlightReceived(bool success, Highlight highlight)
    {
        if (success)
        {
            StartCoroutine(ClearHighlight(highlight));
        }
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

    public IEnumerator getBuildingID()
    {
      Hashtable headers = new Hashtable();
      headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
      WWW www = new WWW(buildingOwnershipURL, null, headers);
      yield return www;

      string returnData = www.text;
      print(returnData);
      // returnData = returnData.Split(',');
      // foreach(var item in returnData) {
      //     print(item.ToString());
      // }
    		// print("hello!");
    		// 	using (WWW www = new WWW(buildingOwnershipURL + buildingID))
    		// 	{
    		// 			yield return www;
    		// 			if(www.error == null){
        //
    		// 				print(www.text);
        //
    		// 			}else{
    		// 				print("you have a problem");
    		// 				print(www.error);
    		// 			}
    		// 	}
    	}

      public void getBuildingIDStart(string ID)
      {
        buildingOwnershipURL = buildingOwnershipURL + ID;
        StartCoroutine("getBuildingID");
      }

}
