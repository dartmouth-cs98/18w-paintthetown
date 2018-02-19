using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class testLoginScreen : MonoBehaviour {

	// the two URL strings that we need to use
	//	string userURL = "https://paint-the-town.herokuapp.com/api/signin";
	//	string localURL= "https://paint-the-town.herokuapp.com/api/signup";

	public GameObject username;
	public GameObject password;
	private string Username;
	private string Password;
	public GameObject signupUsername;
	public GameObject signupPassword;
	private string SignupUsername;
	private string SignupPassword;

	public void SigninButton(){
		print ("You want to sign in!");
	}
		
	public void RegisterButton(){
		print ("You want to register!");
	}
	public void Update() {
		
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (Password != "" && Username != "") {
				SigninButton ();
			}else if (SignupPassword != "" && SignupUsername != "") {
				RegisterButton();
			}
		}

		Username = username.GetComponent<InputField> ().text;
		Password = password.GetComponent<InputField> ().text;
		SignupUsername = signupUsername.GetComponent<InputField> ().text;
		SignupPassword = signupPassword.GetComponent<InputField> ().text;

	}
}
