using UnityEngine;

public class AnimationAssist
{
	public class ANMCTRLINFOST
	{
		private string name;

		public int LoopCnt;

		public int LoopNum;

		public int LinkNo;

		public bool isReverse;

		public bool isStop;

		public WrapMode baseMode;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public ANMCTRLINFOST(AnimationState state)
		{
			name = state.name;
			LoopCnt = 0;
			LoopNum = 0;
			LinkNo = -1;
			isReverse = state.speed < 0f;
			isStop = state.speed == 0f;
			baseMode = state.wrapMode;
		}
	}

	public class ANMCTRLST
	{
		public ANMCTRLINFOST[] info;

		public bool fChange;

		public int BeforePtn;

		public int NowPtn;

		public float msTime;

		public float speed;

		public ANMCTRLST(int num)
		{
			info = new ANMCTRLINFOST[num];
			fChange = false;
			BeforePtn = -1;
			NowPtn = 0;
			msTime = -1f;
			speed = 1f;
		}
	}

	private Animation animation;

	private ANMCTRLST data;

	public Animation NowAnimation
	{
		get
		{
			return animation;
		}
	}

	public AnimationState NowAnimationState
	{
		get
		{
			return animation[GetID(data.NowPtn)];
		}
	}

	public ANMCTRLST Data
	{
		get
		{
			return data;
		}
	}

	public AnimationAssist(Animation _animation)
	{
		animation = _animation;
		int num = 0;
		data = new ANMCTRLST(_animation.GetClipCount());
		foreach (AnimationState item in animation)
		{
			data.info[num++] = new ANMCTRLINFOST(item);
		}
	}

	public string GetID(int id)
	{
		if ((uint)id >= data.info.Length)
		{
			return string.Empty;
		}
		return data.info[id].Name;
	}

	public bool IsAnimeEnd()
	{
		AnimationState nowAnimationState = NowAnimationState;
		bool flag = nowAnimationState.wrapMode == WrapMode.ClampForever;
		flag |= nowAnimationState.wrapMode == WrapMode.Loop;
		if (flag | (nowAnimationState.wrapMode == WrapMode.PingPong))
		{
			if (GetInfo().isReverse)
			{
				if (nowAnimationState.time <= 0f)
				{
					return true;
				}
			}
			else if (nowAnimationState.time >= nowAnimationState.length)
			{
				return true;
			}
		}
		return !animation.isPlaying;
	}

	public void Update()
	{
		AnimationState nowAnimationState = NowAnimationState;
		ANMCTRLINFOST info = GetInfo();
		if (data.msTime != -1f)
		{
			if (Time.timeScale == 0f)
			{
				nowAnimationState.time += data.msTime;
			}
			else
			{
				data.msTime = Time.deltaTime;
			}
		}
		if (nowAnimationState.wrapMode == WrapMode.Loop && info.LoopNum > 0)
		{
			if (IsAnimeEnd())
			{
				info.LoopCnt++;
				if (info.isReverse)
				{
					nowAnimationState.time = nowAnimationState.length;
				}
				else
				{
					nowAnimationState.time = 0f;
				}
			}
			if (info.LoopCnt > info.LoopNum)
			{
				nowAnimationState.wrapMode = WrapMode.Default;
			}
		}
		if (nowAnimationState.wrapMode == WrapMode.Default && IsAnimeEnd() && info.LinkNo != -1 && data.NowPtn != info.LinkNo)
		{
			Play(info.LinkNo);
		}
		if (IsAnimeEnd())
		{
			AnimationState animationState = animation[GetID(data.BeforePtn)];
			if ((bool)animationState)
			{
				animationState.wrapMode = GetInfo(data.BeforePtn).baseMode;
			}
		}
	}

	public void LoopSet(int ptn, int LoopNum)
	{
		ANMCTRLINFOST info = GetInfo(ptn);
		if (info != null)
		{
			info.LoopNum = LoopNum;
			info.LoopCnt = 0;
		}
		AnimationState animationState = animation[GetID(ptn)];
		if ((bool)animationState)
		{
			animationState.wrapMode = ((LoopNum != -1) ? WrapMode.Loop : WrapMode.Default);
		}
	}

