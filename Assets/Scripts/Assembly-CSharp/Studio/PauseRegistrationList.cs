using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class PauseRegistrationList : MonoBehaviour
	{
		[SerializeField]
		private Button buttonClose;

		[SerializeField]
		private InputField inputName;

		[SerializeField]
		private Button buttonSave;

		[SerializeField]
		private Button buttonLoad;

		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject prefabNode;

		[SerializeField]
		private Button buttonDelete;

		[SerializeField]
		private Sprite spriteDelete;

		private OCIChar m_OCIChar;

		private List<string> listPath;

		private Dictionary<int, StudioNode> dicNode = new Dictionary<int, StudioNode>();

		private int select = -1;

		public OCIChar ociChar
		{
			get
			{
				return m_OCIChar;
			}
			set
			{
				m_OCIChar = value;
				if (m_OCIChar != null)
				{
					InitList();
				}
			}
		}

		public bool active
		{
			get
			{
				return base.gameObject.activeSelf;
			}
			set
			{
				if (base.gameObject.activeSelf != value)
				{
					base.gameObject.SetActive(value);
				}
			}
		}

		private void OnClickClose()
		{
			base.gameObject.SetActive(false);
		}

		private void OnEndEditName(string _text)
		{
			buttonSave.interactable = !_text.IsNullOrEmpty();
		}

		private void OnClickSave()
		{
			PauseCtrl.Save(ociChar, inputName.text);
			InitList();
		}

		private void OnClickLoad()
		{
			PauseCtrl.Load(ociChar, listPath[select]);
		}

		private void OnClickDelete()
		{
			CheckScene.sprite = spriteDelete;
			CheckScene.unityActionYes = OnSelectDeleteYes;
			CheckScene.unityActionNo = OnSelectDeleteNo;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioCheck",
				isAdd = true
			}, false);
		}

		private void OnSelectDeleteYes()
		{
			Singleton<Scene>.Instance.UnLoad();
			File.Delete(listPath[select]);
			InitList();
		}

		private void OnSelectDeleteNo()
		{
			Singleton<Scene>.Instance.UnLoad();
		}

		private void OnClickSelect(int _no)
		{
			StudioNode value = null;
			if (dicNode.TryGetValue(select, out value))
			{
				value.select = false;
			}
			select = _no;
			if (dicNode.TryGetValue(select, out value))
			{
				value.select = true;
			}
			if (select != -1)
			{
				buttonLoad.interactable = true;
				buttonDelete.interactable = true;
			}
		}

		private void InitList()
		{
			for (int i = 0; i < transformRoot.childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			select = -1;
			buttonLoad.interactable = false;
			buttonDelete.interactable = false;
			int sex = m_OCIChar.oiCharInfo.sex;
			listPath = (from v in Directory.GetFiles(UserData.Create("studio/pose"), "*.dat")
				where PauseCtrl.CheckIdentifyingCode(v, sex)
				select v).ToList();
			dicNode.Clear();
			for (int j = 0; j < listPath.Count; j++)
			{
				GameObject gameObject = Object.Instantiate(prefabNode);
				gameObject.transform.SetParent(transformRoot, false);
				StudioNode component = gameObject.GetComponent<StudioNode>();
				component.active = true;
				int no = j;
				component.addOnClick = delegate
				{
					OnClickSelect(no);
				};
				component.text = PauseCtrl.LoadName(listPath[j]);
				dicNode.Add(j, component);
			}
		}

		private void Awake()
		{
			buttonClose.onClick.AddListener(OnClickClose);
			inputName.onEndEdit.AddListener(OnEndEditName);
			buttonSave.onClick.AddListener(OnClickSave);
			buttonSave.interactable = false;
			buttonLoad.onClick.AddListener(OnClickLoad);
			buttonDelete.onClick.AddListener(OnClickDelete);
		}
	}
}
