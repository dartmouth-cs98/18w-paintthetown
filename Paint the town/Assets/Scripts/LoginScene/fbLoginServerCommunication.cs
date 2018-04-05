using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class fbLoginServerCommunication : MonoBehaviour {

	// Use this for initialization
	public string url = "https://paint-the-town.herokuapp.com/api/auth/facebook";

	public IEnumerator thisThingSendsAGetRequest()
	{
		print("hello!");
			using (WWW www = new WWW(url))
			{
					yield return www;
					if(www.error == null){
						//print(www);
						print(www.text);
					}else{
						print("you have a problem");
					}
			}
	}

	public void doTheFacebookThing()
	{
		print("hihihihihi");
		StartCoroutine("thisThingSendsAGetRequest");
	}

}
