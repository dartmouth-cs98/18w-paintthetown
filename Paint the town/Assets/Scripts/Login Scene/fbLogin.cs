using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class fbLogin : MonoBehaviour {


	public GameObject DialogLoggedIn;
	public GameObject DialogLoggedOut;
	public GameObject DialogUsername;


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

	void AuthCallBack(IResult result)
	{
		if(result.Error != null){
			Debug.Log(result.Error);
		}else{
			if(FB.IsLoggedIn){
				Debug.Log("Haha! Facebook is logged in");
			}else{
				Debug.Log("Hehe! Facebook is not logged in");
			}
			FacebookMenus(FB.IsLoggedIn);
		}
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
