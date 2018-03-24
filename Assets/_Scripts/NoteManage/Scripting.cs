using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

public class Scripting {
	public const string HEADER = @"
using System;
using System.Collections;
using System.Collections.Generic;
namespace RenamixDynamicCompile {
	public class DynamicCompile($_SEED)NoteData {
		public float Position, Width;
		public int Time, Direction;
		public int Type;
	}
	public class DynamicCompile($_SEED) {
		public List<DynamicCompile($_SEED)NoteData> Notes = new List<DynamicCompile($_SEED)NoteData>();
		public void __wrapper_init (object[] datain) {
			for (int i = 0; i < datain.Length; i += 5) {
				DynamicCompile($_SEED)NoteData c = new DynamicCompile($_SEED)NoteData();
				c.Position = (float) datain[i];
				c.Width = (float) datain[i + 1];
				c.Time = (int) datain[i + 2];
				c.Direction = (int) datain[i + 3];
				c.Type = (int) datain[i + 4];
				Notes.Add(c);
			}
		}
		public List<object> Wrapper() {
";
	public const string FOOTER = @"
			List<object> __wrapper_list = new List<object>();
			for (int i = 0; i < Notes.Count; i++) {
				__wrapper_list.Add(Notes[i].Position);
				__wrapper_list.Add(Notes[i].Width);
				__wrapper_list.Add(Notes[i].Time);
				__wrapper_list.Add(Notes[i].Direction);
				__wrapper_list.Add(Notes[i].Type);
			}
			return __wrapper_list;
		}
	}
}
";
	public void Execute(string code) {
		int rseed = UnityEngine.Random.Range (0, 1073741824);
		string toCompile = HEADER.Replace ("($_SEED)", rseed.ToString()) + code + FOOTER;

		CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider ();
		ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler ();
		CompilerParameters objCompilerParameters = new CompilerParameters ();
		objCompilerParameters.ReferencedAssemblies.Add ("System.dll");
		objCompilerParameters.GenerateExecutable = false;
		objCompilerParameters.GenerateInMemory = true;

		CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource (objCompilerParameters, toCompile);

		if (cr.Errors.HasErrors) {
			StringBuilder builder = new StringBuilder ();
			builder.AppendLine ("Compile Error：");
			foreach (CompilerError err in cr.Errors) {
				builder.AppendLine (err.ErrorText);
			}
			MessageBox.Show (builder.ToString (), "Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		} else {
			Assembly objAssembly = cr.CompiledAssembly;
			object dc = objAssembly.CreateInstance ("RenamixDynamicCompile.DynamicCompile" + rseed);
			MethodInfo mi = dc.GetType ().GetMethod ("__wrapper_init");
			List<object> datain = new List<object> ();
			for (int i = 0; i < NoteData.Instances.Count; i++) {
				datain.Add (NoteData.Instances [i].Position);
				datain.Add (NoteData.Instances [i].Width);
				datain.Add (NoteData.Instances [i].Time);
				datain.Add (NoteData.Instances [i].Direction);
				switch (NoteData.Instances [i].NoteType) {
				case "NORMAL":
					datain.Add (0);
					break;
				case "CHAIN":
					datain.Add (1);
					break;
				case "HOLD":
					datain.Add (2);
					break;
				case "SUB":
					datain.Add (3);
					break;
				}
			}
			List<object> result = mi.Invoke (dc, datain.ToArray ()) as List<object>;

		}
	}
}

