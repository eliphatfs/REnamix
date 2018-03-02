using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Wave;

public class PlaybackSystem : MonoBehaviour {
    static IWavePlayer waveOutDevice;
	static AudioFileReader audioFileReader;
	[HideInInspector]
	public static string FileName;

	public static void DisposeAll() {
        Stop();
        if (audioFileReader != null)
        {
            audioFileReader.Dispose();
            audioFileReader = null;
        }
        if (waveOutDevice != null)
        {
            waveOutDevice.Dispose();
            waveOutDevice = null;
        }
    }

	public static void JumpTo(int millis) {
		if (audioFileReader != null)
			audioFileReader.CurrentTime = System.TimeSpan.FromMilliseconds (millis);
    }

	public static void StopAndDispose() {
        Stop();
        if (waveOutDevice != null)
        {
            waveOutDevice.Dispose();
            waveOutDevice = null;
        }
    }

	public static void Stop() {
        if (waveOutDevice != null)
        {
            waveOutDevice.Stop();
        }
    }

	public static void Load(string filename) {
        FileName = filename;
        if (audioFileReader != null)
        {
            audioFileReader.Dispose();
            audioFileReader = null;
        }
        audioFileReader = new AudioFileReader(filename);
    }

	public static void Play() {
        StopAndDispose();
        if (audioFileReader == null)
            return;
        waveOutDevice = new WaveOut();
        waveOutDevice.Init(audioFileReader);
        waveOutDevice.Play();
    }

	public static int GetMillisTotal() {
		return audioFileReader == null ? 0 : (int) audioFileReader.TotalTime.TotalMilliseconds;
    }

	public void OnDisable() {
		DisposeAll ();
	}
}
