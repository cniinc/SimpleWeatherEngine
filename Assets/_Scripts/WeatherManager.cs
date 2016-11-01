using UnityEngine;
using System.Collections;

public class WeatherManager : MonoBehaviour {
	[Tooltip ("0 is midnight, 12 is noon, 6.5 is 6:30am")] 
	[SerializeField] private float CurrentTime;

	private enum cloudiness {none, clear, mild, clouded};
	private cloudiness m_Cloudiness;
	private enum dayPhase {none, night, sunrise, day, sunset}
	private dayPhase m_DayPhase;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
