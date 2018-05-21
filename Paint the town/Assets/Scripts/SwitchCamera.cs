using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wrld;
using Wrld.Resources.Buildings;
using Wrld.Space;

public class SwitchCamera : MonoBehaviour {

    public Camera uiCam;
    public Camera povCam;
    public Camera setCam;
    public Button switchBtn;
    private DisplayTimer myDT;

	// Use this for initialization
	void Start () {
        Button btn = switchBtn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        myDT = gameObject.GetComponent<DisplayTimer>();
        //Api.Instance.CameraApi.SetControlledCamera(null);
        uiCam.enabled = true;
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

        if(povCam.enabled == true){
          Api.Instance.CameraApi.SetControlledCamera(null);
          myDT.startCountdown(5,59);
        }else{
          Api.Instance.CameraApi.SetControlledCamera(setCam);
        }

        print("Cameras switched!");
    }
}
