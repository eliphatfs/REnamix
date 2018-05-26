using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoSystem {
	public static List<IUndoAction> UndoList = new List<IUndoAction>();
	public static List<IUndoAction> RedoList = new List<IUndoAction>();

	public interface IUndoAction {
		void Undo();
		void Redo();
	}
	public static void RegisterUndo(IUndoAction action) {
		UndoList.Add (action);
		RedoList.Clear ();
	}
	public static void Undo() {
		if (UndoList.Count == 0)
			return;
		RedoList.Add (UndoList [UndoList.Count - 1]);
		UndoList [UndoList.Count - 1].Undo ();
		UndoList.RemoveAt (UndoList.Count - 1);
	}
	public static void Redo() {
		if (RedoList.Count == 0)
			return;
		var x = RedoList [RedoList.Count - 1];
		x.Redo ();
		RedoList.RemoveAt (RedoList.Count - 1);
		UndoList.Add (x);
	}


	public sealed class WrappedUndoAction : IUndoAction {
		private System.Action mUndoAction, mRedoAction;
		public WrappedUndoAction(System.Action UndoAction, System.Action RedoAction) {
			mUndoAction = UndoAction;
			mRedoAction = RedoAction;
		}

		#region IUndoAction implementation
		public void Undo () {
			mUndoAction ();
		}

		public void Redo () {
			mRedoAction ();
		}
		#endregion
	}
}
