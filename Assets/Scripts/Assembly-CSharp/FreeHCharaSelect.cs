using FreeH;
using Manager;
using UniRx;

public class FreeHCharaSelect : BaseLoader
{
	public FreeHPreviewCharaList freeHPreviewCharaList;

	public int sex;

	private ReactiveProperty<SaveData.Heroine> _heroine = new ReactiveProperty<SaveData.Heroine>();

	private ReactiveProperty<SaveData.Player> _player = new ReactiveProperty<SaveData.Player>();

	public CameraControl_Ver2 camCtrl;

	public SaveData.Heroine heroine
	{
		get
		{
			return _heroine.Value;
		}
	}

	public SaveData.Player player
	{
		get
		{
			return _player.Value;
		}
	}

	private void Start()
	{
		freeHPreviewCharaList.onEnter += delegate(ChaFileControl file)
		{
			if (sex == 0)
			{
				_player.Value = new SaveData.Player(file, false);
			}
			else
			{
				_heroine.Value = new SaveData.Heroine(file, false);
			}
			if ((bool)camCtrl)
			{
				camCtrl.NoCtrlCondition = () => false;
			}
			Singleton<Scene>.Instance.UnLoad();
		};
		freeHPreviewCharaList.onCancel += delegate
		{
			if ((bool)camCtrl)
			{
				camCtrl.NoCtrlCondition = () => false;
			}
			Singleton<Scene>.Instance.UnLoad();
		};
	}
}
