using System.Collections.Generic;
using IllusionUtility.GetUtility;
using UnityEngine;

namespace Studio
{
	public class OptionItemCtrl : MonoBehaviour
	{
		private class ChildInfo
		{
			public Vector3 scale = Vector3.one;

			public GameObject obj;

			public ChildInfo(Vector3 _scale, GameObject _obj)
			{
				scale = _scale;
				obj = _obj;
			}
		}

		private class ItemInfo
		{
			public GameObject gameObject;

			public Animator animator;

			public List<ChildInfo> child = new List<ChildInfo>();

			private Renderer[] renderer;

			public float m_Height = 0.5f;

			private bool m_Active = true;

			public Vector3 scale { get; set; }

			public bool isSync { get; set; }

			public float height
			{
				get
				{
					return m_Height;
				}
				set
				{
					m_Height = value;
				}
			}

			public bool active
			{
				get
				{
					return m_Active;
				}
				set
				{
					if (m_Active != value)
					{
						m_Active = value;
						for (int i = 0; i < renderer.Length; i++)
						{
							renderer[i].enabled = value;
						}
					}
				}
			}

			public ItemInfo(float _height)
			{
				m_Height = _height;
			}

			public void Release()
			{
				Object.DestroyImmediate(gameObject);
				for (int i = 0; i < child.Count; i++)
				{
					Object.DestroyImmediate(child[i].obj);
				}
			}

			public void SetRender()
			{
				List<Renderer> list = new List<Renderer>();
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				if (!componentsInChildren.IsNullOrEmpty())
				{
					list.AddRange(componentsInChildren);
				}
				for (int i = 0; i < child.Count; i++)
				{
					componentsInChildren = child[i].obj.GetComponentsInChildren<Renderer>();
					if (!componentsInChildren.IsNullOrEmpty())
					{
						list.AddRange(componentsInChildren);
					}
				}
				renderer = list.ToArray();
			}
		}

		private Animator m_Animator;

		private HashSet<ItemInfo> hashItem = new HashSet<ItemInfo>();

		private bool m_OutsideVisible = true;

		public Animator animator
		{
			get
			{
				if (m_Animator == null)
				{
					m_Animator = base.gameObject.GetComponentInChildren<Animator>();
				}
				return m_Animator;
			}
			set
			{
				m_Animator = value;
			}
		}

		public OICharInfo oiCharInfo { get; set; }

		public bool visible
		{
			get
			{
				return oiCharInfo.animeOptionVisible;
			}
			set
			{
				oiCharInfo.animeOptionVisible = value;
				SetVisible(m_OutsideVisible & oiCharInfo.animeOptionVisible);
			}
		}

		public bool outsideVisible
		{
			get
			{
				return m_OutsideVisible;
			}
			set
			{
				m_OutsideVisible = value;
				SetVisible(m_OutsideVisible & oiCharInfo.animeOptionVisible);
			}
		}

		public float height
		{
			set
			{
				foreach (ItemInfo item in hashItem)
				{
					item.height = value;
				}
			}
		}

		public void LoadAnimeItem(Info.AnimeLoadInfo _info, string _clip, float _height, float _motion)
		{
			ReleaseAllItem();
			if (_info.option.IsNullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < _info.option.Count; i++)
			{
				Info.OptionItemInfo optionItemInfo = _info.option[i];
				GameObject gameObject = Utility.LoadAsset<GameObject>(optionItemInfo.bundlePath, optionItemInfo.fileName, optionItemInfo.manifest);
				if (gameObject == null)
				{
					continue;
				}
				ItemInfo itemInfo = new ItemInfo(_height);
				itemInfo.gameObject = gameObject;
				itemInfo.scale = gameObject.transform.localScale;
				itemInfo.animator = gameObject.GetComponent<Animator>();
				if ((bool)itemInfo.animator)
				{
					if (optionItemInfo.anmInfo.check)
					{
						RuntimeAnimatorController runtimeAnimatorController = CommonLib.LoadAsset<RuntimeAnimatorController>(optionItemInfo.anmInfo.bundlePath, optionItemInfo.anmInfo.fileName, false, string.Empty);
						if (runtimeAnimatorController != null)
						{
							itemInfo.animator.runtimeAnimatorController = runtimeAnimatorController;
						}
						AssetBundleManager.UnloadAssetBundle(optionItemInfo.bundlePath, false);
						itemInfo.animator.Play(_clip);
					}
					itemInfo.animator.SetFloat("height", _height);
					itemInfo.isSync = true;
				}
				else
				{
					itemInfo.isSync = false;
				}
				if (optionItemInfo.parentageInfo.IsNullOrEmpty())
				{
					GameObject gameObject2 = base.gameObject;
					GameObject gameObject3 = gameObject;
					gameObject3.transform.SetParent(gameObject2.transform);
					gameObject3.transform.localPosition = Vector3.zero;
					gameObject3.transform.localRotation = Quaternion.identity;
					gameObject3.transform.localScale = itemInfo.scale;
				}
				else
				{
					for (int j = 0; j < optionItemInfo.parentageInfo.Length; j++)
					{
						GameObject gameObject4 = base.gameObject.transform.FindLoop(optionItemInfo.parentageInfo[j].parent);
						GameObject gameObject5 = gameObject;
						if (!optionItemInfo.parentageInfo[j].child.IsNullOrEmpty())
						{
							gameObject5 = gameObject5.transform.FindLoop(optionItemInfo.parentageInfo[j].child);
							itemInfo.child.Add(new ChildInfo(gameObject5.transform.localScale, gameObject5));
						}
						gameObject5.transform.SetParent(gameObject4.transform);
						gameObject5.transform.localPosition = Vector3.zero;
						gameObject5.transform.localRotation = Quaternion.identity;
						gameObject5.transform.localScale = itemInfo.scale;
					}
				}
				itemInfo.SetRender();
				hashItem.Add(itemInfo);
			}
			SetVisible(visible);
		}

		public void ReleaseAllItem()
		{
			foreach (ItemInfo item in hashItem)
			{
				if (item != null)
				{
					item.Release();
				}
			}
			hashItem.Clear();
		}

		public void SetMotion(float _motion)
		{
			foreach (ItemInfo item in hashItem)
			{
				if ((bool)item.animator && item.isSync)
				{
					item.animator.SetFloat("motion", _motion);
				}
			}
		}

		public void ChangeScale(Vector3 _value)
		{
			foreach (ItemInfo item in hashItem)
			{
				Transform transform = item.gameObject.transform;
				transform.localScale = item.scale;
			}
		}

		private void SetVisible(bool _visible)
		{
			foreach (ItemInfo item in hashItem)
			{
				item.active = _visible;
			}
		}

		private void Awake()
		{
			m_OutsideVisible = true;
		}

		private void LateUpdate()
		{
			if (animator == null || hashItem.Count == 0)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			foreach (ItemInfo item in hashItem)
			{
				if ((bool)item.animator && item.isSync)
				{
					item.animator.Play(currentAnimatorStateInfo.fullPathHash, 0, currentAnimatorStateInfo.normalizedTime);
				}
			}
		}
	}
}
