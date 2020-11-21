using System;
using System.Collections;

namespace UniRx.Operators
{
	internal abstract class NthZipObserverBase<T> : OperatorObserverBase<T, T>, IZipObservable
	{
		private ICollection[] queues;

		private bool[] isDone;

		private int length;

		public NthZipObserverBase(UniRx.IObserver<T> observer, IDisposable cancel)
			: base(observer, cancel)
		{
		}

		protected void SetQueue(ICollection[] queues)
		{
			this.queues = queues;
			length = queues.Length;
			isDone = new bool[length];
		}

		public abstract T GetResult();

		public void Dequeue(int index)
		{
			bool flag = true;
			for (int i = 0; i < length; i++)
			{
				if (queues[i].Count == 0)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				bool flag2 = true;
				for (int j = 0; j < length; j++)
				{
					if (j != index && !isDone[j])
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
			else
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
			}
		}

		public void Done(int index)
		{
			isDone[index] = true;
			bool flag = true;
			for (int i = 0; i < length; i++)
			{
				if (!isDone[i])
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
