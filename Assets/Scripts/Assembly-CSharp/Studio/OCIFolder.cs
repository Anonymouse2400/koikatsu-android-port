using Manager;
using UnityEngine;

namespace Studio
{
	public class OCIFolder : ObjectCtrlInfo
	{
		public GameObject objectItem;

		public Transform childRoot;

		public OIFolderInfo folderInfo
		{
			get
			{
				return objectInfo as OIFolderInfo;
			}
		}

		public string name
		{
			get
			{
				return folderInfo.name;
			}
			set
			{
				folderInfo.name = value;
				treeNodeObject.textName = value;
			}
		}

		public override void OnDelete()
		{
			Singleton<GuideObjectManager>.Instance.Delete(guideObject);
			Object.Destroy(objectItem);
			if (parentInfo != null)
			{
				parentInfo.OnDetachChild(this);
			}
			Studio.DeleteInfo(objectInfo);
		}

		public override void OnAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
			if (_child.parentInfo == null)
			{
				Studio.DeleteInfo(_child.objectInfo, false);
			}
			else
			{
				_child.parentInfo.OnDetachChild(_child);
			}
			if (!folderInfo.child.Contains(_child.objectInfo))
			{
				folderInfo.child.Add(_child.objectInfo);
			}
			_child.guideObject.transformTarget.SetParent(childRoot);
			_child.guideObject.parent = childRoot;
			_child.guideObject.mode = GuideObject.Mode.World;
			_child.guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
			_child.objectInfo.changeAmount.pos = _child.guideObject.transformTarget.localPosition;
			_child.objectInfo.changeAmount.rot = _child.guideObject.transformTarget.localEulerAngles;
			_child.parentInfo = this;
		}

		public override void OnLoadAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
			if (_child.parentInfo == null)
			{
				Studio.DeleteInfo(_child.objectInfo, false);
			}
			else
			{
				_child.parentInfo.OnDetachChild(_child);
			}
			if (!folderInfo.child.Contains(_child.objectInfo))
			{
				folderInfo.child.Add(_child.objectInfo);
			}
			_child.guideObject.transformTarget.SetParent(childRoot, false);
			_child.guideObject.parent = childRoot;
			_child.guideObject.mode = GuideObject.Mode.World;
			_child.guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
			_child.objectInfo.changeAmount.OnChange();
			_child.parentInfo = this;
		}

		public override void OnDetach()
		{
			parentInfo.OnDetachChild(this);
			guideObject.parent = null;
			Studio.AddInfo(objectInfo, this);
			objectItem.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			objectInfo.changeAmount.pos = objectItem.transform.localPosition;
			objectInfo.changeAmount.rot = objectItem.transform.localEulerAngles;
			guideObject.mode = GuideObject.Mode.Local;
			guideObject.moveCalc = GuideMove.MoveCalc.TYPE1;
			treeNodeObject.ResetVisible();
		}

		public override void OnSelect(bool _select)
		{
		}

		public override void OnDetachChild(ObjectCtrlInfo _child)
		{
			if (!folderInfo.child.Remove(_child.objectInfo))
			{
			}
			_child.parentInfo = null;
		}

		public override void OnSavePreprocessing()
		{
			base.OnSavePreprocessing();
		}

		public override void OnVisible(bool _visible)
		{
		}
	}
}
