using System;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public static class AddObjectCamera
	{
		public static Color baseColor = Utility.ConvertColor(0, 104, 183);

		public static Color activeColor = Utility.ConvertColor(200, 0, 0);

		public static OCICamera Add()
		{
			return Load(new OICameraInfo(Studio.GetNewIndex()), null, null, true, Studio.optionSystem.initialPosition);
		}

		public static OCICamera Load(OICameraInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			ChangeAmount source = _info.changeAmount.Clone();
			OCICamera result = Load(_info, _parent, _parentNode, false, -1);
			_info.changeAmount.Copy(source);
			return result;
		}

		public static OCICamera Load(OICameraInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
		{
			OCICamera ocic = new OCICamera();
			ocic.objectInfo = _info;
			GameObject gameObject = CommonLib.LoadAsset<GameObject>("studio/base/06.unity3d", "p_koi_stu_cameraicon00_00", true, string.Empty);
			if (gameObject == null)
			{
				Studio.DeleteIndex(_info.dicKey);
				return null;
			}
			gameObject.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			ocic.objectItem = gameObject;
			ocic.meshRenderer = gameObject.GetComponent<MeshRenderer>();
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, _info.dicKey);
			guideObject.isActive = false;
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.enableScale = false;
			ocic.guideObject = guideObject;
			if (_addInfo)
			{
				Studio.AddInfo(_info, ocic);
			}
			else
			{
				Studio.AddObjectCtrlInfo(ocic);
			}
			TreeNodeObject parent = ((_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject));
			TreeNodeObject treeNodeObject = Studio.AddNode(_info.name, parent);
			treeNodeObject.onVisible = (TreeNodeObject.OnVisibleFunc)Delegate.Combine(treeNodeObject.onVisible, new TreeNodeObject.OnVisibleFunc(ocic.OnVisible));
			treeNodeObject.treeState = _info.treeState;
			treeNodeObject.enableVisible = true;
			treeNodeObject.enableAddChild = false;
			treeNodeObject.visible = _info.visible;
			treeNodeObject.baseColor = ((!_info.active) ? baseColor : activeColor);
			treeNodeObject.colorSelect = treeNodeObject.baseColor;
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			ocic.treeNodeObject = treeNodeObject;
			treeNodeObject.buttonSelect.OnPointerClickAsObservable().Subscribe(delegate(PointerEventData _ped)
			{
				if (_ped.button == PointerEventData.InputButton.Right)
				{
					Singleton<Studio>.Instance.ChangeCamera(ocic);
					Singleton<Studio>.Instance.manipulatePanelCtrl.UpdateInfo(5);
				}
			});
			if (_initialPosition == 1)
			{
				_info.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			_info.changeAmount.OnChange();
			Studio.AddCtrlInfo(ocic);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, ocic);
			}
			Singleton<Studio>.Instance.ChangeCamera(ocic, _info.active);
			return ocic;
		}
	}
}
