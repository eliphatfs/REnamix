using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRenderTricks : MonoBehaviour {
	SpriteRenderer[] mRenderers;
	NoteData mData;
	// Use this for initialization
	void Start () {
		mRenderers = GetComponentsInChildren<SpriteRenderer> ();
		mData = GetComponent<NoteData> ();
		foreach (var mRenderer in mRenderers)
			mRenderer.color = Color.clear;
	}
	
	// Update is called once per frame
	void Update () {
		Color c = new Color (1f, 1f, 1f, 1f);
		if (mData.Direction == 0 && transform.position.y > 1.4f)
			c.a = 2.4f - transform.position.y;
		else if (mData.Direction != 0 && mData.Direction * (transform.position.x) < 2f)
			c.a = mData.Direction * (transform.position.x) - 1f;

		c.a = Mathf.Clamp01 (c.a);
		foreach (var mRenderer in mRenderers)
			mRenderer.color = c;
	}
}
