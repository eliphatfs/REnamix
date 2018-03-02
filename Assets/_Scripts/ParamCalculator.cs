using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParamCalculator {
	
	public static float VanillaPositionToUnityX(float v) {
		return v * 1.5f - 3.75f;
	}

	public static float UnityXToVanillaPosition(float u) {
		return u / 1.5f + 2.5f;
	}

	public static float VanillaWidthToScaleX(float v) {
		return v * 1.5f;
	}

	public static float ScaleXToVanillaWidth(float v) {
		return v / 1.5f;
	}

	public static float TimeToPositionY(int time) {
		return -2 + (time - TimeControlSystem.CurrentTimeMillis) * 0.005f;
	}

	public static int PositionYToTime(float pos) {
		return (int)((pos + 2) / 0.005f) + TimeControlSystem.CurrentTimeMillis;
	}

	public static int BarToTime(float bar) {
		return (int)(bar * 240000f / ChartInfoManager.BPM);
	}

	public static float TimeToBar(int time) {
		return time * ChartInfoManager.BPM / 240000f;
	}

	public static int GCD(int a, int b) {
		if (b == 0)
			return a;
		return GCD (b, a % b);
	}
}
