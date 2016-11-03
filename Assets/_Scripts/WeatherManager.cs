using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class WeatherManager : MonoBehaviour {
	public static WeatherManager instance;

	[Tooltip ("0 is midnight, 12 is noon, 6.5 is 6:30am")] 
	[SerializeField] private float gameTimeOfDay;
	[Tooltip("in seconds")]
	[SerializeField] private float totalDayLength; 
	private float sunRoundTime = 0;
	[Tooltip("In in-game time")]
	[SerializeField] private float transitionHours = 2; //the 'game-time' hours
	private float m_transitionTime; //the actual calculated time based on totalDayLength

	[SerializeField] private Color sunriseSunColor;
	[SerializeField] private Color daySunColor;
	[SerializeField] private Color sunsetSunColor;


	[SerializeField] private Light Sun;
	private float initialSunIntensity;
	[SerializeField] private GameObject NightSky;
	[SerializeField] private Material NightSkyMaterial;

	private enum dayPhase {none, night, sunrise, day, sunset};
	private dayPhase m_DayPhase;

	//set values from the sun cycle
	private float nightStart = 21f;
	private float sunriseStart = 4.5f;
	private float dayStart = 7f;
	private float sunsetStart = 17f;

	//events one can tie to for scripting purposes
	public event Action OnNightStart;
	public event Action OnSunriseStart;
	public event Action OnDayStart;
	public event Action OnSunsetStart;

	[SerializeField] private GameObject[] clouds;
	[SerializeField] private float cloudsOnceEveryXDays;
	[SerializeField] private float cloudsTravelForXHours;

	// Use this for initialization
	void Awake () {
		instance = this;


		initialSunIntensity = Sun.intensity;

		m_transitionTime = totalDayLength / 24 * transitionHours;
	}

	void Start()
	{
		initializeDayPhase ();
		StartCoroutine (setCloudStorm ());
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

	void initializeDayPhase() //should do a common parent function for both Initialize and Update day phase, but due to time just made two separate ones. In future, would refactor.
	{
		
		//day
		if (gameTimeOfDay > dayStart && gameTimeOfDay < sunsetStart) {
			m_DayPhase = dayPhase.day;
//			print ("day");

			Sun.color = daySunColor;
			if (OnDayStart != null)
				OnDayStart ();
		}

		//sunset
		if (gameTimeOfDay > sunsetStart && gameTimeOfDay < nightStart ) {
			m_DayPhase = dayPhase.sunset; 
//			print ("sunset");

			Sun.color = sunsetSunColor;
			NightSkyMaterial.color = new Color (1, 1, 1, 1);

			if (OnSunsetStart != null)
				OnSunsetStart ();
		}
		//night
		if ((gameTimeOfDay > nightStart && gameTimeOfDay < 24) || (gameTimeOfDay >= 0 && gameTimeOfDay < sunriseStart)) {
			
				m_DayPhase = dayPhase.night;

				Sun.intensity = 0;
//				print ("night1");
				Color c = new Color (1, 1, 1, 1);
				NightSkyMaterial.color = c;

				if (OnNightStart != null)
					OnNightStart ();
			
		}
		//sunrise
		if (gameTimeOfDay > sunriseStart && gameTimeOfDay < dayStart) {
			m_DayPhase = dayPhase.sunrise;
//			print ("sunrise");

			NightSkyMaterial.color = new Color (1, 1, 1, 0);
			//			NightSky.SetActive(false);
			Sun.intensity = initialSunIntensity;
			Sun.color = sunriseSunColor;

			if (OnSunriseStart != null)
				OnSunriseStart();
		}
	}

	void updateDayPhase()
	{
		
		//day
		if (gameTimeOfDay > dayStart && gameTimeOfDay < sunsetStart && m_DayPhase != dayPhase.day) {
			m_DayPhase = dayPhase.day;
//			print ("day");

			StartCoroutine (changeSunColorTo (daySunColor));

			if (OnDayStart != null)
				OnDayStart ();
		}
		//sunset
		if (gameTimeOfDay > sunsetStart && gameTimeOfDay < nightStart && m_DayPhase != dayPhase.sunset) {
			m_DayPhase = dayPhase.sunset; 
//			print ("sunset");
			StartCoroutine (fadeInNightSky ((nightStart - sunsetStart)/3));
			StartCoroutine (changeSunColorTo (sunsetSunColor));
			Sun.DOIntensity (0, m_transitionTime * 2);
			if (OnSunsetStart != null)
				OnSunsetStart ();
		}
		//night
		if ((gameTimeOfDay > nightStart && gameTimeOfDay < 24) || (gameTimeOfDay > 0 && gameTimeOfDay < sunriseStart)) {
			if (m_DayPhase != dayPhase.night) {
				m_DayPhase = dayPhase.night;

//				NightSky.SetActive (true);
//				print ("night");
			
				if (OnNightStart != null)
					OnNightStart ();
			}
		}
		//sunrise
		if (gameTimeOfDay > sunriseStart && gameTimeOfDay < dayStart && m_DayPhase != dayPhase.sunrise) {
			m_DayPhase = dayPhase.sunrise;
//			print ("sunrise");
			NightSkyMaterial.DOColor (new Color (1, 1, 1, 0), m_transitionTime);
//			NightSky.SetActive(false);
			Sun.DOIntensity (initialSunIntensity, m_transitionTime);
			StartCoroutine (changeSunColorTo (sunriseSunColor));

			if (OnSunriseStart != null)
				OnSunriseStart();
		}

	}

	//shouldn't need, but just in case
	private IEnumerator fadeInNightSky(float delayTime)
	{
		yield return new WaitForSeconds (delayTime);
//		print ("starting");
		NightSkyMaterial.DOColor (new Color (1, 1, 1, 1), m_transitionTime* 4);

	}



	private IEnumerator changeSunColorTo (Color newColor)
	{
		//do within 30 minutes of game 'time'
		float transitionLength = (m_transitionTime);
//		print ("changing color");
		Sun.DOColor (newColor, transitionLength);

		yield return null;
	}

	private IEnumerator setCloudStorm()
	{
//		print ("starting clouds");
		Vector3 initialPos = clouds [0].transform.position;
		float waitTime = UnityEngine.Random.Range(0f, cloudsOnceEveryXDays) * totalDayLength;
		yield return new WaitForSeconds (waitTime);
//		print ("moving clouds");
		for(int i = 0; i < clouds.Length; i++){
			Vector3 travelFinal = new Vector3 (initialPos.x, initialPos.y, initialPos.z+ 800);
			clouds [i].transform.DOMove (travelFinal, totalDayLength*(cloudsTravelForXHours)/24 *(i+1)/clouds.Length );
				}
		yield return new WaitForSeconds(totalDayLength * cloudsTravelForXHours/24);
//		print ("resetting clouds");
		for(int i = 0; i < clouds.Length; i++){
			clouds [i].transform.position = initialPos;
		}
		StartCoroutine (setCloudStorm ());
	}
}
