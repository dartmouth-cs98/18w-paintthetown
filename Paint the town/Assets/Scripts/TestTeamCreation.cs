using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestTeamCreation : MonoBehaviour {

    string localURL = "http://localhost:9090/api/teams";

    // Use this for initialization
    IEnumerator Start () {
        WWWForm f = new WWWForm();
        f.AddField("name", "Super Green");
        f.AddField("color", "red");

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
