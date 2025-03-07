using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MetaballShoot
{
	public class MetaInfo
	{
		public MetaballInfo objMeta;

		public float timeLerpDrag;

		public float timeLerpDragRand;

		public float nowDrag;
	}

	public string comment = string.Empty;

	public bool isEnable = true;

	[Tooltip("発射軸")]
	public GameObject ShootAxis;

	[Tooltip("生成する弾")]
	public GameObject ShootObj;

	[Tooltip("コンドーム")]
	public GameObject objCondom;

	[Tooltip("SourceRoot")]
	public GameObject objSourceRoot;

	[Tooltip("止まった時に親子付する場所")]
	public Transform parentTransform;

	[Header("PARAM")]
	public float drag = 1f;

	public float dragDropDown = 1f;

	public int maxMetaball = 30;

	public float timeDropDown = 1f;

	public float speedSMin = 1f;

	public float speedSMax = 1f;

	public float speedMMin = 1f;

	public float speedMMax = 1f;

	public float speedLMin = 1f;

	public float speedLMax = 1f;

	public float[] aInterval;

	public Vector2 randomRotationS = Vector2.zero;

	public Vector2 randomRotationM = Vector2.zero;

	public Vector2 randomRotationL = Vector2.zero;

	public ForceMode shootForce;

	public ChaControl chaCustom;

	private const float timeConstMax = 9999999f;

	private List<MetaInfo> lstMeta = new List<MetaInfo>();

	private float timeInterval = 9999999f;

	private int numInterval;

	private bool isCondom;

	public bool isConstMetaMesh { get; private set; }

	public bool IsCreate()
	{
		for (int i = 0; i < lstMeta.Count; i++)
		{
			for (int j = 0; j < lstMeta[i].objMeta.aRigid.Length; j++)
			{
				if (!lstMeta[i].objMeta.aRigid[j].isKinematic && !lstMeta[i].objMeta.aRigid[j].IsSleeping())
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OnValidate()
	{
	}

	public bool ShootMetaBallStart(bool _isCondom)
	{
		numInterval = -1;
		timeInterval = 0f;
		if ((bool)parentTransform)
		{
			isConstMetaMesh = true;
		}
		isCondom = _isCondom;
		return true;
	}

	public void MetaBallClear()
	{
		for (int i = 0; i < lstMeta.Count; i++)
		{
			UnityEngine.Object.Destroy(lstMeta[i].objMeta.gameObject);
		}
		lstMeta.Clear();
		if ((bool)objSourceRoot)
		{
			for (int j = 0; j < objSourceRoot.transform.childCount; j++)
			{
				UnityEngine.Object.Destroy(objSourceRoot.transform.GetChild(j).gameObject);
			}
		}
		isConstMetaMesh = false;
	}

	public bool ShootMetaBall()
	{
		if (timeInterval == 9999999f)
		{
			return false;
		}
		if (numInterval != -1)
		{
			timeInterval += Time.deltaTime;
			if (timeInterval < aInterval[numInterval])
			{
				return false;
			}
		}
		MetaBallInstantiate();
		numInterval++;
		timeInterval = 0f;
		if (aInterval.Length <= numInterval)
		{
			timeInterval = 9999999f;
		}
		return true;
	}

	private bool MetaBallInstantiate()
	{
		if (!ShootAxis || !ShootObj || !objSourceRoot)
		{
			return false;
		}
		if (isCondom && objCondom == null)
		{
			return false;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(isCondom ? objCondom : ShootObj);
		if (!gameObject)
		{
			return false;
		}
		gameObject.name = (isCondom ? objCondom.name : ShootObj.name);
		gameObject.transform.position = ShootAxis.transform.position;
		gameObject.transform.rotation = ShootAxis.transform.rotation;
		gameObject.transform.SetParent(objSourceRoot.transform);
		if (!isCondom)
		{
			MetaballInfo component = gameObject.GetComponent<MetaballInfo>();
			if (!component)
			{
				UnityEngine.Object.Destroy(gameObject);
				return false;
			}
			SetRigidForce(component.rigidBeginning);
			MetaInfo metaInfo = new MetaInfo();
			metaInfo.objMeta = component;
			metaInfo.nowDrag = drag;
			metaInfo.timeLerpDragRand = timeDropDown;
			lstMeta.Add(metaInfo);
			if (lstMeta.Count > maxMetaball)
			{
				UnityEngine.Object.Destroy(lstMeta[0].objMeta.gameObject);
				lstMeta.RemoveAt(0);
			}
		}
		else
		{
			SetRigidForce(gameObject.GetComponent<Rigidbody>());
			isCondom = false;
		}
		return true;
	}

	private bool SetRigidForce(Rigidbody _rigid)
	{
		if (!_rigid)
		{
			return false;
		}
		_rigid.drag = drag;
		float num = 0.5f;
		if (chaCustom != null)
		{
			num = chaCustom.GetShapeBodyValue(0);
		}
		Vector3 vector = _rigid.transform.InverseTransformDirection(ShootAxis.transform.forward);
		float t = ((!(num >= 0.5f)) ? Mathf.InverseLerp(0f, 0.5f, num) : Mathf.InverseLerp(0.5f, 1f, num));
		Vector2 vector2 = ((!(num >= 0.5f)) ? Vector2.Lerp(randomRotationS, randomRotationM, t) : Vector2.Lerp(randomRotationM, randomRotationL, t));
		Quaternion quaternion = Quaternion.Euler(UnityEngine.Random.Range(0f - vector2.x, vector2.x), UnityEngine.Random.Range(0f - vector2.y, vector2.y), 0f);
		vector = quaternion * vector;
		float min = ((!(num >= 0.5f)) ? Mathf.Lerp(speedSMin, speedMMin, t) : Mathf.Lerp(speedMMin, speedLMin, t));
		float max = ((!(num >= 0.5f)) ? Mathf.Lerp(speedSMax, speedMMax, t) : Mathf.Lerp(speedMMax, speedLMax, t));
		_rigid.AddRelativeForce(vector * UnityEngine.Random.Range(min, max), shootForce);
		return true;
	}
}
