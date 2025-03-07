using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChaCustom
{
	public class CustomHistory : Singleton<CustomHistory>
	{
		public class Data
		{
			public byte[] custom;

			public byte[] coordinate;

			public byte[] parameter;

			public bool flag1;

			public bool flag2;

			public bool flag3;

			public bool flag4;

			public string hashStr = string.Empty;

			public event Func<bool> func;

			public event Func<bool, bool> func2;

			public event Func<bool, bool, bool> func3;

			public event Func<bool, bool, bool, bool> func4;

			public event Func<bool, bool, bool, bool, bool> func5;

			public Data(ChaControl cha)
			{
				Set(cha);
				this.func = null;
				this.func2 = null;
				flag1 = false;
				flag2 = false;
				flag3 = false;
				flag4 = false;
			}

			public void Set(ChaControl cha)
			{
				if (null != cha && cha.chaFile != null)
				{
					custom = cha.chaFile.GetCustomBytes();
					coordinate = cha.chaFile.GetCoordinateBytes();
					parameter = cha.chaFile.GetParameterBytes();
					byte[] array = new byte[custom.Length + coordinate.Length + parameter.Length];
					Array.Copy(custom, array, custom.Length);
					Array.Copy(coordinate, 0, array, custom.Length, coordinate.Length);
					Array.Copy(parameter, 0, array, custom.Length + coordinate.Length, parameter.Length);
					byte[] data = YS_Assist.CreateSha256(array, "History");
					hashStr = YS_Assist.ConvertStrX2FromBytes(data);
				}
			}

			public void Restore(ChaControl cha)
			{
				cha.chaFile.SetCustomBytes(custom, ChaFileDefine.ChaFileCustomVersion);
				cha.chaFile.SetCoordinateBytes(coordinate, ChaFileDefine.ChaFileCoordinateVersion);
				cha.chaFile.SetParameterBytes(parameter);
				cha.ChangeCoordinateType();
			}

			public bool IsSameHash(ChaControl cha)
			{
				Data data = new Data(cha);
				return IsSameHash(data);
			}

			public bool IsSameHash(Data data)
			{
				if (data.hashStr == hashStr)
				{
					return true;
				}
				return false;
			}

			public bool Call()
			{
				if (this.func != null)
				{
					return this.func();
				}
				if (this.func2 != null)
				{
					return this.func2(flag1);
				}
				if (this.func3 != null)
				{
					return this.func3(flag1, flag2);
				}
				if (this.func4 != null)
				{
					return this.func4(flag1, flag2, flag3);
				}
				if (this.func5 != null)
				{
					return this.func5(flag1, flag2, flag3, flag4);
				}
				return false;
			}
		}

		[SerializeField]
		private int maxUndo = 5;

		private int undoCnt;

		private List<Data> lstHistory = new List<Data>();

		private Data openData;

		protected override void Awake()
		{
			openData = null;
			Clear();
		}

		public void Clear()
		{
			undoCnt = 0;
			lstHistory.Clear();
		}

		public void SetOpenData(ChaControl cha)
		{
			openData = new Data(cha);
		}

		public void SetOpenData(Data data)
		{
			openData = data;
		}

		public Data GetOpenData()
		{
			return openData;
		}

		public Data GetNowData()
		{
			int num = lstHistory.Count - undoCnt - 1;
			if (num == -1)
			{
				return null;
			}
			return lstHistory[num];
		}

		public bool IsUndo()
		{
			if (lstHistory.Count - undoCnt == 1)
			{
				return false;
			}
			return true;
		}

		public Data Undo(ChaControl cha)
		{
			int num = lstHistory.Count - undoCnt - 1;
			if (IsUndo())
			{
				undoCnt++;
				num--;
				lstHistory[num].Restore(cha);
				int index = num + 1;
				lstHistory[index].Call();
			}
			return lstHistory[num];
		}

		public bool IsRedo()
		{
			if (undoCnt == 0)
			{
				return false;
			}
			return true;
		}

		public Data Redo(ChaControl cha)
		{
			int num = lstHistory.Count - undoCnt - 1;
			if (IsRedo())
			{
				undoCnt--;
				num++;
				lstHistory[num].Restore(cha);
				lstHistory[num].Call();
			}
			return lstHistory[num];
		}

		public void Add1(ChaControl cha, Func<bool> _func)
		{
			Data data = new Data(cha);
			if (IsChange(data))
			{
				data.func += _func;
				Add(data);
			}
		}

		public void Add2(ChaControl cha, Func<bool, bool> _func, bool _flag1)
		{
			Data data = new Data(cha);
			if (IsChange(data))
			{
				data.func2 += _func;
				data.flag1 = _flag1;
				Add(data);
			}
		}

		public void Add3(ChaControl cha, Func<bool, bool, bool> _func, bool _flag1, bool _flag2)
		{
			Data data = new Data(cha);
			if (IsChange(data))
			{
				data.func3 += _func;
				data.flag1 = _flag1;
				data.flag2 = _flag2;
				Add(data);
			}
		}

		public void Add4(ChaControl cha, Func<bool, bool, bool, bool> _func, bool _flag1, bool _flag2, bool _flag3)
		{
			Data data = new Data(cha);
			if (IsChange(data))
			{
				data.func4 += _func;
				data.flag1 = _flag1;
				data.flag2 = _flag2;
				data.flag3 = _flag3;
				Add(data);
			}
		}

		public void Add5(ChaControl cha, Func<bool, bool, bool, bool, bool> _func, bool _flag1, bool _flag2, bool _flag3, bool _flag4)
		{
			Data data = new Data(cha);
			if (IsChange(data))
			{
				data.func5 += _func;
				data.flag1 = _flag1;
				data.flag2 = _flag2;
				data.flag3 = _flag3;
				data.flag4 = _flag4;
				Add(data);
			}
		}

		public void Add(Data data)
		{
			int count = lstHistory.Count;
			if (undoCnt == 0)
			{
				if (maxUndo <= count)
				{
					lstHistory.RemoveAt(0);
				}
			}
			else
			{
				lstHistory.RemoveRange(count - undoCnt, undoCnt);
			}
			lstHistory.Add(data);
			undoCnt = 0;
		}

		public bool IsChange(ChaControl cha)
		{
			Data nowData = GetNowData();
			if (nowData == null)
			{
				return true;
			}
			return !nowData.IsSameHash(cha);
		}

		public bool IsChange(Data newData)
		{
			Data nowData = GetNowData();
			if (nowData == null)
			{
				return true;
			}
			return !nowData.IsSameHash(newData);
		}
	}
}