	public void SpeedSetAll(float speed)
	{
		AnimationState nowAnimationState = NowAnimationState;
		if ((bool)nowAnimationState)
		{
			ANMCTRLST aNMCTRLST = data;
			float speed2 = (nowAnimationState.speed = speed);
			aNMCTRLST.speed = speed2;
		}
	}

	public void SpeedSet(float speed)
	{
		AnimationState nowAnimationState = NowAnimationState;
		if ((bool)nowAnimationState)
		{
			nowAnimationState.speed = speed;
		}
	}

	public void ReStart()
	{
		ANMCTRLINFOST info = GetInfo();
		if (info != null)
		{
			info.isStop = false;
		}
		AnimationState nowAnimationState = NowAnimationState;
		if ((bool)nowAnimationState)
		{
			nowAnimationState.speed = data.speed;
			Play(string.Empty, nowAnimationState.time);
		}
	}

	public void Stop()
	{
		ANMCTRLINFOST info = GetInfo();
		if (info != null)
		{
			info.isStop = true;
		}
		AnimationState nowAnimationState = NowAnimationState;
		if ((bool)nowAnimationState)
		{
			nowAnimationState.speed = 0f;
			float time = nowAnimationState.time;
			animation.Stop();
			nowAnimationState.time = time;
		}
	}

	public int GetNowPtn()
	{
		return data.NowPtn;
	}

	public void Play(int id, float time = -1f, float fadeSpeed = 0.3f, int layer = 0, WrapMode mode = WrapMode.Default)
	{
		Play(GetID(id), time, fadeSpeed, layer, mode);
	}

	public void Play(string name = "", float time = -1f, float fadeSpeed = 0.3f, int layer = 0, WrapMode mode = WrapMode.Default)
	{
		if (name == string.Empty)
		{
			name = GetID(data.NowPtn);
		}
		AnimationState animationState = animation[name];
		if (animationState == null)
		{
			return;
		}
		AnimationState animationState2 = animation[GetID(data.BeforePtn)];
		if ((bool)animationState2)
		{
			animationState2.wrapMode = GetInfo(data.BeforePtn).baseMode;
		}
		for (int i = 0; i < data.info.Length; i++)
		{
			if (name == GetID(i))
			{
				data.BeforePtn = data.NowPtn;
				data.NowPtn = i;
				break;
			}
		}
		animationState.speed = data.speed;
		ANMCTRLINFOST info = GetInfo();
		if (info.isStop)
		{
			animationState.speed = 0f;
			return;
		}
		if (info.isReverse)
		{
			if (animationState.speed > 0f)
			{
				animationState.speed *= -1f;
			}
			animationState.time = animationState.length - time;
			animationState.time = Mathf.Clamp(animationState.time, 0f, animationState.length);
		}
		else if (time >= 0f)
		{
			animationState.time = time;
		}
		animationState.layer = layer;
		if (mode != 0)
		{
			animationState.wrapMode = mode;
		}
		if (fadeSpeed == 0f)
		{
			animation.Play(name);
			return;
		}
		if (animationState.wrapMode == WrapMode.Default)
		{
			animationState.wrapMode = WrapMode.ClampForever;
		}
		animation.CrossFade(name, fadeSpeed);
	}

	public void PlayOverride(int id, float time = -1f, float fadeSpeed = 0.3f, int layer = 1)
	{
		PlayOverride(GetID(id), time, fadeSpeed, layer);
	}

	public void PlayOverride(string name = "", float time = -1f, float fadeSpeed = 0.3f, int layer = 1)
	{
		Play(name, time, fadeSpeed, layer, WrapMode.Once);
	}

	public void Fusion(int id, float weight = 0.5f, int layer = 1)
	{
		Fusion(GetID(id), weight, layer);
	}

	public void Fusion(string name = "", float weight = 0.5f, int layer = 1)
	{
		if (name == string.Empty)
		{
			name = GetID(data.NowPtn);
		}
		AnimationState animationState = animation[name];
		if (!(animationState == null))
		{
			Play(name, -1f, 0f, layer);
			animationState.weight = weight;
		}
	}

