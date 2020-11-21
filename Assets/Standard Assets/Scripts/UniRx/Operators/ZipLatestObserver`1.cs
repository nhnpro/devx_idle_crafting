using System;

namespace UniRx.Operators
{
	internal class ZipLatestObserver<T> : IObserver<T>
	{
		private readonly object gate;

		private readonly IZipLatestObservable parent;

		private readonly int index;

		private T value;

		public T Value => value;

		public ZipLatestObserver(object gate, IZipLatestObservable parent, int index)
		{
			this.gate = gate;
			this.parent = parent;
			this.index = index;
		}

		public void OnNext(T value)
		{
			lock (gate)
			{
				this.value = value;
				parent.Publish(index);
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
