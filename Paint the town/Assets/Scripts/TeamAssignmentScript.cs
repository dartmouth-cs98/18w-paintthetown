using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class TeamAssignmentScript : MonoBehaviour {

	// team info url
	public string url = "https://paint-the-town.herokuapp.com/api/teams";
	public string[] teamInfoList;

	// color IDs
	public string redID;
	public string blueID;
	public string greenID;
	public string yellowID;
	public string purpleID;
	public string orangeID;

	public Image loading;
	public Text loadingText;
	private ShowTextBox myTB;

	// tutorial messages
	private string welcome_1 = "Welcome to the world of Paint the Town!";
	private string welcome_2 = "In this game you will compete alongside your teammates to control as much of the world as you can";
	private string welcome_3 = "But before anything else, you have to choose a team!";
	private string welcome_4 = "Tap on the paint splotch of the color team you’d like to join.";

  IEnumerator Start() {
		// set initial state when entering team assignment scene
		PlayerPrefs.SetString("Tutorial", "true");
		PlayerPrefs.SetString("main scene loaded", "false");
		loadingText.enabled = false;
		loading.enabled = false;

		// request info about teams from the server
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		print (PlayerPrefs.GetString ("token", "no token"));
		WWW www = new WWW(url, null, headers);
		yield return www;

			// got an error
			if (www.error != null){
				print(www.error);
			}else{
				// show what we received
				print(www.text);

				// parse out all of the information
				teamInfoList = Regex.Split(www.text, @"[,:{}]+");
				// grab each team color ID
				for (int i = 3; i <= teamInfoList.Length - 1; i++) {
					if(teamInfoList [i].Trim('"') == "red"){
						redID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "blue"){
						blueID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "yellow"){
						yellowID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "orange"){
						orangeID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "purple"){
						purpleID = teamInfoList [i - 5].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "green"){
						greenID = teamInfoList [i - 5].Trim('"');
					}
				}
			}

			myTB = GetComponent<ShowTextBox>();
			string[] array = new string[4];
			array[0] = welcome_1;
			array[1] = welcome_2;
			array[2] = welcome_3;
			array[3] = welcome_4;
			myTB.show(array);
  }

	public void startAssignPlayerRED()
	{
		if(myTB.image.enabled == false){
			StartCoroutine("assignPlayerRED");
		}
	}

	public void startAssignPlayerORANGE()
	{
		if(myTB.image.enabled == false){
			StartCoroutine("assignPlayerORANGE");
		}
	}

	public void startAssignPlayerYELLOW()
	{
		if(myTB.image.enabled == false){
			StartCoroutine("assignPlayerYELLOW");
		}
	}

	public void startAssignPlayerGREEN()
	{
		if(myTB.image.enabled == false){
			StartCoroutine("assignPlayerGREEN");
		}
	}

	public void startAssignPlayerBLUE()
	{
		if(myTB.image.enabled == false){
			StartCoroutine("assignPlayerBLUE");
		}
	}

	public void startAssignPlayerPURPLE()
	{
		if(myTB.image.enabled == false){
			StartCoroutine("assignPlayerPURPLE");
		}
	}

	IEnumerator assignPlayerRED()
	{
		print ("You're assigning a player to team RED");

		// create form to send to server
		WWWForm signupform = new WWWForm();
		signupform.AddField("team", redID);

		// send to server to sign up for red
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		// got an error
		if (www.error != null)
		{
			print("Error downloading RED: " + www.error);
		}
		else
		{
			// success; set Player Prefs
			print("user assigned to RED!!");
			PlayerPrefs.SetString("teamID", redID);
			PlayerPrefs.SetString("color", "red");
			PlayerPrefs.Save();

			// load next
			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
		}
	}

	IEnumerator assignPlayerORANGE()
	{
		print ("You're assigning a player to team ORANGE");

		// create form to send to server
		WWWForm signupform = new WWWForm();
		signupform.AddField("team", orangeID);

		// send to server to sign up for orange
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		// got an error
		if (www.error != null)
		{
			print("Error downloading ORANGE: " + www.error);
		}
		else
		{
			// success; set Player Prefs
			print("user assigned to ORANGE!!");
			PlayerPrefs.SetString("teamID", orangeID);
			PlayerPrefs.SetString("color", "orange");
			PlayerPrefs.Save();

			// load next
			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
		}
	}

	IEnumerator assignPlayerYELLOW()
	{
		print ("You're assigning a player to team YELLOW");

		// create form to send to server
		WWWForm signupform = new WWWForm();
		signupform.AddField("team", yellowID);

		// send to server to sign up for yellow
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		// got an error
		if (www.error != null)
		{
			print("Error downloading YELLOW: " + www.error);
		}
		else
		{
			// success; set Player Prefs
			print("user assigned to YELLOW!!");
			PlayerPrefs.SetString("teamID", yellowID);
			PlayerPrefs.SetString("color", "yellow");
			PlayerPrefs.Save();

			// load next
			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
		}
	}

	IEnumerator assignPlayerGREEN()
	{
		print ("You're assigning a player to team GREEN");

		// create form to send to server
		WWWForm signupform = new WWWForm();
		signupform.AddField("team", greenID);

		// send to server to sign up for green
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		// got an error
		if (www.error != null)
		{
			print("Error downloading GREEN: " + www.error);
		}
		else
		{
			// success; set Player Prefs
			print("user assigned to GREEN!!");
			PlayerPrefs.SetString("teamID", greenID);
			PlayerPrefs.SetString("color", "green");
			PlayerPrefs.Save();

			// load next
			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
		}
	}

	IEnumerator assignPlayerBLUE()
	{
		print ("You're assigning a player to team BLUE");

		// create form to send to server
		WWWForm signupform = new WWWForm();
		signupform.AddField("team", blueID);

		// sned to server to sign up for blue
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		// got an error
		if (www.error != null)
		{
			print("Error downloading BLUE: " + www.error);
		}
		else
		{
			// success; set Player Prefs
			print("user assigned to BLUE!!");
			PlayerPrefs.SetString("teamID", blueID);
			PlayerPrefs.SetString("color", "blue");
			PlayerPrefs.Save();

			// load next
			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
		}
	}

	IEnumerator assignPlayerPURPLE()
	{
		print ("You're assigning a player to team ORANGE");

		// create form to send to server
		WWWForm signupform = new WWWForm();
		signupform.AddField("team", purpleID);

		// send to server to sign up for purple
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		// got an error
		if (www.error != null)
		{
			print("Error downloading PURPLE: " + www.error);
		}
		else
		{
			// success; set Player Prefs
			print("user assigned to PURPLE!!");
			PlayerPrefs.SetString("teamID", purpleID);
			PlayerPrefs.SetString("color", "purple");
			PlayerPrefs.Save();

			// load next
			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
		}
	}

	// continuously update active scene status
	public void Update() {
		if(PlayerPrefs.GetString("main scene loaded", "false") == "happy"){
			SceneManager.SetActiveScene(SceneManager.GetSceneByName("FirstScene"));
			Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);
			SceneManager.UnloadSceneAsync("TeamAssignment");

		}
		if(loading.enabled){
			loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
		}
	}
}
