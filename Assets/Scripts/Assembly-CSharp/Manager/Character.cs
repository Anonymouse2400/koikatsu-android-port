using System;
using System.Collections.Generic;
using System.Linq;
using Illusion;
using Localize.Translate;
using UnityEngine;

namespace Manager
{
	public class Character : Singleton<Character>
	{
		public class RandamNameInfo
		{
			public List<string> lstLastNameH = new List<string>();

			public List<string> lstLastNameK = new List<string>();

			public List<string> lstFemaleFirstNameH = new List<string>();

			public List<string> lstFemaleFirstNameK = new List<string>();

			public List<string> lstMaleFirstNameH = new List<string>();

			public List<string> lstMaleFirstNameK = new List<string>();
		}

		private class Initializable : InitializeSolution.IInitializable
		{
			private readonly Character c;

			private bool initialized;

			bool InitializeSolution.IInitializable.initialized
			{
				get
				{
					return initialized;
				}
			}

			public Initializable(Character c)
			{
				this.c = c;
			}

			void InitializeSolution.IInitializable.Initialize()
			{
				if (initialized)
				{
					return;
				}
				initialized = true;
				if (!Localize.Translate.Manager.isTranslate)
				{
					return;
				}
				Localize.Translate.Manager.SCENE_ID sceneID = Localize.Translate.Manager.SCENE_ID.CUSTOM_LIST;
				Dictionary<int, Dictionary<int, Data.Param>> translater = Localize.Translate.Manager.LoadScene(sceneID, null);
				Utils.Enum<ChaListDefine.CategoryNo>.Each(delegate(ChaListDefine.CategoryNo e)
				{
					foreach (KeyValuePair<int, ListInfoBase> item in c.chaListCtrl.GetCategoryInfo(e))
					{
						ListInfoBase value = item.Value;
						Data.Param param = translater.Get(value.Category).SafeGet(value.Id);
						if (param != null)
						{
							Dictionary<int, string> dictInfo = value.dictInfo;
							int key = 41;
							if (!param.text.IsNullOrEmpty() && dictInfo.ContainsKey(key))
							{
								dictInfo[key] = param.text;
							}
							if (!param.Bundle.IsNullOrEmpty() && !param.asset.IsNullOrEmpty())
							{
								int key2 = 67;
								int key3 = 68;
								if (dictInfo.ContainsKey(key2) && dictInfo.ContainsKey(key3))
								{
									dictInfo[key2] = param.Bundle;
									dictInfo[key3] = param.asset;
								}
							}
						}
					}
				});
				Localize.Translate.Manager.DisposeScene(sceneID, false);
			}
		}

		public readonly string mainManifestName = "abdata";

		private int rampId = 1;

		private float shadowDepth = 0.26f;

		private float lineDepth = 1f;

		private float lineWidth = 0.307f;

		public List<AssetBundleData> lstLoadAssetBundleInfo = new List<AssetBundleData>();

		public bool loadAssetBundle;

		public ChaListControl chaListCtrl { get; private set; }

		public ChaLoadingTask loading { get; private set; }

		public SortedDictionary<int, ChaControl> dictEntryChara { get; private set; }

		public bool enableCorrectArmSize { get; set; }

		public bool enableCorrectHandSize { get; set; }

		public bool enableCharaLoadGCClear { get; set; }

		public ChaFileControl editChara { get; set; }

		public ChaFileControl[] netRandChara { get; set; }

		public int netRandCharaNum { get; set; }

		public string nextNetworkScene { get; set; }

		public List<string> lstProductId { get; set; }

		public RandamNameInfo randamNameInfo { get; private set; }

		public void BeginLoadAssetBundle()
		{
			lstLoadAssetBundleInfo.Clear();
			loadAssetBundle = true;
		}

		public void AddLoadAssetBundle(string assetBundleName, string manifestName)
		{
			if (manifestName.IsNullOrEmpty())
			{
				manifestName = mainManifestName;
			}
			AssetBundleManifestData assetBundleManifestData = new AssetBundleManifestData();
			assetBundleManifestData.bundle = assetBundleName;
			assetBundleManifestData.manifest = manifestName;
			lstLoadAssetBundleInfo.Add(assetBundleManifestData);
		}

