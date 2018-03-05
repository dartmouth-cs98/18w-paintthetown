using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeamChoosingScript : MonoBehaviour {

	public string token = "JWT " + PlayerPrefs.GetString("token", "no token");

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void chooseRedTeam()
	{
		SceneManager.LoadScene("FirstScene");
	}

	public void chooseBlueTeam()
	{
		SceneManager.LoadScene("FirstScene");
	}
}
