using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class InitializingEnergy : MonoBehaviour {

	private string getUserDataURL = "https://paint-the-town.herokuapp.com/api/users";

	IEnumerator Start () {

		Hashtable headers = new Hashtable();
		headers.Add("Authorization", "JWT " + PlayerPrefs.GetString("token", "no token"));
		WWW www = new WWW(getUserDataURL, null, headers);

		yield return www;

		if(www.text == "null"){
			//the building has never been clicked before
			print(www.error);

		}else{
			string[] subStrings = Regex.Split(www.text, @"[,:{}]+");
			for(int x = 0; x < subStrings.Length; x++){
				if(subStrings[x].Trim('"') == "paintLeft"){
					PlayerPrefs.SetString("Energy", subStrings[x+1]);
				}else if(subStrings[x].Trim('"') == "timeLeft"){

				}

			}
		}
	}
}
