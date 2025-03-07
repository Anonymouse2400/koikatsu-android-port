using System.Linq;
using Illusion.Game;
using Localize.Translate;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChaCustom
{
	public class clothesFileControl : MonoBehaviour
	{
		public enum SettingKind
		{
			load = 0,
			fullpath = 1
		}

		[HideInInspector]
		public ChaControl chaCtrl;

		public CustomFileListCtrl listCtrl;

		public SettingKind kindSet;

		private string fullpath = string.Empty;

		[SerializeField]
		private Button btnClose;

		[SerializeField]
		private Button btnLoad;

		[SerializeField]
		private Button btnCancel;

		[SerializeField]
		private CameraControl_Ver2 camCtrl;

		[SerializeField]
		private TextMeshProUGUI text;

		public UnityAction OnEnter;

		public string FullPath
		{
			get
			{
				return fullpath;
			}
		}

		private void OnEnable()
		{
			if ((bool)camCtrl)
			{
				camCtrl.NoCtrlCondition = () => true;
			}
		}

		private void Start()
		{
			Initialize();
			if ((bool)text && !Localize.Translate.Manager.Bind(text, Localize.Translate.Manager.OtherData.Get(2).Get((int)kindSet), true))
			{
				if (kindSet == SettingKind.load)
				{
					text.text = "読込み";
				}
				else if (kindSet == SettingKind.fullpath)
				{
					text.text = "決定";
				}
			}
			if ((bool)btnLoad)
			{
				btnLoad.OnClickAsObservable().Subscribe(delegate
				{
					if ((bool)listCtrl)
					{
						Utils.Sound.Play(SystemSE.sel);
						CustomFileInfoComponent selectTopItem = listCtrl.GetSelectTopItem();
						if (null != selectTopItem)
						{
							string fullPath = selectTopItem.info.FullPath;
							if (kindSet == SettingKind.load)
							{
								if ((bool)chaCtrl)
								{
									chaCtrl.nowCoordinate.LoadFile(fullPath);
									chaCtrl.Reload(false, true, true, true);
								}
							}
							else if (kindSet == SettingKind.fullpath)
							{
								fullpath = fullPath;
								base.gameObject.SetActive(false);
								if (!OnEnter.IsNullOrEmpty())
								{
									OnEnter();
								}
							}
						}
					}
				});
				Observable.EveryUpdate().Subscribe(delegate
				{
					int[] selectIndex = listCtrl.GetSelectIndex();
					bool interactable = selectIndex != null && selectIndex.Length > 0;
					if ((bool)btnLoad)
					{
						btnLoad.interactable = interactable;
					}
				}).AddTo(this);
			}
			if ((bool)btnClose)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					base.gameObject.SetActive(false);
					Utils.Sound.Play(SystemSE.cancel);
					if ((bool)camCtrl)
					{
						camCtrl.NoCtrlCondition = () => false;
					}
				});
			}
			if ((bool)btnCancel)
			{
				btnCancel.OnClickAsObservable().Subscribe(delegate
				{
					base.gameObject.SetActive(false);
					Utils.Sound.Play(SystemSE.cancel);
					if ((bool)camCtrl)
					{
						camCtrl.NoCtrlCondition = () => false;
					}
				});
			}
			(from _ in this.UpdateAsObservable()
				where Input.GetMouseButton(1)
				select _).Subscribe(delegate
			{
				base.gameObject.SetActive(false);
				Utils.Sound.Play(SystemSE.cancel);
				if ((bool)camCtrl)
				{
					camCtrl.NoCtrlCondition = () => false;
				}
			});
		}

		private void Initialize()
		{
			listCtrl.ClearList();
			foreach (var item in Localize.Translate.Manager.CreateCoordinateInfo(true).Select((Localize.Translate.Manager.ChaCoordinateInfo p, int index) => new { p, index }))
			{
				listCtrl.AddList(new CustomFileInfo(new FolderAssist.FileInfo(item.p.info))
				{
					index = item.index,
					name = item.p.coordinate.coordinateName
				});
			}
			listCtrl.Create(OnChangeSelect);
		}

		public void OnChangeSelect(CustomFileInfo info)
		{
			Utils.Sound.Play(SystemSE.sel);
		}
	}
}
