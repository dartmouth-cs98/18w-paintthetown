using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;



public class Profile : MonoBehaviour {

	public string userURL = "https://paint-the-town.herokuapp.com/api/users";
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
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", token);
		WWW www = new WWW(userURL, null, headers);
		yield return www;

		// user data we can use for this scene
		returnData = www.text;
		subReturnStrings = returnData.Split(',');
		foreach(var item in subReturnStrings) {
				print(item.ToString());
		}

		getName ();
		getTeam ();
		getFriends ();

	}

	void getName() {
		// grab first name
		string[] firstNameItems = subReturnStrings[3].Split(':');
		string firstName = firstNameItems [1];
		firstName = firstName.Replace("\"", "");

		// grab last name
		string[] lastNameItems = subReturnStrings[2].Split(':');
		string lastName = lastNameItems [1];
		lastName = lastName.Replace("\"", "");

		// concatenate
		name = firstName + " " + lastName;
	}

	void getTeam() {
		// grab team / color
		string[] teamItem = subReturnStrings[8].Split(':');
		if (teamItem [1] == "") {
			print ("not assigned to a team");
			team = "not assigned";
		} else {
			team = teamItem [1];
			team = team.Replace("\"", "");
			print ("team: " + team);
		}

	}

	void getFriends() {
		// grab friends
		string[] friendsItem = subReturnStrings[9].Split(':');

		if (friendsItem[1] == "[]") {
			print ("You have no friends");
			//friendsList = new string[]{"jim bob", "mary sue", "plain jane"};
		} else {
			string friendsListString = friendsItem [1];
			friendsListString.Replace ("[", "");
			friendsListString.Replace ("]", "");
			friendsList = friendsListString.Split (',');

			foreach (string friend in friendsList) {
				friend.Replace("\"", "");
				print ("friend: " + friend);
			}
		}
	}

	void OnGUI() {
		// set font color to black
		GUI.contentColor = Color.black;
		// print name in top left corner
		GUI.Label(new Rect(200, 50, 100, 20), name);

		// set font color to team color?
		//print team in top right corner
		GUI.Label(new Rect(500, 50, 100, 20), team);

		// set font color to black
		GUI.contentColor = Color.black;
		GUI.Label(new Rect(200, 100, 100, 20), "Your friends:");
		int y = 125;
		if (friendsList.Length != 0) {
			foreach (var friend in friendsList) {
				GUI.Label (new Rect (200, y, 100, 20), friend);
				y += 25;
			}
		} else {
			GUI.Label (new Rect (200, y, 1000, 20), "No friends yet! Connect to Facebook to import friends.");
		}
	}



}
