using System.Linq;
using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetParent : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "ParentName", "ChildName", "isWorldPositionStays" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					string.Empty,
					string.Empty,
					bool.FalseString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			UnityEngine.Camera advCamera = base.scenario.AdvCamera;
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl != null)
			{
				baseCamCtrl.enabled = false;
			}
			int num = 0;
			string text = args[num++];
			string childName = args[num++];
			int result = 0;
			GameObject gameObject = null;
			if (!int.TryParse(text, out result))
			{
				gameObject = GameObject.Find(text);
			}
			Transform parent = gameObject.transform.GetComponentsInChildren<Transform>().FirstOrDefault((Transform p) => p.name == childName);
			bool worldPositionStays = bool.Parse(args[num++]);
			advCamera.transform.SetParent(parent, worldPositionStays);
		}
	}
}
