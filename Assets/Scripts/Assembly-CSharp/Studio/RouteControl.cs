using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class RouteControl : MonoBehaviour
	{
		[SerializeField]
		private Transform nodeRoot;

		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private Sprite[] spritePlay;

		[SerializeField]
		[Space]
		private Button buttonAll;

		[SerializeField]
		private Button buttonReAll;

		[SerializeField]
		private Button buttonStopAll;

		[Space]
		[SerializeField]
		private MPRouteCtrl mpRouteCtrl;

		[SerializeField]
		private MPRoutePointCtrl mpRoutePointCtrl;

		private BoolReactiveProperty _visible = new BoolReactiveProperty(false);

		private List<ObjectInfo> listInfo;

		private Dictionary<ObjectInfo, RouteNode> dicNode;

		public bool visible
		{
			get
			{
				return _visible.Value;
			}
			set
			{
				_visible.Value = value;
			}
		}

		public void Init()
		{
			int childCount = nodeRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(nodeRoot.GetChild(i).gameObject);
			}
			nodeRoot.DetachChildren();
			scrollRect.verticalNormalizedPosition = 1f;
			dicNode.Clear();
			listInfo = ObjectInfoAssist.Find(4);
			for (int j = 0; j < listInfo.Count; j++)
			{
				GameObject gameObject = Object.Instantiate(objectNode);
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
				gameObject.transform.SetParent(nodeRoot, false);
				RouteNode component = gameObject.GetComponent<RouteNode>();
				OIRouteInfo oIRouteInfo = listInfo[j] as OIRouteInfo;
				component.spritePlay = spritePlay;
				component.text = oIRouteInfo.name;
				int no = j;
				component.buttonSelect.onClick.AddListener(delegate
				{
					OnSelect(no);
				});
				component.buttonPlay.onClick.AddListener(delegate
				{
					OnPlay(no);
				});
				OCIRoute oCIRoute = Studio.GetCtrlInfo(listInfo[j].dicKey) as OCIRoute;
				component.state = ((!oCIRoute.isEnd) ? (oCIRoute.isPlay ? RouteNode.State.Play : RouteNode.State.Stop) : RouteNode.State.End);
				dicNode.Add(listInfo[j], component);
			}
		}

		public void ReflectOption()
		{
			if (listInfo.IsNullOrEmpty())
			{
				listInfo = ObjectInfoAssist.Find(4);
			}
			foreach (OCIRoute item in listInfo.Select((ObjectInfo i) => Studio.GetCtrlInfo(i.dicKey) as OCIRoute))
			{
				item.UpdateLine();
			}
		}

		public void SetState(ObjectInfo _info, RouteNode.State _state)
		{
			if (dicNode != null)
			{
				RouteNode value = null;
				if (dicNode.TryGetValue(_info, out value))
				{
					value.state = _state;
				}
			}
		}

		private void OnSelect(int _idx)
		{
			Singleton<Studio>.Instance.treeNodeCtrl.SelectSingle(Studio.GetCtrlInfo(listInfo[_idx].dicKey).treeNodeObject, false);
		}

		private void OnPlay(int _idx)
		{
			OCIRoute oCIRoute = Studio.GetCtrlInfo(listInfo[_idx].dicKey) as OCIRoute;
			if (oCIRoute.isPlay)
			{
				oCIRoute.Stop();
				dicNode[listInfo[_idx]].state = RouteNode.State.Stop;
			}
			else if (oCIRoute.Play())
			{
				dicNode[listInfo[_idx]].state = RouteNode.State.Play;
			}
			mpRouteCtrl.UpdateInteractable(oCIRoute);
			mpRoutePointCtrl.UpdateInteractable(oCIRoute);
		}

		private void OnClickALL()
		{
			foreach (ObjectInfo item in listInfo)
			{
				OCIRoute oCIRoute = Studio.GetCtrlInfo(item.dicKey) as OCIRoute;
				if (!oCIRoute.isPlay && oCIRoute.Play())
				{
					dicNode[item].state = RouteNode.State.Play;
					mpRouteCtrl.UpdateInteractable(oCIRoute);
					mpRoutePointCtrl.UpdateInteractable(oCIRoute);
				}
			}
		}

		private void OnClickReAll()
		{
			foreach (ObjectInfo item in listInfo)
			{
				OCIRoute oCIRoute = Studio.GetCtrlInfo(item.dicKey) as OCIRoute;
				if (oCIRoute.Play())
				{
					dicNode[item].state = RouteNode.State.Play;
					mpRouteCtrl.UpdateInteractable(oCIRoute);
					mpRoutePointCtrl.UpdateInteractable(oCIRoute);
				}
			}
		}

		private void OnClickStopAll()
		{
			foreach (ObjectInfo item in listInfo)
			{
				OCIRoute oCIRoute = Studio.GetCtrlInfo(item.dicKey) as OCIRoute;
				oCIRoute.Stop();
				dicNode[item].state = RouteNode.State.Stop;
				mpRouteCtrl.UpdateInteractable(oCIRoute);
				mpRoutePointCtrl.UpdateInteractable(oCIRoute);
			}
		}

		private void Awake()
		{
			buttonAll.onClick.AddListener(OnClickALL);
			buttonReAll.onClick.AddListener(OnClickReAll);
			buttonStopAll.onClick.AddListener(OnClickStopAll);
			_visible.Subscribe(delegate(bool _b)
			{
				if (_b)
				{
					Init();
				}
				base.gameObject.SetActive(_b);
			});
			dicNode = new Dictionary<ObjectInfo, RouteNode>();
			base.gameObject.SetActive(false);
		}
	}
}
