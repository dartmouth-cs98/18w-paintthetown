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

  IEnumerator Start()
  {
		Hashtable headers = new Hashtable();
		print("You're retrieving information about teams");
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		print (PlayerPrefs.GetString ("token", "no token"));
		WWW www = new WWW(url, null, headers);
		yield return www;

			if(www.text == "null"){
				print(www.error);
			}else{
				print(www.text);
				print("parsing strings");
				teamInfoList = Regex.Split(www.text, @"[,:{}]+");

				for (int i = 3; i <= teamInfoList.Length - 1; i++) {
					if(teamInfoList [i].Trim('"') == "red"){
						redID = teamInfoList [i - 2].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "blue"){
						blueID = teamInfoList [i - 2].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "yellow"){
						yellowID = teamInfoList [i - 2].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "orange"){
						orangeID = teamInfoList [i - 2].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "purple"){
						purpleID = teamInfoList [i - 2].Trim('"');
					} else if (teamInfoList [i].Trim('"') == "green"){
						greenID = teamInfoList [i - 2].Trim('"');
					}
				}
					print("Red team ID: " + redID);
					print("orange team ID: " + orangeID);
					print("yellow team ID: " + yellowID);
					print("green team ID: " + greenID);
					print("blue team ID: " + blueID);
					print("purple team ID: " + purpleID);
			}
  }

	public void startAssignPlayerRED()
	{
		StartCoroutine("assignPlayerRED");
	}

	public void startAssignPlayerORANGE()
	{
		StartCoroutine("assignPlayerORANGE");
	}

	public void startAssignPlayerYELLOW()
	{
		StartCoroutine("assignPlayerYELLOW");
	}

	public void startAssignPlayerGREEN()
	{
		StartCoroutine("assignPlayerGREEN");
	}

	public void startAssignPlayerBLUE()
	{
		StartCoroutine("assignPlayerBLUE");
	}

	public void startAssignPlayerPURPLE()
	{
		StartCoroutine("assignPlayerPURPLE");
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
			print("Error downloading: ");
		}
		else
		{
			print("user assigned to RED!!");
			PlayerPrefs.SetString("teamID", redID);
			PlayerPrefs.SetString("color", "red");
			PlayerPrefs.Save();
			SceneManager.LoadScene("FirstScene");
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
			print("Error downloading: ");
		}
		else
		{
			PlayerPrefs.SetString("teamID", orangeID);
			PlayerPrefs.SetString("color", "orange");
			PlayerPrefs.Save();
			print("user assigned to ORANGE!!");
			SceneManager.LoadScene("FirstScene");
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
			print("Error downloading: ");
		}
		else
		{
			PlayerPrefs.SetString("teamID", yellowID);
			PlayerPrefs.SetString("color", "yellow");
			PlayerPrefs.Save();
			print("user assigned to YELLOW!!");
			SceneManager.LoadScene("FirstScene");
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
			print("Error downloading: ");
		}
		else
		{
			PlayerPrefs.SetString("teamID", greenID);
			PlayerPrefs.SetString("color", "green");
			PlayerPrefs.Save();
			print("user assigned to GREEN!!");
			SceneManager.LoadScene("FirstScene");
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
			print("Error downloading: ");
		}
		else
		{
			PlayerPrefs.SetString("teamID", blueID);
			PlayerPrefs.SetString("color", "blue");
			PlayerPrefs.Save();
			print("user assigned to BLUE!!");
			SceneManager.LoadScene("FirstScene");
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
			print("Error downloading: ");
		}
		else
		{
			PlayerPrefs.SetString("teamID", purpleID);
			PlayerPrefs.SetString("color", "purple");
			PlayerPrefs.Save();
			print("user assigned to PURPLE!!");
			SceneManager.LoadScene("FirstScene");
		}
	}
}
