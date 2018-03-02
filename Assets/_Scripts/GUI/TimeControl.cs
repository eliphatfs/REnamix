using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeControl : MonoBehaviour {
	public static TimeControl Instance = null;

	public Slider Slider;
	public InputField SeekTo;
	public Text Info;
	// Use this for initialization
	void Start () {
		Instance = this;
		NoteSelection.MouseInterfaceID = Mathf.Abs (GetHashCode ());
	}
	
	// Update is called once per frame
	void Update () {
		DateTime c;
		if (TimeControlSystem.CurrentTimeMillis < 0)
			c = new DateTime (0);
		else
			c = new DateTime (TimeControlSystem.CurrentTimeMillis * 10000L);
		DateTime t = new DateTime (PlaybackSystem.GetMillisTotal () * 10000L);
		Info.text = c.ToString ("m:ss") + "/" + t.ToString ("m:ss");
		if (PlaybackSystem.GetMillisTotal () > 0)
			Slider.value = TimeControlSystem.CurrentTimeMillis / (float)PlaybackSystem.GetMillisTotal ();
	}

	public void OnEndEditSeekTo () {
		if (SeekTo.text != "") {
			TimeControlSystem.CurrentTimeMillis = int.Parse (SeekTo.text);
			if (TimeControlSystem.isPlaying)
				TimeControlSystem.isPlaying = false;
		}
	}

	public void OnSliderValueChanged () {
		if (!TimeControlSystem.isPlaying)
			TimeControlSystem.CurrentTimeMillis = (int)(PlaybackSystem.GetMillisTotal () * Slider.value);
	}
}
