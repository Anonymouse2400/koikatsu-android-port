using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class LipFormInfo : MonoBehaviour
{
	public enum FormType
	{
		A = 0,
		I = 1,
		U = 2,
		E = 3,
		O = 4,
		S = 5
	}

	[Serializable]
	public class TimeLineInfo
	{
		public float time;

		public int form;

		public string msg = string.Empty;
	}

	public List<TimeLineInfo> lstTimeLineInfo = new List<TimeLineInfo>();

	public void ChangeInfoFromText(string fullPath)
	{
		string text = string.Empty;
		using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
			{
				text = streamReader.ReadToEnd();
			}
		}
		string[,] data;
		GetListString(text, out data);
		lstTimeLineInfo.Clear();
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		if (length == 0 || length2 == 0)
		{
			return;
		}
		for (int i = 0; i < length; i++)
		{
			TimeLineInfo timeLineInfo = new TimeLineInfo();
			timeLineInfo.time = float.Parse(data[i, 0]);
			timeLineInfo.msg = data[i, 1];
			foreach (var item in Enum.GetNames(typeof(FormType)).Select((string val, int idx) => new { val, idx }))
			{
				if (item.val == data[i, 2])
				{
					timeLineInfo.form = item.idx;
				}
			}
			lstTimeLineInfo.Add(timeLineInfo);
		}
	}

	public static void GetListString(string text, out string[,] data)
	{
		string[] array = text.Split('\n');
		int num = array.Length;
		if (num != 0 && array[num - 1].Trim() == string.Empty)
		{
			num--;
		}
		string[] array2 = array[0].Split('\t');
		int num2 = array2.Length;
		if (num2 != 0 && array2[num2 - 1].Trim() == string.Empty)
		{
			num2--;
		}
		data = new string[num, num2];
		for (int i = 0; i < num; i++)
		{
			string[] array3 = array[i].Split('\t');
			for (int j = 0; j < array3.Length && j < num2; j++)
			{
				data[i, j] = array3[j];
			}
			data[i, array3.Length - 1] = data[i, array3.Length - 1].Replace("\r", string.Empty).Replace("\n", string.Empty);
		}
	}
}
