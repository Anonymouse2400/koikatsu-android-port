using System;
using System.Linq;
using UnityEngine;

namespace Studio
{
	public class ManipulatePanelCtrl : MonoBehaviour
	{
		[Serializable]
		private class RootInfo
		{
			public GameObject root;

			public virtual bool active
			{
				set
				{
					if (root.activeSelf != value)
					{
						root.SetActive(value);
					}
				}
			}
		}

		[Serializable]
		private class CharaPanelInfo : RootInfo
		{
			private MPCharCtrl m_MPCharCtrl;

			public MPCharCtrl mpCharCtrl
			{
				get
				{
					if (m_MPCharCtrl == null)
					{
						m_MPCharCtrl = root.GetComponent<MPCharCtrl>();
					}
					return m_MPCharCtrl;
				}
			}

			public override bool active
			{
				set
				{
					mpCharCtrl.active = value;
				}
			}
		}

		[Serializable]
		private class ItemPanelInfo : RootInfo
		{
			private MPItemCtrl m_MPItemCtrl;

			public MPItemCtrl mpItemCtrl
			{
				get
				{
					if (m_MPItemCtrl == null)
					{
						m_MPItemCtrl = root.GetComponent<MPItemCtrl>();
					}
					return m_MPItemCtrl;
				}
			}

			public override bool active
			{
				set
				{
					mpItemCtrl.active = value;
				}
			}
		}

		[Serializable]
		private class LightPanelInfo : RootInfo
		{
			private MPLightCtrl m_MPLightCtrl;

			public MPLightCtrl mpLightCtrl
			{
				get
				{
					if (m_MPLightCtrl == null)
					{
						m_MPLightCtrl = root.GetComponent<MPLightCtrl>();
					}
					return m_MPLightCtrl;
				}
			}

			public override bool active
			{
				set
				{
					mpLightCtrl.active = value;
				}
			}
		}

		[Serializable]
		private class FolderPanelInfo : RootInfo
		{
			private MPFolderCtrl m_MPFolderCtrl;

			public MPFolderCtrl mpFolderCtrl
			{
				get
				{
					if (m_MPFolderCtrl == null)
					{
						m_MPFolderCtrl = root.GetComponent<MPFolderCtrl>();
					}
					return m_MPFolderCtrl;
				}
			}

			public override bool active
			{
				set
				{
					mpFolderCtrl.active = value;
				}
			}
		}

		[Serializable]
		private class RoutePanelInfo : RootInfo
		{
			private MPRouteCtrl m_MPRouteCtrl;

			public MPRouteCtrl mpRouteCtrl
			{
				get
				{
					if (m_MPRouteCtrl == null)
					{
						m_MPRouteCtrl = root.GetComponent<MPRouteCtrl>();
					}
					return m_MPRouteCtrl;
				}
			}

			public override bool active
			{
				set
				{
					mpRouteCtrl.active = value;
				}
			}
		}

		[Serializable]
		private class CameraPanelInfo : RootInfo
		{
			private MPCameraCtrl m_MPCameraCtrl;

			public MPCameraCtrl mpCameraCtrl
			{
				get
				{
					if (m_MPCameraCtrl == null)
					{
						m_MPCameraCtrl = root.GetComponent<MPCameraCtrl>();
					}
					return m_MPCameraCtrl;
				}
			}

			public override bool active
			{
				set
				{
					mpCameraCtrl.active = value;
				}
			}
		}

		[Serializable]
		private class RoutePointPanelInfo : RootInfo
		{
			private MPRoutePointCtrl m_MPRoutePointCtrl;

			public MPRoutePointCtrl mpRoutePointCtrl
			{
				get
				{
					if (m_MPRoutePointCtrl == null)
					{
						m_MPRoutePointCtrl = root.GetComponent<MPRoutePointCtrl>();
					}
					return m_MPRoutePointCtrl;
				}
			}

			public override bool active
			{
				set
				{
					mpRoutePointCtrl.active = value;
				}
			}
		}

		[SerializeField]
		private CharaPanelInfo charaPanelInfo = new CharaPanelInfo();

		[SerializeField]
		private ItemPanelInfo itemPanelInfo = new ItemPanelInfo();

		[SerializeField]
		private LightPanelInfo lightPanelInfo = new LightPanelInfo();

		[SerializeField]
		private FolderPanelInfo folderPanelInfo = new FolderPanelInfo();

		[SerializeField]
		private RoutePanelInfo routePanelInfo = new RoutePanelInfo();

		[SerializeField]
		private CameraPanelInfo cameraPanelInfo = new CameraPanelInfo();

		[SerializeField]
		private RoutePointPanelInfo routePointPanelInfo = new RoutePointPanelInfo();

		private int[] kinds = new int[1] { -1 };

		public bool active
		{
			get
			{
				return base.gameObject.activeSelf;
			}
			set
			{
				base.gameObject.SetActive(value);
				if (base.gameObject.activeSelf)
				{
					SetActive();
				}
				else
				{
					Deactivate();
				}
			}
		}

		private RootInfo[] rootPanel { get; set; }

