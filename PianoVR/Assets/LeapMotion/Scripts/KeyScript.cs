using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// wav file from http://www.mediacollege.com/audio/tone/download/

public class KeyScript : MonoBehaviour {
	
	public int pitch;// half steps above/below A440
	public AudioSource sound;
	private bool playing = false;
	private HashSet<Collider> colliders;
	private const float HALF_STEP_RATIO = 1.05946309436f;

	// Use this for initialization
	void Start () {
		colliders = new HashSet<Collider>();
		transform.localScale = transform.localScale * 1.8f;
		transform.position = transform.position * 2;
		setColor();
	}
	
	void setBlack() {
		Color black = new Color(0.1f,0.1f,0.1f,1);
		
		MeshRenderer gameObjectRenderer = gameObject.GetComponent<MeshRenderer>();
		 
		Material newMaterial = new Material(Shader.Find("Standard"));
		 
		newMaterial.color = black;
		gameObjectRenderer.material = newMaterial ;
	}
	
	void setWhite() {
		Color white = new Color(0.9f,0.9f,0.9f,1);
		
		MeshRenderer gameObjectRenderer = gameObject.GetComponent<MeshRenderer>();
		 
		Material newMaterial = new Material(Shader.Find("Standard"));
		 
		newMaterial.color = white;
		gameObjectRenderer.material = newMaterial ;
	}
	
	void setRed() {
		Color red = new Color(0.8f,0.1f,0.1f,1);
		
		MeshRenderer gameObjectRenderer = gameObject.GetComponent<MeshRenderer>();
		 
		Material newMaterial = new Material(Shader.Find("Standard"));
		 
		newMaterial.color = red;
		gameObjectRenderer.material = newMaterial ;
	}
	
	bool isBlackKey()
	{
		sound.pitch = Mathf.Pow(HALF_STEP_RATIO, pitch);
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
	
	
	void setColor() {
		if (isBlackKey())
			setBlack();
		else
			setWhite();
	}
	
	// Update is called once per frame
	void Update () {
		//setColor();
	}
	
	void removeColliding(Collider other)
	{
		colliders.Remove(other);
		if (playing && colliders.Count == 0)
		{
			sound.enabled = false;
			sound.loop = false;
			playing = false;
			setColor();
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		// print(other.gameObject.name);
		if (other.gameObject.name == "bone3")
		{
			if ((Mathf.Abs(other.gameObject.transform.position.x - transform.position.x) < 0.025) && (other.gameObject.transform.position.z+0.025 < transform.position.z))
			{
				if (!playing)
				{
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
	
	void OnTriggerExit(Collider other)
	{
		removeColliding(other);
	}
}
