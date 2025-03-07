using Manager;
using UniRx;
using UnityEngine;

namespace Studio
{
	public class OCICamera : ObjectCtrlInfo
	{
		public GameObject objectItem;

		public MeshRenderer meshRenderer;

		private SingleAssignmentDisposable disposable;

		private bool visibleOutside = true;

		public OICameraInfo cameraInfo
		{
			get
			{
				return objectInfo as OICameraInfo;
			}
		}

		public string name
		{
			get
			{
				return cameraInfo.name;
			}
			set
			{
				cameraInfo.name = value;
				treeNodeObject.textName = value;
			}
		}

		public void SetActive(bool _active)
		{
			cameraInfo.active = _active;
			if (_active)
			{
				if (disposable == null)
				{
					Transform transCamera = Singleton<Studio>.Instance.cameraCtrl.transform;
					disposable = new SingleAssignmentDisposable();
					disposable.Disposable = Observable.EveryLateUpdate().Subscribe(delegate
					{
						transCamera.SetPositionAndRotation(objectItem.transform.position, objectItem.transform.rotation);
					});
					treeNodeObject.baseColor = AddObjectCamera.activeColor;
					if (!Singleton<Studio>.Instance.treeNodeCtrl.CheckSelect(treeNodeObject))
					{
						treeNodeObject.colorSelect = AddObjectCamera.activeColor;
					}
					guideObject.visible = false;
					meshRenderer.enabled = false;
				}
			}
			else
			{
				if (disposable != null)
				{
					disposable.Dispose();
					disposable = null;
				}
				treeNodeObject.baseColor = AddObjectCamera.baseColor;
				if (!Singleton<Studio>.Instance.treeNodeCtrl.CheckSelect(treeNodeObject))
				{
					treeNodeObject.colorSelect = AddObjectCamera.baseColor;
				}
				guideObject.visible = true;
				meshRenderer.enabled = !cameraInfo.active & visibleOutside;
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
			Singleton<Studio>.Instance.DeleteCamera(this);
		}

		public override void OnAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
		}

		public override void OnLoadAttach(TreeNodeObject _parent, ObjectCtrlInfo _child)
		{
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
		}

		public override void OnSavePreprocessing()
		{
			base.OnSavePreprocessing();
		}

		public override void OnVisible(bool _visible)
		{
			visibleOutside = _visible;
			meshRenderer.enabled = !cameraInfo.active & visibleOutside;
		}
	}
}
