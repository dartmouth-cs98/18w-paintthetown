using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;


[Serializable] public class Player {
	public string role;
	public string lastName;
	public string name;
	public string typeOfLogin;
	public string team;
	public string[] friends;
}

public class Profile : MonoBehaviour {

	public string userURL = "https://paint-the-town.herokuapp.com/api/users";
	public string teamurl = "https://paint-the-town.herokuapp.com/api/teams";
	public string redID;
	public string blueID;
	public string fullName;
	public string team;
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
			SceneManager.LoadScene ("test Login Scene");
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

		// set font color and size for "Your friends:" subheading
		GUI.contentColor = Color.magenta;
		style.fontSize = 25;
		// print below name
		GUI.Label(new Rect(200, 100, 100, 20), "Your friends:");
		int y = 125;
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
