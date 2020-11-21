using System;

namespace UniRx
{
	public sealed class SerialDisposable : IDisposable, ICancelable
	{
		private readonly object gate = new object();

		private IDisposable current;

		private bool disposed;

		public bool IsDisposed
		{
			get
			{
				lock (gate)
				{
					return disposed;
				}
			}
		}

		public IDisposable Disposable
		{
			get
			{
				return current;
			}
			set
			{
				bool flag = false;
				IDisposable disposable = null;
				lock (gate)
				{
					flag = disposed;
					if (!flag)
					{
						disposable = current;
						current = value;
					}
				}
				disposable?.Dispose();
				if (flag)
				{
					value?.Dispose();
				}
			}
		}

		public void Dispose()
		{
			IDisposable disposable = null;
			lock (gate)
			{
				if (!disposed)
				{
					disposed = true;
					disposable = current;
					current = null;
				}
			}
			disposable?.Dispose();
		}
	}
}
