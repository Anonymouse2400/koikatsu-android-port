using System;
using System.IO;
using System.Linq;
using Illusion.Game;
using Localize.Translate;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomCharaFile : MonoBehaviour
	{
		[SerializeField]
		private CustomFileListCtrl listCtrl;

		[SerializeField]
		private CustomFileWindow fileWindow;

		[SerializeField]
		private CustomCheckWindow checkWindow;

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

		private ChaFileControl chaFile
		{
			get
			{
				return chaCtrl.chaFile;
			}
		}

		private string GetQuestionTitle(string tag)
		{
			return customBase.translateQuestionTitle.Values.FindTagText(tag);
		}

		private void Start()
		{
			if (Initialize(true, false))
			{
				FileWindowSetting();
			}
		}

		private bool Initialize(bool isDefaultDataAdd, bool reCreate)
		{
			if (listCtrl == null)
			{
				return false;
			}
			int modeSex = customBase.modeSex;
			if (!reCreate)
			{
				FileListControler.Execute(modeSex, Localize.Translate.Manager.CreateChaFileInfo(modeSex, isDefaultDataAdd), listCtrl);
				listCtrl.Create(OnChangeSelect);
			}
			listCtrl.DefaultDataVisible(isDefaultDataAdd);
			int[] selectIndex = listCtrl.GetSelectIndex();
			if (selectIndex.Length >= 2)
			{
				foreach (CustomFileInfo item in from id in selectIndex
					select listCtrl.GetFileInfoFromIndex(id) into p
					where p.isDefaultData
					select p)
				{
					item.fic.OFF();
				}
			}
			return true;
		}

		private bool FileWindowSetting()
		{
			if (fileWindow == null)
			{
				return false;
			}
			fileWindow.eventUpdateWindow += delegate(CustomFileWindow.FileWindowType type)
			{
				switch (type)
				{
				case CustomFileWindow.FileWindowType.CharaSave:
					Initialize(false, true);
					break;
				case CustomFileWindow.FileWindowType.CharaLoad:
					Initialize(true, true);
					break;
				}
			};
			if (!customBase.modeNew && fileWindow.tglChaLoadParam != null)
			{
				fileWindow.tglChaLoadParam.interactable = false;
				Transform transform = fileWindow.tglChaLoadParam.transform.Find("textLoad");
				if (transform != null)
				{
					TextMeshProUGUI component = transform.GetComponent<TextMeshProUGUI>();
					if (component != null)
					{
						component.color = Color.gray;
					}
				}
				fileWindow.tglChaLoadParam.isOn = false;
			}
			if (fileWindow.btnChaLoadAllOn != null)
			{
				fileWindow.btnChaLoadAllOn.OnClickAsObservable().Subscribe(delegate
				{
					fileWindow.tglChaLoadFace.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = true;
					});
					fileWindow.tglChaLoadBody.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = true;
					});
					fileWindow.tglChaLoadHair.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = true;
					});
					fileWindow.tglChaLoadCoorde.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = true;
					});
					if (customBase.modeNew)
					{
						fileWindow.tglChaLoadParam.SafeProcObject(delegate(Toggle tgl)
						{
							tgl.isOn = true;
						});
					}
				});
			}
			if (fileWindow.btnChaLoadAllOff != null)
			{
				fileWindow.btnChaLoadAllOff.OnClickAsObservable().Subscribe(delegate
				{
					fileWindow.tglChaLoadFace.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = false;
					});
					fileWindow.tglChaLoadBody.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = false;
					});
					fileWindow.tglChaLoadHair.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = false;
					});
					fileWindow.tglChaLoadCoorde.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = false;
					});
					fileWindow.tglChaLoadParam.SafeProcObject(delegate(Toggle tgl)
					{
						tgl.isOn = false;
					});
				});
			}
			if (fileWindow.btnChaLoadLoad != null)
			{
				fileWindow.btnChaLoadLoad.OnClickAsObservable().Subscribe(delegate
				{
					CustomFileInfoComponent selectTopItem = listCtrl.GetSelectTopItem();
					if (!(selectTopItem == null))
					{
						Utils.Sound.Play(SystemSE.ok_s);
						bool flag = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadFace);
						bool flag2 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadBody);
						bool flag3 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadHair);
						bool parameter = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadParam);
						bool flag4 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadCoorde);
						chaFile.LoadFileLimited(selectTopItem.info.FullPath, chaCtrl.sex, flag, flag2, flag3, parameter, flag4);
						chaCtrl.ChangeCoordinateType();
						chaCtrl.Reload(!flag4, !flag && !flag4, !flag3, !flag2);
						customBase.updateCustomUI = true;
						Singleton<CustomHistory>.Instance.Add5(chaCtrl, chaCtrl.Reload, !flag4, !flag && !flag4, !flag3, !flag2);
					}
				});
			}
			if (fileWindow.btnSave != null)
			{
				fileWindow.btnSave.OnClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.window_o);
					customBase.customCtrl.saveFileName = string.Empty;
					customBase.customCtrl.saveFileListCtrl = listCtrl;
					customBase.customCtrl.saveNew = true;
					customBase.customCtrl.saveMode = true;
				});
			}
			Observable.EveryUpdate().Subscribe(delegate
			{
				int[] selectIndex = listCtrl.GetSelectIndex();
				bool select = selectIndex.Any();
				fileWindow.btnOverwrite.SafeProcObject(delegate(Button bt)
				{
					bt.interactable = select;
				});
				fileWindow.btnDelete.SafeProcObject(delegate(Button bt)
				{
					bt.interactable = select;
				});
				bool flag5 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadFace);
				bool flag6 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadBody);
				bool flag7 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadHair);
				bool flag8 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadParam);
				bool flag9 = CustomFileWindow.ToggleCheck(fileWindow.tglChaLoadCoorde);
				if (!flag5 && !flag6 && !flag7 && !flag8 && !flag9)
				{
					select = false;
				}
				fileWindow.btnChaLoadLoad.SafeProcObject(delegate(Button bt)
				{
					bt.interactable = select;
				});
			}).AddTo(this);
			if (fileWindow.btnOverwrite != null)
			{
				string question = GetQuestionTitle("Change");
				fileWindow.btnOverwrite.OnClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.window_o);
					checkWindow.Setup(CustomCheckWindow.CheckType.CharaOverwrite, question ?? "キャラ画像を変更しますか？", null, null, OverwriteCharaFileWithCapture, OverwriteCharaFile, null);
				});
			}
			if (fileWindow.btnDelete != null)
			{
				string question2 = GetQuestionTitle("Erase");
				fileWindow.btnDelete.OnClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.window_o);
					checkWindow.Setup(CustomCheckWindow.CheckType.YesNo, question2 ?? "本当に削除しますか？", null, null, DeleteCharaFile, null);
				});
			}
			return true;
		}

		public void DeleteCharaFile(string buf)
		{
			int[] selectIndex = listCtrl.GetSelectIndex();
			CustomFileInfo fileInfoFromIndex = listCtrl.GetFileInfoFromIndex(selectIndex[0]);
			string fullPath = fileInfoFromIndex.FullPath;
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
			listCtrl.Delete(selectIndex[0]);
		}

		public void OverwriteCharaFileWithCapture(string buf)
		{
			int[] selectIndex = listCtrl.GetSelectIndex();
			CustomFileInfo fileInfoFromIndex = listCtrl.GetFileInfoFromIndex(selectIndex[0]);
			customBase.customCtrl.saveFileName = fileInfoFromIndex.FileName;
			customBase.customCtrl.saveFileListCtrl = listCtrl;
			customBase.customCtrl.saveNew = false;
			customBase.customCtrl.saveMode = true;
		}

		public void OverwriteCharaFile(string buf)
		{
			int[] selectIndex = listCtrl.GetSelectIndex();
			CustomFileInfo fileInfoFromIndex = listCtrl.GetFileInfoFromIndex(selectIndex[0]);
			string fullPath = fileInfoFromIndex.FullPath;
			ChaFileControl chaFileControl = new ChaFileControl();
			if (!chaFileControl.LoadCharaFile(fullPath))
			{
			}
			if (chaFileControl.facePngData != null && chaFileControl.pngData != null)
			{
				chaFile.facePngData = chaFileControl.facePngData;
				chaFile.pngData = chaFileControl.pngData;
			}
			chaFile.SaveCharaFile(fullPath);
			fileInfoFromIndex.UpdateInfo(chaFile.parameter, DateTime.Now);
			fileInfoFromIndex.fic.UpdateInfo(null);
			listCtrl.UpdateSort();
		}

		public void OnChangeSelect(CustomFileInfo info)
		{
		}
	}
}
