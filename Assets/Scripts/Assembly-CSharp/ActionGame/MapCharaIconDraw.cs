using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame.Chara;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class MapCharaIconDraw : MonoBehaviour
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private Button _button;

		private ActionScene _actScene;

		public Canvas canvas
		{
			get
			{
				return _canvas;
			}
		}

		public Button button
		{
			get
			{
				return _button;
			}
		}

		private ActionScene actScene
		{
			get
			{
				if (_actScene == null && Singleton<Game>.IsInstance())
				{
					_actScene = Singleton<Game>.Instance.actScene;
				}
				return _actScene;
			}
		}

		private void CreateCharaIcon()
		{
			float canvasWidth = canvas.pixelRect.width;
			float initX = 10f;
			Rect set = new Rect(initX, 10f, 10f, 10f);
			bool isInit = false;
			Action<Image, SaveData.CharaData> iconSet = delegate(Image image, SaveData.CharaData data)
			{
				image.GetComponentInChildren<Text>().text = data.Name;
				if (isInit)
				{
					set.x += set.width + image.rectTransform.rect.width;
					if (set.x >= canvasWidth)
					{
						set.x = initX;
						set.y += set.height + image.rectTransform.rect.height;
					}
				}
				isInit = true;
				image.rectTransform.anchoredPosition = new Vector2(set.x, set.y);
			};
			List<NPC> list = actScene.npcList.Where((NPC p) => actScene.Map.no == p.mapNo).ToList();
			list.ForEach(delegate(NPC item)
			{
				SaveData.Heroine heroine = item.heroine;
				Button copy = UnityEngine.Object.Instantiate(button, canvas.transform, false);
				copy.gameObject.SetActive(true);
				copy.interactable = item.initialized;
				if (!copy.interactable)
				{
					copy.UpdateAsObservable().SkipWhile((Unit _) => !item.initialized).Take(1)
						.Subscribe(delegate
						{
							copy.interactable = true;
						});
				}
				copy.OnClickAsObservable().Subscribe(delegate
				{
					actScene.SceneEvent(item);
				});
				iconSet(copy.image, heroine);
			});
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			canvas.enabled = false;
			yield return new WaitUntil(Singleton<Scene>.IsInstance);
			if (actScene == null || actScene.Map.Info.MapName == "後門")
			{
				yield break;
			}
			CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
			if (canvasGroup != null)
			{
				this.ObserveEveryValueChanged((MapCharaIconDraw _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
				{
					canvasGroup.interactable = isOn;
				});
			}
			base.enabled = true;
		}

		private void Update()
		{
			bool flag = canvas.enabled;
			canvas.enabled = !Program.isADVProcessing && !actScene.isEventNow;
			if (flag == canvas.enabled || !canvas.enabled)
			{
				return;
			}
			List<Button> list = new List<Button>();
			foreach (Transform item in canvas.transform)
			{
				Button component = item.GetComponent<Button>();
				if (!(button == component) && component != null)
				{
					list.Add(component);
				}
			}
			list.ForEach(delegate(Button p)
			{
				UnityEngine.Object.Destroy(p.gameObject);
			});
			CreateCharaIcon();
		}
	}
}
