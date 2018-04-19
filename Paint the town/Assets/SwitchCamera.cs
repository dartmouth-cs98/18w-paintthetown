using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchCamera : MonoBehaviour {

    public Camera povCam;
    public Camera setCam;
    public Button switchBtn;
         
	// Use this for initialization
	void Start () {
        Button btn = switchBtn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

        setCam.enabled = true;
        povCam.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void TaskOnClick()
    {
        povCam.enabled = !povCam.enabled;
        setCam.enabled = !setCam.enabled;
        print("Cameras switched!");
    }
}
