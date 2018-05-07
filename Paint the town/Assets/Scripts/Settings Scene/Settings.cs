using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour {

	public Toggle NotificationsToggle;
	public Toggle SoundsToggle;
	public Button DisconnectFacebookButton;

	// Use this for initialization
	void Start () {
		NotificationsToggle.onValueChanged.AddListener (handleNotifications);
		SoundsToggle.onValueChanged.AddListener (handleSounds);
		DisconnectFacebookButton.onClick.AddListener(disconnectFacebook);
	}

	void handleNotifications(bool turnedOn) {
		if (turnedOn) {
			print ("we will turn on notifications");
		} else {
			print ("we will turn off notifications");
		}
	}

	void handleSounds(bool turnedOn) {
		if (turnedOn) {
			print ("we will turn on sounds");
		} else {
			print ("we will turn off sounds");
		}
	}
		

	void disconnectFacebook() {
		print ("we will disconnect facebook");
		// need to disconnect from facebook and also prompt for password to create normal account
	}
		
}
