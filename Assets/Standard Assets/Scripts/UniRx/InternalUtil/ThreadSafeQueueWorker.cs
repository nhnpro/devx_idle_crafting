using System;

namespace UniRx.InternalUtil
{
	public class ThreadSafeQueueWorker
	{
		private const int MaxArrayLength = 2146435071;

		private const int InitialSize = 16;

		private object gate = new object();

		private bool dequing;

		private int actionListCount;

		private Action<object>[] actionList = new Action<object>[16];

		private object[] actionStates = new object[16];

		private int waitingListCount;

		private Action<object>[] waitingList = new Action<object>[16];

		private object[] waitingStates = new object[16];

		public void Enqueue(Action<object> action, object state)
		{
			lock (gate)
			{
				if (dequing)
				{
					if (waitingList.Length == waitingListCount)
					{
						int num = waitingListCount * 2;
						if ((uint)num > 2146435071u)
						{
							num = 2146435071;
						}
						Action<object>[] destinationArray = new Action<object>[num];
						object[] destinationArray2 = new object[num];
						Array.Copy(waitingList, destinationArray, waitingListCount);
						Array.Copy(waitingStates, destinationArray2, waitingListCount);
						waitingList = destinationArray;
						waitingStates = destinationArray2;
					}
					waitingList[waitingListCount] = action;
					waitingStates[waitingListCount] = state;
					waitingListCount++;
				}
				else
				{
					if (actionList.Length == actionListCount)
					{
						int num2 = actionListCount * 2;
						if ((uint)num2 > 2146435071u)
						{
							num2 = 2146435071;
						}
						Action<object>[] destinationArray3 = new Action<object>[num2];
						object[] destinationArray4 = new object[num2];
						Array.Copy(actionList, destinationArray3, actionListCount);
						Array.Copy(actionStates, destinationArray4, actionListCount);
						actionList = destinationArray3;
						actionStates = destinationArray4;
					}
					actionList[actionListCount] = action;
					actionStates[actionListCount] = state;
					actionListCount++;
				}
			}
		}

		public void ExecuteAll(Action<Exception> unhandledExceptionCallback)
		{
			lock (gate)
			{
				if (actionListCount == 0)
				{
					return;
				}
				dequing = true;
			}
			for (int i = 0; i < actionListCount; i++)
			{
				Action<object> action = actionList[i];
				object obj = actionStates[i];
				try
				{
					action(obj);
				}
				catch (Exception obj2)
				{
					unhandledExceptionCallback(obj2);
				}
				finally
				{
					actionList[i] = null;
					actionStates[i] = null;
				}
			}
			lock (gate)
			{
				dequing = false;
				Action<object>[] array = actionList;
				object[] array2 = actionStates;
				actionListCount = waitingListCount;
				actionList = waitingList;
				actionStates = waitingStates;
				waitingListCount = 0;
				waitingList = array;
				waitingStates = array2;
			}
		}
	}
}
