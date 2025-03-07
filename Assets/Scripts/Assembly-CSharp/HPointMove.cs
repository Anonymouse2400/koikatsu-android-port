using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame;
using H;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HPointMove : BaseLoader
{
	public GameObject objCategoryParent;

	public GameObject objToggleNode;

	private ActionMap map;

	private Action<HPointData, int> actionSelect;

	private Action actionBack;

	private CameraControl_Ver2 cam;

	private HPointData selectData;

	private GameObject objMapNull;

	private GameObject objBasePoint;

	private List<int> lstCategory;

	private Vector3 initPos = Vector3.zero;

	private Quaternion initRot = Quaternion.identity;

	private bool isFreeH;

	private bool isDebug;

	private HFlag flags;

	private SaveData.Heroine.HExperienceKind status;

	private List<HSceneProc.AnimationListInfo>[] lstAnimInfo = new List<HSceneProc.AnimationListInfo>[8];

	private int nowCategory = -1;

	private HashSet<int> hashOmitCategory = new HashSet<int>();

	private Dictionary<int, List<GameObject>> dicObj = new Dictionary<int, List<GameObject>>();

	private Dictionary<int, Toggle> dicToggle = new Dictionary<int, Toggle>();

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.H_POINT_UI));
		}
	}

	private IEnumerator Start()
	{
		int idMap = -1;
		if (Singleton<Manager.Scene>.IsInstance())
		{
			GameObject commonSpace = Singleton<Manager.Scene>.Instance.commonSpace;
			if (commonSpace != null)
			{
				DeliveryHPointData component = commonSpace.GetComponent<DeliveryHPointData>();
				if ((bool)component)
				{
					actionSelect = component.actionSelect;
					actionBack = component.actionBack;
					idMap = component.IDMap;
					cam = component.cam;
					lstCategory = component.lstCategory;
					initPos = component.initPos;
					initRot = component.initRot;
					isFreeH = component.isFreeH;
					status = component.status;
					lstAnimInfo = component.lstAnimInfo;
					isDebug = component.isDebug;
					flags = component.flags;
				}
				UnityEngine.Object.Destroy(component);
			}
		}
		StringBuilder sb = new StringBuilder("HPoint_");
		if (flags.nowAnimationInfo.mode == HFlag.EMode.masturbation || flags.nowAnimationInfo.mode == HFlag.EMode.lesbian)
		{
			sb.Append("Add_");
		}
		else if (flags.nowAnimationInfo.mode == HFlag.EMode.houshi3P || flags.nowAnimationInfo.mode == HFlag.EMode.sonyu3P)
		{
			sb.Append("3P_");
		}
		List<GameObject> objs = GlobalMethod.LoadAllFolder<GameObject>("h/common/", sb.ToString() + idMap);
		if (objs.Count != 0)
		{
			objMapNull = UnityEngine.Object.Instantiate(objs[objs.Count - 1]);
			if ((bool)objMapNull)
			{
				SceneManager.MoveGameObjectToScene(objMapNull, SceneManager.GetSceneByName("HPointMove"));
			}
		}
		bool isInit = lstCategory.Any((int c) => c == 0 || c == 1 || c == 8);
		bool isSpecial = lstCategory.Any((int c) => c >= 1000);
		if (isInit)
		{
			objBasePoint = CommonLib.LoadAsset<GameObject>("h/common/00_00.unity3d", "HPointBase", true, string.Empty);
			AssetBundleManager.UnloadAssetBundle("h/common/00_00.unity3d", true);
			if ((bool)objBasePoint)
			{
				SceneManager.MoveGameObjectToScene(objBasePoint, SceneManager.GetSceneByName("HPointMove"));
				objBasePoint.transform.SetPositionAndRotation(initPos, initRot);
				HPointData component2 = objBasePoint.GetComponent<HPointData>();
				if ((bool)component2)
				{
					component2.category = lstCategory.ToArray();
				}
			}
		}
		LoadOmitCategoryList();
		CreateCategoryToggle();
		for (int i = 0; i < objCategoryParent.transform.childCount; i++)
		{
			Transform child = objCategoryParent.transform.GetChild(i);
			if ((bool)child && child.name.Contains("category"))
			{
				int key = int.Parse(child.name.Substring(8));
				dicToggle[key] = child.GetComponent<Toggle>();
				dicToggle[key].isOn = i == 0;
			}
		}
		if ((bool)objMapNull)
		{
			HPointData[] componentsInChildren = objMapNull.GetComponentsInChildren<HPointData>(true);
			HPointOmitObject component3 = objMapNull.GetComponent<HPointOmitObject>();
			HPointData[] array = componentsInChildren;
			foreach (HPointData hPointData in array)
			{
				hPointData.gameObject.SetActive(false);
				if (isDebug)
				{
					if (!component3.list.Contains(hPointData.gameObject))
					{
						SetCategoryObjects(hPointData);
					}
				}
				else
				{
					if (component3.list.Contains(hPointData.gameObject))
					{
						continue;
					}
					if (isFreeH)
					{
						if (!hPointData.category.Any((int c) => MathfEx.IsRange(2000, c, 2999, true)) && IsCategoryInAnimationList(hPointData.category) && IsExperience(hPointData.Experience))
						{
							SetCategoryObjects(hPointData);
						}
					}
					else if (isSpecial)
					{
						if (hPointData.category.Any((int c) => c >= 1000 && lstCategory.Contains(c)) && IsExperience(hPointData.Experience))
						{
							SetCategoryObjects(hPointData, true);
						}
					}
					else if (!hPointData.category.Any((int c) => c >= 1000) && IsExperience(hPointData.Experience))
					{
						SetCategoryObjects(hPointData);
					}
				}
			}
		}
		if ((bool)objBasePoint)
		{
			foreach (int item in lstCategory)
			{
				if (!dicObj.ContainsKey(item))
				{
					dicObj.Add(item, new List<GameObject>());
				}
				dicObj[item].Add(objBasePoint);
			}
		}
		foreach (KeyValuePair<int, List<GameObject>> item2 in dicObj)
		{
			if (!item2.Value.Any())
			{
				dicToggle.Remove(item2.Key);
			}
		}
		foreach (KeyValuePair<int, List<GameObject>> item3 in dicObj)
		{
			dicToggle[item3.Key].gameObject.SetActive(true);
		}
		List<int> lst = dicObj.Keys.ToList();
		lst.Sort();
		SelectPointVisible(lst[0], true);
		nowCategory = lst[0];
		foreach (KeyValuePair<int, Toggle> t in dicToggle)
		{
			t.Value.OnPointerClickAsObservable().Subscribe(delegate
			{
				OnClick(t.Key);
			});
		}
		yield return null;
	}

	private void Update()
	{
		if (Singleton<Manager.Scene>.Instance.NowSceneNames[0] != "HPointMove")
		{
			return;
		}
		if (!GlobalMethod.IsCameraMoveFlag(cam))
		{
			GlobalMethod.SetCameraMoveFlag(cam, true);
		}
		IsMouseOnCollider();
		if (Input.GetKeyDown(KeyCode.F6))
		{
			Return();
		}
		else if (Input.GetMouseButtonDown(0) && (bool)selectData)
		{
			CheckScene.Parameter param = new CheckScene.Parameter
			{
				Title = (uiTranslater.Get(0).Values.FindTagText("Check") ?? "選択した場所に移動しますか？"),
				Yes = delegate
				{
					actionSelect.Call(selectData, nowCategory);
					Singleton<Manager.Scene>.Instance.UnLoad();
					Singleton<Manager.Scene>.Instance.UnLoad();
				},
				No = delegate
				{
					Singleton<Manager.Scene>.Instance.UnLoad();
				}
			};
			Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param, observer)).StartAsCoroutine();
		}
	}

	public void OnClick(int _category)
	{
		SelectPointVisible(_category);
		nowCategory = _category;
	}

	private bool IsExperience(int _experience)
	{
		switch (_experience)
		{
		case 0:
			return true;
		case 1:
			if (status > SaveData.Heroine.HExperienceKind.不慣れ)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private bool IsMouseOnCollider()
	{
		Ray ray = cam.thisCmaera.ScreenPointToRay(Input.mousePosition);
		List<RaycastHit> list = new List<RaycastHit>(Physics.RaycastAll(ray));
		if (GlobalMethod.IsCameraActionFlag(cam))
		{
			return false;
		}
		if (EventSystem.current.IsPointerOverGameObject())
		{
			SelectDataIsDownAnimation();
			return false;
		}
		if (list.Count == 0)
		{
			SelectDataIsDownAnimation();
			return false;
		}
		RaycastHit raycastHit = (from t in list
			where t.collider.tag.Contains("H/HPoint")
			select t into d
			orderby d.distance
			select d).FirstOrDefault();
		if (raycastHit.collider == null)
		{
			SelectDataIsDownAnimation();
			return false;
		}
		HPointData component = raycastHit.collider.transform.parent.GetComponent<HPointData>();
		if ((bool)component && selectData != component)
		{
			Animator component2 = raycastHit.collider.GetComponent<Animator>();
			if ((bool)component2 && component2.GetCurrentAnimatorStateInfo(0).IsName("idle"))
			{
				component2.SetTrigger("up");
			}
			SelectDataIsDownAnimation();
			selectData = component;
			Utils.Sound.Play(SystemSE.sel);
		}
		return true;
	}

	private bool SelectDataIsDownAnimation()
	{
		if (!selectData)
		{
			return false;
		}
		Animator componentInChildren = selectData.GetComponentInChildren<Animator>();
		if ((bool)componentInChildren)
		{
			if (componentInChildren.GetCurrentAnimatorStateInfo(0).IsName("upidle"))
			{
				componentInChildren.SetTrigger("down");
			}
			else
			{
				componentInChildren.Play("idle");
			}
		}
		selectData = null;
		return true;
	}

	public void Return()
	{
		Observable.NextFrame().Subscribe(delegate
		{
			actionBack.Call();
			Singleton<Manager.Scene>.Instance.UnLoad();
		});
	}

	private bool IsCategoryInAnimationList(int[] _categorys)
	{
		if (flags.nowAnimationInfo.mode == HFlag.EMode.masturbation)
		{
			if (_categorys.Any((int c) => MathfEx.IsRange(1010, c, 1099, true)))
			{
				return true;
			}
			return false;
		}
		if (flags.nowAnimationInfo.mode == HFlag.EMode.lesbian)
		{
			if (_categorys.Any((int c) => MathfEx.IsRange(1100, c, 1199, true)))
			{
				return true;
			}
			return false;
		}
		Dictionary<int, HashSet<int>> dictionary = new Dictionary<int, HashSet<int>>();
		foreach (int item in _categorys)
		{
			for (int j = 0; j < lstAnimInfo.Length; j++)
			{
				for (int k = 0; k < lstAnimInfo[j].Count; k++)
				{
					if (lstAnimInfo[j][k].stateRestriction > (int)status)
					{
						continue;
					}
					List<int> list = lstAnimInfo[j][k].lstCategory.Select((HSceneProc.Category _) => _.category).ToList();
					if (list.Contains(item))
					{
						HashSet<int> value;
						if (!dictionary.TryGetValue(j, out value))
						{
							value = (dictionary[j] = new HashSet<int>());
						}
						value.Add(lstAnimInfo[j][k].id);
					}
				}
			}
		}
		Dictionary<int, HashSet<int>> playHList = Singleton<Game>.Instance.glSaveData.playHList;
		foreach (KeyValuePair<int, HashSet<int>> item2 in dictionary)
		{
			HashSet<int> value2;
			if (!playHList.TryGetValue(item2.Key, out value2))
			{
				continue;
			}
			foreach (int item3 in item2.Value)
			{
				if (value2.Contains(item3))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsCategoryInAnimationList(int _category)
	{
		if (flags.nowAnimationInfo.mode == HFlag.EMode.masturbation)
		{
			if (MathfEx.IsRange(1010, _category, 1099, true))
			{
				return true;
			}
			return false;
		}
		if (flags.nowAnimationInfo.mode == HFlag.EMode.lesbian)
		{
			if (MathfEx.IsRange(1100, _category, 1199, true))
			{
				return true;
			}
			return false;
		}
		Dictionary<int, HashSet<int>> dictionary = new Dictionary<int, HashSet<int>>();
		for (int i = 0; i < lstAnimInfo.Length; i++)
		{
			for (int j = 0; j < lstAnimInfo[i].Count; j++)
			{
				if (lstAnimInfo[i][j].stateRestriction > (int)status)
				{
					continue;
				}
				List<int> list = lstAnimInfo[i][j].lstCategory.Select((HSceneProc.Category _) => _.category).ToList();
				if (list.Contains(_category))
				{
					HashSet<int> value;
					if (!dictionary.TryGetValue(i, out value))
					{
						value = (dictionary[i] = new HashSet<int>());
					}
					value.Add(lstAnimInfo[i][j].id);
				}
			}
		}
		Dictionary<int, HashSet<int>> playHList = Singleton<Game>.Instance.glSaveData.playHList;
		foreach (KeyValuePair<int, HashSet<int>> item in dictionary)
		{
			HashSet<int> value2;
			if (!playHList.TryGetValue(item.Key, out value2))
			{
				continue;
			}
			foreach (int item2 in item.Value)
			{
				if (value2.Contains(item2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void SetCategoryObjects(HPointData _data, bool _isSpecial = false)
	{
		for (int i = 0; i < _data.category.Length; i++)
		{
			int num = _data.category[i];
			if (num != 10 && num != 11 && (!isFreeH || IsCategoryInAnimationList(num)) && (!_isSpecial || (num >= 1000 && lstCategory.Contains(num))))
			{
				if (!dicObj.ContainsKey(num))
				{
					dicObj.Add(num, new List<GameObject>());
				}
				dicObj[num].Add(_data.gameObject);
			}
		}
	}

	private void SelectPointVisible(int _key, bool _isToggleOn = false)
	{
		foreach (KeyValuePair<int, List<GameObject>> item in dicObj)
		{
			foreach (GameObject item2 in item.Value)
			{
				item2.SetActive(false);
			}
		}
		foreach (GameObject item3 in dicObj[_key])
		{
			item3.SetActive(true);
		}
		if (_isToggleOn)
		{
			dicToggle[_key].isOn = true;
		}
	}

	private bool CreateCategoryToggle()
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "HPointToggle");
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
			int num2 = int.Parse(data[i, num++]);
			string text2 = "category" + num2;
			GameObject gameObject = objCategoryParent.transform.FindLoop(text2);
			HPointCategoryInfo component;
			if ((bool)gameObject)
			{
				component = gameObject.GetComponent<HPointCategoryInfo>();
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate(objToggleNode);
				gameObject.name = text2;
				gameObject.transform.SetParent(objCategoryParent.transform, false);
				component = gameObject.GetComponent<HPointCategoryInfo>();
			}
			component.text.text = data[i, num++];
			Localize.Translate.Manager.Bind(component.text, uiTranslater.Get(1).Get(num2), true);
		}
		return true;
	}

	private bool LoadOmitCategoryList()
	{
		if (flags.nowAnimationInfo.mode == HFlag.EMode.masturbation || flags.nowAnimationInfo.mode == HFlag.EMode.lesbian)
		{
			return false;
		}
		string text = GlobalMethod.LoadAllListText("h/list/", "HPointOmitCategory");
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
			hashOmitCategory.Add(int.Parse(data[i, num++]));
		}
		return true;
	}
}
