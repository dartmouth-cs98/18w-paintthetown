using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class ProfileEdit : MonoBehaviour {
	public InputField FirstNameInput;
	public InputField LastNameInput;
	public string originalFirstName;
	public string originalLastName;
	public string newFirstName;
	public string newLastName;

	// Use this for initialization
	void Start () {
		originalFirstName = PlayerPrefs.GetString("firstName", "no first name");
		originalLastName = PlayerPrefs.GetString("lastName", "no last name");
		FirstNameInput.placeholder.GetComponent<Text>().text = originalFirstName;
		LastNameInput.placeholder.GetComponent<Text>().text = originalLastName;
	}
		

	IEnumerator updateServer() {
		print ("newFirstName: " + newFirstName);
		print ("newLastName: " + newLastName);


		// make a form of new changes
		WWWForm updateform = new WWWForm();
		updateform.AddField("name", newFirstName);
		updateform.AddField("lastName", newLastName);

		// make authentication header
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		// post to update user info
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users/updateInfo", updateform.data, headers);
		yield return www;

		if (www.error != null) {
			print("Error downloading: ");
		} else {
			// save new player info
			PlayerPrefs.SetString("firstName", newFirstName);
			PlayerPrefs.SetString("lastName", newLastName);
			PlayerPrefs.Save();
			// go back to profile scene
			SceneManager.LoadScene ("ProfileScene");
		}
	}

	// Update is called once per frame
	void Update () {
		// if the user enters some data in the first or last name input fields
		if (Input.GetKeyDown (KeyCode.Return)) {
			// if they are actual changes
			if (originalFirstName != FirstNameInput.text || originalLastName != LastNameInput.text) {
				// is the first name changed?
				if (FirstNameInput.text == "") {
					newFirstName = originalFirstName;
				} else if ((FirstNameInput.text != "") & (originalFirstName != FirstNameInput.text)) {
					// get the new one
					newFirstName = FirstNameInput.text;
				} else {	// keep it as original; still need a placehold otherwise it will turn null
					newFirstName = originalFirstName;
				}

				// is the last name changed?
				if (LastNameInput.text == "") {
					newLastName = originalLastName;
				} else if ((LastNameInput.text != "") & (originalLastName != LastNameInput.text)) {
					// get the new one
					newLastName = LastNameInput.text;
				} else {	// keep it as original; still need a placeholder otherwise it will turn null
					newLastName = originalLastName;
				}

				// update the server with the changes
				StartCoroutine("updateServer");
			} else {
				// if no actual changes, just load the profile scene
				SceneManager.LoadScene ("ProfileScene");
			}
		}
	}

	void OnGUI() {
		// if "Save Changes" is clicked (same as if they pressed "enter" in Update() function
		if (GUI.Button (new Rect (400, 120, 100, 50), "Save Changes")) {
			// if the user made actual changes
			if ((originalFirstName != FirstNameInput.text) || (originalLastName != LastNameInput.text)) {
				// is the first name changed?
				if (FirstNameInput.text == "") {
					newFirstName = originalFirstName;
				} else if ((FirstNameInput.text != "") & (originalFirstName != FirstNameInput.text)) {
					// get the new one
					newFirstName = FirstNameInput.text;
				} else {	// keep it as original; still need a placehold otherwise it will turn null
					newFirstName = originalFirstName;
				}

				// is the last name changed?
				if (LastNameInput.text == "") {
					newLastName = originalLastName;
				} else if ((LastNameInput.text != "") & (originalLastName != LastNameInput.text)) {
					// get the new one
					newLastName = LastNameInput.text;
				} else {	// keep it as original; still need a placeholder otherwise it will turn null
					newLastName = originalLastName;
				}

				// update the server with the changes
				StartCoroutine("updateServer");
			} else {
				// if no actual changes, just load the profile scene
				SceneManager.LoadScene ("ProfileScene");
			}

		}

		// if "Done" is clicked
		if (GUI.Button (new Rect (400, 70, 75, 30), "Done")) {
			// load the profile scene without saving changes
			SceneManager.LoadScene ("ProfileScene");
		}
	}
}
