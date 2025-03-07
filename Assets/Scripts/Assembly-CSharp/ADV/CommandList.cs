using System.Collections.Generic;
using System.Linq;
using ADV.Commands.Base;
using ADV.Commands.Camera;
using ADV.Commands.CameraEffect;
using ADV.Commands.Chara;
using ADV.Commands.Chara2D;
using ADV.Commands.Effect;
using ADV.Commands.EtcData;
using ADV.Commands.EventCG;
using ADV.Commands.Game;
using ADV.Commands.H;
//using ADV.Commands.Movie;
using ADV.Commands.Object;
using ADV.Commands.Sound.BGM;
using ADV.Commands.Sound.ENV;
using ADV.Commands.Sound.SE2D;
using ADV.Commands.Sound.SE3D;

namespace ADV
{
	public class CommandList : List<CommandBase>
	{
		private TextScenario scenario;

		public CommandList(TextScenario scenario)
		{
			this.scenario = scenario;
		}

		public new void Add(CommandBase item)
		{
		}

		public bool Add(ScenarioData.Param item, int currentLine)
		{
			CommandBase commandBase = CommandGet(item.Command);
			if (commandBase == null)
			{
				return true;
			}
			commandBase.Initialize(scenario, item.Command, item.Args);
			commandBase.ConvertBeforeArgsProc();
			for (int i = 0; i < commandBase.args.Length; i++)
			{
				commandBase.args[i] = scenario.ReplaceVars(commandBase.args[i]);
			}
			commandBase.localLine = currentLine;
			commandBase.Do();
			base.Add(commandBase);
			return item.Multi;
		}

		public bool Process()
		{
			this.Where((CommandBase item) => item.Process()).ToList().ForEach(delegate(CommandBase item)
			{
				Remove(item);
				item.Result(true);
			});
			return this.Any((CommandBase p) => IsWait(p.command));
		}

		public void ProcessEnd()
		{
			ForEach(delegate(CommandBase item)
			{
				item.Result(false);
			});
			Clear();
		}

		private static bool IsWait(Command command)
		{
			switch (command)
			{
			case Command.Choice:
			case Command.FadeWait:
			case Command.CharaKaraokePlay:
			case Command.TaskWait:
				return true;
			default:
				return false;
			}
		}

