using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeamAssignmentScript : MonoBehaviour {

	public string url = "https://paint-the-town.herokuapp.com/api/teams";
	public string[] teamInfoList;
	public string redID;
	public string blueID;

  IEnumerator Start()
  {
		Hashtable headers = new Hashtable();
		print("You're retrieving information about teams");
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW(url, null, headers);
		yield return www;

			if(www.text == "null"){
				print(www.error);
			}else{
				print(www.text);
				print("parsing strings");
				string teamInfo = www.text;
				teamInfoList = teamInfo.Split('"');

			for (int i = 0; i <= teamInfoList.Length - 1; i++) {
				print (teamInfoList [i]);
			}

				redID = teamInfoList[19];
				blueID = teamInfoList[5];
				print("Red team ID: " + redID);
				print("Blue team ID: " + blueID);
			}
  }

	public void startAssignPlayerRED()
	{
		StartCoroutine("assignPlayerRED");
	}

	public void startAssignPlayerBLUE()
	{
		StartCoroutine("assignPlayerBLUE");
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
			PlayerPrefs.Save();
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
			PlayerPrefs.Save();
			print("user assigned to BLUE!!");
			SceneManager.LoadScene("FirstScene");
		}
	}
}
