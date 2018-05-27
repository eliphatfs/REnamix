using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class PlaybackSystem : MonoBehaviour {
	public Font font;
    static IWavePlayer waveOutDevice;
	static AudioFileReader audioFileReader;
	static VarispeedSampleProvider resample;
	[HideInInspector]
	public static string FileName;
	private static float mPlaySpeed = 1.0f;
	public static float PlaySpeed {
		get {
			return mPlaySpeed;
		}
		set {
			if (TimeControlSystem.isPlaying)
				TimeControlSystem.isPlaying = false;
			mPlaySpeed = value;
			if (resample != null)
				resample.PlaybackRate = mPlaySpeed;
		}
	}

	public static void DisposeAll() {
		Stop ();
		if (audioFileReader != null) {
			audioFileReader.Dispose ();
			audioFileReader = null;
		}
		if (waveOutDevice != null) {
			waveOutDevice.Dispose ();
			waveOutDevice = null;
		}
		if (resample != null) {
			resample.Dispose ();
			resample = null;
		}
	}

	public static void JumpTo(int millis) {
		if (audioFileReader != null)
			audioFileReader.CurrentTime = System.TimeSpan.FromMilliseconds (millis);
    }

	public static void StopAndDispose() {
		Stop ();
		if (waveOutDevice != null) {
			waveOutDevice.Dispose ();
			waveOutDevice = null;
		}
		if (resample != null) {
			resample.Dispose ();
			resample = null;
		}
	}

	public static void Stop() {
		if (waveOutDevice != null) {
			waveOutDevice.Stop ();
		}
	}

	public static void Load(string filename) {
		FileName = filename;
		if (audioFileReader != null) {
			audioFileReader.Dispose ();
			audioFileReader = null;
		}
		audioFileReader = new AudioFileReader (filename);
	}

	public static void Play() {
        StopAndDispose();
        if (audioFileReader == null)
            return;
		waveOutDevice = new WaveOut();
		if (audioFileReader != null)
			resample = new VarispeedSampleProvider (audioFileReader, 2000, new SoundTouchProfile (true, true));
		resample.PlaybackRate = PlaySpeed;
		
		waveOutDevice.Init(resample);
        waveOutDevice.Play();
    }

	public static int GetMillisTotal() {
		return audioFileReader == null ? 0 : (int) audioFileReader.TotalTime.TotalMilliseconds;
    }

	public void OnDisable() {
		DisposeAll ();
	}

	public void OnGUI() {
		if (Mathf.Abs (PlaySpeed - 1.0f) > 0.01f) {
			GUI.skin.font = font;
			GUI.skin.font.material.color = Color.white;
			GUI.Label (new Rect (400, 50, 160, 40), "Speed: x" + PlaySpeed.ToString ("0.0"));
		}
	}
}
