using System.Collections.Generic;
using System.IO;
using System.Linq;
using IllusionUtility.GetUtility;
using RootMotion;
using RootMotion.FinalIK;
using UnityEngine;

namespace Studio
{
	public static class AddObjectAssist
	{
		public static void InitBone(OCIChar _ociChar, Transform _transformRoot, Dictionary<int, Info.BoneInfo> _dicBoneInfo)
		{
			foreach (KeyValuePair<int, Info.BoneInfo> item in _dicBoneInfo)
			{
				if (_ociChar.sex == 1 && item.Value.level == 2)
				{
					continue;
				}
				GameObject gameObject = null;
				switch (item.Value.group)
				{
				case 7:
				case 8:
				case 9:
				{
					GameObject referenceInfo = _ociChar.charReference.GetReferenceInfo(ChaReference.RefObjKey.HairParent);
					gameObject = referenceInfo.transform.FindLoop(item.Value.bone);
					break;
				}
				default:
					gameObject = _transformRoot.FindLoop(item.Value.bone);
					if (!(gameObject == null))
					{
					}
					break;
				}
				if (!(gameObject == null))
				{
					OIBoneInfo value = null;
					if (!_ociChar.oiCharInfo.bones.TryGetValue(item.Key, out value))
					{
						value = new OIBoneInfo(Studio.GetNewIndex());
						_ociChar.oiCharInfo.bones.Add(item.Key, value);
					}
					switch (item.Value.group)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						value.group = (OIBoneInfo.BoneGroup)((1 << item.Value.group) | 1);
						break;
					case 7:
					case 8:
					case 9:
						value.group = OIBoneInfo.BoneGroup.Hair;
						break;
					case 10:
						value.group = OIBoneInfo.BoneGroup.Neck;
						break;
					case 11:
					case 12:
						value.group = OIBoneInfo.BoneGroup.Breast;
						break;
					case 13:
						value.group = OIBoneInfo.BoneGroup.Skirt;
						break;
					default:
						value.group = (OIBoneInfo.BoneGroup)(1 << item.Value.group);
						break;
					}
					value.level = item.Value.level;
					GuideObject guideObject = AddBoneGuide(gameObject.transform, value.dicKey, _ociChar.guideObject, item.Value.name);
					_ociChar.listBones.Add(new OCIChar.BoneInfo(guideObject, value, item.Key));
					guideObject.SetActive(false);
					if (item.Value.no == 65)
					{
						_ociChar.transSon = gameObject.transform;
					}
				}
			}
			_ociChar.UpdateFKColor(FKCtrl.parts);
		}

		private static void TransformLoop(Transform _src, List<Transform> _list)
		{
			if (!(_src == null))
			{
				_list.Add(_src);
				for (int i = 0; i < _src.childCount; i++)
				{
					TransformLoop(_src.GetChild(i), _list);
				}
			}
		}

		public static void InitHairBone(OCIChar _ociChar, Dictionary<int, Info.BoneInfo> _dicBoneInfo)
		{
			GameObject referenceInfo = _ociChar.charReference.GetReferenceInfo(ChaReference.RefObjKey.HairParent);
			foreach (KeyValuePair<int, Info.BoneInfo> item in _dicBoneInfo.Where((KeyValuePair<int, Info.BoneInfo> b) => MathfEx.RangeEqualOn(7, b.Value.group, 9)))
			{
				GameObject gameObject = referenceInfo.transform.FindLoop(item.Value.bone);
				if (!(gameObject == null))
				{
					OIBoneInfo value = null;
					if (!_ociChar.oiCharInfo.bones.TryGetValue(item.Key, out value))
					{
						value = new OIBoneInfo(Studio.GetNewIndex());
						_ociChar.oiCharInfo.bones.Add(item.Key, value);
					}
					value.group = OIBoneInfo.BoneGroup.Hair;
					value.level = item.Value.level;
					GuideObject guideObject = AddBoneGuide(gameObject.transform, value.dicKey, _ociChar.guideObject, item.Value.name);
					_ociChar.listBones.Add(new OCIChar.BoneInfo(guideObject, value, item.Key));
					guideObject.SetActive(false);
				}
			}
		}

		private static GuideObject AddBoneGuide(Transform _target, int _dicKey, GuideObject _parent, string _name)
		{
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(_target, _dicKey);
			guideObject.enablePos = false;
			guideObject.enableScale = false;
			guideObject.enableMaluti = false;
			guideObject.calcScale = false;
			guideObject.scaleRate = 0.5f;
			guideObject.scaleRot = 0.025f;
			guideObject.scaleSelect = 0.05f;
			guideObject.parentGuide = _parent;
			return guideObject;
		}

