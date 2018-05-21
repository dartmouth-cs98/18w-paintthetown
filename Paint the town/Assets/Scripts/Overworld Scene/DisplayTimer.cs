using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTimer : MonoBehaviour {
	public Text timeText;
	public Image timePanel;
	public System.DateTime startTime, endTime;
	public System.TimeSpan elapsed;
	private int startMin, startSec;


	// Use this for initialization
	void Start () {
		timeText.enabled = false;
		timePanel.enabled = false;
	}

	// Use this to start timer
	public void startCountdown(int timeMin, int timeSec){
		startTime = System.DateTime.Now;
		timeText.text = timeMin + ":" + timeSec;
		timePanel.enabled = true;
		timeText.enabled = true;
		startMin = timeMin;
		startSec = timeSec;
	}

	// Update is called once per frame
	void Update () {
		endTime = System.DateTime.Now;
		elapsed = endTime.Subtract(startTime);
		timeText.text = (startMin - elapsed.Minutes) + ":" + (startSec - elapsed.Seconds);
	}
}
