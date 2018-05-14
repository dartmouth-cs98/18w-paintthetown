using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clonedHighlight : MonoBehaviour {

	public GameObject originalHighlight;
	private Renderer rend = new Renderer();

	public clonedHighlight(GameObject original){
		originalHighlight = original;
		rend = original.GetComponent<Renderer>();
	}

	public void setOriginal(GameObject original){
		originalHighlight = original;
	}

	public Vector3 getOriginalCenter(){
		rend = originalHighlight.GetComponent<Renderer>();
		return originalHighlight.transform.position;
	}

}
