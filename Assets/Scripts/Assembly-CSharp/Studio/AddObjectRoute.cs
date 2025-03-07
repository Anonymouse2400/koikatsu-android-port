using System;
using Manager;
using UnityEngine;

namespace Studio
{
	public static class AddObjectRoute
	{
		public static OCIRoute Add()
		{
			return Load(new OIRouteInfo(Studio.GetNewIndex()), null, null, true, Studio.optionSystem.initialPosition);
		}

		public static OCIRoute Load(OIRouteInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode)
		{
			ChangeAmount source = _info.changeAmount.Clone();
			OCIRoute oCIRoute = Load(_info, _parent, _parentNode, false, -1);
			_info.changeAmount.Copy(source);
			AddObjectAssist.LoadChild(_info.child, oCIRoute, null);
			return oCIRoute;
		}

		public static OCIRoute Load(OIRouteInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
		{
			OCIRoute oCIRoute = new OCIRoute();
			oCIRoute.objectInfo = _info;
			GameObject gameObject = CommonLib.LoadAsset<GameObject>("studio/base/06.unity3d", "p_Route", true, string.Empty);
			if (gameObject == null)
			{
				Studio.DeleteIndex(_info.dicKey);
				return null;
			}
			gameObject.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
			oCIRoute.objectItem = gameObject;
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, _info.dicKey);
			guideObject.isActive = false;
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.enableScale = false;
			guideObject.SetVisibleCenter(true);
			oCIRoute.guideObject = guideObject;
			oCIRoute.childRoot = gameObject.transform.GetChild(0);
			if (_addInfo)
			{
				Studio.AddInfo(_info, oCIRoute);
			}
			else
			{
				Studio.AddObjectCtrlInfo(oCIRoute);
			}
			TreeNodeObject parent = ((_parentNode != null) ? _parentNode : ((_parent == null) ? null : _parent.treeNodeObject));
			TreeNodeObject treeNodeObject = Studio.AddNode(_info.name, parent);
			treeNodeObject.treeState = _info.treeState;
			treeNodeObject.enableVisible = true;
			treeNodeObject.enableChangeParent = false;
			treeNodeObject.visible = _info.visible;
			treeNodeObject.colorSelect = treeNodeObject.baseColor;
			treeNodeObject.onVisible = (TreeNodeObject.OnVisibleFunc)Delegate.Combine(treeNodeObject.onVisible, new TreeNodeObject.OnVisibleFunc(oCIRoute.OnVisible));
			treeNodeObject.onDelete = (Action)Delegate.Combine(treeNodeObject.onDelete, new Action(oCIRoute.OnDeleteNode));
			treeNodeObject.checkChild = (TreeNodeObject.CheckFunc)Delegate.Combine(treeNodeObject.checkChild, new TreeNodeObject.CheckFunc(oCIRoute.CheckParentLoop));
			treeNodeObject.checkParent = (TreeNodeObject.CheckFunc)Delegate.Combine(treeNodeObject.checkParent, new TreeNodeObject.CheckFunc(oCIRoute.CheckParentLoop));
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			oCIRoute.treeNodeObject = treeNodeObject;
			oCIRoute.routeComponent = gameObject.GetComponent<RouteComponent>();
			TreeNodeObject treeNodeObject2 = Studio.AddNode("子接続先", treeNodeObject);
			treeNodeObject2.enableChangeParent = false;
			treeNodeObject2.enableDelete = false;
			treeNodeObject2.enableCopy = false;
			treeNodeObject2.baseColor = Utility.ConvertColor(204, 128, 164);
			treeNodeObject2.colorSelect = treeNodeObject2.baseColor;
			treeNodeObject.childRoot = treeNodeObject2;
			oCIRoute.childNodeRoot = treeNodeObject2;
			if (_info.route.IsNullOrEmpty())
			{
				oCIRoute.routeInfo.route.Add(new OIRoutePointInfo(Studio.GetNewIndex()));
			}
			foreach (OIRoutePointInfo item in _info.route)
			{
				LoadPoint(oCIRoute, item, -1);
			}
			Singleton<Studio>.Instance.treeNodeCtrl.RefreshHierachy();
			if (_initialPosition == 1)
			{
				_info.changeAmount.pos = Singleton<Studio>.Instance.cameraCtrl.targetPos;
			}
			_info.changeAmount.OnChange();
			Studio.AddCtrlInfo(oCIRoute);
			if (_parent != null)
			{
				_parent.OnLoadAttach((!(_parentNode != null)) ? _parent.treeNodeObject : _parentNode, oCIRoute);
			}
			ChangeAmount changeAmount = _info.changeAmount;
			changeAmount.onChangePos = (Action)Delegate.Combine(changeAmount.onChangePos, new Action(oCIRoute.UpdateLine));
			ChangeAmount changeAmount2 = _info.changeAmount;
			changeAmount2.onChangeRot = (Action)Delegate.Combine(changeAmount2.onChangeRot, new Action(oCIRoute.UpdateLine));
			oCIRoute.ForceUpdateLine();
			oCIRoute.visibleLine = oCIRoute.visibleLine;
			if (oCIRoute.isPlay)
			{
				oCIRoute.Play();
			}
			else
			{
				oCIRoute.Stop();
			}
			return oCIRoute;
		}

