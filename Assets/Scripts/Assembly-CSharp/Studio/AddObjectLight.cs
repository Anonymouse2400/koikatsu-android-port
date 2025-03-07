using Manager;
using UnityEngine;

namespace Studio
{
	public static class AddObjectLight
	{
		public static OCILight Add(int _no)
		{
			int newIndex = Studio.GetNewIndex();
			Singleton<UndoRedoManager>.Instance.Do(new AddObjectCommand.AddLightCommand(_no, newIndex, Studio.optionSystem.initialPosition));
			return Studio.GetCtrlInfo(newIndex) as OCILight;
		}

		public static OCILight Load(OILightInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			ChangeAmount source = _info.changeAmount.Clone();
			OCILight result = Load(_info, _parent, _parentNode, false, -1);
			_info.changeAmount.Copy(source);
			return result;
		}

		public static OCILight Load(OILightInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
		{
			OCILight oCILight = new OCILight();
			Info.LightLoadInfo value = null;
			if (!Singleton<Info>.Instance.dicLightLoadInfo.TryGetValue(_info.no, out value))
			{
				return null;
			}
			oCILight.objectInfo = _info;
			GameObject gameObject = Utility.LoadAsset<GameObject>(value.bundlePath, value.fileName, value.manifest);
			gameObject.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			oCILight.objectLight = gameObject;
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, _info.dicKey);
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.isActive = false;
			guideObject.enableScale = false;
			guideObject.SetVisibleCenter(true);
			oCILight.guideObject = guideObject;
			oCILight.lightColor = gameObject.GetComponent<LightColor>();
			if ((bool)oCILight.lightColor)
			{
				oCILight.lightColor.color = _info.color;
			}
			oCILight.lightTarget = value.target;
			switch (value.target)
			{
			case Info.LightLoadInfo.Target.Chara:
			{
				int cullingMask2 = oCILight.light.cullingMask;
				cullingMask2 ^= LayerMask.GetMask("Map", "MapNoShadow");
				oCILight.light.cullingMask = cullingMask2;
				break;
			}
			case Info.LightLoadInfo.Target.Map:
			{
				int cullingMask = oCILight.light.cullingMask;
				cullingMask ^= LayerMask.GetMask("Chara");
				oCILight.light.cullingMask = cullingMask;
				break;
			}
			}
			if (_addInfo)
			{
				Studio.AddInfo(_info, oCILight);
			}
			else
			{
				Studio.AddObjectCtrlInfo(oCILight);
			}
			TreeNodeObject parent = ((_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject));
			TreeNodeObject treeNodeObject = Studio.AddNode(value.name, parent);
			treeNodeObject.enableAddChild = false;
			treeNodeObject.treeState = _info.treeState;
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			oCILight.treeNodeObject = treeNodeObject;
			if (_initialPosition == 1)
			{
				_info.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			_info.changeAmount.OnChange();
			Studio.AddCtrlInfo(oCILight);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, oCILight);
			}
			oCILight.Update();
			return oCILight;
		}
	}
}
