using System;
using System.Collections.Generic;
using IllusionUtility.SetUtility;
using UnityEngine;

public class ExpressionBone : MonoBehaviour
{
	public enum ExpressionIndex
	{
		ShoulderL = 0,
		UpperarmL = 1,
		ElboL = 2,
		ForearmL = 3,
		HandL = 4,
		ShoulderR = 5,
		UpperarmR = 6,
		ElboR = 7,
		ForearmR = 8,
		HandR = 9,
		LegL = 10,
		ThighL = 11,
		KneeL = 12,
		CalfL = 13,
		LegR = 14,
		ThighR = 15,
		KneeR = 16,
		CalfR = 17,
		Bust = 18,
		BustColDB = 19,
		AcsNull = 20
	}

	public enum ExpressionCategory
	{
		CN_ArmLeft = 0,
		CN_ArmRight = 1,
		CN_LegLeft = 2,
		CN_LegRight = 3,
		CN_ForearmLeft = 4,
		CN_ForearmRight = 5,
		CN_ThighLeft = 6,
		CN_ThighRight = 7,
		CN_Bust = 8,
		CN_AcsNull = 9
	}

	public enum BoneName
	{
		cf_j_waist02 = 0,
		cf_n_height = 1,
		cf_s_elbo_L = 2,
		cf_s_elboback_L = 3,
		cf_j_forearm01_L = 4,
		cf_s_elbo_R = 5,
		cf_s_elboback_R = 6,
		cf_j_forearm01_R = 7,
		cf_d_kneeF_L = 8,
		cf_s_kneeB_L = 9,
		cf_j_leg01_L = 10,
		cf_d_kneeF_R = 11,
		cf_s_kneeB_R = 12,
		cf_j_leg01_R = 13,
		cf_d_leg02_L = 14,
		cf_j_leg03_L = 15,
		cf_d_leg02_R = 16,
		cf_j_leg03_R = 17,
		cf_d_forearm02_L = 18,
		cf_d_hand_L = 19,
		cf_j_hand_L = 20,
		cf_j_middle01_L = 21,
		cf_d_wrist_L = 22,
		cf_d_forearm02_R = 23,
		cf_d_hand_R = 24,
		cf_j_hand_R = 25,
		cf_j_middle01_R = 26,
		cf_d_wrist_R = 27,
		cf_j_arm00_L = 28,
		cf_d_shoulder02_L = 29,
		cf_d_arm01_L = 30,
		cf_d_arm02_L = 31,
		cf_d_arm03_L = 32,
		n_goal_forearm_L = 33,
		cf_j_arm00_R = 34,
		cf_d_shoulder02_R = 35,
		cf_d_arm01_R = 36,
		cf_d_arm02_R = 37,
		cf_d_arm03_R = 38,
		n_goal_forearm_R = 39,
		cf_s_leg_L = 40,
		cf_d_thigh01_L = 41,
		cf_d_thigh02_L = 42,
		cf_d_thigh03_L = 43,
		cf_j_thigh00_L = 44,
		n_leg_goal_L = 45,
		cf_s_leg_R = 46,
		cf_d_thigh01_R = 47,
		cf_d_thigh02_R = 48,
		cf_d_thigh03_R = 49,
		cf_j_thigh00_R = 50,
		n_leg_goal_R = 51,
		cf_d_siri_L = 52,
		cf_d_siri_R = 53,
		cf_d_bust00 = 54,
		cf_j_spine03 = 55,
		cf_d_hit_bust_L = 56,
		cf_j_bust02_L = 57,
		cf_d_hit_bust_R = 58,
		cf_j_bust02_R = 59,
		a_n_neck = 60,
		cf_j_head = 61
	}

	public bool enable = true;

	[SerializeField]
	[Header("< 左腕 >----------------------------")]
	private bool enableShoulderL = true;

	[SerializeField]
	private bool enableUpperarmL = true;

	[SerializeField]
	private bool enableElboL = true;

	[SerializeField]
	private bool enableForearmL = true;

	[SerializeField]
	private bool enableHandL = true;

	[Header("< 右腕 >----------------------------")]
	[SerializeField]
	private bool enableShoulderR = true;

	[SerializeField]
	private bool enableUpperarmR = true;

	[SerializeField]
	private bool enableElboR = true;

	[SerializeField]
	private bool enableForearmR = true;

	[SerializeField]
	private bool enableHandR = true;

	[SerializeField]
	[Header("< 左足 >----------------------------")]
	private bool enableLegL = true;

	[SerializeField]
	private bool enableThighL = true;

	[SerializeField]
	private bool enableKneeL = true;

	[SerializeField]
	private bool enableCalfL = true;

	[SerializeField]
	[Header("< 右足 >----------------------------")]
	private bool enableLegR = true;

	[SerializeField]
	private bool enableThighR = true;

