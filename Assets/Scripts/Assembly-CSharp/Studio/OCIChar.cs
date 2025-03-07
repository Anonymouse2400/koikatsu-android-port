using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion;
using Illusion.Extensions;
using Manager;
using RootMotion.FinalIK;
using UnityEngine;

namespace Studio
{
	public class OCIChar : ObjectCtrlInfo
	{
		public class BoneInfo
		{
			private GameObject m_GameObject;

			public GuideObject guideObject { get; private set; }

			public OIBoneInfo boneInfo { get; private set; }

			public GameObject gameObject
			{
				get
				{
					if (m_GameObject == null)
					{
						m_GameObject = guideObject.gameObject;
					}
					return m_GameObject;
				}
			}

			public bool active
			{
				get
				{
					return gameObject != null && gameObject.activeSelf;
				}
				set
				{
					if ((bool)gameObject)
					{
						gameObject.SetActiveIfDifferent(value);
					}
				}
			}

			public OIBoneInfo.BoneGroup boneGroup
			{
				get
				{
					return boneInfo.group;
				}
			}

			public float scaleRate
			{
				get
				{
					return guideObject.scaleRate;
				}
				set
				{
					guideObject.scaleRate = value;
				}
			}

			public int layer
			{
				set
				{
					guideObject.SetLayer(gameObject, value);
				}
			}

			public Color color
			{
				set
				{
					guideObject.guideSelect.color = value;
				}
			}

			public int boneID { get; private set; }

			public Vector3 posision
			{
				get
				{
					return guideObject.transformTarget.position;
				}
			}

			public BoneInfo(GuideObject _guideObject, OIBoneInfo _boneInfo, int _boneID)
			{
				guideObject = _guideObject;
				boneInfo = _boneInfo;
				boneID = _boneID;
			}
		}

		public class IKInfo
		{
			private GameObject m_GameObject;

			public GuideObject guideObject { get; private set; }

			public OIIKTargetInfo targetInfo { get; private set; }

			public Transform baseObject { get; private set; }

			public Transform targetObject { get; private set; }

			public Transform boneObject { get; private set; }

			public GameObject gameObject
			{
				get
				{
					if (m_GameObject == null)
					{
						m_GameObject = guideObject.gameObject;
					}
					return m_GameObject;
				}
			}

			public bool active
			{
				get
				{
					return gameObject != null && gameObject.activeSelf;
				}
				set
				{
					if ((bool)gameObject)
					{
						gameObject.SetActive(value);
					}
				}
			}

			public OIBoneInfo.BoneGroup boneGroup
			{
				get
				{
					return targetInfo.group;
				}
			}

			public float scaleRate
			{
				get
				{
					return guideObject.scaleRate;
				}
				set
				{
					guideObject.scaleRate = value;
				}
			}

			public int layer
			{
				set
				{
					guideObject.SetLayer(gameObject, value);
				}
			}

			public IKInfo(GuideObject _guideObject, OIIKTargetInfo _targetInfo, Transform _base, Transform _target, Transform _bone)
			{
				guideObject = _guideObject;
				targetInfo = _targetInfo;
				baseObject = _base;
				targetObject = _target;
				boneObject = _bone;
			}

			public void CopyBaseValue()
			{
				targetObject.position = baseObject.position;
				targetObject.eulerAngles = baseObject.eulerAngles;
				guideObject.changeAmount.pos = targetObject.localPosition;
				guideObject.changeAmount.rot = ((!guideObject.enableRot) ? Vector3.zero : targetObject.localEulerAngles);
			}

			public void CopyBone()
			{
				targetObject.position = boneObject.position;
				targetObject.eulerAngles = boneObject.eulerAngles;
				guideObject.changeAmount.pos = targetObject.localPosition;
				guideObject.changeAmount.rot = ((!guideObject.enableRot) ? Vector3.zero : targetObject.localEulerAngles);
			}

			public void CopyBoneRotation()
			{
				targetObject.eulerAngles = boneObject.eulerAngles;
				guideObject.changeAmount.rot = ((!guideObject.enableRot) ? Vector3.zero : targetObject.localEulerAngles);
			}
		}

		public class LookAtInfo
		{
			private GameObject m_GameObject;

			public GuideObject guideObject { get; private set; }

			public LookAtTargetInfo targetInfo { get; private set; }

			public GameObject gameObject
			{
				get
				{
					if (m_GameObject == null)
					{
						m_GameObject = guideObject.gameObject;
					}
					return m_GameObject;
				}
			}

			public Transform target
			{
				get
				{
					return guideObject.transformTarget;
				}
			}

			public bool active
			{
				get
				{
					return gameObject != null && gameObject.activeSelf;
				}
				set
				{
					if ((bool)gameObject)
					{
						gameObject.SetActive(value);
					}
				}
			}

			public int layer
			{
				set
				{
					guideObject.SetLayer(gameObject, value);
				}
			}

