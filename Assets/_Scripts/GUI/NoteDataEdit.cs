using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteDataEdit : MonoBehaviour {
	public NoteData Editing;

	public InputField POSITION, WIDTH, TIME;
	public Dropdown DIR;

	bool initiated;

	// Use this for initialization
	void Start () {
		initiated = false;
		NoteSelection.MouseInterfaceID = Mathf.Abs (GetHashCode ());
	}
	
	// Update is called once per frame
	void Update () {
		if (!initiated) {
			initiated = true;
			POSITION.text = Editing.Position.ToString ();
			WIDTH.text = Editing.Width.ToString ();
			TIME.text = Editing.Time.ToString ();
			DIR.value = Editing.Direction + 1;
		}
		if (Editing.transform.position.x < 0)
			GetComponent<RectTransform> ().anchoredPosition = new Vector2 (820, 38);
		else
			GetComponent<RectTransform> ().anchoredPosition = new Vector2 (120, 38);
		Rect Hitbox = GetComponent<RectTransform> ().rect;
		Hitbox.y -= 50f;
		Hitbox.height += 50f;
		if (Input.GetMouseButtonDown (0) && !Hitbox.Contains (GetComponent<RectTransform> ().worldToLocalMatrix.MultiplyPoint3x4(Camera.main.ScreenToWorldPoint (Input.mousePosition)))) {
			NoteSelection.MouseInterfaceID = -1;
			Destroy (gameObject);
		}
		Editing.Position = float.Parse (POSITION.text);
		Editing.Time = int.Parse (TIME.text);
		Editing.Width = float.Parse (WIDTH.text);
		Editing.NotifyWidth = true;
		Editing.Direction = DIR.value - 1;
		if (Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt)) {
			KeyCode[] ls = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
			for (int i = 0; i < 4; i++) {
				if (Input.GetKeyDown (ls [i])) {
					HotkeySystem.WidthStore [i] = Editing.Width;
				}
			}
		}
	}
}
