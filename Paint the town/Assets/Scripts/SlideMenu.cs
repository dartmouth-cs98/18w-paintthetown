using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlideMenu : MonoBehaviour {

	public Button MenuButton;
	public Button LogoutButton;
	public Button ReturnToPlayButton;
	public Button ChallengesSceneButton;
	public Button MuteButton;
	//public Button ShopSceneButton;
	//public Button SettingsSceneButton;
	//public Button ProfileSceneButton;

	public Animation SlidePanelAnim;
	public bool menuIn = false;

	void Start()
	{
		LogoutButton.onClick.AddListener (logout);
		MenuButton.onClick.AddListener(slideMenu);
		ReturnToPlayButton.onClick.AddListener(goToPlay);
		MuteButton.onClick.AddListener(mute);
		//ShopSceneButton.onClick.AddListener(goToShop);
		//SettingsSceneButton.onClick.AddListener(goToSettings);
		ChallengesSceneButton.onClick.AddListener(goToChallenges);
		//ProfileSceneButton.onClick.AddListener(goToProfile);
	}
		
	void slideMenu()
	{
		if (menuIn == false) {
			SlidePanelAnim.Play ("SlidePanelIn");
			menuIn = true;
		} else {
			SlidePanelAnim.Play ("SlidePanelOut");
			menuIn = false;
		}

	}

	void logout() {
		PlayerPrefs.DeleteAll ();
		SceneManager.LoadScene ("LoginScene");
	}

	void mute() {
		// do this
	}

	/*
	void goToShop(){
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "ShopScene") {
			SceneManager.LoadScene ("ShopScene");
		}

	}


	void goToSettings() {
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "SettingsScene") {
			SceneManager.LoadScene ("SettingsScene");
		}

	}
	*/

	void goToPlay() {
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "FirstScene") {
			SceneManager.LoadScene ("FirstScene");
		}
	}

	void goToChallenges() {
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "ChallengesScene") {
			SceneManager.LoadScene ("ChallengesScene");
		}
	}

	/*
	void goToProfile() {
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "ProfileScene") {
			SceneManager.LoadScene ("ProfileScene");
		}
	}
	*/

}
