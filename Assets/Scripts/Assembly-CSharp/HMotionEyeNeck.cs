using System;
using System.Collections.Generic;
using Illusion.CustomAttributes;
using IllusionUtility.GetUtility;
using Sirenix.OdinInspector;
using UnityEngine;

public class HMotionEyeNeck : SerializedMonoBehaviour
{
	[Serializable]
	public class EyeNeckRange
	{
		[Label("上上限")]
		public float up = -60f;

		[Label("下上限")]
		public float down = 60f;

		[Label("左上限")]
		public float left = -60f;

		[Label("右上限")]
		public float right = 60f;
	}

	[Serializable]
	public class EyeNeckPtnInfo
	{
		[Label("ID")]
		public int id;

		[Label("挙動")]
		public int behaviour;

		[Label("首タゲ")]
		public int targetNeck;

		[Label("目タゲ")]
		public int targetEye;

		[Label("首向き幅")]
		public float rateNeck;

		[Label("目向き幅")]
		public float rateEye;
	}

	[SerializeField]
	protected Dictionary<int, EyeNeckPtnInfo> dicInfo = new Dictionary<int, EyeNeckPtnInfo>();

	protected ChaControl chara;

	protected ChaControl partner;

	[Label("フラグ類")]
	public HFlag flags;

	[Label("相手顔オブジェクト名")]
	public string namePartnerHead = string.Empty;

	[Label("相手性器オブジェクト名")]
	public string namePartnerGenital = string.Empty;

	[Label("自分性器オブジェクト名")]
	public string nameGenital = string.Empty;

	[Label("相手女顔オブジェクト名")]
	public string namePartnerHead1 = string.Empty;

	[Label("相手女性器オブジェクト名")]
	public string namePartnerGenital1 = string.Empty;

	[DisabledGroup("男顔オブジェクト")]
	[SerializeField]
	protected GameObject objPartnerHead;

	[SerializeField]
	[DisabledGroup("男性器オブジェクト")]
	protected GameObject objPartnerKokan;

	[DisabledGroup("自分股間オブジェクト")]
	[SerializeField]
	protected GameObject objKokan;

	[SerializeField]
	[DisabledGroup("女顔オブジェクト")]
	protected GameObject objPartner1Head;

	[SerializeField]
	[DisabledGroup("女性器オブジェクト")]
	protected GameObject objPartner1Kokan;

	[SerializeField]
	[DisabledGroup("相手体メッシュオブジェクト")]
	protected GameObject objPartnerBody;

	[DisabledGroup("相手単色体メッシュオブジェクト")]
	[SerializeField]
	protected GameObject objPartnerSilhouetteBody;

