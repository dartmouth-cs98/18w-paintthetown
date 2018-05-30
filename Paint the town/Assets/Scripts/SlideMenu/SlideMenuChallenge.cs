using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlideMenuChallenge : MonoBehaviour {

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
		ChallengesSceneButton.onClick.AddListener(goToChallenges);
		//ShopSceneButton.onClick.AddListener(goToShop);
		//SettingsSceneButton.onClick.AddListener(goToSettings);
		//ProfileSceneButton.onClick.AddListener(goToProfile);
	}

	void slideMenu()
	{
		if (menuIn == false) {
			SlidePanelAnim.Play ("SlidePanelInChallenge");
			menuIn = true;
		} else {
			SlidePanelAnim.Play ("SlidePanelOutChallenge");
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
		
	void goToPlay() {
		SlidePanelAnim.Play ("SlidePanelOutChallenge");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "FirstScene") {
			SceneManager.LoadScene ("FirstScene");
		}
	}

	void goToChallenges() {
		SlidePanelAnim.Play ("SlidePanelOutChallenge");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "ChallengesScene") {
			SceneManager.LoadScene ("ChallengesScene");
		}
	}

	// DEPRECATED functions below: may implement at a later date

	/*
	void goToProfile() {
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		// if we aren't already in that scene, load it
		if (SceneManager.GetActiveScene ().name != "ProfileScene") {
			SceneManager.LoadScene ("ProfileScene");
		}
	}


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

}
