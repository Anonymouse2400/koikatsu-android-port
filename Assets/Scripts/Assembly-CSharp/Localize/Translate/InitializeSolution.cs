using System.Collections.Generic;

namespace Localize.Translate
{
	public class InitializeSolution
	{
		public interface IInitializable
		{
			bool initialized { get; }

			void Initialize();
		}

		private readonly List<IInitializable> list = new List<IInitializable>();

		public bool initialized { get; private set; }

		public void Add(IInitializable initializable)
		{
			if (initialized)
			{
				Solution(initializable);
			}
			else
			{
				list.Add(initializable);
			}
		}

		public void Execute()
		{
			if (!initialized)
			{
				list.ForEach(Solution);
				list.Clear();
				initialized = true;
			}
		}

		private void Solution(IInitializable initializable)
		{
			if (!initializable.initialized)
			{
				initializable.Initialize();
			}
		}
	}
}
