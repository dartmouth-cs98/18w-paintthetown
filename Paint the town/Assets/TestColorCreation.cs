using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestColorCreation : MonoBehaviour {

    string localURL = "https://paint-the-town.herokuapp.com/api/colors";

    // Use this for initialization
    IEnumerator Start () {
        WWWForm f = new WWWForm();
        f.AddField("name", "Super Green");
        f.AddField("hex", "#00ff00");
        f.AddField("rgb[]", "0");
        f.AddField("rgb[]", "255");
        f.AddField("rgb[]", "0");

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
