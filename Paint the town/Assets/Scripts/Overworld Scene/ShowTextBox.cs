using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowTextBox : MonoBehaviour {
 	public Text textArea;
	public Image image;
	int index = 0;
	int characterIndex = 0;
	public float speed = 0.04f;
	public string[] strings;

	// Use this for initialization
	void Start () {
		image.enabled = false;
		textArea.enabled = false;
		//strings[0] = "ERROR: MESSAGE NOT SET";
	}

	// STUFF
	IEnumerator displayTimer(){
		while(true){

			if((Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Space)) && index == (strings.Length)){

				break;
			}

			yield return new WaitForSeconds(speed);

			if(characterIndex >= strings[index].Length){
        textArea.text = strings[index].Substring(0, characterIndex);
				continue;
			}

			textArea.text = strings[index].Substring(0, characterIndex);
			characterIndex++;
		}
	}

	public void show(string[] inputText){
		image.enabled = true;
		textArea.enabled = true;
		index = 0;
		characterIndex = 0;
		strings = inputText;
		StartCoroutine("displayTimer");
	}

	void Update () {
		if(Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Space)){
			if(image.enabled == true && textArea.enabled == true){
				if (index == strings.Length - 1){
					image.enabled = false;
					textArea.enabled = false;
					index = 0;
					characterIndex = 0;
        } else if(characterIndex < strings[index].Length && index < strings.Length){
          characterIndex = strings[index].Length;

				}else if (index < strings.Length){
					index++;
					characterIndex = 0;
				}
			}
		}
	}
}
