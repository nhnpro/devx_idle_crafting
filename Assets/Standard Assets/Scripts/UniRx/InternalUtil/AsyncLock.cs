using System;
using System.Collections.Generic;

namespace UniRx.InternalUtil
{
	internal sealed class AsyncLock : IDisposable
	{
		private readonly Queue<Action> queue = new Queue<Action>();

		private bool isAcquired;

		private bool hasFaulted;

		public void Wait(Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bool flag = false;
			lock (queue)
			{
				if (!hasFaulted)
				{
					queue.Enqueue(action);
					flag = !isAcquired;
					isAcquired = true;
				}
			}
			if (flag)
			{
				while (true)
				{
					Action action2 = null;
					lock (queue)
					{
						if (queue.Count <= 0)
						{
							isAcquired = false;
							return;
						}
						action2 = queue.Dequeue();
					}
					try
					{
						action2();
					}
					catch
					{
						lock (queue)
						{
							queue.Clear();
							hasFaulted = true;
						}
						throw;
					}
				}
			}
		}

		public void Dispose()
		{
			lock (queue)
			{
				queue.Clear();
				hasFaulted = true;
			}
		}
	}
}
