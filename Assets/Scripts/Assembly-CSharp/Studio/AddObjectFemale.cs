using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using RootMotion.FinalIK;
using UnityEngine;

namespace Studio
{
	public static class AddObjectFemale
	{
		public class NecessaryInfo
		{
			public OCICharFemale ocicf;

			public ChaControl chaControl;

			public OICharInfo oICharInfo;

			public ObjectCtrlInfo parent;

			public TreeNodeObject parentNode;

			public bool addInfo = true;

			public string path { get; private set; }

			public Info.WaitTime waitTime { get; private set; }

			public bool isOver
			{
				get
				{
					return waitTime.isOver;
				}
			}

			public NecessaryInfo(string _path)
			{
				path = _path;
				waitTime = new Info.WaitTime();
			}

			public void Next()
			{
				waitTime.Next();
			}
		}

		public static OCICharFemale Add(string _path)
		{
			ChaControl chaControl = Singleton<Character>.Instance.CreateFemale(Singleton<Scene>.Instance.commonSpace, -1);
			chaControl.chaFile.LoadCharaFile(_path, byte.MaxValue, true);
			OICharInfo info = new OICharInfo(chaControl.chaFile, Studio.GetNewIndex());
			return Add(chaControl, info, null, null, true, Studio.optionSystem.initialPosition);
		}

		public static OCICharFemale Load(OICharInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			ChaControl female = Singleton<Character>.Instance.CreateFemale(Singleton<Scene>.Instance.commonSpace, -1, _info.charFile);
			OCICharFemale oCICharFemale = Add(female, _info, _parent, _parentNode, false, -1);
			foreach (KeyValuePair<int, List<ObjectInfo>> v in _info.child)
			{
				AddObjectAssist.LoadChild(v.Value, oCICharFemale, oCICharFemale.dicAccessoryPoint.First((KeyValuePair<TreeNodeObject, int> x) => x.Value == v.Key).Key);
			}
			return oCICharFemale;
		}

