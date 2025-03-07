using Illusion;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class NullSet : CommandBase
	{
		private enum Type
		{
			Base = 0,
			Camera = 1,
			Chara = 2
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Name", "Type" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					string.Empty,
					Type.Base.ToString()
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string name = args[num++];
			string self = args[num++];
			switch ((Type)self.Check(true, Utils.Enum<Type>.Names))
			{
			case Type.Base:
				Set(base.scenario.commandController.BasePositon, name);
				break;
			case Type.Camera:
			{
				Transform cameraPosition = base.scenario.commandController.CameraPosition;
				Vector3 position = cameraPosition.position;
				Quaternion rotation = cameraPosition.rotation;
				cameraPosition.position = Vector3.zero;
				cameraPosition.rotation = Quaternion.identity;
				Set(base.scenario.AdvCamera.transform, name);
				cameraPosition.SetPositionAndRotation(position, rotation);
				break;
			}
			case Type.Chara:
				Set(base.scenario.commandController.Character, name);
				break;
			}
		}

		private void Set(Transform transform, string name)
		{
			Transform value;
			if (base.scenario.commandController.NullDic.TryGetValue(name, out value))
			{
				transform.SetPositionAndRotation(value.position, value.rotation);
			}
		}
	}
}
