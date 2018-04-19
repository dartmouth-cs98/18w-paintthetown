using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;


public class SignUp : MonoBehaviour {
	string signupURL= "https://paint-the-town.herokuapp.com/api/signup";
	public GameObject signupUsername;
	public GameObject signupPassword;
	private string SignupUsername;
	private string SignupPassword;
	public GameObject signupName;
	public GameObject signupLastName;
	private string SignupName;
	private string SignupLastName;
	public string userUrl = "https://paint-the-town.herokuapp.com/api/users";
	public string[] teamInfoList;
	public Button GoToLoginButton;
	public Button SignUpButton;

	public string[] subReturnStrings;

	void Start () {
		GoToLoginButton.onClick.AddListener(goToLogin);
	}

	public IEnumerator RegisterButton(){

		WWWForm signupform = new WWWForm();

		signupform.AddField("email", SignupUsername);
		signupform.AddField("password", SignupPassword);
		signupform.AddField("name", SignupName);
		signupform.AddField("lastName", SignupLastName);

		var signup = UnityWebRequest.Post(signupURL, signupform);

		// Wait until the download is done
		yield return signup.SendWebRequest();

		if (signup.isNetworkError || signup.isHttpError)
		{
			print("Error downloading: " + signup.error);
		}
		else
		{
			print("user signed up!");
			string token = signup.downloadHandler.text;
			string[] subStrings = token.Split ('"');
			PlayerPrefs.SetString("token", subStrings[3]);
			PlayerPrefs.Save();

			SceneManager.LoadScene("TeamAssignment");
		}
	}

	public void workAroundSignUp() {
		print ("You want to register!");
		StartCoroutine("RegisterButton");
	}

	public void goToLogin() {
		SceneManager.LoadScene("LoginScene");
	}


	// called once per frame
	public void Update() {

		if (SignUpButton) {
			if (SignupPassword != "" && SignupUsername != "") {
				StartCoroutine("RegisterButton");
			}
		}
			
		SignupUsername = signupUsername.GetComponent<InputField> ().text;
		SignupPassword = signupPassword.GetComponent<InputField> ().text;
		SignupName = signupName.GetComponent<InputField> ().text;
		SignupLastName = signupLastName.GetComponent<InputField> ().text;


	}

}

