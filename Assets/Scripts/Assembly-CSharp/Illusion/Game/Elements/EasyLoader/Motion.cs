using System;
using UnityEngine;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public class Motion : BaseMotion
	{
		public bool isCrossFade;

		public float transitionDuration;

		public float normalizedTime;

		public int[] layers;

		public Motion()
		{
		}

		public Motion(string bundle, string asset)
			: base(bundle, asset)
		{
		}

		public Motion(string bundle, string asset, string state)
			: base(bundle, asset, state)
		{
		}

		public virtual bool Setting(Animator animator)
		{
			return Setting(animator, bundle, asset, state, false);
		}

		public virtual bool Setting(Animator animator, string bundle, string asset, string state, bool isCheck)
		{
			if (isCheck && !Check(bundle, asset, state))
			{
				return false;
			}
			bool result = false;
			if (!asset.IsNullOrEmpty())
			{
				base.asset = asset;
				if (!bundle.IsNullOrEmpty())
				{
					base.bundle = bundle;
				}
				LoadAnimator(animator);
				result = true;
			}
			if (!state.IsNullOrEmpty())
			{
				base.state = state;
				result = true;
			}
			return result;
		}

		public void LoadAnimator(Animator animator, string bundle, string asset)
		{
			new Motion(bundle, asset).LoadAnimator(animator);
		}

		public void LoadAnimator(Animator animator)
		{
			if (!((AssetBundleData)this).isEmpty)
			{
				animator.runtimeAnimatorController = GetAsset<RuntimeAnimatorController>();
				UnloadBundle();
			}
		}

		public void Play(Animator animator)
		{
			if (!animator.gameObject.activeInHierarchy)
			{
				return;
			}
			int[] array = ((!layers.IsNullOrEmpty()) ? layers : new int[1]);
			if (!isCrossFade)
			{
				int[] array2 = array;
				foreach (int layer in array2)
				{
					animator.Play(state, layer, normalizedTime);
				}
			}
			else
			{
				int[] array3 = array;
				foreach (int layer2 in array3)
				{
					animator.CrossFade(state, transitionDuration, layer2, normalizedTime);
				}
			}
		}
	}
}
