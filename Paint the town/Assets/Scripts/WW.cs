// Get the latest webcam shot from outside "Friday's" in Times Square
using UnityEngine;
using System.Collections;

public class WW : MonoBehaviour
{
	public string url = "http://images.earthcam.com/ec_metros/ourcams/fridays.jpg";
	IEnumerator Start()
	{
		using (WWW www = new WWW(url))
		{
			yield return www;
			Renderer renderer = GetComponent<Renderer>();
			renderer.material.mainTexture = www.texture;
		}
	}
}