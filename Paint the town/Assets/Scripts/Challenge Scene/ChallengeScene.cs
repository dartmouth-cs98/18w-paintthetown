using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


[Serializable] public class Challenge {
	public string description;
	public bool completed;
}


public class ChallengeScene : MonoBehaviour {
	public RectTransform parent;
	public GridLayoutGroup grid;
	public GameObject buttonCompletePrefab;
	public GameObject buttonIncompletePrefab;
	public string challenges;
	public string challengeChunk;
	public string userUrl = "https://paint-the-town.herokuapp.com/api/users";


	// Use this for initialization
	void Start () {
		challengeChunk = PlayerPrefs.GetString ("ChallengeChunk", "no challenge chunk");
		displayChallenges ();
	}

	void displayChallenges() {

		if (challengeChunk != "no challenge chunk") {
			// manipulating server info to extract challenges
			string finder = "\"challenges\":[";
			string finder2 = "\"team\":\"";
			int index = challengeChunk.IndexOf (finder);
			int index2 = challengeChunk.IndexOf (finder2);
			int toCut = index2 - index;
			challenges = challengeChunk.Substring (index, toCut);
			challenges = challenges.Remove (0, finder.Length);
			challenges = challenges.Remove (challenges.Length - 2, 2);

			// separate into each challenge
			string[] challengesStringList = challenges.Split ('}');

			GameObject tempButton;
			string tempString;
			Challenge tempChallenge;
			foreach (string challenge in challengesStringList) {
				if (challenge == "") {
					break;
				}

				// fix syntax for JSON parsing
				tempString = challenge;
				if (!tempString [tempString.Length - 1].Equals ('}')) {
					tempString = tempString + '}';
				}
				if (tempString [0].Equals (',')) {
					tempString = tempString.Remove (0, 1);
				}

				// cast to serializable class
				if(tempString[0] == ']'){
					break;
				}
				tempChallenge = JsonUtility.FromJson<Challenge> (tempString);
				// completed challenge
				if (tempChallenge.completed == true) {
					// make temp button in complete prefab
					tempButton = (GameObject)Instantiate (buttonCompletePrefab);

				} else { //incomplete challenge
					// make temp button in incomplete prefab
					tempButton = (GameObject)Instantiate (buttonIncompletePrefab);

				}

				// set parent to parent panel; make the button a layout element; set challenge description as button text
				tempButton.transform.SetParent (parent);
				tempButton.AddComponent<LayoutElement> ();
				tempButton.SetActive (true);
				tempButton.transform.GetChild (0).GetComponent<Text> ().text = tempChallenge.description;

			}

		} else {
			StartCoroutine ("getUserData");
		}
	}


	IEnumerator getUserData() {
		// sending request for user data
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW(userUrl, null, headers);
		yield return www;
		if (www.text == "null") {
			print (www.error);
		} else {
			// set chunk for later parsing to get challenges
			challengeChunk = www.text;
			PlayerPrefs.SetString ("ChallengeChunk", www.text);
			PlayerPrefs.Save ();
			displayChallenges ();
		}
	}
}
