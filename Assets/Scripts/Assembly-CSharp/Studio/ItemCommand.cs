using UnityEngine;

namespace Studio
{
	public static class ItemCommand
	{
		public class ColorInfo
		{
			public int dicKey { get; protected set; }

			public Color oldValue { get; protected set; }

			public Color newValue { get; protected set; }

			public ColorInfo(int _dicKey, Color _oldValue, Color _newValue)
			{
				dicKey = _dicKey;
				oldValue = _oldValue;
				newValue = _newValue;
			}
		}

		public class LineColorCommand : ICommand
		{
			private ColorInfo[] arrayInfo;

			public LineColorCommand(ColorInfo[] _array)
			{
				arrayInfo = _array;
			}

			public void Do()
			{
				ColorInfo[] array = arrayInfo;
				foreach (ColorInfo colorInfo in array)
				{
					OCIItem oCIItem = Studio.GetCtrlInfo(colorInfo.dicKey) as OCIItem;
					oCIItem.SetLineColor(colorInfo.newValue);
				}
			}

			public void Redo()
			{
				Do();
			}

			public void Undo()
			{
				ColorInfo[] array = arrayInfo;
				foreach (ColorInfo colorInfo in array)
				{
					OCIItem oCIItem = Studio.GetCtrlInfo(colorInfo.dicKey) as OCIItem;
					oCIItem.SetLineColor(colorInfo.oldValue);
				}
			}
		}
	}
}
