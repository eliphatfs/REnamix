using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class ExportVideoSystem : MonoBehaviour {
	public static ExportVideoSystem Instance = null;
	void Start () {
		Instance = this;
		rect = new Rect (0, 0, Screen.width, Screen.height);
		d = new Texture2D (Screen.width, Screen.height);
	}

	protected int count = 0;
	protected int lastTime = 0;
	public bool IsRecording = false;

	public void RecordOnce() {
		GridManager.Instance.GridDir = 2;
		GridManager.Instance.NumRows = 0;
		HotkeySystem.IsWorking = false;
		Directory.CreateDirectory (Application.dataPath + "/video_temp");
		IsRecording = true;
		count = 0;
		lastTime = -ChartInfoManager.Offset;
		TimeControlSystem.isPlaying = true;
		PlaybackSystem.Stop ();
	}
	Texture2D d = null;
	Rect rect;

	IEnumerator RecordOneFrame () {
		TimeControlSystem.CurrentTimeMillis = lastTime;
		lastTime += 40;
		yield return new WaitForEndOfFrame ();
		d.ReadPixels (rect, 0, 0, false);
		count++;
		File.WriteAllBytes (Application.dataPath + "/video_temp/" + count.ToString ("00000") + ".jpg", d.EncodeToJPG ());

		if (TimeControlSystem.CurrentTimeMillis > PlaybackSystem.GetMillisTotal ()) {
			IsRecording = false;
			TimeControlSystem.isPlaying = false;
			TimeControlSystem.CurrentTimeMillis = 0;
			ProcessStartInfo inf = new ProcessStartInfo (Application.dataPath + "/ffmpeg.exe", " -threads 4 -y -r 25 -i " + Application.dataPath + "/video_temp/%05d.jpg -s " + Screen.width + "x" + Screen.height + " -vcodec copy " + Application.dataPath + "/latent.avi");
			inf.UseShellExecute = true;
			Process pro = Process.Start (inf);
			pro.WaitForExit ();
			Directory.Delete (Application.dataPath + "/video_temp", true);
			inf = new ProcessStartInfo (Application.dataPath + "/ffmpeg.exe", "-i " + Application.dataPath + "/latent.avi -i \"" + PlaybackSystem.FileName + "\" -vcodec copy -acodec aac -map 0:v:0 -map 1:a:0 -shortest " + Application.dataPath + "/output.avi");
			inf.UseShellExecute = true;
			pro = Process.Start (inf);
			pro.WaitForExit ();
			System.Windows.Forms.MessageBox.Show ("Output Completed!");
			HotkeySystem.IsWorking = true;
		}
	}

	void Update () {
		if (IsRecording) {
			StartCoroutine (RecordOneFrame ());
		}
	}
}
