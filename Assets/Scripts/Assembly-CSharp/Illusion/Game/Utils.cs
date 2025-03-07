using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ActionGame;
using ActionGame.MapSound;
using Illusion.Extensions;
using Localize.Translate;
using Manager;
using RootMotion.FinalIK;
using Sound;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.RadarChart;

namespace Illusion.Game
{
	public static class Utils
	{
		public static class IKLoader
		{
			public static void Execute(FullBodyBipedIK ik, List<List<string>> dataList)
			{
				Transform[] componentsInChildren = ik.GetComponentsInChildren<Transform>(true);
				int num = 0;
				List<string> list = dataList[num++];
				int num2 = 0;
				ik.solver.IKPositionWeight = float.Parse(list[num2++]);
				ik.solver.iterations = int.Parse(list[num2++]);
				num2 = 0;
				foreach (List<string> item in dataList.Skip(num))
				{
					if (ik.solver.effectors.Length <= num2)
					{
						break;
					}
					num2++;
					int num3 = 0;
					IKEffector eff = ik.solver.GetEffector(Illusion.Utils.Enum<FullBodyBipedEffector>.Cast(item[num3++]));
					if (eff == null)
					{
						continue;
					}
					eff.positionWeight = float.Parse(item[num3++]);
					eff.rotationWeight = float.Parse(item[num3++]);
					string findFrame = item[num3++];
					if (findFrame == "null")
					{
						eff.target = null;
					}
					else
					{
						componentsInChildren.FirstOrDefault((Transform p) => p.name == findFrame).SafeProc(delegate(Transform frame)
						{
							eff.target = frame;
						});
					}
					if (eff.target != null)
					{
						eff.target.localPosition = item[num3++].GetVector3();
						eff.target.localEulerAngles = item[num3++].GetVector3();
					}
				}
				num += num2;
				num2 = 0;
				foreach (List<string> item2 in dataList.Skip(num))
				{
					if (ik.solver.chain.Length <= num2)
					{
						break;
					}
					FBIKChain fBIKChain = ik.solver.chain[num2++];
					int num4 = 0;
					fBIKChain.pull = float.Parse(item2[num4++]);
					fBIKChain.reach = float.Parse(item2[num4++]);
					fBIKChain.push = float.Parse(item2[num4++]);
					fBIKChain.pushParent = float.Parse(item2[num4++]);
					fBIKChain.reachSmoothing = Illusion.Utils.Enum<FBIKChain.Smoothing>.Cast(item2[num4++]);
					fBIKChain.pushSmoothing = Illusion.Utils.Enum<FBIKChain.Smoothing>.Cast(item2[num4++]);
					fBIKChain.bendConstraint.weight = float.Parse(item2[num4++]);
					string findFrame2 = item2[num4++];
					if (findFrame2 == "null")
					{
						fBIKChain.bendConstraint.bendGoal = null;
					}
					else
					{
						fBIKChain.bendConstraint.bendGoal = componentsInChildren.FirstOrDefault((Transform p) => p.name == findFrame2);
					}
					if (fBIKChain.bendConstraint.bendGoal != null)
					{
						fBIKChain.bendConstraint.bendGoal.localPosition = item2[num4++].GetVector3();
						fBIKChain.bendConstraint.bendGoal.localEulerAngles = item2[num4++].GetVector3();
					}
				}
			}
		}

		public static class RadarGraph
		{
			private const string NodeDataName = "nodeParent";

			public static void SetLabel(WMG_Radar_Graph graph, IEnumerable<string> labels)
			{
				graph.labelStrings.Clear();
				foreach (string label in labels)
				{
					graph.labelStrings.Add(label);
				}
			}

			public static void SetGraph(WMG_Radar_Graph graph, int pointNum, float minValue, float maxValue, int gridNum)
			{
				graph.randomData = false;
				graph.numPoints = pointNum;
				graph.radarMinVal = minValue;
				graph.radarMaxVal = maxValue;
				graph.numGrids = gridNum;
			}

			public static List<Vector2> Offset(WMG_Radar_Graph graph, List<float> statusData, WMG_Series series, Vector3? offset = null)
			{
				if (!offset.HasValue)
				{
					offset = new Vector3(graph.offset.x, graph.offset.y, graph.degreeOffset);
				}
				series.pointValues.SetList(graph.GenRadar(statusData, offset.Value.x, offset.Value.y, offset.Value.z));
				return graph.GenRadar(statusData, offset.Value.x, offset.Value.y, offset.Value.z);
			}

			public static void SetSeries(WMG_Series series, bool isPointDraw, Color lineColor)
			{
				series.connectFirstToLast = true;
				series.hidePoints = !isPointDraw;
				series.lineColor = lineColor;
			}

