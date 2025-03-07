using System;
using System.IO;
using Illusion.CustomAttributes;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;

public class MainCGChara : MonoBehaviour
{
	public class ItemInfo
	{
		public Vector3 pos = Vector3.zero;

		public Vector3 rot = Vector3.zero;

		public Vector3 scl = Vector3.one;

		public string parentName = string.Empty;
	}

	private string[] chaSaveFile = new string[7] { "/WorkSpace/chaNo01.dat", "/WorkSpace/chaNo02.dat", "/WorkSpace/chaNo03.dat", "/WorkSpace/chaNo04.dat", "/WorkSpace/chaNo05.dat", "/WorkSpace/chaNo06.dat", "/WorkSpace/chaNo07.dat" };

	private string[] chaPngFile = new string[7] { "櫻井\u3000野乃花.png", "姫川\u3000舞.png", "結城\u3000桜.png", "柊\u3000このみ.png", "水瀬\u3000亜依.png", "白鳥\u3000葵.png", "リナ\u3000ロベール.png" };

	[SerializeField]
	private CameraControl_Ver2 camCtrl;

	public string anmCtrlName = string.Empty;

	public string itemName01 = string.Empty;

	public string itemName02 = string.Empty;

	public ItemInfo itemInfo01 = new ItemInfo();

	public ItemInfo itemInfo02 = new ItemInfo();

	private ChaControl chaCtrl;

	private GameObject objItem01;

	private GameObject objItem02;

	[Header("保存-------------------------------------")]
	[Button("SaveSetting", "櫻井\u3000野乃花", new object[] { 0 })]
	public int savesetting0;

	[Button("SaveSetting", "姫川\u3000舞", new object[] { 1 })]
	public int savesetting1;

	[Button("SaveSetting", "結城\u3000桜", new object[] { 2 })]
	public int savesetting2;

	[Button("SaveSetting", "柊\u3000このみ", new object[] { 3 })]
	public int savesetting3;

	[Button("SaveSetting", "水瀬\u3000亜依", new object[] { 4 })]
	public int savesetting4;

	[Button("SaveSetting", "白鳥\u3000葵", new object[] { 5 })]
	public int savesetting5;

	[Button("SaveSetting", "リナ\u3000ロベール", new object[] { 6 })]
	public int savesetting6;

	[Button("ReLoad", "櫻井\u3000野乃花", new object[] { 0 })]
	[Header("読み込み---------------------------------")]
	public int loadsetting0;

	[Button("ReLoad", "姫川\u3000舞", new object[] { 1 })]
	public int loadsetting1;

	[Button("ReLoad", "結城\u3000桜", new object[] { 2 })]
	public int loadsetting2;

	[Button("ReLoad", "柊\u3000このみ", new object[] { 3 })]
	public int loadsetting3;

	[Button("ReLoad", "水瀬\u3000亜依", new object[] { 4 })]
	public int loadsetting4;

	[Button("ReLoad", "白鳥\u3000葵", new object[] { 5 })]
	public int loadsetting5;

	[Button("ReLoad", "リナ\u3000ロベール", new object[] { 6 })]
	public int loadsetting6;

	public void Start()
	{
		Singleton<Character>.Instance.enableCorrectArmSize = true;
		Singleton<Character>.Instance.enableCorrectHandSize = true;
		chaCtrl = Singleton<Character>.Instance.CreateFemale(null, 0);
	}

