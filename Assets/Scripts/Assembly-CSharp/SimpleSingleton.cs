public class SimpleSingleton<T> where T : class, new()
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}
			return instance;
		}
	}

	private SimpleSingleton()
	{
	}
}
