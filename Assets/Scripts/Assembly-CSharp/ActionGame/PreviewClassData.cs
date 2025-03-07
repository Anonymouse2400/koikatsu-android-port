using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class PreviewClassData
	{
		public enum VisibleMode
		{
			None = 0,
			CallName = 1,
			Personality = 2
		}

		private VisibleMode _visibleMode;

		private IntReactiveProperty _cardIndex = new IntReactiveProperty(0);

		private string callName;

		private string personality;

		private readonly StudentCardControlComponent[] _cards;

		private readonly GameObject _objHeart;

		private readonly GameObject _objMember;

		public GameObject rootObj { get; private set; }

		public SaveData.CharaData data { get; private set; }

		public Button button { get; private set; }

		public int classIndex { get; private set; }

		public bool isVisible
		{
			get
			{
				return rootObj.activeSelf;
			}
			set
			{
				rootObj.SetActive(value);
			}
		}

		public VisibleMode visibleMode
		{
			set
			{
				_visibleMode = value;
				bool flag = _visibleMode != 0 && data != null && data is SaveData.Heroine;
				switch (_visibleMode)
				{
				case VisibleMode.CallName:
					if (flag)
					{
						int fixCharaID = (data as SaveData.Heroine).fixCharaID;
						flag = fixCharaID == 0 || fixCharaID == -8;
					}
					break;
				case VisibleMode.Personality:
					if (flag)
					{
						flag = (data as SaveData.Heroine).fixCharaID == 0;
					}
					break;
				}
				int num = _cardIndex.Value - 1;
				if (num >= 0)
				{
					StudentCardControlComponent studentCardControlComponent = _cards[num];
					if (flag)
					{
						studentCardControlComponent.SetNickName((_visibleMode != VisibleMode.CallName) ? personality : callName);
					}
					studentCardControlComponent.ChangeDrawNickName(flag);
				}
			}
		}

		public PreviewClassData(GameObject rootObj)
		{
			PreviewClassData previewClassData = this;
			this.rootObj = rootObj;
			classIndex = int.Parse(rootObj.name.Split('_')[1]);
			button = rootObj.GetOrAddComponent<Button>();
			ColorBlock colors = button.colors;
			colors.disabledColor = Color.white;
			button.colors = colors;
			Navigation navigation = new Navigation
			{
				mode = Navigation.Mode.None
			};
			button.navigation = navigation;
			GameObject[] array = rootObj.Children().ToArray();
			Image component = array[0].GetComponent<Image>();
			_cards = (from p in array.Skip(1)
				select p.GetComponent<StudentCardControlComponent>() into p
				where p != null
				select p).ToArray();
			Image[] targetGraphics = new Image[1] { component }.Concat(_cards.Select((StudentCardControlComponent p) => p.BaseImage)).ToArray();
			var selArray = array.Take(targetGraphics.Length).Select((GameObject o, int i) => new { o, i }).ToArray();
			_cardIndex.Subscribe(delegate(int index)
			{
				var array2 = selArray;
				foreach (var anon in array2)
				{
					anon.o.SetActive(anon.i == index);
				}
				previewClassData.button.targetGraphic = targetGraphics[index];
			});
			GameObject[] array3 = array.Skip(targetGraphics.Length).ToArray();
			_objHeart = array3.SafeGet(0);
			_objMember = array3.SafeGet(1);
			Dictionary<int, Data.Param> self = rootObj.LoadTranslater(Localize.Translate.Manager.SCENE_ID.ID_CARD).Get(0);
			List<Data.Param> useList = new List<Data.Param>();
			foreach (var item in targetGraphics.Skip(1).Select((Image image, int ID) => new { image, ID }))
			{
				Data.Param ui = self.Get(item.ID);
				Localize.Translate.Manager.Convert(ui.Load(false)).SafeProc(delegate(Sprite sprite)
				{
					item.image.sprite = sprite;
					useList.Add(ui);
				});
			}
			if (_objMember != null)
			{
				Image image2 = _objMember.GetComponentInChildren<Image>();
				if (image2 != null)
				{
					Data.Param member = self.Get(3);
					Localize.Translate.Manager.Convert(member.Load(false)).SafeProc(delegate(Sprite sprite)
					{
						image2.sprite = sprite;
						useList.Add(member);
					});
				}
			}
			rootObj.OnDestroyAsObservable().Subscribe(delegate
			{
				Localize.Translate.Manager.Unload(useList);
			});
		}

		public void Update()
		{
			if (data != null)
			{
				Set(data);
			}
		}

		public void Clear()
		{
			StudentCardControlComponent[] cards = _cards;
			foreach (StudentCardControlComponent studentCardControlComponent in cards)
			{
				studentCardControlComponent.Clear(Singleton<Game>.Instance.saveData.accademyName);
			}
			_cardIndex.Value = 0;
			_objHeart.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(false);
			});
			_objMember.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(false);
			});
			data = null;
		}

		public void Set(SaveData.CharaData charaData)
		{
			SaveData.Heroine heroine = charaData as SaveData.Heroine;
			int num = ((heroine == null || heroine.fixCharaID == 0) ? 1 : ((!heroine.isTeacher) ? 3 : 2));
			_cardIndex.Value = num;
			data = charaData;
			callName = (data.callName = data.GetCallName());
			SaveData saveData = Singleton<Game>.Instance.saveData;
			StudentCardControlComponent studentCardControlComponent = _cards[num - 1];
			if (heroine != null)
			{
				VoiceInfo.Param value;
				if (Singleton<Voice>.Instance.voiceInfoDic.TryGetValue(heroine.FixCharaIDOrPersonality, out value))
				{
					personality = value.Personality;
				}
				else
				{
					personality = Localize.Translate.Manager.UnknownText;
				}
			}
			bool isHeart = false;
			bool isMember = false;
			if (heroine != null)
			{
				switch (num)
				{
				case 1:
					isHeart = heroine.isGirlfriend;
					isMember = heroine.isStaff;
					break;
				case 3:
					isHeart = heroine.isTaked;
					break;
				}
				Transform child = studentCardControlComponent.transform.GetChild(0);
				foreach (Transform item in from o in new GameObject[2] { _objMember, _objHeart }
					where o != null
					select o.transform)
				{
					item.SetParent(child, true);
					item.SetSiblingIndex(1);
				}
			}
			_objHeart.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isHeart);
			});
			_objMember.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isMember);
			});
			studentCardControlComponent.SetCharaInfo(charaData.charFile, saveData.emblemID, saveData.accademyName, (_visibleMode != VisibleMode.CallName) ? personality : callName);
			if (heroine != null && heroine.isTeacher)
			{
				studentCardControlComponent.SetChargeName(personality);
			}
		}
	}
}
