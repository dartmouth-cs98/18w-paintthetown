using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;


public class Profile : MonoBehaviour {

	public string userURL = "https://paint-the-town.herokuapp.com/api/users";
	public string teamurl = "https://paint-the-town.herokuapp.com/api/teams";
	public string redID;
	public string blueID;
	public string[] teamInfoList;
	public string returnData;
	public string[] subReturnStrings;
	public string name;
	public string team;
	public string[] friendsList = new string[0];
	public string token = "";

	// Use this for initialization
	IEnumerator Start () {
		// get token stored in PlayerPrefs
		token = "JWT " + PlayerPrefs.GetString("token", "no token");

		// POST request to server to fetch user data
		Hashtable userHeaders = new Hashtable();
		userHeaders.Add("Authorization", token);
		WWW userwww = new WWW(userURL, null, userHeaders);
		yield return userwww;

		// user data we can use for this scene
		returnData = userwww.text;
		subReturnStrings = returnData.Split(',');
		for (int i = 0; i < subReturnStrings.Length; i++) {
			print (subReturnStrings [i]);
		}

		Hashtable teamHeaders = new Hashtable();
		teamHeaders.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW teamwww = new WWW(teamurl, null, teamHeaders);
		yield return teamwww;

		if(teamwww.text == "null"){
			print(teamwww.error);
		}else{
			print(teamwww.text);
			string teamInfo = teamwww.text;
			teamInfoList = teamInfo.Split('"');
			redID = teamInfoList[19];
			blueID = teamInfoList[5];
		}

		getName ();
		getTeam ();
		getFriends ();

	}

	void getName() {
		// grab first name
		string[] firstNameItems = subReturnStrings[4].Split(':');
		string firstName = firstNameItems [1];
		firstName = firstName.Replace("\"", "");

		// grab last name
		string[] lastNameItems = subReturnStrings[3].Split(':');
		string lastName = lastNameItems [1];
		lastName = lastName.Replace("\"", "");

		// concatenate
		name = firstName + " " + lastName;
	}

	void getTeam() {
		// grab team / color
		string[] teamItem = subReturnStrings[6].Split(':');

		if (teamItem [1] == "null") {
			team = "You are not assigned to a team yet!";
		} else {
			team = teamItem [1];
			team = team.Replace("\"", "");
			if (team == redID) {
				team = "Red";
			} else if (team == blueID) {
				team = "Blue";
			} else {
				team = "You are not assigned to a team yet!";
			}
		}

	}

	void getFriends() {
		// grab friends
		string[] friendsItem = subReturnStrings[7].Split(':');

		if (friendsItem[1] != "[]") {
			string friendsListString = friendsItem [1];
			friendsListString.Replace ("[", "");
			friendsListString.Replace ("]", "");
			friendsList = friendsListString.Split (',');

			foreach (string friend in friendsList) {
				friend.Replace("\"", "");
			}
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
			//SceneManager.LoadScene ("LoginScene");
			print("need to get correct new name of login scene");
		}

		// set font color and size for Name
		GUI.contentColor = Color.black;
		style.fontSize = 35;
		// print name in top left corner
		GUI.Label(new Rect(200, 50, 100, 20), name, style);

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
