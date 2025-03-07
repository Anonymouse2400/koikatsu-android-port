using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UniRx;
using UnityEngine;
using Vectrosity;

namespace Studio
{
	public class OCIRoute : ObjectCtrlInfo
	{
		public const int limitNum = 10;

		public GameObject objectItem;

		public Transform childRoot;

		public TreeNodeObject childNodeRoot;

		public RouteComponent routeComponent;

		private int nowIndex;

		private SingleAssignmentDisposable disposable;

		private VectorLine line;

		private int segments = 160;

		private GameObject objLine;

		public OIRouteInfo routeInfo
		{
			get
			{
				return objectInfo as OIRouteInfo;
			}
		}

		public string name
		{
			get
			{
				return routeInfo.name;
			}
			set
			{
				routeInfo.name = value;
				treeNodeObject.textName = value;
			}
		}

		public List<OCIRoutePoint> listPoint { get; private set; }

		public bool isPlay
		{
			get
			{
				return routeInfo.active;
			}
		}

		public bool isEnd
		{
			get
			{
				return routeInfo.route.Count > 1 && ((nowIndex >= listPoint.Count - 1) & !routeInfo.active);
			}
		}

		public bool visibleLine
		{
			get
			{
				return routeInfo.visibleLine;
			}
			set
			{
				routeInfo.visibleLine = value;
				SetVisible(value);
			}
		}

		public OCIRoute()
		{
			listPoint = new List<OCIRoutePoint>();
		}

