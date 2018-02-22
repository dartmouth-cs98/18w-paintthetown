using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlideMenu : MonoBehaviour {

	public Button MenuButton;
	public Button ReturnToPlayButton;
	public Button ShopSceneButton;
	public Button SettingsSceneButton;

	public Animation SlidePanelAnim;
	public bool menuIn = false;

	void Start()
	{
		MenuButton.onClick.AddListener(slideMenu);
		ReturnToPlayButton.onClick.AddListener(slideMenu);
		ShopSceneButton.onClick.AddListener(goToShop);
		SettingsSceneButton.onClick.AddListener(goToSettings);
	
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

	void goToShop(){
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		//SceneManager.LoadScene("ShopScene");

	}

	void goToSettings() {
		SlidePanelAnim.Play ("SlidePanelOut");
		menuIn = false;
		//SceneManager.LoadScene("SettingsScene");

	}

}
