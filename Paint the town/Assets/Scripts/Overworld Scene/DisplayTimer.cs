using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class DisplayTimer : MonoBehaviour {
	private string getUserDataURL = "https://paint-the-town.herokuapp.com/api/users";
	public Text timeText;
	public Image timePanel;
	public System.DateTime startTime, endTime;
	public System.TimeSpan elapsed;
	private int startMin, startSec;


	// Use this for initialization
	void Start () {
		timeText.enabled = false;
		timePanel.enabled = false;
	}

	// Use this to start timer
	public void startCountdown(int timeMin, int timeSec){
		startTime = System.DateTime.Now;
		timeText.text = timeMin + ":" + timeSec;
		timePanel.enabled = true;
		timeText.enabled = true;
		startMin = timeMin;
		startSec = timeSec;
	}

	public void setMax(){
		timeText.text = "MAX";
		timePanel.enabled = true;
		timeText.enabled = true;
	}

	IEnumerator sendRequest(){
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW(getUserDataURL, null, headers);

		yield return www;

		if(www.text == "null"){
			//the building has never been clicked before
			print(www.error);

		}else{

			string[] subStrings = Regex.Split(www.text, @"[,:{}]+");
			string timeMin = "";
			string timeSec = "";

			for(int x = 0; x < subStrings.Length; x++){
				if(subStrings[x].Trim('"') == "paintLeft"){
					PlayerPrefs.SetString("Energy", subStrings[x+1]);
				}else if(subStrings[x].Trim('"') == "timeLeftMin"){
					timeMin = subStrings[x+1];
					//PlayerPrefs.SetString("TimeMin", subStrings[x+1]);
				} else if (subStrings[x].Trim('"') == "timeLeftSec"){
					timeSec = subStrings[x+1];
					//PlayerPrefs.SetString("TimeSec", subStrings[x+1]);
				}
			}

			if(timeMin == "" && timeSec == ""){
				setMax();
			} else{

				int MinInt = 0;
				Int32.TryParse(timeMin, out MinInt);

				int SecInt = 0;
				Int32.TryParse(timeSec, out SecInt);
				startCountdown(MinInt, SecInt);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if(timeText.text != "MAX"){
			endTime = System.DateTime.Now;
			elapsed = endTime.Subtract(startTime);
			timeText.text = (startMin - elapsed.Minutes) + ":" + (startSec - elapsed.Seconds);
		}
		if(startMin == 0 && startSec < 2){
			StartCoroutine("sendRequest");
		}
	}
}
