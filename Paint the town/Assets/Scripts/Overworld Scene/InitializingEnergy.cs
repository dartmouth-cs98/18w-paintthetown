using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InitializingEnergy : MonoBehaviour {

	private string getUserDataURL = "https://paint-the-town.herokuapp.com/api/users";
	private DisplayTimer myDT;

	IEnumerator Start () {
		// send request to server for user data
		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW(getUserDataURL, null, headers);
		myDT = gameObject.GetComponent<DisplayTimer>();
		yield return www;
		// got an error
		if(www.text == "null"){
			print(www.error);
		}else{
			// parse out to get paintLeft and timeLeft
			string[] subStrings = Regex.Split(www.text, @"[,:{}]+");
			string MinTime = "";
			string SecTime = "";

			for(int x = 0; x < subStrings.Length; x++){
				if(subStrings[x].Trim('"') == "paintLeft"){
					PlayerPrefs.SetString("Energy", subStrings[x+1]);
				}else if(subStrings[x].Trim('"') == "timeLeftMin"){
					MinTime = subStrings[x+1];
				}else if(subStrings[x].Trim('"') == "timeLeftSec"){
					SecTime = subStrings[x+1];
				}
			}

			// we are at the max amount of paint/energy
			if(MinTime == "" && SecTime == ""){
				myDT.setMax();
			} else{
				// not the max; set the mins and secs
				int MinInt = 0;
				Int32.TryParse(MinTime, out MinInt);

				int SecInt = 0;
				Int32.TryParse(SecTime, out SecInt);
				myDT.startCountdown(MinInt, SecInt);
			}
		}
	}
}
