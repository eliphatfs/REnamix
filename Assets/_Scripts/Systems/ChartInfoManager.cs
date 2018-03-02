using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartInfoManager : MonoBehaviour {
	public static float BPM;
	public static int Offset;
	public static string Name;
	public static Difficulties Difficulty;
	public enum Difficulties {
		B, N, H, M, G, T
	}
	public void Start () {
		BPM = 120f;
		Offset = 0;
	}
}
