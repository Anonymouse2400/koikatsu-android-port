using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class CharaList : MonoBehaviour
	{
		[SerializeField]
		private int sex = 1;

		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private RawImage imageChara;

		[SerializeField]
		private CharaFileSort charaFileSort = new CharaFileSort();

		[SerializeField]
		private Button buttonLoad;

		[SerializeField]
		private Button buttonChange;

		private bool isDelay;

		public bool isInit { get; private set; }

		public void InitCharaList(bool _force = false)
		{
			if (isInit && !_force)
			{
				return;
			}
			charaFileSort.DeleteAllNode();
			if (sex == 1)
			{
				InitFemaleList();
			}
			else
			{
				InitMaleList();
			}
			int count = charaFileSort.cfiList.Count;
			for (int i = 0; i < count; i++)
			{
				CharaFileInfo info = charaFileSort.cfiList[i];
				info.index = i;
				GameObject gameObject = UnityEngine.Object.Instantiate(objectNode);
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
				gameObject.transform.SetParent(charaFileSort.root, false);
				info.node = gameObject.GetComponent<ListNode>();
				info.button = gameObject.GetComponent<Button>();
				info.node.AddActionToButton(delegate
				{
					OnSelectChara(info.index);
				});
				info.node.text = info.name;
				info.node.listEnterAction.Add(delegate
				{
					LoadCharaImage(info.index);
				});
			}
			imageChara.color = Color.clear;
			charaFileSort.Sort(0, false);
			buttonLoad.interactable = false;
			buttonChange.interactable = false;
			isInit = true;
		}

		private void OnSelectChara(int _idx)
		{
			if (charaFileSort.select != _idx)
			{
				charaFileSort.select = _idx;
				buttonLoad.interactable = true;
				ObjectCtrlInfo ctrlInfo = Studio.GetCtrlInfo(Singleton<Studio>.Instance.treeNodeCtrl.selectNode);
				OCIChar oCIChar = ctrlInfo as OCIChar;
				buttonChange.interactable = oCIChar != null && oCIChar.oiCharInfo.sex == sex;
				isDelay = true;
				Observable.Timer(TimeSpan.FromMilliseconds(250.0)).Subscribe(delegate
				{
					isDelay = false;
				}).AddTo(this);
			}
		}

		private void LoadCharaImage(int _idx)
		{
			if (!isDelay)
			{
				CharaFileInfo charaFileInfo = charaFileSort.cfiList[_idx];
				imageChara.texture = PngAssist.LoadTexture(charaFileInfo.file);
				imageChara.color = Color.white;
				Resources.UnloadUnusedAssets();
				GC.Collect();
			}
		}

		public void OnSort(int _type)
		{
			charaFileSort.select = -1;
			buttonLoad.interactable = false;
			buttonChange.interactable = false;
			charaFileSort.Sort(_type);
		}

		public void LoadCharaFemale()
		{
			Singleton<Studio>.Instance.AddFemale(charaFileSort.selectPath);
		}

		public void ChangeCharaFemale()
		{
			OCIChar[] array = (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey
				select Studio.GetCtrlInfo(v) as OCIChar into v
				where v != null
				where v.oiCharInfo.sex == 1
				select v).ToArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				array[i].ChangeChara(charaFileSort.selectPath);
			}
		}

		private void InitFemaleList()
		{
			List<string> files = new List<string>();
			string folder = UserData.Path + "chara/female";
			Utils.File.GetAllFiles(folder, "*.png", ref files);
			charaFileSort.cfiList.Clear();
			foreach (string item in files)
			{
				ChaFileControl chaFileControl = new ChaFileControl();
				if (chaFileControl.LoadCharaFile(item, 1, true))
				{
					charaFileSort.cfiList.Add(new CharaFileInfo(string.Empty, string.Empty)
					{
						file = item,
						name = chaFileControl.parameter.fullname,
						time = File.GetLastWriteTime(item)
					});
				}
			}
		}

		public void LoadCharaMale()
		{
			Singleton<Studio>.Instance.AddMale(charaFileSort.selectPath);
		}

		public void ChangeCharaMale()
		{
			OCIChar[] array = (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey
				select Studio.GetCtrlInfo(v) as OCIChar into v
				where v != null
				where v.oiCharInfo.sex == 0
				select v).ToArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				array[i].ChangeChara(charaFileSort.selectPath);
			}
		}

		private void InitMaleList()
		{
			List<string> files = new List<string>();
			string folder = UserData.Path + "chara/male";
			Utils.File.GetAllFiles(folder, "*.png", ref files);
			charaFileSort.cfiList.Clear();
			foreach (string item in files)
			{
				ChaFileControl chaFileControl = new ChaFileControl();
				if (chaFileControl.LoadCharaFile(item, 0, true))
				{
					charaFileSort.cfiList.Add(new CharaFileInfo(string.Empty, string.Empty)
					{
						file = item,
						name = chaFileControl.parameter.fullname,
						time = File.GetLastWriteTime(item)
					});
				}
			}
		}

		private void OnSelect(TreeNodeObject _node)
		{
			if (_node == null || !Singleton<Studio>.IsInstance())
			{
				return;
			}
			ObjectCtrlInfo value = null;
			if (!Singleton<Studio>.Instance.dicInfo.TryGetValue(_node, out value))
			{
				buttonChange.interactable = false;
				return;
			}
			if (value.kind != 0)
			{
				buttonChange.interactable = false;
				return;
			}
			OCIChar oCIChar = value as OCIChar;
			if (oCIChar == null || oCIChar.oiCharInfo.sex != sex)
			{
				buttonChange.interactable = false;
			}
			else if (charaFileSort.select != -1)
			{
				buttonChange.interactable = true;
			}
		}

		private void OnDeselect(TreeNodeObject _node)
		{
			if (!(_node == null) && Singleton<Studio>.IsInstance())
			{
				OCIChar[] self = (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey
					select Studio.GetCtrlInfo(v) as OCIChar into v
					where v != null
					where v.oiCharInfo.sex == sex
					select v).ToArray();
				buttonChange.interactable = !self.IsNullOrEmpty();
			}
		}

		private void OnDelete(ObjectCtrlInfo _info)
		{
			if (_info != null && _info.kind == 0)
			{
				OCIChar oCIChar = _info as OCIChar;
				if (oCIChar != null && oCIChar.oiCharInfo.sex == sex && charaFileSort.select != -1)
				{
					buttonChange.interactable = false;
				}
			}
		}

		private void Awake()
		{
			isInit = false;
			InitCharaList();
			TreeNodeCtrl treeNodeCtrl = Singleton<Studio>.Instance.treeNodeCtrl;
			treeNodeCtrl.onSelect = (Action<TreeNodeObject>)Delegate.Combine(treeNodeCtrl.onSelect, new Action<TreeNodeObject>(OnSelect));
			Studio instance = Singleton<Studio>.Instance;
			instance.onDelete = (Action<ObjectCtrlInfo>)Delegate.Combine(instance.onDelete, new Action<ObjectCtrlInfo>(OnDelete));
			TreeNodeCtrl treeNodeCtrl2 = Singleton<Studio>.Instance.treeNodeCtrl;
			treeNodeCtrl2.onDeselect = (Action<TreeNodeObject>)Delegate.Combine(treeNodeCtrl2.onDeselect, new Action<TreeNodeObject>(OnDeselect));
		}
	}
}
