using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSelection : MonoBehaviour {
	public static List<NoteSelection> Instances = new List<NoteSelection> ();
	public static int MouseInterfaceID = -1;
	public bool IsSelected = false;
	public Rect Hitbox;

	RectLines mIndicator;
	NoteData mData;

	public bool ShouldAttachGridX = false;
	public bool ShouldAttachGridY = false;

	int lastClick = 0;

	void Start() {
		Instances.Add (this);
		mData = GetComponent<NoteData> ();
	}

	void OnDisable() {
		Instances.Remove (this);

		if (MouseInterfaceID == Mathf.Abs (GetHashCode ()))
			MouseInterfaceID = -1;
	}

	bool JudgeContain() {
		return Hitbox.Contains (Camera.main.ScreenToWorldPoint (Input.mousePosition));
	}

	void Update () {
		StartCoroutine (do_Update ());
	}

	UndoSystem.NoteDataBefore dataBefore;
	IEnumerator do_Update () {
		if (ShouldAttachGridX) {
			ShouldAttachGridX = false;
			AttachToGridX ();
		}
		if (ShouldAttachGridY) {
			ShouldAttachGridY = false;
			AttachToGridY ();
		}
		if (mData.Direction == 0)
			Hitbox = new Rect (transform.position.x - transform.localScale.x, transform.position.y - transform.localScale.y * 0.32f, transform.localScale.x * 2, transform.localScale.y * 2 * 0.32f);
		else
			Hitbox = new Rect (transform.position.x - transform.localScale.y * 0.32f, transform.position.y - transform.localScale.x, transform.localScale.y * 2 * 0.32f, transform.localScale.x * 2);

		if ((Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt))) {
			if (JudgeContain ()) {
				if (Input.GetMouseButton (0))
					AttachToGridX ();
				if (Input.GetMouseButton (1))
					AttachToGridY ();
			}
			yield break;
		}
		lastClick++;

		if (MouseInterfaceID == -1 && Input.GetMouseButton (0) && GridManager.Instance.GridDir == mData.Direction && JudgeContain () && !TimeControlSystem.isPlaying) {
			if (lastClick < 20) {
				Instantiate (Resources.Load<GameObject> ("UI/NoteDataEdit"), HotkeySystem.Instance.Canvas.transform).GetComponent<NoteDataEdit> ().Editing = mData;
				MouseInterfaceID = -2;
			} else {
				MouseInterfaceID = Mathf.Abs (GetHashCode ());
				IsSelected = true;
				dataBefore = new UndoSystem.NoteDataBefore (mData);

				mIndicator = Instantiate (Resources.Load<GameObject> ("RectLines")).GetComponent<RectLines> ();
				mIndicator.Rectangle = Hitbox;
				mIndicator.LineWidth = 0.05f;
			}
			lastClick = 0;
		}
		if (MouseInterfaceID == Mathf.Abs (GetHashCode ())) {
			mIndicator.Rectangle = Hitbox;
			if (!Input.GetMouseButton (0) || GridManager.Instance.GridDir != mData.Direction) {
				MouseInterfaceID = -1;
				IsSelected = false;

				mIndicator.NeedsDestroy = true;
				AttachToGridY ();
				UndoSystem.RegisterUndo (new UndoSystem.NoteDataChangeUndoAction (mData, dataBefore, new UndoSystem.NoteDataBefore (mData)));
			}
		}
		if (MouseInterfaceID == -1 && Input.GetMouseButtonDown (1) && JudgeContain ()) {
			MouseInterfaceID = Mathf.Abs (GetHashCode ());

			yield return new WaitForEndOfFrame ();
			Destroy (gameObject);
			if (mData.NoteType != "HOLD" && mData.NoteType != "SUB")
				UndoSystem.RegisterUndo (new UndoSystem.CreateNoteUndoAction (mData, name.Split ('(') [0], false));
			else if (mData.Sub != null)
				UndoSystem.RegisterUndo(new UndoSystem.HoldCreateUndoAction(mData, mData.Sub, false));
		}
		if (IsSelected) {
			if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
				mData.Width += Input.GetAxis ("Mouse Y") * 0.8f;
				if (mData.Width < 0.05f)
					mData.Width = 0.05f;
				else
					mData.Position -= Input.GetAxis ("Mouse Y") * 0.4f;
			}
			else {
				float UNX;
				float UNY;
				if (mData.Direction == 0) {
					UNX = transform.position.x + Input.GetAxis ("Mouse X");
					UNY = transform.position.y + Input.GetAxis ("Mouse Y");
				} else if (mData.Direction == 1) {
					UNX = (transform.position.y + Input.GetAxis ("Mouse Y")) / 9f * 16f / 0.8f;
					UNY = -(transform.position.x + Input.GetAxis ("Mouse X")) + 4.255f - 2f;
				} else {
					UNX = (transform.position.y + Input.GetAxis ("Mouse Y")) / 9f * 16f / 0.8f;
					UNY = (transform.position.x + Input.GetAxis ("Mouse X")) + 4.25f - 2f;
				}
				mData.Position = ParamCalculator.UnityXToVanillaPosition (UNX) - mData.Width / 2;
				mData.Time = ParamCalculator.PositionYToTime (UNY);
			}
		}
	}

	public void AttachToGridY () {
		if (Mathf.Abs (GridManager.Instance.GridDir) > 1)
			return;
		int minabs = int.MaxValue;
		int time = mData.Time;
		for (int i = 0; i < GridManager.GridLines.Count; i++) {
			if (Mathf.Abs (GridManager.GridLines [i].Time - mData.Time) < minabs) {
				minabs = Mathf.Abs (GridManager.GridLines [i].Time - mData.Time);
				time = GridManager.GridLines [i].Time;
			}
		}
		mData.Time = time;
	}

	public void AttachToGridX () {
		if (GridManager.Instance.NumRows <= 0)
			return;
		dataBefore = new UndoSystem.NoteDataBefore (mData);
		float minabs = int.MaxValue;
		float pos = mData.Position;
		for (int i = 0; i < GridManager.GridCols.Count; i++) {
			float curtime = GridManager.GridCols [i].Time;
			curtime /= 1000f;
			curtime = ParamCalculator.UnityXToVanillaPosition (curtime);
			if (mData.Direction != 0) {
				curtime = curtime * 1.25f - 0.625f;
			}
			curtime -= mData.Width / 2f;
			if (Mathf.Abs (curtime - mData.Position) < minabs) {
				minabs = Mathf.Abs (curtime - mData.Position);
				pos = curtime;
			}
		}
		mData.Position = pos;
		UndoSystem.RegisterUndo (new UndoSystem.NoteDataChangeUndoAction (mData, dataBefore, new UndoSystem.NoteDataBefore (mData)));
	}
}