		public ObjectCtrlInfo objectCtrlInfo
		{
			set
			{
				kinds = ((value != null) ? value.kinds : new int[1] { -1 });
				charaPanelInfo.mpCharCtrl.ociChar = value as OCIChar;
				itemPanelInfo.mpItemCtrl.ociItem = value as OCIItem;
				lightPanelInfo.mpLightCtrl.ociLight = value as OCILight;
				folderPanelInfo.mpFolderCtrl.ociFolder = value as OCIFolder;
				routePanelInfo.mpRouteCtrl.ociRoute = value as OCIRoute;
				cameraPanelInfo.mpCameraCtrl.ociCamera = value as OCICamera;
				routePointPanelInfo.mpRoutePointCtrl.ociRoutePoint = value as OCIRoutePoint;
			}
		}

		public void OnSelect(TreeNodeObject _node)
		{
			Singleton<Studio>.Instance.colorPalette.visible = false;
			ObjectCtrlInfo objectCtrlInfo = TryGetLoop(_node);
			kinds = ((objectCtrlInfo != null) ? objectCtrlInfo.kinds : new int[1] { -1 });
			for (int i = 0; i < kinds.Length; i++)
			{
				switch (kinds[i])
				{
				case 0:
					charaPanelInfo.mpCharCtrl.ociChar = objectCtrlInfo[i] as OCIChar;
					break;
				case 1:
					itemPanelInfo.mpItemCtrl.ociItem = objectCtrlInfo[i] as OCIItem;
					break;
				case 2:
					lightPanelInfo.mpLightCtrl.ociLight = objectCtrlInfo[i] as OCILight;
					break;
				case 3:
					folderPanelInfo.mpFolderCtrl.ociFolder = objectCtrlInfo[i] as OCIFolder;
					break;
				case 4:
					routePanelInfo.mpRouteCtrl.ociRoute = objectCtrlInfo[i] as OCIRoute;
					break;
				case 5:
					cameraPanelInfo.mpCameraCtrl.ociCamera = objectCtrlInfo[i] as OCICamera;
					break;
				case 6:
					routePointPanelInfo.mpRoutePointCtrl.ociRoutePoint = objectCtrlInfo[i] as OCIRoutePoint;
					break;
				}
			}
			if (active)
			{
				SetActive();
			}
		}

		public void OnDeselect(TreeNodeObject _node)
		{
			ObjectCtrlInfo objectCtrlInfo = TryGetLoop(_node);
			switch ((objectCtrlInfo != null) ? objectCtrlInfo.kind : (-1))
			{
			case 0:
				charaPanelInfo.mpCharCtrl.Deselect(objectCtrlInfo as OCIChar);
				break;
			case 1:
				itemPanelInfo.mpItemCtrl.Deselect(objectCtrlInfo as OCIItem);
				break;
			case 2:
				lightPanelInfo.mpLightCtrl.Deselect(objectCtrlInfo as OCILight);
				break;
			case 3:
				folderPanelInfo.mpFolderCtrl.Deselect(objectCtrlInfo as OCIFolder);
				break;
			case 4:
				routePanelInfo.mpRouteCtrl.Deselect(objectCtrlInfo as OCIRoute);
				break;
			case 5:
				cameraPanelInfo.mpCameraCtrl.Deselect(objectCtrlInfo as OCICamera);
				break;
			case 6:
				routePointPanelInfo.mpRoutePointCtrl.Deselect(objectCtrlInfo as OCIRoutePoint);
				break;
			}
		}

		public void UpdateInfo(int _kind)
		{
			switch (_kind)
			{
			case 1:
				itemPanelInfo.mpItemCtrl.UpdateInfo();
				break;
			case 5:
				cameraPanelInfo.mpCameraCtrl.UpdateInfo();
				break;
			}
		}

		private void SetActive()
		{
			for (int i = 0; i < rootPanel.Length; i++)
			{
				rootPanel[i].active = kinds.Contains(i);
			}
		}

		private void Deactivate()
		{
			for (int i = 0; i < rootPanel.Length; i++)
			{
				rootPanel[i].active = false;
			}
		}

		public void OnDelete(TreeNodeObject _node)
		{
			kinds = new int[1] { -1 };
			SetActive();
		}

		private ObjectCtrlInfo TryGetLoop(TreeNodeObject _node)
		{
			if (_node == null)
			{
				return null;
			}
			ObjectCtrlInfo value = null;
			if (Singleton<Studio>.Instance.dicInfo.TryGetValue(_node, out value))
			{
				return value;
			}
			return TryGetLoop(_node.parent);
		}

		private void Awake()
		{
			rootPanel = new RootInfo[7] { charaPanelInfo, itemPanelInfo, lightPanelInfo, folderPanelInfo, routePanelInfo, cameraPanelInfo, routePointPanelInfo };
			kinds = new int[1] { -1 };
			SetActive();
			TreeNodeCtrl treeNodeCtrl = Singleton<Studio>.Instance.treeNodeCtrl;
			treeNodeCtrl.onSelect = (Action<TreeNodeObject>)Delegate.Combine(treeNodeCtrl.onSelect, new Action<TreeNodeObject>(OnSelect));
			TreeNodeCtrl treeNodeCtrl2 = Singleton<Studio>.Instance.treeNodeCtrl;
			treeNodeCtrl2.onDelete = (Action<TreeNodeObject>)Delegate.Combine(treeNodeCtrl2.onDelete, new Action<TreeNodeObject>(OnDelete));
			TreeNodeCtrl treeNodeCtrl3 = Singleton<Studio>.Instance.treeNodeCtrl;
			treeNodeCtrl3.onDeselect = (Action<TreeNodeObject>)Delegate.Combine(treeNodeCtrl3.onDeselect, new Action<TreeNodeObject>(OnDeselect));
		}
	}
}
