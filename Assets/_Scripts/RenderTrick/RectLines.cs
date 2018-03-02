using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class RectLines : MonoBehaviour {
	public Rect Rectangle;
	public float LineWidth;

	public bool NeedsDestroy;

	private LineRenderer mLineRenderer;
	// Use this for initialization
	void Start () {
		mLineRenderer = GetComponent<LineRenderer> ();
		mLineRenderer.enabled = false;
		NeedsDestroy = false;
	}
	
	// Update is called once per frame
	void Update () {
		mLineRenderer.useWorldSpace = true;
		mLineRenderer.startWidth = LineWidth;
		mLineRenderer.endWidth = LineWidth;
		if (mLineRenderer.positionCount != 5)
			mLineRenderer.positionCount = 5;
		mLineRenderer.SetPosition (0, Rectangle.min);
		mLineRenderer.SetPosition (1, new Vector3 (Rectangle.xMin, Rectangle.yMax));
		mLineRenderer.SetPosition (2, Rectangle.max);
		mLineRenderer.SetPosition (3, new Vector3 (Rectangle.xMax, Rectangle.yMin));
		mLineRenderer.SetPosition (4, Rectangle.min);
		if (!mLineRenderer.enabled)
			mLineRenderer.enabled = true;

		if (NeedsDestroy)
			Destroy (gameObject);
	}
}
