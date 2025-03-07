using System;
using System.Collections;
using System.IO;
using System.Linq;
using Illusion.Game;
using IllusionUtility.SetUtility;
using Localize.Translate;
using MessagePack;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomCoordinateFile : MonoBehaviour
	{
		[SerializeField]
		private Camera mainCamera;

		[SerializeField]
		private Camera coordinateCamera;

		[SerializeField]
		private CustomRender customRender;

		[SerializeField]
		private CustomFileListCtrl listCtrl;

		[SerializeField]
		private CustomFileWindow fileWindow;

		[SerializeField]
		private CustomCheckWindow checkWindow;

		private bool overwrite;

		private int poseNo;

		private float backAnmPos;

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

		private CustomControl cmpCustomCtrl
		{
			get
			{
				return customBase.customCtrl;
			}
		}

		private CvsDrawCtrl cmpDrawCtrl
		{
			get
			{
				return cmpCustomCtrl.cmpDrawCtrl;
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
			if (!reCreate)
			{
				listCtrl.ClearList();
				foreach (var item in Localize.Translate.Manager.CreateCoordinateInfo(isDefaultDataAdd).Select((Localize.Translate.Manager.ChaCoordinateInfo p, int index) => new { p, index }))
				{
					FolderAssist.FileInfo info = item.p.info;
					listCtrl.AddList(new CustomFileInfo(new FolderAssist.FileInfo(info))
					{
						index = item.index,
						name = item.p.coordinate.coordinateName,
						isDefaultData = item.p.isDefault
					});
				}
				listCtrl.Create(OnChangeSelect);
			}
			listCtrl.DefaultDataVisible(isDefaultDataAdd);
			int[] selectIndex = listCtrl.GetSelectIndex();
			if (selectIndex.Length >= 2)
			{
				foreach (CustomFileInfo item2 in from id in selectIndex
					select listCtrl.GetFileInfoFromIndex(id) into p
					where p.isDefaultData
					select p)
				{
					item2.fic.OFF();
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
				case CustomFileWindow.FileWindowType.CoordinateSave:
					Initialize(false, true);
					break;
				case CustomFileWindow.FileWindowType.CoordinateLoad:
					Initialize(true, true);
					break;
				}
			};
			if (fileWindow.btnCoordeLoadLoad != null)
			{
				fileWindow.btnCoordeLoadLoad.OnClickAsObservable().Subscribe(delegate
				{
					CustomFileInfoComponent selectTopItem = listCtrl.GetSelectTopItem();
					if (!(selectTopItem == null))
					{
						Utils.Sound.Play(SystemSE.ok_s);
						ChaFileCoordinate nowCoordinate = chaCtrl.nowCoordinate;
						bool flag = CustomFileWindow.ToggleCheck(fileWindow.tglCoordeLoadClothes);
						byte[] bytes = ((!flag) ? MessagePackSerializer.Serialize(nowCoordinate.clothes) : null);
						bool flag2 = CustomFileWindow.ToggleCheck(fileWindow.tglCoordeLoadAcs);
						byte[] bytes2 = ((!flag2) ? MessagePackSerializer.Serialize(nowCoordinate.accessory) : null);
						nowCoordinate.LoadFile(selectTopItem.info.FullPath);
						if (!flag)
						{
							nowCoordinate.clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes);
						}
						if (!flag2)
						{
							nowCoordinate.accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes2);
						}
						chaCtrl.Reload(false, true, true, true);
						chaCtrl.AssignCoordinate((ChaFileDefine.CoordinateType)chaCtrl.chaFile.status.coordinateType);
						customBase.updateCustomUI = true;
						Singleton<CustomHistory>.Instance.Add5(chaCtrl, chaCtrl.Reload, false, true, true, true);
					}
				});
			}
			if (fileWindow.btnSave != null)
			{
				string question = GetQuestionTitle("CoordinateInput");
				fileWindow.btnSave.OnClickAsObservable().Subscribe(delegate
				{
					overwrite = false;
					Utils.Sound.Play(SystemSE.window_o);
					checkWindow.Setup(CustomCheckWindow.CheckType.CoordinateInput, question ?? "コーディネート名を入力して下さい", null, string.Empty, CreateCoordinateFile, null);
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
				bool flag3 = CustomFileWindow.ToggleCheck(fileWindow.tglCoordeLoadClothes);
				bool flag4 = CustomFileWindow.ToggleCheck(fileWindow.tglCoordeLoadAcs);
				if (!flag3 && !flag4)
				{
					select = false;
				}
				fileWindow.btnCoordeLoadLoad.SafeProcObject(delegate(Button bt)
				{
					bt.interactable = select;
				});
			}).AddTo(this);
			if (fileWindow.btnOverwrite != null)
			{
				string question2 = GetQuestionTitle("Overwrite");
				fileWindow.btnOverwrite.OnClickAsObservable().Subscribe(delegate
				{
					overwrite = true;
					int[] selectIndex2 = listCtrl.GetSelectIndex();
					CustomFileInfo fileInfoFromIndex = listCtrl.GetFileInfoFromIndex(selectIndex2[0]);
					Utils.Sound.Play(SystemSE.window_o);
					checkWindow.Setup(CustomCheckWindow.CheckType.CoordinateInput, question2 ?? "本当に上書きしますか？", null, fileInfoFromIndex.name, CreateCoordinateFile, null);
				});
			}
			if (fileWindow.btnDelete != null)
			{
				string question3 = GetQuestionTitle("Erase");
				fileWindow.btnDelete.OnClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.window_o);
					checkWindow.Setup(CustomCheckWindow.CheckType.YesNo, question3 ?? "本当に削除しますか？", null, null, DeleteCoordinateFile, null);
				});
			}
			return true;
		}

		public void DeleteCoordinateFile(string buf)
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

		public void CreateCoordinateFile(string coordinateName)
		{
			CreateCoordinateFileBefore();
			StartCoroutine(CreateCoordinateFileCoroutine(coordinateName));
		}

		public void CreateCoordinateFileBefore()
		{
			chaCtrl.ChangeSettingMannequin(true);
			cmpDrawCtrl.ChangeClothesStateForCapture(true);
			bool flag = chaCtrl.sex == 0;
			string text = ((!flag) ? "f" : "m");
			string assetBundleName = "custom/custom_etc.unity3d";
			customBase.saveFrameAssist.ChangeSaveFrameBackTexture(PngAssist.LoadTexture2DFromAssetBundle(assetBundleName, "coorde_frame_back_" + text));
			customBase.saveFrameAssist.ChangeSaveFrameFrontTexture(PngAssist.LoadTexture2DFromAssetBundle(assetBundleName, "coorde_frame_front_" + text));
			poseNo = cmpDrawCtrl.ddPose.value;
			Animator animBody = chaCtrl.animBody;
			if ((bool)animBody)
			{
				backAnmPos = animBody.GetCurrentAnimatorStateInfo(0).normalizedTime;
			}
			string text2 = "custom/custompose.unity3d";
			string text3 = "custom_pose";
			string stateName = ((!flag) ? "F" : "M") + "mannequin_00";
			if (customBase.animeAssetBundleName != text2 || customBase.animeAssetName != text3)
			{
				customBase.LoadAnimation(text2, text3);
			}
			customBase.PlayAnimation(stateName, 0f);
			chaCtrl.resetDynamicBoneAll = true;
			customRender.update = false;
		}

		public IEnumerator CreateCoordinateFileCoroutine(string coordinateName)
		{
			yield return new WaitForEndOfFrame();
			byte[] pngData = CreateCoordinatePng();
			ChaFileControl chaFile = chaCtrl.chaFile;
			ChaFileCoordinate coordinate = chaFile.coordinate[chaFile.status.coordinateType];
			coordinate.pngData = pngData;
			coordinate.coordinateName = coordinateName;
			DateTime nowTime = DateTime.Now;
			CustomFileInfo info;
			if (overwrite)
			{
				int[] selectIndex = listCtrl.GetSelectIndex();
				info = listCtrl.GetFileInfoFromIndex(selectIndex[0]);
			}
			else
			{
				string text = string.Format("KKCoorde{0}_" + FolderAssist.TimeStamp(nowTime), (chaCtrl.sex != 0) ? "F" : "M");
				string fullPath = UserData.Path + "coordinate/" + text + ".png";
				info = new CustomFileInfo(new FolderAssist.FileInfo(fullPath, text, null))
				{
					index = listCtrl.GetNoUseIndex(),
					name = coordinateName
				};
			}
			coordinate.SaveFile(info.FullPath);
			info.UpdateInfo(coordinateName, nowTime);
			if (!overwrite)
			{
				listCtrl.Add(info);
			}
			else
			{
				info.fic.UpdateInfo(null);
				listCtrl.UpdateSort();
			}
			StartCoroutine(CreateCoordinateFileAfterCoroutine());
		}

		public byte[] CreateCoordinatePng()
		{
			byte[] array = null;
			mainCamera.gameObject.SetActive(false);
			if ((bool)coordinateCamera)
			{
				coordinateCamera.enabled = true;
			}
			if ((bool)coordinateCamera)
			{
				coordinateCamera.gameObject.SetActive(true);
				if (chaCtrl.sex == 0)
				{
					coordinateCamera.transform.SetPosition(0.58f, 1.1f, 4.5f);
					coordinateCamera.transform.SetRotation(3.5f, 187.5f, 0f);
				}
				else
				{
					coordinateCamera.transform.SetPosition(0.58f, 1.08f, 4.4f);
					coordinateCamera.transform.SetRotation(3.5f, 187.5f, 0f);
				}
			}
			customBase.drawSaveFrameTop = true;
			bool drawSaveFrameBack = customBase.drawSaveFrameBack;
			customBase.drawSaveFrameBack = true;
			bool drawSaveFrameFront = customBase.drawSaveFrameFront;
			customBase.drawSaveFrameFront = true;
			array = cmpCustomCtrl.customCap.CapCoordinateCard(false, customBase.saveFrameAssist, coordinateCamera);
			customBase.saveFrameAssist.ChangeSaveFrameBack(2);
			customBase.saveFrameAssist.ChangeSaveFrameFront(2);
			customBase.drawSaveFrameBack = drawSaveFrameBack;
			customBase.drawSaveFrameFront = drawSaveFrameFront;
			customBase.drawSaveFrameTop = false;
			if ((bool)coordinateCamera)
			{
				coordinateCamera.enabled = false;
			}
			if ((bool)coordinateCamera)
			{
				coordinateCamera.gameObject.SetActive(false);
			}
			mainCamera.gameObject.SetActive(true);
			chaCtrl.ChangeSettingMannequin(false);
			cmpDrawCtrl.ChangeClothesStateForCapture(true);
			cmpDrawCtrl.ChangeAnimationForce(poseNo, backAnmPos);
			return array;
		}

		public IEnumerator CreateCoordinateFileAfterCoroutine()
		{
			yield return null;
			customRender.update = true;
		}

		public void OnChangeSelect(CustomFileInfo info)
		{
		}
	}
}