		public static OCIRoutePoint AddPoint(OCIRoute _ocir)
		{
			OIRoutePointInfo oIRoutePointInfo = new OIRoutePointInfo(Studio.GetNewIndex());
			_ocir.routeInfo.route.Add(oIRoutePointInfo);
			OCIRoutePoint result = LoadPoint(_ocir, oIRoutePointInfo, 1);
			_ocir.visibleLine = _ocir.visibleLine;
			Singleton<Studio>.Instance.treeNodeCtrl.RefreshHierachy();
			return result;
		}

		public static OCIRoutePoint LoadPoint(OCIRoute _ocir, OIRoutePointInfo _rpInfo, int _initialPosition)
		{
			int num = ((!_ocir.listPoint.IsNullOrEmpty()) ? (_ocir.listPoint.Count - 1) : (-1));
			GameObject gameObject = CommonLib.LoadAsset<GameObject>("studio/base/06.unity3d", "p_RoutePoint", true, string.Empty);
			if (gameObject == null)
			{
				Studio.DeleteIndex(_rpInfo.dicKey);
				return null;
			}
			gameObject.transform.SetParent(_ocir.objectItem.transform);
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, _rpInfo.dicKey);
			guideObject.isActive = false;
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.enablePos = num != -1;
			guideObject.enableRot = num == -1;
			guideObject.enableScale = false;
			guideObject.mode = GuideObject.Mode.World;
			guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
			TreeNodeObject childRoot = _ocir.treeNodeObject.childRoot;
			_ocir.treeNodeObject.childRoot = null;
			TreeNodeObject treeNodeObject = Studio.AddNode(_rpInfo.name, _ocir.treeNodeObject);
			treeNodeObject.treeState = _rpInfo.treeState;
			treeNodeObject.enableChangeParent = false;
			treeNodeObject.enableDelete = num != -1;
			treeNodeObject.enableAddChild = false;
			treeNodeObject.enableCopy = false;
			treeNodeObject.enableVisible = false;
			guideObject.guideSelect.treeNodeObject = treeNodeObject;
			_ocir.treeNodeObject.childRoot = childRoot;
			OCIRoutePoint oCIRoutePoint = new OCIRoutePoint(_ocir, _rpInfo, gameObject, guideObject, treeNodeObject);
			_ocir.listPoint.Add(oCIRoutePoint);
			_ocir.UpdateNumber();
			treeNodeObject.onVisible = (TreeNodeObject.OnVisibleFunc)Delegate.Combine(treeNodeObject.onVisible, new TreeNodeObject.OnVisibleFunc(oCIRoutePoint.OnVisible));
			guideObject.isActiveFunc = (GuideObject.IsActiveFunc)Delegate.Combine(guideObject.isActiveFunc, new GuideObject.IsActiveFunc(oCIRoutePoint.OnSelect));
			Studio.AddCtrlInfo(oCIRoutePoint);
			InitAid(oCIRoutePoint);
			if (_initialPosition == 1)
			{
				if (num == -1)
				{
					_rpInfo.changeAmount.pos = _ocir.objectInfo.changeAmount.pos;
				}
				else
				{
					OCIRoutePoint oCIRoutePoint2 = _ocir.listPoint[num];
					_rpInfo.changeAmount.pos = _ocir.objectItem.transform.InverseTransformPoint(oCIRoutePoint2.position);
				}
			}
			_rpInfo.changeAmount.OnChange();
			ChangeAmount changeAmount = _rpInfo.changeAmount;
			changeAmount.onChangePosAfter = (Action)Delegate.Combine(changeAmount.onChangePosAfter, new Action(_ocir.UpdateLine));
			ChangeAmount changeAmount2 = _rpInfo.changeAmount;
			changeAmount2.onChangeRot = (Action)Delegate.Combine(changeAmount2.onChangeRot, new Action(_ocir.UpdateLine));
			oCIRoutePoint.connection = oCIRoutePoint.connection;
			return oCIRoutePoint;
		}

		public static void InitAid(OCIRoutePoint _ocirp)
		{
			bool flag = _ocirp.routePointInfo.aidInfo == null;
			if (flag)
			{
				_ocirp.routePointInfo.aidInfo = new OIRoutePointAidInfo(Studio.GetNewIndex());
			}
			Transform transform = _ocirp.routePoint.objAid.transform;
			if (flag)
			{
				_ocirp.routePointInfo.aidInfo.changeAmount.pos = transform.localPosition;
			}
			GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(transform, _ocirp.routePointInfo.aidInfo.dicKey);
			guideObject.enableRot = false;
			guideObject.enableScale = false;
			guideObject.enableMaluti = false;
			guideObject.scaleSelect = 0.1f;
			guideObject.scaleRot = 0.05f;
			guideObject.parentGuide = _ocirp.guideObject;
			guideObject.changeAmount.OnChange();
			guideObject.mode = GuideObject.Mode.World;
			guideObject.moveCalc = GuideMove.MoveCalc.TYPE2;
			_ocirp.pointAidInfo = new OCIRoutePoint.PointAidInfo(guideObject, _ocirp.routePointInfo.aidInfo);
			_ocirp.pointAidInfo.active = false;
			ChangeAmount changeAmount = _ocirp.routePointInfo.aidInfo.changeAmount;
			changeAmount.onChangePosAfter = (Action)Delegate.Combine(changeAmount.onChangePosAfter, new Action(_ocirp.route.UpdateLine));
		}
	}
}
