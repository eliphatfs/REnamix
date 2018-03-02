using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChainScaling : MonoBehaviour, INoteScaling {
	public Transform Left, Right;

	#region INoteScaling implementation
	void INoteScaling.NoteChanged (float position, float width, int direction) {
		Vector3 t;
		t = Left.localScale;
		t.x = ParamCalculator.VanillaWidthToScaleX (width) * 50f / 29f - 21f / 29f;
		Left.localScale = t;
		Right.localScale = t;

		t = Left.localPosition;
		t.x = -0.445f - 0.22f * Left.localScale.x;
		Left.localPosition = t;

		t = Right.localPosition;
		t.x = 0.445f + 0.22f * Right.localScale.x;
		Right.localPosition = t;

		t = transform.localScale;
		t.x = t.y = 0.57f * (direction != 0 ? 9f / 16f * 0.8f : 1f);
		transform.localScale = t;
	}
	#endregion
}
