using System;
using System.Collections.Generic;
using H;
using UnityEngine;

public class PeepingFrameCtrl
{
	[Serializable]
	public class Timing
	{
		public string anim = string.Empty;

		public float key;

		public List<int> lstDo = new List<int>();
	}

	private Dictionary<int, Timing> dicInfo = new Dictionary<int, Timing>();

	private float oldFrame;

	private int oldHash;

	private ChaControl female;

	private HSprite sprite;

	private Toilet toilet;

	private HParticleCtrl particle;

	public bool Init(ChaControl _female, HSprite _sprite, HParticleCtrl _particle)
	{
		Release();
		female = _female;
		sprite = _sprite;
		particle = _particle;
		return true;
	}

	public bool SetMapObj(GameObject _objMap)
	{
		if ((bool)_objMap)
		{
			toilet = _objMap.GetComponentInChildren<Toilet>();
		}
		return true;
	}

	public void Release()
	{
		dicInfo.Clear();
	}

	public bool Load(string _assetpath, int _ctrl)
	{
		dicInfo.Clear();
		string text = GlobalMethod.LoadAllListText(_assetpath, "PeepingFrameCtrl_" + _ctrl.ToString("00"));
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
			int key = int.Parse(data[i, num++]);
			if (!dicInfo.ContainsKey(key))
			{
				dicInfo.Add(key, new Timing());
			}
			Timing timing = dicInfo[key];
			timing.anim = data[i, num++];
			timing.key = float.Parse(data[i, num++]);
			timing.lstDo.Clear();
			string text2 = data[i, num++];
			string[] array = text2.Split(',');
			for (int j = 0; j < array.Length; j++)
			{
				timing.lstDo.Add(int.Parse(array[j]));
			}
		}
		oldFrame = 0f;
		oldHash = 0;
		return true;
	}

	public bool Proc(AnimatorStateInfo _ai)
	{
		if (oldHash != _ai.shortNameHash)
		{
			oldFrame = 0f;
		}
		oldHash = _ai.shortNameHash;
		float num = _ai.normalizedTime % 1f;
		foreach (KeyValuePair<int, Timing> item in dicInfo)
		{
			if (_ai.IsName(item.Value.anim) && !(oldFrame > item.Value.key) && !(item.Value.key >= num))
			{
				for (int i = 0; i < item.Value.lstDo.Count; i++)
				{
					Do(item.Value.lstDo[i]);
				}
			}
		}
		oldFrame = num;
		return true;
	}

	private bool Do(int _ctrl)
	{
		switch (_ctrl)
		{
		case 0:
			GlobalMethod.SetAllClothState(female, true, 2, true);
			break;
		case 1:
			GlobalMethod.SetAllClothState(female, true, 0, true);
			break;
		case 2:
			GlobalMethod.SetAllClothState(female, false, 2, true);
			break;
		case 3:
			GlobalMethod.SetAllClothState(female, false, 0, true);
			break;
		case 4:
			particle.Play(1);
			break;
		case 5:
			if ((bool)toilet)
			{
				toilet.ToiletStart();
			}
			break;
		case 6:
			sprite.FadeState(HSprite.FadeKind.Out, 1f);
			break;
		}
		return true;
	}
}
