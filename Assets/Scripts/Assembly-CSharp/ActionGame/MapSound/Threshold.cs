using System;
using UnityEngine;

namespace ActionGame.MapSound
{
	[Serializable]
	public struct Threshold
	{
		public float min;

		public float max;

		public float RandomValue
		{
			get
			{
				return UnityEngine.Random.Range(min, max);
			}
		}

		public Threshold(float minValue, float maxValue)
		{
			min = minValue;
			max = maxValue;
		}

		public float Lerp(float t)
		{
			return Mathf.Lerp(min, max, t);
		}

		public bool IsRange(float value)
		{
			return value >= min && value <= max;
		}

		public override bool Equals(object obj)
		{
			if (obj is Threshold)
			{
				Threshold threshold = (Threshold)obj;
				return min == threshold.min && max == threshold.max;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return min.GetHashCode() ^ (max.GetHashCode() << 2);
		}

		public static bool operator ==(Threshold a, Threshold b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Threshold a, Threshold b)
		{
			return !a.Equals(b);
		}
	}
}
