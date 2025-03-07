using System;
using System.Collections.Generic;
using System.Linq;
using FileListUI;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomFileListCtrl : ThreadFileListCtrl<CustomFileInfo, CustomFileInfoComponent>
	{
		[SerializeField]
		private CustomFileWindow cfWindow;

		[SerializeField]
		private Toggle tglAddInfo;

		[SerializeField]
		private Button btnSortClub;

		[SerializeField]
		private Button btnSortPersonality;

		private bool ascendClub;

		private bool ascendPersonality;

		protected override Selectable[] addInfos
		{
			get
			{
				return this.GetCache(ref _addInfos, () => new Selectable[2] { tglAddInfo, btnSortPersonality }.Where((Selectable p) => p != null).ToArray());
			}
		}

		protected override void Start()
		{
			base.Start();
			foreach (Button item in base.allInfos.OfType<Button>())
			{
				item.OnClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.sel);
				});
			}
			if (btnSortClub != null)
			{
				btnSortClub.OnClickAsObservable().Subscribe(delegate
				{
					SortClub(!ascendClub);
				});
			}
			if (btnSortPersonality != null)
			{
				btnSortPersonality.OnClickAsObservable().Subscribe(delegate
				{
					SortPersonality(!ascendPersonality);
				});
			}
			if (!(tglAddInfo != null))
			{
				return;
			}
			tglAddInfo.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				base.lstFileInfo.ForEach(delegate(CustomFileInfo item)
				{
					item.fic.DisvisibleAddInfo(isOn);
				});
			});
		}

		public void SortClub(bool ascend)
		{
			Sort(2, ascend, out ascendClub, SortClub);
		}

		private void SortClub()
		{
			Localize.Translate.Manager.SetCulture(delegate
			{
				if (ascendClub)
				{
					base.lstFileInfo.Sort((CustomFileInfo a, CustomFileInfo b) => a.club.CompareTo(b.club));
				}
				else
				{
					base.lstFileInfo.Sort((CustomFileInfo a, CustomFileInfo b) => b.club.CompareTo(a.club));
				}
			});
		}

		public void SortPersonality(bool ascend)
		{
			Sort(3, ascend, out ascendPersonality, SortPersonality);
		}

		private void SortPersonality()
		{
			Localize.Translate.Manager.SetCulture(delegate
			{
				if (ascendPersonality)
				{
					base.lstFileInfo.Sort((CustomFileInfo a, CustomFileInfo b) => a.personality.CompareTo(b.personality));
				}
				else
				{
					base.lstFileInfo.Sort((CustomFileInfo a, CustomFileInfo b) => b.personality.CompareTo(a.personality));
				}
			});
		}

		public override void Add(CustomFileInfo info)
		{
			base.Add(info);
			info.fic.UpdateInfo(CustomFileWindow.ToggleCheck(tglAddInfo));
			UpdateSort();
		}

		public void DefaultDataVisible(bool isOn)
		{
			foreach (CustomFileInfo item in base.lstFileInfo.Where((CustomFileInfo p) => p.fic.info.isDefaultData))
			{
				item.fic.gameObject.SetActiveIfDifferent(isOn);
			}
		}

		public override void Create(Action<CustomFileInfo> onChangeItem, bool reCreate = false)
		{
			base.onChangeItem = onChangeItem;
			base.imgRaycast.Clear();
			objListContent.Children().ForEach(delegate(GameObject go)
			{
				UnityEngine.Object.Destroy(go);
			});
			foreach (CustomFileInfo item in base.lstFileInfo)
			{
				item.DeleteThumb();
				CreateCloneBinding(item);
				item.fic.UpdateInfo(null);
			}
			if (reCreate)
			{
				UpdateSort();
			}
			else
			{
				SortClub(true);
				SortPersonality(true);
				SortName(true);
				SortDate(false);
			}
			ToggleAllOff();
			bool isAddInfo = CustomFileWindow.ToggleCheck(tglAddInfo);
			if (isAddInfo)
			{
				base.lstFileInfo.ForEach(delegate(CustomFileInfo item)
				{
					item.fic.DisvisibleAddInfo(isAddInfo);
				});
			}
		}

		public override void UpdateSort()
		{
			switch (base.lastSort)
			{
			case 0:
				SortClub(ascendClub);
				SortPersonality(ascendPersonality);
				SortDate(ascendData);
				SortName(ascendName);
				break;
			case 1:
				SortClub(ascendClub);
				SortPersonality(ascendPersonality);
				SortName(ascendName);
				SortDate(ascendData);
				break;
			case 2:
				SortPersonality(ascendPersonality);
				SortName(ascendName);
				SortDate(ascendData);
				SortClub(ascendClub);
				break;
			default:
				SortClub(ascendClub);
				SortName(ascendName);
				SortDate(ascendData);
				SortPersonality(ascendPersonality);
				break;
			}
		}

		public void UpdateCategory()
		{
			if (cfWindow == null)
			{
				return;
			}
			Toggle[] tglCategory = cfWindow.tglCategory;
			if (!tglCategory.Any())
			{
				return;
			}
			bool[] array = tglCategory.Select((Toggle x) => x.isOn).ToArray();
			List<CustomFileInfo> list;
			if (array.All((bool b) => !b))
			{
				list = base.lstFileInfo;
				foreach (CustomFileInfo item in base.lstFileInfo)
				{
					item.fic.Disvisible(false);
				}
			}
			else
			{
				list = new List<CustomFileInfo>();
				foreach (CustomFileInfo item2 in base.lstFileInfo)
				{
					bool flag = array[item2.category];
					item2.fic.Disvisible(!flag);
					if (flag)
					{
						list.Add(item2);
					}
				}
			}
			CustomFileInfo customFileInfo = list.Find((CustomFileInfo x) => x.fic.tgl.isOn);
			selectDrawName = ((customFileInfo != null) ? customFileInfo.fic.name : string.Empty);
		}
	}
}
