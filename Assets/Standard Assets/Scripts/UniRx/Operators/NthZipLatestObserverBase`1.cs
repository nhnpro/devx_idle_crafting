using System;

namespace UniRx.Operators
{
	internal abstract class NthZipLatestObserverBase<T> : OperatorObserverBase<T, T>, IZipLatestObservable
	{
		private readonly int length;

		private readonly bool[] isStarted;

		private readonly bool[] isCompleted;

		public NthZipLatestObserverBase(int length, IObserver<T> observer, IDisposable cancel)
			: base(observer, cancel)
		{
			this.length = length;
			isStarted = new bool[length];
			isCompleted = new bool[length];
		}

		public abstract T GetResult();

		public void Publish(int index)
		{
			isStarted[index] = true;
			bool flag = false;
			bool flag2 = true;
			for (int i = 0; i < length; i++)
			{
				if (!isStarted[i])
				{
					flag2 = false;
					break;
				}
				if (i != index && isCompleted[i])
				{
					flag = true;
				}
			}
			if (flag2)
			{
				T val = default(T);
				try
				{
					val = GetResult();
				}
				catch (Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				OnNext(val);
				if (flag)
				{
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
				}
				else
				{
					Array.Clear(isStarted, 0, length);
				}
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < length)
				{
					if (num != index && isCompleted[num] && !isStarted[num])
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			try
			{
				observer.OnCompleted();
			}
			finally
			{
				Dispose();
			}
		}

		public void Done(int index)
		{
			isCompleted[index] = true;
			bool flag = true;
			for (int i = 0; i < length; i++)
			{
				if (!isCompleted[i])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}

		public void Fail(Exception error)
		{
			try
			{
				observer.OnError(error);
			}
			finally
			{
				Dispose();
			}
		}
	}
}