	protected bool LoadEyeNeckPtn()
	{
		dicInfo.Clear();
		string text = GlobalMethod.LoadAllListText("h/list/", "eyeneckbase");
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int num2 = int.Parse(data[i, num++]);
			if (!dicInfo.ContainsKey(num2))
			{
				dicInfo.Add(num2, new EyeNeckPtnInfo());
			}
			EyeNeckPtnInfo eyeNeckPtnInfo = dicInfo[num2];
			eyeNeckPtnInfo.id = num2;
			eyeNeckPtnInfo.behaviour = int.Parse(data[i, num++]);
			eyeNeckPtnInfo.targetNeck = int.Parse(data[i, num++]);
			eyeNeckPtnInfo.targetEye = int.Parse(data[i, num++]);
			eyeNeckPtnInfo.rateNeck = float.Parse(data[i, num++]);
			eyeNeckPtnInfo.rateEye = float.Parse(data[i, num++]);
		}
		return true;
	}

	public bool SetPartner(GameObject _objPartnerBone)
	{
		objPartnerHead = null;
		objPartnerKokan = null;
		if ((bool)_objPartnerBone)
		{
			if (namePartnerHead != string.Empty)
			{
				objPartnerHead = _objPartnerBone.transform.FindLoop(namePartnerHead);
			}
			if (namePartnerGenital != string.Empty)
			{
				objPartnerKokan = _objPartnerBone.transform.FindLoop(namePartnerGenital);
			}
		}
		return true;
	}

	public bool SetFemalePartner(GameObject _objPartnerBone)
	{
		objPartner1Head = null;
		objPartner1Kokan = null;
		if ((bool)_objPartnerBone)
		{
			if (namePartnerHead1 != string.Empty)
			{
				objPartner1Head = _objPartnerBone.transform.FindLoop(namePartnerHead1);
			}
			if (namePartnerGenital1 != string.Empty)
			{
				objPartner1Kokan = _objPartnerBone.transform.FindLoop(namePartnerGenital1);
			}
		}
		return true;
	}

	protected bool SetEyeNeckPtn(int _id, GameObject _objCamera, bool _isConfigEyeDisregard, bool _isConfigNeckDisregard)
	{
		if (_id >= 1000)
		{
			switch (_id)
			{
			case 1000:
				chara.ChangeLookEyesPtn(5);
				SetEyesTarget(0, 0.5f, _objCamera, true);
				break;
			case 1001:
				chara.ChangeLookNeckPtn(5);
				SetNeckTarget(0, 0.5f, _objCamera, true);
				break;
			case 1002:
				chara.ChangeLookEyesPtn(5);
				chara.ChangeLookNeckPtn(5);
				SetNeckTarget(0, 0.5f, _objCamera, true);
				SetEyesTarget(0, 0.5f, _objCamera, true);
				break;
			}
			return true;
		}
		if (!dicInfo.ContainsKey(_id))
		{
			return false;
		}
		EyeNeckPtnInfo eyeNeckPtnInfo = dicInfo[_id];
		SetBehaviour(eyeNeckPtnInfo.behaviour);
		SetNeckTarget(eyeNeckPtnInfo.targetNeck, eyeNeckPtnInfo.rateNeck, _objCamera, _isConfigNeckDisregard);
		SetEyesTarget(eyeNeckPtnInfo.targetEye, eyeNeckPtnInfo.rateNeck, _objCamera, _isConfigEyeDisregard);
		return true;
	}

	private bool SetBehaviour(int _behaviour)
	{
		switch (_behaviour)
		{
		case 0:
			chara.ChangeLookEyesPtn(4);
			chara.ChangeLookNeckPtn(3);
			break;
		case 1:
			chara.ChangeLookEyesPtn(5);
			chara.ChangeLookNeckPtn(3);
			break;
		case 2:
			chara.ChangeLookEyesPtn(5);
			chara.ChangeLookNeckPtn(5);
			break;
		case 3:
			chara.ChangeLookEyesPtn(7);
			chara.ChangeLookNeckPtn(3);
			break;
		case 4:
			chara.ChangeLookEyesPtn(7);
			chara.ChangeLookNeckPtn(6);
			break;
		case 5:
			chara.ChangeLookEyesPtn(7);
			chara.ChangeLookNeckPtn(5);
			break;
		case 6:
			chara.ChangeLookEyesPtn(5);
			chara.ChangeLookNeckPtn(6);
			break;
		case 7:
			chara.ChangeLookEyesPtn(4);
			chara.ChangeLookNeckPtn(5);
			break;
		case 8:
			chara.ChangeLookEyesPtn(4);
			chara.ChangeLookNeckPtn(6);
			break;
		case 9:
			chara.ChangeLookEyesPtn(5);
			chara.ChangeLookNeckPtn(0);
			break;
		case 10:
			chara.ChangeLookEyesPtn(7);
			chara.ChangeLookNeckPtn(0);
			break;
		case 11:
			chara.ChangeLookEyesPtn(4);
			chara.ChangeLookNeckPtn(0);
			break;
		default:
			chara.ChangeLookEyesPtn(4);
			chara.ChangeLookNeckPtn(3);
			break;
		}
		return true;
	}

	private bool SetNeckTarget(int _tag, float _rate, GameObject _objCamera, bool _isConfigDisregard)
	{
		switch (_tag)
		{
		case 0:
			chara.ChangeLookNeckTarget(0, (!_objCamera) ? null : _objCamera.transform);
			break;
		case 1:
			chara.ChangeLookNeckTarget(0, (flags.mode == HFlag.EMode.aibu) ? ((!_objCamera) ? null : _objCamera.transform) : (objPartnerHead ? objPartnerHead.transform : ((!_objCamera) ? null : _objCamera.transform)));
			break;
		case 2:
			chara.ChangeLookNeckTarget(0, (flags.mode == HFlag.EMode.aibu) ? ((!_objCamera) ? null : _objCamera.transform) : (objPartnerKokan ? objPartnerKokan.transform : ((!_objCamera) ? null : _objCamera.transform)));
			break;
		case 3:
			chara.ChangeLookNeckTarget(0, objKokan ? objKokan.transform : ((!_objCamera) ? null : _objCamera.transform));
			break;
		case 4:
			chara.ChangeLookNeckTarget(1, null, _rate);
			break;
		case 5:
			chara.ChangeLookNeckTarget(2, null, _rate);
			break;
		case 6:
			chara.ChangeLookNeckTarget(3, null, _rate);
			break;
		case 7:
			chara.ChangeLookNeckTarget(4, null, _rate);
			break;
		case 8:
			chara.ChangeLookNeckTarget(5, null, _rate);
			break;
		case 9:
			chara.ChangeLookNeckTarget(6, null, _rate);
			break;
		case 10:
			chara.ChangeLookNeckTarget(7, null, _rate);
			break;
		case 11:
			chara.ChangeLookNeckTarget(8, null, _rate);
			break;
		case 12:
			chara.ChangeLookNeckTarget(0, (flags.mode == HFlag.EMode.aibu) ? ((!_objCamera) ? null : _objCamera.transform) : (objPartner1Head ? objPartner1Head.transform : ((!_objCamera) ? null : _objCamera.transform)));
			break;
		case 13:
			chara.ChangeLookNeckTarget(0, (flags.mode == HFlag.EMode.aibu) ? ((!_objCamera) ? null : _objCamera.transform) : (objPartner1Kokan ? objPartner1Kokan.transform : ((!_objCamera) ? null : _objCamera.transform)));
			break;
		default:
			chara.ChangeLookNeckTarget(0, (!_objCamera) ? null : _objCamera.transform);
			break;
		}
		return true;
	}

	private bool SetEyesTarget(int _tag, float _rate, GameObject _objCamera, bool _isConfigDisregard)
	{
		switch (_tag)
		{
		case 0:
			chara.ChangeLookEyesTarget(0, (!_objCamera) ? null : _objCamera.transform);
			break;
		case 1:
			chara.ChangeLookEyesTarget(0, objPartnerHead ? objPartnerHead.transform : ((!_objCamera) ? null : _objCamera.transform));
			break;
		case 2:
			chara.ChangeLookEyesTarget(0, objPartnerKokan ? objPartnerKokan.transform : ((!_objCamera) ? null : _objCamera.transform));
			break;
		case 3:
			chara.ChangeLookEyesTarget(0, objKokan ? objKokan.transform : ((!_objCamera) ? null : _objCamera.transform));
			break;
		case 4:
			chara.ChangeLookEyesTarget(1, null, _rate);
			break;
		case 5:
			chara.ChangeLookEyesTarget(2, null, _rate);
			break;
		case 6:
			chara.ChangeLookEyesTarget(3, null, _rate);
			break;
		case 7:
			chara.ChangeLookEyesTarget(4, null, _rate);
			break;
		case 8:
			chara.ChangeLookEyesTarget(5, null, _rate);
			break;
		case 9:
			chara.ChangeLookEyesTarget(6, null, _rate);
			break;
		case 10:
			chara.ChangeLookEyesTarget(7, null, _rate);
			break;
		case 11:
			chara.ChangeLookEyesTarget(8, null, _rate);
			break;
		case 12:
			chara.ChangeLookEyesTarget(0, objPartner1Head ? objPartner1Head.transform : ((!_objCamera) ? null : _objCamera.transform));
			break;
		case 13:
			chara.ChangeLookEyesTarget(0, objPartner1Kokan ? objPartner1Kokan.transform : ((!_objCamera) ? null : _objCamera.transform));
			break;
		default:
			chara.ChangeLookEyesTarget(0, (!_objCamera) ? null : _objCamera.transform);
			break;
		}
		return true;
	}
}
