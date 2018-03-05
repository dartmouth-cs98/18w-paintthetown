using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class fbLogin : MonoBehaviour {

	//get request to 'https://paint-the-town.herokuapp.com/api/auth/facebook'

	public GameObject DialogLoggedIn;
	public GameObject DialogLoggedOut;
	public GameObject DialogUsername;

	public string url;
	public string token;
	public string returnData;
	public string userURL = "https://paint-the-town.herokuapp.com/api/users";
	public string[] subReturnStrings;


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

	public void AuthCallBack(IResult result)
	{
		if(result.Error != null){
			Debug.Log(result.Error);
		}else{

			print("this is your access token");
			// print(result.ResultDictionary["access_token"]);
			// print(result);
			url = "" + result.ResultDictionary["access_token"];
			print(url);
			passToServerWorkaround();

			if(FB.IsLoggedIn){
				//Debug.Log("Haha! Facebook is logged in");
			}else{
				//Debug.Log("Hehe! Facebook is not logged in");
			}

			FacebookMenus(FB.IsLoggedIn);

		}
	}

	public void passToServerWorkaround()
	{
		StartCoroutine("passToServer");
	}

	public IEnumerator passToServer()
	{
		print("hello!");
			using (WWW www = new WWW("https://paint-the-town.herokuapp.com/api/facebook/tokenize?access_token=" + url))
			{
					yield return www;
					if(www.error == null){

						print(www.text);

						string token = www.text;
						string[] subStrings = token.Split ('"');
						print(subStrings[3]);
						PlayerPrefs.SetString("token", subStrings[3]);
						PlayerPrefs.Save();
						startIsThereATeam();

						SceneManager.LoadScene("FirstScene");

					}else{
						print("you have a problem");
						print(www.error);
					}
			}
	}

	public IEnumerator isThereATeam()
	{
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
	}

	public void startIsThereATeam()
	{
		token = PlayerPrefs.GetString("token", "no token");
		StartCoroutine("getBuildingID");
	}

	void FacebookMenus(bool isLoggedIn)
	{
		if(isLoggedIn){
			DialogLoggedIn.SetActive(true);
			DialogLoggedOut.SetActive(false);
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