		private static OCICharFemale Add(ChaControl _female, OICharInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
		{
			OCICharFemale oCICharFemale = new OCICharFemale();
			ChaFileStatus chaFileStatus = new ChaFileStatus();
			chaFileStatus.Copy(_female.fileStatus);
			_female.Load(true);
			_female.InitializeExpression();
			oCICharFemale.charInfo = _female;
			oCICharFemale.charReference = _female;
			oCICharFemale.preparation = _female.objAnim.GetComponent<Preparation>();
			oCICharFemale.finalIK = oCICharFemale.preparation.fullBodyIK;
			oCICharFemale.charInfo.hideMoz = false;
			oCICharFemale.objectInfo = _info;
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(_female.transform, _info.dicKey);
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.isActiveFunc = (GuideObject.IsActiveFunc)Delegate.Combine(guideObject.isActiveFunc, new GuideObject.IsActiveFunc(oCICharFemale.OnSelect));
			guideObject.SetVisibleCenter(true);
			oCICharFemale.guideObject = guideObject;
			oCICharFemale.optionItemCtrl = _female.gameObject.AddComponent<OptionItemCtrl>();
			oCICharFemale.optionItemCtrl.animator = _female.animBody;
			oCICharFemale.optionItemCtrl.oiCharInfo = _info;
			ChangeAmount changeAmount = _info.changeAmount;
			changeAmount.onChangeScale = (Action<Vector3>)Delegate.Combine(changeAmount.onChangeScale, new Action<Vector3>(oCICharFemale.optionItemCtrl.ChangeScale));
			oCICharFemale.charAnimeCtrl = oCICharFemale.preparation.charAnimeCtrl;
			oCICharFemale.charAnimeCtrl.oiCharInfo = _info;
			oCICharFemale.yureCtrl = new YureCtrl();
			oCICharFemale.yureCtrl.Init(_female);
			AddObjectAssist.InitHandAnime(oCICharFemale);
			if (_info.animeInfo.group == 0 && _info.animeInfo.category == 2 && _info.animeInfo.no == 11)
			{
				int group = _info.animeInfo.group;
				int category = _info.animeInfo.category;
				int no = _info.animeInfo.no;
				float animeNormalizedTime = _info.animeNormalizedTime;
				oCICharFemale.LoadAnime(0, 1, 0);
				_female.animBody.Update(0f);
				_info.animeInfo.group = group;
				_info.animeInfo.category = category;
				_info.animeInfo.no = no;
				_info.animeNormalizedTime = animeNormalizedTime;
			}
			IKSolver iKSolver = oCICharFemale.finalIK.GetIKSolver();
			if (!iKSolver.initiated)
			{
				iKSolver.Initiate(oCICharFemale.finalIK.transform);
			}
			if (_addInfo)
			{
				Studio.AddInfo(_info, oCICharFemale);
			}
			else
			{
				Studio.AddObjectCtrlInfo(oCICharFemale);
			}
			TreeNodeObject parent = ((_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject));
			TreeNodeObject treeNodeObject = Studio.AddNode(_info.charFile.parameter.fullname, parent);
			treeNodeObject.enableChangeParent = true;
			treeNodeObject.treeState = _info.treeState;
			treeNodeObject.onVisible = (TreeNodeObject.OnVisibleFunc)Delegate.Combine(treeNodeObject.onVisible, new TreeNodeObject.OnVisibleFunc(oCICharFemale.OnVisible));
			treeNodeObject.enableVisible = true;
			treeNodeObject.visible = _info.visible;
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			oCICharFemale.treeNodeObject = treeNodeObject;
			_info.changeAmount.OnChange();
			AddObjectAssist.InitBone(oCICharFemale, _female.objBodyBone.transform, Singleton<Info>.Instance.dicBoneInfo);
			AddObjectAssist.InitIKTarget(oCICharFemale, _addInfo);
			AddObjectAssist.InitLookAt(oCICharFemale);
			AddObjectAssist.InitAccessoryPoint(oCICharFemale);
			oCICharFemale.voiceCtrl.ociChar = oCICharFemale;
			oCICharFemale.InitKinematic(_female.gameObject, oCICharFemale.finalIK, _female.neckLookCtrl, GetHairDynamic(_female.objHair), GetSkirtDynamic(_female.objClothes));
			treeNodeObject.enableAddChild = false;
			Studio.AddCtrlInfo(oCICharFemale);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, oCICharFemale);
			}
			if (_initialPosition == 1)
			{
				_info.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			oCICharFemale.LoadAnime(_info.animeInfo.group, _info.animeInfo.category, _info.animeInfo.no, _info.animeNormalizedTime);
			for (int j = 0; j < 5; j++)
			{
				oCICharFemale.ActiveIK((OIBoneInfo.BoneGroup)(1 << j), _info.activeIK[j]);
			}
			oCICharFemale.ActiveKinematicMode(OICharInfo.KinematicMode.IK, _info.enableIK, true);
			foreach (var item in FKCtrl.parts.Select((OIBoneInfo.BoneGroup p, int i) => new { p, i }))
			{
				oCICharFemale.ActiveFK(item.p, oCICharFemale.oiCharInfo.activeFK[item.i]);
			}
			oCICharFemale.ActiveKinematicMode(OICharInfo.KinematicMode.FK, _info.enableFK, true);
			for (int k = 0; k < _info.expression.Length; k++)
			{
				oCICharFemale.charInfo.EnableExpressionCategory(k, _info.expression[k]);
			}
			oCICharFemale.animeSpeed = oCICharFemale.animeSpeed;
			oCICharFemale.animeOptionParam1 = oCICharFemale.animeOptionParam1;
			oCICharFemale.animeOptionParam2 = oCICharFemale.animeOptionParam2;
			byte[] siruLv = _female.fileStatus.siruLv;
			for (int l = 0; l < siruLv.Length; l++)
			{
				siruLv[l] = 0;
			}
			chaFileStatus.visibleSonAlways = _info.visibleSon;
			oCICharFemale.SetSonLength(_info.sonLength);
			AddObjectAssist.UpdateState(oCICharFemale, chaFileStatus);
			return oCICharFemale;
		}

