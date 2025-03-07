using System;
using System.Text.RegularExpressions;

namespace Config
{
	public class SoundData
	{
		private int _volume;

		private bool _switch;

		public int Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				bool flag = _volume != value;
				_volume = value;
				if (flag && !this.ChangeEvent.IsNullOrEmpty())
				{
					this.ChangeEvent(this);
				}
			}
		}

		public bool Switch
		{
			get
			{
				return _switch;
			}
			set
			{
				bool flag = _switch != value;
				_switch = value;
				if (flag && !this.ChangeEvent.IsNullOrEmpty())
				{
					this.ChangeEvent(this);
				}
			}
		}

		public event Action<SoundData> ChangeEvent;

		public SoundData()
		{
			_volume = 100;
			_switch = true;
		}

		public float GetVolume()
		{
			return (!Switch) ? 0f : ((float)Volume * 0.01f);
		}

		public static implicit operator string(SoundData _sd)
		{
			return string.Format("Volume[{0}] : Switch[{1}]", _sd.Volume, _sd.Switch);
		}

		public void Parse(string _str)
		{
			Match match = Regex.Match(_str, "Volume\\[([0-9]*)\\] : Switch\\[([a-zA-Z]*)\\]");
			if (match.Success)
			{
				Volume = int.Parse(match.Groups[1].Value);
				Switch = bool.Parse(match.Groups[2].Value);
			}
		}
	}
}
