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
		if (UndoList.Count >= 32767)
			UndoList.RemoveAt (0);
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

	public class NoteDataChangeUndoAction : IUndoAction {
		NoteData mNDRef;
		NoteDataBefore mBefore, mAfter;
		public NoteDataChangeUndoAction(NoteData ndRef, NoteDataBefore before, NoteDataBefore after) {
			mNDRef = ndRef;
			mBefore = before;
			mAfter = after;
		}
		#region IUndoAction implementation
		public void Undo () {
			mNDRef.Position = mBefore.Position;
			mNDRef.Time = mBefore.Time;
			mNDRef.Width = mBefore.Width;
			mNDRef.Direction = mBefore.Dir;
			mNDRef.NotifyWidth = true;
		}
		public void Redo () {
			mNDRef.Position = mAfter.Position;
			mNDRef.Time = mAfter.Time;
			mNDRef.Width = mAfter.Width;
			mNDRef.Direction = mAfter.Dir;
			mNDRef.NotifyWidth = true;
		}
		#endregion
	}

	public struct NoteDataBefore {
		public float Position, Width;
		public int Time, Dir;
		public NoteDataBefore(NoteData source) {
			Position = source.Position;
			Width = source.Width;
			Time = source.Time;
			Dir = source.Direction;
		}
	}
}
