using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ZipObserver<T> : IObserver<T>
	{
		private readonly object gate;

		private readonly IZipObservable parent;

		private readonly int index;

		private readonly Queue<T> queue;

		public ZipObserver(object gate, IZipObservable parent, int index, Queue<T> queue)
		{
			this.gate = gate;
			this.parent = parent;
			this.index = index;
			this.queue = queue;
		}

		public void OnNext(T value)
		{
			lock (gate)
			{
				queue.Enqueue(value);
				parent.Dequeue(index);
			}
		}

		public void OnError(Exception error)
		{
			lock (gate)
			{
				parent.Fail(error);
			}
		}

		public void OnCompleted()
		{
			lock (gate)
			{
				parent.Done(index);
			}
		}
	}
}
