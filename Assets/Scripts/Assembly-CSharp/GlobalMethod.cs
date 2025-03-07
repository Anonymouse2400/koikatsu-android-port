using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FBSAssist;
using H;
using Illusion.Extensions;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;

public class GlobalMethod
{
	public class RatioRand
	{
		public enum RandError
		{
			Error_None = -999999
		}

		protected class CCheck
		{
			public int no;

			public float minVal;

			public float maxVal;
		}

		private float allVal;

		private List<CCheck> checks = new List<CCheck>();

		public RatioRand()
		{
			allVal = 0f;
		}

		public bool Add(int no, float ratio)
		{
			if (ratio == 0f)
			{
				DebugLog("ランダム 追加登録個数が0");
				return false;
			}
			if (checks.Exists((CCheck i) => i.no == no))
			{
				DebugLog("ランダム 重複登録");
				return false;
			}
			CCheck cCheck = new CCheck();
			cCheck.no = no;
			cCheck.minVal = allVal;
			cCheck.maxVal = allVal + ratio;
			allVal += ratio;
			checks.Add(cCheck);
			return true;
		}

		public int Random()
		{
			if (IsEmpty())
			{
				return -999999;
			}
			float randVal = UnityEngine.Random.Range(0f, allVal);
			CCheck cCheck = checks.Find((CCheck x) => randVal >= x.minVal && randVal <= x.maxVal);
			if (cCheck == null)
			{
				return -999999;
			}
			return cCheck.no;
		}

		public bool IsEmpty()
		{
			return checks.Count == 0;
		}

		public void Clear()
		{
			allVal = 0f;
			checks.Clear();
		}
	}

	public class ShuffleRandIndex
	{
		private int[] no;

		private int cnt;

		public void Init(List<int> _list)
		{
			no = _list.ToArray();
			Shuffle();
		}

		private void Shuffle()
		{
			if (no.Length != 0)
			{
				int[] array = new int[no.Length];
				for (int j = 0; j < no.Length; j++)
				{
					array[j] = j;
				}
				no = array.OrderBy((int i) => Guid.NewGuid()).ToArray();
				cnt = 0;
			}
		}

		public int Get(out int _get)
		{
			_get = -1;
			if (no.Length == 0)
			{
				return -1;
			}
			if (cnt >= no.Length)
			{
				Shuffle();
				_get = no[cnt++];
				return 1;
			}
			_get = no[cnt++];
			return 0;
		}

		public int Get()
		{
			if (no.Length == 0)
			{
				return 0;
			}
			if (cnt >= no.Length)
			{
				Shuffle();
			}
			return no[cnt++];
		}
	}

	public class ShuffleRand
	{
		private int[] no;

		private int cnt;

		public void Init(List<int> _list)
		{
			no = _list.ToArray();
			Shuffle();
		}

		private void Shuffle()
		{
			if (no.Length != 0)
			{
				int[] array = new int[no.Length];
				for (int j = 0; j < no.Length; j++)
				{
					array[j] = no[j];
				}
				no = array.OrderBy((int i) => Guid.NewGuid()).ToArray();
				cnt = 0;
			}
		}

		public int Get()
		{
			if (no.Length == 0)
			{
				return -999999;
			}
			if (cnt >= no.Length)
			{
				Shuffle();
			}
			return no[cnt++];
		}
	}

	public class RandomTimer
	{
		private float randMin = 1000f;

		private float randMax = 1000f;

		private float next;

		private float cnt;

		public void Init(float min, float max)
		{
			randMin = min;
			randMax = max;
			next = UnityEngine.Random.Range(randMin, randMax);
		}

		public void InitTime()
		{
			next = UnityEngine.Random.Range(randMin, randMax);
			cnt = 0f;
		}

		public bool Check()
		{
			cnt += Time.deltaTime;
			if (cnt >= next)
			{
				InitTime();
				return true;
			}
			return false;
		}
	}

	public class FloatBlend
	{
		private bool blend;

		private float min;

		private float max;

