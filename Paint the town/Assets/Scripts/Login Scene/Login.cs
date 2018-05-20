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
	public string orangeID;
	public string yellowID;
	public string greenID;
	public string blueID;
	public string purpleID;

	private bool loadScene = false;

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

		loadScene = true;
		//SceneManager.LoadScene("FirstScene");
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
				// string teamInfo = www.text;
				teamInfoList = Regex.Split(www.text, @"[,:{}]+");


				for (int i = 0; i <= teamInfoList.Length - 1; i++) {
					// print (teamInfoList [i]);
					if(teamInfoList [i].Trim('"') == "red"){
						redID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "orange"){
						orangeID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "yellow"){
						yellowID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "green"){
						greenID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "blue"){
						blueID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "purple"){
						purpleID = teamInfoList [i - 5].Trim('"');
					}
				}
				print("orange team ID" + orangeID);
				print("Red team ID: " + redID);
				print("yellow team ID: " + yellowID);
				print("Green team ID: " + greenID);
				print("purple team ID: " + purpleID);
				print("Blue team ID: " + blueID);

				print(PlayerPrefs.GetString("teamID", "no teamID"));

				if( (PlayerPrefs.GetString("teamID", "no teamID")) == redID)
				{
					PlayerPrefs.SetString("color", "red");
				} else if (PlayerPrefs.GetString("teamID", "no teamID") == orangeID) {
					PlayerPrefs.SetString("color", "orange");
				} else if (PlayerPrefs.GetString("teamID", "no teamID") == yellowID) {
					PlayerPrefs.SetString("color", "yellow");
				} else if (PlayerPrefs.GetString("teamID", "no teamID") == greenID) {
					PlayerPrefs.SetString("color", "green");
				} else if (PlayerPrefs.GetString("teamID", "no teamID") == blueID) {
					PlayerPrefs.SetString("color", "blue");
				} else if (PlayerPrefs.GetString("teamID", "no teamID") == purpleID) {
					PlayerPrefs.SetString("color", "purple");
				}else{
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

		if(loadScene == true){
			loadScene = false;
			StartCoroutine(LoadNewScene());

		}

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

	IEnumerator LoadNewScene() {

		AsyncOperation async = Application.LoadLevelAsync("FirstScene");

		 while (PlayerPrefs.GetString("main scene loaded", "scene not loaded") == "scene not loaded") {

			 yield return null;
		 }
	}

}
