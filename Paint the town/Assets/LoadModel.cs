using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject buildingMesh = Instantiate((GameObject)Resources.Load("dartmouth_hall"));

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
