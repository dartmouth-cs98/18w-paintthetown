using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_Timer : MonoBehaviour {
    private DisplayTimer myDT;


    // Use this for initialization
    void Start () {
        myDT = gameObject.GetComponent<DisplayTimer>();
        myDT.startCountdown(5, 59);
    }
	
	// Update is called once per frame
	void Update () {
        myDT.timeText.enabled = true;
    }
}
