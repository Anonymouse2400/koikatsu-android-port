using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using Illusion;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ChaStatusScene : BaseLoader
{
	[SerializeField]
	private CanvasGroup cgStatus;

	[SerializeField]
	private ChaStatusComponent cmpMale;

	[SerializeField]
	private ChaStatusComponent cmpFix;

	[SerializeField]
	private ChaStatusComponent cmpTeacher;

	[SerializeField]
	private ChaStatusComponent cmpFemale;

	[Header("一列目を主人公とシナリオor先生キャラに限定するためのダミー")]
	[SerializeField]
	private GameObject dummy;

	[SerializeField]
	private Button btnChangeInfo;

	[SerializeField]
	private Button btnSortFavor;

	[SerializeField]
	private Button btnSortName;

	[SerializeField]
	private Button btnClose;

	private const string FORMAT_PERCENT = "{0} %";

	private string _FORMAT_NUM;

	private string _FORMAT_COUNT;

	private Dictionary<int, Data.Param> _uiTranslater;

	private bool isEnd;

	private string FORMAT_NUM
	{
		get
		{
			return this.GetCache(ref _FORMAT_NUM, () => Localize.Translate.Manager.OtherData.Get(4).Values.FindTagText("Members") ?? "{0} 人");
		}
	}

	private string FORMAT_COUNT
	{
		get
		{
			return this.GetCache(ref _FORMAT_COUNT, () => Localize.Translate.Manager.OtherData.Get(4).Values.FindTagText("Times") ?? "{0} 回");
		}
	}

	private Dictionary<int, Data.Param> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.ID_CARD).Get(0));
		}
	}

	private bool isStatus
	{
		get
		{
			return Singleton<Scene>.Instance.NowSceneNames[0] == "Status";
		}
	}

	public void Unload()
	{
		if (isStatus && !isEnd)
		{
			isEnd = true;
			cgStatus.blocksRaycasts = false;
			Singleton<Scene>.Instance.UnLoad();
		}
	}

	private void Start()
	{
		cgStatus.blocksRaycasts = true;
		Singleton<Game>.Instance.ParameterCorrectValues();
		SaveData saveData = Singleton<Game>.Instance.saveData;
		ActionScene actScene = Singleton<Game>.Instance.actScene;
		Action<StudentCardControlComponent, int> action = null;
		List<Data.Param> useLocalizeDataList = new List<Data.Param>();
		if (uiTranslater.Any())
		{
			this.OnDestroyAsObservable().Subscribe(delegate
			{
				Localize.Translate.Manager.Unload(useLocalizeDataList);
			});
			action = delegate(StudentCardControlComponent card, int id)
			{
				Data.Param param = uiTranslater.Get(id);
				Localize.Translate.Manager.Convert(param.Load(false)).SafeProcObject(delegate(Sprite sprite)
				{
					card.BaseImage.sprite = sprite;
				});
				useLocalizeDataList.Add(param);
			};
		}
		Dictionary<int, Data.Param> mapDataLT = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.MAP).Get(0);
		Func<int, string> mapNameConverter = delegate(int mapNo)
		{
			string text = mapDataLT.SafeGetText(mapNo);
			if (text == null)
			{
				switch (mapNo)
				{
				case 2:
					text = "廊下１Ｆ";
					break;
				case 3:
					text = "廊下２Ｆ";
					break;
				case 4:
					text = "廊下３Ｆ";
					break;
				default:
					text = actScene.Map.ConvertMapName(mapNo);
					break;
				}
			}
			return text;
		};
		if (cmpMale != null)
		{
			SaveData.Player player = saveData.player;
			if (cmpMale.cmpStudentCard != null)
			{
				action.Call(cmpMale.cmpStudentCard, 0);
				cmpMale.cmpStudentCard.SetCharaInfo(player.charFile, saveData.emblemID, saveData.accademyName, string.Empty);
			}
			TextMeshProUGUI[] array = cmpMale.GetinfoTexts(0);
			if (array != null)
			{
				int num = 0;
				SaveData.Heroine[] heroines = saveData.heroineList.Where((SaveData.Heroine h) => h.fixCharaID == 0).ToArray();
				array.SafeProc(num++, delegate(TextMeshProUGUI t)
				{
					t.text = string.Format(FORMAT_NUM, heroines.Count((SaveData.Heroine h) => h.isStaff));
				});
				array.SafeProc(num++, delegate(TextMeshProUGUI t)
				{
					t.text = string.Format(FORMAT_NUM, heroines.Count((SaveData.Heroine h) => h.isGirlfriend));
				});
				array.SafeProc(num++, delegate(TextMeshProUGUI t)
				{
					t.text = player.physical.ToString();
				});
				array.SafeProc(num++, delegate(TextMeshProUGUI t)
				{
					t.text = player.intellect.ToString();
				});
				array.SafeProc(num++, delegate(TextMeshProUGUI t)
				{
					t.text = player.hentai.ToString();
				});
			}
		}
		SaveData.Heroine fixHeroine = ((!(actScene.fixChara == null)) ? actScene.fixChara.heroine : null);
		bool flag = fixHeroine != null;
		dummy.SetActiveIfDifferent(!flag);
		if (!flag)
		{
			cmpTeacher.gameObject.SetActiveIfDifferent(false);
			cmpFix.gameObject.SetActiveIfDifferent(false);
		}
		else
		{
			bool isTeacher = fixHeroine.isTeacher;
			cmpTeacher.gameObject.SetActiveIfDifferent(isTeacher);
			cmpFix.gameObject.SetActiveIfDifferent(!isTeacher);
			ChaStatusComponent chaStatusComponent = ((!isTeacher) ? cmpFix : cmpTeacher);
			if (chaStatusComponent != null)
			{
				StudentCardControlComponent cmpStudentCard = chaStatusComponent.cmpStudentCard;
				if (cmpStudentCard != null)
				{
					action.Call(cmpStudentCard, isTeacher ? 1 : 2);
					cmpStudentCard.SetCharaInfo(fixHeroine.charFile, saveData.emblemID, saveData.accademyName, string.Empty);
					if (isTeacher)
					{
						cmpStudentCard.SetChargeName(Singleton<Voice>.Instance.voiceInfoDic[fixHeroine.voiceNo].Personality);
					}
				}
				TextMeshProUGUI[] array2 = chaStatusComponent.GetinfoTexts(0);
				array2.SafeProc(0, delegate(TextMeshProUGUI t)
				{
					t.text = mapNameConverter(fixHeroine.charaBase.mapNo);
				});
				if (isTeacher)
				{
					array2.SafeProc(1, delegate(TextMeshProUGUI t)
					{
						t.text = string.Format("{0} %", fixHeroine.lewdness);
					});
				}
			}
		}
		Transform cmpFemaleParent = cmpFemale.transform.parent;
		string cmpFemaleBaseName = cmpFemale.name;
		action.Call(cmpFemale.cmpStudentCard, 0);
		Image image = cmpFemale.objMember.GetComponentInChildren<Image>();
		if (image != null)
		{
			Data.Param member = uiTranslater.Get(3);
			Localize.Translate.Manager.Convert(member.Load(false)).SafeProcObject(delegate(Sprite sprite)
			{
				image.sprite = sprite;
				useLocalizeDataList.Add(member);
			});
		}
		ChaStatusComponent[] cmpFemales = actScene.npcList.Select((NPC npc) => npc.heroine).Select(delegate(SaveData.Heroine heroine, int i)
		{
			ChaStatusComponent chaStatusComponent2 = UnityEngine.Object.Instantiate(cmpFemale, cmpFemaleParent, false);
			chaStatusComponent2.name = cmpFemaleBaseName + i.ToString("00");
			chaStatusComponent2.heroine = heroine;
			return chaStatusComponent2;
		}).ToArray();
		string[] relationLabels;
		string[] clubJoinLabels;
		string[] dateLabels;
		string[] stateLabels;
		string[] hExperienceLabels;
		if (Localize.Translate.Manager.isTranslate)
		{
			Dictionary<int, Data.Param> dictionary = actScene.uiTranslater.Get(1);
			relationLabels = dictionary.Values.ToArray("Relation");
			clubJoinLabels = dictionary.Values.ToArray("ClubJoin");
			dateLabels = dictionary.Values.ToArray("Date");
			stateLabels = dictionary.Values.ToArray("State");
			hExperienceLabels = dictionary.Values.ToArray("H");
		}
		else
		{
			relationLabels = new string[4] { "初対面", "知り合い", "友達", "恋人" };
			clubJoinLabels = new string[2] { "所属", "未所属" };
			dateLabels = new string[2] { "約束あり", "未定" };
			stateLabels = new string[2] { "普通", "怒り" };
			hExperienceLabels = Illusion.Utils.Enum<SaveData.Heroine.HExperienceKind>.Names;
		}
		ChaStatusComponent[] array3 = cmpFemales;
		foreach (ChaStatusComponent chaStatusComponent3 in array3)
		{
			SaveData.Heroine heroine2 = chaStatusComponent3.heroine;
			if (chaStatusComponent3.cmpStudentCard != null)
			{
				chaStatusComponent3.cmpStudentCard.SetCharaInfo(heroine2.charFile, saveData.emblemID, saveData.accademyName, string.Empty);
			}
			TextMeshProUGUI[] array4 = chaStatusComponent3.GetinfoTexts(0);
			int num2 = 0;
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = mapNameConverter(heroine2.charaBase.mapNo);
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = Singleton<Voice>.Instance.voiceInfoDic[heroine2.voiceNo].Personality;
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = clubJoinLabels[(!heroine2.isStaff) ? 1u : 0u];
			});
			chaStatusComponent3.objMember.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(heroine2.isStaff);
			});
			bool isGirlfriend = heroine2.isGirlfriend;
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = relationLabels[heroine2.relation + 1];
			});
			chaStatusComponent3.objHeart.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isGirlfriend);
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = dateLabels[(!heroine2.isDate) ? 1u : 0u];
			});
			array4 = chaStatusComponent3.GetinfoTexts(1);
			num2 = 0;
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = stateLabels[heroine2.isAnger ? 1 : 0];
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = string.Format("{0} %", heroine2.favor);
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = string.Format("{0} %", heroine2.lewdness);
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = hExperienceLabels[(int)heroine2.HExperience];
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.text = string.Format(FORMAT_COUNT, heroine2.hCount);
			});
			array4 = chaStatusComponent3.GetinfoTexts(2);
			num2 = 0;
			if (!Game.isAdd20)
			{
				chaStatusComponent3.infoObject.SafeProc(2, delegate(ChaStatusComponent.InfoObject o)
				{
					o.enabled = false;
				});
			}
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				Transform parent = t.transform.parent;
				parent.gameObject.SetActiveIfDifferent(isGirlfriend);
				if (isGirlfriend)
				{
					Image component = parent.GetChild(0).GetComponent<Image>();
					int intimacy = heroine2.intimacy;
					component.fillAmount = Mathf.InverseLerp(0f, 100f, intimacy);
					t.text = intimacy.ToString();
				}
			});
			array4.SafeProc(num2++, delegate(TextMeshProUGUI t)
			{
				t.gameObject.SetActiveIfDifferent(!isGirlfriend);
			});
			chaStatusComponent3.gameObject.SetActive(true);
		}
		(from _ in btnChangeInfo.OnClickAsObservable()
			select 1).Scan((int sum, int cur) => cur + sum).Subscribe(delegate(int count)
		{
			Illusion.Game.Utils.Sound.Play(SystemSE.sel);
			ChaStatusComponent[] array5 = cmpFemales;
			foreach (ChaStatusComponent chaStatusComponent4 in array5)
			{
				chaStatusComponent4.SetActiveInfoAll(count);
			}
		});
		SortFavor(cmpFemales);
		btnSortFavor.OnClickAsObservable().Subscribe(delegate
		{
			Illusion.Game.Utils.Sound.Play(SystemSE.sel);
			SortFavor(cmpFemales);
		});
		btnSortName.OnClickAsObservable().Subscribe(delegate
		{
			Illusion.Game.Utils.Sound.Play(SystemSE.sel);
			SortName(cmpFemales);
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			select _).Subscribe(delegate
		{
			Unload();
		});
		btnClose.OnClickAsObservable().Subscribe(delegate
		{
			Illusion.Game.Utils.Sound.Play(SystemSE.cancel);
			Unload();
		});
	}

	private void SortFavor(IEnumerable<ChaStatusComponent> source)
	{
		Localize.Translate.Manager.SetCulture(delegate
		{
			foreach (ChaStatusComponent item in from p in source
				orderby p.heroine.relation descending, p.heroine.favor descending, p.heroine.parameter.fullname
				select p)
			{
				item.transform.SetAsLastSibling();
			}
		});
	}

	private void SortName(IEnumerable<ChaStatusComponent> source)
	{
		Localize.Translate.Manager.SetCulture(delegate
		{
			foreach (ChaStatusComponent item in from p in source
				orderby p.heroine.parameter.fullname, p.heroine.relation descending, p.heroine.favor descending
				select p)
			{
				item.transform.SetAsLastSibling();
			}
		});
	}
}
