using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormChartInfo : MonoBehaviour {
	public InputField BPM, OFS, Name, BPRS, BPRM, GridDir, GridNum;
	public Dropdown Difficulty;

	public void EndEditBPMInput (InputField field) {
		ChartInfoManager.BPM = float.Parse (field.text);
	}

	public void EndEditOFSInput (InputField field) {
		ChartInfoManager.Offset = int.Parse (field.text);
	}

	public void EndEditNameInput (InputField field) {
		ChartInfoManager.Name = field.text;
	}

	public void EndEditDifficulty (Dropdown field) {
		ChartInfoManager.Difficulty = (ChartInfoManager.Difficulties)field.value;
	}

	public void EndEditBPRS (InputField field) {
		GridManager.Instance.BeatPerRowS = int.Parse (field.text);
	}

	public void EndEditBPRM (InputField field) {
		GridManager.Instance.BeatPerRowM = int.Parse (field.text);
	}

	public void EndEditGridDir (InputField field) {
		GridManager.Instance.GridDir = int.Parse (field.text);
	}

	public void EndEditGridNum (InputField field) {
		GridManager.Instance.NumRows = int.Parse (field.text);
	}

	public void OnEnable () {
		HotkeySystem.IsWorking = false;
		BPM.text = ChartInfoManager.BPM.ToString ();
		OFS.text = ChartInfoManager.Offset.ToString ();
		Name.text = ChartInfoManager.Name;
		Difficulty.value = (int)(ChartInfoManager.Difficulty);
		BPRS.text = GridManager.Instance.BeatPerRowS.ToString ();
		BPRM.text = GridManager.Instance.BeatPerRowM.ToString ();
		GridDir.text = GridManager.Instance.GridDir.ToString ();
		GridNum.text = GridManager.Instance.NumRows.ToString ();
	}

	public void Dismiss () {
		HotkeySystem.IsWorking = true;
		DestroyObject (gameObject);
	}
}
