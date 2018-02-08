using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class createUser : MonoBehaviour {

	string localURL= "https://paint-the-town.herokuapp.com/api/signup";

	// Use this for initialization
	IEnumerator Start () {
		WWWForm f = new WWWForm();
		f.AddField("name", "durr");
		f.AddField("lastName", "Hurr");
		f.AddField("email", "email");
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
			Debug.Log(test.downloadHandler.text);
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
