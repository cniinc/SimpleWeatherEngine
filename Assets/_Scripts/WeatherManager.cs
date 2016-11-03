using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

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


	[SerializeField] private Light Sun;
	private float initialSunIntensity;
	[SerializeField] private GameObject NightSky;


	private enum cloudiness {none, clear, mild, clouded};
	private cloudiness m_Cloudiness;
	private enum dayPhase {none, night, sunrise, day, sunset};
	private dayPhase m_DayPhase;

	//set values from the sun cycle
	private float nightStart = 21f;
	private float sunriseStart = 4.5f;
	private float dayStart = 7f;
	[SerializeField] private float sunsetStart;

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
			StartCoroutine (changeSunColorTo (daySunColor));

			if (OnDayStart != null)
				OnDayStart ();
		}
		//sunset
		if (gameTimeOfDay > sunsetStart && gameTimeOfDay < nightStart && m_DayPhase != dayPhase.sunset) {
			m_DayPhase = dayPhase.sunset; 
			print ("sunset");

			StartCoroutine (changeSunColorTo (sunsetSunColor));

			if (OnSunsetStart != null)
				OnSunsetStart ();
		}
		//night
		if ((gameTimeOfDay > nightStart && gameTimeOfDay < 24) || (gameTimeOfDay > 0 && gameTimeOfDay < sunriseStart)) {
			if (m_DayPhase != dayPhase.night) {
				m_DayPhase = dayPhase.night;
				Sun.DOIntensity (0, totalDayLength / 12);
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
			Sun.DOIntensity (initialSunIntensity, totalDayLength / 12);
			StartCoroutine (changeSunColorTo (sunriseSunColor));

			if (OnSunriseStart != null)
				OnSunriseStart();
		}

	}



	private IEnumerator changeSunColorTo (Color newColor)
	{
		//do within 30 minutes of game 'time'
		float transitionLength = (totalDayLength/12);
		print ("changing color");
		Sun.DOColor (newColor, transitionLength);

		yield return null;
	}
}
