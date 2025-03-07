using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using Illusion.CustomAttributes;
using UnityEngine;

public class OutputShapeCalcScriptEx : MonoBehaviour
{
	[SerializeField]
	private TextAsset text;

	private List<string> lstDst = new List<string>();

	private List<string> lstSrc = new List<string>();

	private List<string> lstScript = new List<string>();

	private Dictionary<string, string[]> dictCalcStr = new Dictionary<string, string[]>();

	[Button("CreateShapeScript", "シェイプスクリプト作成", new object[] { })]
	[Header("siriの計算は手動で調整が必要")]
	public int createCSS;

	public void CreateShapeScript()
	{
		lstDst.Clear();
		lstSrc.Clear();
		using (MemoryStream stream = new MemoryStream(text.bytes))
		{
			using (StreamReader streamReader = new StreamReader(stream))
			{
				while (streamReader.Peek() > -1)
				{
					GetSimpleProcess(streamReader);
				}
			}
		}
		OutputText();
	}

	public string ConvertCalcTypeStr(string src)
	{
		switch (src)
		{
		case "translateX":
			return "vctPos.x";
		case "translateY":
			return "vctPos.y";
		case "translateZ":
			return "vctPos.z";
		case "rotateX":
			return "vctRot.x";
		case "rotateY":
			return "vctRot.y";
		case "rotateZ":
			return "vctRot.z";
		case "scaleX":
			return "vctScl.x";
		case "scaleY":
			return "vctScl.y";
		case "scaleZ":
			return "vctScl.z";
		default:
			return string.Empty;
		}
	}

	public bool GetSimpleProcess(StreamReader sr)
	{
		bool flag = true;
		string text = string.Empty;
		int num = 0;
		string text3;
		do
		{
			string text2 = sr.ReadLine();
			text3 = string.Empty;
			num = text2.IndexOf("//");
			switch (num)
			{
			case 0:
				if (flag)
				{
					return false;
				}
				break;
			default:
				text3 = text2.Substring(0, num);
				break;
			case -1:
				text3 = text2;
				break;
			}
			if (string.Empty != text3)
			{
				text += text3;
			}
		}
		while (!text3.Contains(";") && sr.Peek() > -1);
		text = text.Replace(" ", string.Empty);
		text = text.Replace("\u3000", string.Empty);
		text = text.Replace("\t", string.Empty);
		num = text.IndexOf("=");
		string text4 = text.Substring(0, num);
		string text5 = text.Substring(num + 1).Replace(";", string.Empty);
		string[] array = text4.Split('.');
		string[] value = null;
		if (!dictCalcStr.TryGetValue(array[0], out value))
		{
			value = new string[9];
			value[0] = (value[1] = (value[2] = "0.0f"));
			value[3] = (value[4] = (value[5] = "0.0f"));
			value[6] = (value[7] = (value[8] = "1.0f"));
			dictCalcStr[array[0]] = value;
			lstDst.Add(array[0]);
		}
		int num2 = 0;
		switch (array[1])
		{
		case "translateX":
			num2 = 0;
			break;
		case "translateY":
			num2 = 1;
			break;
		case "translateZ":
			num2 = 2;
			break;
		case "rotateX":
			num2 = 3;
			break;
		case "rotateY":
			num2 = 4;
			break;
		case "rotateZ":
			num2 = 5;
			break;
		case "scaleX":
			num2 = 6;
			break;
		case "scaleY":
			num2 = 7;
			break;
		case "scaleZ":
			num2 = 8;
			break;
		}
		List<string> list = new List<string>();
		string[] array2 = text5.Split('+', '-', '*', '/', '(', ')');
		int num3 = array2.Length;
		if (array2[num3 - 1] == string.Empty)
		{
			num3--;
		}
		string text6 = text5;
		for (int i = 0; i < num3; i++)
		{
			if (string.Empty != array2[i])
			{
				double result = 0.0;
				string text7 = array2[i];
				if (!double.TryParse(text7, out result))
				{
					array = text7.Split('.');
					text7 = "dictSrc[(int)SrcName." + array[0] + "]." + ConvertCalcTypeStr(array[1]);
					if (!lstSrc.Contains(array[0]))
					{
						lstSrc.Add(array[0]);
					}
				}
				list.Add(text7);
			}
			text6 = text6.Substring(array2[i].Length);
			if (string.Empty == text6)
			{
				break;
			}
			list.Add(text6.Substring(0, 1));
			text6 = text6.Substring(1);
		}
		for (int j = 0; j < list.Count; j++)
		{
			double result2 = 0.0;
			if (!double.TryParse(list[j], out result2))
			{
				continue;
			}
			bool flag2 = true;
			if (0 < j)
			{
				if ("*" == list[j - 1] || "/" == list[j - 1])
				{
					flag2 = false;
				}
				else if ("-" == list[j - 1] && 1 < j && ("*" == list[j - 2] || "/" == list[j - 2]))
				{
					flag2 = false;
				}
			}
			if (j < list.Count - 1 && ("*" == list[j + 1] || "/" == list[j + 1]))
			{
				flag2 = false;
			}
			if (flag2)
			{
				list[j] = ((float)(result2 * 0.10000000149011612)/*cast due to .constrained prefix*/).ToString();
			}
			list[j] += "f";
		}
		if (list.Count != 0)
		{
			value[num2] = string.Empty;
		}
        string[] array3 = null;
        int num4 = 0;
        for (int k = 0; k < list.Count; k++)
		{
			
			(array3 = value)[num4 = num2] = array3[num4] + list[k];
		}
		return true;
	}

