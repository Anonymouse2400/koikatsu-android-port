using System;
using System.Collections.Generic;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class HSeCtrl : MonoBehaviour
{
	[Serializable]
	private class KeyInfo
	{
		public int id;

		public float key;

		public bool isLoop;

		public int loopSwitch;

		public string pathAsset;

		public string nameSE;

		public bool isShorts;

		public string nameShortsSE;

		public int nFemale;

		public GameObject objParent;
	}

	[Serializable]
	private class Info
	{
		public delegate bool IsCheck<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);

		public string nameAnimation;

		public List<KeyInfo> key = new List<KeyInfo>();

		public List<KeyInfo> IsKey(float _old, float _now)
		{
			IsCheck<float, float, float>[] array = new IsCheck<float, float, float>[2] { IsKeyLoop, IsKeyNormal };
			List<KeyInfo> list = new List<KeyInfo>();
			int num = ((!(_old > _now)) ? 1 : 0);
			for (int i = 0; i < key.Count; i++)
			{
				if (array[num](_old, _now, key[i].key))
				{
					list.Add(key[i]);
				}
			}
			return list;
		}

		private bool IsKeyLoop(float _old, float _now, float _key)
		{
			return _old < _key || _now > _key;
		}

		private bool IsKeyNormal(float _old, float _now, float _key)
		{
			return _old <= _key && _key < _now;
		}
	}

	public HFlag flags;

	[SerializeField]
	private List<Info> lstInfo = new List<Info>();

	private Dictionary<string, Transform> dicLoop = new Dictionary<string, Transform>();

	private int oldnameHash;

	private float oldNormalizeTime;

	public bool Load(string _strAssetFolder, string _file, GameObject _objBoneMale, GameObject _objBoneFemale, GameObject _objMap)
	{
		lstInfo.Clear();
		dicLoop.Clear();
		oldnameHash = 0;
		oldNormalizeTime = 0f;
		Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.GameSE3D);
		if (_file == string.Empty)
		{
			return false;
		}
		GameObject[] array = new GameObject[3] { _objBoneMale, _objBoneFemale, _objMap };
		string text = GlobalMethod.LoadAllListText(_strAssetFolder, _file);
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int j = 0; j < length; j++)
		{
			int num = 0;
			int id = int.Parse(data[j, num++]);
			string nameAnim = data[j, num++];
			Info info = lstInfo.Find((Info i) => i.nameAnimation == nameAnim);
			if (info == null)
			{
				lstInfo.Add(new Info
				{
					nameAnimation = nameAnim
				});
				info = lstInfo[lstInfo.Count - 1];
			}
			KeyInfo key = info.key.Find((KeyInfo i) => i.id == id);
			if (key == null)
			{
				info.key.Add(new KeyInfo());
				key = info.key[info.key.Count - 1];
			}
			key.id = id;
			int num2 = int.Parse(data[j, num++]);
			string nameP = data[j, num++];
			array[num2].SafeProc(delegate(GameObject o)
			{
				key.objParent = o.transform.FindLoop(nameP);
			});
			key.key = float.Parse(data[j, num++]);
			key.isLoop = data[j, num++] == "1";
			key.loopSwitch = ((data[j, num++] == "1") ? 1 : 0);
			key.pathAsset = data[j, num++];
			key.nameSE = data[j, num++];
			key.isShorts = data[j, num++] == "1";
			key.nameShortsSE = data[j, num++];
			key.nFemale = int.Parse(data[j, num++]);
		}
		return true;
	}

	public bool Proc(AnimatorStateInfo _ai, ChaControl[] _females)
	{
		if (_females == null)
		{
			return false;
		}
		if (oldnameHash != _ai.shortNameHash)
		{
			oldNormalizeTime = 0f;
		}
		float now = _ai.normalizedTime % 1f;
		for (int i = 0; i < lstInfo.Count; i++)
		{
			if (!_ai.IsName(lstInfo[i].nameAnimation))
			{
				continue;
			}
			List<KeyInfo> list = lstInfo[i].IsKey(oldNormalizeTime, now);
			for (int j = 0; j < list.Count; j++)
			{
				string nameSE = list[j].nameSE;
				Utils.Sound.Setting setting;
				if (list[j].isLoop)
				{
					if (list[j].loopSwitch == 0)
					{
						if (dicLoop.ContainsKey(nameSE))
						{
							Singleton<Manager.Sound>.Instance.Stop(dicLoop[nameSE]);
							dicLoop.Remove(nameSE);
						}
					}
					else
					{
						if (dicLoop.ContainsKey(nameSE))
						{
							continue;
						}
						setting = new Utils.Sound.Setting();
						setting.type = Manager.Sound.Type.GameSE3D;
						setting.assetBundleName = list[j].pathAsset;
						setting.assetName = nameSE;
						Utils.Sound.Setting s = setting;
						Transform trans = Utils.Sound.Play(s);
						trans.SafeProcObject(delegate(Transform _)
						{
							_.GetComponent<AudioSource>().SafeProcObject(delegate(AudioSource a)
							{
								a.loop = true;
							});
						});
						Vector3 pos = ((!list[j].objParent) ? Vector3.zero : list[j].objParent.transform.position);
						Quaternion rot = ((!list[j].objParent) ? Quaternion.identity : list[j].objParent.transform.rotation);
						this.UpdateAsObservable().Subscribe(delegate
						{
							trans.SafeProcObject(delegate(Transform o)
							{
								o.SetPositionAndRotation(pos, rot);
							});
						}).AddTo(trans);
						dicLoop.Add(nameSE, trans);
					}
					continue;
				}
				setting = new Utils.Sound.Setting();
				setting.type = Manager.Sound.Type.GameSE3D;
				setting.assetBundleName = list[j].pathAsset;
				setting.assetName = nameSE;
				Utils.Sound.Setting setting2 = setting;
				if (list[j].isShorts && !_females[list[j].nFemale].IsKokanHide() && list[j].nameShortsSE != string.Empty)
				{
					setting2.assetName = list[j].nameShortsSE;
				}
				Transform trans2 = Utils.Sound.Play(setting2);
				Vector3 pos2 = ((!list[j].objParent) ? Vector3.zero : list[j].objParent.transform.position);
				Quaternion rot2 = ((!list[j].objParent) ? Quaternion.identity : list[j].objParent.transform.rotation);
				this.UpdateAsObservable().Subscribe(delegate
				{
					trans2.SafeProcObject(delegate(Transform o)
					{
						o.SetPositionAndRotation(pos2, rot2);
					});
				}).AddTo(trans2);
			}
			break;
		}
		oldNormalizeTime = now;
		oldnameHash = _ai.shortNameHash;
		return true;
	}

	public void InitOldMember(int _init = -1)
	{
		if (_init == -1 || _init == 0)
		{
			oldNormalizeTime = 0f;
		}
		if (_init == -1 || _init == 1)
		{
			oldnameHash = 0;
		}
	}
}
