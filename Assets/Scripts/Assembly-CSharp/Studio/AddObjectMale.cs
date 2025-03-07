using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using RootMotion.FinalIK;
using UnityEngine;

namespace Studio
{
	public static class AddObjectMale
	{
		public static OCICharMale Add(string _path)
		{
			ChaControl chaControl = Singleton<Character>.Instance.CreateMale(Singleton<Scene>.Instance.commonSpace, -1);
			chaControl.chaFile.LoadCharaFile(_path, byte.MaxValue, true);
			OICharInfo info = new OICharInfo(chaControl.chaFile, Studio.GetNewIndex());
			return Add(chaControl, info, null, null, true, Studio.optionSystem.initialPosition);
		}

		public static OCICharMale Load(OICharInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			OCICharMale oCICharMale = Add(Singleton<Character>.Instance.CreateMale(Singleton<Scene>.Instance.commonSpace, -1, _info.charFile), _info, _parent, _parentNode, false, -1);
			foreach (KeyValuePair<int, List<ObjectInfo>> v in _info.child)
			{
				AddObjectAssist.LoadChild(v.Value, oCICharMale, oCICharMale.dicAccessoryPoint.First((KeyValuePair<TreeNodeObject, int> x) => x.Value == v.Key).Key);
			}
			return oCICharMale;
		}

		private static OCICharMale Add(ChaControl _male, OICharInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
		{
			OCICharMale oCICharMale = new OCICharMale();
			ChaFileStatus chaFileStatus = new ChaFileStatus();
			chaFileStatus.Copy(_male.fileStatus);
			_male.Load(true);
			_male.InitializeExpression();
			oCICharMale.charInfo = _male;
			oCICharMale.charReference = _male;
			oCICharMale.preparation = _male.objAnim.GetComponent<Preparation>();
			oCICharMale.finalIK = oCICharMale.preparation.fullBodyIK;
			oCICharMale.objectInfo = _info;
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(_male.transform, _info.dicKey);
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.isActiveFunc = (GuideObject.IsActiveFunc)Delegate.Combine(guideObject.isActiveFunc, new GuideObject.IsActiveFunc(oCICharMale.OnSelect));
			guideObject.SetVisibleCenter(true);
			oCICharMale.guideObject = guideObject;
			oCICharMale.optionItemCtrl = _male.gameObject.AddComponent<OptionItemCtrl>();
			oCICharMale.optionItemCtrl.animator = _male.animBody;
			oCICharMale.optionItemCtrl.oiCharInfo = _info;
			ChangeAmount changeAmount = _info.changeAmount;
			changeAmount.onChangeScale = (Action<Vector3>)Delegate.Combine(changeAmount.onChangeScale, new Action<Vector3>(oCICharMale.optionItemCtrl.ChangeScale));
			oCICharMale.charAnimeCtrl = oCICharMale.preparation.charAnimeCtrl;
			oCICharMale.charAnimeCtrl.oiCharInfo = _info;
			oCICharMale.yureCtrl = new YureCtrl();
			oCICharMale.yureCtrl.Init(_male);
			AddObjectAssist.InitHandAnime(oCICharMale);
			int group = _info.animeInfo.group;
			int category = _info.animeInfo.category;
			int no = _info.animeInfo.no;
			float animeNormalizedTime = _info.animeNormalizedTime;
			oCICharMale.LoadAnime(0, 0, 1);
			_male.animBody.Update(0f);
			_info.animeInfo.group = group;
			_info.animeInfo.category = category;
			_info.animeInfo.no = no;
			_info.animeNormalizedTime = animeNormalizedTime;
			IKSolver iKSolver = oCICharMale.finalIK.GetIKSolver();
			if (!iKSolver.initiated)
			{
				iKSolver.Initiate(oCICharMale.finalIK.transform);
			}
			if (_addInfo)
			{
				Studio.AddInfo(_info, oCICharMale);
			}
			else
			{
				Studio.AddObjectCtrlInfo(oCICharMale);
			}
			TreeNodeObject parent = ((_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject));
			TreeNodeObject treeNodeObject = Studio.AddNode(_info.charFile.parameter.fullname, parent);
			treeNodeObject.enableChangeParent = true;
			treeNodeObject.treeState = _info.treeState;
			treeNodeObject.onVisible = (TreeNodeObject.OnVisibleFunc)Delegate.Combine(treeNodeObject.onVisible, new TreeNodeObject.OnVisibleFunc(oCICharMale.OnVisible));
			treeNodeObject.enableVisible = true;
			treeNodeObject.visible = _info.visible;
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			oCICharMale.treeNodeObject = treeNodeObject;
			AddObjectAssist.InitBone(oCICharMale, _male.objBodyBone.transform, Singleton<Info>.Instance.dicBoneInfo);
			AddObjectAssist.InitIKTarget(oCICharMale, _addInfo);
			AddObjectAssist.InitLookAt(oCICharMale);
			AddObjectAssist.InitAccessoryPoint(oCICharMale);
			oCICharMale.voiceCtrl.ociChar = oCICharMale;
			List<DynamicBone> list = new List<DynamicBone>();
			GameObject[] objHair = _male.objHair;
			foreach (GameObject gameObject in objHair)
			{
				list.AddRange(gameObject.GetComponents<DynamicBone>());
			}
			oCICharMale.InitKinematic(_male.gameObject, oCICharMale.finalIK, _male.neckLookCtrl, list.Where((DynamicBone v) => v != null).ToArray(), null);
			treeNodeObject.enableAddChild = false;
			if (_initialPosition == 1)
			{
				_info.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			_info.changeAmount.OnChange();
			treeNodeObject.treeState = TreeNodeObject.TreeState.Close;
			Studio.AddCtrlInfo(oCICharMale);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, oCICharMale);
			}
			oCICharMale.LoadAnime(_info.animeInfo.group, _info.animeInfo.category, _info.animeInfo.no, _info.animeNormalizedTime);
			oCICharMale.ActiveKinematicMode(OICharInfo.KinematicMode.IK, _info.enableIK, true);
			for (int k = 0; k < 5; k++)
			{
				oCICharMale.ActiveIK((OIBoneInfo.BoneGroup)(1 << k), _info.activeIK[k]);
			}
			foreach (var item in FKCtrl.parts.Select((OIBoneInfo.BoneGroup p, int i) => new { p, i }))
			{
				oCICharMale.ActiveFK(item.p, oCICharMale.oiCharInfo.activeFK[item.i], oCICharMale.oiCharInfo.activeFK[item.i]);
			}
			oCICharMale.ActiveKinematicMode(OICharInfo.KinematicMode.FK, _info.enableFK, true);
			for (int l = 0; l < _info.expression.Length; l++)
			{
				oCICharMale.charInfo.EnableExpressionCategory(l, _info.expression[l]);
			}
			oCICharMale.animeSpeed = oCICharMale.animeSpeed;
			oCICharMale.animeOptionParam1 = oCICharMale.animeOptionParam1;
			oCICharMale.animeOptionParam2 = oCICharMale.animeOptionParam2;
			chaFileStatus.visibleSonAlways = _info.visibleSon;
			oCICharMale.SetSonLength(_info.sonLength);
			oCICharMale.SetVisibleSimple(_info.visibleSimple);
			oCICharMale.SetSimpleColor(_info.simpleColor);
			AddObjectAssist.UpdateState(oCICharMale, chaFileStatus);
			return oCICharMale;
		}
	}
}
