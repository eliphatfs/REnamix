using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public static GridManager Instance;

	public int BeatPerRowS = 1;
	public int BeatPerRowM = 2;
	public int GridDir = 0;

	public int NumRows = 1;
	public float BeatPerRow {
		get {
			int gcd = ParamCalculator.GCD (BeatPerRowS, BeatPerRowM);
			BeatPerRowM /= gcd;
			BeatPerRowS /= gcd;
			return BeatPerRowS / (float)BeatPerRowM;
		}
	}


	public static List<TimeTransformPair> GridLines = new List<TimeTransformPair> ();
	public static List<TimeTransformPair> GridCols = new List<TimeTransformPair> ();
	public static readonly Color[] DirColors = {
		new Color (255f / 255f, 184f / 255f, 184f / 255f),
		new Color (184f / 255f, 246f / 255f, 224f / 255f),
		new Color (184f / 255f, 246f / 255f, 255f / 255f)
	};

	public class TimeTransformPair {
		public int Time;
		public Transform Transform;
		public TimeTransformPair(int time, Transform tr) {
			Time = time;
			Transform = tr;
		}
	}

	void Update () {
		Instance = this;
		UpdateHorizons ();
		UpdateVerticals ();
	}

	void UpdateHorizons () {
		if (Mathf.Abs (GridDir) > 1) {
			foreach (var line in GridLines) {
				line.Time = int.MinValue;
				line.Transform.position = new Vector3 (-100, -100, -1);
			}
			return;
		}
		double STEP = (ParamCalculator.BarToTime (1000f) - ParamCalculator.BarToTime (0f)) * BeatPerRow / 4000f;
		double MAXTIME = (int)((TimeControlSystem.CurrentTimeMillis + 1000) / STEP) * STEP;
		double MINTIME = (int)((TimeControlSystem.CurrentTimeMillis - 700) / STEP) * STEP;
		IEnumerator<TimeTransformPair> horis;

		int c = 0;

		for (double i = MINTIME; i < MAXTIME; i += STEP) {
			c++;
		}
		for (int i = GridLines.Count; i < c; i++) {
			Transform n = Instantiate (Resources.Load<GameObject> ("UI/GridLineHorizon"), transform).transform;
			GridLines.Add (new TimeTransformPair (0, n));
		}

		horis = GridLines.GetEnumerator ();
		for (double i = MINTIME; i < MAXTIME; i += STEP) {
			if (!horis.MoveNext ())
				continue;
			horis.Current.Time = (int)i;
			horis.Current.Transform.position = new Vector3 (0, ParamCalculator.TimeToPositionY ((int)i), -1);
			horis.Current.Transform.rotation = Quaternion.Euler (0, 0, 0);
			horis.Current.Transform.GetComponent<LineRenderer> ().startColor = DirColors [GridDir + 1];
			horis.Current.Transform.GetComponent<LineRenderer> ().endColor = DirColors [GridDir + 1];
			if (GridDir != 0) {
				horis.Current.Transform.rotation = Quaternion.Euler (0, 0, 90);
				Vector3 v = horis.Current.Transform.position;
				Vector3 pos = horis.Current.Transform.position;
				pos.x = -v.y - 2f + 4.25f;
				pos.x *= GridDir;
				pos.y = 0;
				horis.Current.Transform.position = pos;
			}
		}
		while (horis.MoveNext ()) {
			horis.Current.Time = int.MinValue;
			horis.Current.Transform.position = new Vector3 (-100, -100, -1);
		}
	}

	void UpdateVerticals () {
		if (NumRows <= 0) {
			foreach (var line in GridCols) {
				line.Time = int.MinValue;
				line.Transform.position = new Vector3 (-100, -100, -1);
			}
			return;
		}
		float STEP = 9.6f / NumRows;
		for (int i = GridCols.Count; i < NumRows + 1; i++) {
			Transform n = Instantiate (Resources.Load<GameObject> ("UI/GridLineVertical"), transform).transform;
			GridCols.Add (new TimeTransformPair (0, n));
		}
		IEnumerator<TimeTransformPair> vertis = GridCols.GetEnumerator ();
		for (float i = -4.8f; i <= 4.8f; i += STEP) {
			if (!vertis.MoveNext ())
				continue;
			vertis.Current.Transform.position = new Vector3 (i, 0, -1);
			vertis.Current.Transform.rotation = Quaternion.Euler (0, 0, 0);
			vertis.Current.Time = (int)(i * 1000f);
			if (GridDir != 0) {
				vertis.Current.Transform.rotation = Quaternion.Euler (0, 0, 90);
				Vector3 v = vertis.Current.Transform.position;
				Vector3 pos = vertis.Current.Transform.position;
				pos.x = 0;
				pos.y = v.x * 9f / 16f;
				vertis.Current.Transform.position = pos;
			}
		}
		while (vertis.MoveNext ()) {
			vertis.Current.Time = int.MinValue;
			vertis.Current.Transform.position = new Vector3 (-100, -100, -1);
		}
	}
}
