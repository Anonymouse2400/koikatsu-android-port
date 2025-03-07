namespace Config
{
	public class ActionSystem : BaseSystem
	{
		public const int MAX_CHARA_NUM = 38;

		public const int TPS_OFFSET_Y = 0;

		public const int TPS_SENSITIVITY_X = 50;

		public const int TPS_SENSITIVITY_Y = 50;

		public const int TPS_SMOOTH_MOVE_TIME = 30;

		public const int FPS_SENSITIVITY_X = 50;

		public const int FPS_SENSITIVITY_Y = 50;

		public const int FPS_SMOOTH_MOVE_TIME = 20;

		public bool MoveLook;

		public int MaxCharaNum = 38;

		public float TPSOffsetY;

		public int TPSSensitivityX = 50;

		public int TPSSensitivityY = 50;

		public int TPSSmoothMoveTime = 30;

		public int FPSSensitivityX = 50;

		public int FPSSensitivityY = 50;

		public int FPSSmoothMoveTime = 20;

		public bool InvertMoveX;

		public bool InvertMoveY;

		public bool CrouchCtrlKey;

		public bool ToiletTPS = true;

		public ActionSystem(string elementName)
			: base(elementName)
		{
		}

		public override void Init()
		{
			MoveLook = false;
			MaxCharaNum = 38;
			TPSOffsetY = 0f;
			TPSSensitivityX = 50;
			TPSSensitivityY = 50;
			TPSSmoothMoveTime = 30;
			FPSSensitivityX = 50;
			FPSSensitivityY = 50;
			FPSSmoothMoveTime = 20;
			InvertMoveX = false;
			InvertMoveY = false;
			CrouchCtrlKey = false;
			ToiletTPS = true;
		}
	}
}