		public static void InitIKTarget(OCIChar _ociChar, bool _addInfo)
		{
			IKSolverFullBodyBiped solver = _ociChar.finalIK.solver;
			BipedReferences references = _ociChar.finalIK.references;
			_ociChar.ikCtrl = _ociChar.preparation.IKCtrl;
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 0, solver.bodyEffector, false, references.pelvis);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 1, solver.leftShoulderEffector, false, references.leftUpperArm);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 2, solver.leftArmChain, false, references.leftForearm);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 3, solver.leftHandEffector, true, references.leftHand);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 4, solver.rightShoulderEffector, false, references.rightUpperArm);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 5, solver.rightArmChain, false, references.rightForearm);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 6, solver.rightHandEffector, true, references.rightHand);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 7, solver.leftThighEffector, false, references.leftThigh);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 8, solver.leftLegChain, false, references.leftCalf);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 9, solver.leftFootEffector, true, references.leftFoot);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 10, solver.rightThighEffector, false, references.rightThigh);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 11, solver.rightLegChain, false, references.rightCalf);
			AddIKTarget(_ociChar, _ociChar.ikCtrl, 12, solver.rightFootEffector, true, references.rightFoot);
			if (_addInfo)
			{
				_ociChar.ikCtrl.InitTarget();
			}
		}

		public static void InitAccessoryPoint(OCIChar _ociChar)
		{
			ExcelData accessoryPointGroup = Singleton<Info>.Instance.accessoryPointGroup;
			int count = accessoryPointGroup.list.Count;
			Dictionary<int, TreeNodeObject> dictionary = new Dictionary<int, TreeNodeObject>();
			for (int i = 1; i < count; i++)
			{
				ExcelData.Param param = accessoryPointGroup.list[i];
				int key = int.Parse(param.list[0]);
				string arg = param.list[1];
				TreeNodeObject treeNodeObject = Studio.AddNode(string.Format("グループ : {0}", arg), _ociChar.treeNodeObject);
				treeNodeObject.treeState = ((!_ociChar.oiCharInfo.dicAccessGroup.ContainsKey(key)) ? TreeNodeObject.TreeState.Close : _ociChar.oiCharInfo.dicAccessGroup[key]);
				treeNodeObject.enableChangeParent = false;
				treeNodeObject.enableDelete = false;
				treeNodeObject.enableCopy = false;
				dictionary.Add(key, treeNodeObject);
				_ociChar.dicAccessPoint.Add(key, new OCIChar.AccessPointInfo(treeNodeObject));
			}
			accessoryPointGroup = Singleton<Info>.Instance.accessoryPoint;
			count = accessoryPointGroup.list.Count;
			for (int j = 1; j < count; j++)
			{
				ExcelData.Param param2 = accessoryPointGroup.list[j];
				int num = int.Parse(param2.list[0]);
				int key2 = int.Parse(param2.list[1]);
				string arg2 = param2.list[3];
				TreeNodeObject parent = dictionary[key2];
				TreeNodeObject treeNodeObject2 = Studio.AddNode(string.Format("部位 : {0}", arg2), parent);
				treeNodeObject2.treeState = ((!_ociChar.oiCharInfo.dicAccessNo.ContainsKey(num)) ? TreeNodeObject.TreeState.Close : _ociChar.oiCharInfo.dicAccessNo[num]);
				treeNodeObject2.enableChangeParent = false;
				treeNodeObject2.enableDelete = false;
				treeNodeObject2.enableCopy = false;
				treeNodeObject2.baseColor = Utility.ConvertColor(204, 128, 164);
				treeNodeObject2.colorSelect = treeNodeObject2.baseColor;
				_ociChar.dicAccessoryPoint.Add(treeNodeObject2, num);
				OCIChar.AccessPointInfo value = null;
				if (_ociChar.dicAccessPoint.TryGetValue(key2, out value))
				{
					value.child.Add(num, treeNodeObject2);
				}
			}
			foreach (KeyValuePair<int, TreeNodeObject> item in dictionary)
			{
				item.Value.enableAddChild = false;
			}
			Singleton<Studio>.Instance.treeNodeCtrl.RefreshHierachy();
		}

		public static void LoadChild(List<ObjectInfo> _child, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			foreach (ObjectInfo item in _child)
			{
				LoadChild(item, _parent, _parentNode);
			}
		}

		public static void LoadChild(Dictionary<int, ObjectInfo> _child, ObjectCtrlInfo _parent = null, TreeNodeObject _parentNode = null)
		{
			foreach (KeyValuePair<int, ObjectInfo> item in _child)
			{
				LoadChild(item.Value, _parent, _parentNode);
			}
		}

		public static void LoadChild(ObjectInfo _child, ObjectCtrlInfo _parent = null, TreeNodeObject _parentNode = null)
		{
			switch (_child.kind)
			{
			case 0:
			{
				OICharInfo oICharInfo = _child as OICharInfo;
				if (oICharInfo.sex == 1)
				{
					AddObjectFemale.Load(oICharInfo, _parent, _parentNode);
				}
				else
				{
					AddObjectMale.Load(oICharInfo, _parent, _parentNode);
				}
				break;
			}
			case 1:
				AddObjectItem.Load(_child as OIItemInfo, _parent, _parentNode);
				break;
			case 2:
				AddObjectLight.Load(_child as OILightInfo, _parent, _parentNode);
				break;
			case 3:
				AddObjectFolder.Load(_child as OIFolderInfo, _parent, _parentNode);
				break;
			case 4:
				AddObjectRoute.Load(_child as OIRouteInfo, _parent, _parentNode);
				break;
			case 5:
				AddObjectCamera.Load(_child as OICameraInfo, _parent, _parentNode);
				break;
			}
		}

		public static void InitHandAnime(OCIChar _ociChar)
		{
			for (int i = 0; i < 2; i++)
			{
				_ociChar.handAnimeCtrl[i] = _ociChar.preparation.handAnimeCtrl[i];
				_ociChar.handAnimeCtrl[i].Init(_ociChar.oiCharInfo.sex);
			}
		}

		public static void InitLookAt(OCIChar _ociChar)
		{
			bool flag = _ociChar.oiCharInfo.lookAtTarget == null;
			if (flag)
			{
				_ociChar.oiCharInfo.lookAtTarget = new LookAtTargetInfo(Studio.GetNewIndex());
			}
			Transform lookAtTarget = _ociChar.preparation.lookAtTarget;
			if (flag)
			{
				_ociChar.oiCharInfo.lookAtTarget.changeAmount.pos = lookAtTarget.localPosition;
			}
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(lookAtTarget, _ociChar.oiCharInfo.lookAtTarget.dicKey);
			guideObject.enableRot = false;
			guideObject.enableScale = false;
			guideObject.enableMaluti = false;
			guideObject.scaleRate = 0.5f;
			guideObject.scaleSelect = 0.25f;
			guideObject.parentGuide = _ociChar.guideObject;
			guideObject.changeAmount.OnChange();
			guideObject.mode = GuideObject.Mode.World;
			guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
			_ociChar.lookAtInfo = new OCIChar.LookAtInfo(guideObject, _ociChar.oiCharInfo.lookAtTarget);
			_ociChar.lookAtInfo.active = false;
		}

		public static void UpdateState(OCIChar _ociChar, ChaFileStatus _status)
		{
			ChaFileStatus charFileStatus = _ociChar.charFileStatus;
			charFileStatus.Copy(_status);
			_ociChar.SetCoordinateInfo((ChaFileDefine.CoordinateType)charFileStatus.coordinateType, true);
			for (int i = 0; i < charFileStatus.clothesState.Length; i++)
			{
				_ociChar.SetClothesState(i, charFileStatus.clothesState[i]);
			}
			for (int j = 0; j < charFileStatus.showAccessory.Length; j++)
			{
				_ociChar.ShowAccessory(j, charFileStatus.showAccessory[j]);
			}
			_ociChar.charInfo.ChangeEyebrowPtn(charFileStatus.eyebrowPtn);
			_ociChar.charInfo.ChangeEyesPtn(charFileStatus.eyesPtn);
			_ociChar.ChangeBlink(charFileStatus.eyesBlink);
			_ociChar.ChangeEyesOpen(charFileStatus.eyesOpenMax);
			_ociChar.charInfo.ChangeMouthPtn(charFileStatus.mouthPtn);
			_ociChar.ChangeMouthOpen(_ociChar.oiCharInfo.mouthOpen);
			_ociChar.ChangeHandAnime(0, _ociChar.oiCharInfo.handPtn[0]);
			_ociChar.ChangeHandAnime(1, _ociChar.oiCharInfo.handPtn[1]);
			_ociChar.ChangeLookEyesPtn(charFileStatus.eyesLookPtn, true);
			if (_ociChar.oiCharInfo.eyesByteData != null)
			{
				using (MemoryStream input = new MemoryStream(_ociChar.oiCharInfo.eyesByteData))
				{
					using (BinaryReader reader = new BinaryReader(input))
					{
						_ociChar.charInfo.eyeLookCtrl.eyeLookScript.LoadAngle(reader, Singleton<Studio>.Instance.sceneInfo.dataVersion);
					}
				}
				_ociChar.oiCharInfo.eyesByteData = null;
			}
			if (_ociChar.oiCharInfo.neckByteData != null)
			{
				using (MemoryStream input2 = new MemoryStream(_ociChar.oiCharInfo.neckByteData))
				{
					using (BinaryReader reader2 = new BinaryReader(input2))
					{
						_ociChar.neckLookCtrl.LoadNeckLookCtrl(reader2);
					}
				}
				_ociChar.oiCharInfo.neckByteData = null;
			}
			_ociChar.ChangeLookNeckPtn(charFileStatus.neckLookPtn);
			for (int k = 0; k < 5; k++)
			{
				_ociChar.SetSiruFlags((ChaFileDefine.SiruParts)k, _ociChar.oiCharInfo.siru[k]);
			}
			if (_ociChar.sex == 1)
			{
				_ociChar.charInfo.ChangeHohoAkaRate(charFileStatus.hohoAkaRate);
			}
			_ociChar.SetVisibleSon(charFileStatus.visibleSonAlways);
		}

		private static OCIChar.IKInfo AddIKTarget(OCIChar _ociChar, IKCtrl _ikCtrl, int _no, IKEffector _effector, bool _usedRot, Transform _bone)
		{
			OCIChar.IKInfo iKInfo = AddIKTarget(_ociChar, _ikCtrl, _no, _effector.target, _usedRot, _bone, true);
			_effector.positionWeight = 1f;
			_effector.rotationWeight = ((!_usedRot) ? 0f : 1f);
			_effector.target = iKInfo.targetObject;
			return iKInfo;
		}

		private static OCIChar.IKInfo AddIKTarget(OCIChar _ociChar, IKCtrl _ikCtrl, int _no, FBIKChain _chain, bool _usedRot, Transform _bone)
		{
			OCIChar.IKInfo iKInfo = AddIKTarget(_ociChar, _ikCtrl, _no, _chain.bendConstraint.bendGoal, _usedRot, _bone, false);
			_chain.bendConstraint.weight = 1f;
			_chain.bendConstraint.bendGoal = iKInfo.targetObject;
			return iKInfo;
		}

		private static OCIChar.IKInfo AddIKTarget(OCIChar _ociChar, IKCtrl _ikCtrl, int _no, Transform _target, bool _usedRot, Transform _bone, bool _isRed)
		{
			OIIKTargetInfo value = null;
			bool flag = !_ociChar.oiCharInfo.ikTarget.TryGetValue(_no, out value);
			if (flag)
			{
				value = new OIIKTargetInfo(Studio.GetNewIndex());
				_ociChar.oiCharInfo.ikTarget.Add(_no, value);
			}
			switch ((OICharInfo.IKTargetEN)_no)
			{
			case OICharInfo.IKTargetEN.Body:
				value.group = OIBoneInfo.BoneGroup.Body;
				break;
			case OICharInfo.IKTargetEN.LeftShoulder:
			case OICharInfo.IKTargetEN.LeftArmChain:
			case OICharInfo.IKTargetEN.LeftHand:
				value.group = OIBoneInfo.BoneGroup.LeftArm;
				break;
			case OICharInfo.IKTargetEN.RightShoulder:
			case OICharInfo.IKTargetEN.RightArmChain:
			case OICharInfo.IKTargetEN.RightHand:
				value.group = OIBoneInfo.BoneGroup.RightArm;
				break;
			case OICharInfo.IKTargetEN.LeftThigh:
			case OICharInfo.IKTargetEN.LeftLegChain:
			case OICharInfo.IKTargetEN.LeftFoot:
				value.group = OIBoneInfo.BoneGroup.LeftLeg;
				break;
			case OICharInfo.IKTargetEN.RightThigh:
			case OICharInfo.IKTargetEN.RightLegChain:
			case OICharInfo.IKTargetEN.RightFoot:
				value.group = OIBoneInfo.BoneGroup.RightLeg;
				break;
			}
			GameObject gameObject = new GameObject(_target.name + "(work)");
			gameObject.transform.SetParent(_ociChar.charInfo.transform);
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, value.dicKey);
			guideObject.mode = GuideObject.Mode.LocalIK;
			guideObject.enableRot = _usedRot;
			guideObject.enableScale = false;
			guideObject.enableMaluti = false;
			guideObject.calcScale = false;
			guideObject.scaleRate = 0.5f;
			guideObject.scaleRot = 0.05f;
			guideObject.scaleSelect = 0.1f;
			guideObject.parentGuide = _ociChar.guideObject;
			guideObject.guideSelect.color = ((!_isRed) ? Color.blue : Color.red);
			guideObject.moveCalc = GuideMove.MoveCalc.TYPE3;
			OCIChar.IKInfo iKInfo = new OCIChar.IKInfo(guideObject, value, _target, gameObject.transform, _bone);
			if (!flag)
			{
				value.changeAmount.OnChange();
			}
			_ikCtrl.addIKInfo = iKInfo;
			_ociChar.listIKTarget.Add(iKInfo);
			guideObject.SetActive(false);
			return iKInfo;
		}
	}
}
