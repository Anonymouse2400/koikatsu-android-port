using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Illusion.Game.Elements.EasyLoader;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ADV.EventCG
{
	public class Data : MonoBehaviour
	{
		[Serializable]
		public class Scene
		{
			[Serializable]
			public class Chara
			{
				[Serializable]
				public class MotionAndItem
				{
					[Serializable]
					public class ItemSet
					{
						public string name;

						public Item.Data data = new Item.Data();

						public void Execute(ChaControl chaCtrl, List<GameObject> itemList)
						{
							GameObject gameObject = data.Load(chaCtrl);
							gameObject.name = name;
							itemList.Add(gameObject);
						}
					}

					[Serializable]
					public class ItemRemove
					{
						public string name;

						public bool Execute(ChaControl chaCtrl, List<GameObject> itemList)
						{
							int index = itemList.FindIndex((GameObject p) => p.name == name);
							GameObject gameObject = itemList.SafeGet(index);
							if (gameObject == null)
							{
								return false;
							}
							itemList.RemoveAt(index);
							UnityEngine.Object.Destroy(gameObject);
							return true;
						}
					}

					public Illusion.Game.Elements.EasyLoader.Motion motion = new Illusion.Game.Elements.EasyLoader.Motion();

					public IKMotion ik = new IKMotion();

					public YureMotion yure = new YureMotion();

					public ItemSet[] items;

					public ItemRemove[] removes;

					public void Change(ChaControl chaCtrl, List<GameObject> itemList)
					{
						Animator animBody = chaCtrl.animBody;
						if (motion.Setting(animBody))
						{
							motion.Play(animBody);
							ik.Setting(chaCtrl, motion.state);
							yure.Setting(chaCtrl, motion.state);
						}
						ItemSet[] array = items;
						foreach (ItemSet itemSet in array)
						{
							itemSet.Execute(chaCtrl, itemList);
						}
						ItemRemove[] array2 = removes;
						foreach (ItemRemove itemRemove in array2)
						{
							itemRemove.Execute(chaCtrl, itemList);
						}
					}
				}

				public int no;

				public MotionAndItem motionAndItem = new MotionAndItem();

				public void Change(ChaControl chaCtrl, List<GameObject> itemList)
				{
					motionAndItem.Change(chaCtrl, itemList);
				}
			}

			public Chara[] charas;

			public Chara this[int index]
			{
				get
				{
					return charas.SafeGet(index);
				}
			}

			public Chara FindGet(int no)
			{
				Chara[] array = charas;
				foreach (Chara chara in array)
				{
					if (chara.no == no)
					{
						return chara;
					}
				}
				return null;
			}
		}

		public const string ParentNameCamera = "camPos";

		public const string ParentNameChara = "chaPos";

		public const string ParentNamePlayer = "playerPos";

		private Transform _camRoot;

		public string mapName;

		public Scene[] scenes;

		private Transform _chaRoot;

		private Camera _targetCamera;

		private Dictionary<Transform, Tuple<Vector3, Quaternion>> backupDic = new Dictionary<Transform, Tuple<Vector3, Quaternion>>();

		public Transform camRoot
		{
			get
			{
				return _camRoot;
			}
			set
			{
				if (base.transform.childCount == 0)
				{
					return;
				}
				Transform child = base.transform.GetChild(0);
				if (!(child.name != "camPos"))
				{
					_camRoot = value;
					backupDic[_camRoot] = Tuple.Create(_camRoot.position, _camRoot.rotation);
					CameraData camData = child.GetComponent<CameraData>();
					Camera component = _camRoot.GetComponent<Camera>();
					camData.SetCameraData(component);
					component.fieldOfView = camData.fieldOfView;
					_camRoot.SetPositionAndRotation(child.position, child.rotation);
					(from _ in child.UpdateAsObservable().TakeUntilDestroy(_camRoot)
						where camData.initialized
						select _).Take(1).Subscribe(delegate
					{
						_camRoot.SetPositionAndRotation(child.position, child.rotation);
					});
				}
			}
		}

		public Dictionary<int, List<GameObject>> itemList { get; private set; }

		public Transform chaRoot
		{
			get
			{
				return _chaRoot;
			}
		}

		public Camera targetCamera
		{
			get
			{
				return this.GetCache(ref _targetCamera, base.GetComponentInChildren<Camera>);
			}
		}

		public List<Transform> GetCharaPosChildren
		{
			get
			{
				return base.transform.Children().Where(IsCharaPos).ToList();
			}
		}

		public static bool IsCharaPos(UnityEngine.Object child)
		{
			return child.name.IndexOf("chaPos") == 0;
		}

		public void SetChaRoot(Transform root, Dictionary<int, CharaData> charaDataDic)
		{
			_chaRoot = root;
			itemList = new Dictionary<int, List<GameObject>>();
			List<Transform> list = base.transform.Children();
			Dictionary<int, Transform> dictionary = list.Where(IsCharaPos).Select((Transform t, int i) => new { t, i }).ToDictionary(v => v.i, v => v.t);
			Transform transform = list.Find((Transform p) => p.name == "playerPos");
			int num = -1;
			if (transform != null)
			{
				dictionary[num] = transform;
			}
			foreach (KeyValuePair<int, CharaData> item in charaDataDic)
			{
				int key = ((!(item.Value.data is SaveData.Heroine)) ? num : item.Key);
				Transform value;
				if (dictionary.TryGetValue(key, out value))
				{
					Transform transform2 = item.Value.transform;
					backupDic[transform2] = Tuple.Create(transform2.position, transform2.rotation);
					transform2.SetPositionAndRotation(value.position, value.rotation);
					itemList.Add(key, new List<GameObject>());
				}
			}
		}

		public void Restore()
		{
			foreach (KeyValuePair<Transform, Tuple<Vector3, Quaternion>> item in backupDic)
			{
				if (!(item.Key == null))
				{
					if (item.Key == _camRoot)
					{
						Camera component = _camRoot.GetComponent<Camera>();
						base.transform.GetChild(0).GetComponent<CameraData>().RepairCameraData(component);
					}
					item.Key.SetPositionAndRotation(item.Value.Item1, item.Value.Item2);
				}
			}
		}

		public void ItemClear()
		{
			foreach (List<GameObject> value in itemList.Values)
			{
				value.ForEach(delegate(GameObject item)
				{
					UnityEngine.Object.Destroy(item);
				});
				value.Clear();
			}
		}

		public void Next(int index, Dictionary<int, CharaData> charaDataDic)
		{
			Scene scene = scenes.SafeGet(index);
			if (scene == null)
			{
				return;
			}
			Scene scene2 = scenes.SafeGet(index - 1);
			foreach (KeyValuePair<int, CharaData> item in charaDataDic)
			{
				Scene.Chara chara = scene.FindGet(item.Key);
				if (chara == null)
				{
					continue;
				}
				Scene.Chara chara2 = null;
				MotionIK motionIK = null;
				YureCtrl yureCtrl = null;
				bool flag = false;
				Scene.Chara.MotionAndItem motionAndItem = chara.motionAndItem;
				if (motionAndItem.ik.bundle.IsNullOrEmpty() && scene2 != null)
				{
					chara2 = scene2.FindGet(item.Key);
					flag = true;
					if (chara2 != null)
					{
						motionIK = chara2.motionAndItem.ik.motionIK;
					}
				}
				if (motionAndItem.yure.bundle.IsNullOrEmpty())
				{
					if (!flag && scene2 != null)
					{
						chara2 = scene2.FindGet(item.Key);
						flag = true;
					}
					if (chara2 != null)
					{
						yureCtrl = chara2.motionAndItem.yure.yureCtrl;
					}
				}
				motionAndItem.ik.Create(item.Value.chaCtrl, motionIK);
				motionAndItem.yure.Create(item.Value.chaCtrl, yureCtrl);
			}
			foreach (KeyValuePair<int, CharaData> item2 in charaDataDic)
			{
				Scene.Chara chara3 = scene.FindGet(item2.Key);
				if (chara3 != null)
				{
					chara3.motionAndItem.ik.motionIK.SetPartners(scene.charas.Select((Scene.Chara p) => p.motionAndItem.ik.motionIK));
					chara3.Change(item2.Value.chaCtrl, itemList[item2.Key]);
				}
			}
		}
	}
}
