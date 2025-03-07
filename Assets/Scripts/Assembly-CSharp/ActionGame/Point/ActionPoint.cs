using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame.Chara;
using ActionGame.MapObject;
using DG.Tweening;
using Illusion;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame.Point
{
	public class ActionPoint : MonoBehaviour
	{
		public enum Type
		{
			なし = 0,
			部活メニュー = 1,
			部活レポート = 2,
			時間割 = 3,
			覗きトイレ = 4,
			覗きシャワー = 5,
			特殊H = 6,
			寝る = 7,
			寝るv2 = 8
		}

		public int MapNo;

		[SerializeField]
		[Header("種類")]
		private Type type;

		[Header("特殊Hタイプ(Hのカテゴリーのこと)")]
		[SerializeField]
		private List<int> hType = new List<int>();

		[Header("特殊マップ読み込む？")]
		[SerializeField]
		private bool isLoadPeepLoom;

		[Header("Hずらし位置")]
		[SerializeField]
		private Vector3 hOffsetPos;

		[Header("Hずらし回転")]
		[SerializeField]
		private Vector3 hOffsetAngle;

		[SerializeField]
		private Collider col;

		[SerializeField]
		private Transform icon;

		[SerializeField]
		private VisibleChange visibleChange;

		[SerializeField]
		private List<GameObject> eventPointList = new List<GameObject>();

		[SerializeField]
		private float distance = -1f;

		private Transform _camTrans;

		private SpriteRenderer _render;

		private BoolReactiveProperty _isIconDraw;

		private static readonly string[] _wakeUpWords = new string[1] { "「ふぁぁ……、すっきりした」" };

		private Player player;

		public Type category
		{
			get
			{
				return type;
			}
		}

		public List<int> HType
		{
			get
			{
				return hType;
			}
		}

		public bool IsLoadPeepLoom
		{
			get
			{
				return isLoadPeepLoom;
			}
		}

		public Vector3 HOffsetPos
		{
			get
			{
				return hOffsetPos;
			}
		}

		public Vector3 HOffsetAngle
		{
			get
			{
				return hOffsetAngle;
			}
		}

		private Transform camTrans
		{
			get
			{
				if (_camTrans == null && Camera.main != null)
				{
					_camTrans = Camera.main.transform;
				}
				return _camTrans;
			}
		}

		public bool isIconDraw
		{
			get
			{
				return _isIconDraw.Value;
			}
			private set
			{
				_isIconDraw.Value = value;
			}
		}

		private string[] wakeUpWords
		{
			get
			{
				string[] array = Singleton<Game>.Instance.actScene.uiTranslater.Get(6).Values.ToArray("Sleep");
				return (!array.Any()) ? _wakeUpWords : array;
			}
		}

		private bool isVisible
		{
			get
			{
				if (!Singleton<Game>.IsInstance())
				{
					return false;
				}
				if (Singleton<Game>.Instance.IsRegulate(true))
				{
					return false;
				}
				ActionScene actScene = Singleton<Game>.Instance.actScene;
				if (actScene == null)
				{
					return false;
				}
				if (actScene.regulate != 0)
				{
					return false;
				}
				if (actScene.AdvScene != null && actScene.AdvScene.gameObject.activeSelf)
				{
					return false;
				}
				return true;
			}
		}

		private bool InitializeHitVisible()
		{
			switch (type)
			{
			default:
				return false;
			}
		}

		private string[] InitializeHitTagNames()
		{
			switch (type)
			{
			case Type.なし:
			case Type.部活メニュー:
			case Type.部活レポート:
			case Type.時間割:
			case Type.覗きトイレ:
			case Type.覗きシャワー:
			case Type.特殊H:
			case Type.寝る:
			case Type.寝るv2:
				return new string[1] { "Player" };
			default:
				return null;
			}
		}

		private bool VisibleIcon()
		{
			switch (type)
			{
			case Type.部活メニュー:
				if (Singleton<Game>.Instance.actScene.npcList.Any((NPC npc) => npc.isActive && (npc.isOnanism || npc.isLesbian)))
				{
					return false;
				}
				break;
			case Type.覗きトイレ:
			case Type.覗きシャワー:
				foreach (GameObject eventPoint in eventPointList)
				{
					Door component = eventPoint.GetComponent<Door>();
					if (component != null && component.IsClose(null))
					{
						return true;
					}
				}
				return false;
			case Type.特殊H:
			{
				if (player == null || player.chaser == null || player.chaser.heroine.hCount == 0)
				{
					return false;
				}
				if (hType.Contains(1002) && player.chaser.heroine.HExperience < SaveData.Heroine.HExperienceKind.慣れ)
				{
					return false;
				}
				BoxCollider boxCollider2 = col as BoxCollider;
				if (boxCollider2 != null)
				{
					Transform transform2 = boxCollider2.transform;
					if (Physics.OverlapBox(transform2.position, transform2.localScale * 0.5f, transform2.rotation, 8192).Any((Collider col) => col.CompareTag("NPC") && col.transform != player.chaser.cachedTransform))
					{
						return false;
					}
				}
				return true;
			}
			case Type.寝る:
			case Type.寝るv2:
			{
				if (player.chaser != null)
				{
					return false;
				}
				BoxCollider boxCollider = col as BoxCollider;
				if (boxCollider == null)
				{
					return false;
				}
				Transform transform = boxCollider.transform;
				if (Physics.OverlapBox(transform.position, transform.localScale * 0.5f, transform.rotation, 8192).Any((Collider p) => p.CompareTag("NPC")))
				{
					return false;
				}
				break;
			}
			}
			return true;
		}

		private bool CheckTag(Collider other, string[] tagNames)
		{
			if (!base.enabled)
			{
				return false;
			}
			return tagNames.IsNullOrEmpty() || tagNames.Any((string tag) => other.CompareTag(tag));
		}

		private IEnumerator Execute(CancellationToken cancel)
		{
			visibleChange.visible = !visibleChange.visible;
			player.isActionNow = true;
			switch (type)
			{
			case Type.部活メニュー:
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					assetBundleName = "action/menu/staffroom/staffroommenu.unity3d",
					levelName = "StaffRoomMenu",
					isAdd = true,
					isAsync = true,
					isFade = true
				}, false);
				yield return new WaitWhile(() => Singleton<Scene>.Instance.IsNowLoadingFade);
				break;
			case Type.部活レポート:
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					assetBundleName = "action/menu/clubreport.unity3d",
					levelName = "ClubReport",
					isAdd = true,
					isAsync = true,
					isFade = true
				}, false);
				yield return new WaitWhile(() => Singleton<Scene>.Instance.IsNowLoadingFade);
				break;
			case Type.時間割:
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					assetBundleName = "action/menu/classschedulemenu.unity3d",
					levelName = "ClassScheduleMenu",
					isAdd = true,
					isAsync = true
				}, false);
				yield return new WaitWhile(() => Singleton<Scene>.Instance.IsNowLoadingFade);
				break;
			case Type.覗きトイレ:
			{
				Door door2 = eventPointList.Select((GameObject item) => item.GetComponent<Door>()).FirstOrDefault((Door item) => item != null);
				if (!(door2 != null))
				{
					break;
				}
				NPC npc2 = door2.hitList.Select((Collider item) => item.gameObject.GetComponent<NPC>()).FirstOrDefault((NPC item) => item != null && item.AI.actionNo != 23);
				if (npc2 != null)
				{
					int id2 = npc2.GetOnanismID(true);
					if (id2 == -1)
					{
						id2 = 2000;
						Singleton<Game>.Instance.rankSaveData.peepToiletCount++;
					}
					else
					{
						id2 = 2003;
					}
					yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Game>.Instance.actScene.ChangeHSpecial(new List<int> { id2 }, true, base.transform, npc2)).StartAsCoroutine(cancel);
				}
				break;
			}
			case Type.覗きシャワー:
			{
				Door door = eventPointList.Select((GameObject item) => item.GetComponent<Door>()).FirstOrDefault((Door item) => item != null);
				if (!(door != null))
				{
					break;
				}
				NPC npc = door.hitList.Select((Collider item) => item.gameObject.GetComponent<NPC>()).FirstOrDefault((NPC item) => item != null && item.AI.actionNo != 23);
				if (npc != null)
				{
					int id = npc.GetOnanismID(true);
					if (id == -1)
					{
						id = 2001;
						Singleton<Game>.Instance.rankSaveData.peepShowerCount++;
					}
					else
					{
						id = 2004;
					}
					yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Game>.Instance.actScene.ChangeHSpecial(new List<int> { id }, true, base.transform, npc)).StartAsCoroutine(cancel);
				}
				break;
			}
			case Type.特殊H:
			{
				if (!(player.chaser != null))
				{
					break;
				}
				string prevBGM3 = string.Empty;
				float prevVolume3 = 1f;
				yield return Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Sound.GetBGMandVolume(delegate(string bgm, float volume)
				{
					prevBGM3 = bgm;
					prevVolume3 = volume;
				})).StartAsCoroutine(cancel);
				Kind kind = eventPointList.Select((GameObject item) => item.GetComponent<Kind>()).FirstOrDefault((Kind item) => item != null);
				if (!hType.Any())
				{
					yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Game>.Instance.actScene.ChangeH(kind, true, isLoadPeepLoom, player.chaser)).StartAsCoroutine(cancel);
				}
				else
				{
					yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Game>.Instance.actScene.ChangeHSpecial(this, kind, player.chaser)).StartAsCoroutine(cancel);
				}
				yield return Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Sound.GetFadePlayerWhileNull(prevBGM3, prevVolume3)).StartAsCoroutine(cancel);
				break;
			}
			case Type.寝る:
			{
				string prevBGM2 = string.Empty;
				float prevVolume2 = 1f;
				yield return Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Sound.GetBGMandVolume(delegate(string bgm, float volume)
				{
					prevBGM2 = bgm;
					prevVolume2 = volume;
				})).StartAsCoroutine(cancel);
				ActionScene actScene2 = Singleton<Game>.Instance.actScene;
				Base target2 = GetSleepEventChara(actScene2, Tuple.Create(20, -1, 4));
				if (target2 != null)
				{
					SetCharaActiveFalse(actScene2, target2);
					List<int> selecter2 = new List<int> { 0 };
					HashSet<int> hash2;
					if (Singleton<Game>.Instance.saveData.clubContents.TryGetValue(1, out hash2) && hash2.Contains(2004))
					{
						selecter2.Add(1);
					}
					int select2 = selecter2.Shuffle().First();
					int advNo2;
					if (target2.heroine.isTeacher)
					{
						advNo2 = 6 + select2;
					}
					else
					{
						advNo2 = 94 + select2;
					}
					Singleton<Scene>.Instance.SetFadeColor(Color.black);
					yield return Observable.FromCoroutine((CancellationToken _) => EventADVStart(advNo2, target2.heroine)).StartAsCoroutine(cancel);
					Singleton<Scene>.Instance.SetFadeColorDefault();
					Tuple<int, Vector3, Quaternion> data2 = GetEventValueAndNull(actScene2);
					int appoint2 = data2.Item1;
					Vector3 pos2 = data2.Item2;
					Quaternion rot2 = data2.Item3;
					yield return Observable.FromCoroutine((CancellationToken _) => actScene2.ChangeHEventWithActionPoint(pos2, rot2, appoint2, target2)).StartAsCoroutine(cancel);
				}
				else
				{
					Singleton<Scene>.Instance.SetFadeColor(Color.black);
					if (Illusion.Game.Utils.Scene.SafeFadeIn() == Scene.Data.FadeType.In)
					{
						yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In)).StartAsCoroutine(cancel);
					}
					yield return Observable.FromCoroutine((CancellationToken _) => MonologueEvent(Tuple.Create("[P名]", wakeUpWords.Shuffle().First()))).StartAsCoroutine(cancel);
				}
				Singleton<Scene>.Instance.SetFadeColorDefault();
				actScene2.Cycle.AddTimer(0.1f);
				yield return Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Sound.GetFadePlayerWhileNull(prevBGM2, prevVolume2)).StartAsCoroutine(cancel);
				break;
			}
			case Type.寝るv2:
			{
				string prevBGM = string.Empty;
				float prevVolume = 1f;
				yield return Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Sound.GetBGMandVolume(delegate(string bgm, float volume)
				{
					prevBGM = bgm;
					prevVolume = volume;
				})).StartAsCoroutine(cancel);
				ActionScene actScene = Singleton<Game>.Instance.actScene;
				Base target = GetSleepEventChara(actScene, Tuple.Create(36, -2, 4));
				if (target != null)
				{
					SetCharaActiveFalse(actScene, target);
					List<int> selecter = new List<int> { 2 };
					HashSet<int> hash;
					if (Singleton<Game>.Instance.saveData.clubContents.TryGetValue(1, out hash) && hash.Contains(1027))
					{
						selecter.Add(3);
					}
					int select = selecter.Shuffle().First();
					int advNo;
					if (target.heroine.isTeacher)
					{
						advNo = 6 + select;
					}
					else
					{
						advNo = 94 + select;
					}
					Singleton<Scene>.Instance.SetFadeColor(Color.black);
					yield return Observable.FromCoroutine((CancellationToken _) => EventADVStart(advNo, target.heroine)).StartAsCoroutine(cancel);
					Singleton<Scene>.Instance.SetFadeColorDefault();
					Tuple<int, Vector3, Quaternion> data = GetEventValueAndNull(actScene);
					int appoint = data.Item1;
					Vector3 pos = data.Item2;
					Quaternion rot = data.Item3;
					yield return Observable.FromCoroutine((CancellationToken _) => actScene.ChangeHEventWithActionPoint(pos, rot, appoint, target)).StartAsCoroutine(cancel);
				}
				else
				{
					Singleton<Scene>.Instance.SetFadeColor(Color.black);
					if (Illusion.Game.Utils.Scene.SafeFadeIn() == Scene.Data.FadeType.In)
					{
						yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In)).StartAsCoroutine(cancel);
					}
					yield return Observable.FromCoroutine((CancellationToken _) => MonologueEvent(Tuple.Create("[P名]", wakeUpWords.Shuffle().First()))).StartAsCoroutine(cancel);
				}
				Singleton<Scene>.Instance.SetFadeColorDefault();
				actScene.Cycle.AddTimer(0.1f);
				yield return Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Sound.GetFadePlayerWhileNull(prevBGM, prevVolume)).StartAsCoroutine(cancel);
				break;
			}
			}
			player.isActionNow = false;
		}

		private Base GetSleepEventChara(ActionScene actScene, Tuple<int, int, int>? fixCondition)
		{
			Base @base = null;
			if (fixCondition.HasValue && @base == null && actScene.fixChara != null)
			{
				SaveData.Heroine heroine = actScene.fixChara.heroine;
				Tuple<int, int, int> value = fixCondition.Value;
				int item = value.Item1;
				int item2 = value.Item2;
				int item3 = value.Item3;
				if (actScene.fixChara.mapNo == item && heroine.fixCharaID == item2 && (item3 == -1 || heroine.talkEvent.Contains(item3)))
				{
					@base = actScene.fixChara;
				}
			}
			if (@base == null)
			{
				@base = actScene.npcList.Shuffle().Where(delegate(NPC p)
				{
					SaveData.Heroine heroine2 = p.heroine;
					if (heroine2.hCount == 0 || heroine2.lewdness < 50 || heroine2.isAnger)
					{
						return false;
					}
					int desire = actScene.actCtrl.GetDesire(5, heroine2);
					return desire > 30 && Illusion.Utils.ProbabilityCalclator.DetectFromPercent(desire);
				}).FirstOrDefault();
			}
			return @base;
		}

		private void SetCharaActiveFalse(ActionScene actScene, Base target)
		{
			actScene.Player.SetActive(false);
			actScene.npcList.ForEach(delegate(NPC p)
			{
				p.SetActive(false);
			});
			actScene.npcList.ForEach(delegate(NPC p)
			{
				p.Pause(true);
			});
			if (actScene.fixChara != null)
			{
				actScene.fixChara.SetActive(false);
			}
			NPC nPC = target as NPC;
			if (nPC != null)
			{
				if (nPC.mapNo != actScene.Player.mapNo)
				{
					nPC.mapNo = actScene.Player.mapNo;
				}
				Transform transform = base.transform.Find("EndPos");
				if (transform != null)
				{
					nPC.SetPositionAndRotation(transform);
				}
				else
				{
					Vector3 position = actScene.Player.position;
					Vector3 forward = Vector3.forward;
					Vector3 vector = actScene.Player.rotation * forward;
					nPC.SetPositionAndRotation(position + vector, actScene.Player.eulerAngles);
				}
			}
			target.chaCtrl.RandomChangeOfClothesLowPolyEnd();
		}

		private Tuple<int, Vector3, Quaternion> GetEventValueAndNull(ActionScene actScene)
		{
			int item = -1;
			Dictionary<string, ValData> vars = actScene.AdvScene.Scenario.Vars;
			ValData value;
			if (vars.TryGetValue("appoint", out value))
			{
				item = (int)value.o;
			}
			string text = null;
			if (vars.TryGetValue("hPos", out value))
			{
				text = (string)value.o;
			}
			int version = 0;
			if (vars.TryGetValue("nullNo", out value))
			{
				version = (int)value.o;
			}
			Vector3 position;
			Quaternion rotation;
			if (text.IsNullOrEmpty())
			{
				position = base.transform.position;
				rotation = base.transform.rotation;
			}
			else
			{
				ActionMap map = actScene.Map;
				Program.GetNull(version, map.ConvertMapName(map.no), text, map, out position, out rotation);
			}
			return Tuple.Create(item, position, rotation);
		}

		private IEnumerator EventADVStart(int advNo, SaveData.Heroine heroine)
		{
			Scene.Data.FadeType fadeType = Illusion.Game.Utils.Scene.SafeFadeIn();
			if (fadeType == Scene.Data.FadeType.In)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			}
			bool isOpenADV = false;
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			yield return StartCoroutine(Program.Open(new Data
			{
				fadeInTime = 0f,
				position = Vector3.zero,
				rotation = Quaternion.identity,
				camera = null,
				heroineList = new List<SaveData.Heroine> { heroine },
				scene = actScene,
				transferList = EventADV(advNo, heroine)
			}, new Program.OpenDataProc
			{
				onLoad = delegate
				{
					isOpenADV = true;
				}
			}));
			yield return new WaitUntil(() => isOpenADV);
			yield return Program.Wait(string.Empty);
			heroine.talkEvent.Add(advNo);
			Singleton<Scene>.Instance.SetFadeColorDefault();
		}

		private List<Program.Transfer> EventADV(int advNo, SaveData.Heroine heroine)
		{
			List<Program.Transfer> list = Program.Transfer.NewList();
			Program.SetParam(Singleton<Game>.Instance.Player, heroine, list);
			list.Add(Program.Transfer.Create(false, Command.CameraSetFov, "23"));
			list.Add(Program.Transfer.Create(false, Command.CharaCreate, "0", "-2"));
			list.Add(Program.Transfer.Create(false, Command.CharaCreate, "-1", "-1"));
			list.Add(Program.Transfer.Open(Program.FindADVBundleFilePath(advNo, heroine), advNo.ToString(), bool.TrueString));
			return list;
		}

		private IEnumerator MonologueEvent(params Tuple<string, string>[] words)
		{
			List<Program.Transfer> transferList = Program.Transfer.NewList();
			Program.SetParam(Singleton<Game>.Instance.Player, transferList);
			transferList.Add(Program.Transfer.Create(false, Command.Regulate, "Add", Regulate.Control.Next.ToString()));
			transferList.Add(Program.Transfer.Create(false, Command.Wait, "1"));
			transferList.Add(Program.Transfer.Create(false, Command.SceneFade, "out", "1"));
			transferList.Add(Program.Transfer.Create(false, Command.Regulate, "Sub", Regulate.Control.Next.ToString()));
			for (int i = 0; i < words.Length; i++)
			{
				Tuple<string, string> tuple = words[i];
				transferList.Add(Program.Transfer.Text(tuple.Item1, tuple.Item2));
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			Transform camT = Camera.main.transform;
			transferList.Add(Program.Transfer.Close());
			bool isOpenADV = false;
			yield return StartCoroutine(Program.Open(new Data
			{
				position = actScene.Player.position,
				rotation = actScene.Player.rotation,
				scene = actScene,
				camera = new OpenData.CameraData
				{
					position = camT.position,
					rotation = camT.rotation
				},
				transferList = transferList
			}, new Program.OpenDataProc
			{
				onLoad = delegate
				{
					isOpenADV = true;
				}
			}));
			yield return new WaitUntil(() => isOpenADV);
			yield return Program.ADVProcessingCheck();
		}

		private void Awake()
		{
			if (icon != null)
			{
				_render = icon.GetComponent<SpriteRenderer>();
			}
		}

		private void Start()
		{
			if (!Singleton<Game>.IsInstance())
			{
				return;
			}
			if (Singleton<Game>.Instance.actScene != null)
			{
				player = Singleton<Game>.Instance.actScene.Player;
			}
			bool isHitVisible = InitializeHitVisible();
			string[] tagNames = InitializeHitTagNames();
			_isIconDraw = new BoolReactiveProperty(!isHitVisible);
			if (_render == null)
			{
				_isIconDraw.Dispose();
			}
			else
			{
				_isIconDraw.TakeUntilDestroy(this).Subscribe(delegate(bool isOn)
				{
					_render.enabled = isOn;
				});
			}
			bool isHit = false;
			(from other in col.OnTriggerEnterAsObservable()
				where CheckTag(other, tagNames)
				select other).Subscribe(delegate(Collider other)
			{
				isHit = true;
				if (other.CompareTag("Player"))
				{
					other.GetComponent<Player>().actionPointList.Add(this);
				}
			});
			(from other in col.OnTriggerExitAsObservable()
				where CheckTag(other, tagNames)
				select other).Subscribe(delegate(Collider other)
			{
				isHit = false;
				if (other.CompareTag("Player"))
				{
					other.GetComponent<Player>().actionPointList.Remove(this);
				}
			});
			Tween iconPosTween = null;
			Vector3 iconPos = icon.localPosition;
			this.UpdateAsObservable().Subscribe(delegate
			{
				if (isHit)
				{
					if (iconPosTween == null)
					{
						iconPosTween = icon.DOLocalJump(iconPos, 0.1f, 1, 3f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Restart);
					}
				}
				else if (iconPosTween != null)
				{
					iconPosTween.Kill();
					iconPosTween = null;
					icon.localPosition = iconPos;
				}
			});
			if (_render != null)
			{
				Color iconColor = _render.color;
				(from _ in this.UpdateAsObservable()
					select isHit).DistinctUntilChanged().Subscribe(delegate(bool hit)
				{
					_render.color = ((!hit) ? iconColor : Color.yellow);
				});
			}
			this.UpdateAsObservable().Subscribe(delegate
			{
				bool flag = (!isHitVisible || isHit) && isVisible && VisibleIcon();
				if (flag && col != null && distance > 0f && player != null)
				{
					flag &= (player.position - col.transform.position).sqrMagnitude < distance * distance;
				}
				isIconDraw = flag;
				Billboard();
			});
			if (player != null)
			{
				ActionChangeUI actionChangeUI = player.actScene.actionChangeUI;
				Type type = this.type;
				if (type == Type.覗きトイレ || type == Type.覗きシャワー)
				{
					ActionChangeUI.ActionType iconType = ActionChangeUI.ActionType.Peeping;
					(from _ in this.UpdateAsObservable().TakeUntilDestroy(actionChangeUI)
						select isIconDraw && isHit).DistinctUntilChanged().Subscribe((Action<bool>)delegate(bool active)
					{
						if (active)
						{
							actionChangeUI.Set(iconType);
						}
						else
						{
							actionChangeUI.Remove(iconType);
						}
					}, (Action)delegate
					{
						if (actionChangeUI != null)
						{
							actionChangeUI.Remove(iconType);
						}
					});
				}
			}
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(player)
				where isHit
				where isIconDraw
				where !player.isActionNow
				where Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty()
				where ActionInput.isAction
				select _).Subscribe(delegate
			{
				Observable.FromCoroutine(Execute).Subscribe();
			});
		}

		private void OnDrawGizmos()
		{
			if (col != null && distance > 0f)
			{
				Color blue = Color.blue;
				blue.a = 0.5f;
				Gizmos.color = blue;
				Gizmos.DrawSphere(col.transform.position, distance);
			}
			if (hOffsetPos.magnitude > 1E-05f || hOffsetAngle.magnitude > 1E-05f)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Vector3 vector = base.transform.position + hOffsetPos;
				float num = 0.25f;
				Gizmos.DrawSphere(vector, num);
				Illusion.Utils.Gizmos.Axis(vector, Quaternion.Euler(base.transform.eulerAngles + hOffsetAngle), num);
			}
		}

		private void Billboard()
		{
			if (isIconDraw && !(camTrans == null))
			{
				Vector3 position = camTrans.position;
				position.y = icon.position.y;
				icon.LookAt(position);
				icon.Rotate(new Vector3(0f, 180f, 0f));
			}
		}
	}
}
