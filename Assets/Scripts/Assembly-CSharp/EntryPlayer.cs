using System;
using System.Linq;
using CharaFiles;
using Illusion.CustomAttributes;
using Illusion.Game;
using Illusion.Game.Extensions;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class EntryPlayer : BaseLoader
{
	[SerializeField]
	[Label("キャラリスト")]
	private ChaFileListCtrl chaFileLists;

	[SerializeField]
	[Label("エンブレムリスト")]
	private EmblemSelectListCtrl emblemLists;

	[SerializeField]
	[Label("学園名入力")]
	private InputField inpAccademy;

	[SerializeField]
	[Label("学生証")]
	private StudentCardControlComponent cmpStudent;

	[SerializeField]
	[Label("キャンバス")]
	private Canvas Canvas;

	[SerializeField]
	private CanvasGroup cgEntryPlayer;

	[SerializeField]
	[Label("決定ボタン")]
	private Button btnNext;

	[SerializeField]
	[Label("戻るボタン")]
	private Button btnPrev;

	private ChaFileControl chaFileCtrl;

	private int emblemId;

	private string accademyName = "コイカツ女学園";

	private bool noPlaySSE;

	private void Start()
	{
		if (cgEntryPlayer != null)
		{
			this.ObserveEveryValueChanged((EntryPlayer _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				cgEntryPlayer.interactable = isOn;
				cgEntryPlayer.blocksRaycasts = isOn;
			});
		}
		string defaultAccademyName = (inpAccademy.placeholder as Text).text;
		accademyName = defaultAccademyName;
		this.SelectSEAdd(btnNext, btnPrev);
		btnNext.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
		});
		btnPrev.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
		});
		SaveData saveData = Singleton<Game>.Instance.saveData;
		SaveData.Player player = (saveData.player = new SaveData.Player(false));
		Localize.Translate.Manager.Convert(base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.ID_CARD).Get(0).Get(0)
			.Load(true)).SafeProcObject(delegate(Sprite sprite)
		{
			cmpStudent.BaseImage.sprite = sprite;
		});
		CardUpdate();
		noPlaySSE = true;
		CreateMaleList();
		chaFileLists.SelectItem(0);
		CreateEmblemList();
		emblemLists.SelectItem(emblemId);
		noPlaySSE = false;
		cmpStudent.SetAccademy(accademyName);
		(from buf in inpAccademy.OnEndEditAsObservable()
			select buf.IsNullOrEmpty() ? defaultAccademyName : buf).Subscribe(delegate(string buf)
		{
			accademyName = buf;
			cmpStudent.SetAccademy(buf);
		});
		(from _ in this.UpdateAsObservable()
			select chaFileCtrl != null).SubscribeToInteractable(btnNext);
		btnNext.OnClickAsObservable().Subscribe(delegate
		{
			saveData.accademyName = accademyName;
			saveData.emblemID = emblemId;
			player.SetCharFile(chaFileCtrl);
			player.charFileInitialized = true;
			string levelName = "ClassRoomSelect";
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = levelName,
				isAsync = true,
				isFade = true
			}, true);
		});
		Action close = delegate
		{
			cgEntryPlayer.blocksRaycasts = false;
			Utils.Sound.Play(SystemSE.cancel);
			Singleton<Scene>.Instance.UnLoad();
		};
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where !Singleton<Scene>.Instance.IsNowLoadingFade
			where Singleton<Scene>.Instance.NowSceneNames[0] == "EntryPlayer"
			select _).Take(1).Subscribe(delegate
		{
			close();
		});
		btnPrev.OnClickAsObservable().Take(1).Subscribe(delegate
		{
			close();
		});
	}

	private void CreateMaleList()
	{
		chaFileLists.ClearList();
		foreach (var item in Localize.Translate.Manager.CreateChaFileInfo(0, true).Select((Localize.Translate.Manager.ChaFileInfo p, int index) => new { p, index }))
		{
			chaFileLists.AddList(new ChaFileInfo(item.p.chaFile, item.p.info)
			{
				index = item.index
			});
		}
		Action<ChaFileInfo> onSelect = delegate(ChaFileInfo info)
		{
			if (!noPlaySSE)
			{
				Utils.Sound.Play(SystemSE.sel);
			}
			if (chaFileLists.GetSelectIndex().Length == 0)
			{
				chaFileCtrl = null;
				cmpStudent.Clear(string.Empty);
				cmpStudent.SetAccademy(accademyName);
				cmpStudent.SetEmblem(emblemId);
			}
			else
			{
				chaFileCtrl = new ChaFileControl();
				chaFileCtrl.LoadCharaFile(info.FullPath);
				cmpStudent.SetCharaInfo(chaFileCtrl);
			}
		};
		chaFileLists.Create(delegate(ChaFileInfo info)
		{
			onSelect(info);
		});
	}

	private void CreateEmblemList()
	{
		foreach (ListInfoBase value in Singleton<Character>.Instance.chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_emblem).Values)
		{
			emblemLists.AddList(value.Id, value.Name, value.GetInfo(ChaListDefine.KeyType.ThumbAB), value.GetInfo(ChaListDefine.KeyType.ThumbTex));
		}
		Action<int> onSelect = delegate(int index)
		{
			if (!noPlaySSE)
			{
				Utils.Sound.Play(SystemSE.sel);
			}
			emblemId = index;
			cmpStudent.SetEmblem(index);
		};
		emblemLists.Create(delegate(int index)
		{
			onSelect(index);
		});
	}

	private void CardUpdate()
	{
		cmpStudent.Clear(string.Empty);
		cmpStudent.SetCharaInfo(chaFileCtrl, emblemId, accademyName, Singleton<Game>.Instance.Player.GetCallName());
	}
}
