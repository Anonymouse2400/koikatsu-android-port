using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;

public class CameraControl_Ver2 : BaseCameraControl_Ver2
{
	public class VisibleObject
	{
		public string nameCollider;

		public float delay;

		public bool isVisible = true;

		public List<MeshRenderer> listRender = new List<MeshRenderer>();
	}

	public bool isOutsideTargetTex;

	public bool isCursorLock;

	private bool isConfigVanish = true;

	private Renderer targetRender;

	public bool isFlashVisible;

	private List<VisibleObject> lstMapVanish = new List<VisibleObject>();

	private List<Collider> listCollider = new List<Collider>();

	public bool isConfigTargetTex { get; set; }

	public bool ConfigVanish
	{
		get
		{
			return isConfigVanish;
		}
		set
		{
			if (isConfigVanish != value)
			{
				isConfigVanish = value;
				VisibleFroceVanish(true);
			}
		}
	}

	public Transform targetTex { get; private set; }

	protected new void Start()
	{
		base.Start();
		targetTex = base.transform.Find("CameraTarget");
		if ((bool)targetTex)
		{
			targetTex.localScale = Vector3.one * 0.01f;
			targetRender = targetTex.GetComponent<Renderer>();
		}
		isOutsideTargetTex = true;
		isConfigTargetTex = true;
		isConfigVanish = true;
		isCursorLock = true;
		viewCollider = base.gameObject.AddComponent<CapsuleCollider>();
		viewCollider.radius = 0.05f;
		viewCollider.isTrigger = true;
		viewCollider.direction = 2;
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		isInit = true;
		listCollider.Clear();
	}

	protected new void LateUpdate()
	{
		if (Singleton<Scene>.Instance.NowSceneNames.Any((string s) => s == "Config" || s == "Check"))
		{
			return;
		}
		if (!Singleton<Scene>.Instance.IsNowLoading && !Singleton<Scene>.Instance.IsNowLoadingFade)
		{
			base.LateUpdate();
		}
		if ((bool)targetTex)
		{
			if (transBase != null)
			{
				targetTex.position = transBase.TransformPoint(CamDat.Pos);
			}
			else
			{
				targetTex.position = CamDat.Pos;
			}
			Vector3 position = base.transform.position;
			position.y = targetTex.position.y;
			targetTex.transform.LookAt(position);
			targetTex.Rotate(new Vector3(90f, 0f, 0f));
			if ((bool)targetRender)
			{
				targetRender.enabled = base.isControlNow & isOutsideTargetTex & isConfigTargetTex;
			}
			if (Singleton<GameCursor>.IsInstance() && isCursorLock)
			{
				Singleton<GameCursor>.Instance.SetCursorLock(base.isControlNow & isOutsideTargetTex);
			}
		}
		VanishProc();
	}

	private void OnDisable()
	{
		VisibleFroceVanish(true);
	}

	protected void OnTriggerEnter(Collider other)
	{
		Collider collider = listCollider.Find((Collider x) => other.name == x.name);
		if (collider == null)
		{
			listCollider.Add(other);
		}
	}