			public LookAtInfo(GuideObject _guideObject, LookAtTargetInfo _targetInfo)
			{
				guideObject = _guideObject;
				targetInfo = _targetInfo;
			}
		}

		public class LoadedAnimeInfo
		{
			public Info.FileInfo baseFile = new Info.FileInfo();

			public Info.FileInfo overrideFile = new Info.FileInfo();

			public bool BaseCheck(string _bundle, string _file)
			{
				return (baseFile.bundlePath != _bundle) | (baseFile.fileName != _file);
			}

			public bool OverrideCheck(string _bundle, string _file)
			{
				return (overrideFile.bundlePath != _bundle) | (overrideFile.fileName != _file);
			}
		}

		public class AccessPointInfo
		{
			public TreeNodeObject root;

			public Dictionary<int, TreeNodeObject> child;

			public AccessPointInfo(TreeNodeObject _root)
			{
				root = _root;
				child = new Dictionary<int, TreeNodeObject>();
			}
		}

		public ChaReference charReference;

		public Dictionary<int, AccessPointInfo> dicAccessPoint = new Dictionary<int, AccessPointInfo>();

		public List<BoneInfo> listBones = new List<BoneInfo>();

		public List<IKInfo> listIKTarget = new List<IKInfo>();

		public LookAtInfo lookAtInfo;

		public ChaControl charInfo;

		public HandAnimeCtrl[] handAnimeCtrl = new HandAnimeCtrl[2];

		public FKCtrl fkCtrl;

		public IKCtrl ikCtrl;

		public FullBodyBipedIK finalIK;

		public NeckLookControllerVer2 neckLookCtrl;

		public DynamicBone[] hairDynamic;

		public DynamicBone[] skirtDynamic;

		public bool[] dynamicBust = new bool[2] { true, true };

		private bool[] enablePV = new bool[4] { true, true, true, true };

		public OptionItemCtrl optionItemCtrl;

		public bool isAnimeMotion;

		public bool isHAnime;

		public CharAnimeCtrl charAnimeCtrl;

		public YureCtrl yureCtrl;

		public string[] animeParam = new string[2] { "height", "Breast" };

		public Dictionary<TreeNodeObject, int> dicAccessoryPoint = new Dictionary<TreeNodeObject, int>();

		private LoadedAnimeInfo _loadedAnimeInfo = new LoadedAnimeInfo();

		public OICharInfo oiCharInfo
		{
			get
			{
				return objectInfo as OICharInfo;
			}
		}

		public Transform transSon
		{
			get
			{
				return (!charAnimeCtrl) ? null : charAnimeCtrl.transSon;
			}
			set
			{
				if ((bool)charAnimeCtrl)
				{
					charAnimeCtrl.transSon = value;
				}
			}
		}

		public ChaFileStatus charFileStatus
		{
			get
			{
				return charInfo.fileStatus;
			}
		}

		public int sex
		{
			get
			{
				return charInfo.fileParam.sex;
			}
		}

		public byte foregroundEyebrow
		{
			get
			{
				return charInfo.fileFace.foregroundEyebrow;
			}
			set
			{
				charInfo.fileFace.foregroundEyebrow = value;
			}
		}

		public byte foregroundEyes
		{
			get
			{
				return charInfo.fileFace.foregroundEyes;
			}
			set
			{
				charInfo.fileFace.foregroundEyes = value;
			}
		}

		public VoiceCtrl voiceCtrl
		{
			get
			{
				return oiCharInfo.voiceCtrl;
			}
		}

		public VoiceCtrl.Repeat voiceRepeat
		{
			get
			{
				return voiceCtrl.repeat;
			}
			set
			{
				voiceCtrl.repeat = value;
			}
		}

		private int neckPtnOld { get; set; }

		protected int breastLayer { get; set; }

		protected LoadedAnimeInfo loadedAnimeInfo
		{
			get
			{
				return _loadedAnimeInfo;
			}
		}

		public Preparation preparation { get; set; }

		public override float animeSpeed
		{
			get
			{
				return oiCharInfo.animeSpeed;
			}
			set
			{
				oiCharInfo.animeSpeed = value;
				if ((bool)charInfo.animBody)
				{
					charInfo.animBody.speed = value;
				}
			}
		}

		public float animePattern
		{
			get
			{
				return oiCharInfo.animePattern;
			}
			set
			{
				oiCharInfo.animePattern = value;
				if (isAnimeMotion)
				{
					charInfo.setAnimatorParamFloat("motion", oiCharInfo.animePattern);
				}
				if ((bool)optionItemCtrl)
				{
					optionItemCtrl.SetMotion(oiCharInfo.animePattern);
				}
			}
		}

		public float[] animeOptionParam
		{
			get
			{
				return oiCharInfo.animeOptionParam;
			}
		}

		public float animeOptionParam1
		{
			get
			{
				return oiCharInfo.animeOptionParam[0];
			}
			set
			{
				oiCharInfo.animeOptionParam[0] = value;
				if (isHAnime)
				{
					charInfo.setAnimatorParamFloat(animeParam[0], value);
				}
			}
		}

