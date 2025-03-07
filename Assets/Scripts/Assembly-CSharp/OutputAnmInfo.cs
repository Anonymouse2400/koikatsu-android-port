using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OutputAnmInfo : MonoBehaviour
{
	public int msgCnt;

	public GameObject objAnm;

	public int start;

	public int end;

	public string outputFile = string.Empty;

	public bool outputDebugText;

	[Header("--------<ExtraOption>---------------------------------------------------------------")]
	public bool UseInfoFlag;

	public TextAsset taUseInfo;

	private List<string> lstUseName = new List<string>();

	private string[] arrUseName;

	private string msg = "アニメキー情報作成終了";

	private void Start()
	{
		if (string.Empty == outputFile)
		{
			return;
		}
		if (UseInfoFlag)
		{
			LoadUseNameList();
			arrUseName = lstUseName.ToArray();
		}
		AnimationKeyInfo animationKeyInfo = new AnimationKeyInfo();
		if (animationKeyInfo.CreateInfo(start, end, objAnm, arrUseName))
		{
			string text = Application.dataPath + "/_CustomShapeOutput";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = text + "/" + outputFile + ".bytes";
			animationKeyInfo.SaveInfo(text2);
			if (outputDebugText)
			{
				string outputPath = text2.Replace(".bytes", ".txt");
				animationKeyInfo.OutputText(outputPath);
			}
		}
		else
		{
			msg = outputFile + " の作成に失敗";
		}
	}

	public void LoadUseNameList()
	{
		if (!(null == taUseInfo))
		{
			string[,] data;
			YS_Assist.GetListString(taUseInfo.text, out data);
			lstUseName.Clear();
			int length = data.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				string item = data[i, 0].Replace("\r", string.Empty).Replace("\n", string.Empty);
				lstUseName.Add(item);
			}
		}
	}

	private void OnGUI()
	{
		GUI.color = Color.white;
		GUILayout.BeginArea(new Rect(10f, 10 + msgCnt * 25, 400f, 20f));
		GUILayout.Label(msg);
		GUILayout.EndArea();
	}

	private void Update()
	{
	}
}
