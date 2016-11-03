using UnityEngine;
using System.Collections;

public class NightLight : TODListener {
	[SerializeField] private Light lightObject;

	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnSunsetStart()
	{
		
		lightObject.intensity = 5;
	}

	public override void OnSunriseStart()
	{
		
		lightObject.intensity = 0;
	}
}
