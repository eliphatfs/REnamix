using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TimeControlSystem : MonoBehaviour {
	private static bool mIsPlaying;
	private static Stopwatch mTimer;
	private static int mTimerStartMillis;

	public static int CurrentTimeMillis {
		get {
			return mTimerStartMillis + (int)mTimer.ElapsedMilliseconds - ChartInfoManager.Offset;
		}
		set {
			if (mTimer.IsRunning)
				mTimer.Stop ();
			mTimer.Reset ();
			mTimerStartMillis = value + ChartInfoManager.Offset;
		}
	}

	public static bool isPlaying {
		get { return mIsPlaying; }
		set {
			mIsPlaying = value;
			CurrentTimeMillis = CurrentTimeMillis + 1;
			if (mIsPlaying) {
				PlaybackSystem.JumpTo (mTimerStartMillis);
				PlaybackSystem.Play ();
				mTimer.Start ();
			}
			else {
				PlaybackSystem.Stop ();
				mTimer.Stop ();
			}
		}
	}

	static TimeControlSystem() {
		mTimer = new Stopwatch ();
		mTimerStartMillis = 0;
		mIsPlaying = false;
	}

	void Update() {
		if (!isPlaying)
			CurrentTimeMillis += (int)(Input.mouseScrollDelta.y * 100f);
	}
}

