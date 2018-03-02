using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NotePositioning : MonoBehaviour {
	[HideInInspector]
	public NoteData NoteData;
	// Use this for initialization
	void Start () {
		NoteData = GetComponent<NoteData> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 t = transform.position;
		t.x = ParamCalculator.VanillaPositionToUnityX (NoteData.Position + NoteData.Width / 2);
		t.y = ParamCalculator.TimeToPositionY (NoteData.Time);

		if (NoteData.Direction == -1) {
			Vector3 pos = t;
			pos.x = t.y + 2f - 4.25f;
			pos.y = t.x * 9f / 16f * 0.8f;
			transform.localRotation = Quaternion.Euler (0f, 0f, -90f);
			transform.position = pos;
		} else if (NoteData.Direction == 1) {
			Vector3 pos = t;
			pos.x = -t.y - 2f + 4.255f;
			pos.y = t.x * 9f / 16f * 0.8f;
			transform.localRotation = Quaternion.Euler (0f, 0f, 90f);
			transform.position = pos;
		} else {
			transform.position = t;
			transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
		}
	}
}
