using UniRx;
using UnityEngine;

namespace Studio
{
	public class HandAnimeCtrl : MonoBehaviour
	{
		public enum HandKind
		{
			Left = 0,
			Right = 1
		}

		[SerializeField]
		private HandKind hand;

		[SerializeField]
		private Animator animator;

		private IntReactiveProperty _ptn = new IntReactiveProperty(-1);

		private string bundlePath = string.Empty;

		private string fileName = string.Empty;

		public int sex { get; private set; }

		public int ptn
		{
			get
			{
				return _ptn.Value;
			}
			set
			{
				_ptn.Value = value;
			}
		}

		public int max { get; private set; }

		public bool isInit { get; private set; }

		public void Init(int _sex)
		{
			if (!isInit)
			{
				max = Singleton<Info>.Instance.dicHandAnime[(int)hand].Count;
				_ptn.Subscribe(delegate
				{
					LoadAnime();
				});
				sex = _sex;
				ptn = 0;
				isInit = true;
			}
		}

		private void LoadAnime()
		{
			Info.HandAnimeInfo value = null;
			if (!Singleton<Info>.Instance.dicHandAnime[(int)hand].TryGetValue(ptn, out value))
			{
				base.enabled = false;
				return;
			}
			if (bundlePath != value.bundlePath || fileName != value.fileName)
			{
				RuntimeAnimatorController runtimeAnimatorController = CommonLib.LoadAsset<RuntimeAnimatorController>(value.bundlePath, value.fileName, false, string.Empty);
				if (runtimeAnimatorController != null)
				{
					bundlePath = value.bundlePath;
					fileName = value.fileName;
					animator.runtimeAnimatorController = runtimeAnimatorController;
				}
				AssetBundleManager.UnloadAssetBundle(value.bundlePath, false);
			}
			base.enabled = true;
			animator.Play(value.clip);
		}

		private void OnEnable()
		{
			animator.enabled = true;
		}

		private void OnDisable()
		{
			animator.enabled = false;
		}
	}
}