		public override void OnDelete()
		{
			if (line != null)
			{
				VectorLine.Destroy(ref line);
			}
			Singleton<GuideObjectManager>.Instance.Delete(guideObject);
			UnityEngine.Object.Destroy(objectItem);
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
			if (!routeInfo.child.Contains(_child.objectInfo))
			{
				routeInfo.child.Add(_child.objectInfo);
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
			if (!routeInfo.child.Contains(_child.objectInfo))
			{
				routeInfo.child.Add(_child.objectInfo);
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
			if (!routeInfo.child.Remove(_child.objectInfo))
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
			if (line != null)
			{
				line.active = _visible && visibleLine;
			}
		}

		public void OnDeleteNode()
		{
			foreach (OCIRoutePoint item in listPoint)
			{
				item.isParentDelete = true;
			}
		}

		private void SetVisible(bool _flag)
		{
			bool flag = _flag & treeNodeObject.visible;
			if (line != null)
			{
				line.active = flag;
			}
			foreach (OCIRoutePoint item in listPoint)
			{
				item.visible = flag;
			}
		}

		public bool CheckParentLoop(TreeNodeObject _parent)
		{
			if (_parent == null)
			{
				return true;
			}
			ObjectCtrlInfo ctrlInfo = Studio.GetCtrlInfo(_parent);
			if (ctrlInfo != null)
			{
				switch (ctrlInfo.kind)
				{
				case 1:
				{
					OCIItem oCIItem = ctrlInfo as OCIItem;
					if (oCIItem.itemInfo.group == 10 || oCIItem.itemInfo.group == 15)
					{
						return false;
					}
					break;
				}
				case 4:
					return false;
				}
			}
			return CheckParentLoop(_parent.parent);
		}

		public OCIRoutePoint AddPoint()
		{
			if (Studio.optionSystem.routePointLimit && routeInfo.route.Count > 10)
			{
				return null;
			}
			OCIRoutePoint result = AddObjectRoute.AddPoint(this);
			UpdateLine();
			return result;
		}

		public void DeletePoint(OCIRoutePoint _routePoint)
		{
			Stop();
			treeNodeObject.RemoveChild(_routePoint.treeNodeObject, true);
			listPoint.Remove(_routePoint);
			routeInfo.route.Remove(_routePoint.routePointInfo);
			UpdateNumber();
			UpdateLine();
		}

		public bool Play()
		{
			if (routeInfo.route.Count <= 1)
			{
				return false;
			}
			Stop(false);
			Transform transform = listPoint[0].objectItem.transform;
			childRoot.SetPositionAndRotation(transform.position, transform.rotation);
			int _index = 0;
			StudioTween studioTween = SetPath(null, ref _index);
			while (_index < listPoint.Count)
			{
				SetPath(studioTween, ref _index);
				if (!routeInfo.loop && _index == listPoint.Count - 1)
				{
					break;
				}
			}
			studioTween.loopType = (routeInfo.loop ? StudioTween.LoopType.loop : StudioTween.LoopType.none);
			if (!routeInfo.loop)
			{
				studioTween.onComplete = (StudioTween.CompleteFunction)Delegate.Combine(studioTween.onComplete, (StudioTween.CompleteFunction)delegate
				{
					routeInfo.active = false;
					Singleton<Studio>.Instance.routeControl.SetState(objectInfo, RouteNode.State.End);
					return true;
				});
			}
			routeInfo.active = true;
			return true;
		}

		private bool Move()
		{
			StudioTween.Stop(childRoot.gameObject);
			if (routeInfo.loop)
			{
				if (nowIndex >= listPoint.Count)
				{
					nowIndex = 0;
				}
			}
			else if (nowIndex >= listPoint.Count - 1)
			{
				routeInfo.active = false;
				Singleton<Studio>.Instance.routeControl.SetState(objectInfo, RouteNode.State.End);
				return false;
			}
			Hashtable hashtable = new Hashtable();
			switch (listPoint[nowIndex].connection)
			{
			case OIRoutePointInfo.Connection.Line:
			{
				Transform[] array = null;
				array = ((nowIndex != listPoint.Count - 1) ? (from v in listPoint.Skip(nowIndex).Take(2)
					select v.objectItem.transform).ToArray() : new Transform[2]
				{
					listPoint[listPoint.Count - 1].objectItem.transform,
					listPoint[0].objectItem.transform
				});
				hashtable.Add("path", array);
				break;
			}
			case OIRoutePointInfo.Connection.Curve:
			{
				List<Transform> list = listPoint[nowIndex].transform.ToList();
				if (nowIndex + 1 >= listPoint.Count)
				{
					list.Add(listPoint[0].objectItem.transform);
				}
				else
				{
					list.Add(listPoint[nowIndex + 1].objectItem.transform);
				}
				hashtable.Add("path", list.ToArray());
				break;
			}
			}
			hashtable.Add("speed", listPoint[nowIndex].routePointInfo.speed);
			hashtable.Add("easetype", listPoint[nowIndex].routePointInfo.easeType);
			hashtable.Add("looptype", StudioTween.LoopType.none);
			switch (routeInfo.orient)
			{
			case OIRouteInfo.Orient.Y:
				hashtable.Add("orienttopath", true);
				hashtable.Add("axis", "y");
				break;
			case OIRouteInfo.Orient.XY:
				hashtable.Add("orienttopath", true);
				break;
			}
			StudioTween studioTween = StudioTween.MoveTo(childRoot.gameObject, hashtable);
			studioTween.onComplete = (StudioTween.CompleteFunction)Delegate.Combine(studioTween.onComplete, new StudioTween.CompleteFunction(Move));
			nowIndex++;
			return true;
		}

		private StudioTween SetPath(StudioTween _tween, ref int _index)
		{
			if (!routeInfo.loop && _index == listPoint.Count - 1)
			{
				return _tween;
			}
			int num = _index++;
			Hashtable hashtable = new Hashtable();
			switch (listPoint[num].connection)
			{
			case OIRoutePointInfo.Connection.Line:
			{
				Transform[] array = null;
				array = ((num != listPoint.Count - 1) ? (from v in listPoint.Skip(num).Take(2)
					select v.objectItem.transform).ToArray() : new Transform[2]
				{
					listPoint[listPoint.Count - 1].objectItem.transform,
					listPoint[0].objectItem.transform
				});
				hashtable.Add("path", array);
				break;
			}
			case OIRoutePointInfo.Connection.Curve:
			{
				List<Transform> list = listPoint[num].transform.ToList();
				while (_index < listPoint.Count && (routeInfo.loop || _index != listPoint.Count - 1) && listPoint[_index].isLink)
				{
					list.AddRange(listPoint[_index].transform);
					_index++;
				}
				if (_index >= listPoint.Count)
				{
					list.Add(listPoint[0].objectItem.transform);
				}
				else
				{
					list.Add(listPoint[_index].objectItem.transform);
				}
				hashtable.Add("path", list.ToArray());
				break;
			}
			}
			hashtable.Add("speed", listPoint[num].routePointInfo.speed);
			hashtable.Add("easetype", listPoint[num].routePointInfo.easeType);
			switch (routeInfo.orient)
			{
			case OIRouteInfo.Orient.Y:
				hashtable.Add("orienttopath", true);
				hashtable.Add("axis", "y");
				break;
			case OIRouteInfo.Orient.XY:
				hashtable.Add("orienttopath", true);
				break;
			}
			if (_tween != null)
			{
				_tween.MoveTo(hashtable);
				return _tween;
			}
			return StudioTween.MoveTo(childRoot.gameObject, hashtable);
		}

		public void Stop(bool _copy = true)
		{
			StudioTween.Stop(childRoot.gameObject);
			if (disposable != null)
			{
				disposable.Dispose();
				disposable = null;
			}
			if (!listPoint.IsNullOrEmpty() && _copy)
			{
				disposable = new SingleAssignmentDisposable();
				disposable.Disposable = Observable.EveryLateUpdate().Subscribe(delegate
				{
					Transform transform = listPoint[0].objectItem.transform;
					childRoot.SetPositionAndRotation(transform.position, transform.rotation);
				}).AddTo(childRoot);
			}
			nowIndex = 0;
			routeInfo.active = false;
		}

		public void UpdateLine()
		{
			if (routeInfo.route.Count <= 1)
			{
				DeleteLine();
				return;
			}
			bool flag = line == null;
			int i = 0;
			int num = 0;
			while (i < listPoint.Count)
			{
				switch (listPoint[i].connection)
				{
				case OIRoutePointInfo.Connection.Line:
					num++;
					i++;
					if (i >= listPoint.Count && routeInfo.loop)
					{
						num++;
					}
					break;
				case OIRoutePointInfo.Connection.Curve:
				{
					if (!routeInfo.loop && i == listPoint.Count - 1)
					{
						num += ((routeInfo.loop || i != listPoint.Count - 1) ? segments : 0) + 1;
						i++;
						break;
					}
					int num2 = 1;
					for (i++; i < listPoint.Count && (routeInfo.loop || i != listPoint.Count - 1) && listPoint[i].isLink; i++)
					{
						num2++;
					}
					num += segments * num2 + 1;
					break;
				}
				}
			}
			if (!flag && line.points3.Count != num)
			{
				VectorLine.Destroy(ref line);
				flag = true;
			}
			if (flag)
			{
				line = new VectorLine("Spline", new List<Vector3>(num), Studio.optionSystem.routeLineWidth, LineType.Continuous);
				objLine = GameObject.Find("Spline");
				if ((bool)objLine)
				{
					objLine.name = "Spline " + routeInfo.name;
					objLine.transform.SetParent(Singleton<Scene>.Instance.commonSpace.transform);
				}
			}
			i = 0;
			int num3 = 0;
			while (i < listPoint.Count)
			{
				switch (listPoint[i].connection)
				{
				case OIRoutePointInfo.Connection.Line:
				{
					List<Vector3> points2 = line.points3;
					points2[num3] = listPoint[i].position;
					i++;
					num3++;
					if (i >= listPoint.Count && routeInfo.loop)
					{
						points2[num3] = listPoint[0].position;
					}
					line.points3 = points2;
					break;
				}
				case OIRoutePointInfo.Connection.Curve:
				{
					if (!routeInfo.loop && i == listPoint.Count - 1)
					{
						List<Vector3> points = line.points3;
						points[num3] = listPoint[i].position;
						i++;
						num3++;
						line.points3 = points;
						break;
					}
					List<Transform> list = listPoint[i].transform.ToList();
					int j = i + 1;
					int num4 = 1;
					for (; j < listPoint.Count && (routeInfo.loop || j != listPoint.Count - 1) && listPoint[j].isLink; j++)
					{
						list.AddRange(listPoint[j].transform);
						num4++;
					}
					bool flag2 = i == 0 && j >= listPoint.Count && routeInfo.loop;
					if (j >= listPoint.Count)
					{
						if (!flag2)
						{
							list.Add(listPoint[0].objectItem.transform);
						}
					}
					else
					{
						list.Add(listPoint[j].objectItem.transform);
					}
					line.MakeSpline(list.Select((Transform v) => v.position).ToArray(), segments * num4, num3, flag2);
					i = j;
					num3 += segments * num4 + 1;
					break;
				}
				}
			}
			line.joins = Joins.Weld;
			line.color = routeInfo.color;
			line.continuousTexture = false;
			line.lineWidth = Studio.optionSystem.routeLineWidth;
			line.Draw3DAuto();
			line.layer = LayerMask.NameToLayer("Studio/Route");
			if (flag)
			{
				line.active = routeInfo.visibleLine;
				Renderer component = objLine.GetComponent<Renderer>();
				if ((bool)component)
				{
					component.material.renderQueue = 2900;
				}
			}
		}

		public void ForceUpdateLine()
		{
			DeleteLine();
			UpdateLine();
		}

		public void DeleteLine()
		{
			if (line != null)
			{
				VectorLine.Destroy(ref line);
			}
		}

		public void SetLineColor(Color _color)
		{
			if (line != null)
			{
				line.SetColor(_color);
			}
		}

		public void UpdateNumber()
		{
			for (int i = 0; i < listPoint.Count; i++)
			{
				listPoint[i].number = i;
			}
		}
	}
}
