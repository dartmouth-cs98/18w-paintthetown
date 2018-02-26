using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Profile : MonoBehaviour {
	public string userName;

	// Use this for initialization
	void Start () {
		// call to server to get name here
		// hard code for now
		userName = "Your Name Here";
	}
	
	void OnGUI() {
		GUI.Label(new Rect(200, 50, 100, 20), userName);
	}
}
