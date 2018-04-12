using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class fbLogin : MonoBehaviour {

	//get request to 'https://paint-the-town.herokuapp.com/api/auth/facebook'

	public GameObject DialogLoggedIn;
	public GameObject DialogLoggedOut;
	public GameObject DialogUsername;

	public string userURL = "https://paint-the-town.herokuapp.com/api/users";
	public bool isThereTeam;
	public string url;
	public string returnData;
	public string[] subReturnStrings;
	public string[] teamItem;
	public string[] teamInfoList;
	public string redID;
	public string blueID;


	// Use this for initialization
	void Awake()
	{
		FB.Init(SetInit, OnHideUnity);
	}

	void SetInit()
	{
		if(FB.IsLoggedIn){
			Debug.Log("Facebook is logged in");
		}else{
			Debug.Log("Facebook is not logged in");
		}
		FacebookMenus(FB.IsLoggedIn);
	}

	void OnHideUnity(bool isGameShown)
	{
		if(!isGameShown){
			Time.timeScale = 0;
		}else{
			Time.timeScale = 1;
		}
	}

	public void FBlogin()
	{
		List<string> permissions = new List<string>();
		permissions.Add("public_profile");
		FB.LogInWithReadPermissions(permissions, AuthCallBack);
	}

	//call back for the facebook login
	public void AuthCallBack(IResult result)
	{
		if(result.Error != null){
			Debug.Log(result.Error);
		}else{


      url = "" + result.ResultDictionary["access_token"];
			passToServerWorkaround();
			// startCheckFirstTimeLogin();
			// startGetColorFromID();
			// FacebookMenus(FB.IsLoggedIn);
		}
	}

	public void passToServerWorkaround(){
		StartCoroutine("passToServer");
	}

	public void startCheckFirstTimeLogin(){
		StartCoroutine("checkFirstTimeLogin");
	}

	public void startGetColorFromID(){
		StartCoroutine("getColorFromID");
	}

	public IEnumerator passToServer()
	{
		print("signing in facebook user!");
			using (WWW www = new WWW("https://paint-the-town.herokuapp.com/api/facebook/tokenize?access_token=" + url))
			{
					yield return www;
					if(www.error == null){
						string token = www.text;
						print(token);
						string[] subStrings = token.Split ('"');
						PlayerPrefs.SetString("token", subStrings[3]);
						PlayerPrefs.Save();

						print("LOOK AT THIS RIGHT HERE " + PlayerPrefs.GetString("token", "no token"));
					}else{
						print("you have a problem");
						print(www.error);
					}
					StartCoroutine("checkFirstTimeLogin");
			}

	}

	//determine whether or not the user has been signed in before
	IEnumerator checkFirstTimeLogin () {

		// get token stored in PlayerPrefs
		print("checking to see if fb user has logged in before");

		string token = "JWT " + PlayerPrefs.GetString("token", "no token");

		// POST request to server to fetch user data
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", token);
		WWW www = new WWW(userURL, null, headers);

		yield return www;

			returnData = www.text;
			subReturnStrings = returnData.Split(',');

			//THIS NEEDS TO BE LOOKED INTO
			//sometimes returning weird shit
			teamItem = subReturnStrings[6].Split(':');

			print(teamItem [0]);
			print(teamItem [1]);

			if(teamItem[1] == "null") {
				print("you are a new user");
				SceneManager.LoadScene("TeamAssignment");
			} else {
				print("you are not a new user");

				//save the teamID for later use
				PlayerPrefs.SetString("teamID", teamItem[1].Split('"')[1]);
				PlayerPrefs.Save();
			}
			StartCoroutine("getColorFromID");
	}

	public IEnumerator getColorFromID()
	{
		Hashtable headers = new Hashtable();
		print("You're retrieving information about teams IN ORDER TO GET THE COLOR");
		print("THIS IS PLAYER TOKEN " + PlayerPrefs.GetString("token", "no token"));
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW wwww = new WWW("https://paint-the-town.herokuapp.com/api/teams", null, headers);
		yield return wwww;

			if(wwww.text == "null"){
				print(wwww.error);
			}else{
				print(wwww.text);
				print("parsing strings");
				string teamInfo = wwww.text;
				teamInfoList = teamInfo.Split('"');

				redID = teamInfoList[19];
				blueID = teamInfoList[5];
				print("Red team ID: " + redID);
				print("Blue team ID: " + blueID);

				if( (PlayerPrefs.GetString("teamID", "no teamID")) == redID)
				{
					PlayerPrefs.SetString("color", "red");
					PlayerPrefs.Save();
				} else if (PlayerPrefs.GetString("teamID", "no teamID") == blueID) {
					PlayerPrefs.SetString("color", "blue");
					PlayerPrefs.Save();
				} else {
					print("Critical error, could not find team color");
				}
			}
			SceneManager.LoadScene("FirstScene");
	}

	void FacebookMenus(bool isLoggedIn)
	{
		if(isLoggedIn){
			DialogLoggedIn.SetActive(true);
			DialogLoggedOut.SetActive(false);
            SceneManager.LoadScene("FirstScene");
			FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
		}else{
			DialogLoggedIn.SetActive(false);
			DialogLoggedOut.SetActive(true);
		}
	}

	void DisplayUsername(IResult result)
	{
		Text Username = DialogUsername.GetComponent<Text>();

		if (result.Error == null){
			Username.text = "Hi there " + result.ResultDictionary ["first_name"];
		} else{
			Debug.Log(result.Error);
		}
	}
}
