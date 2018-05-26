using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class NoteData : MonoBehaviour {
	[SerializeField]
	private float mPosition, mWidth;
	[SerializeField]
	private int mTime, mDirection; // Direction: -1 Left 0 Middle 1 Right

	public static List<NoteData> Instances = new List<NoteData> ();
	public bool NotifyWidth;
	public string NoteType;
	public NoteData Sub;
	public int InnerID {
		get;
		internal set;
	}

	void Start() {
		Instances.Add (this);
	}

	void OnDisable() {
		Instances.Remove (this);
	}

	void Update() {
		if (NotifyWidth) {
			foreach (INoteScaling ns in gameObject.GetComponents<INoteScaling>()) {
				ns.NoteChanged (Position, Width, Direction);
			}
			NotifyWidth = false;
		}
	}

	public float Position {
		get { return mPosition; }
		set { mPosition = value; }
	}

	public float Width {
		get { return mWidth; }
		set {
			mWidth = value;
			NotifyWidth = true;
		}
	}

	public int Time {
		get { return mTime; }
		set { mTime = value; }
	}

	public int Direction {
		get { return mDirection; }
		set { mDirection = value; }
	}
}
