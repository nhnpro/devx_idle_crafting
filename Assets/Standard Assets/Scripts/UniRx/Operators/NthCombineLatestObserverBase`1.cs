using System;

namespace UniRx.Operators
{
	internal abstract class NthCombineLatestObserverBase<T> : OperatorObserverBase<T, T>, ICombineLatestObservable
	{
		private readonly int length;

		private bool isAllValueStarted;

		private readonly bool[] isStarted;

		private readonly bool[] isCompleted;

		public NthCombineLatestObserverBase(int length, IObserver<T> observer, IDisposable cancel)
			: base(observer, cancel)
		{
			this.length = length;
			isAllValueStarted = false;
			isStarted = new bool[length];
			isCompleted = new bool[length];
		}

		public abstract T GetResult();

		public void Publish(int index)
		{
			isStarted[index] = true;
			if (isAllValueStarted)
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
				return;
			}
			bool flag = true;
			for (int i = 0; i < length; i++)
			{
				if (!isStarted[i])
				{
					flag = false;
					break;
				}
			}
			isAllValueStarted = flag;
			if (isAllValueStarted)
			{
				T val2 = default(T);
				try
				{
					val2 = GetResult();
				}
				catch (Exception error2)
				{
					try
					{
						observer.OnError(error2);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				OnNext(val2);
				return;
			}
			bool flag2 = true;
			for (int j = 0; j < length; j++)
			{
				if (j != index && !isCompleted[j])
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
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