	public void ReLoad(int chaNo)
	{
		if ((bool)objItem01)
		{
			UnityEngine.Object.Destroy(objItem01);
		}
		if ((bool)objItem02)
		{
			UnityEngine.Object.Destroy(objItem02);
		}
		chaCtrl.chaFile.LoadCharaFile(chaPngFile[chaNo], 1);
		chaCtrl.ChangeCoordinateType(ChaFileDefine.CoordinateType.Club);
		chaCtrl.Load();
		LoadSetting(chaNo);
		LoadAnimation(chaCtrl, anmCtrlName);
		chaCtrl.SetAccessoryStateCategory(0, true);
		chaCtrl.SetAccessoryStateCategory(1, true);
		objItem01 = LoadItem(itemName01);
		objItem02 = LoadItem(itemName02);
		if ((bool)objItem01 && string.Empty != itemInfo01.parentName)
		{
			GameObject gameObject = chaCtrl.objTop.transform.FindLoop(itemInfo01.parentName);
			if (null != gameObject)
			{
				objItem01.transform.SetParent(gameObject.transform, false);
				objItem01.transform.localPosition = itemInfo01.pos;
				objItem01.transform.localEulerAngles = itemInfo01.rot;
				objItem01.transform.localScale = itemInfo01.scl;
			}
		}
		if ((bool)objItem02 && string.Empty != itemInfo02.parentName)
		{
			GameObject gameObject2 = chaCtrl.objTop.transform.FindLoop(itemInfo02.parentName);
			if (null != gameObject2)
			{
				objItem02.transform.SetParent(gameObject2.transform, false);
				objItem02.transform.localPosition = itemInfo02.pos;
				objItem02.transform.localEulerAngles = itemInfo02.rot;
				objItem02.transform.localScale = itemInfo02.scl;
			}
		}
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

	public void LoadAnimation(ChaControl cha, string path)
	{
		if (!(null == cha.animBody))
		{
			cha.animBody.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(path);
		}
	}

	public GameObject LoadItem(string path)
	{
		UnityEngine.Object @object = Resources.Load(path);
		if (null == @object)
		{
			return null;
		}
		return (GameObject)UnityEngine.Object.Instantiate(@object);
	}

	private void Update()
	{
	}

	private void SaveSetting(int chaNo)
	{
		byte[] statusBytes = chaCtrl.chaFile.GetStatusBytes();
		string path = Application.dataPath + "/.." + chaSaveFile[chaNo];
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
				binaryWriter.Write(anmCtrlName);
				itemInfo01 = new ItemInfo();
				if ((bool)objItem01)
				{
					if ((bool)objItem01.transform.parent)
					{
						itemInfo01.parentName = objItem01.transform.parent.name;
					}
					itemInfo01.pos.x = objItem01.transform.localPosition.x;
					itemInfo01.pos.y = objItem01.transform.localPosition.y;
					itemInfo01.pos.z = objItem01.transform.localPosition.z;
					itemInfo01.rot.x = objItem01.transform.localEulerAngles.x;
					itemInfo01.rot.y = objItem01.transform.localEulerAngles.y;
					itemInfo01.rot.z = objItem01.transform.localEulerAngles.z;
					itemInfo01.scl.x = objItem01.transform.localScale.x;
					itemInfo01.scl.y = objItem01.transform.localScale.y;
					itemInfo01.scl.z = objItem01.transform.localScale.z;
				}
				binaryWriter.Write(itemName01);
				binaryWriter.Write(itemInfo01.parentName);
				binaryWriter.Write(itemInfo01.pos.x);
				binaryWriter.Write(itemInfo01.pos.y);
				binaryWriter.Write(itemInfo01.pos.z);
				binaryWriter.Write(itemInfo01.rot.x);
				binaryWriter.Write(itemInfo01.rot.y);
				binaryWriter.Write(itemInfo01.rot.z);
				binaryWriter.Write(itemInfo01.scl.x);
				binaryWriter.Write(itemInfo01.scl.y);
				binaryWriter.Write(itemInfo01.scl.z);
				itemInfo02 = new ItemInfo();
				if ((bool)objItem02)
				{
					if ((bool)objItem02.transform.parent)
					{
						itemInfo02.parentName = objItem02.transform.parent.name;
					}
					itemInfo02.pos.x = objItem02.transform.localPosition.x;
					itemInfo02.pos.y = objItem02.transform.localPosition.y;
					itemInfo02.pos.z = objItem02.transform.localPosition.z;
					itemInfo02.rot.x = objItem02.transform.localEulerAngles.x;
					itemInfo02.rot.y = objItem02.transform.localEulerAngles.y;
					itemInfo02.rot.z = objItem02.transform.localEulerAngles.z;
					itemInfo02.scl.x = objItem02.transform.localScale.x;
					itemInfo02.scl.y = objItem02.transform.localScale.y;
					itemInfo02.scl.z = objItem02.transform.localScale.z;
				}
				binaryWriter.Write(itemName02);
				binaryWriter.Write(itemInfo02.parentName);
				binaryWriter.Write(itemInfo02.pos.x);
				binaryWriter.Write(itemInfo02.pos.y);
				binaryWriter.Write(itemInfo02.pos.z);
				binaryWriter.Write(itemInfo02.rot.x);
				binaryWriter.Write(itemInfo02.rot.y);
				binaryWriter.Write(itemInfo02.rot.z);
				binaryWriter.Write(itemInfo02.scl.x);
				binaryWriter.Write(itemInfo02.scl.y);
				binaryWriter.Write(itemInfo02.scl.z);
				camCtrl.CameraDataSaveBinary(binaryWriter);
			}
		}
	}

	private void LoadSetting(int chaNo)
	{
		string path = Application.dataPath + "/.." + chaSaveFile[chaNo];
		if (!File.Exists(path))
		{
			itemName01 = string.Empty;
			itemName02 = string.Empty;
			anmCtrlName = string.Empty;
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
				anmCtrlName = binaryReader.ReadString();
				itemName01 = binaryReader.ReadString();
				itemInfo01 = new ItemInfo();
				itemInfo01.parentName = binaryReader.ReadString();
				itemInfo01.pos.x = binaryReader.ReadSingle();
				itemInfo01.pos.y = binaryReader.ReadSingle();
				itemInfo01.pos.z = binaryReader.ReadSingle();
				itemInfo01.rot.x = binaryReader.ReadSingle();
				itemInfo01.rot.y = binaryReader.ReadSingle();
				itemInfo01.rot.z = binaryReader.ReadSingle();
				itemInfo01.scl.x = binaryReader.ReadSingle();
				itemInfo01.scl.y = binaryReader.ReadSingle();
				itemInfo01.scl.z = binaryReader.ReadSingle();
				itemName02 = binaryReader.ReadString();
				itemInfo02 = new ItemInfo();
				itemInfo02.parentName = binaryReader.ReadString();
				itemInfo02.pos.x = binaryReader.ReadSingle();
				itemInfo02.pos.y = binaryReader.ReadSingle();
				itemInfo02.pos.z = binaryReader.ReadSingle();
				itemInfo02.rot.x = binaryReader.ReadSingle();
				itemInfo02.rot.y = binaryReader.ReadSingle();
				itemInfo02.rot.z = binaryReader.ReadSingle();
				itemInfo02.scl.x = binaryReader.ReadSingle();
				itemInfo02.scl.y = binaryReader.ReadSingle();
				itemInfo02.scl.z = binaryReader.ReadSingle();
				camCtrl.CameraDataLoadBinary(binaryReader, true);
			}
		}
	}
}
