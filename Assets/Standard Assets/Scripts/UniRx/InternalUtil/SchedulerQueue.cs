using System;

namespace UniRx.InternalUtil
{
	internal class SchedulerQueue
	{
		private readonly PriorityQueue<ScheduledItem> _queue;

		public int Count => _queue.Count;

		public SchedulerQueue()
			: this(1024)
		{
		}

		public SchedulerQueue(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			_queue = new PriorityQueue<ScheduledItem>(capacity);
		}

		public void Enqueue(ScheduledItem scheduledItem)
		{
			_queue.Enqueue(scheduledItem);
		}

		public bool Remove(ScheduledItem scheduledItem)
		{
			return _queue.Remove(scheduledItem);
		}

		public ScheduledItem Dequeue()
		{
			return _queue.Dequeue();
		}

		public ScheduledItem Peek()
		{
			return _queue.Peek();
		}
	}
}