	public void OutputText()
	{
		lstScript.Clear();
		lstScript.Add("//=== ターゲット =============================================================\n");
		lstScript.Add("public enum DstName\n");
		lstScript.Add("{\n");
		for (int i = 0; i < lstDst.Count; i++)
		{
			lstScript.Add("\t" + lstDst[i] + ",\n");
		}
		lstScript.Add("};\n\n");
		lstScript.Add("//=== 参照 ===================================================================\n");
		lstScript.Add("public enum SrcName\n");
		lstScript.Add("{\n");
		for (int j = 0; j < lstSrc.Count; j++)
		{
			lstScript.Add("\t" + lstSrc[j] + ",\n");
		}
		lstScript.Add("};\n\n");
		lstScript.Add("//=== 計算式 =================================================================\n");
		foreach (KeyValuePair<string, string[]> item in dictCalcStr)
		{
			string empty = string.Empty;
			if (string.Empty != item.Value[0] && "0.0f" != item.Value[0])
			{
				empty = "dictDst[(int)DstName." + item.Key + "].trfBone.SetLocalPositionX(" + item.Value[0] + ");\n";
				lstScript.Add(empty);
			}
			if (string.Empty != item.Value[1] && "0.0f" != item.Value[1])
			{
				empty = "dictDst[(int)DstName." + item.Key + "].trfBone.SetLocalPositionY(" + item.Value[1] + ");\n";
				lstScript.Add(empty);
			}
			if (string.Empty != item.Value[2] && "0.0f" != item.Value[2])
			{
				empty = "dictDst[(int)DstName." + item.Key + "].trfBone.SetLocalPositionZ(" + item.Value[2] + ");\n";
				lstScript.Add(empty);
			}
			if (!("0.0f" == item.Value[3]) || !("0.0f" == item.Value[4]) || !("0.0f" == item.Value[5]))
			{
				empty = "dictDst[(int)DstName." + item.Key + "].trfBone.SetLocalRotation(\n";
				empty = empty + "\t" + item.Value[3] + ",\n";
				empty = empty + "\t" + item.Value[4] + ",\n";
				empty = empty + "\t" + item.Value[5] + ");\n";
				lstScript.Add(empty);
			}
			if (!("1.0f" == item.Value[6]) || !("1.0f" == item.Value[7]) || !("1.0f" == item.Value[8]))
			{
				empty = "dictDst[(int)DstName." + item.Key + "].trfBone.SetLocalScale(\n";
				empty = empty + "\t" + item.Value[6] + ",\n";
				empty = empty + "\t" + item.Value[7] + ",\n";
				empty = empty + "\t" + item.Value[8] + ");\n";
				lstScript.Add(empty);
			}
			lstScript.Add("\n");
		}
		lstScript.Add("\n");
		lstScript.Add("\n");
		string path = Application.dataPath + "/shapecalc.txt";
		using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
			{
				for (int k = 0; k < lstScript.Count; k++)
				{
					streamWriter.Write(lstScript[k]);
				}
				streamWriter.Write("\n\n\n");
			}
		}
	}
}
