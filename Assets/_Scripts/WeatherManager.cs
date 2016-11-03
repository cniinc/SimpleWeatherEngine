using UnityEngine;
using System.Collections;
using System;

public class WeatherManager : MonoBehaviour {
	[Tooltip ("0 is midnight, 12 is noon, 6.5 is sunrise, or 6:30am")] 
	[SerializeField] private float gameTimeOfDay;
	[Tooltip("in seconds")]
	[SerializeField] private float totalDayLength; 
	private float sunRoundTime = 0;

	[SerializeField] private GameObject CelestialBodies;

	private enum cloudiness {none, clear, mild, clouded};
	private cloudiness m_Cloudiness;
	private enum dayPhase {none, night, sunrise, day, sunset};
	private dayPhase m_DayPhase;

	//set values from the sun cycle
	private float nightStart = 18f;
	private float sunriseStart = 4.5f;
	private float dayStart = 7f;
	private float sunsetStart = 16f;

	//events one can tie to for scripting purposes
	public event Action<WeatherManager> OnNightStart;
	public event Action<WeatherManager> OnSunriseStart;
	public event Action<WeatherManager> OnDayStart;
	public event Action<WeatherManager> OnSunsetStart;


	// Use this for initialization
	void Start () {
	
	}



	// Update is called once per frame
	void Update () {
		
		if (gameTimeOfDay > 24f || gameTimeOfDay < 0f) {
			print ("time out of 24h");
			gameTimeOfDay = 0f;
		}

		gameTimeOfDay = sunRoundTime / totalDayLength * 24;

//		CelestialBodies.transform.rotation = Quaternion.FromToRotation (Vector3.down, new Vector3 (0, CurrentTime / 24 * 360, 0));
		CelestialBodies.transform.rotation = Quaternion.Euler((sunRoundTime/totalDayLength*360 * -1) - 90, 0, 0);

		if (sunRoundTime >= totalDayLength || sunRoundTime < 0f)
			sunRoundTime = 0f;
		else sunRoundTime += (Time.deltaTime);

//		print (gameTimeOfDay);
		updateDayPhase ();
	
	}

	void updateDayPhase()
	{
		
		//it is day
		if (gameTimeOfDay > dayStart && gameTimeOfDay < sunsetStart && m_DayPhase != dayPhase.day) {
			m_DayPhase = dayPhase.day;
			print ("day");
			if (OnDayStart != null)
				OnDayStart (this);
		}
		//it is sunset
		if (gameTimeOfDay > sunsetStart && gameTimeOfDay < nightStart && m_DayPhase != dayPhase.sunset) {
			m_DayPhase = dayPhase.sunset; 
			print ("sunset");
			if (OnSunsetStart != null)
				OnSunsetStart (this);
		}
		//it is night
		if ((gameTimeOfDay > nightStart && gameTimeOfDay < 24) || (gameTimeOfDay > 0 && gameTimeOfDay < sunriseStart)) {
			if (m_DayPhase != dayPhase.night) {
				m_DayPhase = dayPhase.night;
				print ("night");
				if (OnNightStart != null)
					OnNightStart (this);
			}
		}
		//it is sunrise
		if (gameTimeOfDay > sunriseStart && gameTimeOfDay < dayStart && m_DayPhase != dayPhase.sunrise) {
			m_DayPhase = dayPhase.sunrise;
			print ("sunrise");
			if (OnSunriseStart != null)
				OnSunriseStart(this);
		}

	}
}
