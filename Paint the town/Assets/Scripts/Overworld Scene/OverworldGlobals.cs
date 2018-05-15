using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OverworldGlobals{
	public static bool isTextboxDisplayed = false;
	public const string ERROR_CAMERA_TOO_HIGH = "Error: You are flying too close to the sun!";
	public const string ERROR_BUILDING_TOO_FAR = "Error: Building too far away to paint!";
	public const string ERROR_CAMERA_TOO_FAR = "Error: Camera too far away from your location!";

	public const int MAX_CAMERA_HEIGHT = 1200;
	public const int MAX_CAMERA_DISTANCE = 300;
}
