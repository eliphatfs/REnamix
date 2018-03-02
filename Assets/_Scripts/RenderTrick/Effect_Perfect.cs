using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Perfect : MonoBehaviour {
	SpriteRenderer srenderer;
	// Use this for initialization
	void Start () {
		srenderer = this.GetComponent<SpriteRenderer> ();
		scale = transform.localScale;
	}
	Vector3 scale;
	// Update is called once per frame
	void Update () {
		scale.x -= 0.007f;
		if (ExportVideoSystem.Instance.IsRecording)
			scale.x -= 0.008f;
		transform.localScale = scale;
		srenderer.color -= new Color (0, 0, 0, 0.04f);
		if (ExportVideoSystem.Instance.IsRecording)
			srenderer.color -= new Color (0, 0, 0, 0.05f);
		if (srenderer.color.a < 0.1f)
			Destroy (this.gameObject);
	}
}
