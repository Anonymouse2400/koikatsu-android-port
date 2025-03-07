using Manager;
using UnityEngine;

namespace Studio
{
	public static class AddObjectFolder
	{
		public static OCIFolder Add()
		{
			return Load(new OIFolderInfo(Studio.GetNewIndex()), null, null, true, Studio.optionSystem.initialPosition);
		}

		public static OCIFolder Load(OIFolderInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			ChangeAmount source = _info.changeAmount.Clone();
			OCIFolder oCIFolder = Load(_info, _parent, _parentNode, false, -1);
			_info.changeAmount.Copy(source);
			AddObjectAssist.LoadChild(_info.child, oCIFolder, null);
			return oCIFolder;
		}

		public static OCIFolder Load(OIFolderInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
		{
			OCIFolder oCIFolder = new OCIFolder();
			oCIFolder.objectInfo = _info;
			GameObject gameObject = new GameObject(_info.name);
			if (gameObject == null)
			{
				Studio.DeleteIndex(_info.dicKey);
				return null;
			}
			gameObject.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			oCIFolder.objectItem = gameObject;
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, _info.dicKey);
			guideObject.isActive = false;
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.enableScale = false;
			guideObject.SetVisibleCenter(true);
			oCIFolder.guideObject = guideObject;
			oCIFolder.childRoot = gameObject.transform;
			if (_addInfo)
			{
				Studio.AddInfo(_info, oCIFolder);
			}
			else
			{
				Studio.AddObjectCtrlInfo(oCIFolder);
			}
			TreeNodeObject parent = ((_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject));
			TreeNodeObject treeNodeObject = Studio.AddNode(_info.name, parent);
			treeNodeObject.treeState = _info.treeState;
			treeNodeObject.enableVisible = true;
			treeNodeObject.visible = _info.visible;
			treeNodeObject.baseColor = Utility.ConvertColor(180, 150, 5);
			treeNodeObject.colorSelect = treeNodeObject.baseColor;
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			oCIFolder.treeNodeObject = treeNodeObject;
			if (_initialPosition == 1)
			{
				_info.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			_info.changeAmount.OnChange();
			Studio.AddCtrlInfo(oCIFolder);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, oCIFolder);
			}
			return oCIFolder;
		}
	}
}