		public float animeOptionParam2
		{
			get
			{
				return oiCharInfo.animeOptionParam[1];
			}
			set
			{
				oiCharInfo.animeOptionParam[1] = value;
				if (isHAnime)
				{
					charInfo.setAnimatorParamFloat(animeParam[1], value);
				}
			}
		}

		public override void OnDelete()
		{
			Singleton<GuideObjectManager>.Instance.Delete(guideObject);
			voiceCtrl.Stop();
			for (int i = 0; i < listBones.Count; i++)
			{
				Singleton<GuideObjectManager>.Instance.Delete(listBones[i].guideObject);
			}
			for (int j = 0; j < listIKTarget.Count; j++)
			{
				Singleton<GuideObjectManager>.Instance.Delete(listIKTarget[j].guideObject);
			}
			Singleton<GuideObjectManager>.Instance.Delete(lookAtInfo.guideObject);
			if (parentInfo != null)
			{
				parentInfo.OnDetachChild(this);
			}
			Studio.DeleteInfo(objectInfo);
		}

		public override void OnAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
			int value = -1;
			if (dicAccessoryPoint.TryGetValue(_parent, out value))
			{
				if (_child.parentInfo == null)
				{
					Studio.DeleteInfo(_child.objectInfo, false);
				}
				else
				{
					_child.parentInfo.OnDetachChild(_child);
				}
				if (!oiCharInfo.child[value].Contains(_child.objectInfo))
				{
					oiCharInfo.child[value].Add(_child.objectInfo);
				}
				bool flag = false;
				string key = Singleton<Info>.Instance.dicAccessoryPointInfo[value].key;
				GameObject referenceInfo = charReference.GetReferenceInfo((ChaReference.RefObjKey)Enum.Parse(typeof(ChaReference.RefObjKey), key));
				if (!flag)
				{
					_child.guideObject.transformTarget.SetParent(referenceInfo.transform);
				}
				_child.guideObject.parent = referenceInfo.transform;
				_child.guideObject.mode = GuideObject.Mode.World;
				_child.guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
				if (!flag)
				{
					_child.objectInfo.changeAmount.pos = _child.guideObject.transformTarget.localPosition;
					_child.objectInfo.changeAmount.rot = _child.guideObject.transformTarget.localEulerAngles;
				}
				else
				{
					_child.objectInfo.changeAmount.pos = _child.guideObject.parent.InverseTransformPoint(_child.objectInfo.changeAmount.pos);
					Quaternion quaternion = Quaternion.Euler(_child.objectInfo.changeAmount.rot) * Quaternion.Inverse(_child.guideObject.parent.rotation);
					_child.objectInfo.changeAmount.rot = quaternion.eulerAngles;
				}
				_child.guideObject.nonconnect = flag;
				_child.guideObject.calcScale = !flag;
				_child.parentInfo = this;
			}
		}

