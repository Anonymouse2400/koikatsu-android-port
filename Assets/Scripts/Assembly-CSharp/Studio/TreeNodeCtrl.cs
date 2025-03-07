using System;
using System.Collections.Generic;
using System.Linq;
using GUITree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	public class TreeNodeCtrl : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private enum Calc
		{
			None = 0,
			Attach = 1,
			Detach = 2,
			Delete = 3,
			Copy = 4
		}

		[SerializeField]
		protected GameObject m_ObjectNode;

		[SerializeField]
		protected GameObject m_ObjectRoot;

		[SerializeField]
		protected TreeRoot m_TreeRoot;

		protected List<TreeNodeObject> m_TreeNodeObject = new List<TreeNodeObject>();

		protected HashSet<TreeNodeObject> hashSelectNode = new HashSet<TreeNodeObject>();

		public Action<TreeNodeObject, TreeNodeObject> onParentage;

		public Action<TreeNodeObject> onDelete;

		public Action<TreeNodeObject> onSelect;

		public Action onSelectMultiple;

		public Action<TreeNodeObject> onDeselect;

		[SerializeField]
		private Canvas canvas;

		[SerializeField]
		private ScrollRect scrollRect;

		public TreeNodeObject selectNode
		{
			get
			{
				TreeNodeObject[] array = selectNodes;
				return (array.Length == 0) ? null : array[0];
			}
			set
			{
				SetSelectNode(value);
			}
		}

		public TreeNodeObject[] selectNodes
		{
			get
			{
				return hashSelectNode.ToArray();
			}
		}

		public ObjectCtrlInfo[] selectObjectCtrl
		{
			get
			{
				List<ObjectCtrlInfo> list = new List<ObjectCtrlInfo>();
				foreach (TreeNodeObject item in hashSelectNode)
				{
					ObjectCtrlInfo value = null;
					if (Singleton<Studio>.Instance.dicInfo.TryGetValue(item, out value))
					{
						list.Add(value);
					}
				}
				return list.ToArray();
			}
		}

		public TreeNodeObject AddNode(string _name, TreeNodeObject _parent = null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_ObjectNode);
			gameObject.SetActive(true);
			gameObject.transform.SetParent(m_ObjectRoot.transform, false);
			TreeNodeObject component = gameObject.GetComponent<TreeNodeObject>();
			component.textName = _name;
			if ((bool)_parent)
			{
				component.SetParent(_parent);
			}
			else
			{
				m_TreeNodeObject.Add(component);
			}
			return component;
		}

		public bool AddNode(TreeNodeObject _node)
		{
			if (_node == null || _node.parent != null)
			{
				return false;
			}
			if (m_TreeNodeObject.Contains(_node))
			{
				return false;
			}
			m_TreeNodeObject.Add(_node);
			return true;
		}

		public void RemoveNode(TreeNodeObject _node)
		{
			if (!(_node == null) && !(_node.parent == null))
			{
				m_TreeNodeObject.Remove(_node);
			}
		}

		public bool CheckNode(TreeNodeObject _node)
		{
			if (_node == null)
			{
				return false;
			}
			return m_TreeNodeObject.Contains(_node);
		}

		public void DeleteNode(TreeNodeObject _node)
		{
			if (_node.enableDelete)
			{
				_node.SetParent(null);
				m_TreeNodeObject.Remove(_node);
				if (_node.onDelete != null)
				{
					_node.onDelete();
				}
				DeleteNodeLoop(_node);
				if (m_TreeNodeObject.Count == 0)
				{
					scrollRect.verticalNormalizedPosition = 1f;
				}
			}
		}

		public void DeleteAllNode()
		{
			int count = m_TreeNodeObject.Count;
			for (int i = 0; i < count; i++)
			{
				DeleteAllNodeLoop(m_TreeNodeObject[i]);
			}
			m_TreeNodeObject.Clear();
			hashSelectNode.Clear();
			scrollRect.verticalNormalizedPosition = 1f;
			scrollRect.horizontalNormalizedPosition = 0f;
		}

		public TreeNodeObject GetNode(int _index)
		{
			int count = m_TreeNodeObject.Count;
			if (count == 0)
			{
				return null;
			}
			return (_index < 0 || _index >= count) ? null : m_TreeNodeObject[_index];
		}

		public void SetParent(TreeNodeObject _node, TreeNodeObject _parent)
		{
			if (!(_node == null) && _node.enableChangeParent && (!CheckNode(_node) || !(_parent == null)) && _node.SetParent(_parent))
			{
				RefreshHierachy();
				m_TreeRoot.Invoke("SetDirty", 0f);
				if (onParentage != null)
				{
					onParentage(_parent, _node);
				}
			}
		}

		public void RefreshHierachy()
		{
			int count = m_TreeNodeObject.Count;
			for (int i = 0; i < count; i++)
			{
				RefreshHierachyLoop(m_TreeNodeObject[i], 0, true);
				RefreshVisibleLoop(m_TreeNodeObject[i]);
			}
		}

		public void SetParent()
		{
			TreeNodeObject[] array = selectNodes;
			for (int i = 1; i < array.Length; i++)
			{
				SetParent(array[i], array[0]);
			}
			SelectSingle(null);
			Singleton<GuideObjectManager>.Instance.selectObject = null;
		}

		public void RemoveNode()
		{
			TreeNodeObject[] array = selectNodes;
			for (int i = 0; i < array.Length; i++)
			{
				SetParent(array[i], null);
			}
			SelectSingle(null);
		}

		public void DeleteNode()
		{
			TreeNodeObject[] array = selectNodes;
			for (int i = 0; i < array.Length; i++)
			{
				DeleteNode(array[i]);
			}
			SelectSingle(null);
		}

		public void CopyChangeAmount()
		{
			TreeNodeObject[] array = selectNodes;
			ObjectCtrlInfo value = null;
			if (!Singleton<Studio>.Instance.dicInfo.TryGetValue(array[0], out value))
			{
				return;
			}
			List<TreeNodeCommand.MoveCopyInfo> list = new List<TreeNodeCommand.MoveCopyInfo>();
			for (int i = 1; i < array.Length; i++)
			{
				ObjectCtrlInfo value2 = null;
				if (Singleton<Studio>.Instance.dicInfo.TryGetValue(array[i], out value2))
				{
					list.Add(new TreeNodeCommand.MoveCopyInfo(value2.objectInfo.dicKey, value2.objectInfo.changeAmount, value.objectInfo.changeAmount));
				}
			}
			Singleton<UndoRedoManager>.Instance.Do(new TreeNodeCommand.MoveCopyCommand(list.ToArray()));
			SelectSingle(null);
		}

		public void SelectMultiple(TreeNodeObject _start, TreeNodeObject _end)
		{
			float y = _start.rectNode.position.y;
			float y2 = _end.rectNode.position.y;
			float min = Mathf.Min(y, y2);
			float max = Mathf.Max(y, y2);
			foreach (TreeNodeObject item in hashSelectNode)
			{
				item.OnDeselect();
			}
			hashSelectNode.Clear();
			AddSelectNode(_start);
			foreach (TreeNodeObject item2 in m_TreeNodeObject)
			{
				SelectMultipleLoop(item2, min, max);
			}
			AddSelectNode(_end, true);
		}

		private void SelectMultipleLoop(TreeNodeObject _source, float _min, float _max)
		{
			if (_source == null)
			{
				return;
			}
			if (MathfEx.RangeEqualOff(_min, _source.rectNode.position.y, _max))
			{
				AddSelectNode(_source, true);
			}
			if (_source.treeState == TreeNodeObject.TreeState.Close)
			{
				return;
			}
			foreach (TreeNodeObject item in _source.child)
			{
				SelectMultipleLoop(item, _min, _max);
			}
		}

		private void RefreshHierachyLoop(TreeNodeObject _source, int _indent, bool _visible)
		{
			_source.transform.SetAsLastSibling();
			_source.treeNode.indent = _indent;
			if (_source.gameObject.activeSelf != _visible)
			{
				_source.gameObject.SetActive(_visible);
			}
			int childCount = _source.childCount;
			if (_visible)
			{
				_visible = _source.treeState == TreeNodeObject.TreeState.Open;
			}
			for (int i = 0; i < childCount; i++)
			{
				RefreshHierachyLoop(_source.child[i], _indent + 1, _visible);
			}
		}

		private void RefreshVisibleLoop(TreeNodeObject _source)
		{
			if (!_source.visible)
			{
				_source.visible = _source.visible;
				return;
			}
			int childCount = _source.childCount;
			for (int i = 0; i < childCount; i++)
			{
				RefreshVisibleLoop(_source.child[i]);
			}
		}

		private void DeleteNodeLoop(TreeNodeObject _node)
		{
			if (!(_node == null))
			{
				if (_node.onDelete != null)
				{
					_node.onDelete();
				}
				int childCount = _node.childCount;
				for (int i = 0; i < childCount; i++)
				{
					DeleteNodeLoop(_node.child[i]);
				}
				UnityEngine.Object.Destroy(_node.gameObject);
				if (onDelete != null)
				{
					onDelete(_node);
				}
			}
		}

		private void DeleteAllNodeLoop(TreeNodeObject _node)
		{
			if (!(_node == null))
			{
				int childCount = _node.childCount;
				for (int i = 0; i < childCount; i++)
				{
					DeleteAllNodeLoop(_node.child[i]);
				}
				UnityEngine.Object.DestroyImmediate(_node.gameObject);
			}
		}

		private void SetSelectNode(TreeNodeObject _node)
		{
			bool flag = Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl);
			Calc calc = Calc.None;
			if ((bool)selectNode && Input.GetKey(KeyCode.P))
			{
				calc = Calc.Attach;
			}
			else if (Input.GetKey(KeyCode.O))
			{
				calc = Calc.Detach;
			}
			else if (Input.GetKey(KeyCode.X))
			{
				calc = Calc.Copy;
			}
			switch (calc)
			{
			case Calc.Attach:
				if (flag)
				{
					hashSelectNode.Add(_node);
					SetParent();
				}
				else
				{
					SetParent(selectNode, _node);
				}
				SelectSingle(_node);
				break;
			case Calc.Detach:
				if (flag)
				{
					hashSelectNode.Add(_node);
					foreach (TreeNodeObject item in hashSelectNode)
					{
						SetParent(item, null);
					}
				}
				else
				{
					SetParent(_node, null);
				}
				SelectSingle(_node);
				break;
			case Calc.Copy:
				if (flag)
				{
					hashSelectNode.Add(_node);
					if (hashSelectNode.Count > 1)
					{
						CopyChangeAmount();
					}
				}
				SelectSingle(_node);
				break;
			default:
				if (flag)
				{
					AddSelectNode(_node);
				}
				else
				{
					SelectSingle(_node);
				}
				break;
			}
		}

		public void SelectSingle(TreeNodeObject _node, bool _deselect = true)
		{
			ObjectCtrlInfo ctrlInfo = Studio.GetCtrlInfo(_node);
			bool flag = hashSelectNode.Count == 1 && (hashSelectNode.Contains(_node) & (ctrlInfo == null || ctrlInfo.guideObject.isActive));
			foreach (TreeNodeObject item in hashSelectNode)
			{
				item.OnDeselect();
			}
			hashSelectNode.Clear();
			if (_deselect && flag)
			{
				DeselectNode(_node);
			}
			else
			{
				AddSelectNode(_node);
			}
		}

		private void AddSelectNode(TreeNodeObject _node, bool _multiple = false)
		{
			if (_node == null)
			{
				return;
			}
			if (hashSelectNode.Add(_node))
			{
				if (onSelect != null && hashSelectNode.Count == 1)
				{
					onSelect(_node);
				}
				else if (onSelectMultiple != null && hashSelectNode.Count > 1)
				{
					onSelectMultiple();
				}
				_node.colorSelect = ((hashSelectNode.Count != 1) ? Utility.ConvertColor(94, 139, 100) : Utility.ConvertColor(91, 164, 82));
				ObjectCtrlInfo value = null;
				if (_multiple)
				{
					Singleton<GuideObjectManager>.Instance.AddSelectMultiple((!Singleton<Studio>.Instance.dicInfo.TryGetValue(_node, out value)) ? null : value.guideObject);
				}
				else
				{
					Singleton<GuideObjectManager>.Instance.selectObject = ((!Singleton<Studio>.Instance.dicInfo.TryGetValue(_node, out value)) ? null : value.guideObject);
				}
			}
			else
			{
				DeselectNode(_node);
			}
		}

		private void DeselectNode(TreeNodeObject _node)
		{
			hashSelectNode.Remove(_node);
			ObjectCtrlInfo value = null;
			if (Singleton<Studio>.Instance.dicInfo.TryGetValue(_node, out value))
			{
				Singleton<GuideObjectManager>.Instance.deselectObject = value.guideObject;
			}
			_node.OnDeselect();
			if (onDeselect != null)
			{
				onDeselect(_node);
			}
		}

		public bool CheckSelect(TreeNodeObject _node)
		{
			return hashSelectNode.Contains(_node);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			SortCanvas.select = canvas;
		}

		private void Start()
		{
			m_ObjectRoot.transform.localPosition = Vector3.zero;
		}
	}
}
