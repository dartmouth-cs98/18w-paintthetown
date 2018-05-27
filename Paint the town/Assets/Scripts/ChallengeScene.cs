using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


[Serializable] public class Challenge {
	public string description;
	public bool completed;
	public string reward;
}

public class ChallengeScene : MonoBehaviour {
	public RectTransform parent;
	public GridLayoutGroup grid;
	public GameObject buttonCompletePrefab;
	public GameObject buttonIncompletePrefab;
	public string challenges;
	public int col,row = 1;
	public int challengeNum;

	// Use this for initialization
	void Start () {
		challenges = PlayerPrefs.GetString("Challenges", "no challenges");
		challengeNum = challenges.Length;

		parent = gameObject.GetComponent<RectTransform> ();
		grid = gameObject.GetComponent<GridLayoutGroup> ();

		displayChallenges ();
	}

	void displayChallenges() {
		

		print (challenges);
		challenges = "{\"description\": \"This is the first challenge.\", \"completed\": \"false\", \"reward\": \"10 cool points\"}; {\"description\": \"This is the second challenge.\", \"completed\": \"true\", \"reward\": \"50 cool points\"}";
		//print (challenges);
		string[] challengesStringList = challenges.Split (';');
		challengeNum = challengesStringList.Length;
		challengesStringList [0] = challengesStringList [0].Remove (0, 1);
		challengesStringList [challengeNum - 1] = challengesStringList [challengeNum - 1].Remove (challengesStringList [challengeNum - 2].Length, 1);

		Button[] toDisplay = new Button[challengeNum];
		GameObject tempButton;
		Challenge tempChallenge;
		int i = 0;

		foreach (string challenge in challengesStringList) {
			// cast to serializable class
			tempChallenge = JsonUtility.FromJson<Challenge>(challenge);

			// completed challenge
			if (tempChallenge.completed == true) {
				tempButton = (GameObject)Instantiate(buttonCompletePrefab);

			} else { //incomplete challenge
				tempButton = (GameObject)Instantiate(buttonIncompletePrefab);

			}

			tempButton.transform.SetParent(parent);
			tempButton.transform.GetChild (0).GetComponent<Text> ().text = tempChallenge.description;
			toDisplay[i] = tempButton.GetComponent<Button>();
			i++;
		}

		// still need to do something about adjusting the number of rows as number of challenges increases?

	}
}
