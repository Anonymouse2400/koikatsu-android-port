using Illusion.Extensions;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomInputNameWindow : MonoBehaviour
	{
		[SerializeField]
		private InputField inpLastName;

		[SerializeField]
		private GameObject objLastNameDummy;

		[SerializeField]
		private Text textLastNameDummy;

		[SerializeField]
		private InputField inpFirstName;

		[SerializeField]
		private GameObject objFirstNameDummy;

		[SerializeField]
		private Text textFirstNameDummy;

		[SerializeField]
		private Button btnLastRand;

		[SerializeField]
		private Button btnFirstRand;

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
			inpLastName.text = param.lastname;
			inpFirstName.text = param.firstname;
		}

		public void ActiveLastNameInput()
		{
			if ((bool)inpLastName)
			{
				inpLastName.ActivateInputField();
			}
		}

		private void Start()
		{
			customBase.lstInputField.Add(inpLastName);
			customBase.lstInputField.Add(inpFirstName);
			if ((bool)inpLastName)
			{
				inpLastName.OnValueChangedAsObservable().Subscribe(delegate
				{
					if ((bool)textLastNameDummy)
					{
						textLastNameDummy.text = inpLastName.text;
					}
				});
			}
			if ((bool)inpFirstName)
			{
				inpFirstName.OnValueChangedAsObservable().Subscribe(delegate
				{
					if ((bool)textFirstNameDummy)
					{
						textFirstNameDummy.text = inpFirstName.text;
					}
				});
			}
			if ((bool)btnLastRand)
			{
				btnLastRand.OnClickAsObservable().Subscribe(delegate
				{
					ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
					Character.RandamNameInfo randamNameInfo = Singleton<Character>.Instance.randamNameInfo;
					if (ChaRandom.GetRandomIndex(95, 5) == 0)
					{
						inpLastName.text = randamNameInfo.lstLastNameH[Random.Range(0, randamNameInfo.lstLastNameH.Count)];
					}
					else
					{
						inpLastName.text = randamNameInfo.lstLastNameK[Random.Range(0, randamNameInfo.lstLastNameK.Count)];
					}
				});
			}
			if ((bool)btnFirstRand)
			{
				btnFirstRand.OnClickAsObservable().Subscribe(delegate
				{
					ChaListControl chaListCtrl2 = Singleton<Character>.Instance.chaListCtrl;
					Character.RandamNameInfo randamNameInfo2 = Singleton<Character>.Instance.randamNameInfo;
					if (ChaRandom.GetRandomIndex(95, 5) == 0)
					{
						if (param.sex == 0)
						{
							inpFirstName.text = randamNameInfo2.lstMaleFirstNameH[Random.Range(0, randamNameInfo2.lstMaleFirstNameH.Count)];
						}
						else
						{
							inpFirstName.text = randamNameInfo2.lstFemaleFirstNameH[Random.Range(0, randamNameInfo2.lstFemaleFirstNameH.Count)];
						}
					}
					else if (param.sex == 0)
					{
						inpFirstName.text = randamNameInfo2.lstMaleFirstNameK[Random.Range(0, randamNameInfo2.lstMaleFirstNameK.Count)];
					}
					else
					{
						inpFirstName.text = randamNameInfo2.lstFemaleFirstNameK[Random.Range(0, randamNameInfo2.lstFemaleFirstNameK.Count)];
					}
				});
			}
			if ((bool)btnYes)
			{
				btnYes.OnClickAsObservable().Subscribe(delegate
				{
					if ((bool)tglReference)
					{
						tglReference.isOn = false;
					}
					cvsChara.ChangeName(inpLastName.text, inpFirstName.text);
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

		private void Update()
		{
			if ((bool)objLastNameDummy && (bool)inpLastName)
			{
				bool isFocused = inpLastName.isFocused;
				if (objLastNameDummy.activeSelf == isFocused)
				{
					objLastNameDummy.SetActiveIfDifferent(!isFocused);
				}
				if (isFocused && Input.GetKeyDown(KeyCode.Tab))
				{
					inpFirstName.ActivateInputField();
				}
			}
			if ((bool)objFirstNameDummy && (bool)inpFirstName)
			{
				bool isFocused2 = inpFirstName.isFocused;
				if (objFirstNameDummy.activeSelf == isFocused2)
				{
					objFirstNameDummy.SetActiveIfDifferent(!isFocused2);
				}
				if (isFocused2 && Input.GetKeyDown(KeyCode.Tab))
				{
					inpLastName.ActivateInputField();
				}
			}
		}
	}
}