		public static DynamicBone[] GetHairDynamic(GameObject[] _objHair)
		{
			if (_objHair.IsNullOrEmpty())
			{
				return null;
			}
			List<DynamicBone> list = new List<DynamicBone>();
			foreach (GameObject item in _objHair.Where((GameObject o) => o != null))
			{
				list.AddRange(item.GetComponents<DynamicBone>());
			}
			return list.Where((DynamicBone v) => v != null).ToArray();
		}

		public static DynamicBone[] GetSkirtDynamic(GameObject[] _objClothes)
		{
			if (_objClothes.IsNullOrEmpty())
			{
				return null;
			}
			string[] target = (from v in Singleton<Info>.Instance.dicBoneInfo
				where v.Value.@group == 13
				select v.Value.bone).ToArray();
			DynamicBone[] array = null;
			List<DynamicBone> list = new List<DynamicBone>();
			int[] array2 = new int[2] { 0, 1 };
			for (int i = 0; i < array2.Length; i++)
			{
				array = GetSkirtDynamic(_objClothes[array2[i]], target);
				if (!array.IsNullOrEmpty())
				{
					list.AddRange(array);
				}
			}
			return list.ToArray();
		}

		private static DynamicBone[] GetSkirtDynamic(GameObject _object, string[] _target)
		{
			if (_object == null)
			{
				return null;
			}
			DynamicBone[] componentsInChildren = _object.GetComponentsInChildren<DynamicBone>();
			return componentsInChildren.Where((DynamicBone v) => CheckNameLoop(v.m_Root, _target)).ToArray();
		}

		private static bool CheckNameLoop(Transform _transform, string[] _target)
		{
			if (_transform == null)
			{
				return false;
			}
			if (_target.Contains(_transform.name))
			{
				return true;
			}
			if (_transform.childCount == 0)
			{
				return false;
			}
			for (int i = 0; i < _transform.childCount; i++)
			{
				if (CheckNameLoop(_transform.GetChild(i), _target))
				{
					return true;
				}
			}
			return false;
		}

		public static IEnumerator AddCoroutine(NecessaryInfo _necessary)
		{
			ChaControl female = Singleton<Character>.Instance.CreateFemale(Singleton<Scene>.Instance.commonSpace, -1);
			female.chaFile.LoadCharaFile(_necessary.path, byte.MaxValue, true);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			_necessary.oICharInfo = new OICharInfo(female.chaFile, Studio.GetNewIndex());
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			_necessary.chaControl = female;
			yield return AddCoroutine(_necessary, null, null, true);
		}

