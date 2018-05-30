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
	public string userUrl = "https://paint-the-town.herokuapp.com/api/users";

	public Button GoToSignUpButton;
	public GameObject username;
	public GameObject password;
	private string Username;
	private string Password;
	public string[] teamInfoList;
	public Image loading;
	public Text loadingText;
	private bool loadScene = false;
	private bool showPopUp = false;

	public string redID;
	public string orangeID;
	public string yellowID;
	public string greenID;
	public string blueID;
	public string purpleID;

	public Image TitleCard;

	// set our initial state; add listener for signup button
	void Start () {

		System.Threading.Thread.Sleep(4000);

		StartCoroutine(FadeImage(true));

		PlayerPrefs.DeleteAll ();

		GoToSignUpButton.onClick.AddListener(goToSignUp);
		PlayerPrefs.SetString("main scene loaded", "false");

		loadingText.enabled = false;
		loading.enabled = false;
	}

	IEnumerator FadeImage(bool fadeAway){
		// fade from opaque to transparent
		if (fadeAway)
		{
				// loop over 1 second backwards
				for (float i = 1; i >= 0; i -= Time.deltaTime)
				{
						// set color with i as alpha
						TitleCard.color = new Color(1, 1, 1, i);
						yield return null;
				}
				TitleCard.enabled = false;
		}
	}

	// load the signup scene if player does not have account
	public void goToSignUp() {
		SceneManager.LoadScene("SignUpScene");
	}

	//
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

		}
	}

	//get the player team when they sign in before loading the game scene
	public IEnumerator setPlayercolor(){
		// request user info from the server
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW(userUrl, null, headers);
		yield return www;

		// got an error
		if(www.text == "null"){
			print(www.error);
		}else{
			// set response for later use in challenge scene
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();

			// split
			string[] teamInfo = Regex.Split(www.text, @"[,:{}]+");

			for(int x = 0; x < teamInfo.Length - 1; x ++){
				if(teamInfo[x].Trim('"') == "team"){
					print("TEAM ID: " + teamInfo[x + 1]);
					PlayerPrefs.SetString("teamID", teamInfo[x + 1].Trim('"'));
					PlayerPrefs.Save();
				} else if(teamInfo[x].Trim('"') == "level"){
					PlayerPrefs.SetString("Level", teamInfo[x + 1].Trim('"'));
					PlayerPrefs.Save();
				}
			}
			StartCoroutine("getColorFromID");
		}
		//SceneManager.LoadScene("FirstScene")
	}

	// get the team color from the server based on team ID we have
	public IEnumerator getColorFromID()
	{
		// request team info from server
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/teams", null, headers);
		yield return www;

			// got an error
			if(www.text == "null"){
				print(www.error);
			}else{
				// print what we received
				print(www.text);
				teamInfoList = Regex.Split(www.text, @"[,:{}]+");

				// iterate through info from server to get each color id
				for (int i = 0; i <= teamInfoList.Length - 1; i++) {
					print(teamInfoList[i]);
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

				PlayerPrefs.Save();
				print(PlayerPrefs.GetString("teamID", "no teamID"));

			// match up the ID we have with the correct color; set Player Prefs color
				if(PlayerPrefs.GetString("teamID", "no teamID") == redID){

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
					print("Critical error: could not find team color");
				}
				PlayerPrefs.Save();

				SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
				loading.enabled = true;
				loadingText.enabled = true;
			}
	}

	// coroutine to work around IEnumerator
	public void workAroundSignIn() {
		StartCoroutine("SigninButton");
	}

	// grab user input constantly until they enter
	public void Update() {

		// if they press enter
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (Password != "" && Username != "") {
				StartCoroutine("SigninButton");
			}
		}

		// grab input text
		Username = username.GetComponent<InputField> ().text;
		Password = password.GetComponent<InputField> ().text;

		// transition between login scene and main scene
		if(PlayerPrefs.GetString("main scene loaded", "false") == "happy"){
			SceneManager.SetActiveScene(SceneManager.GetSceneByName("FirstScene"));
			Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);
			SceneManager.UnloadSceneAsync("LoginScene");

		}

		if(loading.enabled){
			loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
		}

	}

	// popup window if popup flag is signaled for signin error from server
	void OnGUI(){
		if (showPopUp) {
			GUI.Window(0, new Rect((Screen.width/2)-150, (Screen.height/2)-75
				, 250, 200), ShowGUI, "Signin Error");
		}
	}

	// gui to show popup window
	void ShowGUI(int windowID) {
		// put a label to show a message to the player
		GUI.Label(new Rect(45, 40, 200, 30), "Invalid username or password");

		// clear the window
		if(Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Space)){
			username.GetComponent<InputField> ().text = "";
			password.GetComponent<InputField> ().text = "";
			showPopUp = false;
		}
	}
}
