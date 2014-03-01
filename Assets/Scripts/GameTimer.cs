using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public class GameTimer : MonoBehaviour {
	public TimeSpan MinusTime = new TimeSpan(0,0,1);
	public TimeSpan ElapsedTimeRemain = new TimeSpan(2,0,0);
	// Constructor is based off of hour, minute and second. Performing subtract in 
	// just minute category moves to fast, but hour category gives proper result of 
	// 2 minutes.Tested 10 times yielding 2minutes and 5seconds off of phone stopwatch
	// to 2 minutes and 8 seconds. Human error can be apart as stopwatch starts moment
	// play button is pressed.
		void Start ()
	{
		
	}
	
	// Update is called once per frame
	public void OnGUI()
	{
		GUI.Label(new Rect(50,100,100,30), "" + ElapsedTimeRemain);
		
	}
	void Update () 
	{
		TimeCheck ();
		
	}
	public void TimeCheck()
	{
		float BeenOneSecond = 0;
		while (BeenOneSecond < 1) {
						BeenOneSecond += Time.deltaTime;
				}
		if(ElapsedTimeRemain != TimeSpan.Zero)
		{
			ElapsedTimeRemain = ElapsedTimeRemain.Subtract (MinusTime);
			}
	
	}
}