		private TimeProgressCtrl tpc = new TimeProgressCtrl();

		public bool Start(float _min, float _max, float _timeBlend = 0.15f)
		{
			tpc.SetProgressTime(_timeBlend);
			tpc.Start();
			min = _min;
			max = _max;
			blend = true;
			return true;
		}

		public bool Proc(ref float _ans)
		{
			if (!blend)
			{
				return false;
			}
			float num = tpc.Calculate();
			_ans = Mathf.Lerp(min, max, num);
			if (num >= 1f)
			{
				blend = false;
			}
			return true;
		}

		public bool IsBlend()
		{
			return blend;
		}
	}

	public class HPointAppointNullDatail
	{
		public HFlag.EMode mode = HFlag.EMode.none;

		public int idMap;

		public int kindGet;

		public string nameGetNull = string.Empty;

		public Vector3 pos = Vector3.zero;

		public List<int> lstCategory = new List<int>();
	}

	public class HPointAppointNullGetData
	{
		public Vector3 pos = Vector3.zero;

		public Quaternion rot = Quaternion.identity;
	}

	public static void SetCameraMoveFlag(CameraControl_Ver2 _ctrl, bool _bPlay)
	{
		if (!(_ctrl == null))
		{
			_ctrl.NoCtrlCondition = () => !_bPlay;
		}
	}

	public static bool IsCameraMoveFlag(CameraControl_Ver2 _ctrl)
	{
		if (_ctrl == null)
		{
			return false;
		}
		BaseCameraControl_Ver2.NoCtrlFunc noCtrlCondition = _ctrl.NoCtrlCondition;
		bool flag = true;
		if (noCtrlCondition != null)
		{
			flag = noCtrlCondition();
		}
		return !flag;
	}

	public static bool IsCameraActionFlag(CameraControl_Ver2 _ctrl)
	{
		if (_ctrl == null)
		{
			return false;
		}
		return _ctrl.isControlNow;
	}

	public static void SetCameraBase(CameraControl_Ver2 _ctrl, Transform _transTarget)
	{
		if (!(_ctrl == null))
		{
			_ctrl.transBase.localPosition = _transTarget.localPosition;
			_ctrl.transBase.localRotation = _transTarget.localRotation;
			_ctrl.transBase.position = _transTarget.position;
			_ctrl.transBase.rotation = _transTarget.rotation;
		}
	}

