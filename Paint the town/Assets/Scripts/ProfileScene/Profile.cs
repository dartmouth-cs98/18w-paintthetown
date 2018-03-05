using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;



public class Profile : MonoBehaviour {

	public string userURL = "https://paint-the-town.herokuapp.com/api/users";
	public string returnData;
	public string[] subReturnStrings;
	public string name;
	public string team;
	public string[] friendsList;
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
		string[] firstNameItems = subReturnStrings[2].Split(':');
		string firstName = firstNameItems [1];
		firstName = firstName.Replace("\"", "");

		// grab last name
		string[] lastNameItems = subReturnStrings[1].Split(':');
		string lastName = lastNameItems [1];
		lastName = lastName.Replace("\"", "");

		// concatenate
		name = firstName + " " + lastName;
	}

	void getTeam() {
		// grab team / color
		string[] teamItem = subReturnStrings[7].Split(':');
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
		string[] friendsItem = subReturnStrings[8].Split(':');
		if (friendsItem[1] == null || friendsItem[1].Length == 0) {
			print ("You have no friends");
			friendsList [0] = "jo shmo";
			friendsList [1] = "billy bob";
			friendsList [2] = "hey you";
			foreach (var friend in friendsList) {
				friend.Replace("\"", "");
				print ("friend: " + friend);
			}
		} else {
			int i = 0;
			print(friendsItem[1]);
			// foreach (string friend in friendsItem[1]) {
			// 	friend.Replace("\"", "");
			// 	print ("friend: " + friend);
			// 	friendsList[i] = friend;
			// }
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
		GUI.Label(new Rect(200, 150, 100, 20), "Your friends:");
		int y = 250;
		foreach(var friend in friendsList){
			GUI.Label(new Rect(200, y, 100, 20), friend);
			y += 50;
		}

	}



}
