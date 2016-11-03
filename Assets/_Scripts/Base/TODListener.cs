using UnityEngine;
using System.Collections;

public class TODListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
		WeatherManager.instance.OnDayStart += OnDayStart;
		WeatherManager.instance.OnSunsetStart += OnSunsetStart;
		WeatherManager.instance.OnNightStart += OnNightStart;
		WeatherManager.instance.OnSunriseStart += OnSunriseStart;

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void OnDayStart()
	{
		
	}

	public virtual void OnSunsetStart()
	{
		
	}

	public virtual void OnNightStart()
	{
		
	}

	public virtual void OnSunriseStart()
	{
		
	}
	
}
