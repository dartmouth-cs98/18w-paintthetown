using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class TeamAssignmentScript : MonoBehaviour {

	public string url = "https://paint-the-town.herokuapp.com/api/teams";
	public string[] teamInfoList;
	public string redID;
	public string blueID;
	public string greenID;
	public string yellowID;
	public string purpleID;
	public string orangeID;

	public Image loading;
	public Text loadingText;

	private ShowTextBox myTB;

	private string welcome_1 = "Welcome to the world of Paint the Town!";
	private string welcome_2 = "In this game you will compete alongside your teammates to control as much of the world as you can";
	private string welcome_3 = "But before anything else, you have to choose a team!";
	private string welcome_4 = "Tap on the paint splotch of the color team you’d like to join.";

  IEnumerator Start()
  {

		PlayerPrefs.SetString("Tutorial", "true");
		PlayerPrefs.SetString("main scene loaded", "false");
		loadingText.enabled = false;
		loading.enabled = false;
		Hashtable headers = new Hashtable();

		print("You're retrieving information about teams");
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		print (PlayerPrefs.GetString ("token", "no token"));
		WWW www = new WWW(url, null, headers);
		yield return www;

			if (www.error != null){
				print(www.error);
			}else{
				print(www.text);
				print("parsing strings");
				teamInfoList = Regex.Split(www.text, @"[,:{}]+");

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
					print("Red team ID: " + redID);
					print("orange team ID: " + orangeID);
					print("yellow team ID: " + yellowID);
					print("green team ID: " + greenID);
					print("blue team ID: " + blueID);
					print("purple team ID: " + purpleID);
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

		WWWForm signupform = new WWWForm();

		signupform.AddField("team", redID);

		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		if (www.error != null)
		{
			print("Error downloading RED: " + www.error);
		}
		else
		{
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();
			print ("in player prefs ChallengeChunk: " + PlayerPrefs.GetString ("ChallengeChunk", "nothing"));


			print("user assigned to RED!!");
			PlayerPrefs.SetString("teamID", redID);
			PlayerPrefs.SetString("color", "red");
			PlayerPrefs.Save();

			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
			//SceneManager.LoadScene("FirstScene");
		}
	}

	IEnumerator assignPlayerORANGE()
	{
		print ("You're assigning a player to team ORANGE");

		WWWForm signupform = new WWWForm();

		signupform.AddField("team", orangeID);

		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		if (www.error != null)
		{
			print("Error downloading ORANGE: " + www.error);
		}
		else
		{
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();
			print ("in player prefs ChallengeChunk: " + PlayerPrefs.GetString ("ChallengeChunk", "nothing"));


			PlayerPrefs.SetString("teamID", orangeID);
			PlayerPrefs.SetString("color", "orange");
			PlayerPrefs.Save();
			print("user assigned to ORANGE!!");

			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
			//SceneManager.LoadScene("FirstScene");
		}
	}

	IEnumerator assignPlayerYELLOW()
	{
		print ("You're assigning a player to team YELLOW");

		WWWForm signupform = new WWWForm();

		signupform.AddField("team", yellowID);

		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		if (www.error != null)
		{
			print("Error downloading YELLOW: " + www.error);
		}
		else
		{
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();
			print ("in player prefs ChallengeChunk: " + PlayerPrefs.GetString ("ChallengeChunk", "nothing"));


			PlayerPrefs.SetString("teamID", yellowID);
			PlayerPrefs.SetString("color", "yellow");
			PlayerPrefs.Save();
			print("user assigned to YELLOW!!");

			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
			//SceneManager.LoadScene("FirstScene");
		}
	}

	IEnumerator assignPlayerGREEN()
	{
		print ("You're assigning a player to team GREEN");

		WWWForm signupform = new WWWForm();

		signupform.AddField("team", greenID);

		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		if (www.error != null)
		{
			print("Error downloading GREEN: " + www.error);
		}
		else
		{
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();
			print ("in player prefs ChallengeChunk: " + PlayerPrefs.GetString ("ChallengeChunk", "nothing"));


			PlayerPrefs.SetString("teamID", greenID);
			PlayerPrefs.SetString("color", "green");
			PlayerPrefs.Save();
			print("user assigned to GREEN!!");

			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
			//SceneManager.LoadScene("FirstScene");
		}
	}

	IEnumerator assignPlayerBLUE()
	{
		print ("You're assigning a player to team BLUE");

		WWWForm signupform = new WWWForm();

		signupform.AddField("team", blueID);

		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		if (www.error != null)
		{
			print("Error downloading BLUE: " + www.error);
		}
		else
		{
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();
			print ("in player prefs ChallengeChunk: " + PlayerPrefs.GetString ("ChallengeChunk", "nothing"));


			PlayerPrefs.SetString("teamID", blueID);
			PlayerPrefs.SetString("color", "blue");
			PlayerPrefs.Save();
			print("user assigned to BLUE!!");

			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
			//SceneManager.LoadScene("FirstScene");
		}
	}

	IEnumerator assignPlayerPURPLE()
	{
		print ("You're assigning a player to team ORANGE");

		WWWForm signupform = new WWWForm();

		signupform.AddField("team", purpleID);

		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW("https://paint-the-town.herokuapp.com/api/users", signupform.data, headers);
		yield return www;

		if (www.error != null)
		{
			print("Error downloading PURPLE: " + www.error);
		}
		else
		{
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();
			print ("in player prefs ChallengeChunk: " + PlayerPrefs.GetString ("ChallengeChunk", "nothing"));


			PlayerPrefs.SetString("teamID", purpleID);
			PlayerPrefs.SetString("color", "purple");
			PlayerPrefs.Save();
			print("user assigned to PURPLE!!");

			SceneManager.LoadScene("FirstScene", LoadSceneMode.Additive);
			loading.enabled = true;
			loadingText.enabled = true;
			//SceneManager.LoadScene("FirstScene");
		}
	}

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
