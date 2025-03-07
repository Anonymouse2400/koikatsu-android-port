using System;
using System.Collections.Generic;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubjectiveChangeSystem : MonoBehaviour
{
	public enum Mode
	{
		none = 0,
		subjecitve = 1
	}

	[Serializable]
	public class ChangeParam
	{
		public string name;

		[Label("移動制限をするか")]
		public bool isLimitPos;

		[Label("移動制限した時の制限距離(Baseからの距離)")]
		public float limitPos = 2f;

		[Label("ズーム制限をするか")]
		public bool isLimitDir;

		[Label("ズーム制限した時の制限距離(注視点からの距離)")]
		public float limitDir = 10f;

		[Label("パースの最大値")]
		public float limitFov = 40f;

		[Label("回転制限をするか")]
		public bool isLlimitRot;

		[Label("回転制限した時の制限X軸角度(+-この角度で制限)")]
		public float limitRotX = 360f;

		[Label("回転制限した時の制限Y軸角度(+-この角度で制限)")]
		public float limitRotY = 360f;
	}

	[Serializable]
	public class PosRot
	{
		public Vector3 pos;

		public Quaternion rot;
	}

	public CameraControl_Ver2 ctrlCamera;

	public ChangeParam[] subjectives = new ChangeParam[2];

	public Mode mode;

	[RangeIntLabel("どこまで戻る", 0, 9)]
	public int lerpOldIndex;

	[RangeLabel("補間比率", 0f, 1f)]
	public float lerp = 1f;

	[SerializeField]
	private ChaControl male;

	[SerializeField]
	private GameObject objSubjective;

	private bool update;

	private bool config;

	private bool play;

	private Vector3 posLocalBaseOld = Vector3.zero;

	private Vector3 rotLocalBaseOld = Vector3.zero;

	private Vector3 posOld = Vector3.zero;

	private Vector3 rotOld = Vector3.zero;

	private Vector3 dirOld = Vector3.zero;

	private float fovOld = 40f;

	private List<PosRot> pss = new List<PosRot>();

	public bool Config
	{
		get
		{
			return config;
		}
		set
		{
			config = value;
			update = true;
		}
	}

	public bool Play
	{
		get
		{
			return play;
		}
		set
		{
			play = value;
			update = true;
		}
	}

	public void SetFemale(ChaControl _female)
	{
	}

	public void SetMale(ChaControl _male)
	{
		male = _male;
	}

	public void SetSubjectiveObject(GameObject _obj)
	{
		objSubjective = _obj;
	}

	private void LateUpdate()
	{
		if (update)
		{
			if ((play || config) && objSubjective != null)
			{
				posLocalBaseOld = ctrlCamera.transBase.localPosition;
				rotLocalBaseOld = ctrlCamera.transBase.localRotation.eulerAngles;
				ctrlCamera.transBase.SetParent(objSubjective.transform);
				ctrlCamera.SetLocalBase(Vector3.zero, Vector3.zero);
				posOld = ctrlCamera.TargetPos;
				rotOld = ctrlCamera.CameraAngle;
				dirOld = ctrlCamera.CameraDir;
				fovOld = ctrlCamera.CameraFov;
				ctrlCamera.TargetPos = Vector3.zero;
				ctrlCamera.Rot = Vector3.zero;
				ctrlCamera.CameraDir = Vector3.zero;
				ctrlCamera.CameraFov = 45f;
				SetLimit(subjectives[1]);
				mode = Mode.subjecitve;
				if (male != null)
				{
					male.chaFile.status.visibleHeadAlways = false;
				}
				pss.Clear();
				for (int i = 0; i < 10; i++)
				{
					pss.Add(new PosRot
					{
						pos = objSubjective.transform.position,
						rot = objSubjective.transform.rotation
					});
				}
			}
			else
			{
				ctrlCamera.transBase.SetParent(null);
				ctrlCamera.SetLocalBase(posLocalBaseOld, rotLocalBaseOld);
				ctrlCamera.TargetPos = posOld;
				ctrlCamera.CameraAngle = rotOld;
				ctrlCamera.CameraDir = dirOld;
				ctrlCamera.CameraFov = fovOld;
				SetLimit(subjectives[0]);
				mode = Mode.none;
				if (male != null)
				{
					male.chaFile.status.visibleHeadAlways = true;
				}
				Scene sceneByName = SceneManager.GetSceneByName("HTest");
				sceneByName = ((!(sceneByName.name == "HTest")) ? SceneManager.GetSceneByName("HProc") : sceneByName);
				SceneManager.MoveGameObjectToScene(ctrlCamera.transBase.gameObject, sceneByName);
			}
			update = false;
		}
		if (mode == Mode.subjecitve && objSubjective != null)
		{
			Vector3 position = objSubjective.transform.position;
			Quaternion rotation = objSubjective.transform.rotation;
			Vector3 pos = Vector3.Lerp(position, pss[lerpOldIndex].pos, lerp);
			Vector3 eulerAngles = rotation.eulerAngles;
			ctrlCamera.SetWorldBase(pos, eulerAngles);
			pss.Insert(0, new PosRot
			{
				pos = position,
				rot = rotation
			});
			pss.RemoveAt(pss.Count - 1);
		}
	}

	private void SetLimit(ChangeParam _param)
	{
		ctrlCamera.isLimitPos = _param.isLimitPos;
		ctrlCamera.limitPos = _param.limitPos;
		ctrlCamera.isLimitDir = _param.isLimitDir;
		ctrlCamera.limitDir = _param.limitDir;
		ctrlCamera.limitFov = _param.limitFov;
		ctrlCamera.isLlimitRot = _param.isLlimitRot;
		ctrlCamera.limitRotX = _param.limitRotX;
		ctrlCamera.limitRotY = _param.limitRotY;
	}
}