	[SerializeField]
	private bool enableKneeR = true;

	[SerializeField]
	private bool enableCalfR = true;

	[Header("< 胸 >------------------------------")]
	[SerializeField]
	private bool enableBust = true;

	[SerializeField]
	private bool enableBustColDB = true;

	[Header("< アクセサリNULL >------------------")]
	[SerializeField]
	private bool enableAcsNull = true;

	[HideInInspector]
	public Transform trfBoneRoot;

	private Transform calcTranceform;

	public Dictionary<int, Transform> dictBone { get; private set; }

	public void Initialize()
	{
		GameObject gameObject = new GameObject("objCalculation");
		calcTranceform = gameObject.transform;
		calcTranceform.SetParent(base.transform, false);
		FindAssist findAssist = new FindAssist();
		findAssist.Initialize(trfBoneRoot);
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>(findAssist.dictObjName);
		string[] array = new string[2] { "cf_j_shoulder_L", "cf_j_shoulder_R" };
		string[] array2 = new string[2] { "n_goal_forearm_L", "n_goal_forearm_R" };
		for (int i = 0; i < 2; i++)
		{
			GameObject objectFromName = findAssist.GetObjectFromName(array[i]);
			GameObject gameObject2 = new GameObject(array2[i]);
			gameObject2.transform.SetParent(objectFromName.transform, false);
			gameObject2.transform.localPosition = new Vector3(0f, 0.3f, 0f);
			dictionary[array2[i]] = gameObject2;
		}
		string[] array3 = new string[2] { "cf_j_waist02", "cf_j_waist02" };
		string[] array4 = new string[2] { "n_leg_goal_L", "n_leg_goal_R" };
		for (int j = 0; j < 2; j++)
		{
			GameObject objectFromName2 = findAssist.GetObjectFromName(array3[j]);
			GameObject gameObject3 = new GameObject(array4[j]);
			gameObject3.transform.SetParent(objectFromName2.transform, false);
			gameObject3.transform.localPosition = new Vector3(0.083f * ((j != 0) ? 1f : (-1f)), 0.4f, -0.008f);
			dictionary[array4[j]] = gameObject3;
		}
		int length = Enum.GetValues(typeof(BoneName)).Length;
		dictBone = new Dictionary<int, Transform>();
		for (int k = 0; k < length; k++)
		{
			GameObject value = null;
			BoneName boneName = (BoneName)k;
			dictionary.TryGetValue(boneName.ToString(), out value);
			if ((bool)value)
			{
				dictBone[k] = value.transform;
			}
			else
			{
				dictBone[k] = null;
			}
		}
	}

	public void EnableIndex(ExpressionIndex index, bool _enable)
	{
		switch (index)
		{
		case ExpressionIndex.ShoulderL:
			enableShoulderL = _enable;
			break;
		case ExpressionIndex.UpperarmL:
			enableUpperarmL = _enable;
			break;
		case ExpressionIndex.ElboL:
			enableElboL = _enable;
			break;
		case ExpressionIndex.ForearmL:
			enableForearmL = _enable;
			break;
		case ExpressionIndex.HandL:
			enableHandL = _enable;
			break;
		case ExpressionIndex.ShoulderR:
			enableShoulderR = _enable;
			break;
		case ExpressionIndex.UpperarmR:
			enableUpperarmR = _enable;
			break;
		case ExpressionIndex.ElboR:
			enableElboR = _enable;
			break;
		case ExpressionIndex.ForearmR:
			enableForearmR = _enable;
			break;
		case ExpressionIndex.HandR:
			enableHandR = _enable;
			break;
		case ExpressionIndex.LegL:
			enableLegL = _enable;
			break;
		case ExpressionIndex.ThighL:
			enableThighL = _enable;
			break;
		case ExpressionIndex.KneeL:
			enableKneeL = _enable;
			break;
		case ExpressionIndex.CalfL:
			enableCalfL = _enable;
			break;
		case ExpressionIndex.LegR:
			enableLegR = _enable;
			break;
		case ExpressionIndex.ThighR:
			enableThighR = _enable;
			break;
		case ExpressionIndex.KneeR:
			enableKneeR = _enable;
			break;
		case ExpressionIndex.CalfR:
			enableCalfR = _enable;
			break;
		case ExpressionIndex.Bust:
			enableBust = _enable;
			break;
		case ExpressionIndex.BustColDB:
			enableBustColDB = _enable;
			break;
		case ExpressionIndex.AcsNull:
			enableAcsNull = _enable;
			break;
		}
	}

