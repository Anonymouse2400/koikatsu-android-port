using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Localize.Translate
{
	public static class Manager
	{
		public enum SCENE_ID
		{
			SET_UP = 0,
			CUSTOM_UI = 1,
			CUSTOM_LIST = 2,
			CUSTOM_LIST2 = 3,
			H_UI = 4,
			H_POSTURE = 5,
			H_POINT_UI = 6,
			H_RESULT_UI = 7,
			TITLE = 8,
			NICK_NAME = 9,
			MAP = 10,
			CHECK_SCENE = 11,
			SAVE_LOAD = 12,
			RANKING = 13,
			UPLOAD = 14,
			DOWNLOAD = 15,
			ENTRY_PLAYER = 16,
			CLASS_REGISTER = 17,
			NIGHT_MENU = 18,
			ACTION = 19,
			COMMUNICATION = 20,
			TUTORIAL = 21,
			SHORTCUT = 22,
			FREE_H = 23,
			LIVE = 24,
			EXTRA_EVENT = 25,
			CLUB_REPORT = 26,
			CLUB_SUMMON = 27,
			ID_CARD = 28,
			CONFIG = 29,
			NET_CHECK = 30,
			ADV = 31,
			CHARA_LIST = 32,
			LOGO = 33,
			OTHER = 99,
			VR = 100,
			VR_TITLE = 101,
			VR_FREE_H = 102,
			VR_H = 103,
			VR_CONFIG = 104
		}

		public enum LanguageID
		{
			JP = 0,
			US = 1,
			CN = 2,
			TW = 3
		}

		private class SelectData
		{
			public class FontData
			{
				public readonly Font[] fonts;

				public readonly TMP_FontAsset[] assets;

				public readonly Material[] materials;

				public FontData(Font[] fonts, TMP_FontAsset[] assets, Material[] materials)
				{
					this.fonts = fonts;
					this.assets = assets;
					this.materials = materials;
				}

				public bool Set(Text text, int index = 0)
				{
					return text != null && fonts.SafeProc(index, delegate(Font font)
					{
						text.font = font;
					});
				}

				public bool Set(TMP_Text text, int fontIndex = 0, int matIndex = -1)
				{
					if (text == null || assets.IsNullOrEmpty() || materials.IsNullOrEmpty())
					{
						return false;
					}
					string baseMaterialName = text.fontSharedMaterial.name.Substring(text.font.name.Length);
					assets.SafeProc(fontIndex, delegate(TMP_FontAsset font)
					{
						text.font = font;
					});
					int nowFontLength = text.font.name.Length;
					Material material = null;
					if (matIndex == -1)
					{
						material = materials.Where((Material p) => p.name.Length >= nowFontLength).FirstOrDefault((Material p) => baseMaterialName == p.name.Substring(nowFontLength));
						matIndex = 0;
					}
					material = material ?? materials.SafeGet(matIndex);
					if (material != null)
					{
						text.fontSharedMaterial = material;
					}
					return true;
				}
			}

			public readonly bool isValid;

			public readonly string Culture = "en-US";

			public readonly string BaseManifest;

			public readonly FontData fontData;

			public SelectData()
			{
				fontData = new FontData(null, null, null);
			}

			public SelectData(Info.Param param)
			{
				isValid = true;
				if (!param.Culture.IsNullOrEmpty())
				{
					Culture = param.Culture;
				}
				BaseManifest = param.BaseManifest;
				AssetBundleManifestData assetBundleManifestData = new AssetBundleManifestData(LanguagePath + "font.unity3d", null, param.BaseManifest);
				assetBundleManifestData.ClearRequest();
				TMP_FontAsset[] array = assetBundleManifestData.GetAllAssets<TMP_FontAsset>();
				List<Material> matList = new List<Material>();
				if (array != null)
				{
					array = array.OrderBy((TMP_FontAsset p) => p.name).ToArray();
					matList.AddRange(array.Select((TMP_FontAsset p) => p.material));
				}
				assetBundleManifestData.ClearRequest();
				assetBundleManifestData.GetAllAssets<Material>().SafeProc(delegate(Material[] materials)
				{
					matList.AddRange(from mat in materials
						where !matList.Contains(mat)
						select mat into p
						orderby p.name
						select p);
				});
				assetBundleManifestData.ClearRequest();
				Font[] allAssets = assetBundleManifestData.GetAllAssets<Font>();
				fontData = new FontData(allAssets, array, matList.Any() ? matList.ToArray() : null);
				assetBundleManifestData.UnloadBundle(true);
			}
		}

		private class CultureJPScope : CultureScope
		{
			public CultureJPScope()
				: base("ja-JP")
			{
			}
		}

		private class CultureScope : IDisposable
		{
			private readonly CultureInfo cultureInfo;

			public CultureScope(string culture)
			{
				cultureInfo = Thread.CurrentThread.CurrentCulture;
				Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
			}

			void IDisposable.Dispose()
			{
				Thread.CurrentThread.CurrentCulture = cultureInfo;
			}
		}

		public static class DefaultData
		{
			public class DefaultFile
			{
				private string path;

				private FileData _fileData;

				public string Path
				{
					get
					{
						return (_fileData ?? (_fileData = new FileData("DefaultData"))).Path + path;
					}
				}

				public DefaultFile(string path)
				{
					this.path = path;
				}

				public bool IsFile(string path)
				{
					return File.Exists(this.path + path);
				}
			}

			private const string BUNDLE = "defdata";

			private const string PATH = "DefaultData";

			private static string _bundlePath;

			private static DefaultFile _commonFile;

			private static DefaultFile _dataFile;

			public static string BundlePath
			{
				get
				{
					return _bundlePath ?? (_bundlePath = LanguagePath + "defdata/");
				}
			}

			public static DefaultFile commonFile
			{
				get
				{
					return _commonFile ?? (_commonFile = new DefaultFile("common/"));
				}
			}

			private static DefaultFile dataFile
			{
				get
				{
					return _dataFile ?? (_dataFile = new DefaultFile(language.Value + "/"));
				}
			}

			public static string GetCommonPath(string path)
			{
				string text = commonFile.Path + path;
				if (Directory.Exists(text))
				{
					return text;
				}
				return null;
			}

			public static string GetPath(string path)
			{
				string text = dataFile.Path + path;
				if (Directory.Exists(text))
				{
					return text;
				}
				return null;
			}

			public static FileInfo[] UserDataCommonAssist(string path, params string[] filters)
			{
				IEnumerable<FolderAssist.FileInfo> enumerable = Enumerable.Empty<FolderAssist.FileInfo>();
				IEnumerable<FileInfo> first = Enumerable.Empty<FileInfo>();
				string commonPath = GetCommonPath(path);
				if (commonPath != null)
				{
					first = (FolderAssist.CreateFolderInfoExToArray(commonPath, filters) ?? enumerable).Select((FolderAssist.FileInfo p) => new FileInfo(p, true));
				}
				return first.Concat((FolderAssist.CreateFolderInfoExToArray(UserData.Path + path, filters) ?? enumerable).Select((FolderAssist.FileInfo p) => new FileInfo(p, false))).ToArray();
			}

			public static FileInfo[] UserDataAssist(string path, bool useDefaultData = true)
			{
				string text = "*.png";
				IEnumerable<FolderAssist.FileInfo> enumerable = Enumerable.Empty<FolderAssist.FileInfo>();
				IEnumerable<FileInfo> first = Enumerable.Empty<FileInfo>();
				if (useDefaultData)
				{
					string path2 = GetPath(path);
					if (path2 != null)
					{
						first = (FolderAssist.CreateFolderInfoExToArray(path2, text) ?? enumerable).Select((FolderAssist.FileInfo p) => new FileInfo(p, true));
					}
				}
				return first.Concat((FolderAssist.CreateFolderInfoExToArray(UserData.Path + path, text) ?? enumerable).Select((FolderAssist.FileInfo p) => new FileInfo(p, false))).ToArray();
			}

			public static string GetBundlePath(string path)
			{
				return GetBundlePath(ref path) ? path : null;
			}

			public static bool GetBundlePath(ref string path)
			{
				if (!isTranslate)
				{
					return false;
				}
				path = BundlePath + path;
				return true;
			}

			public static List<string> GetAssetBundleNameListFromPath(string path, bool subdirCheck)
			{
				string bundlePath = GetBundlePath(path);
				if (bundlePath != null)
				{
					List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(bundlePath, subdirCheck);
					if (assetBundleNameListFromPath.Any())
					{
						return assetBundleNameListFromPath;
					}
				}
				return CommonLib.GetAssetBundleNameListFromPath(path, subdirCheck);
			}
		}

		public class FileInfo
		{
			public readonly FolderAssist.FileInfo info;

			public readonly bool isDefault;

			public FileInfo(FolderAssist.FileInfo info, bool isDefault)
			{
				this.info = info;
				this.isDefault = isDefault;
			}

			public FileInfo(FileInfo src)
			{
				info = src.info;
				isDefault = src.isDefault;
			}
		}

		public class ChaFileInfo : FileInfo
		{
			public readonly ChaFileControl chaFile;

			public ChaFileInfo(ChaFileControl chaFile, FileInfo info)
				: base(info)
			{
				this.chaFile = chaFile;
			}
		}

		public class ChaCoordinateInfo : FileInfo
		{
			public readonly ChaFileCoordinate coordinate;

			public ChaCoordinateInfo(ChaFileCoordinate coordinate, FileInfo info)
				: base(info)
			{
				this.coordinate = coordinate;
			}
		}

		public static readonly InitializeSolution initializeSolution = new InitializeSolution();

		private const string MAIN_PATH = "localize/";

		private const string INFO_PATH = "localize/info/";

		private const string BASE_PATH = "localize/translate/";

		private const string UI_PATH = "ui/";

		private const string TYPE_PATH = "type/";

		private const string SCENARIO_PATH = "scenario/";

		private const string SCENARIO_REPLACE_NAME_PATH = "scenario/charaname/";

		private const string FONT_FILE = "font.unity3d";

		private const string DEFAULT_CULTURE = "en-US";

		private static IntReactiveProperty language = new IntReactiveProperty(1);

		private static Dictionary<int, Dictionary<int, Data.Param>> _OtherData = null;

		private static Dictionary<string, List<ScenarioCharaName.Param>> _ScenarioReplaceNameData = null;

		private static readonly Dictionary<int, Dictionary<int, Dictionary<int, Data.Param>>> scene = new Dictionary<int, Dictionary<int, Dictionary<int, Data.Param>>>();

		private static readonly Dictionary<int, int> refCnt = new Dictionary<int, int>();

		public static string LanguageUIPath
		{
			get
			{
				return _languageUIPath ?? (_languageUIPath = LanguagePath + "ui/");
			}
		}

		private static string _languageUIPath { get; set; }

		private static string LanguagePath
		{
			get
			{
				return _languagePath;
			}
			set
			{
				_languagePath = "localize/translate/" + value + "/";
			}
		}

		private static string _languagePath { get; set; }

		public static bool initialized { get; private set; }

		public static bool isTranslate
		{
			get
			{
				return select.isValid;
			}
		}

		public static int Language
		{
			get
			{
				return language.Value;
			}
		}

		public static string AdvScenarioPath
		{
			get
			{
				return "adv/scenario/";
			}
		}

		public static Dictionary<int, Dictionary<int, Data.Param>> OtherData
		{
			get
			{
				return _OtherData ?? (_OtherData = LoadScene(SCENE_ID.OTHER, null));
			}
		}

		public static string UnknownText
		{
			get
			{
				return OtherData.Get(3).SafeGetText(4) ?? "不明";
			}
		}

		private static Dictionary<string, List<ScenarioCharaName.Param>> ScenarioReplaceNameData
		{
			get
			{
				return _ScenarioReplaceNameData ?? (_ScenarioReplaceNameData = LoadScenarioCharaNames());
			}
		}

		private static SelectData select { get; set; }

		[Conditional("TODO_CHECK")]
		public static void TODO_CHECK()
		{
		}

		public static bool Check(LanguageID id)
		{
			return language.Value == (int)id;
		}

		public static bool SetLanguage(LanguageID id)
		{
			return SetLanguage((int)id);
		}

		public static bool SetLanguage(int value)
		{
			if (value == 0)
			{
				UnityEngine.Debug.LogError("Japanese is not supported");
				value = 1;
			}
			language.Value = value;
			return value == 0 || select.isValid;
		}

		public static string BirthdayText(int type)
		{
			return OtherData.Get(3).Values.ToArray("Birthday").SafeGet(type);
		}

		public static void GetName(ChaFileParameter parameter, bool check, out string personalityName, out string clubName)
		{
			if (parameter.sex == 0)
			{
				personalityName = string.Empty;
				clubName = string.Empty;
			}
			else
			{
				GetName(parameter.personality, parameter.clubActivities, check, out personalityName, out clubName);
			}
		}

		public static void GetName(int personality, int clubActivities, bool check, out string personalityName, out string clubName)
		{
			personalityName = GetPersonalityName(personality, check);
			clubName = GetClubName(clubActivities, check);
		}

		public static string GetPersonalityName(int personality, bool check)
		{
			Dictionary<int, VoiceInfo.Param> voiceInfoDic = Singleton<Voice>.Instance.voiceInfoDic;
			VoiceInfo.Param value;
			bool flag = voiceInfoDic.TryGetValue(personality, out value);
			if (check && !flag)
			{
				flag = voiceInfoDic.TryGetValue(0, out value);
			}
			return (!flag) ? UnknownText : value.Personality;
		}

		public static string GetClubName(int clubActivities, bool check)
		{
			Dictionary<int, ClubInfo.Param> clubInfos = Game.ClubInfos;
			ClubInfo.Param value;
			bool flag = clubInfos.TryGetValue(clubActivities, out value);
			if (check && !flag)
			{
				flag = clubInfos.TryGetValue(0, out value);
			}
			return (!flag) ? UnknownText : value.Name;
		}

		public static void Initialize()
		{
			if (initialized)
			{
				return;
			}
			language.Subscribe(delegate(int value)
			{
				_languageUIPath = null;
				LanguagePath = value.ToString();
				select = null;
				if (value == 0)
				{
					select = new SelectData();
				}
				else
				{
					List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("localize/info/", true);
					assetBundleNameListFromPath.Sort((string a, string b) => b.CompareTo(a));
					foreach (string item in assetBundleNameListFromPath)
					{
						Info.Param param = AssetBundleManager.LoadAllAsset(item, typeof(Info)).GetAllAssets<Info>().SelectMany((Info p) => p.param)
							.FirstOrDefault((Info.Param p) => p.ID == value);
						AssetBundleManager.UnloadAssetBundle(item, false);
						if (param != null)
						{
							select = new SelectData(param);
							break;
						}
					}
					select = select ?? new SelectData();
				}
			});
			if (!SetLanguage(SetupData.LoadLanguage()) && !Check(LanguageID.US))
			{
				bool flag = SetLanguage(LanguageID.US);
			}
			initialized = true;
		}

		public static Dictionary<int, Dictionary<int, Data.Param>> LoadScene(SCENE_ID sceneID, GameObject o)
		{
			return LoadScene((int)sceneID, o);
		}

		public static Dictionary<int, Dictionary<int, Data.Param>> LoadScene(int sceneID, GameObject o)
		{
			RefCntAdd(sceneID);
			if (o != null)
			{
				o.OnDestroyAsObservable().Subscribe(delegate
				{
					if (RefCntSub(sceneID))
					{
						RemoveScene(sceneID, true);
					}
				});
			}
			Dictionary<int, Dictionary<int, Data.Param>> value;
			if (scene.TryGetValue(sceneID, out value))
			{
				return value;
			}
			value = (scene[sceneID] = new Dictionary<int, Dictionary<int, Data.Param>>());
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(LanguagePath + "type/" + sceneID + "/", true);
			assetBundleNameListFromPath.Sort();
			foreach (string item in assetBundleNameListFromPath)
			{
				Data[] allAssets = AssetBundleManager.LoadAllAsset(item, typeof(Data)).GetAllAssets<Data>();
				Data[] array = allAssets;
				foreach (Data data in array)
				{
					int result;
					if (!int.TryParse(data.name.Split('_')[0], out result))
					{
						continue;
					}
					Dictionary<int, Data.Param> value2;
					if (!value.TryGetValue(result, out value2))
					{
						value2 = (value[result] = new Dictionary<int, Data.Param>());
					}
					foreach (Data.Param item2 in data.param)
					{
						item2.SetBaseManifest(select.BaseManifest);
						value2[item2.ID] = item2;
					}
				}
				AssetBundleManager.UnloadAssetBundle(item, false);
			}
			return value;
		}

		public static bool DisposeScene(SCENE_ID sceneID, bool isForce)
		{
			return DisposeScene((int)sceneID, isForce);
		}

		public static bool DisposeScene(int sceneID, bool isForce)
		{
			if (!scene.ContainsKey(sceneID))
			{
				return true;
			}
			bool flag = false;
			if (isForce)
			{
				return RemoveScene(sceneID, true);
			}
			return RefCntSub(sceneID) && RemoveScene(sceneID, true);
		}

		public static Dictionary<int, Data.Param> GetCategory(SCENE_ID sceneID, int categoryID)
		{
			return GetCategory((int)sceneID, categoryID);
		}

		public static Dictionary<int, Data.Param> GetCategory(int sceneID, int categoryID)
		{
			Dictionary<int, Dictionary<int, Data.Param>> value;
			if (!scene.TryGetValue(sceneID, out value))
			{
			}
			Dictionary<int, Data.Param> value2;
			(value ?? new Dictionary<int, Dictionary<int, Data.Param>>()).TryGetValue(categoryID, out value2);
			return value2 ?? new Dictionary<int, Data.Param>();
		}

		private static UIBehaviour CheckUI(Component target)
		{
			return target as UIBehaviour;
		}

		private static UIBehaviour CheckUI(GameObject target)
		{
			UIBehaviour uIBehaviour = null;
			uIBehaviour = CheckUI(target.GetComponent<Graphic>());
			if (uIBehaviour != null)
			{
				return uIBehaviour;
			}
			uIBehaviour = CheckUI(target.GetComponent<Selectable>());
			if (uIBehaviour != null)
			{
				return uIBehaviour;
			}
			return uIBehaviour;
		}

		public static bool Bind(GameObject target, Data.Param data, bool isUnload)
		{
			if (target == null)
			{
				return false;
			}
			GameObject gameObject;
			switch ((Data.Type)data.type)
			{
			case Data.Type.DestroyObj:
				UnityEngine.Object.Destroy(target);
				return true;
			case Data.Type.DisableObj:
				target.SetActive(false);
				return true;
			case Data.Type.DestroyUI:
				return CheckUI(target).SafeProcObject(delegate(UIBehaviour ui)
				{
					UnityEngine.Object.Destroy(ui);
				});
			case Data.Type.DisableUI:
				return CheckUI(target).SafeProcObject(delegate(UIBehaviour ui)
				{
					ui.enabled = false;
				});
			default:
				{
					if (data.Bundle.IsNullOrEmpty())
					{
						if (Replace(target.GetComponent<TextMeshProUGUI>(), data.text, true))
						{
							return true;
						}
						if (Replace(target.GetComponent<Text>(), data.text, true))
						{
							return true;
						}
						goto IL_03d7;
					}
					UnityEngine.Object @object = data.Load(isUnload);
					if (@object == null)
					{
						return false;
					}
					gameObject = @object as GameObject;
					if (gameObject == null)
					{
						if (Replace(data.text, target.ToArray<TextMeshProUGUI>(new UnityEngine.Object[1] { @object })))
						{
							return true;
						}
						if (Replace(data.text, target.ToArray<Text>(new UnityEngine.Object[1] { @object })))
						{
							return true;
						}
						if (Replace(target.ToArray<Button>(new UnityEngine.Object[1] { @object })))
						{
							return true;
						}
						if (Replace(target.ToArray<Toggle>(new UnityEngine.Object[1] { @object })))
						{
							return true;
						}
						if (Replace(target.ToArray<Selectable>(new UnityEngine.Object[1] { @object })))
						{
							return true;
						}
						if (Replace(true, target.ToArray<Image>(new UnityEngine.Object[1] { @object })))
						{
							return true;
						}
						if (Replace(target.ToArray<RawImage>(new UnityEngine.Object[1] { @object })))
						{
							return true;
						}
						if (Replace(target, Convert(@object)))
						{
							return true;
						}
						if (Replace(target.GetComponent<RawImage>(), @object as Texture))
						{
							return true;
						}
						return false;
					}
					TextMeshProUGUI[] array = ToArray<TextMeshProUGUI>(new GameObject[2] { target, gameObject });
					if (array[0] != null)
					{
						bool flag = Replace(data.text, array);
						UITextSetting component = gameObject.GetComponent<UITextSetting>();
						if (component != null)
						{
							component.Set(array[0]);
							if (component.isOrderOnly)
							{
								return true;
							}
						}
						if (flag)
						{
							if (array[1] == null)
							{
								goto IL_03b8;
							}
							return true;
						}
					}
					Text[] array2 = ToArray<Text>(new GameObject[2] { target, gameObject });
					if (array2[0] != null)
					{
						bool flag2 = Replace(data.text, array2);
						UITextSetting component2 = gameObject.GetComponent<UITextSetting>();
						if (component2 != null)
						{
							component2.Set(array2[0]);
							if (component2.isOrderOnly)
							{
								return true;
							}
						}
						if (flag2)
						{
							if (array2[1] == null)
							{
								goto IL_03b8;
							}
							return true;
						}
					}
					Button[] array3 = ToArray<Button>(new GameObject[2] { target, gameObject });
					if (Replace(array3))
					{
						return true;
					}
					Toggle[] array4 = ToArray<Toggle>(new GameObject[2] { target, gameObject });
					if (Replace(array4))
					{
						return true;
					}
					Selectable[] array5 = ToArray<Selectable>(new GameObject[2] { target, gameObject });
					if (Replace(array5))
					{
						return true;
					}
					Image[] array6 = ToArray<Image>(new GameObject[2] { target, gameObject });
					if (Replace(true, array6))
					{
						return true;
					}
					RawImage[] array7 = ToArray<RawImage>(new GameObject[2] { target, gameObject });
					if (Replace(array7))
					{
						return true;
					}
					goto IL_03b8;
				}
				IL_03b8:
				if (Replace(ToArray<RectTransform>(new GameObject[2] { target, gameObject })))
				{
					return true;
				}
				goto IL_03d7;
				IL_03d7:
				return false;
			}
		}

		public static bool Bind(Component target, Data.Param data, bool isUnload)
		{
			if (target == null)
			{
				return false;
			}
			switch ((Data.Type)data.type)
			{
			case Data.Type.DestroyObj:
				UnityEngine.Object.Destroy(target.gameObject);
				return true;
			case Data.Type.DisableObj:
				target.gameObject.SetActive(false);
				return true;
			case Data.Type.DestroyUI:
				UnityEngine.Object.Destroy(target);
				return true;
			case Data.Type.DisableUI:
				return CheckUI(target).SafeProcObject(delegate(UIBehaviour ui)
				{
					ui.enabled = false;
				});
			default:
				if (data.Bundle.IsNullOrEmpty())
				{
					if (Replace(target as TextMeshProUGUI, data.text, true))
					{
						return true;
					}
					if (Replace(target as Text, data.text, true))
					{
						return true;
					}
				}
				else
				{
					UnityEngine.Object @object = data.Load(isUnload);
					if (@object == null)
					{
						return false;
					}
					GameObject gameObject = @object as GameObject;
					if (gameObject == null)
					{
						if (Replace(data.text, new UnityEngine.Object[2] { target, @object }.ToArray<TextMeshProUGUI>()))
						{
							return true;
						}
						if (Replace(data.text, new UnityEngine.Object[2] { target, @object }.ToArray<Text>()))
						{
							return true;
						}
						if (Replace(new UnityEngine.Object[2] { target, @object }.ToArray<Button>()))
						{
							return true;
						}
						if (Replace(new UnityEngine.Object[2] { target, @object }.ToArray<Toggle>()))
						{
							return true;
						}
						if (Replace(new UnityEngine.Object[2] { target, @object }.ToArray<Selectable>()))
						{
							return true;
						}
						if (Replace(true, new UnityEngine.Object[2] { target, @object }.ToArray<Image>()))
						{
							return true;
						}
						if (Replace(new UnityEngine.Object[2] { target, @object }.ToArray<RawImage>()))
						{
							return true;
						}
						if (Replace(target, Convert(@object)))
						{
							return true;
						}
						if (Replace(target as RawImage, @object as Texture))
						{
							return true;
						}
						return false;
					}
					if (Replace(data.text, target.ToArray<TextMeshProUGUI>(new GameObject[1] { gameObject })))
					{
						return true;
					}
					if (Replace(data.text, target.ToArray<Text>(new GameObject[1] { gameObject })))
					{
						return true;
					}
					if (Replace(target.ToArray<Button>(new GameObject[1] { gameObject })))
					{
						return true;
					}
					if (Replace(target.ToArray<Toggle>(new GameObject[1] { gameObject })))
					{
						return true;
					}
					if (Replace(target.ToArray<Selectable>(new GameObject[1] { gameObject })))
					{
						return true;
					}
					if (Replace(true, target.ToArray<Image>(new GameObject[1] { gameObject })))
					{
						return true;
					}
					if (Replace(target.ToArray<RawImage>(new GameObject[1] { gameObject })))
					{
						return true;
					}
				}
				return false;
			}
		}

		public static void Unload(Data.Param data)
		{
			if (data != null)
			{
				data.ToData().UnloadBundle();
			}
		}

		public static void Unload(IEnumerable<Data.Param> iter)
		{
			foreach (Data.Param item in iter)
			{
				Unload(item);
			}
		}

		public static Sprite Convert(UnityEngine.Object texture)
		{
			return (texture as Sprite) ?? ToSprite(texture as Texture2D);
		}

		private static Sprite ToSprite(Texture2D texture)
		{
			return (!(texture == null)) ? Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero) : null;
		}

		public static string GetScenarioCharaName(string target, string bundle, string asset)
		{
			string text = null;
			List<ScenarioCharaName.Param> value;
			if (ScenarioReplaceNameData.TryGetValue(target, out value))
			{
				foreach (ScenarioCharaName.Param item in value)
				{
					bool flag = false;
					bool flag2 = !item.Bundle.IsNullOrEmpty();
					bool flag3 = !item.Asset.IsNullOrEmpty();
					if ((flag2 && flag3) ? (bundle.IndexOf(item.Bundle) != -1 && asset.IndexOf(item.Asset) != -1) : (flag2 ? (bundle.IndexOf(item.Bundle) != -1) : (!flag3 || asset.IndexOf(item.Asset) != -1)))
					{
						text = item.Replace;
						break;
					}
				}
			}
			return text ?? target;
		}

		private static Dictionary<string, List<ScenarioCharaName.Param>> LoadScenarioCharaNames()
		{
			Dictionary<string, List<ScenarioCharaName.Param>> dictionary = new Dictionary<string, List<ScenarioCharaName.Param>>();
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(LanguagePath + "scenario/charaname/", true);
			assetBundleNameListFromPath.Sort();
			foreach (string item in assetBundleNameListFromPath)
			{
				ScenarioCharaName[] allAssets = AssetBundleManager.LoadAllAsset(item, typeof(ScenarioCharaName)).GetAllAssets<ScenarioCharaName>();
				ScenarioCharaName[] array = allAssets;
				foreach (ScenarioCharaName scenarioCharaName in array)
				{
					foreach (ScenarioCharaName.Param item2 in scenarioCharaName.param)
					{
						if (!item2.Target.IsNullOrEmpty() && !item2.Replace.IsNullOrEmpty())
						{
							List<ScenarioCharaName.Param> value;
							if (!dictionary.TryGetValue(item2.Target, out value))
							{
								value = (dictionary[item2.Target] = new List<ScenarioCharaName.Param>());
							}
							value.Add(item2);
						}
					}
				}
				AssetBundleManager.UnloadAssetBundle(item, false);
			}
			foreach (KeyValuePair<string, List<ScenarioCharaName.Param>> item3 in dictionary)
			{
				item3.Value.Sort((ScenarioCharaName.Param a, ScenarioCharaName.Param b) => a.priority.CompareTo(b.priority));
			}
			return dictionary;
		}

		private static T[] ToArray<T>(params GameObject[] gameObjects) where T : Component
		{
			T[] array = new T[gameObjects.Length];
			for (int i = 0; i < gameObjects.Length; i++)
			{
				array[i] = gameObjects[i].GetComponent<T>();
			}
			return array;
		}

		private static T[] ToArray<T>(this UnityEngine.Object[] self)
		{
			return self.OfType<T>().ToArray();
		}

		private static T[] ToArray<T>(this Component self, params GameObject[] gameObjects) where T : Component
		{
			T[] array = new T[gameObjects.Length + 1];
			array[0] = self as T;
			for (int i = 0; i < gameObjects.Length; i++)
			{
				array[i + 1] = gameObjects[i].GetComponent<T>();
			}
			return array;
		}

		private static T[] ToArray<T>(this GameObject self, params UnityEngine.Object[] objects) where T : UnityEngine.Object
		{
			T[] array = new T[objects.Length + 1];
			array[0] = self.GetComponent<T>();
			for (int i = 0; i < objects.Length; i++)
			{
				array[i + 1] = objects[i] as T;
			}
			return array;
		}

		private static void RefCntAdd(int sceneID)
		{
			int value;
			refCnt.TryGetValue(sceneID, out value);
			value = (refCnt[sceneID] = value + 1);
		}

		private static bool RefCntSub(int sceneID)
		{
			int value;
			if (!refCnt.TryGetValue(sceneID, out value))
			{
				return false;
			}
			value = (refCnt[sceneID] = value - 1);
			if (value > 0)
			{
				return false;
			}
			return true;
		}

		private static bool RemoveScene(int sceneID, bool isForce)
		{
			if ((refCnt.Remove(sceneID) || isForce) && scene.Remove(sceneID))
			{
				return true;
			}
			return false;
		}

		public static void SetCultureJP(Action action)
		{
			using (new CultureJPScope())
			{
				action();
			}
		}

		public static void SetCulture(Action action)
		{
			using (new CultureScope(select.Culture))
			{
				action();
			}
		}

		public static ChaFileInfo[] CreateChaFileInfo(int sex, bool useDefaultData)
		{
			return (from p in DefaultData.UserDataAssist((sex != 0) ? "chara/female/" : "chara/male/", useDefaultData).Select(delegate(FileInfo file)
				{
					ChaFileControl chaFileControl = new ChaFileControl();
					bool flag = chaFileControl.LoadCharaFile(file.info.FullPath);
					if (!flag)
					{
					}
					return new
					{
						file = file,
						data = chaFileControl,
						success = (flag && chaFileControl.parameter.sex == sex)
					};
				})
				where p.success
				select new ChaFileInfo(p.data, p.file)).ToArray();
		}

		public static ChaCoordinateInfo[] CreateCoordinateInfo(bool useDefaultData)
		{
			return (from p in DefaultData.UserDataAssist("coordinate/", useDefaultData).Select(delegate(FileInfo file)
				{
					ChaFileCoordinate chaFileCoordinate = new ChaFileCoordinate();
					bool flag = chaFileCoordinate.LoadFile(file.info.FullPath);
					if (!flag)
					{
					}
					return new
					{
						file = file,
						data = chaFileCoordinate,
						success = flag
					};
				})
				where p.success
				select new ChaCoordinateInfo(p.data, p.file)).ToArray();
		}

		public static ChaFileControl[] GetRandomDefaultDataFemaleCard(int num)
		{
			return GetRandomFemaleCard(num, DefaultData.GetPath("chara/female/"));
		}

		public static ChaFileControl[] GetRandomUserDataFemaleCard(int num)
		{
			return GetRandomFemaleCard(num, UserData.Path + "chara/female/");
		}

		private static ChaFileControl[] GetRandomFemaleCard(int num, string path)
		{
			if (path.IsNullOrEmpty())
			{
				return null;
			}
			if (num <= 0)
			{
				return null;
			}
			return (from p in (FolderAssist.CreateFolderInfoExToArray(path, "*.png") ?? Enumerable.Empty<FolderAssist.FileInfo>()).OrderBy((FolderAssist.FileInfo _) => Guid.NewGuid()).Take(num).Select(delegate(FolderAssist.FileInfo file)
				{
					ChaFileControl chaFileControl = new ChaFileControl();
					bool flag = chaFileControl.LoadCharaFile(file.FullPath, 1, true);
					if (!flag)
					{
					}
					return new
					{
						data = chaFileControl,
						success = (flag && chaFileControl.parameter.sex == 1)
					};
				})
				where p.success
				select p.data).ToArray();
		}

		public static bool BindFont(TMP_Text text)
		{
			if (select.fontData.Set(text))
			{
				return true;
			}
			return false;
		}

		public static bool BindFont(Text text)
		{
			if (select.fontData.Set(text))
			{
				return true;
			}
			return false;
		}

		public static bool BindFont(GameObject target)
		{
			if (target.GetComponent<TMP_Text>().SafeProcObject(delegate(TMP_Text text)
			{
				BindFont(text);
			}))
			{
				return true;
			}
			if (target.GetComponent<Text>().SafeProcObject(delegate(Text text)
			{
				BindFont(text);
			}))
			{
				return true;
			}
			return false;
		}

		public static bool Replace(string overrideText, params Text[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			Text text = array[0];
			if (text == null)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = !overrideText.IsNullOrEmpty();
			bool useFont = true;
			for (int i = 1; i < array.Length; i++)
			{
				Text text2 = array[i];
				if (!(text2 == null))
				{
					Replace(text.GetComponent<RectTransform>(), text2.GetComponent<RectTransform>());
					if (text2.font != null)
					{
						text.font = text2.font;
						useFont = false;
					}
					text.fontStyle = text2.fontStyle;
					text.fontSize = text2.fontSize;
					text.lineSpacing = text2.lineSpacing;
					text.supportRichText = text2.supportRichText;
					text.alignment = text2.alignment;
					text.alignByGeometry = text2.alignByGeometry;
					text.horizontalOverflow = text2.horizontalOverflow;
					text.verticalOverflow = text2.verticalOverflow;
					text.resizeTextForBestFit = text2.resizeTextForBestFit;
					text.color = text2.color;
					if (!flag2)
					{
						flag |= Replace(text, text2.text, false);
					}
				}
			}
			return flag || Replace(text, overrideText, useFont);
		}

		public static bool Replace(Text my, string text, bool useFont)
		{
			if (my == null)
			{
				return false;
			}
			if (useFont)
			{
				BindFont(my);
			}
			if (text.IsNullOrEmpty())
			{
				return false;
			}
			my.text = text;
			return true;
		}

		public static bool Replace(string overrideText, params TMP_Text[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			TMP_Text tMP_Text = array[0];
			if (tMP_Text == null)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = !overrideText.IsNullOrEmpty();
			bool useFont = true;
			for (int i = 1; i < array.Length; i++)
			{
				TMP_Text tMP_Text2 = array[i];
				if (!(tMP_Text2 == null))
				{
					Replace(tMP_Text.GetComponent<RectTransform>(), tMP_Text2.GetComponent<RectTransform>());
					if (tMP_Text2.font != null)
					{
						tMP_Text.font = tMP_Text2.font;
						tMP_Text.fontSharedMaterial = tMP_Text2.fontSharedMaterial;
						useFont = false;
					}
					tMP_Text.fontSize = tMP_Text2.fontSize;
					tMP_Text.enableAutoSizing = tMP_Text2.enableAutoSizing;
					tMP_Text.fontSizeMin = tMP_Text2.fontSizeMin;
					tMP_Text.fontSizeMax = tMP_Text2.fontSizeMax;
					tMP_Text.alignment = tMP_Text2.alignment;
					if (!flag2)
					{
						flag |= Replace(tMP_Text, tMP_Text2.text, false);
					}
				}
			}
			return flag || Replace(tMP_Text, overrideText, useFont);
		}

		public static bool Replace(TMP_Text my, string text, bool useFont)
		{
			if (my == null)
			{
				return false;
			}
			if (useFont)
			{
				BindFont(my);
			}
			if (text.IsNullOrEmpty())
			{
				return false;
			}
			my.text = text;
			return true;
		}

		public static bool Replace(params Button[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			Button button = array[0];
			if (button == null)
			{
				return false;
			}
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				Button button2 = array[i];
				if (!(button2 == null))
				{
					if (button.targetGraphic != button.image && button2.targetGraphic != button2.image)
					{
						Replace(true, button.targetGraphic as Image, button2.targetGraphic as Image);
					}
					flag = true;
				}
			}
			return flag && Replace(array.OfType<Selectable>().ToArray());
		}

		public static bool Replace(params Toggle[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			Toggle toggle = array[0];
			if (toggle == null)
			{
				return false;
			}
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				Toggle toggle2 = array[i];
				if (!(toggle2 == null))
				{
					if (toggle.targetGraphic != toggle.image && toggle2.targetGraphic != toggle2.image)
					{
						Replace(true, toggle.targetGraphic as Image, toggle2.targetGraphic as Image);
					}
					Replace(true, toggle.graphic as Image, toggle2.graphic as Image);
					flag = true;
				}
			}
			return flag && Replace(array.OfType<Selectable>().ToArray());
		}

		public static bool Replace(params Selectable[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			Selectable selectable = array[0];
			if (selectable == null)
			{
				return false;
			}
			bool result = false;
			for (int i = 1; i < array.Length; i++)
			{
				Selectable selectable2 = array[i];
				if (!(selectable2 == null))
				{
					bool flag = false;
					if (selectable.image != null && selectable2.image != null)
					{
						flag = selectable.transform == selectable.image.transform && selectable2.transform == selectable2.image.transform;
					}
					Replace(flag, selectable.image, selectable2.image);
					if (!flag)
					{
						Replace(selectable.GetComponent<RectTransform>(), selectable2.GetComponent<RectTransform>());
					}
					switch (selectable.transition)
					{
					case Selectable.Transition.ColorTint:
						selectable.colors = selectable2.colors;
						break;
					case Selectable.Transition.SpriteSwap:
						selectable.spriteState = selectable2.spriteState;
						break;
					case Selectable.Transition.Animation:
						selectable.animationTriggers = selectable2.animationTriggers;
						break;
					}
					result = true;
				}
			}
			return result;
		}

		public static bool Replace(GameObject target, Sprite sprite)
		{
			if (sprite == null)
			{
				return false;
			}
			if (Replace(target.GetComponent<Selectable>(), sprite))
			{
				return true;
			}
			return Replace(target.GetComponent<Image>(), sprite);
		}

		public static bool Replace(Component target, Sprite sprite)
		{
			if (Replace(target as Selectable, sprite))
			{
				return true;
			}
			return Replace(target as Image, sprite);
		}

		public static bool Replace(Selectable sel, Sprite sprite)
		{
			return !(sel == null) && Replace(sel.image, sprite);
		}

		public static bool Replace(RectTransform my, RectTransform ta)
		{
			if (my == null || ta == null)
			{
				return false;
			}
			my.pivot = ta.pivot;
			my.anchorMin = ta.anchorMin;
			my.anchorMax = ta.anchorMax;
			my.anchoredPosition = ta.anchoredPosition;
			my.sizeDelta = ta.sizeDelta;
			return true;
		}

		public static bool Replace(params RectTransform[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			RectTransform rectTransform = array[0];
			if (rectTransform == null)
			{
				return false;
			}
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				RectTransform rectTransform2 = array[i];
				if (!(rectTransform2 == null))
				{
					flag |= Replace(rectTransform, rectTransform2);
				}
			}
			return flag;
		}

		public static bool Replace(bool isCopyRect, params Image[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			Image image = array[0];
			if (image == null)
			{
				return false;
			}
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				Image image2 = array[i];
				if (!(image2 == null))
				{
					if (isCopyRect)
					{
						Replace(image.GetComponent<RectTransform>(), image2.GetComponent<RectTransform>());
					}
					flag |= Replace(image, image2.sprite);
				}
			}
			return flag;
		}

		public static bool Replace(Image my, Sprite sprite)
		{
			if (my == null || sprite == null)
			{
				return false;
			}
			my.sprite = sprite;
			return true;
		}

		public static bool Replace(params RawImage[] array)
		{
			if (array.Length == 0)
			{
				return false;
			}
			RawImage rawImage = array[0];
			if (rawImage == null)
			{
				return false;
			}
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				RawImage rawImage2 = array[i];
				if (!(rawImage2 == null))
				{
					Replace(rawImage.GetComponent<RectTransform>(), rawImage2.GetComponent<RectTransform>());
					flag |= Replace(rawImage, rawImage2.texture);
				}
			}
			return flag;
		}

		public static bool Replace(RawImage my, Texture texture)
		{
			if (my == null || texture == null)
			{
				return false;
			}
			my.texture = texture;
			return true;
		}
	}
}
