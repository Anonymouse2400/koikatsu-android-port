using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Component.UI;
using Illusion.Extensions;
using Illusion.Game;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace StaffRoom
{
	public class StaffRoomMapListCtrl : MonoBehaviour
	{
		[SerializeField]
		private GameObject objListContent;

		[SerializeField]
		private GameObject objLineTemp;

		[SerializeField]
		private bool multiSelect;

		[SerializeField]
		[Tooltip("OFFだと最低でも一つは選択されてる必要がある")]
		private bool allowSwitchOff = true;

		private StringReactiveProperty selName = new StringReactiveProperty(string.Empty);

		private string selectDrawName = string.Empty;

		private List<MapFileInfo> lstFileInfo = new List<MapFileInfo>();

		public event Action<MapFileInfo> OnPointerEnter = delegate
		{
		};

		public event Action<MapFileInfo> OnPointerExit = delegate
		{
		};

		public event Action<MapFileInfo> OnPointerClick = delegate
		{
		};

		public void ClearList()
		{
			lstFileInfo.Clear();
		}

		public void AddList(int index, string _asset, string _file)
		{
			MapFileInfo mapFileInfo = new MapFileInfo();
			mapFileInfo.index = index;
			mapFileInfo.asset = _asset;
			mapFileInfo.file = _file;
			lstFileInfo.Add(mapFileInfo);
		}

		public void Create(Action<StaffRoomMapInfoComponent> infoListProc)
		{
			objListContent.Children().ForEach(UnityEngine.Object.Destroy);
			ToggleGroup tglGrp = objListContent.GetComponent<ToggleGroup>();
			tglGrp.allowSwitchOff = allowSwitchOff;
			Transform parent = objListContent.transform;
			lstFileInfo.ForEach(delegate(MapFileInfo item)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objLineTemp, parent, false);
				StaffRoomMapInfoComponent component = gameObject.GetComponent<StaffRoomMapInfoComponent>();
				component.info = item;
				component.info.fic = component;
				if (!multiSelect)
				{
					component.tgl.group = tglGrp;
				}
				SetToggleHandler(gameObject);
				component.Disvisible(false);
				component.Disable(false);
				Utils.Bundle.LoadSprite(item.asset, item.file, component.imgThumb, false);
				infoListProc.Call(component);
			});
			ToggleAllOff();
		}

		public MapFileInfo GetFileInfoFromIndex(int index)
		{
			return lstFileInfo.Find((MapFileInfo item) => item.index == index);
		}

		public int[] GetSelectIndex()
		{
			return (from item in lstFileInfo
				where item.fic.gameObject.activeSelf
				where item.fic.tgl.interactable
				where item.fic.tgl.isOn
				select item.index).ToArray();
		}

		public StaffRoomMapInfoComponent GetSelectTopItem()
		{
			int[] selectIndex = GetSelectIndex();
			if (!selectIndex.Any())
			{
				return null;
			}
			MapFileInfo fileInfoFromIndex = GetFileInfoFromIndex(selectIndex[0]);
			return (fileInfoFromIndex != null) ? fileInfoFromIndex.fic : null;
		}

		public StaffRoomMapInfoComponent GetSelectableTopItem()
		{
			MapFileInfo mapFileInfo = (from item in lstFileInfo
				where item.fic.gameObject.activeSelf
				where item.fic.tgl.interactable
				orderby item.fic.gameObject.transform.GetSiblingIndex()
				select item).FirstOrDefault();
			return (mapFileInfo != null) ? mapFileInfo.fic : null;
		}

		public int GetDrawOrderFromIndex(int index)
		{
			var anon = (from item in lstFileInfo
				where item.fic.gameObject.activeSelf
				where item.fic.tgl.interactable
				orderby item.fic.gameObject.transform.GetSiblingIndex()
				select item).Select((MapFileInfo item, int idx) => new { item, idx }).FirstOrDefault(p => p.item.index == index);
			return (anon != null) ? anon.idx : (-1);
		}

		public int GetInclusiveCount()
		{
			return lstFileInfo.Count((MapFileInfo item) => item.fic.gameObject.activeSelf);
		}

		public void SelectItem(int index)
		{
			MapFileInfo mapFileInfo = lstFileInfo.Find((MapFileInfo item) => item.index == index);
			mapFileInfo.fic.tgl.isOn = true;
			ChangeItem(mapFileInfo.fic.gameObject);
		}

		public void ChangeItem(GameObject obj)
		{
			StaffRoomMapInfoComponent component = obj.GetComponent<StaffRoomMapInfoComponent>();
			MapFileInfo obj2 = null;
			if (component != null)
			{
				obj2 = ((!component.tgl.isOn) ? null : component.info);
			}
			this.OnPointerClick(obj2);
		}

		public void ToggleAllOff()
		{
			lstFileInfo.ForEach(delegate(MapFileInfo item)
			{
				item.fic.tgl.isOn = false;
			});
		}

		public void ToggleAllOn()
		{
			lstFileInfo.ForEach(delegate(MapFileInfo item)
			{
				item.fic.tgl.isOn = true;
			});
		}

		public void DisableItem(int index, bool disable)
		{
			DisableItem(lstFileInfo.Find((MapFileInfo item) => item.index == index), disable);
		}

		public void DisvisibleItem(int index, bool disvisible)
		{
			DisvisibleItem(lstFileInfo.Find((MapFileInfo item) => item.index == index), disvisible);
		}

		private void SetToggleHandler(GameObject obj)
		{
			PointerClickCheck orAddComponent = obj.GetOrAddComponent<PointerClickCheck>();
			orAddComponent.buttonType = PointerClickCheck.ButtonType.Left;
			orAddComponent.onPointerClick.AddListener(delegate
			{
				CheckToggleEvent(obj, delegate
				{
					ChangeItem(obj);
				});
			});
			Selectable component = obj.GetComponent<Selectable>();
			component.OnPointerEnterAsObservable().Subscribe(delegate
			{
				CheckToggleEvent(obj, delegate(StaffRoomMapInfoComponent fic)
				{
					this.OnPointerEnter(fic.info);
				});
			});
			component.OnPointerExitAsObservable().Subscribe(delegate
			{
				CheckToggleEvent(obj, delegate(StaffRoomMapInfoComponent fic)
				{
					selName.Value = selectDrawName;
					this.OnPointerExit(fic.info);
				});
			});
		}

		private static void CheckToggleEvent(GameObject obj, Action<StaffRoomMapInfoComponent> act)
		{
			if (!(obj == null))
			{
				StaffRoomMapInfoComponent component = obj.GetComponent<StaffRoomMapInfoComponent>();
				if (!(component == null) && component.tgl.interactable)
				{
					act.Call(component);
				}
			}
		}

		private static void DisableItem(MapFileInfo fi, bool disable)
		{
			if (fi != null)
			{
				fi.fic.Disable(disable);
			}
		}

		private static void DisvisibleItem(MapFileInfo fi, bool disvisible)
		{
			if (fi != null)
			{
				fi.fic.Disvisible(disvisible);
			}
		}
	}
}