	public void EnableCategory(ExpressionCategory category, bool _enable)
	{
		switch (category)
		{
		case ExpressionCategory.CN_ArmLeft:
			enableShoulderL = _enable;
			enableUpperarmL = _enable;
			enableElboL = _enable;
			enableHandL = _enable;
			break;
		case ExpressionCategory.CN_ArmRight:
			enableShoulderR = _enable;
			enableUpperarmR = _enable;
			enableElboR = _enable;
			enableHandR = _enable;
			break;
		case ExpressionCategory.CN_LegLeft:
			enableLegL = _enable;
			enableKneeL = _enable;
			enableCalfL = _enable;
			break;
		case ExpressionCategory.CN_LegRight:
			enableLegR = _enable;
			enableKneeR = _enable;
			enableCalfR = _enable;
			break;
		case ExpressionCategory.CN_ForearmLeft:
			enableForearmL = _enable;
			break;
		case ExpressionCategory.CN_ForearmRight:
			enableForearmR = _enable;
			break;
		case ExpressionCategory.CN_ThighLeft:
			enableThighL = _enable;
			break;
		case ExpressionCategory.CN_ThighRight:
			enableThighR = _enable;
			break;
		case ExpressionCategory.CN_Bust:
			enableBust = _enable;
			enableBustColDB = _enable;
			break;
		case ExpressionCategory.CN_AcsNull:
			enableAcsNull = _enable;
			break;
		}
	}

	public void OnDestroy()
	{
		if ((bool)calcTranceform)
		{
			UnityEngine.Object.Destroy(calcTranceform);
		}
	}