		public override void OnLoadAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
			int value = -1;
			if (dicAccessoryPoint.TryGetValue(_parent, out value))
			{
				if (_child.parentInfo == null)
				{
					Studio.DeleteInfo(_child.objectInfo, false);
				}
				else
				{
					_child.parentInfo.OnDetachChild(_child);
				}
				if (!oiCharInfo.child[value].Contains(_child.objectInfo))
				{
					oiCharInfo.child[value].Add(_child.objectInfo);
				}
				bool flag = false;
				string key = Singleton<Info>.Instance.dicAccessoryPointInfo[value].key;
				GameObject referenceInfo = charReference.GetReferenceInfo((ChaReference.RefObjKey)Enum.Parse(typeof(ChaReference.RefObjKey), key));
				if (!flag)
				{
					_child.guideObject.transformTarget.SetParent(referenceInfo.transform, false);
				}
				_child.guideObject.parent = referenceInfo.transform;
				_child.guideObject.mode = GuideObject.Mode.World;
				_child.guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
				_child.guideObject.changeAmount.OnChange();
				_child.guideObject.nonconnect = flag;
				_child.guideObject.calcScale = !flag;
				_child.parentInfo = this;
			}
		}

		public override void OnDetach()
		{
			parentInfo.OnDetachChild(this);
			guideObject.parent = null;
			Studio.AddInfo(objectInfo, this);
			guideObject.transformTarget.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			objectInfo.changeAmount.pos = guideObject.transformTarget.localPosition;
			objectInfo.changeAmount.rot = guideObject.transformTarget.localEulerAngles;
			guideObject.mode = GuideObject.Mode.Local;
			guideObject.moveCalc = GuideMove.MoveCalc.TYPE1;
			treeNodeObject.ResetVisible();
		}

		public override void OnSelect(bool _select)
		{
			int layer = LayerMask.NameToLayer((!_select) ? "Studio/Select" : "Studio/Col");
			lookAtInfo.layer = layer;
			for (int i = 0; i < listBones.Count; i++)
			{
				listBones[i].layer = layer;
			}
			for (int j = 0; j < listIKTarget.Count; j++)
			{
				listIKTarget[j].layer = layer;
			}
		}

		public override void OnDetachChild(ObjectCtrlInfo _child)
		{
			int count = oiCharInfo.child.Count;
			for (int i = 0; i < count && !oiCharInfo.child[i].Remove(_child.objectInfo); i++)
			{
			}
			_child.parentInfo = null;
		}

		public override void OnSavePreprocessing()
		{
			base.OnSavePreprocessing();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					neckLookCtrl.SaveNeckLookCtrl(writer);
					oiCharInfo.neckByteData = memoryStream.ToArray();
				}
			}
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (BinaryWriter writer2 = new BinaryWriter(memoryStream2))
				{
					charInfo.eyeLookCtrl.eyeLookScript.SaveAngle(writer2);
					oiCharInfo.eyesByteData = memoryStream2.ToArray();
				}
			}
			AnimatorStateInfo currentAnimatorStateInfo = charInfo.animBody.GetCurrentAnimatorStateInfo(0);
			oiCharInfo.animeNormalizedTime = currentAnimatorStateInfo.normalizedTime;
			oiCharInfo.dicAccessGroup = new Dictionary<int, TreeNodeObject.TreeState>();
			oiCharInfo.dicAccessNo = new Dictionary<int, TreeNodeObject.TreeState>();
			foreach (KeyValuePair<int, AccessPointInfo> item in dicAccessPoint)
			{
				oiCharInfo.dicAccessGroup.Add(item.Key, item.Value.root.treeState);
				foreach (KeyValuePair<int, TreeNodeObject> item2 in item.Value.child)
				{
					oiCharInfo.dicAccessNo.Add(item2.Key, item2.Value.treeState);
				}
			}
		}

		public override void OnVisible(bool _visible)
		{
			charInfo.visibleAll = _visible;
			if ((bool)optionItemCtrl)
			{
				optionItemCtrl.outsideVisible = _visible;
			}
			foreach (BoneInfo listBone in listBones)
			{
				listBone.guideObject.visibleOutside = _visible;
			}
			foreach (IKInfo item in listIKTarget)
			{
				item.guideObject.visibleOutside = _visible;
			}
			if (lookAtInfo != null && (bool)lookAtInfo.guideObject)
			{
				lookAtInfo.guideObject.visibleOutside = _visible;
			}
		}

		public void InitKinematic(GameObject _target, FullBodyBipedIK _finalIK, NeckLookControllerVer2 _neckLook, DynamicBone[] _hairDynamic, DynamicBone[] _skirtDynamic)
		{
			neckLookCtrl = _neckLook;
			neckPtnOld = charFileStatus.neckLookPtn;
			hairDynamic = _hairDynamic;
			skirtDynamic = _skirtDynamic;
			InitFK(_target);
			for (int i = 0; i < listIKTarget.Count; i++)
			{
				listIKTarget[i].active = false;
			}
			finalIK = _finalIK;
			finalIK.enabled = false;
		}

		public void InitFK(GameObject _target)
		{
			if (fkCtrl == null && _target != null)
			{
				fkCtrl = _target.AddComponent<FKCtrl>();
			}
			fkCtrl.InitBones(oiCharInfo, charReference);
			fkCtrl.enabled = false;
			for (int i = 0; i < listBones.Count; i++)
			{
				listBones[i].active = false;
			}
		}

		public void ActiveKinematicMode(OICharInfo.KinematicMode _mode, bool _active, bool _force)
		{
			switch (_mode)
			{
			case OICharInfo.KinematicMode.IK:
				if (_force || finalIK.enabled != _active)
				{
					finalIK.enabled = _active;
					oiCharInfo.enableIK = _active;
					for (int j = 0; j < 5; j++)
					{
						ActiveIK((OIBoneInfo.BoneGroup)(1 << j), _active && oiCharInfo.activeIK[j], true);
					}
					if (oiCharInfo.enableIK)
					{
						ActiveKinematicMode(OICharInfo.KinematicMode.FK, false, _force);
					}
				}
				break;
			case OICharInfo.KinematicMode.FK:
				if (_force || fkCtrl.enabled != _active)
				{
					fkCtrl.enabled = _active;
					oiCharInfo.enableFK = _active;
					OIBoneInfo.BoneGroup[] parts = FKCtrl.parts;
					for (int i = 0; i < parts.Length; i++)
					{
						ActiveFK(parts[i], _active && oiCharInfo.activeFK[i], true);
					}
					if (oiCharInfo.enableFK)
					{
						ActiveKinematicMode(OICharInfo.KinematicMode.IK, false, _force);
					}
				}
				break;
			}
			for (int k = 0; k < 4; k++)
			{
				preparation.pvCopy[k] = !oiCharInfo.enableFK && enablePV[k];
			}
		}

		public void ActiveFK(OIBoneInfo.BoneGroup _group, bool _active, bool _force = false)
		{
			OIBoneInfo.BoneGroup[] parts = FKCtrl.parts;
			for (int i = 0; i < parts.Length; i++)
			{
				if ((_group & parts[i]) == 0 || (!_force && (!Utility.SetStruct(ref oiCharInfo.activeFK[i], _active) || !oiCharInfo.enableFK)))
				{
					continue;
				}
				ActiveFKGroup(parts[i], _active);
				foreach (BoneInfo item in listBones.Where((BoneInfo v) => (v.boneGroup & parts[i]) != 0))
				{
					item.active = (_force ? _active : (oiCharInfo.enableFK & oiCharInfo.activeFK[i]));
				}
			}
		}

		public bool IsFKGroup(OIBoneInfo.BoneGroup _group)
		{
			return listBones.Any((BoneInfo v) => (v.boneGroup & _group) != 0);
		}

		public void InitFKBone(OIBoneInfo.BoneGroup _group)
		{
			foreach (BoneInfo item in listBones.Where((BoneInfo v) => (v.boneGroup & _group) != 0))
			{
				item.boneInfo.changeAmount.Reset();
			}
		}

		private void ActiveFKGroup(OIBoneInfo.BoneGroup _group, bool _active)
		{
			switch (_group)
			{
			case OIBoneInfo.BoneGroup.Neck:
				if (_active)
				{
					neckPtnOld = charFileStatus.neckLookPtn;
					ChangeLookNeckPtn(4);
				}
				else
				{
					ChangeLookNeckPtn(neckPtnOld);
				}
				break;
			case OIBoneInfo.BoneGroup.Breast:
				charInfo.playDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL, !_active && dynamicBust[0]);
				charInfo.playDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR, !_active && dynamicBust[1]);
				break;
			}
			fkCtrl.SetEnable(_group, _active);
			switch (_group)
			{
			case OIBoneInfo.BoneGroup.Hair:
				if (!hairDynamic.IsNullOrEmpty())
				{
					for (int j = 0; j < hairDynamic.Length; j++)
					{
						hairDynamic[j].enabled = !_active;
					}
				}
				break;
			case OIBoneInfo.BoneGroup.Skirt:
				if (!skirtDynamic.IsNullOrEmpty())
				{
					for (int i = 0; i < skirtDynamic.Length; i++)
					{
						skirtDynamic[i].enabled = !_active;
					}
				}
				break;
			}
		}

		public void ActiveIK(OIBoneInfo.BoneGroup _group, bool _active, bool _force = false)
		{
			for (int i = 0; i < 5; i++)
			{
				OIBoneInfo.BoneGroup target = (OIBoneInfo.BoneGroup)(1 << i);
				if ((_group & target) == 0 || (!_force && !Utility.SetStruct(ref oiCharInfo.activeIK[i], _active)))
				{
					continue;
				}
				ActiveIKGroup(target, _active);
				foreach (IKInfo item in listIKTarget.Where((IKInfo v) => (v.boneGroup & target) != 0))
				{
					item.active = (_force ? _active : (oiCharInfo.enableIK & oiCharInfo.activeIK[i]));
				}
			}
		}

		private void ActiveIKGroup(OIBoneInfo.BoneGroup _group, bool _active)
		{
			IKSolverFullBodyBiped solver = finalIK.solver;
			float num = ((!_active) ? 0f : 1f);
			switch (_group)
			{
			case OIBoneInfo.BoneGroup.Body:
				solver.spineMapping.twistWeight = num;
				solver.SetEffectorWeights(FullBodyBipedEffector.Body, num, num);
				break;
			case OIBoneInfo.BoneGroup.RightArm:
				solver.rightArmMapping.weight = num;
				solver.SetEffectorWeights(FullBodyBipedEffector.RightShoulder, num, num);
				solver.SetEffectorWeights(FullBodyBipedEffector.RightHand, num, num);
				break;
			case OIBoneInfo.BoneGroup.LeftArm:
				solver.leftArmMapping.weight = num;
				solver.SetEffectorWeights(FullBodyBipedEffector.LeftShoulder, num, num);
				solver.SetEffectorWeights(FullBodyBipedEffector.LeftHand, num, num);
				break;
			case OIBoneInfo.BoneGroup.RightLeg:
				solver.rightLegMapping.weight = num;
				solver.SetEffectorWeights(FullBodyBipedEffector.RightThigh, num, num);
				solver.SetEffectorWeights(FullBodyBipedEffector.RightFoot, num, num);
				break;
			case OIBoneInfo.BoneGroup.LeftLeg:
				solver.leftLegMapping.weight = num;
				solver.SetEffectorWeights(FullBodyBipedEffector.LeftThigh, num, num);
				solver.SetEffectorWeights(FullBodyBipedEffector.LeftFoot, num, num);
				break;
			}
		}

		public void UpdateFKColor(params OIBoneInfo.BoneGroup[] _parts)
		{
			if (_parts.IsNullOrEmpty())
			{
				return;
			}
			foreach (BoneInfo v in listBones)
			{
				int num = Array.FindIndex(_parts, (OIBoneInfo.BoneGroup p) => (p & v.boneGroup) != 0);
				if (num != -1)
				{
					switch (_parts[num])
					{
					case OIBoneInfo.BoneGroup.Hair:
						v.color = Studio.optionSystem.colorFKHair;
						break;
					case OIBoneInfo.BoneGroup.Neck:
						v.color = Studio.optionSystem.colorFKNeck;
						break;
					case OIBoneInfo.BoneGroup.Breast:
						v.color = Studio.optionSystem.colorFKBreast;
						break;
					case OIBoneInfo.BoneGroup.Body:
						v.color = Studio.optionSystem.colorFKBody;
						break;
					case OIBoneInfo.BoneGroup.RightHand:
						v.color = Studio.optionSystem.colorFKRightHand;
						break;
					case OIBoneInfo.BoneGroup.LeftHand:
						v.color = Studio.optionSystem.colorFKLeftHand;
						break;
					case OIBoneInfo.BoneGroup.Skirt:
						v.color = Studio.optionSystem.colorFKSkirt;
						break;
					}
				}
			}
		}

		public void VisibleFKGuide(bool _visible)
		{
			foreach (BoneInfo listBone in listBones)
			{
				listBone.guideObject.visible = _visible;
			}
		}

		public void VisibleIKGuide(bool _visible)
		{
			foreach (IKInfo item in listIKTarget)
			{
				item.guideObject.visible = _visible;
			}
		}

		public void EnableExpressionCategory(int _category, bool _value)
		{
			oiCharInfo.expression[_category] = _value;
			charInfo.EnableExpressionCategory(_category, _value);
		}

		public virtual void LoadAnime(int _group, int _category, int _no, float _normalizedTime = 0f)
		{
			Dictionary<int, Dictionary<int, Info.AnimeLoadInfo>> value = null;
			if (!Singleton<Info>.Instance.dicAnimeLoadInfo.TryGetValue(_group, out value))
			{
				return;
			}
			Dictionary<int, Info.AnimeLoadInfo> value2 = null;
			if (!value.TryGetValue(_category, out value2))
			{
				return;
			}
			Info.AnimeLoadInfo value3 = null;
			if (!value2.TryGetValue(_no, out value3))
			{
				return;
			}
			if (loadedAnimeInfo.BaseCheck(value3.bundlePath, value3.fileName))
			{
				charInfo.LoadAnimation(value3.bundlePath, value3.fileName, string.Empty);
				loadedAnimeInfo.baseFile.bundlePath = value3.bundlePath;
				loadedAnimeInfo.baseFile.fileName = value3.fileName;
			}
			if (value3 is Info.HAnimeLoadInfo)
			{
				Info.HAnimeLoadInfo hAnimeLoadInfo = value3 as Info.HAnimeLoadInfo;
				if (hAnimeLoadInfo.overrideFile.check && loadedAnimeInfo.OverrideCheck(hAnimeLoadInfo.overrideFile.bundlePath, hAnimeLoadInfo.overrideFile.fileName))
				{
					CommonLib.LoadAsset<RuntimeAnimatorController>(hAnimeLoadInfo.overrideFile.bundlePath, hAnimeLoadInfo.overrideFile.fileName, false, string.Empty).SafeProc(delegate(RuntimeAnimatorController rac)
					{
						charAnimeCtrl.animator.runtimeAnimatorController = Utils.Animator.SetupAnimatorOverrideController(charAnimeCtrl.animator.runtimeAnimatorController, rac);
					});
					AssetBundleManager.UnloadAssetBundle(hAnimeLoadInfo.overrideFile.bundlePath, true);
					loadedAnimeInfo.overrideFile.bundlePath = hAnimeLoadInfo.overrideFile.bundlePath;
					loadedAnimeInfo.overrideFile.fileName = hAnimeLoadInfo.overrideFile.fileName;
				}
				isAnimeMotion = hAnimeLoadInfo.isMotion;
				isHAnime = true;
				animeParam[1] = ((!CheckAnimeParam("Breast1")) ? "Breast" : "Breast1");
				charInfo.setAnimatorParamFloat("Breast", charInfo.fileBody.shapeValueBody[4]);
				if (breastLayer != -1)
				{
					charAnimeCtrl.animator.SetLayerWeight(breastLayer, 0f);
					breastLayer = -1;
				}
				if (hAnimeLoadInfo.isBreastLayer)
				{
					charAnimeCtrl.animator.SetLayerWeight(hAnimeLoadInfo.breastLayer, 1f);
					breastLayer = hAnimeLoadInfo.breastLayer;
				}
				if (hAnimeLoadInfo.isMotion)
				{
					charInfo.setAnimatorParamFloat("motion", oiCharInfo.animePattern);
				}
				dynamicBust[0] = hAnimeLoadInfo.dynamic[0];
				dynamicBust[1] = hAnimeLoadInfo.dynamic[1];
				charInfo.playDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL, !oiCharInfo.activeFK[2] && hAnimeLoadInfo.dynamic[0]);
				charInfo.playDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR, !oiCharInfo.activeFK[2] && hAnimeLoadInfo.dynamic[1]);
				for (int i = 0; i < 4; i++)
				{
					enablePV[i] = hAnimeLoadInfo.pv[i];
					preparation.pvCopy[i] = !oiCharInfo.enableFK && enablePV[i];
				}
				if (hAnimeLoadInfo.yureFile.check && yureCtrl.LoadExcel(hAnimeLoadInfo.yureFile.bundlePath, hAnimeLoadInfo.yureFile.fileName))
				{
					yureCtrl.Proc(value3.clip);
				}
				else
				{
					yureCtrl.ResetShape();
				}
			}
			else
			{
				loadedAnimeInfo.overrideFile.Clear();
				for (int j = 0; j < 4; j++)
				{
					enablePV[j] = true;
					preparation.pvCopy[j] = !oiCharInfo.enableFK && enablePV[j];
				}
				isAnimeMotion = false;
				isHAnime = false;
			}
			optionItemCtrl.LoadAnimeItem(value3, value3.clip, charInfo.fileBody.shapeValueBody[0], oiCharInfo.animePattern);
			if (_normalizedTime != 0f)
			{
				charAnimeCtrl.Play(value3.clip, _normalizedTime);
			}
			else
			{
				charInfo.AnimPlay(value3.clip);
			}
			animeOptionParam1 = animeOptionParam1;
			animeOptionParam2 = animeOptionParam2;
			animeParam[0] = ((!CheckAnimeParam("height1")) ? "height" : "height1");
			charInfo.setAnimatorParamFloat("height", charInfo.fileBody.shapeValueBody[0]);
			charAnimeCtrl.nameHadh = Animator.StringToHash("Base Layer." + value3.clip);
			oiCharInfo.animeInfo.Set(_group, _category, _no);
			SetNipStand(oiCharInfo.nipple);
			SetSonLength(oiCharInfo.sonLength);
		}

		public virtual void ChangeHandAnime(int _type, int _ptn)
		{
			oiCharInfo.handPtn[_type] = _ptn;
			handAnimeCtrl[_type].ptn = _ptn;
		}

		public virtual void RestartAnime()
		{
			AnimatorStateInfo animatorStateInfo = charInfo.getAnimatorStateInfo(0);
			charInfo.animBody.Play(animatorStateInfo.shortNameHash, 0, 0f);
		}

		private bool CheckAnimeParam(string _name)
		{
			AnimatorControllerParameter[] parameters = charInfo.animBody.parameters;
			if (parameters.IsNullOrEmpty())
			{
				return false;
			}
			AnimatorControllerParameter[] array = parameters;
			foreach (AnimatorControllerParameter animatorControllerParameter in array)
			{
				if (animatorControllerParameter.name == _name)
				{
					return true;
				}
			}
			return false;
		}

		public virtual void ChangeChara(string _path)
		{
			foreach (BoneInfo item in listBones.Where((BoneInfo v) => v.boneGroup == OIBoneInfo.BoneGroup.Hair).ToList())
			{
				Singleton<GuideObjectManager>.Instance.Delete(item.guideObject);
			}
			listBones = listBones.Where((BoneInfo v) => v.boneGroup != OIBoneInfo.BoneGroup.Hair).ToList();
			int[] array = (from b in oiCharInfo.bones
				where b.Value.@group == OIBoneInfo.BoneGroup.Hair
				select b.Key).ToArray();
			for (int j = 0; j < array.Length; j++)
			{
				oiCharInfo.bones.Remove(array[j]);
			}
			hairDynamic = null;
			skirtDynamic = null;
			charInfo.chaFile.LoadCharaFile(_path, byte.MaxValue, true);
			charInfo.ChangeCoordinateType((ChaFileDefine.CoordinateType)charFileStatus.coordinateType);
			charInfo.Reload();
			treeNodeObject.textName = charInfo.chaFile.parameter.fullname;
			AddObjectAssist.InitHairBone(this, Singleton<Info>.Instance.dicBoneInfo);
			hairDynamic = AddObjectFemale.GetHairDynamic(charInfo.objHair);
			skirtDynamic = AddObjectFemale.GetSkirtDynamic(charInfo.objClothes);
			InitFK(null);
			foreach (var item2 in FKCtrl.parts.Select((OIBoneInfo.BoneGroup p, int i) => new { p, i }))
			{
				ActiveFK(item2.p, oiCharInfo.activeFK[item2.i], oiCharInfo.activeFK[item2.i]);
			}
			ActiveKinematicMode(OICharInfo.KinematicMode.FK, oiCharInfo.enableFK, true);
			UpdateFKColor(OIBoneInfo.BoneGroup.Hair);
			ChangeEyesOpen(charFileStatus.eyesOpenMax);
			ChangeBlink(charFileStatus.eyesBlink);
			ChangeMouthOpen(oiCharInfo.mouthOpen);
		}

		public virtual void SetCoordinateInfo(ChaFileDefine.CoordinateType _type, bool _force = false)
		{
			if (_force || charInfo.fileStatus.coordinateType != (int)_type)
			{
				charInfo.ChangeCoordinateType(_type);
				charInfo.Reload(false, true, true, true);
			}
		}

		public virtual void SetShoesType(int _type)
		{
			charInfo.fileStatus.shoesType = (byte)_type;
		}

		public virtual void SetClothesStateAll(int _state)
		{
		}

		public virtual void SetClothesState(int _id, byte _state)
		{
			charInfo.SetClothesState(_id, _state);
		}

		public virtual void ShowAccessory(int _id, bool _flag)
		{
			charFileStatus.showAccessory[_id] = _flag;
		}

		public virtual void LoadClothesFile(string _path)
		{
			charInfo.nowCoordinate.LoadFile(_path);
			charInfo.AssignCoordinate((ChaFileDefine.CoordinateType)charInfo.fileStatus.coordinateType);
			charInfo.Reload(false, true, true, true);
		}

		public virtual void SetSiruFlags(ChaFileDefine.SiruParts _parts, byte _state)
		{
			oiCharInfo.siru[(int)_parts] = _state;
		}

		public virtual byte GetSiruFlags(ChaFileDefine.SiruParts _parts)
		{
			return 0;
		}

		public virtual void SetNipStand(float _value)
		{
		}

		public virtual void SetVisibleSimple(bool _flag)
		{
		}

		public bool GetVisibleSimple()
		{
			return oiCharInfo.visibleSimple;
		}

		public virtual void SetSimpleColor(Color _color)
		{
			oiCharInfo.simpleColor = _color;
		}

		public virtual void SetVisibleSon(bool _flag)
		{
			oiCharInfo.visibleSon = _flag;
			charFileStatus.visibleSonAlways = _flag;
		}

		public virtual float GetSonLength()
		{
			return oiCharInfo.sonLength;
		}

		public virtual void SetSonLength(float _value)
		{
			oiCharInfo.sonLength = _value;
		}

		public virtual void SetTearsLv(byte _state)
		{
			charFileStatus.tearsLv = _state;
		}

		public virtual byte GetTearsLv()
		{
			return charFileStatus.tearsLv;
		}

		public virtual void SetHohoAkaRate(float _value)
		{
			charInfo.ChangeHohoAkaRate(_value);
		}

		public virtual float GetHohoAkaRate()
		{
			return charInfo.fileStatus.hohoAkaRate;
		}

		public virtual void ChangeLookEyesPtn(int _ptn, bool _force = false)
		{
			int num = ((!_force) ? charInfo.fileStatus.eyesLookPtn : (-1));
			if (_ptn == 4 && num != 4)
			{
				charInfo.eyeLookCtrl.target = lookAtInfo.target;
				lookAtInfo.active = true;
			}
			else if (num == 4 && _ptn != 4)
			{
				charInfo.eyeLookCtrl.target = Camera.main.transform;
				lookAtInfo.active = false;
			}
			charInfo.ChangeLookEyesPtn(_ptn);
		}

		public virtual void ChangeLookNeckPtn(int _ptn)
		{
			charInfo.ChangeLookNeckPtn(_ptn);
		}

		public virtual void ChangeEyesOpen(float _value)
		{
			charInfo.ChangeEyesOpenMax(_value);
		}

		public virtual void ChangeBlink(bool _value)
		{
			charInfo.ChangeEyesBlinkFlag(_value);
		}

		public virtual void ChangeMouthOpen(float _value)
		{
			oiCharInfo.mouthOpen = _value;
			if (charInfo.mouthCtrl != null)
			{
				charInfo.mouthCtrl.FixedRate = ((!voiceCtrl.isPlay || !oiCharInfo.lipSync) ? _value : (-1f));
			}
		}

		public virtual void ChangeLipSync(bool _value)
		{
			oiCharInfo.lipSync = _value;
			charInfo.SetVoiceTransform((!_value) ? null : oiCharInfo.voiceCtrl.transVoice);
			ChangeMouthOpen(oiCharInfo.mouthOpen);
		}

		public virtual void SetVoice()
		{
			ChangeLipSync(oiCharInfo.lipSync);
		}

		public virtual void AddVoice(int _group, int _category, int _no)
		{
			voiceCtrl.list.Add(new VoiceCtrl.VoiceInfo(_group, _category, _no));
		}

		public virtual void DeleteVoice(int _index)
		{
			voiceCtrl.list.RemoveAt(_index);
			if (voiceCtrl.index == _index)
			{
				voiceCtrl.index = -1;
				voiceCtrl.Stop();
			}
		}

		public virtual void DeleteAllVoice()
		{
			voiceCtrl.list.Clear();
			voiceCtrl.Stop();
		}

		public virtual bool PlayVoice(int _index)
		{
			return voiceCtrl.Play((_index >= 0) ? _index : 0);
		}

		public virtual void StopVoice()
		{
			voiceCtrl.Stop();
		}
	}
}
