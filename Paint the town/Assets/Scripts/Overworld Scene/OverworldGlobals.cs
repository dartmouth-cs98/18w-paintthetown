using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OverworldGlobals{
	public static bool isTextboxDisplayed = false;
	public const string ERROR_CAMERA_TOO_HIGH = "Error: You are flying too close to the sun!";
	public const string ERROR_BUILDING_TOO_FAR = "Error: Building too far away to paint!";
	public const string ERROR_CAMERA_TOO_FAR = "Error: Camera too far away from your location!";

	public const string OVERWORLDTUTORIAL_1 = "Excellent! Now that you’ve selected a team, I can teach you a little about Paint the Town.";
	public const string OVERWORLDTUTORIAL_2 = "Right now, you are looking down at the world around you.";
	public const string OVERWORLDTUTORIAL_3 = "Here you can scroll around and see what teams have colored buildings nearby";
	public const string OVERWORLDTUTORIAL_4 = "If your eager to get painting then click the spray can in the top right corner to go to first person view";
	public const string OVERWORLDTUTORIAL_5 = "Keep in mind your paint is displayed in the bottom right corner";
	public const string OVERWORLDTUTORIAL_6 = "When you paint a building, you use up some of your paint. The bigger the building, the more paint you’ll have to use.";
	public const string OVERWORLDTUTORIAL_7 = "Don’t worry though, you’ll get back some of your paint every couple of minutes!";
	public const string OVERWORLDTUTORIAL_8 = "One other thing, to center the overhead camera, click the compass in the top right corner.";
	public const string OVERWORLDTUTORIAL_9 = "That’s all for now! Have fun painting the world around you!";

	public const int MAX_CAMERA_HEIGHT = 1200;
	public const int MAX_CAMERA_DISTANCE = 300;
}
