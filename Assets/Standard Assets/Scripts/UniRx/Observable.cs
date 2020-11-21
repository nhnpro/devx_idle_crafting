using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx.Operators;
using UniRx.Triggers;
using UnityEngine;

namespace UniRx
{
	public static class Observable
	{
		private class ConnectableObservable<T> : IConnectableObservable<T>, UniRx.IObservable<T>
		{
			private class Connection : IDisposable
			{
				private readonly ConnectableObservable<T> parent;

				private IDisposable subscription;

				public Connection(ConnectableObservable<T> parent, IDisposable subscription)
				{
					this.parent = parent;
					this.subscription = subscription;
				}

				public void Dispose()
				{
					lock (parent.gate)
					{
						if (subscription != null)
						{
							subscription.Dispose();
							subscription = null;
							parent.connection = null;
						}
					}
				}
			}

			private readonly UniRx.IObservable<T> source;

			private readonly ISubject<T> subject;

			private readonly object gate = new object();

			private Connection connection;

			public ConnectableObservable(UniRx.IObservable<T> source, ISubject<T> subject)
			{
				this.source = source.AsObservable();
				this.subject = subject;
			}

			public IDisposable Connect()
			{
				lock (gate)
				{
					if (connection == null)
					{
						IDisposable subscription = source.Subscribe(subject);
						connection = new Connection(this, subscription);
					}
					return connection;
				}
			}

			public IDisposable Subscribe(UniRx.IObserver<T> observer)
			{
				return subject.Subscribe(observer);
			}
		}

		private class EveryAfterUpdateInvoker : IEnumerator
		{
			private long count = -1L;

			private readonly IObserver<long> observer;

			private readonly CancellationToken cancellationToken;

			public object Current => null;

			public EveryAfterUpdateInvoker(UniRx.IObserver<long> observer, CancellationToken cancellationToken)
			{
				this.observer = observer;
				this.cancellationToken = cancellationToken;
			}

			public bool MoveNext()
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					if (count != -1)
					{
						observer.OnNext(count++);
					}
					else
					{
						count++;
					}
					return true;
				}
				return false;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1);

		private static readonly HashSet<Type> YieldInstructionTypes = new HashSet<Type>
		{
			typeof(WWW),
			typeof(WaitForEndOfFrame),
			typeof(WaitForFixedUpdate),
			typeof(WaitForSeconds),
			typeof(AsyncOperation),
			typeof(Coroutine)
		};

		[CompilerGenerated]
		private static Func<IObserver<long>, CancellationToken, IEnumerator> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<IObserver<long>, CancellationToken, IEnumerator> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Func<IObserver<long>, CancellationToken, IEnumerator> _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static Func<IObserver<Unit>, CancellationToken, IEnumerator> _003C_003Ef__mg_0024cache3;

		private static UniRx.IObservable<T> AddRef<T>(UniRx.IObservable<T> xs, RefCountDisposable r)
		{
			return Create((UniRx.IObserver<T> observer) => new CompositeDisposable(r.GetDisposable(), xs.Subscribe(observer)));
		}

		public static UniRx.IObservable<TSource> Scan<TSource>(this UniRx.IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
		{
			return new ScanObservable<TSource>(source, accumulator);
		}

		public static UniRx.IObservable<TAccumulate> Scan<TSource, TAccumulate>(this UniRx.IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
		{
			return new ScanObservable<TSource, TAccumulate>(source, seed, accumulator);
		}

		public static UniRx.IObservable<TSource> Aggregate<TSource>(this UniRx.IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
		{
			return new AggregateObservable<TSource>(source, accumulator);
		}

		public static UniRx.IObservable<TAccumulate> Aggregate<TSource, TAccumulate>(this UniRx.IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
		{
			return new AggregateObservable<TSource, TAccumulate>(source, seed, accumulator);
		}

		public static UniRx.IObservable<TResult> Aggregate<TSource, TAccumulate, TResult>(this UniRx.IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector)
		{
			return new AggregateObservable<TSource, TAccumulate, TResult>(source, seed, accumulator, resultSelector);
		}

		public static IConnectableObservable<T> Multicast<T>(this UniRx.IObservable<T> source, ISubject<T> subject)
		{
			return new ConnectableObservable<T>(source, subject);
		}

		public static IConnectableObservable<T> Publish<T>(this UniRx.IObservable<T> source)
		{
			return source.Multicast(new Subject<T>());
		}

		public static IConnectableObservable<T> Publish<T>(this UniRx.IObservable<T> source, T initialValue)
		{
			return source.Multicast(new BehaviorSubject<T>(initialValue));
		}

		public static IConnectableObservable<T> PublishLast<T>(this UniRx.IObservable<T> source)
		{
			return source.Multicast(new AsyncSubject<T>());
		}

		public static IConnectableObservable<T> Replay<T>(this UniRx.IObservable<T> source)
		{
			return source.Multicast(new ReplaySubject<T>());
		}

		public static IConnectableObservable<T> Replay<T>(this UniRx.IObservable<T> source, IScheduler scheduler)
		{
			return source.Multicast(new ReplaySubject<T>(scheduler));
		}

		public static IConnectableObservable<T> Replay<T>(this UniRx.IObservable<T> source, int bufferSize)
		{
			return source.Multicast(new ReplaySubject<T>(bufferSize));
		}

		public static IConnectableObservable<T> Replay<T>(this UniRx.IObservable<T> source, int bufferSize, IScheduler scheduler)
		{
			return source.Multicast(new ReplaySubject<T>(bufferSize, scheduler));
		}

		public static IConnectableObservable<T> Replay<T>(this UniRx.IObservable<T> source, TimeSpan window)
		{
			return source.Multicast(new ReplaySubject<T>(window));
		}

		public static IConnectableObservable<T> Replay<T>(this UniRx.IObservable<T> source, TimeSpan window, IScheduler scheduler)
		{
			return source.Multicast(new ReplaySubject<T>(window, scheduler));
		}

		public static IConnectableObservable<T> Replay<T>(this UniRx.IObservable<T> source, int bufferSize, TimeSpan window, IScheduler scheduler)
		{
			return source.Multicast(new ReplaySubject<T>(bufferSize, window, scheduler));
		}

		public static UniRx.IObservable<T> RefCount<T>(this IConnectableObservable<T> source)
		{
			return new RefCountObservable<T>(source);
		}

		public static UniRx.IObservable<T> Share<T>(this UniRx.IObservable<T> source)
		{
			return source.Publish().RefCount();
		}

		public static T Wait<T>(this UniRx.IObservable<T> source)
		{
			return new Wait<T>(source, InfiniteTimeSpan).Run();
		}

		public static T Wait<T>(this UniRx.IObservable<T> source, TimeSpan timeout)
		{
			return new Wait<T>(source, timeout).Run();
		}

		private static IEnumerable<UniRx.IObservable<T>> CombineSources<T>(UniRx.IObservable<T> first, UniRx.IObservable<T>[] seconds)
		{
			yield return first;
			for (int i = 0; i < seconds.Length; i++)
			{
				yield return seconds[i];
			}
		}

		public static UniRx.IObservable<TSource> Concat<TSource>(params UniRx.IObservable<TSource>[] sources)
		{
			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}
			return new ConcatObservable<TSource>(sources);
		}

		public static UniRx.IObservable<TSource> Concat<TSource>(this IEnumerable<UniRx.IObservable<TSource>> sources)
		{
			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}
			return new ConcatObservable<TSource>(sources);
		}

		public static UniRx.IObservable<TSource> Concat<TSource>(this UniRx.IObservable<UniRx.IObservable<TSource>> sources)
		{
			return sources.Merge(1);
		}

		public static UniRx.IObservable<TSource> Concat<TSource>(this UniRx.IObservable<TSource> first, params UniRx.IObservable<TSource>[] seconds)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (seconds == null)
			{
				throw new ArgumentNullException("seconds");
			}
			ConcatObservable<TSource> concatObservable = first as ConcatObservable<TSource>;
			if (concatObservable != null)
			{
				return concatObservable.Combine(seconds);
			}
			return CombineSources(first, seconds).Concat();
		}

		public static UniRx.IObservable<TSource> Merge<TSource>(this IEnumerable<UniRx.IObservable<TSource>> sources)
		{
			return sources.Merge(Scheduler.DefaultSchedulers.ConstantTimeOperations);
		}

		public static UniRx.IObservable<TSource> Merge<TSource>(this IEnumerable<UniRx.IObservable<TSource>> sources, IScheduler scheduler)
		{
			return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler == Scheduler.CurrentThread);
		}

		public static UniRx.IObservable<TSource> Merge<TSource>(this IEnumerable<UniRx.IObservable<TSource>> sources, int maxConcurrent)
		{
			return sources.Merge(maxConcurrent, Scheduler.DefaultSchedulers.ConstantTimeOperations);
		}

		public static UniRx.IObservable<TSource> Merge<TSource>(this IEnumerable<UniRx.IObservable<TSource>> sources, int maxConcurrent, IScheduler scheduler)
		{
			return new MergeObservable<TSource>(sources.ToObservable(scheduler), maxConcurrent, scheduler == Scheduler.CurrentThread);
		}

		public static UniRx.IObservable<TSource> Merge<TSource>(params UniRx.IObservable<TSource>[] sources)
		{
			return Merge(Scheduler.DefaultSchedulers.ConstantTimeOperations, sources);
		}

