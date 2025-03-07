using Illusion;
using UnityEngine;

namespace ADV.Commands.Camera
{
	public class AnimeSetParam : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Name", "Value" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string name = args[num++];
			string text = args[num++];
			Animator component = base.scenario.AdvCamera.GetComponent<Animator>();
			if (!(component == null))
			{
				AnimatorControllerParameter animeParam = Utils.Animator.GetAnimeParam(name, component);
				switch (animeParam.type)
				{
				case AnimatorControllerParameterType.Float:
					component.SetFloat(name, float.Parse(text));
					break;
				case AnimatorControllerParameterType.Int:
					component.SetInteger(name, int.Parse(text));
					break;
				case AnimatorControllerParameterType.Bool:
					component.SetBool(name, bool.Parse(text));
					break;
				}
			}
		}
	}
}
