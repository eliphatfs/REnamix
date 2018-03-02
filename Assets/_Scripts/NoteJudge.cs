using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteJudge : MonoBehaviour {
	NoteData mData;
	public bool Judged {
		get;
		private set;
	}
	// Use this for initialization
	void Start () {
		mData = GetComponent<NoteData> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Judged && !TimeControlSystem.isPlaying) {
			foreach (var sr in GetComponentsInChildren<SpriteRenderer> ())
				sr.enabled = true;
			Judged = false;
		}
		if ((!Judged) && TimeControlSystem.isPlaying) {
			if (mData.Time - TimeControlSystem.CurrentTimeMillis >= -40 && mData.Time - TimeControlSystem.CurrentTimeMillis <= 40)
				Judge ();
		}
	}

	void Judge() {
		foreach (var sr in GetComponentsInChildren<SpriteRenderer> ())
			sr.enabled = false;
		GameObject pf = Instantiate (Resources.Load<GameObject> ("EffectPerfect"));
		Vector3 v;
		switch (mData.Direction) {
		case 0:
			v = transform.position;
			v.y = -0.52f;
			pf.transform.position  = v;
			break;
		case -1:
		case 1:
			v = transform.position;
			v.x = 2.79f * mData.Direction;
			pf.transform.position = v;
			break;
		}
		pf.transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles);
		Vector3 pls = transform.localScale;
		pls.y = 1;
		pls.x = 1.25f * ParamCalculator.VanillaWidthToScaleX (mData.Width) * 0.5f * (mData.Direction != 0 ? 9f / 16f * 0.8f : 1f);
		pf.transform.localScale = pls;
		Judged = true;
	}
}
