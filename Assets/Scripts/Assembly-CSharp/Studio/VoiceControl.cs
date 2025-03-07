using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class VoiceControl : MonoBehaviour
	{
		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private Button buttonRepeat;

		[SerializeField]
		private Sprite[] spriteRepeat;

		[SerializeField]
		private Button buttonStop;

		[SerializeField]
		private Button buttonPlay;

		[SerializeField]
		private Image imagePlayNow;

		[SerializeField]
		private Button buttonPlayAll;

		[SerializeField]
		private Button buttonStopAll;

		[SerializeField]
		private Button buttonExpansion;

		[SerializeField]
		private Sprite[] spriteExpansion;

		[SerializeField]
		private GameObject objBeneath;

		[SerializeField]
		private Button buttonSave;

		[SerializeField]
		private Button buttonDeleteAll;

		[SerializeField]
		private VoiceRegistrationList voiceRegistrationList;

		private OCIChar m_OCIChar;

		private List<VoicePlayNode> listNode = new List<VoicePlayNode>();

		private int select = -1;

		public OCIChar ociChar
		{
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

		public void InitList()
		{
			for (int i = 0; i < transformRoot.childCount; i++)
			{
				UnityEngine.Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			select = -1;
			listNode.Clear();
			foreach (VoiceCtrl.VoiceInfo item in m_OCIChar.voiceCtrl.list)
			{
				Info.LoadCommonInfo voiceInfo = Singleton<Info>.Instance.GetVoiceInfo(item.group, item.category, item.no);
				if (voiceInfo != null)
				{
					AddNode(voiceInfo.name);
				}
			}
			scrollRect.verticalNormalizedPosition = 1f;
			imagePlayNow.enabled = m_OCIChar != null && m_OCIChar.voiceCtrl.isPlay;
			buttonRepeat.image.sprite = spriteRepeat[(int)m_OCIChar.voiceRepeat];
			voiceRegistrationList.ociChar = m_OCIChar;
		}

		private void AddNode(string _name)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(objectNode);
			gameObject.transform.SetParent(transformRoot, false);
			VoicePlayNode vpn = gameObject.GetComponent<VoicePlayNode>();
			vpn.active = true;
			vpn.addOnClick = delegate
			{
				OnClickSelect(vpn);
			};
			vpn.addOnClickDelete = delegate
			{
				OnClickDelete(vpn);
			};
			vpn.text = _name;
			listNode.Add(vpn);
		}

		private void OnClickRepeat()
		{
			int num = Enum.GetNames(typeof(VoiceCtrl.Repeat)).Length;
			int voiceRepeat = (int)m_OCIChar.voiceRepeat;
			voiceRepeat = (voiceRepeat + 1) % num;
			m_OCIChar.voiceRepeat = (VoiceCtrl.Repeat)voiceRepeat;
			buttonRepeat.image.sprite = spriteRepeat[voiceRepeat];
		}

		private void OnClickStop()
		{
			m_OCIChar.StopVoice();
		}

		private void OnClickPlay()
		{
			bool flag = m_OCIChar.PlayVoice(select);
			imagePlayNow.enabled = flag;
		}

		private void OnClickPlayAll()
		{
			OCIChar[] array = (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey
				select Studio.GetCtrlInfo(v) as OCIChar into v
				where v != null
				select v).ToArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				array[i].PlayVoice(0);
			}
		}

		private void OnClickStopAll()
		{
			OCIChar[] array = (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey
				select Studio.GetCtrlInfo(v) as OCIChar into v
				where v != null
				select v).ToArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				array[i].StopVoice();
			}
		}

		private void OnClickExpansion()
		{
			bool flag = !objBeneath.activeSelf;
			objBeneath.SetActive(flag);
			buttonExpansion.image.sprite = spriteExpansion[flag ? 1 : 0];
		}

		private void OnClickSave()
		{
			voiceRegistrationList.active = !voiceRegistrationList.active;
			if (voiceRegistrationList.active)
			{
				voiceRegistrationList.ociChar = m_OCIChar;
			}
		}

		private void OnClickDeleteAll()
		{
			int count = listNode.Count;
			for (int i = 0; i < count; i++)
			{
				listNode[i].Destroy();
			}
			listNode.Clear();
			scrollRect.verticalNormalizedPosition = 1f;
			m_OCIChar.DeleteAllVoice();
		}

		private void OnClickSelect(VoicePlayNode _vpn)
		{
			if (MathfEx.RangeEqualOn(0, select, listNode.Count))
			{
				listNode[select].select = false;
			}
			select = listNode.FindIndex((VoicePlayNode v) => v == _vpn);
			listNode[select].select = true;
		}

		private void OnClickDelete(VoicePlayNode _vpn)
		{
			int num = listNode.FindIndex((VoicePlayNode v) => v == _vpn);
			listNode.RemoveAt(num);
			_vpn.Destroy();
			scrollRect.verticalNormalizedPosition = 1f;
			m_OCIChar.DeleteVoice(num);
			if (select == num)
			{
				select = -1;
			}
		}

		private void Start()
		{
			buttonRepeat.onClick.AddListener(OnClickRepeat);
			buttonStop.onClick.AddListener(OnClickStop);
			buttonPlay.onClick.AddListener(OnClickPlay);
			buttonPlayAll.onClick.AddListener(OnClickPlayAll);
			buttonStopAll.onClick.AddListener(OnClickStopAll);
			buttonExpansion.onClick.AddListener(OnClickExpansion);
			buttonSave.onClick.AddListener(OnClickSave);
			buttonDeleteAll.onClick.AddListener(OnClickDeleteAll);
		}

		private void Update()
		{
			if (imagePlayNow.enabled)
			{
				imagePlayNow.enabled = m_OCIChar != null && m_OCIChar.voiceCtrl.isPlay;
			}
		}
	}
}
