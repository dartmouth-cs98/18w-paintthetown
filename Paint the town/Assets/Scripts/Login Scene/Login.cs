using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

	// the two URL strings that we need to use
	string signinURL = "https://paint-the-town.herokuapp.com/api/signin";

	public Button GoToSignUpButton;
	public GameObject username;
	public GameObject password;
	private string Username;
	private string Password;
	public string userUrl = "https://paint-the-town.herokuapp.com/api/users";
	public string[] teamInfoList;
	public string redID;
	public string blueID;

	private bool showPopUp = false;
	public string[] subReturnStrings;

	void Start () {
		GoToSignUpButton.onClick.AddListener(goToSignUp);
	}

	public void goToSignUp() {
		SceneManager.LoadScene("SignUpScene");
	}

	public IEnumerator SigninButton(){

		WWWForm f = new WWWForm();
		print(Username);
		print(Password);
		f.AddField("email", Username);
		f.AddField("password", Password);

		print ("this is the password you put in: " + Password);
		print ("this is the username you put in: " + Username);

		var test = UnityWebRequest.Post(signinURL, f);

		// Wait until the download is done
		yield return test.SendWebRequest();

		if (test.isNetworkError || test.isHttpError) {
			print ("Error downloading: " + test.error);
			print("before: " +showPopUp);
			showPopUp = true;
			print("after: " + showPopUp);

		} else {

			print ("user posted!");
			string token = test.downloadHandler.text;
			string[] subStrings = token.Split ('"');
			print(subStrings[3]);

			//setting player token on login
			PlayerPrefs.SetString("token", subStrings[3]);
			PlayerPrefs.Save();
			StartCoroutine("setPlayercolor");
			StartCoroutine("getColorFromID");
		}
	}

	//get the player team when they sign in before loading the game scene
	public IEnumerator setPlayercolor(){
		Hashtable headers = new Hashtable();
		print("You're retrieving information about the user");
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW(userUrl, null, headers);
		yield return www;
		if(www.text == "null"){
			print(www.error);
		}else{

			string[] teamInfo = Regex.Split(www.text, @"[,:{}]+");

			for(int x = 0; x < teamInfo.Length - 1; x ++){
				if(teamInfo[x].Trim('"') == "team"){
					print("TEAM ID: " + teamInfo[x + 1]);
					PlayerPrefs.SetString("teamID", teamInfo[x + 1].Trim('"'));
					PlayerPrefs.Save();
				}
			}
		}
		SceneManager.LoadScene("FirstScene");
	}

	public IEnumerator getColorFromID()
	{
		Hashtable headers = new Hashtable();
		print("You're retrieving information about teams");
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		print (PlayerPrefs.GetString ("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/teams", null, headers);
		yield return www;

			if(www.text == "null"){
				print(www.error);
			}else{
				print(www.text);
				string teamInfo = www.text;
				teamInfoList = teamInfo.Split('"');

        //
				// for (int i = 0; i <= teamInfoList.Length - 1; i++) {
				// 	print (teamInfoList [i]);
				// }
				print("THIS IS REDID: " + teamInfoList[19]);
				print("THIS IS BLUEID: " + teamInfoList[5]);

				redID = teamInfoList[19];
				blueID = teamInfoList[5];
				print("Red team ID: " + redID);
				print("Blue team ID: " + blueID);
				if( (PlayerPrefs.GetString("teamID", "no teamID")) == redID)
				{
					PlayerPrefs.SetString("color", "red");
				} else if (PlayerPrefs.GetString("teamID", "no teamID") == blueID) {
					PlayerPrefs.SetString("color", "blue");
				} else {
					print("Critical error, could not find team color");
				}
			}
	}

	public void workAroundSignIn() {
		print("You are signing in");
		StartCoroutine("SigninButton");
	}


	public void GoToSignUp() {
		SceneManager.LoadScene ("SignUpScene");
	}

	public void Update() {

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (Password != "" && Username != "") {
				StartCoroutine("SigninButton");
			}
		}

		Username = username.GetComponent<InputField> ().text;
		Password = password.GetComponent<InputField> ().text;

	}

	void OnGUI(){
		if (showPopUp) {
			GUI.Window(0, new Rect((Screen.width/2)-150, (Screen.height/2)-75
				, 250, 200), ShowGUI, "Signin Error");
		}
	}

	void ShowGUI(int windowID) {
		// put a label to show a message to the player
		GUI.Label(new Rect(45, 40, 200, 30), "Invalid username or password");

		// You may put a button to close the pop up too
		if(Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Space)){
			username.GetComponent<InputField> ().text = "";
			password.GetComponent<InputField> ().text = "";
			showPopUp = false;
		}
	}
}
