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

	// Use this for initialization
	void Start () {
		timeText.enabled = false;
		timePanel.enabled = false;
	}

	// Use this to start timer
	public void startCountdown(int timeMin, int timeSec){
		TimeSpan buffer = new TimeSpan(0,timeMin,timeSec);
		endTime = System.DateTime.Now.Add(buffer);

		timeText.text = timeMin + ":" + timeSec;
		timePanel.enabled = true;
		timeText.enabled = true;
	}

	// show the user that they are at the max capacity of paint
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
			// separate out server response
			string[] subStrings = Regex.Split(www.text, @"[,:{}]+");
			string timeMin = "";
			string timeSec = "";

			// iterate through server response to grab paintLeft and timeLeftMin/Sec
			for(int x = 0; x < subStrings.Length; x++){
				if(subStrings[x].Trim('"') == "paintLeft"){
					PlayerPrefs.SetString("Energy", subStrings[x+1]);
				}else if(subStrings[x].Trim('"') == "timeLeftMin"){
					timeMin = subStrings[x+1];
				} else if (subStrings[x].Trim('"') == "timeLeftSec"){
					timeSec = subStrings[x+1];
				}
			}

			// user is at max capacity of paint
			if(timeMin == "null" && timeSec == "null"){
				setMax();
			} else{
				// get mins
				int MinInt = 0;
				Int32.TryParse(timeMin, out MinInt);

				// get secs
				int SecInt = 0;
				Int32.TryParse(timeSec, out SecInt);
				startCountdown(MinInt, SecInt);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		// if not at max, calculate time until refill
		if(timeText.text != "MAX"){
			startTime = System.DateTime.Now;
			elapsed = endTime.Subtract(startTime);

			string sec = "";
			if (elapsed.Seconds.ToString().Length == 1){
				sec = "0" + elapsed.Seconds;
			} else {
				sec = elapsed.Seconds.ToString();
			}

			timeText.text = elapsed.Minutes.ToString() + ":" + sec;
		}

		// send request to update timer
		if(elapsed.Minutes <= 0 && elapsed.Seconds == 0){
			StartCoroutine("sendRequest");
		}

		// send request to update timer
		if(PlayerPrefs.GetString("SendTimerUpdate", "false") == "true" && timeText.text == "MAX"){
			StartCoroutine("sendRequest");
			PlayerPrefs.SetString("SendTimerUpdate", "false");
		}
	}
}
