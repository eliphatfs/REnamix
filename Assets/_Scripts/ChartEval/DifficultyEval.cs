using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyEval : MonoBehaviour {
	Finger[] fingers;
	float moveTime = 0.3f;
	float relatedConstraint = 623.5f; // 540 / (sqrt(3) / 2)
	// Use this for initialization
	void Start () {
		fingers = new Finger[4];
		// Right Index
		fingers[0] = new Finger(1f / 6f);
		fingers [0].Position = new Vector2 (620, 30);
		// Right Middle
		fingers[1] = new Finger(1f / 5f);
		fingers [1].Position = new Vector2 (900, 30);
		// Left Index
		fingers[2] = new Finger(1f / 5.5f);
		fingers [2].Position = new Vector2 (340, 30);
		// Left Middle
		fingers[3] = new Finger(1f / 4.5f);
		fingers [3].Position = new Vector2 (60, 30);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetRelatedFingerID (int thiz) {
		return thiz ^ 1;
	}

	public struct Finger {
		public Vector2 Position;
		public float PressTime;
		public uint Tiredness = 0;
		public float TiredCoefficient {
			get { return (Mathf.Log (Tiredness + 50) - Mathf.Log (50)) / 10f; }
		}
		public Finger(float pressTime) {
			PressTime = pressTime;
		}
	}
}
