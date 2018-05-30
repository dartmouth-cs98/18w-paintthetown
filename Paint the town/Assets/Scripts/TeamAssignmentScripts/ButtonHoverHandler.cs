using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverHandler : MonoBehaviour {

	public Text theText;

	void Start () {
		theText.enabled = false; 
	}

	public void onHover(){
		theText.enabled = true;
	}

	public void onLeave(){
		theText.enabled = false;
	}

}
