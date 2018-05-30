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
    public Image overhead;
    public Image pov;
    public Image mainPic;
    public Button centerCamButton;

	// Use this for initialization
	void Start () {
        Button btn = switchBtn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

		uiCam.enabled = true;
        setCam.enabled = true;
        povCam.enabled = false;

	}

  void TaskOnClick()
  {
      povCam.enabled = !povCam.enabled;
      setCam.enabled = !setCam.enabled;

      if(povCam.enabled == true){
        centerCamButton.enabled = false;
        centerCamButton.image.enabled = false;
        Api.Instance.CameraApi.SetControlledCamera(null);

      }else{
        centerCamButton.enabled = true;
        centerCamButton.image.enabled = true;
        Api.Instance.CameraApi.SetControlledCamera(setCam);
      }

      if(povCam.enabled){
      	mainPic.sprite = pov.sprite;
      }else{
        mainPic.sprite = overhead.sprite;
      }

      print("Cameras switched!");
  }
}
