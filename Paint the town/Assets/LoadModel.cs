using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string bID = PlayerPrefs.GetString("bid");
        // GameObject buildingMesh = Instantiate((GameObject)Resources.Load(bID));
        GameObject buildingMesh = Instantiate((GameObject)Resources.Load("dartmouth_hall"));

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
