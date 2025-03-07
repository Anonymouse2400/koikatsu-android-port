using System.Collections.Generic;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;

public class MetaballCtrl
{
	public class ShootInfo
	{
		public bool isInside;

		public string nameAnimation;

		public float timeShoot;

		public float timeOld;
	}

	public class ShootCtrl
	{
		public List<ShootInfo> lstShoot = new List<ShootInfo>();

		public bool IsAnimation(bool _isInside, AnimatorStateInfo _stateInfo)
		{
			for (int i = 0; i < lstShoot.Count; i++)
			{
				ShootInfo shootInfo = lstShoot[i];
				if (!_stateInfo.IsName(shootInfo.nameAnimation))
				{
					shootInfo.timeOld = 0f;
				}
				else if (!shootInfo.isInside || _isInside)
				{
					if (!(shootInfo.timeOld > shootInfo.timeShoot) && !(shootInfo.timeShoot >= _stateInfo.normalizedTime))
					{
						shootInfo.timeOld = _stateInfo.normalizedTime;
						return true;
					}
					shootInfo.timeOld = _stateInfo.normalizedTime;
				}
			}
			return false;
		}
	}

	public class MetaballParameterInfo
	{
		public Vector2 speedS;

		public Vector2 speedM;

		public Vector2 speedL;

		public Vector2 rotationS;

		public Vector2 rotationM;

		public Vector2 rotationL;

		public GameObject objShoot;

		public GameObject objParent;

		public GameObject objCondom;
	}

	private const int cMetaBallNum = 8;

	public MetaballShoot[] ctrlMetaball = new MetaballShoot[8];

	private UpdateMeta[] ctrlUpdate;

	private ShootCtrl[] actrlShoot = new ShootCtrl[8];

	private GameObject[] aobjBody = new GameObject[4];

	private MetaballParameterInfo[] aParam = new MetaballParameterInfo[8];

	private HParticleCtrl particle;

	public MetaballCtrl(GameObject _objBase, GameObject _objMale, ChaControl _female)
	{
		if ((bool)_objBase)
		{
			ctrlUpdate = _objBase.GetComponentsInChildren<UpdateMeta>();
			UpdateMeta[] array = ctrlUpdate;
			foreach (UpdateMeta updateMeta in array)
			{
				foreach (MetaballShoot item in updateMeta.lstShoot)
				{
					if (item.comment.Contains("外出しアナル"))
					{
						ctrlMetaball[4] = item;
						ctrlMetaball[4].chaCustom = _female;
					}
					else if (item.comment.Contains("中出し"))
					{
						ctrlMetaball[0] = item;
						ctrlMetaball[0].chaCustom = _female;
					}
					else if (item.comment.Contains("外出し"))
					{
						ctrlMetaball[1] = item;
						ctrlMetaball[1].chaCustom = _female;
					}
					else if (item.comment.Contains("吐く"))
					{
						ctrlMetaball[2] = item;
						ctrlMetaball[2].chaCustom = _female;
					}
					else if (item.comment.Contains("アナル"))
					{
						ctrlMetaball[3] = item;
						ctrlMetaball[3].chaCustom = _female;
					}
				}
			}
		}
		aobjBody[0] = _objMale;
		aobjBody[1] = _female.objBodyBone;
		for (int j = 0; j < 8; j++)
		{
			actrlShoot[j] = new ShootCtrl();
			aParam[j] = new MetaballParameterInfo();
		}
	}

	public void SetParticle(HParticleCtrl _particle)
	{
		particle = _particle;
	}

