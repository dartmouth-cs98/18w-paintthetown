using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Wrld;
using Wrld.Space;
using Wrld.Resources.Buildings;

using UnityEngine;
using UnityEngine.Networking;
public class HandleGyro : MonoBehaviour {

	public Camera povCam;
	public Camera setCam;
	public ParticleSystem pLauncherPOV;

	private const float lowPassFilterFactor = 0.2f;
	private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);
	private readonly Quaternion landscapeRight = Quaternion.Euler(0, 0, 90);
	private readonly Quaternion landscapeLeft = Quaternion.Euler(0, 0, -90);
	private readonly Quaternion upsideDown = Quaternion.Euler(0, 0, 180);
	private Quaternion cameraBase = Quaternion.identity;
	private Quaternion calibration = Quaternion.identity;
	private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix = Quaternion.identity;
	private Quaternion referenceRotation = Quaternion.identity;
	// Use this for initialization
	void Start () {
			Input.gyro.enabled = true;
			ResetBaseOrientation();
			UpdateCalibration(true);
			UpdateCameraBaseRotation(true);
			RecalculateReferenceRotation();
	}

	private void UpdateCalibration(bool onlyHorizontal)
	{
			if (onlyHorizontal)
			{
					var fw = (Input.gyro.attitude) * (-Vector3.forward);
					fw.z = 0;
					if (fw == Vector3.zero)
					{
							calibration = Quaternion.identity;
					}
					else
					{
							calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
					}
			}
			else
			{
					calibration = Input.gyro.attitude;
			}
	}

	private void UpdateCameraBaseRotation(bool onlyHorizontal)
	{
			if (onlyHorizontal)
			{
					var fw = transform.forward;
					fw.y = 0;
					if (fw == Vector3.zero)
					{
							cameraBase = Quaternion.identity;
					}
					else
					{
							cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
					}
			}
			else
			{
					cameraBase = transform.rotation;
			}
	}

	private static Quaternion ConvertRotation(Quaternion q)
	{
			return new Quaternion(q.x, q.y, -q.z, -q.w);
	}

	private Quaternion GetRotFix()
	{
			if (Screen.orientation == ScreenOrientation.Portrait)
				return Quaternion.identity;

			if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
				return landscapeLeft;

			if (Screen.orientation == ScreenOrientation.LandscapeRight)
				return landscapeRight;

			if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
				return upsideDown;
			return Quaternion.identity;
	}

	private void ResetBaseOrientation()
	{
			baseOrientationRotationFix = GetRotFix();
			baseOrientation = baseOrientationRotationFix * baseIdentity;
	}

	private void RecalculateReferenceRotation()
	{
			referenceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
	}


	// Update is called once per frame
	void Update () {
		// Create temp Vector 3, dynamically change its height
		RaycastHit hit;
		Vector3 tempPOVposition = new Vector3(setCam.transform.position.x, 400, setCam.transform.position.z + 40);
		if (Physics.Raycast(tempPOVposition,Vector3.down,out hit, 600))
		{
				tempPOVposition.y = hit.point.y + 15f;
		}

		// Set POV cam to the temp Vector 3 created above
		povCam.transform.position = tempPOVposition;
		povCam.transform.rotation = Quaternion.Slerp(povCam.transform.rotation,cameraBase * (ConvertRotation(referenceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor);
		pLauncherPOV.transform.SetPositionAndRotation(povCam.transform.position, povCam.transform.rotation);
	}
}
