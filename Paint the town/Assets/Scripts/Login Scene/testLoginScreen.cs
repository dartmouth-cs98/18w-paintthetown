using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class testLoginScreen : MonoBehaviour {

	// the two URL strings that we need to use
	string signinURL = "https://paint-the-town.herokuapp.com/api/signin";
	string signupURL= "https://paint-the-town.herokuapp.com/api/signup";

	public GameObject username;
	public GameObject password;
	private string Username;
	private string Password;
	public GameObject signupUsername;
	public GameObject signupPassword;
	private string SignupUsername;
	private string SignupPassword;
	public GameObject signupName;
	public GameObject signupLastName;
	private string SignupName;
	private string SignupLastName;

	public IEnumerator SigninButton(){

		WWWForm f = new WWWForm();

		f.AddField("email", Username);
		f.AddField("password", Password);

		print ("this is the password you put in: " + Password);
		print ("this is the username you put in: " + Username);

		var test = UnityWebRequest.Post(signinURL, f);

		// Wait until the download is done
		yield return test.SendWebRequest();

		print (test.isHttpError);
		if (test.isNetworkError || test.isHttpError) {
			print ("Error downloading: " + test.error);
		} else {
			// show the highscores
			print ("user posted!");
			string token = test.downloadHandler.text;
			string[] subStrings = token.Split ('"');

			print (subStrings [3]);
		}

	}
		
	public IEnumerator RegisterButton(){
		print ("You want to register!");

		WWWForm signupform = new WWWForm();

		signupform.AddField("email", SignupUsername);
		signupform.AddField("password", SignupPassword);
		signupform.AddField("name", SignupName);
		signupform.AddField("lastName", SignupName);

		var signup = UnityWebRequest.Post(signupURL, signupform);

		// Wait until the download is done
		yield return signup.SendWebRequest();

		if (signup.isNetworkError || signup.isHttpError)
		{
			print("Error downloading: " + signup.error);
		}
		else
		{
			// show the highscores
			print("user signed up!");
			string token = signup.downloadHandler.text;
			string[] subStrings = token.Split ('"');

			print(subStrings[3]);
			// if we want to do something with the token
			// string getToken = "JWT " + subStrings[3];
		}

	}

	public void workAroundSignIn() {
		StartCoroutine("SigninButton");
	}

	public void workAroundSignUp() {
		StartCoroutine("RegisterButton");
	}

	public void Update() {
		
		if (Input.GetKeyDown (KeyCode.Return)) {
			print ("haha");
			if (Password != "" && Username != "") {
				print ("hello!");
				StartCoroutine("SigninButton");
			}else if (SignupPassword != "" && SignupUsername != "") {
				StartCoroutine("RegisterButton");
			}
		}

		Username = username.GetComponent<InputField> ().text;
		Password = password.GetComponent<InputField> ().text;
		SignupUsername = signupUsername.GetComponent<InputField> ().text;
		SignupPassword = signupPassword.GetComponent<InputField> ().text;
		SignupName = signupName.GetComponent<InputField> ().text;
		SignupLastName = signupLastName.GetComponent<InputField> ().text;

	}
}
