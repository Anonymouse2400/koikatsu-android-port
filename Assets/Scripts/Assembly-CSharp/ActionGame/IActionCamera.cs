using UnityEngine;

namespace ActionGame
{
	public interface IActionCamera
	{
		Vector3 Angle { get; set; }

		bool isControl { get; }
	}
}
