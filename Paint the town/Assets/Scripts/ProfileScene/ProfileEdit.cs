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
	public string token = "";
	public string returnData;
	public string[] subReturnStrings;
	public string userURL = "https://paint-the-town.herokuapp.com/api/users";

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

		getName ();
		FirstNameInput.placeholder.GetComponent<Text>().text = originalFirstName;
		LastNameInput.placeholder.GetComponent<Text>().text = originalLastName;


	}

	void getName() {
		// grab first name
		string[] firstNameItems = subReturnStrings[4].Split(':');
		originalFirstName = firstNameItems [1];
		originalFirstName = originalFirstName.Replace("\"", "");

		// grab last name
		string[] lastNameItems = subReturnStrings[3].Split(':');
		originalLastName = lastNameItems [1];
		originalLastName = originalLastName.Replace("\"", "");
	}

	void updateServer() {
		print("tell server here");
		WWWForm updateform = new WWWForm();

		updateform.AddField("name", newFirstName);
		updateform.AddField("lastName", newLastName);

		/* SERVER NEEDS TO BE ABLE TO DO THIS
		var update = UnityWebRequest.Post(userURL, updateform);
		// Wait until the download is done
		yield return update.SendWebRequest();
		if (update.isNetworkError || update.isHttpError) {
			print("Error downloading: " + update.error);
		}
		*/

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (originalFirstName != FirstNameInput.text || originalLastName != LastNameInput.text) {
				newFirstName = FirstNameInput.text;
				newLastName = LastNameInput.text;
				updateServer ();
				SceneManager.LoadScene ("ProfileScene");
			}
		}
			
	}

	void OnGUI() {
		if (GUI.Button (new Rect (400, 120, 75, 50), "Save Changes")) {
			// load the edit profile scene
			if (originalFirstName != FirstNameInput.text || originalLastName != LastNameInput.text) {
				newFirstName = FirstNameInput.text;
				newLastName = LastNameInput.text;
				updateServer ();
			}
			SceneManager.LoadScene ("ProfileScene");
		}

		if (GUI.Button (new Rect (400, 70, 75, 30), "Done")) {
			// load the profile scene without saving changes
			SceneManager.LoadScene ("ProfileScene");
		}
	}
}
