using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomFileListCtrlPointerEvent : CustomFileListCtrl
	{
		[SerializeField]
		[Header("ResetLayout")]
		private GameObject _resetLayout;

		[SerializeField]
		private Button[] _enter;

		[SerializeField]
		private Button[] _cancel;

		public Button[] enter
		{
			get
			{
				return _enter;
			}
		}

		public Button[] cancel
		{
			get
			{
				return _cancel;
			}
		}

		public event Action<CustomFileInfo> eventOnPointerClick = delegate
		{
		};

		public event Action<CustomFileInfo> eventOnPointerEnter = delegate
		{
		};

		public event Action<CustomFileInfo> eventOnPointerExit = delegate
		{
		};

		protected override void Start()
		{
			base.Start();
			if (_resetLayout != null)
			{
				if (_resetLayout.activeSelf)
				{
					_resetLayout.SetActive(false);
				}
				_resetLayout.SetActive(true);
			}
		}

		private static CustomFileInfo GetSelect(CustomFileInfoComponent fic)
		{
			return (!fic.tgl.isOn) ? null : fic.info;
		}

		public override bool OnPointerClick(CustomFileInfoComponent fic)
		{
			if (!base.OnPointerClick(fic))
			{
				return false;
			}
			this.eventOnPointerClick(GetSelect(fic));
			return true;
		}

		public override bool OnPointerEnter(CustomFileInfoComponent fic)
		{
			if (!base.OnPointerEnter(fic))
			{
				return false;
			}
			this.eventOnPointerEnter(GetSelect(fic));
			return true;
		}

		public override bool OnPointerExit(CustomFileInfoComponent fic)
		{
			if (!base.OnPointerExit(fic))
			{
				return false;
			}
			this.eventOnPointerExit(GetSelect(fic));
			return true;
		}
	}
}
