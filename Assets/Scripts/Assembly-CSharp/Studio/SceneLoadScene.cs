using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Illusion.Extensions;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class SceneLoadScene : MonoBehaviour
	{
		public static int page;

		[SerializeField]
		private ThumbnailNode[] buttonThumbnail;

		[SerializeField]
		private Button buttonClose;

		[SerializeField]
		private Canvas canvasWork;

		[SerializeField]
		private CanvasGroup canvasGroupWork;

		[SerializeField]
		private RawImage imageThumbnail;

		[SerializeField]
		private Button buttonLoad;

		[SerializeField]
		private Sprite spriteLoad;

		[SerializeField]
		private Button buttonImport;

		[SerializeField]
		private Sprite spriteImport;

		[SerializeField]
		private Button buttonCancel;

		[SerializeField]
		private Button buttonDelete;

		[SerializeField]
		private Sprite spriteDelete;

		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject prefabButton;

		private List<string> listPath;

		private int thumbnailNum = -1;

		private Dictionary<int, StudioNode> dicPage = new Dictionary<int, StudioNode>();

		private int pageNum = -1;

		private int select = -1;

		public void OnClickThumbnail(int _id)
		{
			canvasWork.enabled = true;
			canvasGroupWork.Enable(true);
			select = 12 * page + _id;
			imageThumbnail.texture = buttonThumbnail[_id].texture;
		}

		private void OnClickClose()
		{
			Singleton<Scene>.Instance.UnLoad();
		}

		private void OnClickPage(int _page)
		{
			SetPage(_page);
		}

		private void OnClickLoad()
		{
			canvasGroupWork.Enable(false);
			StartCoroutine(LoadScene(listPath[select]));
		}

		private IEnumerator LoadScene(string _path)
		{
			yield return Singleton<Studio>.Instance.LoadSceneCoroutine(_path);
			yield return null;
			canvasWork.enabled = false;
			NotificationScene.spriteMessage = spriteLoad;
			NotificationScene.waitTime = 1f;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioNotification",
				isAdd = true
			}, false);
		}

		private IEnumerator NotificationLoadCoroutine()
		{
			yield return null;
			NotificationScene.spriteMessage = spriteLoad;
			NotificationScene.waitTime = 1f;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioNotification",
				isAdd = true
			}, false);
		}

		private void OnClickImport()
		{
			Singleton<Studio>.Instance.ImportScene(listPath[select]);
			canvasWork.enabled = false;
			StartCoroutine("NotificationImportCoroutine");
		}

		private IEnumerator NotificationImportCoroutine()
		{
			yield return null;
			NotificationScene.spriteMessage = spriteImport;
			NotificationScene.waitTime = 1f;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioNotification",
				isAdd = true
			}, false);
		}

		private void OnClickCancel()
		{
			canvasWork.enabled = false;
		}

		private void OnClickDelete()
		{
			CheckScene.sprite = spriteDelete;
			CheckScene.unityActionYes = OnSelectDeleteYes;
			CheckScene.unityActionNo = OnSelectDeleteNo;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioCheck",
				isAdd = true
			}, false);
		}

		private void OnSelectDeleteYes()
		{
			Singleton<Scene>.Instance.UnLoad();
			File.Delete(listPath[select]);
			canvasWork.enabled = false;
			InitInfo();
			SetPage(page);
		}

		private void OnSelectDeleteNo()
		{
			Singleton<Scene>.Instance.UnLoad();
		}

		private void InitInfo()
		{
			for (int i = 0; i < transformRoot.childCount; i++)
			{
				UnityEngine.Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			List<KeyValuePair<DateTime, string>> list = (from s in Directory.GetFiles(UserData.Create("studio/scene"), "*.png")
				select new KeyValuePair<DateTime, string>(File.GetLastWriteTime(s), s)).ToList();
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
			list.Sort((KeyValuePair<DateTime, string> a, KeyValuePair<DateTime, string> b) => b.Key.CompareTo(a.Key));
			Thread.CurrentThread.CurrentCulture = currentCulture;
			listPath = list.Select((KeyValuePair<DateTime, string> v) => v.Value).ToList();
			thumbnailNum = listPath.Count;
			pageNum = thumbnailNum / 12 + ((thumbnailNum % 12 != 0) ? 1 : 0);
			dicPage.Clear();
			for (int j = 0; j < pageNum; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(prefabButton);
				gameObject.transform.SetParent(transformRoot, false);
				StudioNode component = gameObject.GetComponent<StudioNode>();
				component.active = true;
				int page = j;
				component.addOnClick = delegate
				{
					OnClickPage(page);
				};
				component.text = string.Format("{0}", j + 1);
				dicPage.Add(j, component);
			}
		}

		private void SetPage(int _page)
		{
			StudioNode value = null;
			if (dicPage.TryGetValue(page, out value))
			{
				value.select = false;
			}
			_page = Mathf.Clamp(_page, 0, pageNum - 1);
			int num = 12 * _page;
			for (int i = 0; i < 12; i++)
			{
				int num2 = num + i;
				if (!MathfEx.RangeEqualOn(0, num2, thumbnailNum - 1))
				{
					buttonThumbnail[i].interactable = false;
					continue;
				}
				buttonThumbnail[i].texture = PngAssist.LoadTexture(listPath[num2]);
				buttonThumbnail[i].interactable = true;
			}
			page = _page;
			if (dicPage.TryGetValue(page, out value))
			{
				value.select = true;
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		private void Awake()
		{
			InitInfo();
			SetPage(page);
			buttonClose.onClick.AddListener(OnClickClose);
			buttonLoad.onClick.AddListener(OnClickLoad);
			buttonImport.onClick.AddListener(OnClickImport);
			buttonCancel.onClick.AddListener(OnClickCancel);
			buttonDelete.onClick.AddListener(OnClickDelete);
			canvasWork.enabled = false;
		}
	}
}
