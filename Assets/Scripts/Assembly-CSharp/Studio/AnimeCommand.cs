namespace Studio
{
	public static class AnimeCommand
	{
		public class SpeedCommand : ICommand
		{
			private int[] arrayKey;

			private float speed;

			private float[] oldSpeed;

			public SpeedCommand(int[] _arrayKey, float _speed, float[] _oldSpeed)
			{
				arrayKey = _arrayKey;
				speed = _speed;
				oldSpeed = _oldSpeed;
			}

			public void Do()
			{
				for (int i = 0; i < arrayKey.Length; i++)
				{
					ObjectCtrlInfo ctrlInfo = Studio.GetCtrlInfo(arrayKey[i]);
					if (ctrlInfo != null)
					{
						ctrlInfo.animeSpeed = speed;
					}
				}
			}

			public void Redo()
			{
				Do();
			}

			public void Undo()
			{
				for (int i = 0; i < arrayKey.Length; i++)
				{
					ObjectCtrlInfo ctrlInfo = Studio.GetCtrlInfo(arrayKey[i]);
					if (ctrlInfo != null)
					{
						ctrlInfo.animeSpeed = oldSpeed[i];
					}
				}
			}
		}

		public class PatternCommand : ICommand
		{
			private int[] arrayKey;

			private float value;

			private float[] oldvalue;

			public PatternCommand(int[] _arrayKey, float _value, float[] _oldvalue)
			{
				arrayKey = _arrayKey;
				value = _value;
				oldvalue = _oldvalue;
			}

			public void Do()
			{
				for (int i = 0; i < arrayKey.Length; i++)
				{
					OCIChar oCIChar = Studio.GetCtrlInfo(arrayKey[i]) as OCIChar;
					if (oCIChar != null)
					{
						oCIChar.animePattern = value;
					}
				}
			}

			public void Redo()
			{
				Do();
			}

			public void Undo()
			{
				for (int i = 0; i < arrayKey.Length; i++)
				{
					OCIChar oCIChar = Studio.GetCtrlInfo(arrayKey[i]) as OCIChar;
					if (oCIChar != null)
					{
						oCIChar.animePattern = oldvalue[i];
					}
				}
			}
		}

		public class OptionParamCommand : ICommand
		{
			private int[] arrayKey;

			private float value;

			private float[] oldvalue;

			private int kind;

			public OptionParamCommand(int[] _arrayKey, float _value, float[] _oldvalue, int _kind)
			{
				arrayKey = _arrayKey;
				value = _value;
				oldvalue = _oldvalue;
				kind = _kind;
			}

			public void Do()
			{
				for (int i = 0; i < arrayKey.Length; i++)
				{
					OCIChar oCIChar = Studio.GetCtrlInfo(arrayKey[i]) as OCIChar;
					if (oCIChar != null)
					{
						if (kind == 0)
						{
							oCIChar.animeOptionParam1 = value;
						}
						else
						{
							oCIChar.animeOptionParam2 = value;
						}
					}
				}
			}

			public void Redo()
			{
				Do();
			}

			public void Undo()
			{
				for (int i = 0; i < arrayKey.Length; i++)
				{
					OCIChar oCIChar = Studio.GetCtrlInfo(arrayKey[i]) as OCIChar;
					if (oCIChar != null)
					{
						if (kind == 0)
						{
							oCIChar.animeOptionParam1 = oldvalue[i];
						}
						else
						{
							oCIChar.animeOptionParam2 = oldvalue[i];
						}
					}
				}
			}
		}
	}
}
