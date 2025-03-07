using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsChara : MonoBehaviour
	{
		[SerializeField]
		private Toggle tglName;

		[SerializeField]
		private Text textName;

		[SerializeField]
		private CanvasGroup cgName;

		[SerializeField]
		private CustomInputNameWindow cmpNameWin;

		[SerializeField]
		private Toggle tglNickName;

		[SerializeField]
		private Text textNickName;

		[SerializeField]
		private CanvasGroup cgNickName;

		[SerializeField]
		private CustomInputNickNameWindow cmpNickNameWin;

		[SerializeField]
		private Toggle tglPersonality;

		[SerializeField]
		private TextMeshProUGUI textPersonality;

		[SerializeField]
		private CanvasGroup cgPersonality;

		[SerializeField]
		private CustomPersonalityWindow cmpPersonalityWin;

		[SerializeField]
		private Toggle[] tglBloodType;

		[SerializeField]
		private TMP_Dropdown ddBirthMonth;

		[SerializeField]
		private TMP_Dropdown ddBirthDay;

		[SerializeField]
		private TMP_Dropdown ddClubActivity;

		[SerializeField]
		private Slider sldPitchPow;

		[SerializeField]
		private TMP_InputField inpPitchPow;

		[SerializeField]
		private Button btnPitchPow;

		private AudioSource audioSource;

		private ShuffleRand shuffle = new ShuffleRand(3);

		private List<NickName.Param> lstNickSp;

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
				return customBase.chaCtrl;
			}
		}

		private ChaFileParameter param
		{
			get
			{
				return chaCtrl.chaFile.parameter;
			}
		}

		public void CalculateUI()
		{
			sldPitchPow.value = param.voiceRate;
			for (int i = 0; i < 4; i++)
			{
				tglBloodType[i].isOn = i == param.bloodType;
			}
			ddBirthMonth.value = param.birthMonth - 1;
			ddBirthDay.value = param.birthDay - 1;
			ddClubActivity.value = ConvertNoFromClubActivity(param.clubActivities);
			int[] array = customBase.dictPersonality.Keys.ToArray();
			string[] array2 = customBase.dictPersonality.Values.ToArray();
			int num = Array.IndexOf(array, param.personality);
			if (num == -1)
			{
				param.personality = array[0];
				textPersonality.text = array2[0];
			}
			else
			{
				textPersonality.text = array2[num];
			}
			textName.text = param.fullname;
			textNickName.text = param.nickname;
			cmpPersonalityWin.UpdateUI();
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		public int ConvertCallTypeFromNo(int no)
		{
			if (lstNickSp == null || no >= lstNickSp.Count)
			{
				return -1;
			}
			return lstNickSp[no].ID;
		}

		public int ConvertNoFromCallType(int type)
		{
			int num = lstNickSp.FindIndex((NickName.Param x) => x.ID == type);
			if (num == -1)
			{
				return 0;
			}
			return num;
		}

		public int ConvertClubActivityFromNo(int no)
		{
			int[] array = Game.ClubInfos.Keys.ToArray();
			if (array.Length <= no)
			{
				return -1;
			}
			return array[no];
		}

		public int ConvertNoFromClubActivity(int activity)
		{
			ClubInfo.Param value;
			if (!Game.ClubInfos.TryGetValue(activity, out value))
			{
				param.clubActivities = 0;
				return 0;
			}
			int[] array = Game.ClubInfos.Keys.ToArray();
			return Array.IndexOf(array, activity);
		}

		public void UpdateBirthDayDD()
		{
			int num = param.birthDay - 1;
			ddBirthDay.ClearOptions();
			int[] array = new int[12]
			{
				31, 29, 31, 30, 31, 30, 31, 31, 30, 31,
				30, 31
			};
			List<string> list = new List<string>();
			for (int i = 0; i < array[param.birthMonth - 1]; i++)
			{
				list.Add((i + 1).ToString());
			}
			ddBirthDay.AddOptions(list);
			if (num > array[param.birthMonth - 1] - 1)
			{
				ddBirthDay.value = 0;
			}
			else
			{
				ddBirthDay.value = num;
			}
			param.birthDay = (byte)(ddBirthDay.value + 1);
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			inpPitchPow.text = CustomBase.ConvertTextFromRate(0, 100, param.voiceRate);
		}

		public void ChangeName(string last, string first)
		{
			if (!customBase.updateCustomUI && (param.lastname != last || param.firstname != first))
			{
				param.lastname = last;
				param.firstname = first;
				textName.text = param.fullname;
				customBase.changeCharaName = true;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
			}
		}

		public void ChangeNickName(string nick)
		{
			if (!customBase.updateCustomUI && param.nickname != nick)
			{
				param.nickname = nick;
				textNickName.text = param.nickname;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
			}
		}

		public void ChangePersonailty(int personality, int idx)
		{
			if (!customBase.updateCustomUI && param.personality != personality)
			{
				param.personality = personality;
				string[] array = customBase.dictPersonality.Values.ToArray();
				textPersonality.text = array[idx];
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
			}
		}

		public void PlayVoice()
		{
			if (!customBase.playSampleVoice)
			{
				customBase.backEyebrowPtn = chaCtrl.fileStatus.eyebrowPtn;
				customBase.backEyesPtn = chaCtrl.fileStatus.eyesPtn;
				customBase.backBlink = chaCtrl.fileStatus.eyesBlink;
				customBase.backEyesOpen = chaCtrl.fileStatus.eyesOpenMax;
				customBase.backMouthPtn = chaCtrl.fileStatus.mouthPtn;
				customBase.backMouthFix = true;
				customBase.backMouthOpen = chaCtrl.fileStatus.mouthOpenMax;
			}
			ListInfoBase listInfo = Singleton<Character>.Instance.chaListCtrl.GetListInfo(ChaListDefine.CategoryNo.cha_sample_voice, param.personality);
			if (listInfo != null)
			{
				int num = shuffle.Get();
				ChaListDefine.KeyType[] array = new ChaListDefine.KeyType[3]
				{
					ChaListDefine.KeyType.Eyebrow01,
					ChaListDefine.KeyType.Eyebrow02,
					ChaListDefine.KeyType.Eyebrow03
				};
				ChaListDefine.KeyType[] array2 = new ChaListDefine.KeyType[3]
				{
					ChaListDefine.KeyType.Eye01,
					ChaListDefine.KeyType.Eye02,
					ChaListDefine.KeyType.Eye03
				};
				ChaListDefine.KeyType[] array3 = new ChaListDefine.KeyType[3]
				{
					ChaListDefine.KeyType.Mouth01,
					ChaListDefine.KeyType.Mouth02,
					ChaListDefine.KeyType.Mouth03
				};
				ChaListDefine.KeyType[] array4 = new ChaListDefine.KeyType[3]
				{
					ChaListDefine.KeyType.EyeHiLight01,
					ChaListDefine.KeyType.EyeHiLight02,
					ChaListDefine.KeyType.EyeHiLight03
				};
				ChaListDefine.KeyType[] array5 = new ChaListDefine.KeyType[3]
				{
					ChaListDefine.KeyType.Data01,
					ChaListDefine.KeyType.Data02,
					ChaListDefine.KeyType.Data03
				};
				chaCtrl.ChangeEyebrowPtn(listInfo.GetInfoInt(array[num]));
				chaCtrl.ChangeEyesPtn(listInfo.GetInfoInt(array2[num]));
				chaCtrl.HideEyeHighlight(("0" == listInfo.GetInfo(array4[num])) ? true : false);
				chaCtrl.ChangeEyesBlinkFlag(false);
				chaCtrl.ChangeEyesOpenMax(1f);
				chaCtrl.ChangeMouthPtn(listInfo.GetInfoInt(array3[num]));
				chaCtrl.ChangeMouthFixed(false);
				chaCtrl.ChangeMouthOpenMax(1f);
				customBase.playSampleVoice = true;
				Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.SystemSE);
				Utils.Sound.Setting setting = new Utils.Sound.Setting();
				setting.type = Manager.Sound.Type.SystemSE;
				setting.assetBundleName = listInfo.GetInfo(ChaListDefine.KeyType.MainAB);
				setting.assetName = listInfo.GetInfo(array5[num]);
				Transform transform = Utils.Sound.Play(setting);
				audioSource = transform.GetComponent<AudioSource>();
				audioSource.pitch = param.voicePitch;
				chaCtrl.SetVoiceTransform(transform);
			}
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			if (chaCtrl.sex == 0)
			{
				if ((bool)tglNickName)
				{
					tglNickName.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				if ((bool)tglPersonality)
				{
					tglPersonality.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				if ((bool)ddClubActivity)
				{
					ddClubActivity.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				if ((bool)sldPitchPow)
				{
					sldPitchPow.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
			}
			ddBirthDay.ClearOptions();
			List<string> list = new List<string>();
			for (int i = 1; i <= 31; i++)
			{
				list.Add(i.ToString());
			}
			ddBirthDay.AddOptions(list);
			List<string> options = Game.ClubInfos.Values.Select((ClubInfo.Param x) => x.Name).ToList();
			ddClubActivity.ClearOptions();
			ddClubActivity.AddOptions(options);
			customBase.lstTmpInputField.Add(inpPitchPow);
			customBase.actUpdateCvsChara += UpdateCustomUI;
			if ((bool)tglName)
			{
				tglName.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgName)
					{
						bool flag = ((cgName.alpha != 0f) ? true : false);
						if (flag != isOn)
						{
							cgName.Enable(isOn);
							if (isOn)
							{
								tglNickName.isOn = false;
								tglPersonality.isOn = false;
								cmpNameWin.UpdateUI();
								cmpNameWin.ActiveLastNameInput();
							}
						}
					}
				});
			}
			if ((bool)tglNickName)
			{
				tglNickName.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgNickName)
					{
						bool flag2 = ((cgNickName.alpha != 0f) ? true : false);
						if (flag2 != isOn)
						{
							cgNickName.Enable(isOn);
							if (isOn)
							{
								tglName.isOn = false;
								tglPersonality.isOn = false;
								cmpNickNameWin.UpdateUI();
								cmpNickNameWin.ActiveLastNameInput();
							}
						}
					}
				});
			}
			if ((bool)tglPersonality)
			{
				tglPersonality.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgPersonality)
					{
						bool flag3 = ((cgPersonality.alpha != 0f) ? true : false);
						if (flag3 != isOn)
						{
							cgPersonality.Enable(isOn);
							if (isOn)
							{
								tglName.isOn = false;
								tglNickName.isOn = false;
								cmpPersonalityWin.UpdateUI();
							}
						}
					}
				});
			}
			tglBloodType.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!customBase.updateCustomUI && isOn && param.bloodType != p.index)
					{
						param.bloodType = p.index;
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
					}
				});
			});
			ddBirthMonth.onValueChanged.AddListener(delegate(int idx)
			{
				if (!customBase.updateCustomUI && param.birthMonth != (byte)(idx + 1))
				{
					param.birthMonth = (byte)(idx + 1);
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
					UpdateBirthDayDD();
				}
			});
			ddBirthDay.onValueChanged.AddListener(delegate(int idx)
			{
				if (!customBase.updateCustomUI && param.birthDay != (byte)(idx + 1))
				{
					param.birthDay = (byte)(idx + 1);
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			ddClubActivity.onValueChanged.AddListener(delegate(int idx)
			{
				if (!customBase.updateCustomUI)
				{
					int num = ConvertNoFromClubActivity(param.clubActivities);
					if (num != idx)
					{
						param.clubActivities = (byte)ConvertClubActivityFromNo(idx);
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
					}
				}
			});
			sldPitchPow.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				param.voiceRate = value;
				inpPitchPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
				if (Singleton<Manager.Sound>.Instance.IsPlay(Manager.Sound.Type.SystemSE))
				{
					audioSource.pitch = param.voicePitch;
				}
			});
			sldPitchPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				if (!Singleton<Manager.Sound>.Instance.IsPlay(Manager.Sound.Type.SystemSE))
				{
					PlayVoice();
				}
			});
			sldPitchPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPitchPow.value = Mathf.Clamp(sldPitchPow.value + scl.scrollDelta.y * customBase.sliderWheelSensitive, 0f, 100f);
				inpPitchPow.text = CustomBase.ConvertTextFromRate(0, 100, sldPitchPow.value);
			});
			inpPitchPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPitchPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
			});
			btnPitchPow.onClick.AsObservable().Subscribe(delegate
			{
				param.voiceRate = 0.5f;
				inpPitchPow.text = CustomBase.ConvertTextFromRate(0, 100, 0.5f);
				sldPitchPow.value = 0.5f;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
			});
			StartCoroutine(SetInputText());
		}
	}
}
