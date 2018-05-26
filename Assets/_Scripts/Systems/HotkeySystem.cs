using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

public class HotkeySystem : MonoBehaviour {
	public Canvas Canvas;
	public static HotkeySystem Instance;
	public static bool IsWorking = true;

	public static float[] WidthStore = { 1, 1, 1, 1 };
	public static int WidthStoreN = 0;

	void Update () {
		Instance = this;
		if (!IsWorking)
			return;
		if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
		    && (Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt)))
			KeysWithControlAlt ();
		else
			KeysBared ();
	}

	public void KeysWithControlAlt() {
		if (Input.GetKeyDown (KeyCode.O)) {
			Dialogs.OpenXml ();
		} else if (Input.GetKeyDown (KeyCode.I)) {
			Dialogs.OpenMusic ();
		} else if (Input.GetKeyDown (KeyCode.R)) {
			if (PlaybackSystem.GetMillisTotal () < 500)
				MessageBox.Show ("You Should Load A Music First!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else if (MessageBox.Show ("Confirm Exporting Video?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) {
				ExportVideoSystem.Instance.RecordOnce ();
			}
		} else if (Input.GetKeyDown (KeyCode.K)) {
			Instantiate (Resources.Load ("UI/FormChartInfo"), Canvas.transform);
		} else if (Input.GetKeyDown (KeyCode.S)) {
			Dialogs.SaveXml ();
		} else if (Input.GetKeyDown (KeyCode.V)) {
			if (TimeControl.Instance == null)
				Instantiate (Resources.Load<GameObject> ("UI/TimeControl"), Canvas.transform);
			else {
				NoteSelection.MouseInterfaceID = -1;
				DestroyObject (TimeControl.Instance.gameObject);
				TimeControl.Instance = null;
			}
		} else if (Input.GetKeyDown (KeyCode.Z)) {
			UndoSystem.Undo ();
		} else if (Input.GetKeyDown (KeyCode.Y) || Input.GetKeyDown (KeyCode.X)) {
			UndoSystem.Redo ();
		}
	}

	public void KeysBared() {
		if (Input.GetKeyDown (KeyCode.Space))
			TimeControlSystem.isPlaying = !TimeControlSystem.isPlaying;
		if (Input.GetKeyDown (KeyCode.Z)) {
			if (Mathf.Abs (GridManager.Instance.GridDir) > 1)
				MessageBox.Show ("You Should Enable Grids First!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			NewNote ("NoteNormal");
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			if (Mathf.Abs (GridManager.Instance.GridDir) > 1)
				MessageBox.Show ("You Should Enable Grids First!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			NewNote ("NoteChain");
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			if (Mathf.Abs (GridManager.Instance.GridDir) > 1)
				MessageBox.Show ("You Should Enable Grids First!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			GameObject go = NewNote ("NoteHoldSub", 500);
			Transform ed = go.transform;

			go = NewNote ("NoteHoldHold");
			Transform bg = go.transform;

			go = GameObject.Instantiate (Resources.Load<GameObject> ("NoteHold"));
			HoldScaling hs = go.GetComponent<HoldScaling> ();
			hs.Begin = bg;
			hs.End = ed;
		}

		KeyCode[] ls = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
		for (int i = 0; i < 4; i++)
			if (Input.GetKeyDown (ls [i]))
				WidthStoreN = i;

		if (Input.GetKeyDown (KeyCode.G)) {
			GridManager.Instance.GridDir++;
			if (GridManager.Instance.GridDir > 1)
				GridManager.Instance.GridDir = -1;
		}
	}

	public GameObject NewNote(string path, int timedelta=0) {
		GameObject go = GameObject.Instantiate (Resources.Load<GameObject> (path));
		NoteData nd = go.GetComponent<NoteData> ();
		nd.Direction = GridManager.Instance.GridDir;
		go.transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 tmp = go.transform.position;
		tmp.z = 0;
		if (GridManager.Instance.GridDir == 1) {
			tmp.x = go.transform.position.y / 9f * 16f / 0.8f;
			tmp.y = -go.transform.position.x + 4.255f - 2f;
		} else if (GridManager.Instance.GridDir == -1) {
			tmp.x = go.transform.position.y / 9f * 16f / 0.8f;
			tmp.y = go.transform.position.x + 4.25f - 2f;
		}
		go.transform.position = tmp;
		nd.Position = ParamCalculator.UnityXToVanillaPosition (tmp.x) - nd.Width / 2;
		nd.Time = ParamCalculator.PositionYToTime (tmp.y) + timedelta;
		nd.Width = WidthStore [WidthStoreN];
		nd.NotifyWidth = true;
		go.GetComponent<NoteSelection> ().ShouldAttachGridY = true;
		return go;
	}

}
