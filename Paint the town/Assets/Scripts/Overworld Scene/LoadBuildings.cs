using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Wrld;
using Wrld.Space;
using Wrld.Resources.Buildings;

using UnityEngine;
using UnityEngine.Networking;

public class LoadBuildings : MonoBehaviour {

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
	private float time = 0.25f;
	public string[] parsingString;
	public ArrayList poiList;

	public GameObject prefab;
	GameObject toBeDestroyedMarker;

	double uMinLng;
	double uMinLat;
	double uMaxLng;
	double uMaxLat;

	public Material highlightMaterialRed;
	public static Mesh highlightMesh;
	public Camera setCam;

  Dictionary<string, float> oldBuildingsD = new Dictionary<string, float>();
  Dictionary<string, float> newBuildingsD = new Dictionary<string, float>();

  HashSet<BuildingPOIStuff> oldBuildings = new HashSet<BuildingPOIStuff>();
  HashSet<BuildingPOIStuff> newBuildings = new HashSet<BuildingPOIStuff>();

  HashSet<BuildingStuff> oldBuildingsColor = new HashSet<BuildingStuff>();
  HashSet<BuildingStuff> newBuildingsColor = new HashSet<BuildingStuff>();

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
	};

	// Use this for initialization
	void Start () {
		poiList = new ArrayList();
		poiList.Add("71a5f824a0dc35526a4b13078541adee");

		InvokeRepeating("updateMap", 2.0f, time);
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

			parsingString = Regex.Split(www.text, @"[,:{}]+");

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
					// print("LALALLALALALA");
					// print(r);
					// print(g);
					// print(b);
				}

				// ***********************************
				//BUILDING HIGHLIGHT DESTROY AND LOAD
				// ***********************************

					var value = (float)Convert.ToDouble(r) + ((float)Convert.ToDouble(g) * 10) + ((float)Convert.ToDouble(b) * 25);

					if(id == "0e91d94e8cf9315668e31b33706787a5"){
						// print(value);
					}

					if(id != ""){

						var key = id;

						newBuildingsD.Add(key, value);

						if(oldBuildingsD.ContainsKey(key)){

							if(key == "0e91d94e8cf9315668e31b33706787a5"){
								// print("papi");
								// print(oldBuildingsD[key]);
							}

							if(oldBuildingsD[key] != value){
								// print("WHHAAAAAOOOOOO");
									var boxLocation = LatLongAltitude.FromDegrees(lnge, lat, alt);
									//create RGB from the list
									Color color = new Color( (float)Convert.ToDouble(r)/255, (float)Convert.ToDouble(g)/255, (float)Convert.ToDouble(b)/255, 0.7f);
									StartCoroutine(MakeHighlight(id, boxLocation, color));
							}
						} else{
							// print("THIS HAPPENS FIRST");
							var boxLocation = LatLongAltitude.FromDegrees(lnge, lat, alt);
							//create RGB from the list
							Color color = new Color( (float)Convert.ToDouble(r)/255, (float)Convert.ToDouble(g)/255, (float)Convert.ToDouble(b)/255, 0.7f);
							StartCoroutine(MakeHighlight(id, boxLocation, color));
						}
					} else {
						if(value > 0){
							// print("OH NOOOOOO");
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

	IEnumerator MakeBox(string id, LatLongAltitude latLongAlt){

			var viewpoint = Api.Instance.CameraApi.GeographicToViewportPoint(latLongAlt);
			var worldpoint = setCam.ViewportToWorldPoint(viewpoint);
			GameObject cloneMarker = Instantiate(prefab, worldpoint, Quaternion.Euler(45, 0, 0)) as GameObject;;
			cloneMarker.name = id;
			yield return null;
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

	IEnumerator destroy(string id){
		toBeDestroyedMarker = GameObject.Find(id);
		Destroy(toBeDestroyedMarker);
		yield return null;
	}

	// Update is called once per frame
	void Update () {

	}
}
