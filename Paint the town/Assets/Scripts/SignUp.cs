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
	private bool showPopUp = false;
	private string errorMessage;
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
			print(signup.downloadHandler.text);
			string[] subStrings = Regex.Split(signup.downloadHandler.text, @"[,:{}]+");

			if(subStrings[1].Trim('"') != "error"){
				PlayerPrefs.SetString("token", subStrings[2].Trim('"'));
				PlayerPrefs.Save();

				SceneManager.LoadScene("TeamAssignment");
			}else{
				showPopUp = true;
				errorMessage = subStrings[5];
			}
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

	void OnGUI(){
		if (showPopUp) {
			GUI.Window(0, new Rect((Screen.width/2)-150, (Screen.height/2)-75, 250, 200), ShowGUI, "Signup Error");
		}
	}

	void ShowGUI(int windowID) {
		// put a label to show a message to the player
		GUI.Label(new Rect(45, 40, 200, 30), errorMessage.Trim('"'));

		// You may put a button to close the pop up too
		if(Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Space)){
			signupUsername.GetComponent<InputField> ().text = "";
			signupPassword.GetComponent<InputField> ().text = "";
			signupName.GetComponent<InputField> ().text = "";
			signupLastName.GetComponent<InputField> ().text = "";
			showPopUp = false;
		}
	}

}
