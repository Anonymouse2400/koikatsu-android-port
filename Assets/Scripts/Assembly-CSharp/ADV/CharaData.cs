using System.Collections.Generic;
using ADV.Commands.Base;
using Illusion.Extensions;
using Illusion.Game;
using Illusion.Game.Elements.EasyLoader;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ADV
{
	public class CharaData
	{
		public class MotionReserver
		{
			public IKMotion ikMotion { get; set; }

			public YureMotion yureMotion { get; set; }

			public MotionOverride motionOverride { get; set; }
		}

		private class Backup
		{
			public Transform transform { get; private set; }

			public Transform parent { get; private set; }

			public Vector3 position { get; private set; }

			public Quaternion rotation { get; private set; }

			public Backup(Transform transform, TextScenario scenario)
			{
				this.transform = transform;
				parent = transform.parent;
				position = transform.localPosition;
				rotation = transform.localRotation;
				transform.SetParent(scenario.commandController.Character, false);
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}

			public void Repair()
			{
				if (!(transform == null))
				{
					transform.SetParent(parent, false);
					transform.localPosition = position;
					transform.localRotation = rotation;
				}
			}
		}

		public class FaceIcon
		{
			private CharaData chara;

			private TextScenario scenario;

			private Image image;

			private GameObject icon;

			private ADVFaceIconData.Param param;

			private Transform _target;

			private Transform target
			{
				get
				{
					return this.GetCache(ref _target, () => chara.chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.HeadParent).transform);
				}
			}

			public FaceIcon(CharaData chara, TextScenario scenario)
			{
				this.chara = chara;
				this.scenario = scenario;
			}

			public void Update()
			{
				SetPosition();
			}

			public void Load(string key)
			{
				Release();
				param = scenario.FaceIconParamDic[key];
				string assetBundleName = "adv/faceicon/icon/" + param.path + ".unity3d";
				icon = Object.Instantiate(scenario.FaceIcons.GetChild(0).gameObject, scenario.FaceIcons);
				icon.name = param.name;
				icon.SetActive(true);
				image = icon.GetComponent<Image>();
				Utils.Bundle.LoadSprite(assetBundleName, param.name, image, true, param.anim);
				float[] rot = param.rot;
				float[] scal = param.scal;
				RectTransform rectTransform = image.rectTransform;
				rectTransform.eulerAngles = new Vector3(rot[0], rot[1]);
				rectTransform.localScale = new Vector3(scal[0], scal[1]);
				SetPosition();
			}

			public void Release()
			{
				if (image != null)
				{
					Object.Destroy(image);
				}
				if (icon != null)
				{
					Object.Destroy(icon);
				}
				image = null;
				icon = null;
				param = null;
			}

			private void SetPosition()
			{
				if (!(image == null))
				{
					image.rectTransform.SetPosition(target, new Vector3(param.pos[0], param.pos[1]), scenario.AdvCamera);
				}
			}
		}

		public class CharaItem
		{
			private Illusion.Game.Elements.EasyLoader.Motion motion;

			public GameObject item { get; private set; }

			public CharaItem()
			{
			}

			public CharaItem(GameObject item)
			{
				this.item = item;
			}

			public void Delete()
			{
				if (item != null)
				{
					Object.Destroy(item);
					item = null;
				}
			}

			public void LoadObject(string bundle, string asset, Transform root, bool worldPositionStays = false, string manifest = null)
			{
				Delete();
				GameObject asset2 = AssetBundleManager.LoadAsset(bundle, asset, typeof(GameObject), manifest).GetAsset<GameObject>();
				item = Object.Instantiate(asset2);
				AssetBundleManager.UnloadAssetBundle(bundle, false, manifest);
				item.name = asset2.name;
				item.transform.SetParent(root, worldPositionStays);
			}

			public void LoadAnimator(string bundle, string asset, string state)
			{
				Animator orAddComponent = item.GetOrAddComponent<Animator>();
				if (motion == null)
				{
					motion = new Illusion.Game.Elements.EasyLoader.Motion(bundle, asset, state);
				}
				if (motion.Setting(orAddComponent, bundle, asset, state, true))
				{
					motion.Play(orAddComponent);
				}
			}
		}

		private ChaControl _chaCtrl;

		private MotionOverride _motionOverride = new MotionOverride();

		private RuntimeAnimatorController anmCtrl;

		private TextScenario scenario;

		public FaceIcon faceIcon { get; private set; }

		public Dictionary<int, CharaItem> itemDic { get; private set; }

		public bool initialized { get; private set; }

		private bool isADVCreateChara { get; set; }

		private GameObject dataBaseRoot { get; set; }

		public Transform root
		{
			get
			{
				if (chaCtrl == null)
				{
					return null;
				}
				return chaCtrl.transform;
			}
		}

		public SaveData.Heroine heroine
		{
			get
			{
				return data as SaveData.Heroine;
			}
		}

		public int voiceNo
		{
			get
			{
				return data.voiceNo;
			}
		}

		public float voicePitch
		{
			get
			{
				return data.voicePitch;
			}
		}

		public Transform voiceTrans
		{
			get
			{
				if (chaCtrl == null || !chaCtrl.loadEnd)
				{
					return null;
				}
				GameObject referenceInfo = chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.HeadParent);
				if (referenceInfo == null)
				{
					return null;
				}
				return referenceInfo.transform;
			}
		}

		public SaveData.CharaData data { get; private set; }

		public ChaControl chaCtrl
		{
			get
			{
				return this.GetCacheObject(ref _chaCtrl, () => data.chaCtrl);
			}
		}

		public Transform transform
		{
			get
			{
				if (data.transform != null)
				{
					return data.transform;
				}
				if (chaCtrl == null)
				{
					return null;
				}
				return chaCtrl.transform;
			}
		}

		private MotionReserver motionReserver { get; set; }

		public IKMotion ikMotion
		{
			get
			{
				return _ikMotion;
			}
		}

		private IKMotion _ikMotion { get; set; }

		public YureMotion yureMotion
		{
			get
			{
				return _yureMotion;
			}
		}

		private YureMotion _yureMotion { get; set; }

		public MotionOverride motionOverride
		{
			get
			{
				return _motionOverride;
			}
		}

		private Backup backup { get; set; }

		public CharaData(SaveData.CharaData data, TextScenario scenario)
		{
			this.data = data;
			this.scenario = scenario;
			_chaCtrl = Singleton<Character>.Instance.CreateChara(Game.CharaDataToSex(data), scenario.commandController.Character.gameObject, -1, data.charFile, false);
			faceIcon = new FaceIcon(this, scenario);
			itemDic = new Dictionary<int, CharaItem>();
			isADVCreateChara = true;
			initialized = true;
		}

		public CharaData(SaveData.CharaData data, TextScenario scenario, MotionReserver motionReserver, bool isParent = false)
		{
			this.data = data;
			this.scenario = scenario;
			this.motionReserver = motionReserver;
			isADVCreateChara = data.chaCtrl == null;
			_chaCtrl = data.chaCtrl;
			dataBaseRoot = data.root;
			if (_chaCtrl != null)
			{
				Initialize();
				if (isParent)
				{
					backup = new Backup(_chaCtrl.transform, scenario);
				}
			}
		}

		public CharaData(SaveData.CharaData data, ChaControl chaCtrl, TextScenario scenario, MotionReserver motionReserver, bool hiPoly = true, bool isParent = false)
		{
			this.motionReserver = motionReserver;
			isADVCreateChara = chaCtrl == null;
			if (isADVCreateChara)
			{
				ChaFileControl chaFileControl = new ChaFileControl();
				ChaFile.CopyChaFile(chaFileControl, (!(data.chaCtrl == null)) ? data.chaCtrl.chaFile : data.charFile);
				chaCtrl = Singleton<Character>.Instance.CreateChara(Game.CharaDataToSex(data), scenario.commandController.Character.gameObject, 0, chaFileControl, hiPoly);
			}
			this.data = data;
			_chaCtrl = chaCtrl;
			this.scenario = scenario;
			dataBaseRoot = data.root;
			data.SetRoot(chaCtrl.gameObject);
			if (isADVCreateChara)
			{
				Singleton<Character>.Instance.loading.LoadAsync(scenario.commandController.LoadingCharaList.Count, chaCtrl);
				scenario.commandController.LoadingCharaList.Add(this);
			}
			else
			{
				Initialize();
			}
			if (isParent)
			{
				backup = new Backup(_chaCtrl.transform, scenario);
			}
		}

		public void Initialize()
		{
			faceIcon = new FaceIcon(this, scenario);
			itemDic = new Dictionary<int, CharaItem>();
			if (motionReserver != null)
			{
				if (motionReserver.ikMotion != null)
				{
					_ikMotion = motionReserver.ikMotion;
				}
				if (motionReserver.yureMotion != null)
				{
					_yureMotion = motionReserver.yureMotion;
				}
				if (motionReserver.motionOverride != null)
				{
					_motionOverride = motionReserver.motionOverride;
				}
			}
			if (_yureMotion == null)
			{
				_yureMotion = new YureMotion();
				_yureMotion.Create(chaCtrl);
			}
			if (chaCtrl.animBody != null)
			{
				anmCtrl = chaCtrl.animBody.runtimeAnimatorController;
			}
			if (isADVCreateChara)
			{
				scenario.commandController.LoadingCharaList.Remove(this);
				chaCtrl.SetActiveTop(true);
				MotionIK motionIK = MotionDefault();
				if (_ikMotion == null)
				{
					_ikMotion = new IKMotion();
					_ikMotion.Create(chaCtrl, motionIK);
				}
			}
			if (_ikMotion == null)
			{
				_ikMotion = new IKMotion();
				_ikMotion.Create(chaCtrl, null);
			}
			initialized = true;
		}

		public MotionIK MotionDefault()
		{
			Game.AnimePack advAnimePack = Singleton<Game>.Instance.advAnimePack;
			MotionIK motionIK;
			advAnimePack.SetDefalut(chaCtrl, out motionIK);
			_motionOverride.state = advAnimePack.defaultName;
			return motionIK;
		}

		public void MotionPlay(ADV.Commands.Base.Motion.Data motion, bool isCrossFade)
		{
			if (isCrossFade)
			{
				scenario.CrossFadeStart();
			}
			if (_motionOverride.Setting(chaCtrl.animBody, motion.assetBundleName, motion.assetName, motion.overrideAssetBundleName, motion.overrideAssetName, motion.stateName, true))
			{
				Info.Anime.Play play = scenario.info.anime.play;
				_motionOverride.isCrossFade = false;
				_motionOverride.layers = motion.layerNo;
				_motionOverride.transitionDuration = play.transitionDuration;
				_motionOverride.normalizedTime = play.normalizedTime;
				_motionOverride.Play(chaCtrl.animBody);
			}
			_ikMotion.Setting(chaCtrl, motion.ikAssetBundleName, motion.ikAssetName, motion.stateName, false);
			_yureMotion.Setting(chaCtrl, motion.shakeAssetBundleName, motion.shakeAssetName, motion.stateName, false);
		}

		public void Update()
		{
			if (!initialized)
			{
				if (Singleton<Character>.Instance.loading.IsEnd(chaCtrl))
				{
					Initialize();
				}
			}
			else
			{
				faceIcon.Update();
			}
		}

		public void Release()
		{
			if (initialized)
			{
				faceIcon.Release();
				foreach (KeyValuePair<int, CharaItem> item in itemDic)
				{
					item.Value.Delete();
				}
			}
			if (data != null)
			{
				data.SetRoot(dataBaseRoot);
			}
			if (anmCtrl != null && chaCtrl != null && !chaCtrl.hiPoly && chaCtrl.animBody != null)
			{
				chaCtrl.animBody.runtimeAnimatorController = anmCtrl;
			}
			if (isADVCreateChara && Singleton<Character>.IsInstance() && chaCtrl != null && !Singleton<Character>.Instance.loading.Cancel(chaCtrl))
			{
				Singleton<Character>.Instance.loading.SafeDeleteChara(chaCtrl);
			}
			if (backup != null)
			{
				backup.Repair();
			}
		}
	}
}
