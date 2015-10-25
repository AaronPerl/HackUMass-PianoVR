using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// wav file from http://www.mediacollege.com/audio/tone/download/

public class KeyScript : MonoBehaviour {
	
	public int pitch;// half steps above/below A440
	public AudioSource sound;
	private bool playing = false;
	private HashSet<Collider> colliders;
	private const float HALF_STEP_RATIO = 1.05946309436f;
	public TutorScript tutor;
	private bool isCurrentNote = false;

	// Use this for initialization
	public void Start () {
		colliders = new HashSet<Collider>();
		transform.localScale = transform.localScale * 0.9f;
		transform.localScale = new Vector3(transform.localScale.x * 1.3f, transform.localScale.y, transform.localScale.z);
		transform.position = transform.position * 1.0f;
		transform.position = new Vector3(transform.position.x * 1.3f, transform.position.y, transform.position.z);
		setColor();
	}
	
	public void setNote() {
		isCurrentNote = true;
		setBlue();
	}
	
	private int getRealPitch() {
		return pitch + 12*Convert.ToInt32(transform.parent.gameObject.name,10);
	}
	
	private void setColor(float r, float g, float b)
	{
		gameObject.GetComponent<Renderer>().material.color = new Color(r,g,b);
	}
	
	private void setBlack() {
		setColor(0.1f, 0.1f, 0.1f);
	}
	
	private void setWhite() {
		setColor(0.9f, 0.9f, 0.9f);
	}
	
	private void setRed() {
		setColor(0.8f, 0.1f, 0.1f);
	}
	
	private void setBlue() {
		setColor(0.1f, 0.1f, 0.8f);
	}
	
	private bool isBlackKey()
	{
		sound.pitch = Mathf.Pow(HALF_STEP_RATIO, getRealPitch());
		int pitchMod = pitch % 12;
		if (pitchMod < 0)
			pitchMod = pitchMod + 12;
		switch (pitchMod)
		{
		case 1: // A#
		case 4: // C#
		case 6: // D#
		case 9: // F#
		case 11: // G#
			return true;
		default:
			return false;
		}
	}
	
	
	private void setColor() {
		if (isBlackKey())
			setBlack();
		else
			setWhite();
	}
	
	// Update is called once per frame
	public void Update () {
		//setColor();
	}
	
	private void removeColliding(Collider other)
	{
		colliders.Remove(other);
		if (playing && colliders.Count == 0)
		{
			sound.enabled = false;
			sound.loop = false;
			playing = false;
			if (!isCurrentNote)
				setColor();
			else
				setBlue();
		}
	}
	
	private void OnTriggerStay(Collider other)
	{
		// print(other.gameObject.name);
		if (other.gameObject.name == "bone3")
		{
			if ((Mathf.Abs(other.gameObject.transform.position.x - transform.position.x) < 0.0125) &&
			    (isBlackKey()||(other.gameObject.transform.position.z+0.02 < transform.position.z)))
			{
				if (!playing)
				{
					if (isCurrentNote)
					{
						isCurrentNote = false;
						tutor.curNotePressed();
					}
					sound.enabled = true;
					sound.loop = true;
					playing = true;
					setRed();
				}
			}
			else
				removeColliding(other);
			colliders.Add(other);
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		removeColliding(other);
	}
}
