using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomInputNickNameWindow : MonoBehaviour
	{
		[SerializeField]
		private InputField inpNickName;

		[SerializeField]
		private Button btnYes;

		[SerializeField]
		private Button btnNo;

		[SerializeField]
		private Toggle tglReference;

		[SerializeField]
		private CvsChara cvsChara;

		private CustomBase customBase
		{
			get
			{
				return Singleton<CustomBase>.Instance;
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileParameter param
		{
			get
			{
				return chaCtrl.chaFile.parameter;
			}
		}

		public void UpdateUI()
		{
			inpNickName.text = param.nickname;
		}

		public void ActiveLastNameInput()
		{
			if ((bool)inpNickName)
			{
				inpNickName.ActivateInputField();
			}
		}

		private void Start()
		{
			customBase.lstInputField.Add(inpNickName);
			if ((bool)btnYes)
			{
				btnYes.OnClickAsObservable().Subscribe(delegate
				{
					if ((bool)tglReference)
					{
						tglReference.isOn = false;
					}
					cvsChara.ChangeNickName(inpNickName.text);
				});
			}
			if (!btnNo)
			{
				return;
			}
			btnNo.OnClickAsObservable().Subscribe(delegate
			{
				if ((bool)tglReference)
				{
					tglReference.isOn = false;
				}
			});
		}
	}
}
