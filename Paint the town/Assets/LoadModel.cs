using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModel : MonoBehaviour {

    // Use this for initialization
    void Start () {
        string bID = PlayerPrefs.GetString("bid");
        // this will load in the desired building on scene strt, but it depends on the bID being one of the imported files
        GameObject buildingMesh = Instantiate((GameObject)Resources.Load(bID));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
