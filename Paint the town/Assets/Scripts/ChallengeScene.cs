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
	public string updateTeamData;
	public int col,row = 1;
	public int challengeNum;

	// Use this for initialization
	void Start () {
		updateTeamData = PlayerPrefs.GetString ("UpdateTeamData", "no team data");

		displayChallenges ();
	}

	void displayChallenges() {

		if (updateTeamData != "no team data") {
			// manipulating server info to extract challenges
			string finder = "\"challenges\":[";
			string finder2 = "\"team\":\"";
			int index = updateTeamData.IndexOf(finder);
			int index2 = updateTeamData.IndexOf(finder2);
			int toCut = index2 - index;
			challenges = updateTeamData.Substring(index, toCut);
			challenges = challenges.Remove (0, finder.Length);
			challenges = challenges.Remove (challenges.Length - 2, 2);

			// separate into each challenge
			string[] challengesStringList = challenges.Split ('}');
			// get num of challenges to display
			challengeNum = challengesStringList.Length;

			GameObject tempButton;
			string tempString;
			Challenge tempChallenge;
			int i = 0;
			foreach (string challenge in challengesStringList) {
				if (challenge == "") {
					break;
				}

				// fix syntax for JSON parsing
				tempString = challenge;
				if (!tempString [tempString.Length - 1].Equals ('}')) {
					tempString = tempString + "}";
				}
				if (tempString [0].Equals (',')) {
					tempString = tempString.Remove (0, 1);
				}

				// cast to serializable class
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
		}
	}
}