	private void PlaySync(Animation anime, int num)
	{
		AnimationState animationState = anime[GetID(num)];
		AnimationState nowAnimationState = NowAnimationState;
		if ((bool)animationState && (bool)nowAnimationState)
		{
			nowAnimationState.time = animationState.time;
		}
	}

	public ANMCTRLINFOST GetInfo(int nPtn = -1)
	{
		if (nPtn == -1)
		{
			return data.info[data.NowPtn];
		}
		if ((uint)nPtn >= data.info.Length)
		{
			return null;
		}
		return data.info[nPtn];
	}

	public bool SetNowFrame(float nowFrame)
	{
		AnimationState nowAnimationState = NowAnimationState;
		if (!nowAnimationState)
		{
			return false;
		}
		nowAnimationState.time = nowFrame;
		return true;
	}

	public bool GetNowFrame(ref float nowFrame)
	{
		AnimationState nowAnimationState = NowAnimationState;
		if (!nowAnimationState)
		{
			return false;
		}
		nowFrame = nowAnimationState.time;
		return true;
	}

	public bool GetNowEndFrame(ref float endFrame)
	{
		AnimationState nowAnimationState = NowAnimationState;
		if (!nowAnimationState)
		{
			return false;
		}
		endFrame = nowAnimationState.length;
		return true;
	}

	public bool GetPtnNow(ref int nowPtn)
	{
		nowPtn = data.NowPtn;
		return true;
	}

	public bool GetPtnBefore(ref int beforePtn)
	{
		beforePtn = data.BeforePtn;
		return true;
	}

	public bool GetName(int nPtn, ref string name)
	{
		name = GetID(nPtn);
		return name != string.Empty;
	}

	public bool GetEndFrame(int nPtn, ref float endFrame)
	{
		AnimationState animationState = animation[GetID(nPtn)];
		if (!animationState)
		{
			return false;
		}
		endFrame = animationState.length;
		return true;
	}

	public bool SetSpeed(int nPtn, float speed)
	{
		AnimationState animationState = animation[GetID(nPtn)];
		if (!animationState)
		{
			return false;
		}
		animationState.speed = speed;
		return true;
	}

	public bool GetSpeed(int nPtn, ref float speed)
	{
		AnimationState animationState = animation[GetID(nPtn)];
		if (!animationState)
		{
			return false;
		}
		speed = animationState.speed;
		return true;
	}

	public bool SetWrapMode(int nPtn, WrapMode mode)
	{
		AnimationState animationState = animation[GetID(nPtn)];
		if (!animationState)
		{
			return false;
		}
		animationState.wrapMode = mode;
		return true;
	}

	public bool GetWrapMode(int nPtn, ref WrapMode mode)
	{
		AnimationState animationState = animation[GetID(nPtn)];
		if (!animationState)
		{
			return false;
		}
		mode = animationState.wrapMode;
		return true;
	}

	public bool SetLoopCnt(int nPtn, int loopCnt)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		info.LoopCnt = loopCnt;
		return true;
	}

	public bool GetLoopCnt(int nPtn, ref int loopCnt)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		loopCnt = info.LoopCnt;
		return true;
	}

	public bool SetLoopNum(int nPtn, int loopNum)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		info.LoopNum = loopNum;
		return true;
	}

	public bool GetLoopNum(int nPtn, ref int loopNum)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		loopNum = info.LoopNum;
		return true;
	}

	public bool SetLinkNo(int nPtn, int LinkNo)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		info.LinkNo = LinkNo;
		return true;
	}

	public bool GetLinkNo(int nPtn, ref int LinkNo)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		LinkNo = info.LinkNo;
		return true;
	}

	public bool SetReverseFlag(int nPtn, bool isReverse)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		info.isReverse = isReverse;
		return true;
	}

	public bool GetReverseFlag(int nPtn, ref bool isReverse)
	{
		ANMCTRLINFOST info = GetInfo(nPtn);
		if (info == null)
		{
			return false;
		}
		isReverse = info.isReverse;
		return true;
	}
}