		public static CommandBase CommandGet(Command command)
		{
			switch (command)
			{
			case Command.Tag:
				return new Tag();
			case Command.Text:
				return new Text();
			case Command.Voice:
				return new Voice();
			case Command.Motion:
				return new ADV.Commands.Base.Motion();
			case Command.Expression:
				return new ADV.Commands.Base.Expression();
			case Command.ExpressionIcon:
				return new ADV.Commands.Base.ExpressionIcon();
			case Command.VAR:
				return new VAR();
			case Command.RandomVar:
				return new RandomVar();
			case Command.IF:
				return new IF();
			case Command.Switch:
				return new Switch();
			case Command.Calc:
				return new Calc();
			case Command.Clamp:
				return new Clamp();
			case Command.Min:
				return new Min();
			case Command.Max:
				return new Max();
			case Command.Lerp:
				return new Lerp();
			case Command.LerpAngle:
				return new LerpAngle();
			case Command.InverseLerp:
				return new InverseLerp();
			case Command.LerpV3:
				return new LerpV3();
			case Command.LerpAngleV3:
				return new LerpAngleV3();
			case Command.Open:
				return new Open();
			case Command.Close:
				return new Close();
			case Command.Jump:
				return new Jump();
			case Command.Choice:
				return new Choice();
			case Command.Wait:
				return new Wait();
			case Command.TextClear:
				return new Clear();
			case Command.FontColor:
				return new FontColor();
			case Command.Scene:
				return new Scene();
			case Command.WindowActive:
				return new WindowActive();
			case Command.WindowImage:
				return new WindowImage();
			case Command.Regulate:
				return new ADV.Commands.Base.Regulate();
			case Command.Replace:
				return new Replace();
			case Command.Reset:
				return new ADV.Commands.Base.Reset();
			case Command.Vector:
				return new Vector();
			case Command.Format:
				return new Format();
			case Command.NullLoad:
				return new NullLoad();
			case Command.NullRelease:
				return new NullRelease();
			case Command.NullSet:
				return new NullSet();
			case Command.BGLoad:
				return new BackGroundLoad();
			case Command.BGRelease:
				return new BackGroundRelease();
			case Command.BGVisible:
				return new BackGroundVisible();
			case Command.InfoAudioEco:
				return new InfoAudioEco();
			case Command.InfoAnimePlay:
				return new InfoAnimePlay();
			case Command.Fade:
				return new ADV.Commands.Effect.Fade();
			case Command.SceneFade:
				return new ADV.Commands.Effect.SceneFade();
			case Command.SceneFadeRegulate:
				return new SceneFadeRegulate();
			case Command.FadeWait:
				return new FadeWait();
			case Command.FilterImageLoad:
				return new FilterImageLoad();
			case Command.FilterImageRelease:
				return new FilterImageRelease();
			case Command.FadeSet:
				return new FadeSet();
			case Command.FilterSet:
				return new FilterSet();
			case Command.Filter:
				return new Filter();
			case Command.ImageLoad:
				return new ImageLoad();
			case Command.ImageRelease:
				return new ImageRelease();
			case Command.EjaculationEffect:
				return new EjaculationEffect();
			case Command.EcstacyEffect:
				return new EcstacyEffect();
			case Command.EcstacySyncEffect:
				return new EcstacySyncEffect();
			case Command.CrossFade:
				return new ADV.Commands.CameraEffect.CrossFade();
			case Command.BlurEffect:
				return new BlurEffect();
			case Command.DOFTarget:
				return new DOFTarget();
			case Command.SepiaEffect:
				return new SepiaEffect();
			case Command.CameraActive:
				return new Active();
			case Command.CameraAspect:
				return new Aspect();
			case Command.CameraChange:
				return new Change();
			case Command.CameraDirectionAdd:
				return new AddDirection();
			case Command.CameraDirectionSet:
				return new SetDirection();
			case Command.CameraLerpNullMove:
				return new LerpNullMove();
			case Command.CameraLerpNullSet:
				return new LerpNullSet();
			case Command.CameraLerpAdd:
				return new LerpAdd();
			case Command.CameraLerpSet:
				return new LerpSet();
			case Command.CameraLerpAnimationAdd:
				return new AnimationLerpAdd();
			case Command.CameraLerpAnimationSet:
				return new AnimationLerpSet();
			case Command.CameraLerpTargetAdd:
				return new LerpAddTarget();
			case Command.CameraLerpTargetSet:
				return new LerpSetTarget();
			case Command.CameraPositionAdd:
				return new ADV.Commands.Camera.AddPosition();
			case Command.CameraPositionSet:
				return new ADV.Commands.Camera.SetPosition();
			case Command.CameraRotationAdd:
				return new AddRotation();
			case Command.CameraRotationSet:
				return new SetRotation();
			case Command.CameraDefault:
				return new SetDefault();
			case Command.CameraParent:
				return new SetParent();
			case Command.CameraNull:
				return new SetNull();
			case Command.CameraTarget:
				return new SetTarget();
			case Command.CameraTargetFront:
				return new SetTargetFront();
			case Command.CameraTargetCharacter:
				return new SetTargetCharacter();
			case Command.CameraReset:
				return new ADV.Commands.Camera.Reset();
			case Command.CameraLock:
				return new Lock();
			case Command.CameraGetFov:
				return new GetFov();
			case Command.CameraSetFov:
				return new SetFov();
			case Command.BGMPlay:
				return new ADV.Commands.Sound.BGM.Play();
			case Command.BGMStop:
				return new ADV.Commands.Sound.BGM.Stop();
			case Command.EnvPlay:
				return new ADV.Commands.Sound.ENV.Play();
			case Command.EnvStop:
				return new ADV.Commands.Sound.ENV.Stop();
			case Command.SE2DPlay:
				return new ADV.Commands.Sound.SE2D.Play();
			case Command.SE2DStop:
				return new ADV.Commands.Sound.SE2D.Stop();
			case Command.SE3DPlay:
				return new ADV.Commands.Sound.SE3D.Play();
			case Command.SE3DStop:
				return new ADV.Commands.Sound.SE3D.Stop();
			case Command.CharaCreate:
				return new ADV.Commands.Chara.Create();
			case Command.CharaFixCreate:
				return new FixCreate();
			case Command.CharaMobCreate:
				return new MobCreate();
			case Command.CharaDelete:
				return new ADV.Commands.Chara.Delete();
			case Command.CharaStand:
				return new ADV.Commands.Chara.StandPosition();
			case Command.CharaStandFind:
				return new StandFindPosition();
			case Command.CharaPositionAdd:
				return new ADV.Commands.Chara.AddPosition();
			case Command.CharaPositionSet:
				return new ADV.Commands.Chara.SetPosition();
			case Command.CharaPositionLocalAdd:
				return new AddPositionLocal();
			case Command.CharaPositionLocalSet:
				return new SetPositionLocal();
			case Command.CharaMotion:
				return new ADV.Commands.Chara.Motion();
			case Command.CharaMotionDefault:
				return new MotionDefault();
			case Command.CharaMotionWait:
				return new MotionWait();
			case Command.CharaMotionLayerWeight:
				return new MotionLayerWeight();
			case Command.CharaMotionSetParam:
				return new MotionSetParam();
			case Command.CharaMotionIKSetPartner:
				return new MotionIKSetPartner();
			case Command.CharaExpression:
				return new ADV.Commands.Chara.Expression();
			case Command.CharaFixEyes:
				return new FixEyes();
			case Command.CharaFixMouth:
				return new FixMouth();
			case Command.CharaExpressionIcon:
				return new ADV.Commands.Chara.ExpressionIcon();
			case Command.CharaGetShape:
				return new GetShape();
			case Command.CharaCoordinate:
				return new Coordinate();
			case Command.CharaClothState:
				return new ClothState();
			case Command.CharaSiruState:
				return new SiruState();
			case Command.CharaVoicePlay:
				return new VoicePlay();
			case Command.CharaVoiceStop:
				return new VoiceStop();
			case Command.CharaVoiceStopAll:
				return new VoiceStopAll();
			case Command.CharaVoiceWait:
				return new VoiceWait();
			case Command.CharaVoiceWaitAll:
				return new VoiceWaitAll();
			case Command.CharaLookEyes:
				return new LookEyes();
			case Command.CharaLookEyesTarget:
				return new LookEyesTarget();
			case Command.CharaLookEyesTargetChara:
				return new LookEyesTargetChara();
			case Command.CharaLookNeck:
				return new LookNeck();
			case Command.CharaLookNeckTarget:
				return new LookNeckTarget();
			case Command.CharaLookNeckTargetChara:
				return new LookNeckTargetChara();
			case Command.CharaLookNeckSkip:
				return new LookNeckSkip();
			case Command.CharaItemCreate:
				return new ItemCreate();
			case Command.CharaItemDelete:
				return new ItemDelete();
			case Command.CharaItemAnime:
				return new ItemAnime();
			case Command.CharaItemFind:
				return new ItemFind();
			case Command.EventCGSetting:
				return new Setting();
			case Command.EventCGRelease:
				return new Release();
			case Command.EventCGNext:
				return new Next();
			case Command.MotionLoad:
				return new MotionLoad();
			case Command.ExpLoad:
				return new ExpLoad();
			case Command.Chara2DCreate:
				return new ADV.Commands.Chara2D.Create();
			case Command.Chara2DDelete:
				return new ADV.Commands.Chara2D.Delete();
			case Command.Chara2DStand:
				return new ADV.Commands.Chara2D.StandPosition();
			case Command.Chara2DPositionAdd:
				return new ADV.Commands.Chara2D.AddPosition();
			case Command.Chara2DPositionSet:
				return new ADV.Commands.Chara2D.SetPosition();
			case Command.ObjectCreate:
				return new ADV.Commands.Object.Create();
			case Command.ObjectLoad:
				return new Load();
			case Command.ObjectDelete:
				return new ADV.Commands.Object.Delete();
			case Command.ObjectPosition:
				return new Position();
			case Command.ObjectRotation:
				return new Rotation();
			case Command.ObjectScale:
				return new Scale();
			case Command.ObjectParent:
				return new Parent();
			case Command.ObjectComponent:
				return new Component();
			case Command.ObjectAnimeParam:
				return new AnimeParam();
			//case Command.MoviePlay:
				//return new ADV.Commands.Movie.Play();
			case Command.CharaActive:
				return new CharaActive();
			case Command.CharaVisible:
				return new CharaVisible();
			case Command.CharaColor:
				return new CharaColor();
			case Command.CharaParam:
				return new CharaParam();
			case Command.CharaChange:
				return new CharaChange();
			case Command.CharaNameGet:
				return new CharaNameGet();
			case Command.CharaEvent:
				return new CharaEvent();
			case Command.HeroineParam:
				return new HeroineParam();
			case Command.HeroineCallNameChange:
				return new HeroineCallNameChange();
			case Command.HeroineFinCG:
				return new HeroineFinCG();
			case Command.PlayerParam:
				return new PlayerParam();
			case Command.CycleChange:
				return new CycleChange();
			case Command.WeekChange:
				return new WeekChange();
			case Command.MapChange:
				return new MapChange();
			case Command.MapUnload:
				return new MapUnload();
			case Command.MapVisible:
				return new MapVisible();
			case Command.MapObjectActive:
				return new MapObjectActive();
			case Command.DayTimeChange:
				return new DayTimeChange();
			case Command.GetGatePos:
				return new GetGatePos();
			case Command.AddPosture:
				return new AddPosture();
			case Command.AddCollider:
				return new AddCollider();
			case Command.ColliderSetActive:
				return new ColliderSetActive();
			case Command.AddNavMeshAgent:
				return new AddNavMeshAgent();
			case Command.NavMeshAgentSetActive:
				return new NavMeshAgentSetActive();
			case Command.CameraLookAt:
				return new CameraLookAt();
			case Command.MozVisible:
				return new MozVisible();
			case Command.LookAtDankonAdd:
				return new LookAtDankonAdd();
			case Command.LookAtDankonRemove:
				return new LookAtDankonRemove();
			case Command.HMotionShakeAdd:
				return new MotionShakeAdd();
			case Command.HMotionShakeRemove:
				return new MotionShakeRemove();
			case Command.HitReaction:
				return new ADV.Commands.H.HitReaction();
			case Command.BundleCheck:
				return new BundleCheck();
			case Command.CharaPersonal:
				return new CharaPersonal();
			case Command.HNamaOK:
				return new HNamaOK();
			case Command.HNamaNG:
				return new HNamaNG();
			case Command.CameraShakePos:
				return new ShakePos();
			case Command.CameraShakeRot:
				return new ShakeRot();
			case Command.Prob:
				return new Prob();
			case Command.Probs:
				return new Probs();
			case Command.FormatVAR:
				return new FormatVAR();
			case Command.CharaKaraokePlay:
				return new KaraokePlay();
			case Command.CharaKaraokeStop:
				return new KaraokeStop();
			case Command.Task:
				return new Task();
			case Command.TaskWait:
				return new TaskWait();
			case Command.TaskEnd:
				return new TaskEnd();
			case Command.ParameterFile:
				return new ParameterFile();
			case Command.Log:
				return new Log();
			case Command.HSafeDaySet:
				return new HSafeDaySet();
			case Command.HDangerDaySet:
				return new HDangerDaySet();
			case Command.HeroineWeddingInfo:
				return new HeroineWeddingInfo();
			case Command.CameraLightOffset:
				return new CameraCorrectLightOffset();
			case Command.CameraLightActive:
				return new CameraCorrectLightActive();
			case Command.CameraLightAngle:
				return new CameraCorrectLightAngle();
			case Command.CharaSetShape:
				return new SetShape();
			case Command.CharaCoordinateChange:
				return new CoordinateChange();
			case Command.CameraAnimeLoad:
				return new AnimeLoad();
			case Command.CameraAnimePlay:
				return new AnimePlay();
			case Command.CameraAnimeWait:
				return new AnimeWait();
			case Command.CameraAnimeLayerWeight:
				return new AnimeLayerWeight();
			case Command.CameraAnimeSetParam:
				return new AnimeSetParam();
			case Command.CameraAnimeRelease:
				return new AnimeRelease();
			case Command.CharaShoesChange:
				return new ShoesChange();
			case Command.InfoAudio:
				return new InfoAudio();
			case Command.CharaCreateEmpty:
				return new CreateEmpty();
			case Command.CharaCreateDummy:
				return new CreateDummy();
			case Command.CharaFixCreateDummy:
				return new FixCreateDummy();
			case Command.CharaMobCreateDummy:
				return new MobCreateDummy();
			case Command.ReplaceLanguage:
				return new ReplaceLanguage();
			default:
				return null;
			}
		}
	}
}