	public void LateUpdate()
	{
		if (dictBone == null || dictBone.Count == 0 || !enable)
		{
			return;
		}
		Vector3 vector = Vector3.one;
		Transform transform = dictBone[1];
		if ((bool)transform)
		{
			vector = transform.localScale;
		}
		if (enableShoulderL)
		{
			Transform transform2 = dictBone[28];
			Transform transform3 = dictBone[29];
			Transform transform4 = dictBone[4];
			Transform transform5 = dictBone[33];
			if ((bool)transform2 && (bool)transform3 && (bool)transform5 && (bool)transform4)
			{
				Vector3 vector2 = transform5.InverseTransformPoint(transform4.position);
				float num = Mathf.Abs(vector2.y);
				float num2 = Mathf.Abs(vector2.z);
				float num3 = 0f;
				if (num <= 0.3f)
				{
					num3 = (0.3f - num) * 250f;
					transform3.SetLocalPositionX(transform2.localPosition.x - (0.3f - num) * 0.03f - num2 * 0.03f);
					transform3.SetLocalPositionY((0.3f - num) * 0.2f);
				}
				else
				{
					num3 = 0f;
					transform3.SetLocalPositionX(transform2.localPosition.x - num2 * 0.03f);
					transform3.SetLocalPositionY(0f);
				}
				Vector3 vector3 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.yzx, transform3.localRotation);
				vector3.y = vector2.z * 50f;
				vector3.z = Mathf.Min(0f, 0f - num3);
				Quaternion localRotation = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.yzx, vector3.x, vector3.y, vector3.z);
				transform3.localRotation = localRotation;
				transform3.SetLocalPositionZ((0f - vector2.z) * 0.06f);
			}
		}
		if (enableShoulderR)
		{
			Transform transform6 = dictBone[34];
			Transform transform7 = dictBone[35];
			Transform transform8 = dictBone[7];
			Transform transform9 = dictBone[39];
			if ((bool)transform6 && (bool)transform7 && (bool)transform9 && (bool)transform8)
			{
				Vector3 vector4 = transform9.InverseTransformPoint(transform8.position);
				float num4 = Mathf.Abs(vector4.y);
				float num5 = Mathf.Abs(vector4.z);
				float num6 = 0f;
				if (num4 <= 0.3f)
				{
					num6 = (0.3f - num4) * -250f;
					transform7.SetLocalPositionX(transform6.localPosition.x + (0.3f - num4) * 0.03f + num5 * 0.03f);
					transform7.SetLocalPositionY((0.3f - num4) * 0.2f);
				}
				else
				{
					num6 = 0f;
					transform7.SetLocalPositionX(transform6.localPosition.x + num5 * 0.03f);
					transform7.SetLocalPositionY(0f);
				}
				Vector3 vector5 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.yzx, transform7.localRotation);
				vector5.y = (0f - vector4.z) * 50f;
				vector5.z = Mathf.Max(0f, 0f - num6);
				Quaternion localRotation2 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.yzx, vector5.x, vector5.y, vector5.z);
				transform7.localRotation = localRotation2;
				transform7.SetLocalPositionZ((0f - vector4.z) * 0.06f);
			}
		}
		if (enableUpperarmL)
		{
			Transform transform10 = dictBone[28];
			if ((bool)transform10)
			{
				float x = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform10.localRotation).x;
				Transform transform11 = dictBone[30];
				if ((bool)transform11)
				{
					Vector3 vector6 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform11.localRotation);
					vector6.x = (0f - x) * 0.6f;
					Quaternion localRotation3 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector6.x, vector6.y, vector6.z);
					transform11.localRotation = localRotation3;
				}
				transform11 = dictBone[31];
				if ((bool)transform11)
				{
					Vector3 vector6 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform11.localRotation);
					vector6.x = (0f - x) * 0.5f;
					Quaternion localRotation4 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector6.x, vector6.y, vector6.z);
					transform11.localRotation = localRotation4;
				}
				transform11 = dictBone[32];
				if ((bool)transform11)
				{
					Vector3 vector6 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform11.localRotation);
					vector6.x = (0f - x) * 0.125f;
					Quaternion localRotation5 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector6.x, vector6.y, vector6.z);
					transform11.localRotation = localRotation5;
				}
			}
		}
		if (enableUpperarmR)
		{
			Transform transform12 = dictBone[34];
			if ((bool)transform12)
			{
				float x2 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform12.localRotation).x;
				Transform transform13 = dictBone[36];
				if ((bool)transform13)
				{
					Vector3 vector7 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform13.localRotation);
					vector7.x = (0f - x2) * 0.6f;
					Quaternion localRotation6 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector7.x, vector7.y, vector7.z);
					transform13.localRotation = localRotation6;
				}
				transform13 = dictBone[37];
				if ((bool)transform13)
				{
					Vector3 vector7 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform13.localRotation);
					vector7.x = (0f - x2) * 0.5f;
					Quaternion localRotation7 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector7.x, vector7.y, vector7.z);
					transform13.localRotation = localRotation7;
				}
				transform13 = dictBone[38];
				if ((bool)transform13)
				{
					Vector3 vector7 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform13.localRotation);
					vector7.x = (0f - x2) * 0.125f;
					Quaternion localRotation8 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector7.x, vector7.y, vector7.z);
					transform13.localRotation = localRotation8;
				}
			}
		}
		if (enableElboL)
		{
			Transform transform14 = dictBone[4];
			if ((bool)transform14)
			{
				float y = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zxy, transform14.localRotation).y;
				Transform transform15 = dictBone[2];
				if ((bool)transform15)
				{
					Vector3 vector8 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zxy, transform15.localRotation);
					vector8.y = (0f - y) * 0.5f;
					Quaternion localRotation9 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zxy, vector8.x, vector8.y, vector8.z);
					transform15.localRotation = localRotation9;
					transform15.SetLocalPositionX(Mathf.Sin(y * 0.015f) * 0.22f * 0.1f);
					transform15.SetLocalPositionZ((-0.25f + Mathf.Sin((y - 1f) * 0.006f) * 0.18f) * 0.1f);
				}
				transform15 = dictBone[3];
				if ((bool)transform15)
				{
					Vector3 vector8 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zxy, transform15.localRotation);
					vector8.y = (0f - y) * 0.5f;
					Quaternion localRotation10 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zxy, vector8.x, vector8.y, vector8.z);
					transform15.localRotation = localRotation10;
					transform15.SetLocalPositionX((0f - Mathf.Tan(y * 0.0039f)) * 12f * 0.1f);
					transform15.SetLocalPositionZ((0.3f - Mathf.Sin((0f - y) * 0.009f) * 7.6f - Mathf.Tan(y * 0.0022f) * 16f) * 0.1f);
				}
			}
		}
		if (enableElboR)
		{
			Transform transform16 = dictBone[7];
			if ((bool)transform16)
			{
				float y2 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zxy, transform16.localRotation).y;
				Transform transform17 = dictBone[5];
				if ((bool)transform17)
				{
					Vector3 vector9 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zxy, transform17.localRotation);
					vector9.y = (0f - y2) * 0.5f;
					Quaternion localRotation11 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zxy, vector9.x, vector9.y, vector9.z);
					transform17.localRotation = localRotation11;
					transform17.SetLocalPositionX(Mathf.Sin(y2 * 0.015f) * 0.22f * 0.1f);
					transform17.SetLocalPositionZ((-0.25f - Mathf.Sin((y2 - 1f) * 0.006f) * 0.18f) * 0.1f);
				}
				transform17 = dictBone[6];
				if ((bool)transform17)
				{
					Vector3 vector9 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zxy, transform17.localRotation);
					vector9.y = (0f - y2) * 0.5f;
					Quaternion localRotation12 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zxy, vector9.x, vector9.y, vector9.z);
					transform17.localRotation = localRotation12;
					transform17.SetLocalPositionX((0f - Mathf.Tan(y2 * 0.0039f)) * 12f * 0.1f);
					transform17.SetLocalPositionZ((0.3f - Mathf.Sin(y2 * 0.009f) * 7.6f + Mathf.Tan(y2 * 0.0022f) * 16f) * 0.1f);
				}
			}
		}
		if (enableForearmL)
		{
			Transform transform18 = dictBone[4];
			Transform transform19 = dictBone[20];
			Transform transform20 = dictBone[21];
			if ((bool)transform18 && (bool)transform19 && (bool)transform20)
			{
				calcTranceform.localScale = Vector3.one;
				calcTranceform.position = transform19.position;
				calcTranceform.rotation = transform18.rotation;
				Vector3 vector10 = calcTranceform.InverseTransformPoint(transform20.position);
				vector10.y = Mathf.Max(0f, vector10.y);
				float f = vector10.y * 100f * ((float)Math.PI / 180f);
				Transform transform21 = dictBone[19];
				if ((bool)transform21)
				{
					transform21.SetLocalPositionX((0f - (0.25f - Mathf.Sin(f) * 15f)) * 0.1f);
					transform21.SetLocalPositionY(Mathf.Sin(f) * 15f * 0.1f);
				}
				calcTranceform.position = transform19.position;
				calcTranceform.rotation = transform19.rotation;
				calcTranceform.localScale = transform19.localScale;
				calcTranceform.Translate(-0.01f, 0.01f, 0f, Space.Self);
				vector10 = calcTranceform.position;
				vector10 = transform18.InverseTransformPoint(vector10);
				vector10.x = 0f;
				vector10 = vector10.normalized;
				float x3 = Vector3.Angle(Vector3.up, vector10) * Mathf.Sign(vector10.z);
				transform21 = dictBone[22];
				if ((bool)transform21)
				{
					transform21.SetLocalRotationX(x3);
				}
			}
		}
		if (enableForearmR)
		{
			Transform transform22 = dictBone[7];
			Transform transform23 = dictBone[25];
			Transform transform24 = dictBone[26];
			if ((bool)transform22 && (bool)transform23 && (bool)transform24)
			{
				calcTranceform.localScale = Vector3.one;
				calcTranceform.position = transform23.position;
				calcTranceform.rotation = transform22.rotation;
				Vector3 vector11 = calcTranceform.InverseTransformPoint(transform24.position);
				vector11.y = Mathf.Max(0f, vector11.y);
				float f2 = vector11.y * 100f * ((float)Math.PI / 180f);
				Transform transform25 = dictBone[24];
				if ((bool)transform25)
				{
					transform25.SetLocalPositionX((0f - (-0.25f + Mathf.Sin(f2) * 15f)) * 0.1f);
					transform25.SetLocalPositionY(Mathf.Sin(f2) * 15f * 0.1f);
				}
				calcTranceform.position = transform23.position;
				calcTranceform.rotation = transform23.rotation;
				calcTranceform.localScale = transform23.localScale;
				calcTranceform.Translate(0.01f, 0.01f, 0f, Space.Self);
				vector11 = calcTranceform.position;
				vector11 = transform22.InverseTransformPoint(vector11);
				vector11.x = 0f;
				vector11 = vector11.normalized;
				float x4 = Vector3.Angle(Vector3.up, vector11) * Mathf.Sign(vector11.z);
				transform25 = dictBone[27];
				if ((bool)transform25)
				{
					transform25.SetLocalRotationX(x4);
				}
			}
		}
		if (enableHandL)
		{
			Transform transform26 = dictBone[20];
			if ((bool)transform26)
			{
				float x5 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform26.localRotation).x;
				Transform transform27 = dictBone[18];
				if ((bool)transform27)
				{
					Vector3 vector12 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform27.localRotation);
					vector12.x = x5 * 0.45f;
					Quaternion localRotation13 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector12.x, vector12.y, vector12.z);
					transform27.localRotation = localRotation13;
				}
			}
		}
		if (enableHandR)
		{
			Transform transform28 = dictBone[25];
			if ((bool)transform28)
			{
				float x6 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform28.localRotation).x;
				Transform transform29 = dictBone[23];
				if ((bool)transform29)
				{
					Vector3 vector13 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform29.localRotation);
					vector13.x = x6 * 0.45f;
					Quaternion localRotation14 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector13.x, vector13.y, vector13.z);
					transform29.localRotation = localRotation14;
				}
			}
		}
		if (enableLegL)
		{
			float num7 = 0f;
			Transform transform30 = dictBone[0];
			Transform transform31 = dictBone[44];
			Transform transform32 = dictBone[10];
			if ((bool)transform30 && (bool)transform31 && (bool)transform32)
			{
				calcTranceform.localScale = Vector3.one;
				calcTranceform.position = transform31.position;
				calcTranceform.rotation = transform30.rotation;
				Vector3 vector14 = calcTranceform.InverseTransformPoint(transform32.position) / vector.y * 10f;
				if (0f <= vector14.y)
				{
					num7 = Mathf.Abs(vector14.y);
				}
				transform30 = dictBone[45];
				transform31 = dictBone[10];
				if ((bool)transform30 && (bool)transform31)
				{
					vector14 = transform30.InverseTransformPoint(transform31.position) * -10f;
					Transform transform33 = dictBone[40];
					if ((bool)transform33)
					{
						transform33.SetLocalPositionX((1f + vector14.x * 0.2f + (Mathf.Abs(vector14.z * 1.1f) - (8.849f - vector14.y)) * 0.06f) * -0.1f);
						transform33.SetLocalPositionY((1f + (8.849f - vector14.y) * 0.4f + vector14.x * 0.2f + num7 * 0.6f) * 0.1f);
						transform33.SetLocalPositionZ(((0f - vector14.z) * 0.2f + vector14.x * 0.05f - num7 * 0.4f) * 0.1f);
						float x7 = vector14.z * 6f - vector14.x * 2f - num7 * 5f;
						float y3 = (0f - vector14.x) * 12f - vector14.z * 3f;
						float z = 0f - vector14.x;
						Quaternion localRotation15 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, x7, y3, z);
						transform33.localRotation = localRotation15;
						transform33.SetLocalScaleX(1f + Mathf.Abs(vector14.z) * 0.2f);
						transform33.SetLocalScaleY(1f + num7 * 0.4f);
						transform33.SetLocalScaleZ(1f + Mathf.Abs(vector14.z) * 0.3f + num7 * 0.3f);
					}
					transform33 = dictBone[52];
					if ((bool)transform33)
					{
						transform33.SetLocalPositionX((-0.7f + vector14.x * 0.01f - Mathf.Abs(vector14.x * 0.02f) - Mathf.Abs(vector14.z * 0.005f)) * 0.1f);
						transform33.SetLocalPositionY(((8.849f - vector14.y) * 0.01f + Mathf.Abs(vector14.z * 0.02f) + vector14.x * 0.04f) * 0.1f);
						transform33.SetLocalPositionZ(((0f - (8.849f - vector14.y)) * 0.03f - (vector14.z - num7 * 3f) * 0.03f - Mathf.Abs(vector14.x) * 0.02f) * 0.1f);
						float x8 = vector14.z * 4f - (8.849f - vector14.y) * 4f - vector14.x * 2f - num7 * 3f;
						float y4 = (0f - vector14.x) * 6f - num7 * 4f;
						float z2 = (0f - vector14.x) * 6f;
						Quaternion quaternion = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, x8, y4, z2);
						transform33.SetLocalScaleX(1f + Mathf.Abs(vector14.z) * 0.02f);
						transform33.SetLocalScaleY(1f + Mathf.Abs(num7) * 0.01f);
					}
				}
			}
		}
		if (enableLegR)
		{
			float num8 = 0f;
			Transform transform34 = dictBone[0];
			Transform transform35 = dictBone[50];
			Transform transform36 = dictBone[13];
			if ((bool)transform34 && (bool)transform35 && (bool)transform36)
			{
				calcTranceform.localScale = Vector3.one;
				calcTranceform.position = transform35.position;
				calcTranceform.rotation = transform34.rotation;
				if ((bool)transform34 && (bool)transform35)
				{
					Vector3 vector15 = calcTranceform.InverseTransformPoint(transform36.position) / vector.y * 10f;
					if (0f <= vector15.y)
					{
						num8 = Mathf.Abs(vector15.y);
					}
					transform34 = dictBone[51];
					transform35 = dictBone[13];
					vector15 = transform34.InverseTransformPoint(transform35.position) * -10f;
					Transform transform37 = dictBone[46];
					if ((bool)transform37)
					{
						transform37.SetLocalPositionX((-1f + vector15.x * 0.2f - (Mathf.Abs(vector15.z * 1.1f) - (8.849f - vector15.y)) * 0.06f) * -0.1f);
						transform37.SetLocalPositionY((1f + (8.849f - vector15.y) * 0.4f - vector15.x * 0.2f + num8 * 0.6f) * 0.1f);
						transform37.SetLocalPositionZ(((0f - vector15.z) * 0.2f - vector15.x * 0.05f - num8 * 0.4f) * 0.1f);
						float x9 = vector15.z * 6f + vector15.x * 2f - num8 * 5f;
						float y5 = (0f - vector15.x) * 12f + vector15.z * 3f;
						float z3 = 0f - vector15.x;
						Quaternion localRotation16 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, x9, y5, z3);
						transform37.localRotation = localRotation16;
						transform37.SetLocalScaleX(1f + Mathf.Abs(vector15.z) * 0.2f);
						transform37.SetLocalScaleY(1f + num8 * 0.4f);
						transform37.SetLocalScaleZ(1f + Mathf.Abs(vector15.z) * 0.3f + num8 * 0.3f);
					}
					transform37 = dictBone[53];
					if ((bool)transform37)
					{
						transform37.SetLocalPositionX((0.7f + vector15.x * 0.01f + Mathf.Abs(vector15.x * 0.02f) + Mathf.Abs(vector15.z * 0.005f)) * 0.1f);
						transform37.SetLocalPositionY(((8.849f - vector15.y) * 0.01f + Mathf.Abs(vector15.z * 0.02f) - vector15.x * 0.04f) * 0.1f);
						transform37.SetLocalPositionZ(((0f - (8.849f - vector15.y)) * 0.03f - (vector15.z - num8 * 3f) * 0.03f - Mathf.Abs(vector15.x) * 0.02f) * 0.1f);
						float x10 = vector15.z * 4f - (8.849f - vector15.y) * 4f + vector15.x * 2f - num8 * 3f;
						float y6 = (0f - vector15.x) * 6f + num8 * 4f;
						float z4 = (0f - vector15.x) * 6f;
						Quaternion quaternion2 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, x10, y6, z4);
						transform37.SetLocalScaleX(1f + Mathf.Abs(vector15.z) * 0.02f);
						transform37.SetLocalScaleY(1f + Mathf.Abs(num8) * 0.01f);
					}
				}
			}
		}
		if (enableThighL)
		{
			Transform transform38 = dictBone[44];
			if (null != transform38)
			{
				float y7 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform38.localRotation).y;
				Transform transform39 = dictBone[41];
				Vector3 vector16 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform39.localRotation);
				vector16.y = (0f - y7) * 0.75f;
				Quaternion localRotation17 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector16.x, vector16.y, vector16.z);
				transform39.localRotation = localRotation17;
				transform39 = dictBone[42];
				vector16 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform39.localRotation);
				vector16.y = (0f - y7) * 0.5f;
				localRotation17 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector16.x, vector16.y, vector16.z);
				transform39.localRotation = localRotation17;
				transform39 = dictBone[43];
				vector16 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform39.localRotation);
				vector16.y = (0f - y7) * 0.25f;
				localRotation17 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector16.x, vector16.y, vector16.z);
				transform39.localRotation = localRotation17;
			}
		}
		if (enableThighR)
		{
			Transform transform40 = dictBone[50];
			if (null != transform40)
			{
				float y8 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform40.localRotation).y;
				Transform transform41 = dictBone[47];
				Vector3 vector17 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform41.localRotation);
				vector17.y = (0f - y8) * 0.75f;
				Quaternion localRotation18 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector17.x, vector17.y, vector17.z);
				transform41.localRotation = localRotation18;
				transform41 = dictBone[48];
				vector17 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform41.localRotation);
				vector17.y = (0f - y8) * 0.5f;
				localRotation18 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector17.x, vector17.y, vector17.z);
				transform41.localRotation = localRotation18;
				transform41 = dictBone[49];
				vector17 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform41.localRotation);
				vector17.y = (0f - y8) * 0.25f;
				localRotation18 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector17.x, vector17.y, vector17.z);
				transform41.localRotation = localRotation18;
			}
		}
		if (enableKneeL)
		{
			Transform transform42 = dictBone[10];
			if ((bool)transform42)
			{
				float x11 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform42.localRotation).x;
				Transform transform43 = dictBone[9];
				if ((bool)transform43)
				{
					Vector3 vector18 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform43.localRotation);
					vector18.x = (0f - x11) * 0.5f;
					vector18.z = x11 * 0.1f;
					Quaternion localRotation19 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector18.x, vector18.y, vector18.z);
					transform43.localRotation = localRotation19;
					transform43.SetLocalPositionY((0f - x11 * 0.048f + Mathf.Sin(x11 * 0.01f) * 1.1f) * 0.1f);
					transform43.SetLocalPositionZ((-0.6f - Mathf.Sin(x11 * 0.0115f) * 4f + Mathf.Tan(x11 * 0.0032f) * 5f) * 0.1f);
				}
				transform43 = dictBone[8];
				if ((bool)transform43)
				{
					Vector3 vector18 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform43.localRotation);
					vector18.x = (0f - x11) * 0.5f;
					Quaternion localRotation20 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector18.x, vector18.y, vector18.z);
					transform43.localRotation = localRotation20;
					transform43.SetLocalPositionY((Mathf.Sin(x11 * 0.012f) * 1.8f - Mathf.Tan(x11 * 0.00335f) * 4.5f) * 0.1f);
					transform43.SetLocalPositionZ((0.4f + x11 * 0.0125f - Mathf.Tan(x11 * 0.0032f) * 4.5f) * 0.1f);
				}
			}
		}
		if (enableKneeR)
		{
			Transform transform44 = dictBone[13];
			if ((bool)transform44)
			{
				float x12 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform44.localRotation).x;
				Transform transform45 = dictBone[12];
				if ((bool)transform45)
				{
					Vector3 vector19 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform45.localRotation);
					vector19.x = (0f - x12) * 0.5f;
					vector19.z = (0f - x12) * 0.1f;
					Quaternion localRotation21 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector19.x, vector19.y, vector19.z);
					transform45.localRotation = localRotation21;
					transform45.SetLocalPositionY((0f - x12 * 0.048f + Mathf.Sin(x12 * 0.01f) * 1.1f) * 0.1f);
					transform45.SetLocalPositionZ((-0.6f - Mathf.Sin(x12 * 0.0115f) * 4f + Mathf.Tan(x12 * 0.0032f) * 5f) * 0.1f);
				}
				transform45 = dictBone[11];
				if ((bool)transform45)
				{
					Vector3 vector19 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform45.localRotation);
					vector19.x = (0f - x12) * 0.5f;
					Quaternion localRotation22 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector19.x, vector19.y, vector19.z);
					transform45.localRotation = localRotation22;
					transform45.SetLocalPositionY((Mathf.Sin(x12 * 0.012f) * 1.8f - Mathf.Tan(x12 * 0.00335f) * 4.5f) * 0.1f);
					transform45.SetLocalPositionZ((0.4f + x12 * 0.0125f - Mathf.Tan(x12 * 0.0032f) * 4.5f) * 0.1f);
				}
			}
		}
		if (enableCalfL)
		{
			Transform transform46 = dictBone[15];
			if ((bool)transform46)
			{
				float y9 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform46.localRotation).y;
				Transform transform47 = dictBone[14];
				if ((bool)transform47)
				{
					Vector3 vector20 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform47.localRotation);
					vector20.y = y9 * 0.5f;
					Quaternion localRotation23 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector20.x, vector20.y, vector20.z);
					transform47.localRotation = localRotation23;
				}
			}
		}
		if (enableCalfR)
		{
			Transform transform48 = dictBone[17];
			if ((bool)transform48)
			{
				float y10 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform48.localRotation).y;
				Transform transform49 = dictBone[16];
				if ((bool)transform49)
				{
					Vector3 vector21 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform49.localRotation);
					vector21.y = y10 * 0.5f;
					Quaternion localRotation24 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, vector21.x, vector21.y, vector21.z);
					transform49.localRotation = localRotation24;
				}
			}
		}
		if (enableBust)
		{
			Transform transform50 = dictBone[55];
			Transform transform51 = dictBone[54];
			if ((bool)transform50 && (bool)transform51)
			{
				Vector3 vector22 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform50.localRotation);
				Quaternion localRotation25 = ConvertRotation.ConvertDegreeToQuaternion(ConvertRotation.RotationOrder.zyx, (0f - vector22.x) * 0.35f, (0f - vector22.y) * 0.35f, (0f - vector22.z) * 0.35f);
				transform51.localRotation = localRotation25;
				transform51.SetLocalPositionY((-0.081f + vector22.x * 0.003f) * 0.1f);
				transform51.SetLocalPositionZ(vector22.x * 0.004f * 0.1f);
			}
		}
		if (enableBustColDB)
		{
			Transform transform52 = dictBone[57];
			Transform transform53 = dictBone[56];
			if ((bool)transform52 && (bool)transform53)
			{
				transform53.SetLocalPosition((0f - transform52.localPosition.x) * 0.5f, (0f - transform52.localPosition.y) * 0.5f, (0f - transform52.localPosition.z) * 0.25f);
				transform53.SetLocalScale(1.5f - transform52.localScale.x * 0.5f, 1.5f - transform52.localScale.y * 0.5f, 1.5f - transform52.localScale.z * 0.5f);
			}
			transform52 = dictBone[59];
			transform53 = dictBone[58];
			if ((bool)transform52 && (bool)transform53)
			{
				transform53.SetLocalPosition((0f - transform52.localPosition.x) * 0.5f, (0f - transform52.localPosition.y) * 0.5f, (0f - transform52.localPosition.z) * 0.25f);
				transform53.SetLocalScale(1.5f - transform52.localScale.x * 0.5f, 1.5f - transform52.localScale.y * 0.5f, 1.5f - transform52.localScale.z * 0.5f);
			}
		}
		if (!enableAcsNull)
		{
			return;
		}
		Transform transform54 = dictBone[61];
		if ((bool)transform54)
		{
			Vector3 vector23 = ConvertRotation.ConvertDegreeFromQuaternion(ConvertRotation.RotationOrder.zyx, transform54.localRotation);
			Transform transform55 = dictBone[60];
			if ((bool)transform55)
			{
				transform55.SetLocalPositionX(vector23.z * 0.0002f);
				transform55.SetLocalPositionZ((-0.08f - vector23.x * 0.002f) * 0.1f);
			}
		}
	}
}