	public static void CameraKeyCtrl(CameraControl_Ver2 _ctrl, ChaControl[] _Females)
	{
		if (_Females == null || _ctrl == null)
		{
			return;
		}
		if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				GameObject gameObject = _Females[0].objBodyBone.transform.FindLoop("cf_j_head");
				if (!(gameObject == null))
				{
					_ctrl.TargetPos = _ctrl.transBase.InverseTransformPoint(gameObject.transform.position);
				}
			}
			else if (Input.GetKeyDown(KeyCode.W))
			{
				GameObject gameObject2 = _Females[0].objBodyBone.transform.FindLoop("cf_d_bust00");
				if (!(gameObject2 == null))
				{
					_ctrl.TargetPos = _ctrl.transBase.InverseTransformPoint(gameObject2.transform.position);
				}
			}
			else if (Input.GetKeyDown(KeyCode.E))
			{
				GameObject gameObject3 = _Females[0].objBodyBone.transform.FindLoop("cf_j_kokan");
				if (!(gameObject3 == null))
				{
					_ctrl.TargetPos = _ctrl.transBase.InverseTransformPoint(gameObject3.transform.position);
				}
			}
		}
		else
		{
			if (_Females.Length <= 1)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.Q))
			{
				GameObject gameObject4 = _Females[1].objBodyBone.transform.FindLoop("cf_j_head");
				if (!(gameObject4 == null))
				{
					_ctrl.TargetPos = _ctrl.transBase.InverseTransformPoint(gameObject4.transform.position);
				}
			}
			else if (Input.GetKeyDown(KeyCode.W))
			{
				GameObject gameObject5 = _Females[1].objBodyBone.transform.FindLoop("cf_d_bust00");
				if (!(gameObject5 == null))
				{
					_ctrl.TargetPos = _ctrl.transBase.InverseTransformPoint(gameObject5.transform.position);
				}
			}
			else if (Input.GetKeyDown(KeyCode.E))
			{
				GameObject gameObject6 = _Females[1].objBodyBone.transform.FindLoop("cf_j_kokan");
				if (!(gameObject6 == null))
				{
					_ctrl.TargetPos = _ctrl.transBase.InverseTransformPoint(gameObject6.transform.position);
				}
			}
		}
	}

	public static void SaveCamera(CameraControl_Ver2 _ctrl, string _strAssetPath, string _strfile)
	{
		if (!(_ctrl == null))
		{
			_ctrl.CameraDataSave(_strAssetPath, _strfile);
		}
	}

	public static void LoadCamera(CameraControl_Ver2 _ctrl, string _assetbundleFolder, string _strfile, bool _isDirect = false)
	{
		if (!(_ctrl == null))
		{
			_ctrl.CameraDataLoad(_assetbundleFolder, _strfile, _isDirect);
		}
	}

	public static void LoadResetCamera(CameraControl_Ver2 _ctrl, string _assetbundleFolder, string _strfile, bool _isDirect = false)
	{
		if (!(_ctrl == null))
		{
			_ctrl.CameraResetDataLoad(_assetbundleFolder, _strfile, _isDirect);
		}
	}

	public static void HGlobalSaveData(int _mode, int _id)
	{
		Dictionary<int, HashSet<int>> playHList = Singleton<Game>.Instance.glSaveData.playHList;
		HashSet<int> value;
		if (!playHList.TryGetValue(_mode, out value))
		{
			playHList[_mode] = new HashSet<int>();
		}
		playHList[_mode].Add(_id);
	}

	public static void DebugLog(string _str, int _state = 0)
	{
	}

	public static void SetAllClothState(ChaControl _female, bool _isUpper, int _state, bool _isForce = false)
	{
		if (_female == null)
		{
			return;
		}
		if (_state < 0)
		{
			_state = 0;
		}
		List<int> list = new List<int>();
		if (_isUpper)
		{
			list.Add(0);
			list.Add(2);
			if (_female.IsClothesStateKind(0) && (_female.chaFile.status.clothesState[0] < _state || _isForce))
			{
				_female.SetClothesState(0, (byte)_state);
			}
		}
		else
		{
			list.Add(1);
			list.Add(3);
			list.Add(5);
		}
		foreach (int item in list)
		{
			if (_female.chaFile.status.clothesState[item] < _state || _isForce)
			{
				_female.SetClothesState(item, (byte)_state);
			}
		}
	}

	public static int Loop(int _now, int _max)
	{
		return (_max > 1) ? ((_now % _max + _max) % _max) : 0;
	}

	public static int LoopArea(int _now, int _min, int _max)
	{
		return Loop(_now - _min, _max - _min) + _min;
	}

	public static void GetListString(string text, out string[,] data)
	{
		string[] array = text.Split('\n');
		int num = array.Length;
		if (num != 0 && array[num - 1].Trim() == string.Empty)
		{
			num--;
		}
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			string[] array2 = array[i].Split('\t');
			num2 = Mathf.Max(num2, array2.Length);
		}
		data = new string[num, num2];
		for (int j = 0; j < num; j++)
		{
			string[] array3 = array[j].Split('\t');
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k] = array3[k].Replace("\r", string.Empty).Replace("\n", string.Empty);
				if (k >= num2)
				{
					break;
				}
				data[j, k] = array3[k];
			}
		}
	}

	public static int GetIntTryParse(string _text, int _init = 0)
	{
		int result = 0;
		if (int.TryParse(_text, out result))
		{
			return result;
		}
		return _init;
	}

	public static float LerpBeziereAppoint(Vector2 _vS, Vector2 _vE, Vector2 _vH, float _ft)
	{
		Vector2 vector = default(Vector2);
		vector.x = (4f * _vH.x - _vS.x - _vE.x) / 4f;
		vector.y = (4f * _vH.y - _vS.y - _vE.y) / 4f;
		float num = 1f - _ft;
		float num2 = Mathf.Pow(num, 2f);
		float num3 = 2f * num * _ft;
		float num4 = Mathf.Pow(_ft, 2f);
		Vector2 vector2 = default(Vector2);
		vector2.x = num2 * _vS.x + num3 * vector.x + num4 * _vE.x;
		vector2.y = num2 * _vS.y + num3 * vector.y + num4 * _vE.y;
		return vector2.y;
	}

	public static float LerpBeziere(float _fS, float _fE, Vector2 _vH, float _ft)
	{
		Vector2 vector = default(Vector2);
		vector.x = (4f * _vH.x - 1f) / 4f;
		vector.y = (4f * _vH.y - _fS - _fE) / 4f;
		Vector2 vector2 = new Vector2(0f, _fS);
		Vector2 vector3 = new Vector2(1f, _fE);
		float num = 1f - _ft;
		float num2 = Mathf.Pow(num, 2f);
		float num3 = 2f * num * _ft;
		float num4 = Mathf.Pow(_ft, 2f);
		Vector2 vector4 = default(Vector2);
		vector4.x = num2 * vector2.x + num3 * vector.x + num4 * vector3.x;
		vector4.y = num2 * vector2.y + num3 * vector.y + num4 * vector3.y;
		return vector4.y;
	}

	public static string LoadAllListText(string _assetbundleFolder, string _strLoadFile, List<string> _OmitFolderName = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(_assetbundleFolder);
		assetBundleNameListFromPath.Sort();
		for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetBundleNameListFromPath[i]);
			fileNameWithoutExtension = YS_Assist.GetStringRight(fileNameWithoutExtension, 2);
			if (_OmitFolderName != null && _OmitFolderName.Contains(fileNameWithoutExtension))
			{
				continue;
			}
			string[] allAssetName = AssetBundleCheck.GetAllAssetName(assetBundleNameListFromPath[i], false);
			bool flag = false;
			for (int j = 0; j < allAssetName.Length; j++)
			{
				if (allAssetName[j].Compare(_strLoadFile, true))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				DebugLog("[" + assetBundleNameListFromPath[i] + "][" + _strLoadFile + "]は見つかりません", 1);
			}
			else
			{
				TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleNameListFromPath[i], _strLoadFile, false, string.Empty);
				AssetBundleManager.UnloadAssetBundle(assetBundleNameListFromPath[i], true);
				if (!(textAsset == null))
				{
					stringBuilder.Append(textAsset.text);
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static List<string> LoadAllListTextFromList(string _assetbundleFolder, string _strLoadFile, List<string> _OmitFolderName = null)
	{
		List<string> list = new List<string>();
		List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(_assetbundleFolder);
		assetBundleNameListFromPath.Sort();
		for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetBundleNameListFromPath[i]);
			fileNameWithoutExtension = YS_Assist.GetStringRight(fileNameWithoutExtension, 2);
			if (_OmitFolderName != null && _OmitFolderName.Contains(fileNameWithoutExtension))
			{
				continue;
			}
			string[] allAssetName = AssetBundleCheck.GetAllAssetName(assetBundleNameListFromPath[i], false);
			bool flag = false;
			for (int j = 0; j < allAssetName.Length; j++)
			{
				if (allAssetName[j].Compare(_strLoadFile, true))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				DebugLog("[" + assetBundleNameListFromPath[i] + "][" + _strLoadFile + "]は見つかりません", 1);
			}
			else
			{
				TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleNameListFromPath[i], _strLoadFile, false, string.Empty);
				AssetBundleManager.UnloadAssetBundle(assetBundleNameListFromPath[i], true);
				if (!(textAsset == null))
				{
					list.Add(textAsset.text);
				}
			}
		}
		return list;
	}

	public static List<ExcelData.Param> LoadExcelData(string _strAssetPath, string _strFileName, int sCell, int sRow, int eCell, int eRow, bool _isWarning = true)
	{
		if (!AssetBundleCheck.IsFile(_strAssetPath, _strFileName))
		{
			if (_isWarning)
			{
				DebugLog("excel : [" + _strAssetPath + "][" + _strFileName + "]は見つかりません", 1);
			}
			return null;
		}
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(_strAssetPath, _strFileName, typeof(ExcelData));
		AssetBundleManager.UnloadAssetBundle(_strAssetPath, true);
		if (assetBundleLoadAssetOperation.IsEmpty())
		{
			if (_isWarning)
			{
				DebugLog("excel : [" + _strFileName + "]は[" + _strAssetPath + "]の中に入っていません", 1);
			}
			return null;
		}
		ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
		List<string> cell = asset.GetCell(sCell);
		List<string> row = asset.GetRow(sRow);
		return asset.Get(new ExcelData.Specify(eCell, eRow), new ExcelData.Specify(cell.Count, row.Count));
	}

	public static List<ExcelData.Param> LoadExcelDataAlFindlFile(string _strAssetPath, string _strFileName, int sCell, int sRow, int eCell, int eRow, List<string> _OmitFolderName = null, bool _isWarning = true)
	{
		List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(_strAssetPath);
		assetBundleNameListFromPath.Sort();
		for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetBundleNameListFromPath[i]);
			fileNameWithoutExtension = YS_Assist.GetStringRight(fileNameWithoutExtension, 2);
			if (_OmitFolderName != null && _OmitFolderName.Contains(fileNameWithoutExtension))
			{
				continue;
			}
			string[] allAssetName = AssetBundleCheck.GetAllAssetName(assetBundleNameListFromPath[i], false);
			bool flag = false;
			for (int j = 0; j < allAssetName.Length; j++)
			{
				if (allAssetName[j].Compare(_strFileName, true))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				DebugLog("[" + assetBundleNameListFromPath[i] + "][" + _strFileName + "]は見つかりません", 1);
			}
			else
			{
				List<ExcelData.Param> list = LoadExcelData(assetBundleNameListFromPath[i], _strFileName, sCell, sRow, eCell, eRow, _isWarning);
				if (list != null)
				{
					return list;
				}
			}
		}
		return null;
	}

	public static T LoadAllFolderInOneFile<T>(string _findFolder, string _strLoadFile, List<string> _OmitFolderName = null) where T : UnityEngine.Object
	{
		List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(_findFolder);
		assetBundleNameListFromPath.Sort();
		for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetBundleNameListFromPath[i]);
			fileNameWithoutExtension = YS_Assist.GetStringRight(fileNameWithoutExtension, 2);
			if (_OmitFolderName != null && _OmitFolderName.Contains(fileNameWithoutExtension))
			{
				continue;
			}
			string[] allAssetName = AssetBundleCheck.GetAllAssetName(assetBundleNameListFromPath[i], false);
			bool flag = false;
			for (int j = 0; j < allAssetName.Length; j++)
			{
				if (allAssetName[j].Compare(_strLoadFile, true))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				DebugLog("[" + assetBundleNameListFromPath[i] + "][" + _strLoadFile + "]は見つかりません", 1);
				continue;
			}
			T result = CommonLib.LoadAsset<T>(assetBundleNameListFromPath[i], _strLoadFile, false, string.Empty);
			AssetBundleManager.UnloadAssetBundle(assetBundleNameListFromPath[i], true);
			return result;
		}
		return (T)null;
	}

	public static List<T> LoadAllFolder<T>(string _findFolder, string _strLoadFile, List<string> _OmitFolderName = null, HashSet<string> _assetbundle = null) where T : UnityEngine.Object
	{
		List<T> list = new List<T>();
		List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(_findFolder);
		assetBundleNameListFromPath.Sort();
		for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetBundleNameListFromPath[i]);
			fileNameWithoutExtension = YS_Assist.GetStringRight(fileNameWithoutExtension, 2);
			if (_OmitFolderName != null && _OmitFolderName.Contains(fileNameWithoutExtension))
			{
				continue;
			}
			string[] allAssetName = AssetBundleCheck.GetAllAssetName(assetBundleNameListFromPath[i], false, null, true);
			bool flag = false;
			for (int j = 0; j < allAssetName.Length; j++)
			{
				if (allAssetName[j].Compare(_strLoadFile, true))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				DebugLog("[" + assetBundleNameListFromPath[i] + "][" + _strLoadFile + "]は見つかりません", 1);
				continue;
			}
			T val = CommonLib.LoadAsset<T>(assetBundleNameListFromPath[i], _strLoadFile, false, string.Empty);
			if (_assetbundle != null)
			{
				_assetbundle.Add(assetBundleNameListFromPath[i]);
			}
			else
			{
				AssetBundleManager.UnloadAssetBundle(assetBundleNameListFromPath[i], true);
			}
			if ((bool)val)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public static bool CheckFlagsArray(bool[] flags, int _check = 0)
	{
		if (flags.Length == 0)
		{
			return false;
		}
		bool flag = _check == 0;
		foreach (bool flag2 in flags)
		{
			if ((_check != 0) ? flag2 : (!flag2))
			{
				return !flag;
			}
		}
		return flag;
	}

	public static void PlaySelectSE()
	{
		if (!Utils.Sound.Get(SystemSE.ok_s).isPlaying)
		{
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public static void ForcePlaySelectSE()
	{
		Utils.Sound.Play(SystemSE.sel);
	}

	public static void PlayDecisionSE()
	{
		Utils.Sound.Play(SystemSE.ok_s);
	}

	public static void PlayCancelSE()
	{
		Utils.Sound.Play(SystemSE.cancel);
	}

	public static HPointAppointNullGetData GetHPointAppointNull(HPointAppointNullDatail _detail)
	{
		HPointAppointNullGetData hPointAppointNullGetData = new HPointAppointNullGetData();
		StringBuilder stringBuilder = new StringBuilder("HPoint_");
		if (_detail.mode == HFlag.EMode.masturbation || _detail.mode == HFlag.EMode.lesbian)
		{
			stringBuilder.Append("Add_");
		}
		else if (_detail.mode == HFlag.EMode.houshi3P || _detail.mode == HFlag.EMode.sonyu3P)
		{
			stringBuilder.Append("3P_");
		}
		List<GameObject> list = LoadAllFolder<GameObject>("h/common/", stringBuilder.ToString() + _detail.idMap);
		if (list.Count == 0)
		{
			return hPointAppointNullGetData;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(list[list.Count - 1]);
		if (_detail.kindGet == 0)
		{
			GameObject gameObject2 = gameObject.transform.FindLoop(_detail.nameGetNull);
			if ((bool)gameObject2)
			{
				hPointAppointNullGetData.pos = gameObject2.transform.position;
				hPointAppointNullGetData.rot = gameObject2.transform.rotation;
			}
			UnityEngine.Object.Destroy(gameObject);
			return hPointAppointNullGetData;
		}
		HPointData[] componentsInChildren = gameObject.GetComponentsInChildren<HPointData>(true);
		HPointOmitObject omit = gameObject.GetComponent<HPointOmitObject>();
		IEnumerable<HPointData> source = componentsInChildren.Where((HPointData d) => !omit.list.Contains(d.gameObject));
		if (_detail.kindGet == 2)
		{
			source = source.Where((HPointData d) => d.category.Intersect(_detail.lstCategory).Any());
		}
		HPointData hPointData = source.OrderBy((HPointData d) => (_detail.pos - d.transform.position).sqrMagnitude).FirstOrDefault();
		if ((bool)hPointData)
		{
			hPointAppointNullGetData.pos = hPointData.transform.position;
			hPointAppointNullGetData.rot = hPointData.transform.rotation;
		}
		UnityEngine.Object.Destroy(gameObject);
		return hPointAppointNullGetData;
	}
}
