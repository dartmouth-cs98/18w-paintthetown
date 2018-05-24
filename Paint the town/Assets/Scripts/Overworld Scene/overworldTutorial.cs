using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overworldTutorial : MonoBehaviour {

	private ShowTextBox myTB;

	void Start () {
		if(PlayerPrefs.GetString("Tutorial", "false") == "true"){

			PlayerPrefs.SetString("Tutorial", "false");

			myTB = GetComponent<ShowTextBox>();
			string[] array = new string[9];
			array[0] = OverworldGlobals.OVERWORLDTUTORIAL_1;
			array[1] = OverworldGlobals.OVERWORLDTUTORIAL_2;
			array[2] = OverworldGlobals.OVERWORLDTUTORIAL_3;
			array[3] = OverworldGlobals.OVERWORLDTUTORIAL_4;
			array[4] = OverworldGlobals.OVERWORLDTUTORIAL_5;
			array[5] = OverworldGlobals.OVERWORLDTUTORIAL_6;
			array[6] = OverworldGlobals.OVERWORLDTUTORIAL_7;
			array[7] = OverworldGlobals.OVERWORLDTUTORIAL_8;
			array[8] = OverworldGlobals.OVERWORLDTUTORIAL_9;
			myTB.show(array);
		}
	}
}