	public bool Load(string _strAssetPath, string _nameFile, GameObject _objMale, GameObject _objMale1, ChaControl _female1, HashSet<string> _hasAssetPath)
	{
		aobjBody[0] = _objMale;
		aobjBody[2] = _objMale1;
		aobjBody[3] = ((!_female1) ? null : _female1.objBodyBone);
		if (ctrlMetaball[5] != null)
		{
			ctrlMetaball[5].chaCustom = _female1;
		}
		ShootCtrl[] array = actrlShoot;
		foreach (ShootCtrl shootCtrl in array)
		{
			shootCtrl.lstShoot.Clear();
		}
		List<MetaballParam> list = GlobalMethod.LoadAllFolder<MetaballParam>(_strAssetPath, _nameFile);
		foreach (MetaballParam item in list)
		{
			foreach (MetaballParam.Param item2 in item.param)
			{
				ctrlMetaball[item2.ID].isEnable = item2.Enable;
				if (!item2.Enable)
				{
					continue;
				}
				ctrlMetaball[item2.ID].ShootAxis = ((!aobjBody[item2.Dir_parent_sex]) ? null : aobjBody[item2.Dir_parent_sex].transform.FindLoop(item2.ObjDirParent));
				GameObject gameObject = ((!aobjBody[item2.Stop_parent_sex]) ? null : aobjBody[item2.Stop_parent_sex].transform.FindLoop(item2.ObjStopParent));
				ctrlMetaball[item2.ID].parentTransform = ((!gameObject) ? null : gameObject.transform);
				aParam[item2.ID].objParent = gameObject;
				aParam[item2.ID].objShoot = LoadSiruObject(item2.BulletAssetPath, item2.ObjBullet, item2.BulletManifesto);
				ctrlMetaball[item2.ID].ShootObj = aParam[item2.ID].objShoot;
				if (_hasAssetPath != null)
				{
					_hasAssetPath.Add(item2.BulletAssetPath);
				}
				else
				{
					AssetBundleManager.UnloadAssetBundle(item2.BulletAssetPath, true, item2.BulletManifesto);
				}
				aParam[item2.ID].objCondom = LoadSiruObject(item2.CondomAssetPath, item2.ObjCondom, item2.CondomManifesto);
				ctrlMetaball[item2.ID].objCondom = aParam[item2.ID].objCondom;
				if (_hasAssetPath != null)
				{
					_hasAssetPath.Add(item2.CondomAssetPath);
				}
				else
				{
					AssetBundleManager.UnloadAssetBundle(item2.CondomAssetPath, true, item2.CondomManifesto);
				}
				ctrlMetaball[item2.ID].aInterval = new float[item2.Interval.Length];
				item2.Interval.CopyTo(ctrlMetaball[item2.ID].aInterval, 0);
				aParam[item2.ID].speedS = new Vector2(item2.S_Speed_Min, item2.S_Speed_Max);
				aParam[item2.ID].rotationS = new Vector2(item2.S_RandomDirX, item2.S_RandomDirY);
				aParam[item2.ID].speedM = new Vector2(item2.M_Speed_Min, item2.M_Speed_Max);
				aParam[item2.ID].rotationM = new Vector2(item2.M_RandomDirX, item2.M_RandomDirY);
				aParam[item2.ID].speedL = new Vector2(item2.L_Speed_Min, item2.L_Speed_Max);
				aParam[item2.ID].rotationL = new Vector2(item2.L_RandomDirX, item2.L_RandomDirY);
				ctrlMetaball[item2.ID].speedSMin = aParam[item2.ID].speedS.x;
				ctrlMetaball[item2.ID].speedSMax = aParam[item2.ID].speedS.y;
				ctrlMetaball[item2.ID].randomRotationS = aParam[item2.ID].rotationS;
				ctrlMetaball[item2.ID].speedMMin = aParam[item2.ID].speedM.x;
				ctrlMetaball[item2.ID].speedMMax = aParam[item2.ID].speedM.y;
				ctrlMetaball[item2.ID].randomRotationM = aParam[item2.ID].rotationM;
				ctrlMetaball[item2.ID].speedLMin = aParam[item2.ID].speedL.x;
				ctrlMetaball[item2.ID].speedLMax = aParam[item2.ID].speedL.y;
				ctrlMetaball[item2.ID].randomRotationL = aParam[item2.ID].rotationL;
				actrlShoot[item2.ID].lstShoot.Clear();
				string[] shootCondition = item2.ShootCondition;
				foreach (string text in shootCondition)
				{
					ShootInfo shootInfo = new ShootInfo();
					string[] array2 = text.Split('/');
					if (array2.Length == 3)
					{
						shootInfo.isInside = array2[0] == "1";
						shootInfo.nameAnimation = array2[1];
						shootInfo.timeShoot = float.Parse(array2[2]);
						actrlShoot[item2.ID].lstShoot.Add(shootInfo);
					}
				}
			}
		}
		return true;
	}

	public bool SetParameterFromState()
	{
		for (int i = 0; i < ctrlMetaball.Length; i++)
		{
			if (ctrlMetaball[i] != null)
			{
				ctrlMetaball[i].speedSMin = aParam[i].speedS.x;
				ctrlMetaball[i].speedSMax = aParam[i].speedS.y;
				ctrlMetaball[i].randomRotationS = aParam[i].rotationS;
				ctrlMetaball[i].speedMMin = aParam[i].speedM.x;
				ctrlMetaball[i].speedMMax = aParam[i].speedM.y;
				ctrlMetaball[i].randomRotationM = aParam[i].rotationM;
				ctrlMetaball[i].speedLMin = aParam[i].speedL.x;
				ctrlMetaball[i].speedLMax = aParam[i].speedL.y;
				ctrlMetaball[i].randomRotationL = aParam[i].rotationL;
				ctrlMetaball[i].ShootObj = aParam[i].objShoot;
				ctrlMetaball[i].parentTransform = ((!aParam[i].objParent) ? null : aParam[i].objParent.transform);
			}
		}
		return true;
	}

	public bool Proc(AnimatorStateInfo _stateInfo, HFlag _flags, bool _isInside = false)
	{
		if (Manager.Config.EtcData.SemenType == 0)
		{
			return true;
		}
		for (int i = 0; i < actrlShoot.Length; i++)
		{
			if ((Manager.Config.EtcData.SemenType == 2 && i != 1 && i != 4) || !actrlShoot[i].IsAnimation(_isInside, _stateInfo))
			{
				continue;
			}
			if (Manager.Config.EtcData.SemenType == 2)
			{
				if (particle != null && (i == 1 || i == 4))
				{
					particle.Play(0);
				}
			}
			else if (ctrlMetaball[i] != null)
			{
				ctrlMetaball[i].ShootMetaBallStart(_flags.isCondom);
				if (i == 1 || i == 4)
				{
					_flags.isCondom = false;
				}
			}
		}
		return true;
	}

	public bool Clear()
	{
		UpdateMeta[] array = ctrlUpdate;
		foreach (UpdateMeta updateMeta in array)
		{
			updateMeta.MetaBallClear();
		}
		return true;
	}

	private GameObject LoadSiruObject(string _pathAsset, string _nameFile, string _manifest)
	{
		if (_nameFile == string.Empty)
		{
			return null;
		}
		return CommonLib.LoadAsset<GameObject>(_pathAsset, _nameFile, false, _manifest);
	}
}
