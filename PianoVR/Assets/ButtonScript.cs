using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public TutorScript tutor;
	private float lastTime;
	private readonly float Debounce = 1.0f;

	void Start () {
		lastTime = 0.0f;
	}
	
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		if (Time.time - lastTime < Debounce)
			return;
		lastTime = Time.time;
		print("collision!");
		if (!tutor.isTutoring)
		{
			tutor.startTutor();
		}
		else if (!tutor.isPlaying)
		{
			tutor.startPlaying();
		}
		else
		{
			tutor.stopPlaying();
		}
	}
}
