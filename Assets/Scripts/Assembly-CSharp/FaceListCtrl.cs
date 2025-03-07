using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;

public class FaceListCtrl : SerializedMonoBehaviour
{
	[Serializable]
	public class FaceInfo
	{
		public int id;

		public string name = string.Empty;

		public int eyebrow;

		public int eye;

		public float openMaxEye = 1f;

		public int mouth;

		public float openMinMouth;

		public int tears;

		public float cheek;

		public bool highlight = true;

		public bool eyesblink = true;

		public bool yure;

		public FaceInfo()
		{
			MemberInit();
		}

		public FaceInfo(FaceInfo _copy)
		{
			id = _copy.id;
			name = _copy.name;
			eyebrow = _copy.eyebrow;
			eye = _copy.eye;
			openMaxEye = _copy.openMaxEye;
			mouth = _copy.mouth;
			openMinMouth = _copy.openMinMouth;
			tears = _copy.tears;
			cheek = _copy.cheek;
			highlight = _copy.highlight;
			eyesblink = _copy.eyesblink;
			yure = _copy.yure;
		}

		public void Copy(FaceInfo _copy)
		{
			id = _copy.id;
			name = _copy.name;
			eyebrow = _copy.eyebrow;
			eye = _copy.eye;
			openMaxEye = _copy.openMaxEye;
			mouth = _copy.mouth;
			openMinMouth = _copy.openMinMouth;
			tears = _copy.tears;
			cheek = _copy.cheek;
			highlight = _copy.highlight;
			eyesblink = _copy.eyesblink;
			yure = _copy.yure;
		}

		public void MemberInit()
		{
			id = 0;
			name = string.Empty;
			eyebrow = 0;
			eye = 0;
			openMaxEye = 1f;
			mouth = 0;
			openMinMouth = 0f;
			tears = 0;
			cheek = 0f;
			highlight = true;
			eyesblink = true;
			yure = false;
		}
	}

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
	public List<List<Dictionary<int, FaceInfo>>> facelib = new List<List<Dictionary<int, FaceInfo>>>();

	private GlobalMethod.FloatBlend blendEye = new GlobalMethod.FloatBlend();

	private GlobalMethod.FloatBlend blendMouth = new GlobalMethod.FloatBlend();

	private void Start()
	{
		for (int i = 0; i < 3; i++)
		{
			facelib.Add(new List<Dictionary<int, FaceInfo>>());
		}
		for (int j = 0; j < 8; j++)
		{
			facelib[0].Add(new Dictionary<int, FaceInfo>());
		}
		for (int k = 0; k < 9; k++)
		{
			facelib[1].Add(new Dictionary<int, FaceInfo>());
		}
		facelib[2].Add(new Dictionary<int, FaceInfo>());
	}

	public bool Release()
	{
		for (int i = 0; i < facelib[0].Count; i++)
		{
			facelib[0][i].Clear();
		}
		for (int j = 0; j < facelib[1].Count; j++)
		{
			facelib[1][j].Clear();
		}
		facelib[2][0].Clear();
		return true;
	}

	public bool LoadText(string _chaNo)
	{
		Release();
		LoadInfo(_chaNo, 0, 0, facelib[0][0]);
		LoadInfo(_chaNo, 0, 1, facelib[0][1]);
		LoadInfo(_chaNo, 0, 2, facelib[0][2]);
		LoadInfo(_chaNo, 0, 3, facelib[0][3]);
		LoadInfo(_chaNo, 0, 4, facelib[0][4]);
		LoadInfo(_chaNo, 0, 5, facelib[0][5]);
		LoadInfo(_chaNo, 0, 6, facelib[0][6]);
		LoadInfo(_chaNo, 0, 7, facelib[0][7]);
		LoadInfo(_chaNo, 1, 0, facelib[1][0]);
		LoadInfo(_chaNo, 1, 1, facelib[1][1]);
		LoadInfo(_chaNo, 1, 2, facelib[1][2]);
		LoadInfo(_chaNo, 1, 3, facelib[1][3]);
		LoadInfo(_chaNo, 1, 4, facelib[1][4]);
		LoadInfo(_chaNo, 1, 5, facelib[1][5]);
		LoadInfo(_chaNo, 1, 6, facelib[1][6]);
		LoadInfo(_chaNo, 1, 7, facelib[1][7]);
		LoadInfo(_chaNo, 1, 8, facelib[1][8]);
		LoadInfo(_chaNo, 2, 0, facelib[2][0]);
		return true;
	}

