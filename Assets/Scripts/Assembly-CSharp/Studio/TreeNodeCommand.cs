using UnityEngine;

namespace Studio
{
	public static class TreeNodeCommand
	{
		public class MoveCopyInfo
		{
			public int dicKey;

			public Vector3[] oldValue = new Vector3[2]
			{
				Vector3.zero,
				Vector3.zero
			};

			public Vector3[] newValue = new Vector3[2]
			{
				Vector3.zero,
				Vector3.zero
			};

			public MoveCopyInfo(int _dicKey, ChangeAmount _old, ChangeAmount _new)
			{
				dicKey = _dicKey;
				oldValue = new Vector3[2] { _old.pos, _old.rot };
				newValue = new Vector3[2] { _new.pos, _new.rot };
			}
		}

		public class MoveCopyCommand : ICommand
		{
			private MoveCopyInfo[] changeAmountInfo;

			public MoveCopyCommand(MoveCopyInfo[] _changeAmountInfo)
			{
				changeAmountInfo = _changeAmountInfo;
			}

			public void Do()
			{
				for (int i = 0; i < changeAmountInfo.Length; i++)
				{
					ChangeAmount changeAmount = Studio.GetChangeAmount(changeAmountInfo[i].dicKey);
					if (changeAmount != null)
					{
						changeAmount.pos = changeAmountInfo[i].newValue[0];
						changeAmount.rot = changeAmountInfo[i].newValue[1];
					}
				}
			}

			public void Redo()
			{
				Do();
			}

			public void Undo()
			{
				for (int i = 0; i < changeAmountInfo.Length; i++)
				{
					ChangeAmount changeAmount = Studio.GetChangeAmount(changeAmountInfo[i].dicKey);
					if (changeAmount != null)
					{
						changeAmount.pos = changeAmountInfo[i].oldValue[0];
						changeAmount.rot = changeAmountInfo[i].oldValue[1];
					}
				}
			}
		}
	}
}
