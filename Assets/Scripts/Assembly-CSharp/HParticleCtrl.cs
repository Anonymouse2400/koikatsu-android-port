using System;
using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using Sirenix.OdinInspector;
using UnityEngine;

public class HParticleCtrl : SerializedMonoBehaviour
{
	[Serializable]
	public class ParticleInfo
	{
		public string assetPath;

		public string file;

		public int numParent;

		public string nameParent;

		public Vector3 pos;

		public Vector3 rot;

		public ParticleSystem particle;
	}

	[SerializeField]
	private Dictionary<int, ParticleInfo> dicParticle = new Dictionary<int, ParticleInfo>();

	private HFlag flags;

	public bool Init(string _strAssetFolderPath, GameObject _objFemaleBody, HFlag _flags)
	{
		flags = _flags;
		string text = GlobalMethod.LoadAllListText(_strAssetFolderPath, "h_particle");
		if (text == string.Empty)
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
			if (!dicParticle.ContainsKey(key))
			{
				dicParticle.Add(key, new ParticleInfo());
			}
			ParticleInfo particleInfo = dicParticle[key];
			particleInfo.assetPath = data[i, num++];
			particleInfo.file = data[i, num++];
			particleInfo.numParent = int.Parse(data[i, num++]);
			particleInfo.nameParent = data[i, num++];
			particleInfo.pos = new Vector3(float.Parse(data[i, num++]), float.Parse(data[i, num++]), float.Parse(data[i, num++]));
			particleInfo.rot = new Vector3(float.Parse(data[i, num++]), float.Parse(data[i, num++]), float.Parse(data[i, num++]));
		}
		Load(_objFemaleBody, 1);
		return true;
	}

	public bool Load(GameObject _objBody, int _sex)
	{
		if (_objBody == null)
		{
			return false;
		}
		IEnumerable<KeyValuePair<int, ParticleInfo>> enumerable = dicParticle.Where((KeyValuePair<int, ParticleInfo> p) => p.Value != null && p.Value.numParent == _sex && p.Value.particle == null);
		foreach (KeyValuePair<int, ParticleInfo> item in enumerable)
		{
			GameObject gameObject = _objBody.transform.FindLoop(item.Value.nameParent);
			if (!(gameObject == null))
			{
				GameObject gameObject2 = CommonLib.LoadAsset<GameObject>(item.Value.assetPath, item.Value.file, true, string.Empty);
				flags.hashAssetBundle.Add(item.Value.assetPath);
				if (!(gameObject2 == null))
				{
					item.Value.particle = gameObject2.GetComponent<ParticleSystem>();
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localPosition = item.Value.pos;
					gameObject2.transform.localRotation = Quaternion.Euler(item.Value.rot);
					gameObject2.transform.localScale = Vector3.one;
				}
			}
		}
		return true;
	}

	public bool ReleaseObject(int _sex)
	{
		IEnumerable<KeyValuePair<int, ParticleInfo>> enumerable = dicParticle.Where((KeyValuePair<int, ParticleInfo> p) => p.Value != null && p.Value.numParent == _sex && (bool)p.Value.particle);
		foreach (KeyValuePair<int, ParticleInfo> item in enumerable)
		{
			UnityEngine.Object.Destroy(item.Value.particle);
			item.Value.particle = null;
		}
		return true;
	}

	public bool Play(int _particle)
	{
		if (!dicParticle.ContainsKey(_particle))
		{
			return false;
		}
		if (dicParticle[_particle].particle == null)
		{
			return false;
		}
		dicParticle[_particle].particle.Simulate(0f);
		dicParticle[_particle].particle.Play();
		return true;
	}

	public bool IsPlaying(int _particle)
	{
		if (!dicParticle.ContainsKey(_particle))
		{
			return false;
		}
		if (dicParticle[_particle].particle == null)
		{
			return false;
		}
		return dicParticle[_particle].particle.isPlaying;
	}
}
