using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardcodeResolution : MonoBehaviour {
	void Awake () {
		#if UNITY_EDITOR
		return;
		#else
		Screen.SetResolution (960, 540, false, 60);
		#endif
	}
}
