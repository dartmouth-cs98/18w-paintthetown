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



		grid.cellSize = new Vector2 (parent.rect.width / col, parent.rect.height / row);



	}
	

}
