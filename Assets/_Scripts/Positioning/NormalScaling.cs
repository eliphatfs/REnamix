using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NormalScaling : MonoBehaviour, INoteScaling {
	#region INoteScaling implementation
	void INoteScaling.NoteChanged (float position, float width, int direction) {
		Vector3 t;
		t = transform.localScale;
		t.x = ParamCalculator.VanillaWidthToScaleX (width) * 0.5f * (direction != 0 ? 9f / 16f * 0.8f : 1f);
		transform.localScale = t;
	}
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
