using UnityEngine;
using System.Collections;

public class NightLight : TODListener {


	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnSunsetStart()
	{
		print ("turning on light");
	}
}
