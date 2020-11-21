using System;

public abstract class DisposableSingleton<T> : Singleton<T>, IDisposable where T : class, new()
{
	public override void Release()
	{
		base.Release();
		Dispose();
	}

	public abstract void Dispose();
}