		public static UniRx.IObservable<TSource> Merge<TSource>(IScheduler scheduler, params UniRx.IObservable<TSource>[] sources)
		{
			return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler == Scheduler.CurrentThread);
		}

		public static UniRx.IObservable<T> Merge<T>(this UniRx.IObservable<T> first, params UniRx.IObservable<T>[] seconds)
		{
			return CombineSources(first, seconds).Merge();
		}

		public static UniRx.IObservable<T> Merge<T>(this UniRx.IObservable<T> first, UniRx.IObservable<T> second, IScheduler scheduler)
		{
			return Merge<T>(scheduler, first, second);
		}

		public static UniRx.IObservable<T> Merge<T>(this UniRx.IObservable<UniRx.IObservable<T>> sources)
		{
			return new MergeObservable<T>(sources, isRequiredSubscribeOnCurrentThread: false);
		}

		public static UniRx.IObservable<T> Merge<T>(this UniRx.IObservable<UniRx.IObservable<T>> sources, int maxConcurrent)
		{
			return new MergeObservable<T>(sources, maxConcurrent, isRequiredSubscribeOnCurrentThread: false);
		}

		public static UniRx.IObservable<TResult> Zip<TLeft, TRight, TResult>(this UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
		{
			return new ZipObservable<TLeft, TRight, TResult>(left, right, selector);
		}

		public static UniRx.IObservable<IList<T>> Zip<T>(this IEnumerable<UniRx.IObservable<T>> sources)
		{
			return Zip(sources.ToArray());
		}

		public static UniRx.IObservable<IList<T>> Zip<T>(params UniRx.IObservable<T>[] sources)
		{
			return new ZipObservable<T>(sources);
		}

		public static UniRx.IObservable<TR> Zip<T1, T2, T3, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, ZipFunc<T1, T2, T3, TR> resultSelector)
		{
			return new ZipObservable<T1, T2, T3, TR>(source1, source2, source3, resultSelector);
		}

		public static UniRx.IObservable<TR> Zip<T1, T2, T3, T4, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, ZipFunc<T1, T2, T3, T4, TR> resultSelector)
		{
			return new ZipObservable<T1, T2, T3, T4, TR>(source1, source2, source3, source4, resultSelector);
		}

		public static UniRx.IObservable<TR> Zip<T1, T2, T3, T4, T5, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, ZipFunc<T1, T2, T3, T4, T5, TR> resultSelector)
		{
			return new ZipObservable<T1, T2, T3, T4, T5, TR>(source1, source2, source3, source4, source5, resultSelector);
		}

		public static UniRx.IObservable<TR> Zip<T1, T2, T3, T4, T5, T6, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, ZipFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
		{
			return new ZipObservable<T1, T2, T3, T4, T5, T6, TR>(source1, source2, source3, source4, source5, source6, resultSelector);
		}

		public static UniRx.IObservable<TR> Zip<T1, T2, T3, T4, T5, T6, T7, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, UniRx.IObservable<T7> source7, ZipFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
		{
			return new ZipObservable<T1, T2, T3, T4, T5, T6, T7, TR>(source1, source2, source3, source4, source5, source6, source7, resultSelector);
		}

		public static UniRx.IObservable<TResult> CombineLatest<TLeft, TRight, TResult>(this UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
		{
			return new CombineLatestObservable<TLeft, TRight, TResult>(left, right, selector);
		}

		public static UniRx.IObservable<IList<T>> CombineLatest<T>(this IEnumerable<UniRx.IObservable<T>> sources)
		{
			return CombineLatest(sources.ToArray());
		}

		public static UniRx.IObservable<IList<TSource>> CombineLatest<TSource>(params UniRx.IObservable<TSource>[] sources)
		{
			return new CombineLatestObservable<TSource>(sources);
		}

		public static UniRx.IObservable<TR> CombineLatest<T1, T2, T3, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, CombineLatestFunc<T1, T2, T3, TR> resultSelector)
		{
			return new CombineLatestObservable<T1, T2, T3, TR>(source1, source2, source3, resultSelector);
		}

		public static UniRx.IObservable<TR> CombineLatest<T1, T2, T3, T4, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, CombineLatestFunc<T1, T2, T3, T4, TR> resultSelector)
		{
			return new CombineLatestObservable<T1, T2, T3, T4, TR>(source1, source2, source3, source4, resultSelector);
		}

		public static UniRx.IObservable<TR> CombineLatest<T1, T2, T3, T4, T5, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, CombineLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector)
		{
			return new CombineLatestObservable<T1, T2, T3, T4, T5, TR>(source1, source2, source3, source4, source5, resultSelector);
		}

		public static UniRx.IObservable<TR> CombineLatest<T1, T2, T3, T4, T5, T6, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, CombineLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
		{
			return new CombineLatestObservable<T1, T2, T3, T4, T5, T6, TR>(source1, source2, source3, source4, source5, source6, resultSelector);
		}

		public static UniRx.IObservable<TR> CombineLatest<T1, T2, T3, T4, T5, T6, T7, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, UniRx.IObservable<T7> source7, CombineLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
		{
			return new CombineLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR>(source1, source2, source3, source4, source5, source6, source7, resultSelector);
		}

		public static UniRx.IObservable<TResult> ZipLatest<TLeft, TRight, TResult>(this UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
		{
			return new ZipLatestObservable<TLeft, TRight, TResult>(left, right, selector);
		}

		public static UniRx.IObservable<IList<T>> ZipLatest<T>(this IEnumerable<UniRx.IObservable<T>> sources)
		{
			return ZipLatest(sources.ToArray());
		}

		public static UniRx.IObservable<IList<TSource>> ZipLatest<TSource>(params UniRx.IObservable<TSource>[] sources)
		{
			return new ZipLatestObservable<TSource>(sources);
		}

		public static UniRx.IObservable<TR> ZipLatest<T1, T2, T3, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, ZipLatestFunc<T1, T2, T3, TR> resultSelector)
		{
			return new ZipLatestObservable<T1, T2, T3, TR>(source1, source2, source3, resultSelector);
		}

		public static UniRx.IObservable<TR> ZipLatest<T1, T2, T3, T4, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, ZipLatestFunc<T1, T2, T3, T4, TR> resultSelector)
		{
			return new ZipLatestObservable<T1, T2, T3, T4, TR>(source1, source2, source3, source4, resultSelector);
		}

		public static UniRx.IObservable<TR> ZipLatest<T1, T2, T3, T4, T5, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, ZipLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector)
		{
			return new ZipLatestObservable<T1, T2, T3, T4, T5, TR>(source1, source2, source3, source4, source5, resultSelector);
		}

		public static UniRx.IObservable<TR> ZipLatest<T1, T2, T3, T4, T5, T6, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, ZipLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
		{
			return new ZipLatestObservable<T1, T2, T3, T4, T5, T6, TR>(source1, source2, source3, source4, source5, source6, resultSelector);
		}

		public static UniRx.IObservable<TR> ZipLatest<T1, T2, T3, T4, T5, T6, T7, TR>(this UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, UniRx.IObservable<T7> source7, ZipLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
		{
			return new ZipLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR>(source1, source2, source3, source4, source5, source6, source7, resultSelector);
		}

		public static UniRx.IObservable<T> Switch<T>(this UniRx.IObservable<UniRx.IObservable<T>> sources)
		{
			return new SwitchObservable<T>(sources);
		}

		public static UniRx.IObservable<TResult> WithLatestFrom<TLeft, TRight, TResult>(this UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
		{
			return new WithLatestFromObservable<TLeft, TRight, TResult>(left, right, selector);
		}

		public static UniRx.IObservable<T[]> WhenAll<T>(params UniRx.IObservable<T>[] sources)
		{
			if (sources.Length == 0)
			{
				return Return(new T[0]);
			}
			return new WhenAllObservable<T>(sources);
		}

		public static UniRx.IObservable<Unit> WhenAll(params UniRx.IObservable<Unit>[] sources)
		{
			if (sources.Length == 0)
			{
				return ReturnUnit();
			}
			return new WhenAllObservable(sources);
		}

		public static UniRx.IObservable<T[]> WhenAll<T>(this IEnumerable<UniRx.IObservable<T>> sources)
		{
			UniRx.IObservable<T>[] array = sources as UniRx.IObservable<T>[];
			if (array != null)
			{
				return WhenAll(array);
			}
			return new WhenAllObservable<T>(sources);
		}

		public static UniRx.IObservable<Unit> WhenAll(this IEnumerable<UniRx.IObservable<Unit>> sources)
		{
			UniRx.IObservable<Unit>[] array = sources as UniRx.IObservable<Unit>[];
			if (array != null)
			{
				return WhenAll(array);
			}
			return new WhenAllObservable(sources);
		}

		public static UniRx.IObservable<T> StartWith<T>(this UniRx.IObservable<T> source, T value)
		{
			return new StartWithObservable<T>(source, value);
		}

		public static UniRx.IObservable<T> StartWith<T>(this UniRx.IObservable<T> source, Func<T> valueFactory)
		{
			return new StartWithObservable<T>(source, valueFactory);
		}

		public static UniRx.IObservable<T> StartWith<T>(this UniRx.IObservable<T> source, params T[] values)
		{
			return source.StartWith(Scheduler.DefaultSchedulers.ConstantTimeOperations, values);
		}

		public static UniRx.IObservable<T> StartWith<T>(this UniRx.IObservable<T> source, IEnumerable<T> values)
		{
			return source.StartWith(Scheduler.DefaultSchedulers.ConstantTimeOperations, values);
		}

		public static UniRx.IObservable<T> StartWith<T>(this UniRx.IObservable<T> source, IScheduler scheduler, T value)
		{
			return Return(value, scheduler).Concat(source);
		}

		public static UniRx.IObservable<T> StartWith<T>(this UniRx.IObservable<T> source, IScheduler scheduler, IEnumerable<T> values)
		{
			T[] array = values as T[];
			if (array == null)
			{
				array = values.ToArray();
			}
			return source.StartWith(scheduler, array);
		}

		public static UniRx.IObservable<T> StartWith<T>(this UniRx.IObservable<T> source, IScheduler scheduler, params T[] values)
		{
			return values.ToObservable(scheduler).Concat(source);
		}

		public static UniRx.IObservable<T> Synchronize<T>(this UniRx.IObservable<T> source)
		{
			return new SynchronizeObservable<T>(source, new object());
		}

		public static UniRx.IObservable<T> Synchronize<T>(this UniRx.IObservable<T> source, object gate)
		{
			return new SynchronizeObservable<T>(source, gate);
		}

		public static UniRx.IObservable<T> ObserveOn<T>(this UniRx.IObservable<T> source, IScheduler scheduler)
		{
			return new ObserveOnObservable<T>(source, scheduler);
		}

		public static UniRx.IObservable<T> SubscribeOn<T>(this UniRx.IObservable<T> source, IScheduler scheduler)
		{
			return new SubscribeOnObservable<T>(source, scheduler);
		}

		public static UniRx.IObservable<T> DelaySubscription<T>(this UniRx.IObservable<T> source, TimeSpan dueTime)
		{
			return new DelaySubscriptionObservable<T>(source, dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> DelaySubscription<T>(this UniRx.IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
		{
			return new DelaySubscriptionObservable<T>(source, dueTime, scheduler);
		}

		public static UniRx.IObservable<T> DelaySubscription<T>(this UniRx.IObservable<T> source, DateTimeOffset dueTime)
		{
			return new DelaySubscriptionObservable<T>(source, dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> DelaySubscription<T>(this UniRx.IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
		{
			return new DelaySubscriptionObservable<T>(source, dueTime, scheduler);
		}

		public static UniRx.IObservable<T> Amb<T>(params UniRx.IObservable<T>[] sources)
		{
			return Amb((IEnumerable<UniRx.IObservable<T>>)sources);
		}

		public static UniRx.IObservable<T> Amb<T>(IEnumerable<UniRx.IObservable<T>> sources)
		{
			UniRx.IObservable<T> observable = Never<T>();
			foreach (UniRx.IObservable<T> source in sources)
			{
				UniRx.IObservable<T> second = source;
				observable = observable.Amb(second);
			}
			return observable;
		}

		public static UniRx.IObservable<T> Amb<T>(this UniRx.IObservable<T> source, UniRx.IObservable<T> second)
		{
			return new AmbObservable<T>(source, second);
		}

		public static UniRx.IObservable<T> AsObservable<T>(this UniRx.IObservable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source is AsObservableObservable<T>)
			{
				return source;
			}
			return new AsObservableObservable<T>(source);
		}

		public static UniRx.IObservable<T> ToObservable<T>(this IEnumerable<T> source)
		{
			return source.ToObservable(Scheduler.DefaultSchedulers.Iteration);
		}

		public static UniRx.IObservable<T> ToObservable<T>(this IEnumerable<T> source, IScheduler scheduler)
		{
			return new ToObservableObservable<T>(source, scheduler);
		}

		public static UniRx.IObservable<TResult> Cast<TSource, TResult>(this UniRx.IObservable<TSource> source)
		{
			return new CastObservable<TSource, TResult>(source);
		}

		public static UniRx.IObservable<TResult> Cast<TSource, TResult>(this UniRx.IObservable<TSource> source, TResult witness)
		{
			return new CastObservable<TSource, TResult>(source);
		}

		public static UniRx.IObservable<TResult> OfType<TSource, TResult>(this UniRx.IObservable<TSource> source)
		{
			return new OfTypeObservable<TSource, TResult>(source);
		}

		public static UniRx.IObservable<TResult> OfType<TSource, TResult>(this UniRx.IObservable<TSource> source, TResult witness)
		{
			return new OfTypeObservable<TSource, TResult>(source);
		}

		public static UniRx.IObservable<Unit> AsUnitObservable<T>(this UniRx.IObservable<T> source)
		{
			return new AsUnitObservableObservable<T>(source);
		}

		public static UniRx.IObservable<Unit> AsSingleUnitObservable<T>(this UniRx.IObservable<T> source)
		{
			return new AsSingleUnitObservableObservable<T>(source);
		}

		public static UniRx.IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new CreateObservable<T>(subscribe);
		}

		public static UniRx.IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new CreateObservable<T>(subscribe, isRequiredSubscribeOnCurrentThread);
		}

		public static UniRx.IObservable<T> CreateWithState<T, TState>(TState state, Func<TState, IObserver<T>, IDisposable> subscribe)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new CreateObservable<T, TState>(state, subscribe);
		}

		public static UniRx.IObservable<T> CreateWithState<T, TState>(TState state, Func<TState, IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new CreateObservable<T, TState>(state, subscribe, isRequiredSubscribeOnCurrentThread);
		}

		public static UniRx.IObservable<T> CreateSafe<T>(Func<IObserver<T>, IDisposable> subscribe)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new CreateSafeObservable<T>(subscribe);
		}

		public static UniRx.IObservable<T> CreateSafe<T>(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
		{
			if (subscribe == null)
			{
				throw new ArgumentNullException("subscribe");
			}
			return new CreateSafeObservable<T>(subscribe, isRequiredSubscribeOnCurrentThread);
		}

		public static UniRx.IObservable<T> Empty<T>()
		{
			return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
		}

		public static UniRx.IObservable<T> Empty<T>(IScheduler scheduler)
		{
			if (scheduler == Scheduler.Immediate)
			{
				return ImmutableEmptyObservable<T>.Instance;
			}
			return new EmptyObservable<T>(scheduler);
		}

		public static UniRx.IObservable<T> Empty<T>(T witness)
		{
			return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
		}

		public static UniRx.IObservable<T> Empty<T>(IScheduler scheduler, T witness)
		{
			return Empty<T>(scheduler);
		}

		public static UniRx.IObservable<T> Never<T>()
		{
			return ImmutableNeverObservable<T>.Instance;
		}

		public static UniRx.IObservable<T> Never<T>(T witness)
		{
			return ImmutableNeverObservable<T>.Instance;
		}

		public static UniRx.IObservable<T> Return<T>(T value)
		{
			return Return(value, Scheduler.DefaultSchedulers.ConstantTimeOperations);
		}

		public static UniRx.IObservable<T> Return<T>(T value, IScheduler scheduler)
		{
			if (scheduler == Scheduler.Immediate)
			{
				return new ImmediateReturnObservable<T>(value);
			}
			return new ReturnObservable<T>(value, scheduler);
		}

		public static UniRx.IObservable<Unit> Return(Unit value)
		{
			return ImmutableReturnUnitObservable.Instance;
		}

		public static UniRx.IObservable<bool> Return(bool value)
		{
			object result;
			if (value)
			{
				UniRx.IObservable<bool> instance = ImmutableReturnTrueObservable.Instance;
				result = instance;
			}
			else
			{
				result = ImmutableReturnFalseObservable.Instance;
			}
			return (UniRx.IObservable<bool>)result;
		}

		public static UniRx.IObservable<Unit> ReturnUnit()
		{
			return ImmutableReturnUnitObservable.Instance;
		}

		public static UniRx.IObservable<T> Throw<T>(Exception error)
		{
			return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
		}

		public static UniRx.IObservable<T> Throw<T>(Exception error, T witness)
		{
			return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
		}

		public static UniRx.IObservable<T> Throw<T>(Exception error, IScheduler scheduler)
		{
			return new ThrowObservable<T>(error, scheduler);
		}

		public static UniRx.IObservable<T> Throw<T>(Exception error, IScheduler scheduler, T witness)
		{
			return Throw<T>(error, scheduler);
		}

		public static UniRx.IObservable<int> Range(int start, int count)
		{
			return Range(start, count, Scheduler.DefaultSchedulers.Iteration);
		}

		public static UniRx.IObservable<int> Range(int start, int count, IScheduler scheduler)
		{
			return new RangeObservable(start, count, scheduler);
		}

		public static UniRx.IObservable<T> Repeat<T>(T value)
		{
			return Repeat(value, Scheduler.DefaultSchedulers.Iteration);
		}

		public static UniRx.IObservable<T> Repeat<T>(T value, IScheduler scheduler)
		{
			if (scheduler == null)
			{
				throw new ArgumentNullException("scheduler");
			}
			return new RepeatObservable<T>(value, null, scheduler);
		}

		public static UniRx.IObservable<T> Repeat<T>(T value, int repeatCount)
		{
			return Repeat(value, repeatCount, Scheduler.DefaultSchedulers.Iteration);
		}

		public static UniRx.IObservable<T> Repeat<T>(T value, int repeatCount, IScheduler scheduler)
		{
			if (repeatCount < 0)
			{
				throw new ArgumentOutOfRangeException("repeatCount");
			}
			if (scheduler == null)
			{
				throw new ArgumentNullException("scheduler");
			}
			return new RepeatObservable<T>(value, repeatCount, scheduler);
		}

		public static UniRx.IObservable<T> Repeat<T>(this UniRx.IObservable<T> source)
		{
			return RepeatInfinite(source).Concat();
		}

		private static IEnumerable<UniRx.IObservable<T>> RepeatInfinite<T>(UniRx.IObservable<T> source)
		{
			while (true)
			{
				yield return source;
			}
		}

		public static UniRx.IObservable<T> RepeatSafe<T>(this UniRx.IObservable<T> source)
		{
			return new RepeatSafeObservable<T>(RepeatInfinite(source), source.IsRequiredSubscribeOnCurrentThread());
		}

		public static UniRx.IObservable<T> Defer<T>(Func<UniRx.IObservable<T>> observableFactory)
		{
			return new DeferObservable<T>(observableFactory);
		}

		public static UniRx.IObservable<T> Start<T>(Func<T> function)
		{
			return new StartObservable<T>(function, null, Scheduler.DefaultSchedulers.AsyncConversions);
		}

		public static UniRx.IObservable<T> Start<T>(Func<T> function, TimeSpan timeSpan)
		{
			return new StartObservable<T>(function, timeSpan, Scheduler.DefaultSchedulers.AsyncConversions);
		}

		public static UniRx.IObservable<T> Start<T>(Func<T> function, IScheduler scheduler)
		{
			return new StartObservable<T>(function, null, scheduler);
		}

		public static UniRx.IObservable<T> Start<T>(Func<T> function, TimeSpan timeSpan, IScheduler scheduler)
		{
			return new StartObservable<T>(function, timeSpan, scheduler);
		}

		public static UniRx.IObservable<Unit> Start(Action action)
		{
			return new StartObservable<Unit>(action, null, Scheduler.DefaultSchedulers.AsyncConversions);
		}

		public static UniRx.IObservable<Unit> Start(Action action, TimeSpan timeSpan)
		{
			return new StartObservable<Unit>(action, timeSpan, Scheduler.DefaultSchedulers.AsyncConversions);
		}

		public static UniRx.IObservable<Unit> Start(Action action, IScheduler scheduler)
		{
			return new StartObservable<Unit>(action, null, scheduler);
		}

		public static UniRx.IObservable<Unit> Start(Action action, TimeSpan timeSpan, IScheduler scheduler)
		{
			return new StartObservable<Unit>(action, timeSpan, scheduler);
		}

		public static Func<UniRx.IObservable<T>> ToAsync<T>(Func<T> function)
		{
			return ToAsync(function, Scheduler.DefaultSchedulers.AsyncConversions);
		}

		public static Func<UniRx.IObservable<T>> ToAsync<T>(Func<T> function, IScheduler scheduler)
		{
			return delegate
			{
				AsyncSubject<T> subject = (AsyncSubject<T>)new AsyncSubject<T>();
				scheduler.Schedule(delegate
				{
					T val = default(T);
					try
					{
						val = function();
					}
					catch (Exception error)
					{
						((AsyncSubject<T>)subject).OnError(error);
						return;
					}
					((AsyncSubject<T>)subject).OnNext(val);
					((AsyncSubject<T>)subject).OnCompleted();
				});
				return ((UniRx.IObservable<T>)subject).AsObservable();
			};
		}

		public static Func<UniRx.IObservable<Unit>> ToAsync(Action action)
		{
			return ToAsync(action, Scheduler.DefaultSchedulers.AsyncConversions);
		}

		public static Func<UniRx.IObservable<Unit>> ToAsync(Action action, IScheduler scheduler)
		{
			return delegate
			{
				AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
				scheduler.Schedule(delegate
				{
					try
					{
						action();
					}
					catch (Exception error)
					{
						subject.OnError(error);
						return;
					}
					subject.OnNext(Unit.Default);
					subject.OnCompleted();
				});
				return subject.AsObservable();
			};
		}

		public static UniRx.IObservable<TR> Select<T, TR>(this UniRx.IObservable<T> source, Func<T, TR> selector)
		{
			WhereObservable<T> whereObservable = source as WhereObservable<T>;
			if (whereObservable != null)
			{
				return whereObservable.CombineSelector(selector);
			}
			return new SelectObservable<T, TR>(source, selector);
		}

		public static UniRx.IObservable<TR> Select<T, TR>(this UniRx.IObservable<T> source, Func<T, int, TR> selector)
		{
			return new SelectObservable<T, TR>(source, selector);
		}

		public static UniRx.IObservable<T> Where<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			WhereObservable<T> whereObservable = source as WhereObservable<T>;
			if (whereObservable != null)
			{
				return whereObservable.CombinePredicate(predicate);
			}
			ISelect<T> select = source as ISelect<T>;
			if (select != null)
			{
				return select.CombinePredicate(predicate);
			}
			return new WhereObservable<T>(source, predicate);
		}

		public static UniRx.IObservable<T> Where<T>(this UniRx.IObservable<T> source, Func<T, int, bool> predicate)
		{
			return new WhereObservable<T>(source, predicate);
		}

		public static UniRx.IObservable<TR> ContinueWith<T, TR>(this UniRx.IObservable<T> source, UniRx.IObservable<TR> other)
		{
			return source.ContinueWith((T _) => other);
		}

		public static UniRx.IObservable<TR> ContinueWith<T, TR>(this UniRx.IObservable<T> source, Func<T, UniRx.IObservable<TR>> selector)
		{
			return new ContinueWithObservable<T, TR>(source, selector);
		}

		public static UniRx.IObservable<TR> SelectMany<T, TR>(this UniRx.IObservable<T> source, UniRx.IObservable<TR> other)
		{
			return source.SelectMany((T _) => other);
		}

		public static UniRx.IObservable<TR> SelectMany<T, TR>(this UniRx.IObservable<T> source, Func<T, UniRx.IObservable<TR>> selector)
		{
			return new SelectManyObservable<T, TR>(source, selector);
		}

		public static UniRx.IObservable<TResult> SelectMany<TSource, TResult>(this UniRx.IObservable<TSource> source, Func<TSource, int, UniRx.IObservable<TResult>> selector)
		{
			return new SelectManyObservable<TSource, TResult>(source, selector);
		}

		public static UniRx.IObservable<TR> SelectMany<T, TC, TR>(this UniRx.IObservable<T> source, Func<T, UniRx.IObservable<TC>> collectionSelector, Func<T, TC, TR> resultSelector)
		{
			return new SelectManyObservable<T, TC, TR>(source, collectionSelector, resultSelector);
		}

		public static UniRx.IObservable<TResult> SelectMany<TSource, TCollection, TResult>(this UniRx.IObservable<TSource> source, Func<TSource, int, UniRx.IObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
		{
			return new SelectManyObservable<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static UniRx.IObservable<TResult> SelectMany<TSource, TResult>(this UniRx.IObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			return new SelectManyObservable<TSource, TResult>(source, selector);
		}

		public static UniRx.IObservable<TResult> SelectMany<TSource, TResult>(this UniRx.IObservable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			return new SelectManyObservable<TSource, TResult>(source, selector);
		}

		public static UniRx.IObservable<TResult> SelectMany<TSource, TCollection, TResult>(this UniRx.IObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			return new SelectManyObservable<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static UniRx.IObservable<TResult> SelectMany<TSource, TCollection, TResult>(this UniRx.IObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
		{
			return new SelectManyObservable<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static UniRx.IObservable<T[]> ToArray<T>(this UniRx.IObservable<T> source)
		{
			return new ToArrayObservable<T>(source);
		}

		public static UniRx.IObservable<IList<T>> ToList<T>(this UniRx.IObservable<T> source)
		{
			return new ToListObservable<T>(source);
		}

		public static UniRx.IObservable<T> Do<T>(this UniRx.IObservable<T> source, IObserver<T> observer)
		{
			return new DoObserverObservable<T>(source, observer);
		}

		public static UniRx.IObservable<T> Do<T>(this UniRx.IObservable<T> source, Action<T> onNext)
		{
			return new DoObservable<T>(source, onNext, Stubs.Throw, Stubs.Nop);
		}

		public static UniRx.IObservable<T> Do<T>(this UniRx.IObservable<T> source, Action<T> onNext, Action<Exception> onError)
		{
			return new DoObservable<T>(source, onNext, onError, Stubs.Nop);
		}

		public static UniRx.IObservable<T> Do<T>(this UniRx.IObservable<T> source, Action<T> onNext, Action onCompleted)
		{
			return new DoObservable<T>(source, onNext, Stubs.Throw, onCompleted);
		}

		public static UniRx.IObservable<T> Do<T>(this UniRx.IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
		{
			return new DoObservable<T>(source, onNext, onError, onCompleted);
		}

		public static UniRx.IObservable<T> DoOnError<T>(this UniRx.IObservable<T> source, Action<Exception> onError)
		{
			return new DoOnErrorObservable<T>(source, onError);
		}

		public static UniRx.IObservable<T> DoOnCompleted<T>(this UniRx.IObservable<T> source, Action onCompleted)
		{
			return new DoOnCompletedObservable<T>(source, onCompleted);
		}

		public static UniRx.IObservable<T> DoOnTerminate<T>(this UniRx.IObservable<T> source, Action onTerminate)
		{
			return new DoOnTerminateObservable<T>(source, onTerminate);
		}

		public static UniRx.IObservable<T> DoOnSubscribe<T>(this UniRx.IObservable<T> source, Action onSubscribe)
		{
			return new DoOnSubscribeObservable<T>(source, onSubscribe);
		}

		public static UniRx.IObservable<T> DoOnCancel<T>(this UniRx.IObservable<T> source, Action onCancel)
		{
			return new DoOnCancelObservable<T>(source, onCancel);
		}

		public static UniRx.IObservable<Notification<T>> Materialize<T>(this UniRx.IObservable<T> source)
		{
			return new MaterializeObservable<T>(source);
		}

		public static UniRx.IObservable<T> Dematerialize<T>(this UniRx.IObservable<Notification<T>> source)
		{
			return new DematerializeObservable<T>(source);
		}

		public static UniRx.IObservable<T> DefaultIfEmpty<T>(this UniRx.IObservable<T> source)
		{
			return new DefaultIfEmptyObservable<T>(source, default(T));
		}

		public static UniRx.IObservable<T> DefaultIfEmpty<T>(this UniRx.IObservable<T> source, T defaultValue)
		{
			return new DefaultIfEmptyObservable<T>(source, defaultValue);
		}

		public static UniRx.IObservable<TSource> Distinct<TSource>(this UniRx.IObservable<TSource> source)
		{
			IEqualityComparer<TSource> @default = UnityEqualityComparer.GetDefault<TSource>();
			return new DistinctObservable<TSource>(source, @default);
		}

		public static UniRx.IObservable<TSource> Distinct<TSource>(this UniRx.IObservable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			return new DistinctObservable<TSource>(source, comparer);
		}

		public static UniRx.IObservable<TSource> Distinct<TSource, TKey>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector)
		{
			IEqualityComparer<TKey> @default = UnityEqualityComparer.GetDefault<TKey>();
			return new DistinctObservable<TSource, TKey>(source, keySelector, @default);
		}

		public static UniRx.IObservable<TSource> Distinct<TSource, TKey>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return new DistinctObservable<TSource, TKey>(source, keySelector, comparer);
		}

		public static UniRx.IObservable<T> DistinctUntilChanged<T>(this UniRx.IObservable<T> source)
		{
			IEqualityComparer<T> @default = UnityEqualityComparer.GetDefault<T>();
			return new DistinctUntilChangedObservable<T>(source, @default);
		}

		public static UniRx.IObservable<T> DistinctUntilChanged<T>(this UniRx.IObservable<T> source, IEqualityComparer<T> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DistinctUntilChangedObservable<T>(source, comparer);
		}

		public static UniRx.IObservable<T> DistinctUntilChanged<T, TKey>(this UniRx.IObservable<T> source, Func<T, TKey> keySelector)
		{
			IEqualityComparer<TKey> @default = UnityEqualityComparer.GetDefault<TKey>();
			return new DistinctUntilChangedObservable<T, TKey>(source, keySelector, @default);
		}

		public static UniRx.IObservable<T> DistinctUntilChanged<T, TKey>(this UniRx.IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DistinctUntilChangedObservable<T, TKey>(source, keySelector, comparer);
		}

		public static UniRx.IObservable<T> IgnoreElements<T>(this UniRx.IObservable<T> source)
		{
			return new IgnoreElementsObservable<T>(source);
		}

		public static UniRx.IObservable<Unit> ForEachAsync<T>(this UniRx.IObservable<T> source, Action<T> onNext)
		{
			return new ForEachAsyncObservable<T>(source, onNext);
		}

		public static UniRx.IObservable<Unit> ForEachAsync<T>(this UniRx.IObservable<T> source, Action<T, int> onNext)
		{
			return new ForEachAsyncObservable<T>(source, onNext);
		}

		public static UniRx.IObservable<T> Finally<T>(this UniRx.IObservable<T> source, Action finallyAction)
		{
			return new FinallyObservable<T>(source, finallyAction);
		}

		public static UniRx.IObservable<T> Catch<T, TException>(this UniRx.IObservable<T> source, Func<TException, UniRx.IObservable<T>> errorHandler) where TException : Exception
		{
			return new CatchObservable<T, TException>(source, errorHandler);
		}

		public static UniRx.IObservable<TSource> Catch<TSource>(this IEnumerable<UniRx.IObservable<TSource>> sources)
		{
			return new CatchObservable<TSource>(sources);
		}

		public static UniRx.IObservable<TSource> CatchIgnore<TSource>(this UniRx.IObservable<TSource> source)
		{
			return source.Catch<TSource, Exception>(Stubs.CatchIgnore<TSource>);
		}

		public static UniRx.IObservable<TSource> CatchIgnore<TSource, TException>(this UniRx.IObservable<TSource> source, Action<TException> errorAction) where TException : Exception
		{
			return source.Catch(delegate(TException ex)
			{
				errorAction(ex);
				return Empty<TSource>();
			});
		}

		public static UniRx.IObservable<TSource> Retry<TSource>(this UniRx.IObservable<TSource> source)
		{
			return RepeatInfinite(source).Catch();
		}

		public static UniRx.IObservable<TSource> Retry<TSource>(this UniRx.IObservable<TSource> source, int retryCount)
		{
			return Enumerable.Repeat(source, retryCount).Catch();
		}

		public static UniRx.IObservable<TSource> OnErrorRetry<TSource>(this UniRx.IObservable<TSource> source)
		{
			return source.Retry();
		}

		public static UniRx.IObservable<TSource> OnErrorRetry<TSource, TException>(this UniRx.IObservable<TSource> source, Action<TException> onError) where TException : Exception
		{
			return source.OnErrorRetry(onError, TimeSpan.Zero);
		}

		public static UniRx.IObservable<TSource> OnErrorRetry<TSource, TException>(this UniRx.IObservable<TSource> source, Action<TException> onError, TimeSpan delay) where TException : Exception
		{
			return source.OnErrorRetry(onError, int.MaxValue, delay);
		}

		public static UniRx.IObservable<TSource> OnErrorRetry<TSource, TException>(this UniRx.IObservable<TSource> source, Action<TException> onError, int retryCount) where TException : Exception
		{
			return source.OnErrorRetry(onError, retryCount, TimeSpan.Zero);
		}

		public static UniRx.IObservable<TSource> OnErrorRetry<TSource, TException>(this UniRx.IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay) where TException : Exception
		{
			return source.OnErrorRetry(onError, retryCount, delay, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<TSource> OnErrorRetry<TSource, TException>(this UniRx.IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay, IScheduler delayScheduler) where TException : Exception
		{
			return Defer(delegate
			{
				TimeSpan dueTime = (delay.Ticks >= 0) ? delay : TimeSpan.Zero;
				int count = 0;
				UniRx.IObservable<TSource> self = null;
				self = (UniRx.IObservable<TSource>)source.Catch(delegate(TException ex)
				{
					onError(ex);
					return (++count >= retryCount) ? Throw<TSource>(ex) : ((!(dueTime == TimeSpan.Zero)) ? ((UniRx.IObservable<TSource>)self).DelaySubscription(dueTime, delayScheduler).SubscribeOn(Scheduler.CurrentThread) : ((UniRx.IObservable<TSource>)self).SubscribeOn(Scheduler.CurrentThread));
				});
				return (UniRx.IObservable<TSource>)self;
			});
		}

		public static UniRx.IObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler) where TEventArgs : EventArgs
		{
			return new FromEventPatternObservable<TDelegate, TEventArgs>(conversion, addHandler, removeHandler);
		}

		public static UniRx.IObservable<Unit> FromEvent<TDelegate>(Func<Action, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
		{
			return new FromEventObservable<TDelegate>(conversion, addHandler, removeHandler);
		}

		public static UniRx.IObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
		{
			return new FromEventObservable<TDelegate, TEventArgs>(conversion, addHandler, removeHandler);
		}

		public static UniRx.IObservable<Unit> FromEvent(Action<Action> addHandler, Action<Action> removeHandler)
		{
			return new FromEventObservable(addHandler, removeHandler);
		}

		public static UniRx.IObservable<T> FromEvent<T>(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
		{
			return new FromEventObservable_<T>(addHandler, removeHandler);
		}

		public static Func<UniRx.IObservable<TResult>> FromAsyncPattern<TResult>(Func<AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
		{
			return delegate
			{
				AsyncSubject<TResult> subject = (AsyncSubject<TResult>)new AsyncSubject<TResult>();
				try
				{
					begin(delegate(IAsyncResult iar)
					{
						TResult value;
						try
						{
							value = end(iar);
						}
						catch (Exception error2)
						{
							((AsyncSubject<TResult>)subject).OnError(error2);
							return;
						}
						((AsyncSubject<TResult>)subject).OnNext(value);
						((AsyncSubject<TResult>)subject).OnCompleted();
					}, null);
				}
				catch (Exception error)
				{
					return Throw<TResult>(error, Scheduler.DefaultSchedulers.AsyncConversions);
				}
				return ((UniRx.IObservable<TResult>)subject).AsObservable();
			};
		}

		public static Func<T1, UniRx.IObservable<TResult>> FromAsyncPattern<T1, TResult>(Func<T1, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
		{
			return delegate(T1 x)
			{
				AsyncSubject<TResult> subject = (AsyncSubject<TResult>)new AsyncSubject<TResult>();
				try
				{
					begin(x, delegate(IAsyncResult iar)
					{
						TResult value;
						try
						{
							value = end(iar);
						}
						catch (Exception error2)
						{
							((AsyncSubject<TResult>)subject).OnError(error2);
							return;
						}
						((AsyncSubject<TResult>)subject).OnNext(value);
						((AsyncSubject<TResult>)subject).OnCompleted();
					}, null);
				}
				catch (Exception error)
				{
					return Throw<TResult>(error, Scheduler.DefaultSchedulers.AsyncConversions);
				}
				return ((UniRx.IObservable<TResult>)subject).AsObservable();
			};
		}

		public static Func<T1, T2, UniRx.IObservable<TResult>> FromAsyncPattern<T1, T2, TResult>(Func<T1, T2, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
		{
			return delegate(T1 x, T2 y)
			{
				AsyncSubject<TResult> subject = (AsyncSubject<TResult>)new AsyncSubject<TResult>();
				try
				{
					begin(x, y, delegate(IAsyncResult iar)
					{
						TResult value;
						try
						{
							value = end(iar);
						}
						catch (Exception error2)
						{
							((AsyncSubject<TResult>)subject).OnError(error2);
							return;
						}
						((AsyncSubject<TResult>)subject).OnNext(value);
						((AsyncSubject<TResult>)subject).OnCompleted();
					}, null);
				}
				catch (Exception error)
				{
					return Throw<TResult>(error, Scheduler.DefaultSchedulers.AsyncConversions);
				}
				return ((UniRx.IObservable<TResult>)subject).AsObservable();
			};
		}

		public static Func<UniRx.IObservable<Unit>> FromAsyncPattern(Func<AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
		{
			return FromAsyncPattern(begin, delegate(IAsyncResult iar)
			{
				end(iar);
				return Unit.Default;
			});
		}

		public static Func<T1, UniRx.IObservable<Unit>> FromAsyncPattern<T1>(Func<T1, AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
		{
			return FromAsyncPattern(begin, delegate(IAsyncResult iar)
			{
				end(iar);
				return Unit.Default;
			});
		}

		public static Func<T1, T2, UniRx.IObservable<Unit>> FromAsyncPattern<T1, T2>(Func<T1, T2, AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
		{
			return FromAsyncPattern(begin, delegate(IAsyncResult iar)
			{
				end(iar);
				return Unit.Default;
			});
		}

		public static UniRx.IObservable<T> Take<T>(this UniRx.IObservable<T> source, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return Empty<T>();
			}
			TakeObservable<T> takeObservable = source as TakeObservable<T>;
			if (takeObservable != null && takeObservable.scheduler == null)
			{
				return takeObservable.Combine(count);
			}
			return new TakeObservable<T>(source, count);
		}

		public static UniRx.IObservable<T> Take<T>(this UniRx.IObservable<T> source, TimeSpan duration)
		{
			return source.Take(duration, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> Take<T>(this UniRx.IObservable<T> source, TimeSpan duration, IScheduler scheduler)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (scheduler == null)
			{
				throw new ArgumentNullException("scheduler");
			}
			TakeObservable<T> takeObservable = source as TakeObservable<T>;
			if (takeObservable != null && takeObservable.scheduler == scheduler)
			{
				return takeObservable.Combine(duration);
			}
			return new TakeObservable<T>(source, duration, scheduler);
		}

		public static UniRx.IObservable<T> TakeWhile<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new TakeWhileObservable<T>(source, predicate);
		}

		public static UniRx.IObservable<T> TakeWhile<T>(this UniRx.IObservable<T> source, Func<T, int, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new TakeWhileObservable<T>(source, predicate);
		}

		public static UniRx.IObservable<T> TakeUntil<T, TOther>(this UniRx.IObservable<T> source, UniRx.IObservable<TOther> other)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			return new TakeUntilObservable<T, TOther>(source, other);
		}

		public static UniRx.IObservable<T> TakeLast<T>(this UniRx.IObservable<T> source, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return new TakeLastObservable<T>(source, count);
		}

		public static UniRx.IObservable<T> TakeLast<T>(this UniRx.IObservable<T> source, TimeSpan duration)
		{
			return source.TakeLast(duration, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> TakeLast<T>(this UniRx.IObservable<T> source, TimeSpan duration, IScheduler scheduler)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new TakeLastObservable<T>(source, duration, scheduler);
		}

		public static UniRx.IObservable<T> Skip<T>(this UniRx.IObservable<T> source, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			SkipObservable<T> skipObservable = source as SkipObservable<T>;
			if (skipObservable != null && skipObservable.scheduler == null)
			{
				return skipObservable.Combine(count);
			}
			return new SkipObservable<T>(source, count);
		}

		public static UniRx.IObservable<T> Skip<T>(this UniRx.IObservable<T> source, TimeSpan duration)
		{
			return source.Skip(duration, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> Skip<T>(this UniRx.IObservable<T> source, TimeSpan duration, IScheduler scheduler)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (scheduler == null)
			{
				throw new ArgumentNullException("scheduler");
			}
			SkipObservable<T> skipObservable = source as SkipObservable<T>;
			if (skipObservable != null && skipObservable.scheduler == scheduler)
			{
				return skipObservable.Combine(duration);
			}
			return new SkipObservable<T>(source, duration, scheduler);
		}

		public static UniRx.IObservable<T> SkipWhile<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new SkipWhileObservable<T>(source, predicate);
		}

		public static UniRx.IObservable<T> SkipWhile<T>(this UniRx.IObservable<T> source, Func<T, int, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new SkipWhileObservable<T>(source, predicate);
		}

		public static UniRx.IObservable<T> SkipUntil<T, TOther>(this UniRx.IObservable<T> source, UniRx.IObservable<TOther> other)
		{
			return new SkipUntilObservable<T, TOther>(source, other);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException("count <= 0");
			}
			return new BufferObservable<T>(source, count, 0);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, int count, int skip)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException("count <= 0");
			}
			if (skip <= 0)
			{
				throw new ArgumentOutOfRangeException("skip <= 0");
			}
			return new BufferObservable<T>(source, count, skip);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, TimeSpan timeSpan)
		{
			return source.Buffer(timeSpan, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, TimeSpan timeSpan, IScheduler scheduler)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new BufferObservable<T>(source, timeSpan, timeSpan, scheduler);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, TimeSpan timeSpan, int count)
		{
			return source.Buffer(timeSpan, count, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, TimeSpan timeSpan, int count, IScheduler scheduler)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException("count <= 0");
			}
			return new BufferObservable<T>(source, timeSpan, count, scheduler);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, TimeSpan timeSpan, TimeSpan timeShift)
		{
			return new BufferObservable<T>(source, timeSpan, timeShift, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<IList<T>> Buffer<T>(this UniRx.IObservable<T> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new BufferObservable<T>(source, timeSpan, timeShift, scheduler);
		}

		public static UniRx.IObservable<IList<TSource>> Buffer<TSource, TWindowBoundary>(this UniRx.IObservable<TSource> source, UniRx.IObservable<TWindowBoundary> windowBoundaries)
		{
			return new BufferObservable<TSource, TWindowBoundary>(source, windowBoundaries);
		}

		public static UniRx.IObservable<Pair<T>> Pairwise<T>(this UniRx.IObservable<T> source)
		{
			return new PairwiseObservable<T>(source);
		}

		public static UniRx.IObservable<TR> Pairwise<T, TR>(this UniRx.IObservable<T> source, Func<T, T, TR> selector)
		{
			return new PairwiseObservable<T, TR>(source, selector);
		}

		public static UniRx.IObservable<T> Last<T>(this UniRx.IObservable<T> source)
		{
			return new LastObservable<T>(source, useDefault: false);
		}

		public static UniRx.IObservable<T> Last<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new LastObservable<T>(source, predicate, useDefault: false);
		}

		public static UniRx.IObservable<T> LastOrDefault<T>(this UniRx.IObservable<T> source)
		{
			return new LastObservable<T>(source, useDefault: true);
		}

		public static UniRx.IObservable<T> LastOrDefault<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new LastObservable<T>(source, predicate, useDefault: true);
		}

		public static UniRx.IObservable<T> First<T>(this UniRx.IObservable<T> source)
		{
			return new FirstObservable<T>(source, useDefault: false);
		}

		public static UniRx.IObservable<T> First<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new FirstObservable<T>(source, predicate, useDefault: false);
		}

		public static UniRx.IObservable<T> FirstOrDefault<T>(this UniRx.IObservable<T> source)
		{
			return new FirstObservable<T>(source, useDefault: true);
		}

		public static UniRx.IObservable<T> FirstOrDefault<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new FirstObservable<T>(source, predicate, useDefault: true);
		}

		public static UniRx.IObservable<T> Single<T>(this UniRx.IObservable<T> source)
		{
			return new SingleObservable<T>(source, useDefault: false);
		}

		public static UniRx.IObservable<T> Single<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new SingleObservable<T>(source, predicate, useDefault: false);
		}

		public static UniRx.IObservable<T> SingleOrDefault<T>(this UniRx.IObservable<T> source)
		{
			return new SingleObservable<T>(source, useDefault: true);
		}

		public static UniRx.IObservable<T> SingleOrDefault<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
		{
			return new SingleObservable<T>(source, predicate, useDefault: true);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.GroupBy(keySelector, Stubs<TSource>.Identity);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return source.GroupBy(keySelector, Stubs<TSource>.Identity, comparer);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			IEqualityComparer<TKey> @default = UnityEqualityComparer.GetDefault<TKey>();
			return source.GroupBy(keySelector, elementSelector, @default);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			return new GroupByObservable<TSource, TKey, TElement>(source, keySelector, elementSelector, null, comparer);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, int capacity)
		{
			return source.GroupBy(keySelector, Stubs<TSource>.Identity, capacity);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, int capacity, IEqualityComparer<TKey> comparer)
		{
			return source.GroupBy(keySelector, Stubs<TSource>.Identity, capacity, comparer);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity)
		{
			IEqualityComparer<TKey> @default = UnityEqualityComparer.GetDefault<TKey>();
			return source.GroupBy(keySelector, elementSelector, capacity, @default);
		}

		public static UniRx.IObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity, IEqualityComparer<TKey> comparer)
		{
			return new GroupByObservable<TSource, TKey, TElement>(source, keySelector, elementSelector, capacity, comparer);
		}

		public static UniRx.IObservable<long> Interval(TimeSpan period)
		{
			return new TimerObservable(period, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<long> Interval(TimeSpan period, IScheduler scheduler)
		{
			return new TimerObservable(period, period, scheduler);
		}

		public static UniRx.IObservable<long> Timer(TimeSpan dueTime)
		{
			return new TimerObservable(dueTime, null, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<long> Timer(DateTimeOffset dueTime)
		{
			return new TimerObservable(dueTime, null, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
		{
			return new TimerObservable(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period)
		{
			return new TimerObservable(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<long> Timer(TimeSpan dueTime, IScheduler scheduler)
		{
			return new TimerObservable(dueTime, null, scheduler);
		}

		public static UniRx.IObservable<long> Timer(DateTimeOffset dueTime, IScheduler scheduler)
		{
			return new TimerObservable(dueTime, null, scheduler);
		}

		public static UniRx.IObservable<long> Timer(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
		{
			return new TimerObservable(dueTime, period, scheduler);
		}

		public static UniRx.IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
		{
			return new TimerObservable(dueTime, period, scheduler);
		}

		public static UniRx.IObservable<Timestamped<TSource>> Timestamp<TSource>(this UniRx.IObservable<TSource> source)
		{
			return source.Timestamp(Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<Timestamped<TSource>> Timestamp<TSource>(this UniRx.IObservable<TSource> source, IScheduler scheduler)
		{
			return new TimestampObservable<TSource>(source, scheduler);
		}

		public static UniRx.IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this UniRx.IObservable<TSource> source)
		{
			return source.TimeInterval(Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this UniRx.IObservable<TSource> source, IScheduler scheduler)
		{
			return new TimeIntervalObservable<TSource>(source, scheduler);
		}

		public static UniRx.IObservable<T> Delay<T>(this UniRx.IObservable<T> source, TimeSpan dueTime)
		{
			return source.Delay(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<TSource> Delay<TSource>(this UniRx.IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
		{
			return new DelayObservable<TSource>(source, dueTime, scheduler);
		}

		public static UniRx.IObservable<T> Sample<T>(this UniRx.IObservable<T> source, TimeSpan interval)
		{
			return source.Sample(interval, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> Sample<T>(this UniRx.IObservable<T> source, TimeSpan interval, IScheduler scheduler)
		{
			return new SampleObservable<T>(source, interval, scheduler);
		}

		public static UniRx.IObservable<TSource> Throttle<TSource>(this UniRx.IObservable<TSource> source, TimeSpan dueTime)
		{
			return source.Throttle(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<TSource> Throttle<TSource>(this UniRx.IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
		{
			return new ThrottleObservable<TSource>(source, dueTime, scheduler);
		}

		public static UniRx.IObservable<TSource> ThrottleFirst<TSource>(this UniRx.IObservable<TSource> source, TimeSpan dueTime)
		{
			return source.ThrottleFirst(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<TSource> ThrottleFirst<TSource>(this UniRx.IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
		{
			return new ThrottleFirstObservable<TSource>(source, dueTime, scheduler);
		}

		public static UniRx.IObservable<T> Timeout<T>(this UniRx.IObservable<T> source, TimeSpan dueTime)
		{
			return source.Timeout(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> Timeout<T>(this UniRx.IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
		{
			return new TimeoutObservable<T>(source, dueTime, scheduler);
		}

		public static UniRx.IObservable<T> Timeout<T>(this UniRx.IObservable<T> source, DateTimeOffset dueTime)
		{
			return source.Timeout(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
		}

		public static UniRx.IObservable<T> Timeout<T>(this UniRx.IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
		{
			return new TimeoutObservable<T>(source, dueTime, scheduler);
		}

		public static UniRx.IObservable<Unit> FromCoroutine(Func<IEnumerator> coroutine, bool publishEveryYield = false)
		{
			return FromCoroutine((UniRx.IObserver<Unit> observer, CancellationToken cancellationToken) => WrapEnumerator(coroutine(), observer, cancellationToken, publishEveryYield));
		}

		public static UniRx.IObservable<Unit> FromCoroutine(Func<CancellationToken, IEnumerator> coroutine, bool publishEveryYield = false)
		{
			return FromCoroutine((UniRx.IObserver<Unit> observer, CancellationToken cancellationToken) => WrapEnumerator(coroutine(cancellationToken), observer, cancellationToken, publishEveryYield));
		}

		public static UniRx.IObservable<Unit> FromMicroCoroutine(Func<IEnumerator> coroutine, bool publishEveryYield = false, FrameCountType frameCountType = FrameCountType.Update)
		{
			return FromMicroCoroutine((UniRx.IObserver<Unit> observer, CancellationToken cancellationToken) => WrapEnumerator(coroutine(), observer, cancellationToken, publishEveryYield), frameCountType);
		}

		public static UniRx.IObservable<Unit> FromMicroCoroutine(Func<CancellationToken, IEnumerator> coroutine, bool publishEveryYield = false, FrameCountType frameCountType = FrameCountType.Update)
		{
			return FromMicroCoroutine((UniRx.IObserver<Unit> observer, CancellationToken cancellationToken) => WrapEnumerator(coroutine(cancellationToken), observer, cancellationToken, publishEveryYield), frameCountType);
		}

		private static IEnumerator WrapEnumerator(IEnumerator enumerator, IObserver<Unit> observer, CancellationToken cancellationToken, bool publishEveryYield)
		{
			bool raisedError = false;
			bool hasNext;
			do
			{
				try
				{
					hasNext = enumerator.MoveNext();
				}
				catch (Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						(enumerator as IDisposable)?.Dispose();
					}
					yield break;
				}
				if (hasNext && publishEveryYield)
				{
					try
					{
						observer.OnNext(Unit.Default);
					}
					catch
					{
						(enumerator as IDisposable)?.Dispose();
						throw;
					}
				}
				if (!hasNext)
				{
					continue;
				}
				object current = enumerator.Current;
				ICustomYieldInstructionErrorHandler customHandler = current as ICustomYieldInstructionErrorHandler;
				if (customHandler != null && customHandler.IsReThrowOnError)
				{
					customHandler.ForceDisableRethrowOnError();
					yield return current;
					customHandler.ForceEnableRethrowOnError();
					if (customHandler.HasError)
					{
						try
						{
							observer.OnError(customHandler.Error);
						}
						finally
						{
							(enumerator as IDisposable)?.Dispose();
						}
						yield break;
					}
				}
				else
				{
					yield return enumerator.Current;
				}
			}
			while (hasNext && !cancellationToken.IsCancellationRequested);
			try
			{
				if (!raisedError && !cancellationToken.IsCancellationRequested)
				{
					observer.OnNext(Unit.Default);
					observer.OnCompleted();
				}
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
		}

		public static UniRx.IObservable<T> FromCoroutineValue<T>(Func<IEnumerator> coroutine, bool nullAsNextUpdate = true)
		{
			return FromCoroutine((UniRx.IObserver<T> observer, CancellationToken cancellationToken) => WrapEnumeratorYieldValue(coroutine(), observer, cancellationToken, nullAsNextUpdate));
		}

		public static UniRx.IObservable<T> FromCoroutineValue<T>(Func<CancellationToken, IEnumerator> coroutine, bool nullAsNextUpdate = true)
		{
			return FromCoroutine((UniRx.IObserver<T> observer, CancellationToken cancellationToken) => WrapEnumeratorYieldValue(coroutine(cancellationToken), observer, cancellationToken, nullAsNextUpdate));
		}

		private static IEnumerator WrapEnumeratorYieldValue<T>(IEnumerator enumerator, IObserver<T> observer, CancellationToken cancellationToken, bool nullAsNextUpdate)
		{
			object current = null;
			bool raisedError = false;
			bool hasNext;
			do
			{
				try
				{
					hasNext = enumerator.MoveNext();
					if (hasNext)
					{
						current = enumerator.Current;
					}
				}
				catch (Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						(enumerator as IDisposable)?.Dispose();
					}
					yield break;
				}
				if (!hasNext)
				{
					continue;
				}
				if (current != null && YieldInstructionTypes.Contains(current.GetType()))
				{
					yield return current;
				}
				else if (current is IEnumerator)
				{
					ICustomYieldInstructionErrorHandler customHandler = current as ICustomYieldInstructionErrorHandler;
					if (customHandler != null && customHandler.IsReThrowOnError)
					{
						customHandler.ForceDisableRethrowOnError();
						yield return current;
						customHandler.ForceEnableRethrowOnError();
						if (customHandler.HasError)
						{
							try
							{
								observer.OnError(customHandler.Error);
							}
							finally
							{
								(enumerator as IDisposable)?.Dispose();
							}
							yield break;
						}
					}
					else
					{
						yield return current;
					}
				}
				else if (current == null && nullAsNextUpdate)
				{
					yield return null;
				}
				else
				{
					try
					{
						observer.OnNext((T)current);
					}
					catch
					{
						(enumerator as IDisposable)?.Dispose();
						throw;
					}
				}
			}
			while (hasNext && !cancellationToken.IsCancellationRequested);
			try
			{
				if (!raisedError && !cancellationToken.IsCancellationRequested)
				{
					observer.OnCompleted();
				}
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
		}

		public static UniRx.IObservable<T> FromCoroutine<T>(Func<IObserver<T>, IEnumerator> coroutine)
		{
			return FromCoroutine((UniRx.IObserver<T> observer, CancellationToken cancellationToken) => WrapToCancellableEnumerator(coroutine(observer), cancellationToken));
		}

		public static UniRx.IObservable<T> FromMicroCoroutine<T>(Func<IObserver<T>, IEnumerator> coroutine, FrameCountType frameCountType = FrameCountType.Update)
		{
			return FromMicroCoroutine((UniRx.IObserver<T> observer, CancellationToken cancellationToken) => WrapToCancellableEnumerator(coroutine(observer), cancellationToken), frameCountType);
		}

		private static IEnumerator WrapToCancellableEnumerator(IEnumerator enumerator, CancellationToken cancellationToken)
		{
			bool hasNext;
			do
			{
				try
				{
					hasNext = enumerator.MoveNext();
				}
				catch
				{
					(enumerator as IDisposable)?.Dispose();
					yield break;
				}
				yield return enumerator.Current;
			}
			while (hasNext && !cancellationToken.IsCancellationRequested);
			(enumerator as IDisposable)?.Dispose();
		}

		public static UniRx.IObservable<T> FromCoroutine<T>(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
		{
			return new FromCoroutineObservable<T>(coroutine);
		}

		public static UniRx.IObservable<T> FromMicroCoroutine<T>(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine, FrameCountType frameCountType = FrameCountType.Update)
		{
			return new FromMicroCoroutineObservable<T>(coroutine, frameCountType);
		}

		public static UniRx.IObservable<Unit> SelectMany<T>(this UniRx.IObservable<T> source, IEnumerator coroutine, bool publishEveryYield = false)
		{
			return source.SelectMany(FromCoroutine(() => coroutine, publishEveryYield));
		}

		public static UniRx.IObservable<Unit> SelectMany<T>(this UniRx.IObservable<T> source, Func<IEnumerator> selector, bool publishEveryYield = false)
		{
			return source.SelectMany(FromCoroutine(() => selector(), publishEveryYield));
		}

		public static UniRx.IObservable<Unit> SelectMany<T>(this UniRx.IObservable<T> source, Func<T, IEnumerator> selector)
		{
			return source.SelectMany((T x) => FromCoroutine(() => selector(x)));
		}

		public static UniRx.IObservable<Unit> ToObservable(this IEnumerator coroutine, bool publishEveryYield = false)
		{
			return FromCoroutine((UniRx.IObserver<Unit> observer, CancellationToken cancellationToken) => WrapEnumerator(coroutine, observer, cancellationToken, publishEveryYield));
		}

		public static ObservableYieldInstruction<Unit> ToYieldInstruction(this IEnumerator coroutine)
		{
			return coroutine.ToObservable().ToYieldInstruction();
		}

		public static ObservableYieldInstruction<Unit> ToYieldInstruction(this IEnumerator coroutine, bool throwOnError)
		{
			return coroutine.ToObservable().ToYieldInstruction(throwOnError);
		}

		public static ObservableYieldInstruction<Unit> ToYieldInstruction(this IEnumerator coroutine, CancellationToken cancellationToken)
		{
			return coroutine.ToObservable().ToYieldInstruction(cancellationToken);
		}

		public static ObservableYieldInstruction<Unit> ToYieldInstruction(this IEnumerator coroutine, bool throwOnError, CancellationToken cancellationToken)
		{
			return coroutine.ToObservable().ToYieldInstruction(throwOnError, cancellationToken);
		}

		public static UniRx.IObservable<long> EveryUpdate()
		{
			return FromMicroCoroutine<long>(EveryCycleCore);
		}

		public static UniRx.IObservable<long> EveryFixedUpdate()
		{
			return FromMicroCoroutine<long>(EveryCycleCore, FrameCountType.FixedUpdate);
		}

		public static UniRx.IObservable<long> EveryEndOfFrame()
		{
			return FromMicroCoroutine<long>(EveryCycleCore, FrameCountType.EndOfFrame);
		}

		private static IEnumerator EveryCycleCore(UniRx.IObserver<long> observer, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				yield break;
			}
			long count = 0L;
			while (true)
			{
				yield return null;
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				long value;
				count = (value = count) + 1;
				observer.OnNext(value);
			}
		}

		public static UniRx.IObservable<long> EveryGameObjectUpdate()
		{
			return MainThreadDispatcher.UpdateAsObservable().Scan(-1L, (long x, Unit y) => x + 1);
		}

		public static UniRx.IObservable<long> EveryLateUpdate()
		{
			return MainThreadDispatcher.LateUpdateAsObservable().Scan(-1L, (long x, Unit y) => x + 1);
		}

		[Obsolete]
		public static UniRx.IObservable<long> EveryAfterUpdate()
		{
			return FromCoroutine((UniRx.IObserver<long> observer, CancellationToken cancellationToken) => new EveryAfterUpdateInvoker(observer, cancellationToken));
		}

		public static UniRx.IObservable<Unit> NextFrame(FrameCountType frameCountType = FrameCountType.Update)
		{
			return FromMicroCoroutine<Unit>(NextFrameCore, frameCountType);
		}

		private static IEnumerator NextFrameCore(UniRx.IObserver<Unit> observer, CancellationToken cancellation)
		{
			yield return null;
			if (!cancellation.IsCancellationRequested)
			{
				observer.OnNext(Unit.Default);
				observer.OnCompleted();
			}
		}

		public static UniRx.IObservable<long> IntervalFrame(int intervalFrameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			return TimerFrame(intervalFrameCount, intervalFrameCount, frameCountType);
		}

		public static UniRx.IObservable<long> TimerFrame(int dueTimeFrameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			return FromMicroCoroutine((UniRx.IObserver<long> observer, CancellationToken cancellation) => TimerFrameCore(observer, dueTimeFrameCount, cancellation), frameCountType);
		}

		public static UniRx.IObservable<long> TimerFrame(int dueTimeFrameCount, int periodFrameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			return FromMicroCoroutine((UniRx.IObserver<long> observer, CancellationToken cancellation) => TimerFrameCore(observer, dueTimeFrameCount, periodFrameCount, cancellation), frameCountType);
		}

		private static IEnumerator TimerFrameCore(UniRx.IObserver<long> observer, int dueTimeFrameCount, CancellationToken cancel)
		{
			if (dueTimeFrameCount <= 0)
			{
				dueTimeFrameCount = 0;
			}
			int currentFrame = 0;
			while (true)
			{
				if (!cancel.IsCancellationRequested)
				{
					int num;
					currentFrame = (num = currentFrame) + 1;
					if (num == dueTimeFrameCount)
					{
						break;
					}
					yield return null;
					continue;
				}
				yield break;
			}
			observer.OnNext(0L);
			observer.OnCompleted();
		}

		private static IEnumerator TimerFrameCore(UniRx.IObserver<long> observer, int dueTimeFrameCount, int periodFrameCount, CancellationToken cancel)
		{
			if (dueTimeFrameCount <= 0)
			{
				dueTimeFrameCount = 0;
			}
			if (periodFrameCount <= 0)
			{
				periodFrameCount = 1;
			}
			long sendCount = 0L;
			int currentFrame = 0;
			while (!cancel.IsCancellationRequested)
			{
				int num;
				currentFrame = (num = currentFrame) + 1;
				if (num == dueTimeFrameCount)
				{
					long value;
					sendCount = (value = sendCount) + 1;
					observer.OnNext(value);
					currentFrame = -1;
					break;
				}
				yield return null;
			}
			while (!cancel.IsCancellationRequested)
			{
				int num;
				currentFrame = (num = currentFrame + 1);
				if (num == periodFrameCount)
				{
					long value;
					sendCount = (value = sendCount) + 1;
					observer.OnNext(value);
					currentFrame = 0;
				}
				yield return null;
			}
		}

		public static UniRx.IObservable<T> DelayFrame<T>(this UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			if (frameCount < 0)
			{
				throw new ArgumentOutOfRangeException("frameCount");
			}
			return new DelayFrameObservable<T>(source, frameCount, frameCountType);
		}

		public static UniRx.IObservable<T> Sample<T, T2>(this UniRx.IObservable<T> source, UniRx.IObservable<T2> sampler)
		{
			return new SampleObservable<T, T2>(source, sampler);
		}

		public static UniRx.IObservable<T> SampleFrame<T>(this UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			if (frameCount < 0)
			{
				throw new ArgumentOutOfRangeException("frameCount");
			}
			return new SampleFrameObservable<T>(source, frameCount, frameCountType);
		}

		public static UniRx.IObservable<TSource> ThrottleFrame<TSource>(this UniRx.IObservable<TSource> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			if (frameCount < 0)
			{
				throw new ArgumentOutOfRangeException("frameCount");
			}
			return new ThrottleFrameObservable<TSource>(source, frameCount, frameCountType);
		}

		public static UniRx.IObservable<TSource> ThrottleFirstFrame<TSource>(this UniRx.IObservable<TSource> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			if (frameCount < 0)
			{
				throw new ArgumentOutOfRangeException("frameCount");
			}
			return new ThrottleFirstFrameObservable<TSource>(source, frameCount, frameCountType);
		}

		public static UniRx.IObservable<T> TimeoutFrame<T>(this UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			if (frameCount < 0)
			{
				throw new ArgumentOutOfRangeException("frameCount");
			}
			return new TimeoutFrameObservable<T>(source, frameCount, frameCountType);
		}

		public static UniRx.IObservable<T> DelayFrameSubscription<T>(this UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
		{
			if (frameCount < 0)
			{
				throw new ArgumentOutOfRangeException("frameCount");
			}
			return new DelayFrameSubscriptionObservable<T>(source, frameCount, frameCountType);
		}

		public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this UniRx.IObservable<T> source)
		{
			return new ObservableYieldInstruction<T>(source, reThrowOnError: true, CancellationToken.None);
		}

		public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this UniRx.IObservable<T> source, CancellationToken cancel)
		{
			return new ObservableYieldInstruction<T>(source, reThrowOnError: true, cancel);
		}

		public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this UniRx.IObservable<T> source, bool throwOnError)
		{
			return new ObservableYieldInstruction<T>(source, throwOnError, CancellationToken.None);
		}

		public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this UniRx.IObservable<T> source, bool throwOnError, CancellationToken cancel)
		{
			return new ObservableYieldInstruction<T>(source, throwOnError, cancel);
		}

		public static IEnumerator ToAwaitableEnumerator<T>(this UniRx.IObservable<T> source, CancellationToken cancel = default(CancellationToken))
		{
			return source.ToAwaitableEnumerator(Stubs<T>.Ignore, Stubs.Throw, cancel);
		}

		public static IEnumerator ToAwaitableEnumerator<T>(this UniRx.IObservable<T> source, Action<T> onResult, CancellationToken cancel = default(CancellationToken))
		{
			return source.ToAwaitableEnumerator(onResult, Stubs.Throw, cancel);
		}

		public static IEnumerator ToAwaitableEnumerator<T>(this UniRx.IObservable<T> source, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
		{
			return source.ToAwaitableEnumerator(Stubs<T>.Ignore, onError, cancel);
		}

		public static IEnumerator ToAwaitableEnumerator<T>(this UniRx.IObservable<T> source, Action<T> onResult, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
		{
			ObservableYieldInstruction<T> enumerator = new ObservableYieldInstruction<T>(source, reThrowOnError: false, cancel);
			IEnumerator<T> e = enumerator;
			while (e.MoveNext() && !cancel.IsCancellationRequested)
			{
				yield return null;
			}
			if (cancel.IsCancellationRequested)
			{
				enumerator.Dispose();
			}
			else if (enumerator.HasResult)
			{
				onResult(enumerator.Result);
			}
			else if (enumerator.HasError)
			{
				onError(enumerator.Error);
			}
		}

		public static Coroutine StartAsCoroutine<T>(this UniRx.IObservable<T> source, CancellationToken cancel = default(CancellationToken))
		{
			return source.StartAsCoroutine(Stubs<T>.Ignore, Stubs.Throw, cancel);
		}

		public static Coroutine StartAsCoroutine<T>(this UniRx.IObservable<T> source, Action<T> onResult, CancellationToken cancel = default(CancellationToken))
		{
			return source.StartAsCoroutine(onResult, Stubs.Throw, cancel);
		}

		public static Coroutine StartAsCoroutine<T>(this UniRx.IObservable<T> source, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
		{
			return source.StartAsCoroutine(Stubs<T>.Ignore, onError, cancel);
		}

		public static Coroutine StartAsCoroutine<T>(this UniRx.IObservable<T> source, Action<T> onResult, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
		{
			return MainThreadDispatcher.StartCoroutine(source.ToAwaitableEnumerator(onResult, onError, cancel));
		}

		public static UniRx.IObservable<T> ObserveOnMainThread<T>(this UniRx.IObservable<T> source)
		{
			return source.ObserveOn(Scheduler.MainThread);
		}

		public static UniRx.IObservable<T> ObserveOnMainThread<T>(this UniRx.IObservable<T> source, MainThreadDispatchType dispatchType)
		{
			switch (dispatchType)
			{
			case MainThreadDispatchType.Update:
				return source.ObserveOnMainThread();
			case MainThreadDispatchType.FixedUpdate:
				return source.SelectMany((T _) => EveryFixedUpdate().Take(1), (T x, long _) => x);
			case MainThreadDispatchType.EndOfFrame:
				return source.SelectMany((T _) => EveryEndOfFrame().Take(1), (T x, long _) => x);
			case MainThreadDispatchType.GameObjectUpdate:
				return source.SelectMany((T _) => MainThreadDispatcher.UpdateAsObservable().Take(1), (T x, Unit _) => x);
			case MainThreadDispatchType.LateUpdate:
				return source.SelectMany((T _) => MainThreadDispatcher.LateUpdateAsObservable().Take(1), (T x, Unit _) => x);
			case MainThreadDispatchType.AfterUpdate:
				return source.SelectMany((T _) => EveryAfterUpdate().Take(1), (T x, long _) => x);
			default:
				throw new ArgumentException("type is invalid");
			}
		}

		public static UniRx.IObservable<T> SubscribeOnMainThread<T>(this UniRx.IObservable<T> source)
		{
			return source.SubscribeOn(Scheduler.MainThread);
		}

		public static UniRx.IObservable<bool> EveryApplicationPause()
		{
			return MainThreadDispatcher.OnApplicationPauseAsObservable().AsObservable();
		}

		public static UniRx.IObservable<bool> EveryApplicationFocus()
		{
			return MainThreadDispatcher.OnApplicationFocusAsObservable().AsObservable();
		}

		public static UniRx.IObservable<Unit> OnceApplicationQuit()
		{
			return MainThreadDispatcher.OnApplicationQuitAsObservable().Take(1);
		}

		public static UniRx.IObservable<T> TakeUntilDestroy<T>(this UniRx.IObservable<T> source, Component target)
		{
			return source.TakeUntil(target.OnDestroyAsObservable());
		}

		public static UniRx.IObservable<T> TakeUntilDestroy<T>(this UniRx.IObservable<T> source, GameObject target)
		{
			return source.TakeUntil(target.OnDestroyAsObservable());
		}

		public static UniRx.IObservable<T> TakeUntilDisable<T>(this UniRx.IObservable<T> source, Component target)
		{
			return source.TakeUntil(target.OnDisableAsObservable());
		}

		public static UniRx.IObservable<T> TakeUntilDisable<T>(this UniRx.IObservable<T> source, GameObject target)
		{
			return source.TakeUntil(target.OnDisableAsObservable());
		}

		public static UniRx.IObservable<T> RepeatUntilDestroy<T>(this UniRx.IObservable<T> source, GameObject target)
		{
			return RepeatInfinite(source).RepeatUntilCore(target.OnDestroyAsObservable(), target);
		}

		public static UniRx.IObservable<T> RepeatUntilDestroy<T>(this UniRx.IObservable<T> source, Component target)
		{
			return RepeatInfinite(source).RepeatUntilCore(target.OnDestroyAsObservable(), (!(target != null)) ? null : target.gameObject);
		}

		public static UniRx.IObservable<T> RepeatUntilDisable<T>(this UniRx.IObservable<T> source, GameObject target)
		{
			return RepeatInfinite(source).RepeatUntilCore(target.OnDisableAsObservable(), target);
		}

		public static UniRx.IObservable<T> RepeatUntilDisable<T>(this UniRx.IObservable<T> source, Component target)
		{
			return RepeatInfinite(source).RepeatUntilCore(target.OnDisableAsObservable(), (!(target != null)) ? null : target.gameObject);
		}

		private static UniRx.IObservable<T> RepeatUntilCore<T>(this IEnumerable<UniRx.IObservable<T>> sources, UniRx.IObservable<Unit> trigger, GameObject lifeTimeChecker)
		{
			return new RepeatUntilObservable<T>(sources, trigger, lifeTimeChecker);
		}

		public static UniRx.IObservable<FrameInterval<T>> FrameInterval<T>(this UniRx.IObservable<T> source)
		{
			return new FrameIntervalObservable<T>(source);
		}

		public static UniRx.IObservable<TimeInterval<T>> FrameTimeInterval<T>(this UniRx.IObservable<T> source, bool ignoreTimeScale = false)
		{
			return new FrameTimeIntervalObservable<T>(source, ignoreTimeScale);
		}

		public static UniRx.IObservable<IList<T>> BatchFrame<T>(this UniRx.IObservable<T> source)
		{
			return source.BatchFrame(0, FrameCountType.EndOfFrame);
		}

		public static UniRx.IObservable<IList<T>> BatchFrame<T>(this UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType)
		{
			if (frameCount < 0)
			{
				throw new ArgumentException("frameCount must be >= 0, frameCount:" + frameCount);
			}
			return new BatchFrameObservable<T>(source, frameCount, frameCountType);
		}

		public static UniRx.IObservable<Unit> BatchFrame(this UniRx.IObservable<Unit> source)
		{
			return source.BatchFrame(0, FrameCountType.EndOfFrame);
		}

		public static UniRx.IObservable<Unit> BatchFrame(this UniRx.IObservable<Unit> source, int frameCount, FrameCountType frameCountType)
		{
			if (frameCount < 0)
			{
				throw new ArgumentException("frameCount must be >= 0, frameCount:" + frameCount);
			}
			return new BatchFrameObservable(source, frameCount, frameCountType);
		}
	}
}
