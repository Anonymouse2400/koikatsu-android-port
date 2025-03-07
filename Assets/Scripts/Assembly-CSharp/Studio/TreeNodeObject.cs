using System;
using System.Collections.Generic;
using GUITree;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class TreeNodeObject : MonoBehaviour
	{
		public enum TreeState
		{
			Open = 0,
			Close = 1
		}

		public delegate void OnVisibleFunc(bool _b);

		public delegate bool CheckFunc(TreeNodeObject _parent);

		[SerializeField]
		protected TreeNode m_TreeNode;

		[SerializeField]
		protected Sprite[] m_SpriteState;

		[SerializeField]
		protected Button m_ButtonState;

		[SerializeField]
		protected Button m_ButtonSelect;

		[SerializeField]
		protected Image m_ImageSelect;

		[SerializeField]
		protected RectTransform m_TransSelect;

		[SerializeField]
		protected Text m_TextName;

		[SerializeField]
		protected Canvas canvas;

		protected TreeState m_TreeState;

		protected Image m_ImageState;

		[SerializeField]
		protected TreeNodeCtrl m_TreeNodeCtrl;

		protected bool m_Visible = true;

		[SerializeField]
		protected Button m_ButtonVisible;

		protected Image m_ImageVisible;

		[SerializeField]
		protected Sprite[] m_SpriteVisible;

		public OnVisibleFunc onVisible;

		[SerializeField]
		private float textPosX = 40f;

		[SerializeField]
		[Space(10f)]
		private RectTransform _rectNode;

		protected List<TreeNodeObject> m_child = new List<TreeNodeObject>();

		private float _addPosX;

		public Action onDelete;

		public CheckFunc checkChild;

		public CheckFunc checkParent;

		public TreeNode treeNode
		{
			get
			{
				return m_TreeNode;
			}
		}

		public Button buttonState
		{
			get
			{
				return m_ButtonState;
			}
		}

		public Button buttonSelect
		{
			get
			{
				return m_ButtonSelect;
			}
		}

		public Image imageSelect
		{
			get
			{
				return m_ImageSelect;
			}
		}

		public Color colorSelect
		{
			set
			{
				imageSelect.color = value;
			}
		}

		public string textName
		{
			get
			{
				return m_TextName.text;
			}
			set
			{
				m_TextName.text = value;
			}
		}

		public TreeState treeState
		{
			get
			{
				return m_TreeState;
			}
			set
			{
				if (Utility.SetStruct(ref m_TreeState, value))
				{
					SetTreeState(m_TreeState);
				}
			}
		}

		public Image imageState
		{
			get
			{
				if (m_ImageState == null)
				{
					m_ImageState = m_ButtonState.GetComponent<Image>();
				}
				return m_ImageState;
			}
		}

		public bool visible
		{
			get
			{
				return m_Visible;
			}
			set
			{
				SetVisible(value);
			}
		}

		public Button buttonVisible
		{
			get
			{
				return m_ButtonVisible;
			}
		}

		public Image imageVisible
		{
			get
			{
				if (m_ImageVisible == null)
				{
					m_ImageVisible = m_ButtonVisible.image;
				}
				return m_ImageVisible;
			}
		}

		public float imageVisibleWidth
		{
			get
			{
				return imageVisible.rectTransform.sizeDelta.x;
			}
		}

		public bool enableVisible
		{
			set
			{
				m_ButtonVisible.gameObject.SetActive(value);
				RecalcSelectButtonPos();
			}
		}

		public RectTransform rectNode
		{
			get
			{
				return _rectNode;
			}
		}

		public TreeNodeObject parent { get; set; }

		public bool isParent
		{
			get
			{
				return parent != null && enableChangeParent;
			}
		}

		public int childCount
		{
			get
			{
				return m_child.Count;
			}
		}

		public List<TreeNodeObject> child
		{
			get
			{
				return m_child;
			}
		}

		public bool enableChangeParent { get; set; }

		public bool enableDelete { get; set; }

		public bool enableAddChild { get; set; }

		public bool enableCopy { get; set; }

		public Color baseColor { get; set; }

		public float addPosX
		{
			set
			{
				_addPosX = value;
				RecalcSelectButtonPos();
			}
		}

		public TreeNodeObject childRoot { get; set; }

		public void OnClickState()
		{
			SortCanvas.select = canvas;
			SetTreeState((m_TreeState == TreeState.Open) ? TreeState.Close : TreeState.Open);
		}

		public void OnClickSelect()
		{
			Select(true);
		}

		public void OnClickVisible()
		{
			SortCanvas.select = canvas;
			SetVisible(!m_Visible);
		}

		public void OnDeselect()
		{
			colorSelect = baseColor;
			ObjectCtrlInfo value = null;
			if (Singleton<Studio>.Instance.dicInfo.TryGetValue(this, out value))
			{
				value.guideObject.SetActive(false);
			}
		}

		public bool SetParent(TreeNodeObject _parent)
		{
			if (!enableChangeParent)
			{
				return false;
			}
			if (!CheckChildLoop(this, _parent))
			{
				return false;
			}
			if (_parent != null && _parent.childRoot != null)
			{
				_parent = _parent.childRoot;
			}
			if (CheckParentLoop(_parent, this))
			{
				return false;
			}
			if ((bool)_parent && (_parent.child.Contains(this) || !_parent.enableAddChild))
			{
				return false;
			}
			bool flag = true;
			if (checkParent != null)
			{
				flag &= checkParent(_parent);
			}
			if (!flag)
			{
				return false;
			}
			if (parent != null)
			{
				parent.RemoveChild(this);
			}
			if ((bool)_parent)
			{
				_parent.AddChild(this);
			}
			else
			{
				parent = null;
				m_TreeNodeCtrl.AddNode(this);
			}
			return true;
		}

		public bool AddChild(TreeNodeObject _child)
		{
			if (!enableAddChild)
			{
				return false;
			}
			if (_child == null || _child == this)
			{
				return false;
			}
			if (m_child.Contains(_child))
			{
				return false;
			}
			m_child.Add(_child);
			_child.parent = this;
			m_TreeNodeCtrl.RemoveNode(_child);
			SetStateVisible(true);
			SetTreeState(m_TreeState);
			SetVisibleChild(_child, m_Visible);
			return true;
		}

		public void RemoveChild(TreeNodeObject _child, bool _removeOnly = false)
		{
			_child.transform.SetAsLastSibling();
			m_child.Remove(_child);
			if (!_removeOnly)
			{
				_child.parent = null;
				m_TreeNodeCtrl.AddNode(_child);
				SetStateVisible(childCount != 0);
			}
		}

		public void SetTreeState(TreeState _state)
		{
			m_TreeState = _state;
			imageState.sprite = m_SpriteState[(int)_state];
			bool flag = _state == TreeState.Open;
			foreach (TreeNodeObject item in m_child)
			{
				SetVisibleLoop(item, flag);
			}
		}

		public void SetVisible(bool _visible)
		{
			m_Visible = _visible;
			if (onVisible != null)
			{
				onVisible(_visible);
			}
			imageVisible.sprite = m_SpriteVisible[_visible ? 1 : 0];
			foreach (TreeNodeObject item in m_child)
			{
				SetVisibleChild(item, _visible);
			}
		}

		public void ResetVisible()
		{
			if (onVisible != null)
			{
				onVisible(m_Visible);
			}
			imageVisible.sprite = m_SpriteVisible[m_Visible ? 1 : 0];
			buttonVisible.interactable = true;
			foreach (TreeNodeObject item in child)
			{
				SetVisibleChild(item, m_Visible);
			}
		}

		public void Select(bool _button = false)
		{
			SortCanvas.select = canvas;
			TreeNodeObject selectNode = m_TreeNodeCtrl.selectNode;
			if (_button && selectNode != null && (Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift)))
			{
				m_TreeNodeCtrl.SelectMultiple(selectNode, this);
			}
			else
			{
				m_TreeNodeCtrl.selectNode = this;
			}
		}

		protected void SetStateVisible(bool _visible)
		{
			if ((bool)m_ButtonState)
			{
				m_ButtonState.gameObject.SetActive(_visible);
			}
		}

		protected void SetVisibleLoop(TreeNodeObject _source, bool _visible)
		{
			if (_source.gameObject.activeSelf != _visible)
			{
				_source.gameObject.SetActive(_visible);
			}
			if (_visible && _source.treeState == TreeState.Close)
			{
				_visible = false;
			}
			foreach (TreeNodeObject item in _source.child)
			{
				SetVisibleLoop(item, _visible);
			}
		}

		protected bool CheckParentLoop(TreeNodeObject _source, TreeNodeObject _target)
		{
			if (_source == null)
			{
				return false;
			}
			if (_source == _target)
			{
				return true;
			}
			return CheckParentLoop(_source.parent, _target);
		}

		protected void SetVisibleChild(TreeNodeObject _source, bool _visible)
		{
			bool b = (!_visible || _source.visible) && _visible;
			if (_source.onVisible != null)
			{
				_source.onVisible(b);
			}
			_source.buttonVisible.interactable = _visible;
			foreach (TreeNodeObject item in _source.child)
			{
				SetVisibleChild(item, b);
			}
		}

		protected void RecalcSelectButtonPos()
		{
			Vector2 anchoredPosition = m_TransSelect.anchoredPosition;
			anchoredPosition.x = _addPosX + ((!m_ButtonVisible.gameObject.activeSelf) ? (textPosX * 0.5f) : textPosX);
			m_TransSelect.anchoredPosition = anchoredPosition;
		}

		protected bool CheckChildLoop(TreeNodeObject _source, TreeNodeObject _parent)
		{
			if (_source == null || _parent == null)
			{
				return true;
			}
			bool flag = true;
			if (_source.checkChild != null)
			{
				flag &= _source.checkChild(_parent);
			}
			if (!flag)
			{
				return false;
			}
			foreach (TreeNodeObject item in _source.child)
			{
				if (!CheckChildLoop(item, _parent))
				{
					return false;
				}
			}
			return true;
		}

		private void Awake()
		{
			enableChangeParent = true;
			enableDelete = true;
			enableAddChild = true;
			enableCopy = true;
			baseColor = Utility.ConvertColor(100, 99, 94);
			colorSelect = baseColor;
		}

		private void Start()
		{
			SetStateVisible(childCount != 0);
		}
	}
}
