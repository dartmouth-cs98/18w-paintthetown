using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;


[Serializable] public class City {
	public string name;
	public string country;
	public int[] centroid;
	public int[] bbox;
}

[Serializable] public class Player {
	public string role;
	public string lastName;
	public string name;
	public string typeOfLogin;
	public string team;
	public string[] friends;
	public int buildingsPainted;
	public string[] citiesPainted;
}

public class Profile : MonoBehaviour {

	public string userURL = "https://paint-the-town.herokuapp.com/api/users";
	public string teamurl = "https://paint-the-town.herokuapp.com/api/teams";
	public string citiesURL = "https://paint-the-town.herokuapp.com/api/cities/names";

	public string redID;
	public string blueID;
	public string fullName;
	public string team;
	public int buildingsPainted;
	public string[] citiesPainted = new string[0];
	public string[] friendsList = new string[0];
	public string token = "";
	public Player player;
	public string[] teamInfoList;

	// Use this for initialization
	IEnumerator Start () {
		// get token stored in PlayerPrefs
		token = "JWT " + PlayerPrefs.GetString("token", "no token");
		team = PlayerPrefs.GetString("teamID", "no team");

		// make the authorization header hashtable
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", token);

		// get the user data
		WWW userwww = new WWW(userURL, null, headers);
		yield return userwww;
	
		if(userwww.text == "null"){
			print(userwww.error);
		}else{
			// user data we can use for this scene
			player = JsonUtility.FromJson<Player>(userwww.text);
			headers.Add ("cities", player.citiesPainted);
		}
			
		WWW citieswww = new WWW(citiesURL, null, headers);
		yield return citieswww;
		if(citieswww.text == "null"){
			print(citieswww.error);
		}else{
			print(citieswww.text);
			string cities = citieswww.text;
			print ("hi i give you cities: " + cities);
		}


		// get the IDs for team data
		WWW teamwww = new WWW(teamurl, null, headers);
		yield return teamwww;
		if(teamwww.text == "null"){
			print(teamwww.error);
		}else{
			string teamInfo = teamwww.text;
			teamInfoList = teamInfo.Split('"');
			blueID = teamInfoList[5];
			redID = teamInfoList[19];
		}

		// save player info
		PlayerPrefs.SetString("firstName", player.name);
		PlayerPrefs.SetString("lastName", player.lastName);
		PlayerPrefs.Save();

		// concatenate name
		fullName = player.name + " " + player.lastName;
		// grab friendsList
		friendsList = player.friends;
		// get the team color
		getTeam ();

	}

	void getTeam() {
		// compare team ID to color IDs		
		if (team == redID) {
			team = "Red";
		} else if (team == blueID) {
			team = "Blue";
		} else {
			team = "You are not assigned to a team yet!";
		}

	}

	void OnGUI() {
		// to manipulate font sizes and colors
		GUIStyle style = new GUIStyle();

		if (GUI.Button (new Rect (500, 70, 75, 30), "Edit Profile")) {
			// load the edit profile scene
			SceneManager.LoadScene ("ProfileEditScene");
		}

		if (GUI.Button (new Rect (500, 150, 75, 30), "Logout")) {
			// load the login scene
			PlayerPrefs.DeleteAll();
			SceneManager.LoadScene ("LoginScene");
		}

		// set font color and size for Name
		GUI.contentColor = Color.black;
		style.fontSize = 35;
		// print name in top left corner
		GUI.Label(new Rect(200, 50, 100, 20), fullName, style);

		// set font size and color to team color?
		GUI.contentColor = Color.black;	//black for now
		style.fontSize = 30;
		// print team in top right corner
		GUI.Label(new Rect(500, 50, 700, 20), team);

		// set font size and color to team color?
		GUI.contentColor = Color.black;	//black for now
		style.fontSize = 30;

		//stats
		buildingsPainted = player.buildingsPainted;
		print ("citiesPainted is :"+player.citiesPainted);
		citiesPainted = player.citiesPainted;
		GUI.Label(new Rect(200, 100, 200, 20), "Your stats:");
		GUI.Label(new Rect(200, 125, 200, 20), "# buildings: " + buildingsPainted);
		GUI.Label(new Rect(200, 150, 200, 20), "Towns: ");
		int yCities = 175;

		if (citiesPainted.Length > 0) {
			print ("CITIES ARE HERE " + citiesPainted.GetValue(0));
			print ("your city painted id should be: " +citiesPainted[0]+" and the name is " +citiesPainted[0]);
			foreach (var city in citiesPainted) {
				GUI.Label (new Rect (200, yCities, 700, 20), city);
				yCities += 25;
			}
		} else {
			GUI.Label (new Rect (200, yCities, 700, 20), "Hanover, man");
		}       
			

		// set font color and size for "Your friends:" subheading
		GUI.contentColor = Color.magenta;
		style.fontSize = 25;
		// print below name
		GUI.Label(new Rect(200, 200, 100, 20), "Your friends:");
		int y = 225;
		// lower font size for list of friends
		style.fontSize = 20;
		if (friendsList.Length != 0) {
			foreach (var friend in friendsList) {
				GUI.Label (new Rect (200, y, 100, 20), friend);
				y += 25;
			}
		} else {
			GUI.Label (new Rect (200, y, 1000, 20), "No friends yet!");
		}
	}
}
