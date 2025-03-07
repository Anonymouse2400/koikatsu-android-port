using UnityEngine;

namespace Studio
{
	public static class GuideCommand
	{
		public class AddInfo
		{
			public int dicKey;

			public Vector3 value;
		}

		public class EqualsInfo
		{
			public int dicKey;

			public Vector3 oldValue;

			public Vector3 newValue;
		}

		public class MoveAddCommand : ICommand
		{
			private AddInfo[] changeAmountInfo;

			public MoveAddCommand(AddInfo[] _changeAmountInfo)
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
						changeAmount.pos += changeAmountInfo[i].value;
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
						changeAmount.pos -= changeAmountInfo[i].value;
					}
				}
			}
		}

		public class MoveEqualsCommand : ICommand
		{
			private EqualsInfo[] changeAmountInfo;

			public MoveEqualsCommand(EqualsInfo[] _changeAmountInfo)
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
						changeAmount.pos = changeAmountInfo[i].newValue;
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
						changeAmount.pos = changeAmountInfo[i].oldValue;
					}
				}
			}
		}

		public class RotationAddCommand : ICommand
		{
			private AddInfo[] changeAmountInfo;

			public RotationAddCommand(AddInfo[] _changeAmountInfo)
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
						changeAmount.rot += changeAmountInfo[i].value;
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
						changeAmount.rot = changeAmountInfo[i].value;
					}
				}
			}
		}

		public class RotationEqualsCommand : ICommand
		{
			private EqualsInfo[] changeAmountInfo;

			public RotationEqualsCommand(EqualsInfo[] _changeAmountInfo)
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
						changeAmount.rot = changeAmountInfo[i].newValue;
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
						changeAmount.rot = changeAmountInfo[i].oldValue;
					}
				}
			}
		}

		public class ScaleEqualsCommand : ICommand
		{
			private EqualsInfo[] changeAmountInfo;

			public ScaleEqualsCommand(EqualsInfo[] _changeAmountInfo)
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
						changeAmount.scale = changeAmountInfo[i].newValue;
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
						changeAmount.scale = changeAmountInfo[i].oldValue;
					}
				}
			}
		}
	}
}
