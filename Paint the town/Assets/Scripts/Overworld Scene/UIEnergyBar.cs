using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIEnergyBar : MonoBehaviour {

	//private Transform energy;

	public Slider Energy;

	private float MAX_ENERGY = 25000.0f;

	private Color sliderColor;

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

		string color = PlayerPrefs.GetString("color", "no color");

		if(color != "no color"){
			if(color == "red"){
				red.enabled = true;
				sliderColor = Color.red;
			} else if(color == "orange"){
				orange.enabled = true;
				sliderColor = new Color(1.0F, 165.0F/255.0F, 0.0F);
			} else if(color == "yellow"){
				yellow.enabled = true;
				sliderColor = Color.yellow;
			} else if(color == "green"){
				green.enabled = true;
				sliderColor = Color.green;
			} else if(color == "blue"){
				blue.enabled = true;
				sliderColor = Color.blue;
			} else if(color == "purple"){
				purple.enabled = true;
				sliderColor = new Color(160.0F/255F, 32.0F/255F, 240.0F/255F);
			} else {
				print("Honestly I don't know what's going on");
			}
		} else{
			print("ERROR RETRIEVING PLAYER COLOR: COLOR NOT SET");
		}

		Transform topChild = gameObject.transform.Find("Energy");

		Transform FillArea = topChild.Find("Fill Area");

		Transform Fill = FillArea.Find("Fill");

		Fill.gameObject.GetComponent<Image>().color = sliderColor;

		Transform HandleSlideArea = topChild.Find("Handle Slide Area");

		Transform Handle = HandleSlideArea.Find("Handle");

		Handle.gameObject.GetComponent<Image>().color = sliderColor;

	}

	// Update is called once per frame
	void Update () {

		string energyString = PlayerPrefs.GetString("Energy", "No Energy");
		if(energyString != "No Energy"){
			int energyInt = 0;

			Int32.TryParse(energyString, out energyInt);
			Energy.value = (float)energyInt/MAX_ENERGY;

		} else{

			Energy.value = 1;
		}
	}
}
