using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class WorkspaceCtrl : MonoBehaviour
	{
		[SerializeField]
		private Button buttonClose;

		[SerializeField]
		private Button buttonRemove;

		[SerializeField]
		private Button buttonParent;

		[SerializeField]
		private TreeNodeCtrl treeNodeCtrl;

		[SerializeField]
		private Button buttonDelete;

		[SerializeField]
		private Button buttonCopy;

		[SerializeField]
		private Button buttonDuplicate;

		[SerializeField]
		private Button buttonFolder;

		[SerializeField]
		private Button buttonCamera;

		[SerializeField]
		private Button buttonRoute;

		[SerializeField]
		private StudioScene studioScene;

		[SerializeField]
		private Text textBar;

		private Button[] buttons { get; set; }

		private void OnClickClose()
		{
			studioScene.OnClickDraw(1);
		}

		private void OnClickRemove()
		{
			treeNodeCtrl.RemoveNode();
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
		}

		private void OnClickParent()
		{
			treeNodeCtrl.SetParent();
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
		}

		public void OnClickDelete()
		{
			treeNodeCtrl.DeleteNode();
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
			Singleton<UndoRedoManager>.Instance.Clear();
		}

		private void OnClickCopy()
		{
			treeNodeCtrl.CopyChangeAmount();
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
		}

		private void OnClickDuplicate()
		{
			Singleton<Studio>.Instance.Duplicate();
		}

		private void OnClickFolder()
		{
			Singleton<Studio>.Instance.AddFolder();
		}

		private void OnClickCamera()
		{
			Singleton<Studio>.Instance.AddCamera();
		}

		private void OnClickRoute()
		{
			Singleton<Studio>.Instance.AddRoute();
		}

		public void UpdateUI()
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
		}

		public void OnParentage(TreeNodeObject _parent, TreeNodeObject _child)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
		}

		public void OnDeleteNode(TreeNodeObject _node)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
		}

		public void OnSelectSingle(TreeNodeObject _node)
		{
			buttonParent.interactable = false;
			buttonRemove.interactable = _node.isParent;
			buttonDelete.interactable = _node.enableDelete;
			buttonCopy.interactable = false;
			buttonDuplicate.interactable = _node.enableCopy;
		}

		public void OnSelectMultiple()
		{
			TreeNodeObject[] selectNodes = treeNodeCtrl.selectNodes;
			if (!selectNodes.IsNullOrEmpty())
			{
				buttonParent.interactable = selectNodes.Any((TreeNodeObject v) => v.enableChangeParent);
				buttonRemove.interactable = selectNodes.Any((TreeNodeObject v) => v.isParent);
				buttonDelete.interactable = selectNodes.Any((TreeNodeObject v) => v.enableDelete);
				buttonCopy.interactable = selectNodes[0].enableCopy && selectNodes.Count((TreeNodeObject v) => v.enableCopy) > 1;
				buttonDuplicate.interactable = selectNodes.Any((TreeNodeObject v) => v.enableCopy);
			}
		}

		public void OnDeselectSingle(TreeNodeObject _node)
		{
			TreeNodeObject[] selectNodes = treeNodeCtrl.selectNodes;
			if (selectNodes.IsNullOrEmpty())
			{
				for (int i = 0; i < buttons.Length; i++)
				{
					buttons[i].interactable = false;
				}
				return;
			}
			buttonParent.interactable = selectNodes.Length > 1 && selectNodes.Any((TreeNodeObject v) => v.enableChangeParent);
			buttonRemove.interactable = selectNodes.Any((TreeNodeObject v) => v.isParent);
			buttonDelete.interactable = selectNodes.Any((TreeNodeObject v) => v.enableDelete);
			buttonCopy.interactable = selectNodes.Length > 1 && selectNodes[0].enableCopy && selectNodes.Count((TreeNodeObject v) => v.enableCopy) > 1;
			buttonDuplicate.interactable = selectNodes.Any((TreeNodeObject v) => v.enableCopy);
		}

		private void Awake()
		{
			textBar.font.material.mainTexture.filterMode = FilterMode.Point;
			buttonClose.onClick.AddListener(OnClickClose);
			buttonRemove.onClick.AddListener(OnClickRemove);
			buttonParent.onClick.AddListener(OnClickParent);
			buttonDelete.onClick.AddListener(OnClickDelete);
			buttonCopy.onClick.AddListener(OnClickCopy);
			buttonDuplicate.onClick.AddListener(OnClickDuplicate);
			buttonFolder.onClick.AddListener(OnClickFolder);
			buttonCamera.image.enabled = false;
			buttonRoute.image.enabled = false;
			TreeNodeCtrl obj = treeNodeCtrl;
			obj.onParentage = (Action<TreeNodeObject, TreeNodeObject>)Delegate.Combine(obj.onParentage, new Action<TreeNodeObject, TreeNodeObject>(OnParentage));
			TreeNodeCtrl obj2 = treeNodeCtrl;
			obj2.onDelete = (Action<TreeNodeObject>)Delegate.Combine(obj2.onDelete, new Action<TreeNodeObject>(OnDeleteNode));
			TreeNodeCtrl obj3 = treeNodeCtrl;
			obj3.onSelect = (Action<TreeNodeObject>)Delegate.Combine(obj3.onSelect, new Action<TreeNodeObject>(OnSelectSingle));
			TreeNodeCtrl obj4 = treeNodeCtrl;
			obj4.onSelectMultiple = (Action)Delegate.Combine(obj4.onSelectMultiple, new Action(OnSelectMultiple));
			TreeNodeCtrl obj5 = treeNodeCtrl;
			obj5.onDeselect = (Action<TreeNodeObject>)Delegate.Combine(obj5.onDeselect, new Action<TreeNodeObject>(OnDeselectSingle));
			buttons = new Button[5] { buttonRemove, buttonParent, buttonDelete, buttonCopy, buttonDuplicate };
		}
	}
}
