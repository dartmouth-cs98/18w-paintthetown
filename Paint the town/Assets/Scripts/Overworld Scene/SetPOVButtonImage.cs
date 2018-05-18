using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SetPOVButtonImage : MonoBehaviour {

	public Image red;
	public Image orange;
	public Image yellow;
	public Image green;
	public Image blue;
	public Image purple;

	// Use this for initialization
	void Start () {

		red.enabled = false;
		orange.enabled = false;
		yellow.enabled = false;
		green.enabled = false;
		blue.enabled = false;
		purple.enabled = false;

		Transform topChild = gameObject.transform.Find("sButton");

		string color = PlayerPrefs.GetString("color", "no color");

		if(color != "no color"){
			if(color == "red"){
				topChild.GetComponent<Image>().sprite =  red.sprite;
			} else if(color == "orange"){
				topChild.GetComponent<Image>().sprite =  orange.sprite;
			} else if(color == "yellow"){
				topChild.GetComponent<Image>().sprite =  yellow.sprite;
			} else if(color == "green"){
				topChild.GetComponent<Image>().sprite =  green.sprite;
			} else if(color == "blue"){
				topChild.GetComponent<Image>().sprite =  blue.sprite;
			} else if(color == "purple"){
				topChild.GetComponent<Image>().sprite =  purple.sprite;
			} else {
				print("Honestly I don't know what's going on");
			}
		} else{
			print("ERROR RETRIEVING PLAYER COLOR: COLOR NOT SET");
		}
	}
}
