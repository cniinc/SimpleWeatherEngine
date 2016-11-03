using UnityEngine;
using System.Collections;
using System;

public class WeatherManager : MonoBehaviour {
	public static WeatherManager instance;

	[Tooltip ("0 is midnight, 12 is noon, 6.5 is sunrise, or 6:30am")] 
	[SerializeField] private float gameTimeOfDay;
	[Tooltip("in seconds")]
	[SerializeField] private float totalDayLength; 
	private float sunRoundTime = 0;

	[SerializeField] private Color sunriseSunColor;
	[SerializeField] private Color daySunColor;
	[SerializeField] private Color sunsetSunColor;
	[SerializeField] private Color nightSunColor;

	[SerializeField] private Light Sun;
	private float initialSunIntensity;
	[SerializeField] private GameObject NightSky;


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
	public event Action OnNightStart;
	public event Action OnSunriseStart;
	public event Action OnDayStart;
	public event Action OnSunsetStart;


	// Use this for initialization
	void Awake () {
		instance = this;


		initialSunIntensity = Sun.intensity;
	
	}



	// Update is called once per frame
	void Update () {
		
		if (gameTimeOfDay > 24f || gameTimeOfDay < 0f) {
			print ("time out of 24h");
			gameTimeOfDay = 0f;
		}

		gameTimeOfDay = sunRoundTime / totalDayLength * 24;

//		CelestialBodies.transform.rotation = Quaternion.FromToRotation (Vector3.down, new Vector3 (0, CurrentTime / 24 * 360, 0));
		Sun.gameObject.transform.rotation = Quaternion.Euler((sunRoundTime/totalDayLength*360 * -1) - 90, 0, 0);

		if (sunRoundTime >= totalDayLength || sunRoundTime < 0f)
			sunRoundTime = 0f;
		else sunRoundTime += (Time.deltaTime);

		updateDayPhase ();
	
	}

	void updateDayPhase()
	{
		
		//day
		if (gameTimeOfDay > dayStart && gameTimeOfDay < sunsetStart && m_DayPhase != dayPhase.day) {
			m_DayPhase = dayPhase.day;
			print ("day");
			if (OnDayStart != null)
				OnDayStart ();
		}
		//sunset
		if (gameTimeOfDay > sunsetStart && gameTimeOfDay < nightStart && m_DayPhase != dayPhase.sunset) {
			m_DayPhase = dayPhase.sunset; 
			print ("sunset");
			NightSky.SetActive (true);
			if (OnSunsetStart != null)
				OnSunsetStart ();
		}
		//night
		if ((gameTimeOfDay > nightStart && gameTimeOfDay < 24) || (gameTimeOfDay > 0 && gameTimeOfDay < sunriseStart)) {
			if (m_DayPhase != dayPhase.night) {
				m_DayPhase = dayPhase.night;
				Sun.intensity = 0;
				if (!NightSky.activeSelf)
					NightSky.SetActive (true);
				print ("night");
				if (OnNightStart != null)
					OnNightStart ();
			}
		}
		//sunrise
		if (gameTimeOfDay > sunriseStart && gameTimeOfDay < dayStart && m_DayPhase != dayPhase.sunrise) {
			m_DayPhase = dayPhase.sunrise;
			print ("sunrise");
			NightSky.SetActive (false);
			Sun.intensity = initialSunIntensity;
			if (OnSunriseStart != null)
				OnSunriseStart();
		}

	}
}
