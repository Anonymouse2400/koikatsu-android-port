using System;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class RootButtonCtrl : MonoBehaviour
	{
		[Serializable]
		private class CommonInfo
		{
			public GameObject objRoot;

			public Button button;

			public Canvas canvas { get; set; }

			public virtual bool active
			{
				set
				{
					if (objRoot.activeSelf != value)
					{
						objRoot.SetActive(value);
						select = value;
					}
				}
			}

			public bool select
			{
				set
				{
					button.image.color = ((!value) ? Color.white : Color.green);
					SortCanvas.select = canvas;
				}
			}
		}

		[Serializable]
		private class ManipulateInfo : CommonInfo
		{
			[SerializeField]
			private ManipulatePanelCtrl m_ManipulatePanelCtrl;

			public ManipulatePanelCtrl manipulatePanelCtrl
			{
				get
				{
					if (m_ManipulatePanelCtrl == null)
					{
						m_ManipulatePanelCtrl = objRoot.GetComponent<ManipulatePanelCtrl>();
					}
					return m_ManipulatePanelCtrl;
				}
			}

			public override bool active
			{
				set
				{
					manipulatePanelCtrl.active = value;
					base.select = value;
				}
			}
		}

		[SerializeField]
		private CommonInfo add = new CommonInfo();

		[SerializeField]
		private ManipulateInfo manipulate = new ManipulateInfo();

		[SerializeField]
		private CommonInfo sound = new CommonInfo();

		[SerializeField]
		private CommonInfo system = new CommonInfo();

		[SerializeField]
		private Canvas canvas;

		private CommonInfo[] ciArray;

		public ObjectCtrlInfo objectCtrlInfo
		{
			set
			{
				manipulate.manipulatePanelCtrl.objectCtrlInfo = value;
			}
		}

		public int select { get; private set; }

		public void OnClick(int _kind)
		{
			select = ((select != _kind) ? _kind : (-1));
			for (int i = 0; i < ciArray.Length; i++)
			{
				ciArray[i].active = i == select;
			}
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		private void Start()
		{
			select = -1;
			ciArray = new CommonInfo[4] { add, manipulate, sound, system };
			for (int i = 0; i < ciArray.Length; i++)
			{
				ciArray[i].canvas = canvas;
				ciArray[i].active = false;
			}
		}
	}
}
