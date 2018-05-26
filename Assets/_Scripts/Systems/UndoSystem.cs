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
		int mID;
		NoteDataBefore mBefore, mAfter;
		public NoteDataChangeUndoAction(NoteData ndRef, NoteDataBefore before, NoteDataBefore after) {
			mID = ndRef.InnerID;
			mBefore = before;
			mAfter = after;
		}
		#region IUndoAction implementation
		public void Undo () {
			var mNDRef = NoteData.Instances.Find ((i) => i.InnerID == mID);
			mNDRef.Position = mBefore.Position;
			mNDRef.Time = mBefore.Time;
			mNDRef.Width = mBefore.Width;
			mNDRef.Direction = mBefore.Dir;
			mNDRef.NotifyWidth = true;
		}
		public void Redo () {
			var mNDRef = NoteData.Instances.Find ((i) => i.InnerID == mID);
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
		public int ID;
		public NoteDataBefore(NoteData source) {
			Position = source.Position;
			Width = source.Width;
			Time = source.Time;
			Dir = source.Direction;
			ID = source.InnerID;
		}
	}

	public class CreateNoteUndoAction : IUndoAction {
		NoteDataBefore mNoteData;
		public NoteData CurrentData;
		string mPrefabPath;
		bool mIsCreate;
		public CreateNoteUndoAction (NoteData data, string path, bool createOrdestroy=true) {
			mNoteData = new NoteDataBefore(data);
			mPrefabPath = path;
			mIsCreate = createOrdestroy;
		}

		private void _undo () {
			GameObject.DestroyObject (NoteData.Instances.Find ((i) => i.InnerID == mNoteData.ID).gameObject);
		}
		private void _redo () {
			CurrentData = HotkeySystem.NewNote (mPrefabPath).GetComponent<NoteData>();
			CurrentData.Position = mNoteData.Position;
			CurrentData.Time = mNoteData.Time;
			CurrentData.Width = mNoteData.Width;
			CurrentData.Direction = mNoteData.Dir;
			CurrentData.InnerID = mNoteData.ID;
			CurrentData.NotifyWidth = true;
		}

		#region IUndoAction implementation
		public void Undo () {
			if (mIsCreate)
				_undo ();
			else
				_redo ();
		}

		public void Redo () {
			if (!mIsCreate)
				_undo ();
			else
				_redo ();
		}
		#endregion
	}

	public class HoldCreateUndoAction : IUndoAction {
		CreateNoteUndoAction mBeginAction, mSubAction;
		bool mIsCreate;
		public HoldCreateUndoAction(NoteData begin, NoteData sub, bool isCreate=true) {
			mBeginAction = new CreateNoteUndoAction(begin, "NoteHoldHold", isCreate);
			mSubAction = new CreateNoteUndoAction(sub, "NoteHoldSub", isCreate);
			mIsCreate = isCreate;
		}
		private void _recreate() {
			var go = GameObject.Instantiate (Resources.Load<GameObject> ("NoteHold"));
			HoldScaling hs = go.GetComponent<HoldScaling> ();
			hs.Begin = mBeginAction.CurrentData.transform;
			hs.End = mSubAction.CurrentData.transform;
		}
		#region IUndoAction implementation
		public void Undo () {
			mBeginAction.Undo ();
			mSubAction.Undo ();
			if (!mIsCreate)
				_recreate ();
		}
		public void Redo () {
			mBeginAction.Redo ();
			mSubAction.Redo ();
			if (mIsCreate)
				_recreate ();
		}
		#endregion
	}
}