		public void EndLoadAssetBundle(bool forceRemove = false)
		{
			foreach (AssetBundleData item in lstLoadAssetBundleInfo)
			{
				AssetBundleManager.UnloadAssetBundle(item.bundle, true, null, forceRemove);
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			lstLoadAssetBundleInfo.Clear();
			loadAssetBundle = false;
		}

		public ChaControl CreateFemale(GameObject parent, int id, ChaFileControl _chaFile = null, bool hiPoly = true)
		{
			return CreateChara(1, parent, id, _chaFile, hiPoly);
		}

		public ChaControl CreateMale(GameObject parent, int id, ChaFileControl _chaFile = null, bool hiPoly = true)
		{
			return CreateChara(0, parent, id, _chaFile, hiPoly);
		}

		public ChaControl CreateChara(byte _sex, GameObject parent, int id, ChaFileControl _chaFile = null, bool hiPoly = true)
		{
			int num = 0;
			int num2 = 1;
			foreach (KeyValuePair<int, ChaControl> item in dictEntryChara)
			{
				if (item.Value.sex == _sex)
				{
					num2++;
				}
				if (num != item.Key)
				{
					break;
				}
				num++;
			}
			string text = ((_sex != 0) ? "chaF_" : "chaM_") + num2.ToString("000");
			GameObject gameObject = new GameObject(text);
			if ((bool)parent)
			{
				gameObject.transform.SetParent(parent.transform, false);
			}
			ChaControl chaControl = gameObject.AddComponent<ChaControl>();
			if ((bool)chaControl)
			{
				chaControl.Initialize(_sex, hiPoly, gameObject, id, num, _chaFile);
			}
			dictEntryChara.Add(num, chaControl);
			return chaControl;
		}

		public bool IsChara(ChaControl cha)
		{
			foreach (KeyValuePair<int, ChaControl> item in dictEntryChara)
			{
				if (item.Value == cha)
				{
					return true;
				}
			}
			return false;
		}

		public bool DeleteChara(ChaControl cha, bool entryOnly = false)
		{
			foreach (KeyValuePair<int, ChaControl> item in dictEntryChara)
			{
				if (item.Value == cha)
				{
					if (!entryOnly)
					{
						item.Value.gameObject.name = "Delete_Reserve";
						item.Value.transform.SetParent(null);
						UnityEngine.Object.Destroy(item.Value.gameObject);
					}
					dictEntryChara.Remove(item.Key);
					return true;
				}
			}
			return false;
		}

		public void DeleteCharaAll()
		{
			foreach (KeyValuePair<int, ChaControl> item in dictEntryChara)
			{
				if ((bool)item.Value)
				{
					item.Value.gameObject.name = "Delete_Reserve";
					item.Value.transform.SetParent(null);
					UnityEngine.Object.Destroy(item.Value.gameObject);
				}
			}
			dictEntryChara.Clear();
		}

		public List<ChaControl> GetCharaList(byte _sex)
		{
			return (from c in dictEntryChara
				where c.Value.sex == _sex
				select c into x
				select x.Value).ToList();
		}

		public ChaControl GetChara(byte _sex, int _id)
		{
			try
			{
				return dictEntryChara.Where((KeyValuePair<int, ChaControl> s) => s.Value.sex == _sex).First((KeyValuePair<int, ChaControl> v) => v.Value.chaID == _id).Value;
			}
			catch (ArgumentNullException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			return null;
		}

		public ChaControl GetChara(int _id)
		{
			try
			{
				return dictEntryChara.First((KeyValuePair<int, ChaControl> v) => v.Value.chaID == _id).Value;
			}
			catch (ArgumentNullException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			return null;
		}

		public ChaControl GetCharaFromLoadNo(int _no)
		{
			try
			{
				return dictEntryChara.First((KeyValuePair<int, ChaControl> v) => v.Value.loadNo == _no).Value;
			}
			catch (ArgumentNullException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			return null;
		}

		public static void ChangeRootParent(ChaControl cha, Transform trfNewParent)
		{
			if (null != cha)
			{
				cha.transform.SetParent(trfNewParent, false);
			}
		}

		protected new void Awake()
		{
			if (CheckInstance())
			{
				enableCharaLoadGCClear = true;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				chaListCtrl = new ChaListControl();
				loading = new ChaLoadingTask();
				dictEntryChara = new SortedDictionary<int, ChaControl>();
				editChara = null;
				netRandChara = null;
				netRandCharaNum = 0;
				lstProductId = new List<string>();
				chaListCtrl.LoadListInfoAll();
				Localize.Translate.Manager.initializeSolution.Add(new Initializable(this));
				randamNameInfo = new RandamNameInfo();
				string text = "list/random_name/random_name.unity3d";
				if (Localize.Translate.Manager.isTranslate)
				{
					text = Localize.Translate.Manager.DefaultData.BundlePath + text;
				}
				List<ExcelData.Param> source = ChaListControl.LoadExcelData(text, "random_name", 2, 1);
				randamNameInfo.lstLastNameH = (from x in source
					select x.list[0] into x
					where x != "0"
					select x).ToList();
				randamNameInfo.lstLastNameK = (from x in source
					select x.list[1] into x
					where x != "0"
					select x).ToList();
				randamNameInfo.lstFemaleFirstNameH = (from x in source
					select x.list[2] into x
					where x != "0"
					select x).ToList();
				randamNameInfo.lstFemaleFirstNameK = (from x in source
					select x.list[3] into x
					where x != "0"
					select x).ToList();
				randamNameInfo.lstMaleFirstNameH = (from x in source
					select x.list[4] into x
					where x != "0"
					select x).ToList();
				randamNameInfo.lstMaleFirstNameK = (from x in source
					select x.list[5] into x
					where x != "0"
					select x).ToList();
			}
		}

		protected void Start()
		{
			rampId = Config.EtcData.rampId;
			shadowDepth = Config.EtcData.shadowDepth;
			lineDepth = Config.EtcData.lineDepth;
			lineWidth = Config.EtcData.lineWidth;
			UpdateGlobalShader(true, true, true, true);
			enableCorrectHandSize = true;
		}

		protected void Update()
		{
			if (!CheckInstance())
			{
				return;
			}
			bool ramp = rampId != Config.EtcData.rampId;
			bool shadowD = shadowDepth != Config.EtcData.shadowDepth;
			bool lineD = lineDepth != Config.EtcData.lineDepth;
			bool lineW = lineWidth != Config.EtcData.lineWidth;
			UpdateGlobalShader(ramp, shadowD, lineD, lineW);
			foreach (KeyValuePair<int, ChaControl> item in dictEntryChara)
			{
				item.Value.UpdateForce();
			}
		}

		protected void LateUpdate()
		{
			foreach (KeyValuePair<int, ChaControl> item in dictEntryChara)
			{
				item.Value.LateUpdateForce();
			}
		}

		private void UpdateGlobalShader(bool ramp, bool shadowD, bool lineD, bool lineW)
		{
			if (ramp)
			{
				rampId = Config.EtcData.rampId;
				Texture2D texture2D = null;
				ListInfoBase listInfo = chaListCtrl.GetListInfo(ChaListDefine.CategoryNo.mt_ramp, rampId);
				if (listInfo != null)
				{
					string info = listInfo.GetInfo(ChaListDefine.KeyType.MainTexAB);
					string info2 = listInfo.GetInfo(ChaListDefine.KeyType.MainTex);
					if ("0" != info && "0" != info2)
					{
						texture2D = CommonLib.LoadAsset<Texture2D>(info, info2, false, string.Empty);
					}
					if ((bool)texture2D)
					{
						ChaShader.ChangeRampTexture(texture2D);
					}
				}
			}
			if (shadowD)
			{
				shadowDepth = Config.EtcData.shadowDepth;
				ChaShader.ChangeAmbientShaodwColor(shadowDepth);
			}
			if (lineD)
			{
				lineDepth = Config.EtcData.lineDepth;
				ChaShader.ChangeLineColor(lineDepth);
			}
			if (lineW)
			{
				lineWidth = Config.EtcData.lineWidth;
				ChaShader.ChangeLineWidth(lineWidth);
			}
		}

		private void OnDestroy()
		{
			chaListCtrl.SaveItemID();
		}
	}
}