		private static IEnumerator AddCoroutine(NecessaryInfo _necessary, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo)
		{
			OCICharFemale ocicf = new OCICharFemale();
			ChaFileStatus status = new ChaFileStatus();
			status.Copy(_necessary.chaControl.fileStatus);
			yield return _necessary.chaControl.LoadAsync(true);
			_necessary.chaControl.SetActiveTop(true);
			_necessary.chaControl.InitializeExpression();
			ocicf.charInfo = _necessary.chaControl;
			ocicf.charReference = _necessary.chaControl;
			ocicf.preparation = _necessary.chaControl.objAnim.GetComponent<Preparation>();
			ocicf.finalIK = ocicf.preparation.fullBodyIK;
			ocicf.objectInfo = _necessary.oICharInfo;
			GuideObject go = Singleton<GuideObjectManager>.Instance.Add(_necessary.chaControl.transform, _necessary.oICharInfo.dicKey);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			go.scaleSelect = 0.1f;
			go.scaleRot = 0.05f;
			go.isActiveFunc = (GuideObject.IsActiveFunc)Delegate.Combine(go.isActiveFunc, new GuideObject.IsActiveFunc(ocicf.OnSelect));
			ocicf.guideObject = go;
			ocicf.charAnimeCtrl = ocicf.preparation.charAnimeCtrl;
			ocicf.charAnimeCtrl.oiCharInfo = _necessary.oICharInfo;
			AddObjectAssist.InitHandAnime(ocicf);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			if (_necessary.oICharInfo.animeInfo.group == 0 && _necessary.oICharInfo.animeInfo.category == 2 && _necessary.oICharInfo.animeInfo.no == 11)
			{
				int group = _necessary.oICharInfo.animeInfo.group;
				int category = _necessary.oICharInfo.animeInfo.category;
				int no = _necessary.oICharInfo.animeInfo.no;
				float animeNormalizedTime = _necessary.oICharInfo.animeNormalizedTime;
				ocicf.LoadAnime(0, 1, 0);
				_necessary.chaControl.animBody.Update(0f);
				_necessary.oICharInfo.animeInfo.group = group;
				_necessary.oICharInfo.animeInfo.category = category;
				_necessary.oICharInfo.animeInfo.no = no;
				_necessary.oICharInfo.animeNormalizedTime = animeNormalizedTime;
			}
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			if (_addInfo)
			{
				Studio.AddInfo(_necessary.oICharInfo, ocicf);
			}
			else
			{
				Studio.AddObjectCtrlInfo(ocicf);
			}
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			TreeNodeObject tno = Studio.AddNode(_parent: (_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject), _name: _necessary.oICharInfo.charFile.parameter.fullname);
			tno.enableChangeParent = true;
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			tno.treeState = _necessary.oICharInfo.treeState;
			tno.onVisible = (TreeNodeObject.OnVisibleFunc)Delegate.Combine(tno.onVisible, new TreeNodeObject.OnVisibleFunc(ocicf.OnVisible));
			tno.enableVisible = true;
			tno.visible = _necessary.oICharInfo.visible;
			go.guideSelect.treeNodeObject = tno;
			ocicf.treeNodeObject = tno;
			_necessary.oICharInfo.changeAmount.OnChange();
			AddObjectAssist.InitBone(ocicf, _necessary.chaControl.objBodyBone.transform, Singleton<Info>.Instance.dicBoneInfo);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			AddObjectAssist.InitIKTarget(ocicf, _addInfo);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			AddObjectAssist.InitLookAt(ocicf);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			AddObjectAssist.InitAccessoryPoint(ocicf);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			ocicf.voiceCtrl.ociChar = ocicf;
			ocicf.InitKinematic(_necessary.chaControl.gameObject, ocicf.finalIK, _necessary.chaControl.neckLookCtrl, GetHairDynamic(_necessary.chaControl.objHair), GetSkirtDynamic(_necessary.chaControl.objClothes));
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			tno.enableAddChild = false;
			Studio.AddCtrlInfo(ocicf);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, ocicf);
			}
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			int initialPosition = Studio.optionSystem.initialPosition;
			if (initialPosition == 1)
			{
				_necessary.oICharInfo.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			ocicf.LoadAnime(_necessary.oICharInfo.animeInfo.group, _necessary.oICharInfo.animeInfo.category, _necessary.oICharInfo.animeInfo.no, _necessary.oICharInfo.animeNormalizedTime);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			for (int j = 0; j < 5; j++)
			{
				ocicf.ActiveIK((OIBoneInfo.BoneGroup)(1 << j), _necessary.oICharInfo.activeIK[j]);
			}
			ocicf.ActiveKinematicMode(OICharInfo.KinematicMode.IK, _necessary.oICharInfo.enableIK, true);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			foreach (var item in FKCtrl.parts.Select((OIBoneInfo.BoneGroup p, int i) => new { p, i }))
			{
				ocicf.ActiveFK(item.p, ocicf.oiCharInfo.activeFK[item.i], ocicf.oiCharInfo.activeFK[item.i]);
			}
			ocicf.ActiveKinematicMode(OICharInfo.KinematicMode.FK, _necessary.oICharInfo.enableFK, true);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			for (int k = 0; k < _necessary.oICharInfo.expression.Length; k++)
			{
				ocicf.charInfo.EnableExpressionCategory(k, _necessary.oICharInfo.expression[k]);
			}
			ocicf.animeSpeed = ocicf.animeSpeed;
			byte[] siru = _necessary.chaControl.fileStatus.siruLv;
			for (int l = 0; l < siru.Length; l++)
			{
				siru[l] = 0;
			}
			status.visibleSonAlways = _necessary.oICharInfo.visibleSon;
			AddObjectAssist.UpdateState(ocicf, status);
			if (_necessary.isOver)
			{
				yield return null;
				_necessary.Next();
			}
			_necessary.ocicf = ocicf;
		}
	}
}
