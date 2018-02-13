using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class createUser : MonoBehaviour {

	Text myText;

	string localURL= "https://paint-the-town.herokuapp.com/api/signup";
	string userURL = "https://paint-the-town.herokuapp.com/api/users";

	// Use this for initialization

	IEnumerator Start () {
		
		WWWForm f = new WWWForm();

		myText = GetComponent<Text>();

		System.Random rnd = new System.Random();
		string email = rnd.Next(1,10000).ToString();
		email = email + "@gmail.com";
		f.AddField("name", "durr");
		f.AddField("lastName", "Hurr");
		f.AddField("email", email);
		f.AddField("password", "255");

		var test = UnityWebRequest.Post(localURL, f);

		// Wait until the download is done
		yield return test.SendWebRequest();

		if (test.isNetworkError || test.isHttpError)
		{
			print("Error downloading: " + test.error);
		}
		else
		{
			// show the highscores
			print("user posted!");
			string token = test.downloadHandler.text;
			string[] subStrings = token.Split ('"');

			print(subStrings[3]);

			string getToken = "JWT " + subStrings[3];

			Hashtable headers = new Hashtable();
			headers.Add("Authorization", getToken);
			WWW www = new WWW(userURL, null, headers);
			yield return www;

			//Debug.Log(www.text);
			string returnData = www.text;
			string[] subReturnStrings = returnData.Split(',');

			myText.text = subReturnStrings[5];

//			var form = new WWWForm ();
//			var headers = new Hashtable();
//			headers.Add("Authorization", subStrings[3]);
//			WWW www = new WWW(localURL, null, headers);
//			yield return www;
//			Debug.Log(www.text);
			//print (test.downloadHandler.text);
			//Debug.Log(test.downloadHandler.text);
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
