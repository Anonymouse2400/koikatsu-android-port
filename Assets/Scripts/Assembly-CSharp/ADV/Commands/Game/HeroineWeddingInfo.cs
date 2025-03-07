using System.Linq;
using DG.Tweening;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ADV.Commands.Game
{
	public class HeroineWeddingInfo : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return null;
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		public override void Do()
		{
			base.Do();
			if (!Singleton<Manager.Game>.Instance.weddingData.personality.Any())
			{
				DisplayInformation(3f);
			}
			Singleton<Manager.Game>.Instance.weddingData.personality.Add(base.scenario.currentHeroine.personality);
		}

		private void DisplayInformation(float time)
		{
			ActionScene actScene = Singleton<Manager.Game>.Instance.actScene;
			if (actScene == null)
			{
				return;
			}
			time *= 0.5f;
			string assetBundleName = "action/information/20.unity3d";
			AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleName, "WeddingInformation", typeof(GameObject));
			AssetBundleManager.UnloadAssetBundle(assetBundleName, false);
			if (assetBundleLoadAssetOperation.IsEmpty())
			{
				return;
			}
			GameObject asset = assetBundleLoadAssetOperation.GetAsset<GameObject>();
			if (!(asset == null))
			{
				GameObject information = UnityEngine.Object.Instantiate(asset, actScene.transform, false);
				information.GetComponent<CanvasGroup>().DOFade(1f, time).SetEase(Ease.OutExpo)
					.SetLoops(2, LoopType.Yoyo)
					.OnComplete(delegate
					{
						UnityEngine.Object.Destroy(information);
					});
				Canvas canvas = information.GetComponent<Canvas>();
				canvas.enabled = true;
				information.UpdateAsObservable().Subscribe(delegate
				{
					canvas.sortingOrder = Singleton<Scene>.Instance.sceneFade.canvas.sortingOrder + 1;
				});
			}
		}
	}
}
