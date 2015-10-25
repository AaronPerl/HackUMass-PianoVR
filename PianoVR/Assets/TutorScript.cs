using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Ratio {
	public int numerator { get; private set; }
	public int denominator { get; private set; }
	public Ratio(int numerator, int denominator) {
		this.numerator = numerator;
		this.denominator = denominator;
	}
	public float value() {
		return (float)numerator / denominator;
	}
}

public class Note {
	private static readonly ReadOnlyCollection<string> Names = new ReadOnlyCollection<string>(
	new string[] {
		"A",
		"A#",
		"B",
		"C",
		"C#",
		"D",
		"D#",
		"E",
		"F",
		"F#",
		"G",
		"G#"
	});
	private static readonly Dictionary<string, int> Pitches = new Dictionary<string, int>
	{
		{ "A", 0 },
		{ "A#", 1 },
		{ "B", 2 },
		{ "C", 3 },
		{ "C#", 4 },
		{ "D", 5 },
		{ "D#", 6 },
		{ "E", 7 },
		{ "E#", 8 },
		{ "F", 8 },
		{ "F#", 9 },
		{ "G", 10 },
		{ "G#", 11 }
	};
	public int pitch { get; private set; }
	public Ratio duration { get; private set; }
	public Note(int pitch, Ratio duration) {
		this.pitch = pitch;
		this.duration = duration;
	}
	public string getName()
	{
		int pitchMod = pitch % 12;
		if (pitchMod < 0) pitchMod += 12;
			return Names[pitchMod];
	}
	
	public static int getPitch(string noteName)
	{
		return Pitches[noteName];
	}
	
}

public class TutorScript : MonoBehaviour {
	
	private List<Note> notes;
	private int i;
	private int j;
	private float lastTime;
	private int bpm;
	public GameObject piano;
	public string tutorFile;
	public AudioSource sound;
	private const float HALF_STEP_RATIO = 1.05946309436f;
	public bool isPlaying { get; private set; }
	public bool isTutoring { get; private set; }

	public void Start () {
		loadSong(tutorFile);
		bpm = 50;
		isPlaying = false;
		isTutoring = false;
	}
	
	public void Update () {
		if (isPlaying)
		{
			float deltaTime = notes[j].duration.value() * 4.0f / bpm * 60;
			if (Time.time - lastTime > deltaTime)
			{
				j++;
				if (j < notes.Count)
				{
					print(notes[j].getName());
					sound.enabled = false;
					sound.pitch = Mathf.Pow(HALF_STEP_RATIO, notes[j].pitch);
					sound.enabled = true;
					lastTime = Time.time;
				}
				else
				{
					isPlaying = false;
				}
			}
		}
	}
	
	public void startPlaying() {
		j = 0;
		isPlaying = true;
		lastTime = Time.time;
		sound.enabled = false;
		sound.pitch = Mathf.Pow(HALF_STEP_RATIO, notes[j].pitch);
		sound.enabled = true;
	}
	
	// note: steps above/below C
	private void triggerNote(Note note) {
		int octave = (int) Mathf.Floor((note.pitch-3) / 12.0f);
		int pitchMod = note.pitch % 12;
		if (pitchMod < 0) pitchMod += 12;
		Transform octaveObj = piano.transform.Find(octave.ToString());
		string noteName = note.getName();
		print(noteName);
		Transform key = octaveObj.Find(noteName);
		key.gameObject.GetComponent<KeyScript>().setNote();
	}
	
	private void loadSong(string filename) {
		StreamReader reader = new StreamReader(filename, Encoding.Default);
		string line = reader.ReadLine();
		notes = new List<Note>();
		using (reader) {
			while (line != null) {
				string[] noteString = line.Split(',');
				foreach (string curNote in noteString) {
					string[] noteProperties = curNote.Trim().Split(':');
					if (noteProperties.Length != 4) {
						Debug.LogError("Invalid note in tutor file!");
					}
					string noteName = noteProperties[0];
					int octave = Convert.ToInt32(noteProperties[1], 10);
					int numer = Convert.ToInt32(noteProperties[2],10);
					int denom = Convert.ToInt32(noteProperties[3],10);
					int pitch = Note.getPitch(noteName) + 12 * octave;
					notes.Add(new Note(pitch, new Ratio(numer, denom)));
				}
				line = reader.ReadLine();
			}
		}
	}
	
	public void startTutor() {
		isTutoring = true;
		triggerNote(notes[0]);
		i = 0;
	}
	
	public void stopPlaying() {
		isPlaying = false;
	}
	
	public void curNotePressed()
	{
		i++;
		if (i < notes.Count)
			triggerNote(notes[i]);
		else
			isTutoring = false;
	}
}
