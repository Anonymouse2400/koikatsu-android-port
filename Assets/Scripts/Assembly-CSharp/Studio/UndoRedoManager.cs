using System;
using System.Collections.Generic;
using UnityEngine;

namespace Studio
{
	public class UndoRedoManager : Singleton<UndoRedoManager>
	{
		private class Command : ICommand
		{
			private Delegate doMethod;

			private Delegate undoMethod;

			private object[] doParamater;

			private object[] undoParamater;

			public Command(Delegate _doMethod, object[] _doParamater, Delegate _undoMethod, object[] _undoParamater)
			{
				doMethod = _doMethod;
				doParamater = _doParamater;
				undoMethod = _undoMethod;
				undoParamater = _undoParamater;
			}

			public void Do()
			{
				doMethod.DynamicInvoke(doParamater);
			}

			public void Undo()
			{
				undoMethod.DynamicInvoke(undoParamater);
			}

			public void Redo()
			{
				doMethod.DynamicInvoke(doParamater);
			}
		}

		private Stack<ICommand> undo = new Stack<ICommand>();

		private Stack<ICommand> redo = new Stack<ICommand>();

		private bool m_CanUndo;

		private bool m_CanRedo;

		public bool CanUndo
		{
			get
			{
				return m_CanUndo;
			}
			private set
			{
				if (m_CanUndo != value)
				{
					m_CanUndo = value;
					if (this.CanUndoChange != null)
					{
						this.CanUndoChange(this, EventArgs.Empty);
					}
				}
			}
		}

		public bool CanRedo
		{
			get
			{
				return m_CanRedo;
			}
			private set
			{
				if (m_CanRedo != value)
				{
					m_CanRedo = value;
					if (this.CanRedoChange != null)
					{
						this.CanRedoChange(this, EventArgs.Empty);
					}
				}
			}
		}

		public event EventHandler CanUndoChange;

		public event EventHandler CanRedoChange;

		public void Do(ICommand _command)
		{
			if (_command != null)
			{
				undo.Push(_command);
				CanUndo = true;
				_command.Do();
				redo.Clear();
				CanRedo = false;
			}
		}

		public void Do(Delegate _doMethod, object[] _doParamater, Delegate _undoMethod, object[] _undoParamater)
		{
			Command command = new Command(_doMethod, _doParamater, _undoMethod, _undoParamater);
			Do(command);
		}

		public void Do()
		{
			if (undo.Count > 0)
			{
				Do(undo.Peek());
			}
		}

		public void Undo()
		{
			if (undo.Count > 0)
			{
				ICommand command = undo.Pop();
				CanUndo = undo.Count > 0;
				command.Undo();
				redo.Push(command);
				CanRedo = true;
			}
		}

		public void Redo()
		{
			if (redo.Count > 0)
			{
				ICommand command = redo.Pop();
				CanRedo = redo.Count > 0;
				command.Redo();
				undo.Push(command);
				CanUndo = true;
			}
		}

		public void Push(ICommand _command)
		{
			if (_command != null)
			{
				undo.Push(_command);
				CanUndo = true;
				redo.Clear();
				CanRedo = false;
			}
		}

		public void Clear()
		{
			undo.Clear();
			redo.Clear();
			CanUndo = false;
			CanRedo = false;
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}
	}
}
