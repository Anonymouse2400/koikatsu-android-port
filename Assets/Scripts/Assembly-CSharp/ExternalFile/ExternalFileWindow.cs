using System;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ExternalFile
{
	public class ExternalFileWindow : MonoBehaviour
	{
		public enum FileWindowType
		{
			BackImage = 0,
			BackFrame = 1,
			FrontFrame = 2
		}

		[Serializable]
		public class TypeReactiveProperty : ReactiveProperty<FileWindowType>
		{
			public TypeReactiveProperty()
			{
			}

			public TypeReactiveProperty(FileWindowType initialValue)
				: base(initialValue)
			{
			}
		}

		[SerializeField]
		private TypeReactiveProperty _fwType = new TypeReactiveProperty(FileWindowType.BackImage);

		[SerializeField]
		private TextMeshProUGUI textTitle;

		[SerializeField]
		private Button btnClose;

		[SerializeField]
		private Button _btnLoad;

		public FileWindowType fwType
		{
			get
			{
				return _fwType.Value;
			}
			set
			{
				_fwType.Value = value;
			}
		}

		public Button btnLoad
		{
			get
			{
				return _btnLoad;
			}
		}

		public void Awake()
		{
		}

		public void Start()
		{
			_fwType.TakeUntilDestroy(this).Subscribe(delegate
			{
				UpdateWindow();
			});
			if ((bool)btnClose)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					base.gameObject.SetActiveIfDifferent(false);
				});
			}
		}

		public void UpdateWindow()
		{
			switch (fwType)
			{
			case FileWindowType.BackImage:
				textTitle.text = "背景画像";
				break;
			case FileWindowType.BackFrame:
				textTitle.text = "背面フレーム";
				break;
			case FileWindowType.FrontFrame:
				textTitle.text = "前面フレーム";
				break;
			}
		}
	}
}
