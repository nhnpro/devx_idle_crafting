using System;

namespace UniRx
{
	public struct CancellationToken
	{
		private readonly ICancelable source;

		public static readonly CancellationToken Empty = new CancellationToken(null);

		public static readonly CancellationToken None = new CancellationToken(null);

		public bool IsCancellationRequested => source != null && source.IsDisposed;

		public CancellationToken(ICancelable source)
		{
			this.source = source;
		}

		public void ThrowIfCancellationRequested()
		{
			if (IsCancellationRequested)
			{
				throw new OperationCanceledException();
			}
		}
	}
}
