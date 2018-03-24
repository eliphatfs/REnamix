using System;
using System.IO;
using System.Windows.Forms;

public static class Dialogs {
	
	public static void OpenXml () {
		OpenFileDialog dlg = new OpenFileDialog ();
		dlg.Title = "Choose a Map XML File to read";
		dlg.Filter = "XML File (*.xml)|*.xml";
		dlg.CheckFileExists = true;
		dlg.CheckPathExists = true;
		DialogResult result = dlg.ShowDialog ();
		if (result == DialogResult.OK || result == DialogResult.Yes) {
			FileStream s = dlg.OpenFile () as FileStream;
			XMLIO.Read (s);
			s.Close ();
		}
	}

	public static void SaveXml () {
		SaveFileDialog dlg = new SaveFileDialog ();
		dlg.Title = "Choose a path to output";
		dlg.Filter = "XML File (*.xml)|*.xml";
		dlg.AddExtension = true;
		dlg.CheckPathExists = true;
		dlg.OverwritePrompt = true;
		DialogResult result = dlg.ShowDialog ();
		if (result == DialogResult.OK || result == DialogResult.Yes) {
			Stream s = dlg.OpenFile ();
			XMLIO.Write (s);
			s.Close ();
		}
	}

	public static void OpenMusic () {
		OpenFileDialog dlg = new OpenFileDialog ();
		dlg.Title = "Choose an Audio File to read";
		dlg.Filter = "Audio File (*.wav, *.mp3)|*.wav;*.mp3";
		dlg.CheckFileExists = true;
		dlg.CheckPathExists = true;
		DialogResult result = dlg.ShowDialog ();
		if (result == DialogResult.OK || result == DialogResult.Yes) {
			PlaybackSystem.Load (dlg.FileName);
		}
	}

}
