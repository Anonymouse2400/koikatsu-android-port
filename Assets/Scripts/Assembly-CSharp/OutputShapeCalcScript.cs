using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class OutputShapeCalcScript : MonoBehaviour
{
	public class Info
	{
		public List<string> lstPosX = new List<string>();

		public List<string> lstPosY = new List<string>();

		public List<string> lstPosZ = new List<string>();

		public List<string> lstRotX = new List<string>();

		public List<string> lstRotY = new List<string>();

		public List<string> lstRotZ = new List<string>();

		public List<string> lstSclX = new List<string>();

		public List<string> lstSclY = new List<string>();

		public List<string> lstSclZ = new List<string>();
	}

	public TextAsset text;

	private Dictionary<string, Info> dictBone = new Dictionary<string, Info>();

	private List<string> lstSrc = new List<string>();

	private void Start()
	{
		dictBone.Clear();
		if (!(null != text))
		{
			return;
		}
		string[,] data = null;
		YS_Assist.GetListString(text.text, out data);
		Info value = null;
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		if (length != 0 && length2 != 0)
		{
			for (int i = 0; i < length; i++)
			{
				if (!dictBone.TryGetValue(data[i, 0], out value))
				{
					value = new Info();
					dictBone[data[i, 0]] = value;
				}
				if ("v" == data[i, 2])
				{
					value.lstPosX.Add(data[i, 1]);
				}
				if ("v" == data[i, 3])
				{
					value.lstPosY.Add(data[i, 1]);
				}
				if ("v" == data[i, 4])
				{
					value.lstPosZ.Add(data[i, 1]);
				}
				if ("v" == data[i, 5])
				{
					value.lstRotX.Add(data[i, 1]);
				}
				if ("v" == data[i, 6])
				{
					value.lstRotY.Add(data[i, 1]);
				}
				if ("v" == data[i, 7])
				{
					value.lstRotZ.Add(data[i, 1]);
				}
				if ("v" == data[i, 8])
				{
					value.lstSclX.Add(data[i, 1]);
				}
				if ("v" == data[i, 9])
				{
					value.lstSclY.Add(data[i, 1]);
				}
				if ("v" == data[i, 10])
				{
					value.lstSclZ.Add(data[i, 1]);
				}
				if (!lstSrc.Contains(data[i, 1]))
				{
					lstSrc.Add(data[i, 1]);
				}
			}
		}
		string outputPath = Application.dataPath + "/shapecalc.txt";
		OutputText(outputPath);
	}

	public void OutputText(string outputPath)
	{
		StringBuilder stringBuilder = new StringBuilder(2048);
		stringBuilder.Length = 0;
		stringBuilder.Append("=== 計算式 ===================================================================\n");
		foreach (KeyValuePair<string, Info> item in dictBone)
		{
			if (item.Value.lstPosX.Count != 0)
			{
				stringBuilder.Append("dictDstBoneInfo[(int)DstBoneName.").Append(item.Key).Append("].trfBone.SetLocalPositionX(");
				for (int i = 0; i < item.Value.lstPosX.Count; i++)
				{
					stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstPosX[i]).Append("].vctPos.x");
					if (i + 1 < item.Value.lstPosX.Count)
					{
						stringBuilder.Append(" + ");
					}
					else
					{
						stringBuilder.Append(");\n");
					}
				}
			}
			if (item.Value.lstPosY.Count != 0)
			{
				stringBuilder.Append("dictDstBoneInfo[(int)DstBoneName.").Append(item.Key).Append("].trfBone.SetLocalPositionY(");
				for (int j = 0; j < item.Value.lstPosY.Count; j++)
				{
					stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstPosY[j]).Append("].vctPos.y");
					if (j + 1 < item.Value.lstPosY.Count)
					{
						stringBuilder.Append(" + ");
					}
					else
					{
						stringBuilder.Append(");\n");
					}
				}
			}
			if (item.Value.lstPosZ.Count != 0)
			{
				stringBuilder.Append("dictDstBoneInfo[(int)DstBoneName.").Append(item.Key).Append("].trfBone.SetLocalPositionZ(");
				for (int k = 0; k < item.Value.lstPosZ.Count; k++)
				{
					stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstPosZ[k]).Append("].vctPos.z");
					if (k + 1 < item.Value.lstPosZ.Count)
					{
						stringBuilder.Append(" + ");
					}
					else
					{
						stringBuilder.Append(");\n");
					}
				}
			}
			if (item.Value.lstRotX.Count != 0 || item.Value.lstRotY.Count != 0 || item.Value.lstRotZ.Count != 0)
			{
				stringBuilder.Append("dictDstBoneInfo[(int)DstBoneName.").Append(item.Key).Append("].trfBone.SetLocalRotation(\n");
				stringBuilder.Append("\t");
				if (item.Value.lstRotX.Count != 0)
				{
					for (int l = 0; l < item.Value.lstRotX.Count; l++)
					{
						stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstRotX[l]).Append("].vctRot.x");
						if (l + 1 < item.Value.lstRotX.Count)
						{
							stringBuilder.Append(" + ");
						}
						else
						{
							stringBuilder.Append(",\n");
						}
					}
				}
				else
				{
					stringBuilder.Append("0.0f,\n");
				}
				stringBuilder.Append("\t");
				if (item.Value.lstRotY.Count != 0)
				{
					for (int m = 0; m < item.Value.lstRotY.Count; m++)
					{
						stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstRotY[m]).Append("].vctRot.y");
						if (m + 1 < item.Value.lstRotY.Count)
						{
							stringBuilder.Append(" + ");
						}
						else
						{
							stringBuilder.Append(",\n");
						}
					}
				}
				else
				{
					stringBuilder.Append("0.0f,\n");
				}
				stringBuilder.Append("\t");
				if (item.Value.lstRotZ.Count != 0)
				{
					for (int n = 0; n < item.Value.lstRotZ.Count; n++)
					{
						stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstRotZ[n]).Append("].vctRot.z");
						if (n + 1 < item.Value.lstRotZ.Count)
						{
							stringBuilder.Append(" + ");
						}
						else
						{
							stringBuilder.Append(");\n");
						}
					}
				}
				else
				{
					stringBuilder.Append("0.0f);\n");
				}
			}
			if (item.Value.lstSclX.Count != 0 || item.Value.lstSclY.Count != 0 || item.Value.lstSclZ.Count != 0)
			{
				stringBuilder.Append("dictDstBoneInfo[(int)DstBoneName.").Append(item.Key).Append("].trfBone.SetLocalScale(\n");
				stringBuilder.Append("\t");
				if (item.Value.lstSclX.Count != 0)
				{
					for (int num = 0; num < item.Value.lstSclX.Count; num++)
					{
						stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstSclX[num]).Append("].vctScl.x");
						if (num + 1 < item.Value.lstSclX.Count)
						{
							stringBuilder.Append(" + ");
						}
						else
						{
							stringBuilder.Append(",\n");
						}
					}
				}
				else
				{
					stringBuilder.Append("1.0f,\n");
				}
				stringBuilder.Append("\t");
				if (item.Value.lstSclY.Count != 0)
				{
					for (int num2 = 0; num2 < item.Value.lstSclY.Count; num2++)
					{
						stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstSclY[num2]).Append("].vctScl.y");
						if (num2 + 1 < item.Value.lstSclY.Count)
						{
							stringBuilder.Append(" + ");
						}
						else
						{
							stringBuilder.Append(",\n");
						}
					}
				}
				else
				{
					stringBuilder.Append("1.0f,\n");
				}
				stringBuilder.Append("\t");
				if (item.Value.lstSclZ.Count != 0)
				{
					for (int num3 = 0; num3 < item.Value.lstSclZ.Count; num3++)
					{
						stringBuilder.Append("dictSrcBoneInfo[(int)SrcBoneName.").Append(item.Value.lstSclZ[num3]).Append("].vctScl.z");
						if (num3 + 1 < item.Value.lstSclZ.Count)
						{
							stringBuilder.Append(" + ");
						}
						else
						{
							stringBuilder.Append(");\n");
						}
					}
				}
				else
				{
					stringBuilder.Append("1.0f);\n");
				}
			}
			stringBuilder.Append("\n");
		}
		for (int num4 = 0; num4 < 10; num4++)
		{
			stringBuilder.Append("\n");
		}
		stringBuilder.Append("=== 転送先 ===================================================================\n");
		stringBuilder.Append("public enum DstBoneName\n").Append("{\n");
		int num5 = 0;
		foreach (KeyValuePair<string, Info> item2 in dictBone)
		{
			stringBuilder.Append("\t").Append(item2.Key).Append(",");
			num5++;
			if (num5 == 4)
			{
				stringBuilder.Append("\n");
				num5 = 0;
			}
			else
			{
				stringBuilder.Append("\t\t\t");
			}
		}
		stringBuilder.Append("\n};");
		for (int num6 = 0; num6 < 10; num6++)
		{
			stringBuilder.Append("\n");
		}
		stringBuilder.Append("=== 転送元 ===================================================================\n");
		stringBuilder.Append("public enum SrcBoneName\n").Append("{\n");
		num5 = 0;
		for (int num7 = 0; num7 < lstSrc.Count; num7++)
		{
			stringBuilder.Append("\t").Append(lstSrc[num7]).Append(",");
			num5++;
			if (num5 == 4)
			{
				stringBuilder.Append("\n");
				num5 = 0;
			}
			else
			{
				stringBuilder.Append("\t\t\t");
			}
		}
		stringBuilder.Append("\n};");
		using (FileStream stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
			{
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Write("\n");
			}
		}
	}

	private void OnGUI()
	{
		GUI.color = Color.white;
		GUILayout.BeginArea(new Rect(10f, 10f, 400f, 20f));
		GUILayout.Label("シェイプ計算スクリプト補助データ作成終了");
		GUILayout.EndArea();
	}

	private void Update()
	{
	}
}
