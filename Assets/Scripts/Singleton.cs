public abstract class Singleton<T> : IReleasable where T : class, new()
{
	public static T Instance
	{
		get;
		private set;
	}

	public static void Construct()
	{
		Instance = new T();
		SingletonManager.Register((IReleasable)Instance);
	}

	public virtual void Release()
	{
		Instance = (T)null;
	}
}
