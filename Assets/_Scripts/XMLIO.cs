using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public static class XMLIO {
	public static void Read (FileStream file) {
		foreach (var nd in NoteData.Instances.ToArray ())
			MonoBehaviour.DestroyObject (nd.gameObject);
		XmlDocument doc = new XmlDocument ();
		doc.Load (file);
		XmlNode cmap = doc.SelectSingleNode ("CMap");
		ChartInfoManager.Name = cmap.SelectSingleNode ("m_path") == null ? file.Name.Replace(".xml", "") : cmap.SelectSingleNode ("m_path").InnerText;
		ChartInfoManager.BPM = float.Parse (cmap.SelectSingleNode ("m_barPerMin").InnerText) * 4;
		ChartInfoManager.Offset = (int)(-float.Parse (cmap.SelectSingleNode ("m_timeOffset").InnerText) * 240000 / ChartInfoManager.BPM);

		XmlNode piano = cmap.SelectSingleNode ("m_notes");
		piano = piano.SelectSingleNode ("m_notes");
		int kind = 0;
		try {
			ReadNotes (0, piano.ChildNodes);
		}
		catch {
			kind++;
			piano = piano.ParentNode;
			ReadNotes (0, piano.ChildNodes);
		}

		XmlNode left = cmap.SelectSingleNode ("m_notesLeft");
		if (kind == 0)
			left = left.SelectSingleNode ("m_notes");
		ReadNotes (-1, left.ChildNodes);

		XmlNode right = cmap.SelectSingleNode ("m_notesRight");
		if (kind == 0)
			right = right.SelectSingleNode ("m_notes");
		ReadNotes (1, right.ChildNodes);
		try {
			ChartInfoManager.Difficulty = (ChartInfoManager.Difficulties)Enum.Parse (
				typeof(ChartInfoManager.Difficulties), 
				cmap.SelectSingleNode ("m_mapID").InnerText.Substring (cmap.SelectSingleNode ("m_mapID").InnerText.Length - 1),
				true
			);
		}
		catch {
			Debug.LogWarning ("Difficulty Unknown");
		}
	}

	public static void ReadNotes (int direction, XmlNodeList nodes) {
		SubsToRefer.Clear ();
		foreach (XmlNode node in nodes) {
			GameObject go = ReadNote1 (nodes, node);
			if (go != null)
				go.GetComponent<NoteData> ().Direction = direction;
		}
		foreach (XmlNode node in nodes) {
			GameObject go = ReadNote2 (nodes, node);
			if (go != null)
				go.GetComponent<NoteData> ().Direction = direction;
		}
	}

	public static SortedDictionary<int, NoteData> SubsToRefer = new SortedDictionary<int, NoteData>();

	public static GameObject ReadNote1 (XmlNodeList list, XmlNode node) {
		GameObject go = null;
		NoteData nd = null;
		Debug.Log (node.OuterXml);
		switch (node.SelectSingleNode ("m_type").InnerText) {
		case "NORMAL":
		case "0":
			go = GameObject.Instantiate (Resources.Load<GameObject> ("NoteNormal"));
			break;
		case "CHAIN":
		case "1":
			go = GameObject.Instantiate (Resources.Load<GameObject> ("NoteChain"));
			break;
		case "HOLD":
		case "2":
			go = GameObject.Instantiate (Resources.Load<GameObject> ("NoteHoldHold"));
			int sub = int.Parse (node.SelectSingleNode ("m_subId").InnerText);
			SubsToRefer.Add (sub, go.GetComponent<NoteData> ());
			break;
		case "SUB":
		case "3":
			return null;
		}
		nd = go.GetComponent<NoteData> ();
		nd.Position = float.Parse (node.SelectSingleNode ("m_position").InnerText);
		nd.Width = float.Parse (node.SelectSingleNode ("m_width").InnerText);
		nd.Time = ParamCalculator.BarToTime (float.Parse (node.SelectSingleNode ("m_time").InnerText));
		nd.NotifyWidth = true;
		return go;
	}

	public static GameObject ReadNote2 (XmlNodeList list, XmlNode node) {
		GameObject go = null;
		NoteData nd = null;
		Debug.Log (node.OuterXml);
		switch (node.SelectSingleNode ("m_type").InnerText) {
		case "SUB":
		case "3":
			go = GameObject.Instantiate (Resources.Load<GameObject> ("NoteHoldSub"));
			int id = int.Parse (node.SelectSingleNode ("m_id").InnerText);
			SubsToRefer [id].Sub = go.GetComponent<NoteData> ();

			GameObject render = GameObject.Instantiate (Resources.Load<GameObject> ("NoteHold"));
			render.GetComponent<HoldScaling> ().Begin = SubsToRefer [id].transform;
			render.GetComponent<HoldScaling> ().End = SubsToRefer [id].Sub.transform;

			SubsToRefer.Remove (id);
			break;
		default:
			return null;
		}
		nd = go.GetComponent<NoteData> ();
		nd.Position = float.Parse (node.SelectSingleNode ("m_position").InnerText);
		nd.Width = float.Parse (node.SelectSingleNode ("m_width").InnerText);
		nd.Time = ParamCalculator.BarToTime (float.Parse (node.SelectSingleNode ("m_time").InnerText));
		nd.NotifyWidth = true;
		return go;
	}

	public static void Write (Stream file) {
		XmlDocument doc = new XmlDocument ();
		doc.AppendChild (doc.CreateXmlDeclaration ("1.0", "utf-8", null));

		XmlElement cmap = doc.CreateElement ("CMap");
		cmap.SetAttribute ("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
		cmap.SetAttribute ("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
		doc.AppendChild (cmap);

		var mpath = doc.CreateElement ("m_path");
		mpath.InnerText = ChartInfoManager.Name;
		cmap.AppendChild (mpath);

		var mbpm = doc.CreateElement ("m_barPerMin");
		mbpm.InnerText = (ChartInfoManager.BPM / 4f).ToString ("0.00000");
		cmap.AppendChild (mbpm);

		var mofs = doc.CreateElement ("m_timeOffset");
		mofs.InnerText = (-ChartInfoManager.Offset * ChartInfoManager.BPM / 240000f).ToString ("0.00000");
		cmap.AppendChild (mofs);

		List<NoteData> pianoNotes = NoteData.Instances.FindAll ((n) => n.Direction == 0);
		pianoNotes.Sort ((a, b) => a.Time.CompareTo (b.Time));

		List<NoteData> leftNotes = NoteData.Instances.FindAll ((n) => n.Direction == -1);
		leftNotes.Sort ((a, b) => a.Time.CompareTo (b.Time));

		List<NoteData> rightNotes = NoteData.Instances.FindAll ((n) => n.Direction == 1);
		rightNotes.Sort ((a, b) => a.Time.CompareTo (b.Time));

		var mlr = doc.CreateElement ("m_leftRegion");
		mlr.InnerText = leftNotes.Exists ((n) => n.NoteType == "CHAIN") ? "MIXER" : "PAD";
		cmap.AppendChild (mlr);

		var mrr = doc.CreateElement ("m_rightRegion");
		mrr.InnerText = rightNotes.Exists ((n) => n.NoteType == "CHAIN") ? "MIXER" : "PAD";
		cmap.AppendChild (mrr);

		var mmid = doc.CreateElement ("m_mapID");
		mmid.InnerText = "_map_" + ChartInfoManager.Name + "_" + ChartInfoManager.Difficulty.ToString ();
		cmap.AppendChild (mmid);

		var mpiano = doc.CreateElement ("m_notes");
		var mpianon = doc.CreateElement ("m_notes");
		AddNotes (mpianon, pianoNotes);
		mpiano.AppendChild (mpianon);
		cmap.AppendChild (mpiano);

		var mleft = doc.CreateElement ("m_notesLeft");
		var mleftn = doc.CreateElement ("m_notes");
		AddNotes (mleftn, leftNotes);
		mleft.AppendChild (mleftn);
		cmap.AppendChild (mleft);

		var mright = doc.CreateElement ("m_notesRight");
		var mrightn = doc.CreateElement ("m_notes");
		AddNotes (mrightn, rightNotes);
		mright.AppendChild (mrightn);
		cmap.AppendChild (mright);

		XmlTextWriter xtw = new XmlTextWriter (file, System.Text.Encoding.UTF8);
		xtw.Formatting = Formatting.Indented;
		doc.WriteTo (xtw);
		xtw.Flush ();
		xtw.Close ();
	}

	public static void AddNotes (XmlElement element, List<NoteData> notes) {
		for (var i = 0; i < notes.Count; i++) {
			NoteData note = notes [i];

			XmlElement e = element.OwnerDocument.CreateElement ("CMapNoteAsset");

			XmlElement id = element.OwnerDocument.CreateElement ("m_id");
			id.InnerText = i.ToString ();
			e.AppendChild (id);

			XmlElement ty = element.OwnerDocument.CreateElement ("m_type");
			ty.InnerText = note.NoteType;
			e.AppendChild (ty);

			XmlElement ti = element.OwnerDocument.CreateElement ("m_time");
			ti.InnerText = ParamCalculator.TimeToBar (note.Time).ToString ("0.00000");
			e.AppendChild (ti);

			XmlElement po = element.OwnerDocument.CreateElement ("m_position");
			po.InnerText = note.Position.ToString ("0.00000");
			e.AppendChild (po);

			XmlElement wi = element.OwnerDocument.CreateElement ("m_width");
			wi.InnerText = note.Width.ToString ("0.00000");
			e.AppendChild (wi);

			XmlElement su = element.OwnerDocument.CreateElement ("m_subId");
			su.InnerText = note.Sub ? notes.IndexOf (note.Sub).ToString () : "-1";
			e.AppendChild (su);

			element.AppendChild (e);
		}
	}
}