	public bool LoadInfo(string _chaNo, int _voiceKind, int _action, Dictionary<int, FaceInfo> _dic)
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "HFaceList_" + _chaNo + "_" + _voiceKind.ToString("00") + "_" + _action.ToString("00"));
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int num2 = int.Parse(data[i, num++]);
			if (!_dic.ContainsKey(num2))
			{
				_dic.Add(num2, new FaceInfo());
			}
			FaceInfo faceInfo = _dic[num2];
			faceInfo.id = num2;
			faceInfo.name = data[i, num++];
			faceInfo.eyebrow = int.Parse(data[i, num++]);
			faceInfo.eye = int.Parse(data[i, num++]);
			faceInfo.openMaxEye = float.Parse(data[i, num++]);
			faceInfo.mouth = int.Parse(data[i, num++]);
			faceInfo.openMinMouth = float.Parse(data[i, num++]);
			faceInfo.tears = int.Parse(data[i, num++]);
			faceInfo.cheek = float.Parse(data[i, num++]);
			faceInfo.highlight = data[i, num++] == "1";
			faceInfo.eyesblink = data[i, num++] == "1";
			faceInfo.yure = data[i, num++] == "1";
		}
		return true;
	}

	public bool SetFace(int _idFace, ChaControl _chara, int _voiceKind, int _action)
	{
		if (_chara == null)
		{
			return false;
		}
		if (facelib.Count <= _voiceKind || _voiceKind < 0)
		{
			return false;
		}
		if (facelib[_voiceKind].Count <= _action || _action < 0)
		{
			return false;
		}
		if (!facelib[_voiceKind][_action].ContainsKey(_idFace))
		{
			return false;
		}
		FaceInfo info = facelib[_voiceKind][_action][_idFace];
		_chara.ChangeEyebrowPtn(info.eyebrow);
		_chara.ChangeEyesPtn(info.eye);
		_chara.eyesCtrl.SafeProc(delegate(FBSCtrlEyes e)
		{
			blendEye.Start(e.OpenMax, info.openMaxEye, 0.3f);
		});
		_chara.ChangeMouthPtn(info.mouth);
		_chara.mouthCtrl.SafeProc(delegate(FBSCtrlMouth m)
		{
			blendMouth.Start(m.OpenMin, info.openMinMouth, 0.3f);
		});
		_chara.tearsLv = (byte)info.tears;
		_chara.ChangeHohoAkaRate(info.cheek);
		_chara.HideEyeHighlight(!info.highlight);
		_chara.ChangeEyesBlinkFlag(info.eyesblink);
		_chara.ChangeEyesShaking(info.yure);
		_chara.DisableShapeMouth(MathfEx.IsRange(21, info.mouth, 22, true));
		return true;
	}

	public bool OpenCtrl(ChaControl _female)
	{
		if (!_female)
		{
			return false;
		}
		float _ans = 0f;
		if (blendEye.Proc(ref _ans))
		{
			_female.ChangeEyesOpenMax(_ans);
		}
		_female.mouthCtrl.SafeProc(delegate(FBSCtrlMouth m)
		{
			float _ans2 = 0f;
			if (blendMouth.Proc(ref _ans2))
			{
				m.OpenMin = _ans2;
			}
		});
		return true;
	}

	private void SaveText(StreamWriter _writer, int _voiceKind, int _action)
	{
		foreach (KeyValuePair<int, FaceInfo> item in facelib[_voiceKind][_action])
		{
			_writer.Write(item.Value.id + "\t");
			_writer.Write(item.Value.name + "\t");
			_writer.Write(item.Value.eyebrow + "\t");
			_writer.Write(item.Value.eye + "\t");
			_writer.Write(item.Value.openMaxEye + "\t");
			_writer.Write(item.Value.mouth + "\t");
			_writer.Write(item.Value.openMinMouth + "\t");
			_writer.Write(item.Value.tears + "\t");
			_writer.Write(item.Value.cheek + "\t");
			_writer.Write((!item.Value.highlight) ? "0\t" : "1");
			_writer.Write((!item.Value.eyesblink) ? "0\t" : "1");
			_writer.Write((!item.Value.yure) ? "0\t" : "1");
			_writer.Write("\n");
		}
	}
}
