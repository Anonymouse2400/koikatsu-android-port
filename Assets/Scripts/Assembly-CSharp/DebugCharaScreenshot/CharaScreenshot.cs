using System;
using System.IO;
using Manager;
using UnityEngine;

namespace DebugCharaScreenshot
{
	public class CharaScreenshot : Singleton<CharaScreenshot>
	{
		[HideInInspector]
		public string[] charaPngName = new string[30]
		{
			"00_セクシー系お姉さま", "01_お嬢様", "02_タカビー", "03_小悪魔っ子", "04_ミステリアス", "05_電波", "06_大和撫子", "07_ボーイッシュ", "08_純粋無垢な子供", "09_アホの子",
			"10_邪気眼", "11_母性的お姉さん", "12_姉御肌", "13_コギャル", "14_不良少女", "15_野生的", "16_意識高いクールな女性", "17_ひねくれ", "18_不幸少女", "19_文学少女",
			"20_モジモジ", "21_正統派ヒロイン", "22_ミーハー", "23_オタク女子", "24_ヤンデレ", "25_ダル", "26_無口", "27_意地っ張り", "28_ロリばばあ", "29_素直クール"
		};

		private const string dirSave = "WorkSpace/CharaScreenshot/";

		[SerializeField]
		private CameraControl_Ver2 camCtrl;

		[HideInInspector]
		public int personalityNo;

		public GameScreenShot gsShot;

		[HideInInspector]
		public ChaControl chaCtrl { get; private set; }

		public void Start()
		{
			Singleton<Character>.Instance.enableCorrectArmSize = true;
			Singleton<Character>.Instance.enableCorrectHandSize = true;
			chaCtrl = Singleton<Character>.Instance.CreateFemale(null, 0);
			ReLoad(personalityNo);
		}

		public void ReLoad(int chaNo)
		{
			string filename = Application.dataPath + "/../WorkSpace/CharaScreenshot/" + charaPngName[chaNo] + ".png";
			chaCtrl.chaFile.LoadCharaFile(filename, 1);
			chaCtrl.ChangeCoordinateType(ChaFileDefine.CoordinateType.School01);
			chaCtrl.Load();
			LoadSetting(chaNo);
			LoadAnimation();
			chaCtrl.SetAccessoryStateCategory(0, true);
			chaCtrl.SetAccessoryStateCategory(1, true);
			ChaFileStatus status = chaCtrl.chaFile.status;
			chaCtrl.ChangeEyebrowPtn(status.eyebrowPtn);
			chaCtrl.ChangeEyebrowOpenMax(status.eyebrowOpenMax);
			chaCtrl.ChangeEyesPtn(status.eyesPtn);
			chaCtrl.ChangeEyesOpenMax(status.eyesOpenMax);
			chaCtrl.ChangeEyesBlinkFlag(false);
			chaCtrl.ChangeMouthPtn(status.mouthPtn);
			chaCtrl.ChangeMouthOpenMax(status.mouthOpenMax);
			chaCtrl.ChangeMouthFixed(status.mouthFixed);
			chaCtrl.ChangeLookEyesPtn(status.eyesLookPtn);
			if (chaCtrl.chaFile.status.eyesTargetType == 0 && 0 == 0)
			{
				chaCtrl.ChangeLookEyesTarget(0);
			}
			else
			{
				chaCtrl.ChangeLookEyesTarget(999, null, chaCtrl.chaFile.status.eyesTargetRate, chaCtrl.chaFile.status.eyesTargetAngle, chaCtrl.chaFile.status.eyesTargetRange);
			}
			chaCtrl.ChangeLookNeckPtn(status.neckLookPtn);
			if (chaCtrl.chaFile.status.neckTargetType == 0 && 0 == 0)
			{
				chaCtrl.ChangeLookNeckTarget(0);
			}
			else
			{
				chaCtrl.ChangeLookNeckTarget(999, null, chaCtrl.chaFile.status.neckTargetRate, chaCtrl.chaFile.status.neckTargetAngle, chaCtrl.chaFile.status.neckTargetRange);
			}
			chaCtrl.ChangeHohoAkaRate(status.hohoAkaRate);
			chaCtrl.ChangeSettingSkinGlossPower();
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		public void LoadAnimation()
		{
			if (!(null == chaCtrl.animBody))
			{
				chaCtrl.animBody.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("custom_pose");
				chaCtrl.animBody.speed = 0f;
				chaCtrl.syncPlay(string.Format("Stand_{0:00}_00", personalityNo), 0, 0f);
			}
		}

		public void SaveSetting(int chaNo)
		{
			byte[] statusBytes = chaCtrl.chaFile.GetStatusBytes();
			string path = Application.dataPath + "/../WorkSpace/CharaScreenshot/chara" + chaNo + ".sav";
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output))
				{
					binaryWriter.Write(statusBytes.Length);
					binaryWriter.Write(statusBytes);
					Vector3 position = chaCtrl.GetPosition();
					binaryWriter.Write(position.x);
					binaryWriter.Write(position.y);
					binaryWriter.Write(position.z);
					Vector3 rotation = chaCtrl.GetRotation();
					binaryWriter.Write(rotation.x);
					binaryWriter.Write(rotation.y);
					binaryWriter.Write(rotation.z);
					camCtrl.CameraDataSaveBinary(binaryWriter);
				}
			}
		}

		private void LoadSetting(int chaNo)
		{
			string path = Application.dataPath + "/../WorkSpace/CharaScreenshot/chara" + chaNo + ".sav";
			if (!File.Exists(path))
			{
				return;
			}
			using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int count = binaryReader.ReadInt32();
					byte[] statusBytes = binaryReader.ReadBytes(count);
					chaCtrl.chaFile.SetStatusBytes(statusBytes);
					Vector3 zero = Vector3.zero;
					zero.x = binaryReader.ReadSingle();
					zero.y = binaryReader.ReadSingle();
					zero.z = binaryReader.ReadSingle();
					chaCtrl.SetPosition(zero);
					Vector3 zero2 = Vector3.zero;
					zero2.x = binaryReader.ReadSingle();
					zero2.y = binaryReader.ReadSingle();
					zero2.z = binaryReader.ReadSingle();
					chaCtrl.SetPosition(zero2);
					camCtrl.CameraDataLoadBinary(binaryReader, true);
				}
			}
		}
	}
}
