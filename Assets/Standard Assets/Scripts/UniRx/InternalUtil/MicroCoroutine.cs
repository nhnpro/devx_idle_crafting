using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.InternalUtil
{
	public class MicroCoroutine
	{
		private const int InitialSize = 16;

		private readonly object runningAndQueueLock = new object();

		private readonly object arrayLock = new object();

		private readonly Action<Exception> unhandledExceptionCallback;

		private int tail;

		private bool running;

		private IEnumerator[] coroutines = new IEnumerator[16];

		private Queue<IEnumerator> waitQueue = new Queue<IEnumerator>();

		public MicroCoroutine(Action<Exception> unhandledExceptionCallback)
		{
			this.unhandledExceptionCallback = unhandledExceptionCallback;
		}

		public void AddCoroutine(IEnumerator enumerator)
		{
			lock (runningAndQueueLock)
			{
				if (running)
				{
					waitQueue.Enqueue(enumerator);
					return;
				}
			}
			lock (arrayLock)
			{
				if (coroutines.Length == tail)
				{
					Array.Resize(ref coroutines, checked(tail * 2));
				}
				coroutines[tail++] = enumerator;
			}
		}

		public void Run()
		{
			lock (runningAndQueueLock)
			{
				running = true;
			}
			lock (arrayLock)
			{
				int num = tail - 1;
				for (int i = 0; i < coroutines.Length; i++)
				{
					IEnumerator enumerator = coroutines[i];
					if (enumerator != null)
					{
						try
						{
							if (!enumerator.MoveNext())
							{
								coroutines[i] = null;
								goto IL_011a;
							}
						}
						catch (Exception obj)
						{
							coroutines[i] = null;
							try
							{
								unhandledExceptionCallback(obj);
							}
							catch
							{
							}
							goto IL_011a;
						}
						continue;
					}
					goto IL_011a;
					IL_011a:
					while (i < num)
					{
						IEnumerator enumerator2 = coroutines[num];
						if (enumerator2 != null)
						{
							try
							{
								if (enumerator2.MoveNext())
								{
									coroutines[i] = enumerator2;
									coroutines[num] = null;
									num--;
									goto IL_0132;
								}
								coroutines[num] = null;
								num--;
							}
							catch (Exception obj3)
							{
								coroutines[num] = null;
								num--;
								try
								{
									unhandledExceptionCallback(obj3);
								}
								catch
								{
								}
							}
						}
						else
						{
							num--;
						}
					}
					tail = i;
					break;
					IL_0132:;
				}
				lock (runningAndQueueLock)
				{
					running = false;
					while (waitQueue.Count != 0)
					{
						if (coroutines.Length == tail)
						{
							Array.Resize(ref coroutines, checked(tail * 2));
						}
						coroutines[tail++] = waitQueue.Dequeue();
					}
				}
			}
		}
	}
}
