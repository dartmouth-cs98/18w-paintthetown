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

		// separate into each challenge
		string[] challengesStringList = challenges.Split (';');
		// get num of challenges to display
		challengeNum = challengesStringList.Length;
		// remove quotes from first and last index?
		challengesStringList [0] = challengesStringList [0].Remove (0, 1);
		challengesStringList [challengeNum - 1] = challengesStringList [challengeNum - 1].Remove (challengesStringList [challengeNum - 2].Length, 1);

		// list of buttons to display -- may not actually need this, will see
		Button[] toDisplay = new Button[challengeNum];
		GameObject tempButton;
		Challenge tempChallenge;
		int i = 0;

		foreach (string challenge in challengesStringList) {
			// cast to serializable class
			tempChallenge = JsonUtility.FromJson<Challenge> (challenge);

			// completed challenge
			if (tempChallenge.completed == true) {
				// make temp button in complete prefab
				tempButton = (GameObject)Instantiate (buttonCompletePrefab);

			} else { //incomplete challenge
				// make temp button in incomplete prefab
				tempButton = (GameObject)Instantiate (buttonIncompletePrefab);

			}

			// set parent to parent panel; set challenge description as button text; add button to button list
			tempButton.transform.SetParent(parent);
			tempButton.transform.GetChild (0).GetComponent<Text> ().text = tempChallenge.description;
			toDisplay[i] 		= tempButton.GetComponent<Button>();
			i++;
		}

		// still need to do something about adjusting the number of rows as number of challenges increases?

	}
}