			public static FillDrawer CreateFill(WMG_Radar_Graph graph, WMG_Series series, Color color, string objName = null)
			{
				FillDrawer fillDrawer = new GameObject(objName).AddComponent<FillDrawer>();
				Transform transform = fillDrawer.transform;
				transform.SetParent(graph.transform, false);
				transform.position = series.transform.position;
				Vector3 localPosition = transform.localPosition;
				List<Vector2> list = new List<Vector2>();
				Transform transform2 = series.transform.Find("nodeParent");
				for (int i = 0; i < transform2.childCount; i++)
				{
					list.Add(transform2.GetChild(i).localPosition + localPosition);
				}
				list.Add(graph.offset * 0.5f);
				fillDrawer.transform.localPosition = Vector2.zero;
				fillDrawer.color = color;
				fillDrawer.status.points = list.ToArray();
				return fillDrawer;
			}

			public static LineDrawer CreateLine(WMG_Radar_Graph graph, WMG_Series series, Color color, Vector2? pos = null, Vector2? size = null, string objName = null)
			{
				Vector3 zero = Vector3.zero;
				LineDrawer lineDrawer = new GameObject(objName).AddComponent<LineDrawer>();
				Transform transform = lineDrawer.transform;
				transform.SetParent(graph.transform, false);
				transform.position = series.transform.position;
				transform.SetSiblingIndex(series.transform.parent.GetSiblingIndex());
				lineDrawer.transform.localPosition = zero;
				lineDrawer.color = color;
				lineDrawer.status.segment = series.transform.Find("nodeParent").childCount;
				lineDrawer.status.point = zero;
				if (pos.HasValue)
				{
					lineDrawer.rectTransform.localPosition = pos.Value;
				}
				if (size.HasValue)
				{
					lineDrawer.rectTransform.sizeDelta = size.Value;
				}
				return lineDrawer;
			}
		}

		public static class Scene
		{
			public static class Check
			{
				private const string sceneName = "Check";

				public static IEnumerator Load(CheckScene.Parameter param, IObserver<CheckScene> observer)
				{
					yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Manager.Scene>.Instance.LoadStart(new Manager.Scene.Data
					{
						levelName = "Check",
						isAdd = true,
						isOverlap = true,
						onLoad = delegate
						{
							CheckScene rootComponent = Manager.Scene.GetRootComponent<CheckScene>("Check");
							if (!(rootComponent == null))
							{
								rootComponent.Set(param);
								observer.OnNext(rootComponent);
							}
						}
					}, false)).StartAsCoroutine();
					observer.OnCompleted();
				}

				public static IEnumerator Load(CheckScene.Parameter param, Action<CheckScene> act)
				{
					yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Manager.Scene>.Instance.LoadStart(new Manager.Scene.Data
					{
						levelName = "Check",
						isAdd = true,
						isOverlap = true,
						onLoad = delegate
						{
							CheckScene rootComponent = Manager.Scene.GetRootComponent<CheckScene>("Check");
							if (!(rootComponent == null))
							{
								rootComponent.Set(param);
								act.Call(rootComponent);
							}
						}
					}, false)).StartAsCoroutine();
				}
			}

			public static bool IsFadeOutOK
			{
				get
				{
					if (Manager.Scene.isReturnTitle || Manager.Scene.isGameEnd)
					{
						return false;
					}
					SceneFade sceneFade = Singleton<Manager.Scene>.Instance.sceneFade;
					if (sceneFade._Fade == SimpleFade.Fade.In)
					{
						return true;
					}
					if (sceneFade.IsEnd)
					{
						return true;
					}
					return false;
				}
			}

			public static void GameEnd(bool isCheck = true)
			{
				Singleton<Manager.Scene>.Instance.GameEnd(isCheck);
			}

			public static IEnumerator ReturnTitle(bool skipCheck = false, bool isForce = false)
			{
				string sceneName = "Title";
				bool isBaseTitle = Singleton<Manager.Scene>.Instance.LoadSceneName == sceneName;
				if (isBaseTitle)
				{
					skipCheck = true;
				}
				Action act = delegate
				{
					if (isBaseTitle)
					{
						Singleton<Manager.Scene>.Instance.UnloadAddScene();
					}
					else
					{
						Manager.Scene.isReturnTitle = true;
						Observable.FromCoroutine((CancellationToken _) => Singleton<Manager.Scene>.Instance.UnLoad(sceneName, delegate(bool isFind)
						{
							if (!isFind)
							{
								Singleton<Manager.Scene>.Instance.LoadReserve(new Manager.Scene.Data
								{
									levelName = sceneName,
									isAsync = true,
									isFade = true
								}, true);
							}
						})).StartAsCoroutine();
					}
				};
				if (!skipCheck || isForce)
				{
					CheckScene.Parameter param = new CheckScene.Parameter();
					param.Title = Localize.Translate.Manager.OtherData.Get(100).Values.FindTagText("ReturnTitle") ?? "タイトルへ戻りますか？";
					param.Yes = delegate
					{
						act();
					};
					param.No = delegate
					{
						Observable.NextFrame().Subscribe(delegate
						{
							Singleton<Manager.Scene>.Instance.UnLoad();
						});
					};
					param.isYesFocus = false;
					yield return Observable.FromCoroutine((IObserver<CheckScene> observer) => Check.Load(param, observer)).StartAsCoroutine();
				}
				else
				{
					act();
				}
			}

