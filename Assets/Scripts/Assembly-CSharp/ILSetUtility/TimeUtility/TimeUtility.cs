using Config;
using Manager;
using UnityEngine;

namespace ILSetUtility.TimeUtility
{
	public class TimeUtility : MonoBehaviour
	{
		private float fps;

		[Range(0f, 50f)]
		public float time_scale = 1f;

		private float deltaTime;

		private uint frame_cnt;

		private float time_cnt;

		public bool mode_mem;

		private float memTime;

		public bool ForceDrawFPS;

		private TimeUtilityDrawer drawer;

		private void Awake()
		{
			fps = 0f;
			time_scale = 1f;
			deltaTime = 0f;
			frame_cnt = 0u;
			time_cnt = 0f;
			mode_mem = false;
			memTime = 0f;
		}

		private void Start()
		{
			drawer = this.GetOrAddComponent<TimeUtilityDrawer>();
			drawer.enabled = false;
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Delete))
			{
				DebugSystem debugStatus = Manager.Config.DebugStatus;
				debugStatus.FPS = !debugStatus.FPS;
				Singleton<Manager.Config>.Instance.Save();
			}
			drawer.enabled = ForceDrawFPS || Manager.Config.DebugStatus.FPS;
			if (drawer.enabled)
			{
				drawer.fps = fps;
				deltaTime = Time.deltaTime * time_scale;
				if (mode_mem)
				{
					memTime += Time.deltaTime;
				}
				time_cnt += Time.deltaTime;
				frame_cnt++;
				if (1f <= time_cnt)
				{
					fps = (float)frame_cnt / time_cnt;
					frame_cnt = 0u;
					time_cnt = 0f;
				}
			}
		}

		public void SetTimeScale(float value)
		{
			time_scale = value;
		}

		public float GetTimeScale()
		{
			return time_scale;
		}

		public float GetFps()
		{
			return fps;
		}

		public float GetTime()
		{
			return deltaTime;
		}

		public void ChangeMemoryFlags(bool flags)
		{
			mode_mem = flags;
		}

		public float GetMemoryTime()
		{
			return memTime;
		}
	}
}
