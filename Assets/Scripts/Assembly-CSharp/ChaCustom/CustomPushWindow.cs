using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomPushWindow : MonoBehaviour
	{
		public enum SelectWindowType
		{
			MoleLayout = 0,
			FacePaint01Layout = 1,
			FacePaint02Layout = 2
		}

		[Serializable]
		public class TypeReactiveProperty : ReactiveProperty<SelectWindowType>
		{
			public TypeReactiveProperty()
			{
			}

			public TypeReactiveProperty(SelectWindowType initialValue)
				: base(initialValue)
			{
			}
		}

		[SerializeField]
		private TypeReactiveProperty _pwType = new TypeReactiveProperty(SelectWindowType.MoleLayout);

		[SerializeField]
		private TextMeshProUGUI textTitle;

		[SerializeField]
		private Button btnClose;

		[SerializeField]
		private Toggle tglReference;

		public SelectWindowType pwType
		{
			get
			{
				return _pwType.Value;
			}
			set
			{
				_pwType.Value = value;
			}
		}

		public void Awake()
		{
		}

		public void Start()
		{
			_pwType.TakeUntilDestroy(this).Subscribe(delegate
			{
				switch (pwType)
				{
				case SelectWindowType.MoleLayout:
					ChangeTitle(Singleton<CustomBase>.Instance.TranslateSetPositionTitle(0) ?? "ホクロ配置");
					break;
				case SelectWindowType.FacePaint01Layout:
					ChangeTitle(Singleton<CustomBase>.Instance.TranslateSetPositionTitle(1) ?? "フェイスペイント０１配置");
					break;
				case SelectWindowType.FacePaint02Layout:
					ChangeTitle(Singleton<CustomBase>.Instance.TranslateSetPositionTitle(2) ?? "フェイスペイント０２配置");
					break;
				}
			});
			if (!btnClose)
			{
				return;
			}
			btnClose.OnClickAsObservable().Subscribe(delegate
			{
				if ((bool)tglReference)
				{
					tglReference.isOn = false;
				}
			});
		}

		public void ChangeTitle(string title)
		{
			if (!(null == textTitle))
			{
				textTitle.text = title;
			}
		}
	}
}