	protected void OnTriggerStay(Collider other)
	{
		Collider collider = listCollider.Find((Collider x) => other.name == x.name);
		if (collider == null)
		{
			listCollider.Add(other);
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		listCollider.Clear();
	}

	public void CleraVanishCollider()
	{
		listCollider.Clear();
	}

	public void autoCamera(float _fSpeed)
	{
		CamDat.Rot.y = (CamDat.Rot.y + _fSpeed * Time.deltaTime) % 360f;
	}

	public void CameraDataSave(string _strCreateAssetPath, string _strFile)
	{
		FileData fileData = new FileData(string.Empty);
		string path = fileData.Create(_strCreateAssetPath) + _strFile + ".txt";
		using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
		{
			streamWriter.Write(CamDat.Pos.x);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Pos.y);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Pos.z);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Dir.x);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Dir.y);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Dir.z);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Rot.x);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Rot.y);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Rot.z);
			streamWriter.Write('\n');
			streamWriter.Write(CamDat.Fov);
			streamWriter.Write('\n');
		}
	}

	public void CameraDataSaveBinary(BinaryWriter bw)
	{
		bw.Write(CamDat.Pos.x);
		bw.Write(CamDat.Pos.y);
		bw.Write(CamDat.Pos.z);
		bw.Write(CamDat.Dir.x);
		bw.Write(CamDat.Dir.y);
		bw.Write(CamDat.Dir.z);
		bw.Write(CamDat.Rot.x);
		bw.Write(CamDat.Rot.y);
		bw.Write(CamDat.Rot.z);
		bw.Write(CamDat.Fov);
	}

	public bool CameraDataLoad(string _assetbundleFolder, string _strFile, bool _isDirect = false)
	{
		string text = string.Empty;
		if (!_isDirect)
		{
			text = GlobalMethod.LoadAllListText(_assetbundleFolder, _strFile);
		}
		else
		{
			TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(_assetbundleFolder, _strFile, false, string.Empty);
			AssetBundleManager.UnloadAssetBundle(_assetbundleFolder, true);
			if ((bool)textAsset)
			{
				text = textAsset.text;
			}
		}
		if (text == string.Empty)
		{
			GlobalMethod.DebugLog("cameraファイル読み込めません", 1);
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		CamDat.Pos.x = float.Parse(data[0, 0]);
		CamDat.Pos.y = float.Parse(data[1, 0]);
		CamDat.Pos.z = float.Parse(data[2, 0]);
		CamDat.Dir.x = float.Parse(data[3, 0]);
		CamDat.Dir.y = float.Parse(data[4, 0]);
		CamDat.Dir.z = float.Parse(data[5, 0]);
		CamDat.Rot.x = float.Parse(data[6, 0]);
		CamDat.Rot.y = float.Parse(data[7, 0]);
		CamDat.Rot.z = float.Parse(data[8, 0]);
		CamDat.Fov = float.Parse(data[9, 0]);
		if (base.thisCmaera != null)
		{
			base.thisCmaera.fieldOfView = CamDat.Fov;
		}
		CamReset.Copy(CamDat, Quaternion.identity);
		CameraUpdate();
		if (!isInit)
		{
			isInit = true;
		}
		return true;
	}

	public bool CameraResetDataLoad(string _assetbundleFolder, string _strFile, bool _isDirect = false)
	{
		string text = string.Empty;
		if (!_isDirect)
		{
			text = GlobalMethod.LoadAllListText(_assetbundleFolder, _strFile);
		}
		else
		{
			TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(_assetbundleFolder, _strFile, false, string.Empty);
			AssetBundleManager.UnloadAssetBundle(_assetbundleFolder, true);
			if ((bool)textAsset)
			{
				text = textAsset.text;
			}
		}
		if (text == string.Empty)
		{
			GlobalMethod.DebugLog("cameraファイル読み込めません", 1);
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		CameraData copy = default(CameraData);
		copy.Pos.x = float.Parse(data[0, 0]);
		copy.Pos.y = float.Parse(data[1, 0]);
		copy.Pos.z = float.Parse(data[2, 0]);
		copy.Dir.x = float.Parse(data[3, 0]);
		copy.Dir.y = float.Parse(data[4, 0]);
		copy.Dir.z = float.Parse(data[5, 0]);
		copy.Rot.x = float.Parse(data[6, 0]);
		copy.Rot.y = float.Parse(data[7, 0]);
		copy.Rot.z = float.Parse(data[8, 0]);
		copy.Fov = float.Parse(data[9, 0]);
		CamReset.Copy(copy, Quaternion.identity);
		return true;
	}

	public bool CameraDataLoadBinary(BinaryReader br, bool isUpdate)
	{
		CameraData copy = default(CameraData);
		copy.Pos.x = br.ReadSingle();
		copy.Pos.y = br.ReadSingle();
		copy.Pos.z = br.ReadSingle();
		copy.Dir.x = br.ReadSingle();
		copy.Dir.y = br.ReadSingle();
		copy.Dir.z = br.ReadSingle();
		copy.Rot.x = br.ReadSingle();
		copy.Rot.y = br.ReadSingle();
		copy.Rot.z = br.ReadSingle();
		copy.Fov = br.ReadSingle();
		CamReset.Copy(copy, Quaternion.identity);
		if (isUpdate)
		{
			CamDat.Copy(copy);
			if (base.thisCmaera != null)
			{
				base.thisCmaera.fieldOfView = copy.Fov;
			}
			CameraUpdate();
			if (!isInit)
			{
				isInit = true;
			}
		}
		return true;
	}

	public bool LoadVanish(string _assetbundleFolder, string _strMap, GameObject _objMap)
	{
		lstMapVanish.Clear();
		if (_objMap == null)
		{
			return false;
		}
		string text = GlobalMethod.LoadAllListText(_assetbundleFolder, _strMap);
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			VisibleObject visibleObject = new VisibleObject();
			visibleObject.nameCollider = data[i, 0];
			for (int j = 1; j < length2; j++)
			{
				string text2 = data[i, j];
				if (text2 == string.Empty)
				{
					break;
				}
				GameObject gameObject = _objMap.transform.FindLoop(text2);
				if (!(gameObject == null))
				{
					MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>(true);
					visibleObject.listRender.AddRange(componentsInChildren);
				}
			}
			lstMapVanish.Add(visibleObject);
		}
		return true;
	}

	public void VisibleFroceVanish(bool _visible)
	{
		foreach (VisibleObject item in lstMapVanish)
		{
			foreach (MeshRenderer item2 in item.listRender)
			{
				if ((bool)item2)
				{
					item2.enabled = _visible;
				}
			}
			item.isVisible = _visible;
			item.delay = ((!_visible) ? 0f : 0.3f);
		}
	}

	private void VisibleFroceVanish(VisibleObject _obj, bool _visible)
	{
		if (_obj != null && _obj.listRender != null)
		{
			for (int i = 0; i < _obj.listRender.Count; i++)
			{
				_obj.listRender[i].enabled = _visible;
			}
			_obj.delay = ((!_visible) ? 0f : 0.3f);
			_obj.isVisible = _visible;
		}
	}

	private bool VanishProc()
	{
		if (!isConfigVanish)
		{
			return false;
		}
		int i;
		for (i = 0; i < lstMapVanish.Count; i++)
		{
			Collider collider = listCollider.Find((Collider x) => lstMapVanish[i].nameCollider == x.name);
			if (collider == null)
			{
				VanishDelayVisible(lstMapVanish[i]);
			}
			else if (lstMapVanish[i].isVisible)
			{
				VisibleFroceVanish(lstMapVanish[i], false);
			}
		}
		return true;
	}

	private bool VanishDelayVisible(VisibleObject _visible)
	{
		if (_visible.isVisible)
		{
			return false;
		}
		if (!isFlashVisible)
		{
			_visible.delay += Time.deltaTime;
			if (_visible.delay >= 0.3f)
			{
				VisibleFroceVanish(_visible, true);
			}
		}
		else
		{
			VisibleFroceVanish(_visible, true);
		}
		return true;
	}
}
