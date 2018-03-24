using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HoldScaling : MonoBehaviour, INoteScaling {
	public Transform Begin, End;
	#region INoteScaling implementation
	void INoteScaling.NoteChanged (float position, float width, int direction) {
		Vector3 t;
		t = transform.localScale;
		t.x = ParamCalculator.VanillaWidthToScaleX (width) * 0.5f * (direction != 0 ? 9f / 16f : 1f);
		transform.localScale = t;
	}
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Begin == null || !Begin.gameObject.activeInHierarchy) {
			if (End != null && End.gameObject.activeInHierarchy)
				DestroyObject (End.gameObject);
			Destroy (gameObject);
			return;
		}
		if (End == null || !End.gameObject.activeInHierarchy) {
			if (Begin != null && Begin.gameObject.activeInHierarchy)
				DestroyObject (Begin.gameObject);
			Destroy (gameObject);
			return;
		}
		Vector3 bgp = Begin.position;
		GetComponent<SpriteRenderer> ().enabled = !End.GetComponent<NoteJudge> ().Judged;
		Vector3 sc = transform.localScale;
		if (Begin.GetComponent<NoteData> ().Direction == 0) {
			if (Begin.GetComponent<NoteJudge> ().Judged)
				bgp.y = -2;
			sc.x = Begin.localScale.x * 4f;
			sc.y = (End.position.y - bgp.y) * 2f;
		} else {
			if (Begin.GetComponent<NoteJudge> ().Judged)
				bgp.x = Begin.GetComponent<NoteData> ().Direction * 4.25f;
			sc.x = Begin.localScale.x * 4f;
			sc.y = Mathf.Abs (End.position.x - bgp.x) * 2f;
		}
		End.GetComponent<NoteData> ().Time = Mathf.Max (Begin.GetComponent<NoteData> ().Time + 60, End.GetComponent<NoteData> ().Time);
		End.GetComponent<NoteData> ().Position = Begin.GetComponent<NoteData> ().Position;
		End.GetComponent<NoteData> ().Width = Begin.GetComponent<NoteData> ().Width;
		End.GetComponent<NoteData> ().NotifyWidth = true;
		End.GetComponent<NoteData> ().Direction = Begin.GetComponent<NoteData> ().Direction;
		Begin.GetComponent<NoteData> ().Sub = End.GetComponent<NoteData> ();
		transform.position = (bgp + End.position) / 2;
		transform.localRotation = Begin.localRotation;

		Color c = new Color (1f, 1f, 1f, 1f);
		if (Begin.GetComponent<NoteData> ().Direction == 0 && Begin.position.y > 1f)
			c.a = 2f - transform.position.y;
		else if (Begin.GetComponent<NoteData> ().Direction != 0 && Begin.GetComponent<NoteData> ().Direction * (Begin.position.x) < 2f)
			c.a = Begin.GetComponent<NoteData> ().Direction * (transform.position.x) - 1f;

		c.a = Mathf.Clamp01 (c.a);
		GetComponent<SpriteRenderer> ().color = c;
		GetComponent<SpriteRenderer> ().size = new Vector2 (sc.x, sc.y);
	}
}
