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

	// Use this for initialization
	IEnumerator Start () {
		// get token stored in PlayerPrefs
		string token = "JWT " + PlayerPrefs.GetString("token", "no token");

		// GET request to server to fetch user data
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", token);
		WWW www = new WWW(userURL, null, headers);
		yield return www;

		// user data we can use for this scene
		returnData = www.text;
		subReturnStrings = returnData.Split(',');
		print ("substrings returned: " + subReturnStrings);
		getName ();

	}
	
	void getName() {
		// print first and last name in top left corner
		string[] firstNameItems = subReturnStrings[2].Split(':');
		print ("first name after : split " + firstNameItems);
		string firstName = firstNameItems [1];
		print ("index into first name alone: " + firstName);
		firstName = firstName.Replace("\"", "");
		print ("after replacing \": " + firstName);


		string[] lastNameItems = subReturnStrings[1].Split(':');
		print ("last name after : split " + lastNameItems);
		string lastName = lastNameItems [1];
		print ("index into last name alone: " + lastName);
		lastName = lastName.Replace("\"", "");
		print ("after replacing \": " + lastName);

		name = firstName + " " + lastName;
	}

	void OnGUI() {
		// set font color to black
		GUI.contentColor = Color.black;
		GUI.Label(new Rect(200, 50, 100, 20), name);

	}



}
