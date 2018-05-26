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
		if (ExportVideoSystem.Instance.IsRecording)
			scale.x -= 0.015f;
		else
			scale.x -= 0.42f * Time.deltaTime;
		transform.localScale = scale;
		if (ExportVideoSystem.Instance.IsRecording)
			srenderer.color -= new Color (0, 0, 0, 0.09f);
		else
			srenderer.color -= new Color (0, 0, 0, 2.4f * Time.deltaTime);
		if (srenderer.color.a < 0.1f)
			Destroy (this.gameObject);
	}
}
