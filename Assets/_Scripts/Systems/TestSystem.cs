using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class TestSystem : MonoBehaviour {
	public bool TestAviOutput = false;
	public bool TestTest = false;
	public bool TestTimeErrorProblem = false;
	void Update () {
		if (TestAviOutput) {
			StartCoroutine (_testAviOutput ());
		}
		if (TestTest) {
			print (Application.targetFrameRate);
			print (Application.dataPath);
			print (File.Exists (Application.dataPath + "/ffmpeg.exe"));
			print (Application.dataPath + "/ffmpeg.exe" + " -i " + Application.dataPath + "/latent.avi -i \"" + PlaybackSystem.FileName + "\" -vcodec copy -acodec aac -map 0:v:0 -map 1:a:0 -shortest " + Application.dataPath + "/output.avi");

			TestTest = false;
		}
		if (TestTimeErrorProblem) {
			TestTimeErrorProblem = false;

			for (int i = 0; i < NoteData.Instances.Count; i++)
				for (int j = i + 1; j < NoteData.Instances.Count; j++) {
					if (NoteData.Instances [i].NoteType == "CHAIN" && NoteData.Instances [i].Direction != 0)
						continue;
					if (NoteData.Instances [j].NoteType == "CHAIN" && NoteData.Instances [j].Direction != 0)
						continue;
					if (NoteData.Instances [i].NoteType != "CHAIN" && NoteData.Instances [j].NoteType != "CHAIN")
					if (Mathf.Abs (NoteData.Instances [i].Time - NoteData.Instances [j].Time) > 0 && Mathf.Abs (NoteData.Instances [i].Time - NoteData.Instances [j].Time) < 80)
						print ("Maybe Time Error Problem: " + NoteData.Instances [i].Time + " " + NoteData.Instances [j].Time);
				}
		}
	}

	int count = 0;
	IEnumerator _testAviOutput () {
		Directory.CreateDirectory (Application.dataPath + "/video_temp");
		Rect rect = new Rect (0, 0, Screen.width, Screen.height);
		Texture2D d = new Texture2D ((int) rect.width, (int) rect.height);
		TimeControlSystem.CurrentTimeMillis += 40;
		yield return new WaitForEndOfFrame ();
		count++;
		d.ReadPixels (rect, 0, 0, false);
		File.WriteAllBytes (Application.dataPath + "/video_temp/" + count.ToString ("00000") + ".jpg", d.EncodeToJPG ());

		if (count >= 60) {
			count = 0;
			TestAviOutput = false;
			Process.Start (Application.dataPath + "/ffmpeg.exe", " -threads 4 -y -r 25 -i " + Application.dataPath + "/video_temp/%05d.jpg -s " + Screen.width + "x" + Screen.height + " -vcodec copy " + Application.dataPath + "output.avi");
		}
	}
}