			public static bool IsTutorialRead(int no)
			{
				return Singleton<Manager.Game>.Instance.glSaveData.tutorialHash.Contains(no);
			}

			public static bool OpenTutorial(int no, bool isAll = false)
			{
				if (!isAll && IsTutorialRead(no))
				{
					return false;
				}
				string levelName = "Tutorial";
				Manager.Scene.Data data = new Manager.Scene.Data();
				data.levelName = levelName;
				data.isAdd = true;
				data.isFade = false;
				data.onLoad = delegate
				{
					Tutorial rootComponent = Manager.Scene.GetRootComponent<Tutorial>(levelName);
					if (!(rootComponent == null))
					{
						rootComponent.nowTutorial = no;
					}
				};
				Manager.Scene.Data data2 = data;
				Singleton<Manager.Scene>.Instance.LoadReserve(data2, false);
				return true;
			}

			public static bool IsTutorial()
			{
				return Singleton<Manager.Scene>.IsInstance() && Singleton<Manager.Scene>.Instance.NowSceneNames.Contains("Tutorial");
			}

			public static Manager.Scene.Data.FadeType SafeFadeIn()
			{
				return (Singleton<Manager.Scene>.Instance.sceneFade._Fade != 0) ? Manager.Scene.Data.FadeType.In : Manager.Scene.Data.FadeType.None;
			}
		}

		public static class UniRx
		{
			public static class FPSCounter
			{
				private const int BufferSize = 5;

				public static IReadOnlyReactiveProperty<float> Current { get; private set; }

				static FPSCounter()
				{
					Current = (from x in (from _ in Observable.EveryUpdate()
							select Time.deltaTime).Buffer(5, 1)
						select 1f / x.Average()).ToReadOnlyReactiveProperty();
				}
			}

			public static void FixPerspectiveObject<T>(T o, Camera camera) where T : UnityEngine.Component
			{
				Transform transform = o.transform;
				Func<float> distance = () => (transform.position - camera.transform.position).magnitude;
				Vector3 baseScale = transform.localScale / distance();
				(from _ in o.UpdateAsObservable().TakeWhile((Unit _) => camera != null)
					select baseScale * distance()).Subscribe(delegate(Vector3 scale)
				{
					transform.localScale = scale;
				});
			}
		}

		public static class Bundle
		{
			public static void LoadSprite(string assetBundleName, string assetName, Image image, bool isTexSize, string spAnimeName = null)
			{
				AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleName, assetName, typeof(Sprite));
				Sprite sprite = assetBundleLoadAssetOperation.GetAsset<Sprite>();
				if (sprite == null)
				{
					Texture2D asset = assetBundleLoadAssetOperation.GetAsset<Texture2D>();
					sprite = Sprite.Create(asset, new Rect(0f, 0f, asset.width, asset.height), Vector2.zero);
				}
				image.sprite = sprite;
				RectTransform rectTransform = image.rectTransform;
				Vector2 vector = ((!isTexSize) ? rectTransform.sizeDelta : new Vector2(sprite.rect.width, sprite.rect.height));
				if (!spAnimeName.IsNullOrEmpty())
				{
					Animator component = image.GetComponent<Animator>();
					component.enabled = true;
					component.runtimeAnimatorController = AssetBundleManager.LoadAsset(assetBundleName, spAnimeName, typeof(RuntimeAnimatorController)).GetAsset<RuntimeAnimatorController>();
					Func<float, float, float> func = (float x, float y) => x / y;
					Func<float, float, bool> func2 = (float a, float b) => a > b && Mathf.FloorToInt(a - 1f) > 0;
					float num = func(vector.x, vector.y);
					float num2 = func(vector.y, vector.x);
					if (func2(num, num2))
					{
						rectTransform.sizeDelta = new Vector2(vector.y, vector.y);
					}
					else if (func2(num2, num))
					{
						rectTransform.sizeDelta = new Vector2(vector.x, vector.x);
					}
					else
					{
						rectTransform.sizeDelta = new Vector2(vector.x, vector.y);
					}
					AssetBundleManager.UnloadAssetBundle(assetBundleName, false);
				}
				else
				{
					rectTransform.sizeDelta = new Vector2(vector.x, vector.y);
				}
				AssetBundleManager.UnloadAssetBundle(assetBundleName, false);
			}
		}

		public static class Layout
		{
			public class EnabledScope : GUI.Scope
			{
				private readonly bool enabled;

				public EnabledScope()
				{
					enabled = GUI.enabled;
				}

				public EnabledScope(bool enabled)
				{
					this.enabled = GUI.enabled;
					GUI.enabled = enabled;
				}

				protected override void CloseScope()
				{
					GUI.enabled = enabled;
				}
			}

			public class ColorScope : GUI.Scope
			{
				private readonly Color[] colors;

				public ColorScope()
				{
					colors = new Color[3]
					{
						GUI.color,
						GUI.backgroundColor,
						GUI.contentColor
					};
				}

				public ColorScope(params Color[] colors)
				{
					this.colors = new Color[3]
					{
						GUI.color,
						GUI.backgroundColor,
						GUI.contentColor
					};
					foreach (var item in colors.Take(this.colors.Length).Select((Color color, int index) => new { color, index }))
					{
						switch (item.index)
						{
						case 0:
							GUI.color = item.color;
							break;
						case 1:
							GUI.backgroundColor = item.color;
							break;
						case 2:
							GUI.contentColor = item.color;
							break;
						}
					}
				}

				public ColorScope(Colors colors)
				{
					this.colors = new Color[3]
					{
						GUI.color,
						GUI.backgroundColor,
						GUI.contentColor
					};
					if (colors.color.HasValue)
					{
						GUI.color = colors.color.Value;
					}
					if (colors.backgroundColor.HasValue)
					{
						GUI.backgroundColor = colors.backgroundColor.Value;
					}
					if (colors.contentColor.HasValue)
					{
						GUI.contentColor = colors.contentColor.Value;
					}
				}

				protected override void CloseScope()
				{
					int num = 0;
					GUI.color = colors[num++];
					GUI.backgroundColor = colors[num++];
					GUI.contentColor = colors[num++];
				}
			}
		}

		public static class ScreenShot
		{
			public static string Path
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder(256);
					stringBuilder.Append(UserData.Create("cap"));
					DateTime now = DateTime.Now;
					stringBuilder.Append(now.Year.ToString("0000"));
					stringBuilder.Append(now.Month.ToString("00"));
					stringBuilder.Append(now.Day.ToString("00"));
					stringBuilder.Append(now.Hour.ToString("00"));
					stringBuilder.Append(now.Minute.ToString("00"));
					stringBuilder.Append(now.Second.ToString("00"));
					stringBuilder.Append(now.Millisecond.ToString("000"));
					stringBuilder.Append(".png");
					return stringBuilder.ToString();
				}
			}

			public static IEnumerator CaptureGSS(List<ScreenShotCamera> ssCamList, string path, Texture capMark, int capRate = 1)
			{
				if (ssCamList.IsNullOrEmpty() || path.IsNullOrEmpty())
				{
					yield break;
				}
				yield return new WaitForEndOfFrame();
				Action<RenderTexture> shotProc = delegate(RenderTexture rt)
				{
					Graphics.Blit(ssCamList[0].renderTexture, rt);
					foreach (ScreenShotCamera item in ssCamList.Skip(1))
					{
						Graphics.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), item.renderTexture);
					}
					if (capMark != null)
					{
						DrawCapMark(capMark, null);
					}
				};
				Capture(shotProc, path, capRate);
				yield return null;
			}

			public static IEnumerator CaptureCameras(List<Camera> cameraList, string path, Texture capMark, int capRate = 1)
			{
				yield return new WaitForEndOfFrame();
				Action<RenderTexture> shotProc = delegate(RenderTexture rt)
				{
					Graphics.SetRenderTarget(rt);
					GL.Clear(true, true, Color.black);
					Graphics.SetRenderTarget(null);
					foreach (Camera item in cameraList.Where((Camera p) => p != null))
					{
						bool enabled = item.enabled;
						RenderTexture targetTexture = item.targetTexture;
						Rect rect = item.rect;
						item.enabled = true;
						item.targetTexture = rt;
						item.Render();
						item.targetTexture = targetTexture;
						item.rect = rect;
						item.enabled = enabled;
					}
					if (capMark != null)
					{
						Graphics.SetRenderTarget(rt);
						DrawCapMark(capMark, null);
						Graphics.SetRenderTarget(null);
					}
				};
				Capture(shotProc, path, capRate);
				yield return null;
			}

			public static void Capture(Action<RenderTexture> proc, string path, int capRate = 1)
			{
				int num = ((capRate == 0) ? 1 : capRate);
				Texture2D texture2D = new Texture2D(Screen.width * num, Screen.height * num, TextureFormat.RGB24, false);
				RenderTexture temporary = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, (QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing);
				proc(temporary);
				RenderTexture.active = temporary;
				texture2D.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
				texture2D.Apply();
				RenderTexture.active = null;
				byte[] bytes = texture2D.EncodeToPNG();
				RenderTexture.ReleaseTemporary(temporary);
				UnityEngine.Object.Destroy(texture2D);
				texture2D = null;
				File.WriteAllBytes(path, bytes);
			}

			public static void DrawCapMark(Texture tex, Vector2? pos)
			{
				float num = (float)Screen.width / 1280f;
				if (!pos.HasValue)
				{
					pos = new Vector2(1152f, 688f);
				}
				Graphics.DrawTexture(new Rect(pos.Value.x * num, pos.Value.y * num, (float)tex.width * num, (float)tex.height * num), tex);
			}
		}

		public static class Sound
		{
			public class SettingBGM : Setting
			{
				private string _assetBundleName;

				private string _assetName;

				public override string assetBundleName
				{
					get
					{
						return _assetBundleName;
					}
					set
					{
						_assetBundleName = value;
						_assetName = Path.GetFileNameWithoutExtension(value);
					}
				}

				public override string assetName
				{
					get
					{
						return _assetName;
					}
					set
					{
						_assetName = value;
					}
				}

				public SettingBGM()
				{
					Initialize();
				}

				public SettingBGM(int bgmNo)
				{
					Setting(Convert(bgmNo));
				}

				public SettingBGM(BGM bgm)
				{
					Setting(Convert((int)bgm));
				}

				public SettingBGM(string assetBundleName)
				{
					Setting(assetBundleName);
				}

				private void Setting(string assetBundleName)
				{
					this.assetBundleName = assetBundleName;
					Initialize();
				}

				private string Convert(int bgmNo)
				{
					return string.Format("sound/data/bgm/bgm_{0:00}{1}", bgmNo, ".unity3d");
				}

				private void Initialize()
				{
					type = Manager.Sound.Type.BGM;
					fadeTime = 0.8f;
					isAssetEqualPlay = false;
					isBundleUnload = true;
				}
			}

			public class Setting
			{
				public Manager.Sound.Type type;

				public float delayTime;

				public float fadeTime;

				public bool isAssetEqualPlay = true;

				public bool isAsync = true;

				public int settingNo = -1;

				public bool isBundleUnload;

				public virtual string assetBundleName { get; set; }

				public virtual string assetName { get; set; }

				public Setting()
				{
				}

				public Setting(SystemSE se)
				{
					Cast(Manager.Sound.Type.SystemSE);
					assetName = SystemSECast[se];
				}

				public Setting(Manager.Sound.Type type)
				{
					Cast(type);
				}

				public void Cast(Manager.Sound.Type type)
				{
					this.type = type;
					assetBundleName = SoundBasePath[this.type];
				}
			}

			private class SystemSEComparer : IEqualityComparer<SystemSE>
			{
				public bool Equals(SystemSE x, SystemSE y)
				{
					return x == y;
				}

				public int GetHashCode(SystemSE obj)
				{
					return (int)obj;
				}
			}

			private class SoundTypeComparer : IEqualityComparer<Manager.Sound.Type>
			{
				public bool Equals(Manager.Sound.Type x, Manager.Sound.Type y)
				{
					return x == y;
				}

				public int GetHashCode(Manager.Sound.Type obj)
				{
					return (int)obj;
				}
			}

			private static Dictionary<int, FootStepSE> _footStepAreaTypes = new Dictionary<int, FootStepSE>();

			private static bool _initializedSoundInfo = false;

			public static readonly Dictionary<SystemSE, string> SystemSECast = new Dictionary<SystemSE, string>(new SystemSEComparer())
			{
				{
					SystemSE.sel,
					"sse_00_01"
				},
				{
					SystemSE.ok_s,
					"sse_00_02"
				},
				{
					SystemSE.ok_l,
					"sse_00_03"
				},
				{
					SystemSE.cancel,
					"sse_00_04"
				},
				{
					SystemSE.photo,
					"sse_00_05"
				},
				{
					SystemSE.title,
					"se_06_title"
				},
				{
					SystemSE.ok_s2,
					"se_07_button_A"
				},
				{
					SystemSE.window_o,
					"se_08_window_B"
				},
				{
					SystemSE.save,
					"se_09_save_A"
				},
				{
					SystemSE.result_single,
					"result_00"
				},
				{
					SystemSE.result_gauge,
					"result_01"
				},
				{
					SystemSE.result_end,
					"result_02"
				}
			};

			public static readonly Dictionary<Manager.Sound.Type, string> SoundBasePath = new Dictionary<Manager.Sound.Type, string>(new SoundTypeComparer())
			{
				{
					Manager.Sound.Type.BGM,
					"sound/data/bgm/00.unity3d"
				},
				{
					Manager.Sound.Type.ENV,
					"sound/data/env/00.unity3d"
				},
				{
					Manager.Sound.Type.GameSE2D,
					"sound/data/se/00.unity3d"
				},
				{
					Manager.Sound.Type.GameSE3D,
					"sound/data/se/00.unity3d"
				},
				{
					Manager.Sound.Type.SystemSE,
					"sound/data/systemse/00.unity3d"
				}
			};

			public static FootStepSE FootStepAreaDefaultType { get; set; }

			public static Dictionary<int, FootStepSE> FootStepAreaTypes
			{
				get
				{
					return _footStepAreaTypes;
				}
			}

			public static Action<int, Transform> FootStepPlayCall { get; set; }

			public static Func<Transform, int, bool, Threshold, AudioSource> SEPlayCall { get; set; }

			public static Action<AudioSource> SEStopCall { get; set; }

			public static Dictionary<int, BGMArea.PlayInfo> MapBGMTable { get; set; }

			public static Dictionary<string, GameObject> EnvAreaTable { get; set; }

			public static Dictionary<int, AudioClip> EnvClipTable { get; set; }

			public static Dictionary<int, AudioClip> SEClipTable { get; set; }

			public static LoadSound GetBGM()
			{
				if (Singleton<Manager.Sound>.IsInstance() && Singleton<Manager.Sound>.Instance.currentBGM != null)
				{
					return Singleton<Manager.Sound>.Instance.currentBGM.GetComponentInChildren<LoadSound>();
				}
				return null;
			}

			public static AudioSource Get(Manager.Sound.Type type, AssetBundleData data)
			{
				return Singleton<Manager.Sound>.IsInstance() ? Singleton<Manager.Sound>.Instance.CreateCache(type, data) : null;
			}

			public static AudioSource Get(Manager.Sound.Type type, AssetBundleManifestData data)
			{
				return Singleton<Manager.Sound>.IsInstance() ? Singleton<Manager.Sound>.Instance.CreateCache(type, data) : null;
			}

			public static AudioSource Get(Manager.Sound.Type type, string bundle, string asset, string manifest = null)
			{
				return Singleton<Manager.Sound>.IsInstance() ? Singleton<Manager.Sound>.Instance.CreateCache(type, bundle, asset, manifest) : null;
			}

			public static void Remove(Manager.Sound.Type type, string bundle, string asset, string manifest = null)
			{
				if (Singleton<Manager.Sound>.IsInstance())
				{
					Singleton<Manager.Sound>.Instance.ReleaseCache(type, bundle, asset, manifest);
				}
			}

			public static AudioSource Get(SystemSE se)
			{
				return Get(Manager.Sound.Type.SystemSE, SoundBasePath[Manager.Sound.Type.SystemSE], SystemSECast[se]);
			}

			public static void Remove(SystemSE se)
			{
				Remove(Manager.Sound.Type.SystemSE, SoundBasePath[Manager.Sound.Type.SystemSE], SystemSECast[se]);
			}

			public static void Play(SystemSE se)
			{
				AudioSource audioSource = Get(se);
				if (!(audioSource == null))
				{
					audioSource.Play();
				}
			}

			public static AudioSource Play(Manager.Sound.Type type, AudioClip clip, float fadeTime = 0f)
			{
				AudioSource audio = Singleton<Manager.Sound>.Instance.Play(type, clip, fadeTime);
				(from __ in audio.UpdateAsObservable()
					where !audio.isPlaying
					select __).Take(1).Subscribe(delegate
				{
					UnityEngine.Object.Destroy(audio.gameObject);
				});
				return audio;
			}

			public static IEnumerator GetBGMandVolume(Action<string, float> bgmAndVolume)
			{
				string bgm = string.Empty;
				float volume = 1f;
				LoadSound now = GetBGM();
				if (now != null && now.audioSource != null)
				{
					FadePlayer fadePlayer = now.audioSource.GetComponent<FadePlayer>();
					while (fadePlayer != null && !(fadePlayer.nowState is FadePlayer.Playing))
					{
						yield return null;
					}
					bgm = now.assetBundleName;
					volume = now.audioSource.volume;
				}
				bgmAndVolume(bgm, volume);
			}

			public static IEnumerator GetFadePlayerWhileNull(string bgm, float volume)
			{
				Transform source = Play(new SettingBGM(bgm)
				{
					isAsync = false
				});
				FadePlayer player = null;
				while (!(source == null))
				{
					player = source.GetComponent<FadePlayer>();
					yield return null;
					if (!(player == null))
					{
						break;
					}
				}
				if (player != null)
				{
					player.currentVolume = volume;
				}
				else
				{
					if (!Singleton<Manager.Sound>.IsInstance() || !(Singleton<Manager.Sound>.Instance.currentBGM != null))
					{
						yield break;
					}
					player = Singleton<Manager.Sound>.Instance.currentBGM.GetComponent<FadePlayer>();
					if (player != null)
					{
						player.currentVolume = volume;
						yield break;
					}
					AudioSource component = Singleton<Manager.Sound>.Instance.currentBGM.GetComponent<AudioSource>();
					if (component != null)
					{
						component.volume = volume;
					}
				}
			}

			public static void InitMapSoundInfo()
			{
				if (!_initializedSoundInfo)
				{
					InitMapBGM();
					InitMapEnv();
					InitEnvClipTable();
					InitSEClipTable();
					_initializedSoundInfo = true;
				}
			}

			private static void InitMapBGM()
			{
				MapBGMTable = new Dictionary<int, BGMArea.PlayInfo>();
				List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/sound/bgm/");
				assetBundleNameListFromPath.Sort();
				for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
				{
					string text = assetBundleNameListFromPath[i];
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
					if (!AssetBundleCheck.IsFile(text, fileNameWithoutExtension))
					{
						continue;
					}
					AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleNameListFromPath[i], fileNameWithoutExtension, typeof(ExcelData));
					if (assetBundleLoadAssetOperation == null)
					{
						continue;
					}
					ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
					if (asset == null)
					{
						continue;
					}
					int maxCell = asset.MaxCell;
					for (int j = 1; j < maxCell; j++)
					{
						List<string> row = asset.GetRow(j);
						int num = 0;
						int result;
						if (int.TryParse(row.ElementAtOrDefault(num++), out result))
						{
							string text2 = row.ElementAtOrDefault(num++);
							int result2;
							bool result3;
							float result4;
							if (int.TryParse(row.ElementAtOrDefault(num++), out result2) && bool.TryParse(row.ElementAtOrDefault(num++), out result3) && float.TryParse(row.ElementAtOrDefault(num++), out result4))
							{
								BGMArea.PlayInfo playInfo = new BGMArea.PlayInfo();
								playInfo.BGMID = result2;
								playInfo.EnableVolumeModification = result3;
								playInfo.Volume = result4;
								BGMArea.PlayInfo value = playInfo;
								MapBGMTable[result] = value;
							}
						}
					}
				}
			}

			private static void InitMapEnv()
			{
				EnvAreaTable = new Dictionary<string, GameObject>();
				List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("map/sound/");
				assetBundleNameListFromPath.Sort();
				for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
				{
					string assetBundleName = assetBundleNameListFromPath[i];
					if (!AssetBundleCheck.IsFile(assetBundleName, string.Empty))
					{
						continue;
					}
					AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAllAsset(assetBundleName, typeof(GameObject));
					if (assetBundleLoadAssetOperation == null)
					{
						continue;
					}
					GameObject[] allAssets = assetBundleLoadAssetOperation.GetAllAssets<GameObject>();
					if (allAssets.IsNullOrEmpty())
					{
						continue;
					}
					GameObject[] array = allAssets;
					foreach (GameObject gameObject in array)
					{
						EnvArea3D component = gameObject.GetComponent<EnvArea3D>();
						if (!(component == null))
						{
							EnvAreaTable[gameObject.name] = gameObject;
						}
					}
				}
			}

			private static void InitEnvClipTable()
			{
				EnvClipTable = new Dictionary<int, AudioClip>();
				List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/sound/se/env/");
				assetBundleNameListFromPath.Sort();
				Regex regex = new Regex("#([a-zA-Z]+)", RegexOptions.IgnoreCase);
				for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
				{
					string text = assetBundleNameListFromPath[i];
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
					if (!AssetBundleCheck.IsFile(text, fileNameWithoutExtension))
					{
						continue;
					}
					AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(text, fileNameWithoutExtension, typeof(ExcelData));
					if (assetBundleLoadAssetOperation == null)
					{
						continue;
					}
					ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
					if (asset == null)
					{
						continue;
					}
					int count = asset.list.Count;
					string text2 = string.Empty;
					for (int j = 1; j < count; j++)
					{
						ExcelData.Param param = asset.list[j];
						int count2 = param.list.Count;
						for (int k = 0; k < count2; k++)
						{
							int num = 0;
							string input = param.list[num++];
							Match match = regex.Match(input);
							if (match.Success)
							{
								text2 = match.Groups[1].Value;
							}
							else
							{
								if (text2 == null || !(text2 == "clip"))
								{
									continue;
								}
								string text3 = param.list[num++];
								string text4 = param.list[num++];
								string text5 = param.list[num++];
								string text6 = param.list[num++];
								string assetBundleName = text4;
								string assetName = text5;
								string manifestName = text6;
								ExcelData excelData = CommonLib.LoadAsset<ExcelData>(assetBundleName, assetName, false, manifestName);
								if (excelData == null)
								{
									continue;
								}
								for (int l = 1; l < excelData.MaxCell; l++)
								{
									List<string> row = excelData.GetRow(l);
									int num2 = 0;
									int result;
									if (int.TryParse(row[num2++], out result))
									{
										string text7 = row[num2++];
										string text8 = row[num2++];
										string text9 = row[num2++];
										string text10 = row[num2++];
										manifestName = text8;
										assetName = text9;
										assetBundleName = text10;
										AudioClip value = CommonLib.LoadAsset<AudioClip>(manifestName, assetName, false, assetBundleName);
										EnvClipTable[result] = value;
									}
								}
							}
						}
					}
				}
			}

			private static void InitSEClipTable()
			{
				SEClipTable = new Dictionary<int, AudioClip>();
				List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/sound/se/action/");
				assetBundleNameListFromPath.Sort();
				for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
				{
					string text = assetBundleNameListFromPath[i];
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
					if (!AssetBundleCheck.IsFile(text, fileNameWithoutExtension))
					{
						continue;
					}
					AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(text, fileNameWithoutExtension, typeof(ExcelData));
					if (assetBundleLoadAssetOperation.IsEmpty())
					{
						AssetBundleManager.UnloadAssetBundle(text, true);
						continue;
					}
					ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
					if (asset == null)
					{
						continue;
					}
					int count = asset.list.Count;
					for (int j = 1; j < count; j++)
					{
						List<string> row = asset.GetRow(j);
						int num = 0;
						int result;
						if (int.TryParse(row.ElementAtOrDefault(num++), out result))
						{
							string text2 = row.ElementAtOrDefault(num++);
							string text3 = row.ElementAtOrDefault(num++);
							string text4 = row.ElementAtOrDefault(num++);
							string text5 = row.ElementAtOrDefault(num++);
							string assetBundleName = text3;
							string assetName = text4;
							string manifestName = text5;
							AudioClip value = CommonLib.LoadAsset<AudioClip>(assetBundleName, assetName, false, manifestName);
							SEClipTable[result] = value;
						}
					}
				}
			}

			public static bool isPlay(SystemSE se)
			{
				return Singleton<Manager.Sound>.IsInstance() && Singleton<Manager.Sound>.Instance.IsPlay(Manager.Sound.Type.SystemSE, SystemSECast[se]);
			}

			public static Transform Play(Setting s)
			{
				return Singleton<Manager.Sound>.IsInstance() ? Singleton<Manager.Sound>.Instance.Play(s.type, s.assetBundleName, s.assetName, s.delayTime, s.fadeTime, s.isAssetEqualPlay, s.isAsync, s.settingNo, s.isBundleUnload) : null;
			}
		}

		public static class Voice
		{
			public class Setting
			{
				public string assetBundleName;

				public string assetName;

				public Manager.Voice.Type type;

				public int no;

				public float pitch = 1f;

				public Transform voiceTrans;

				public float delayTime;

				public float fadeTime;

				public bool isAsync = true;

				public int settingNo = -1;

				public bool isPlayEndDelete = true;

				public bool isBundleUnload;

				public bool is2D;
			}

			public static AudioSource Get(int voiceNo, AssetBundleData data)
			{
				return Singleton<Manager.Voice>.IsInstance() ? Singleton<Manager.Voice>.Instance.CreateCache(voiceNo, data) : null;
			}

			public static AudioSource Get(int voiceNo, AssetBundleManifestData data)
			{
				return Singleton<Manager.Voice>.IsInstance() ? Singleton<Manager.Voice>.Instance.CreateCache(voiceNo, data) : null;
			}

			public static AudioSource Get(int voiceNo, string bundle, string asset, string manifest = null)
			{
				return Singleton<Manager.Voice>.IsInstance() ? Singleton<Manager.Voice>.Instance.CreateCache(voiceNo, bundle, asset, manifest) : null;
			}

			public static void Remove(int voiceNo, string bundle, string asset, string manifest = null)
			{
				if (Singleton<Manager.Voice>.IsInstance())
				{
					Singleton<Manager.Voice>.Instance.ReleaseCache(voiceNo, bundle, asset, manifest);
				}
			}

			public static Transform Play(Setting s)
			{
				return Singleton<Manager.Voice>.IsInstance() ? Singleton<Manager.Voice>.Instance.Play(s.no, s.assetBundleName, s.assetName, s.pitch, s.delayTime, s.fadeTime, s.isAsync, s.voiceTrans, s.type, s.settingNo, s.isPlayEndDelete, s.isBundleUnload, s.is2D) : null;
			}

			public static Transform OnecePlay(Setting s)
			{
				return Singleton<Manager.Voice>.IsInstance() ? Singleton<Manager.Voice>.Instance.OnecePlay(s.no, s.assetBundleName, s.assetName, s.pitch, s.delayTime, s.fadeTime, s.isAsync, s.voiceTrans, s.type, s.settingNo, s.isPlayEndDelete, s.isBundleUnload, s.is2D) : null;
			}

			public static Transform OnecePlayChara(Setting s)
			{
				return Singleton<Manager.Voice>.IsInstance() ? Singleton<Manager.Voice>.Instance.OnecePlayChara(s.no, s.assetBundleName, s.assetName, s.pitch, s.delayTime, s.fadeTime, s.isAsync, s.voiceTrans, s.type, s.settingNo, s.isPlayEndDelete, s.isBundleUnload, s.is2D) : null;
			}
		}
	}
}
