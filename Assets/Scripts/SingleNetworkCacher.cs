using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public abstract class SingleNetworkCacher : IDisposable, Initable
{
	public int waitTimeForTheNextProvider = 4;

	public int preloadNotAvailableWaitingPeriod = 60;

	protected CompositeDisposable _disposable = new CompositeDisposable();

	protected List<FlooredAdProvider> _flooredProviders => PersistentSingleton<FlooredProvidersFactory>.Instance.FlooredAdProviders;

	public void Dispose()
	{
		_disposable.Dispose();
	}

	protected void bindPreloadingOfZonedSource(AdNetwork network)
	{
		List<FlooredAdProvider> placements = (from p in _flooredProviders
			where p.NetworkId == network
			select p).ToList();
		bindPreloading(placements);
	}

	protected void bindSinglePreloader(AdProvider provider)
	{
		(from r in provider.AdsReady
			where !r
			select r into _
			select Observable.Return(0L).Concat(Observable.Interval(TimeSpan.FromSeconds(preloadNotAvailableWaitingPeriod))).TakeUntil(from r in provider.AdsReady
				where r
				select r)).Subscribe(delegate
		{
			preload(provider);
		}).AddTo(_disposable);
	}

	protected void bindPreloading(List<FlooredAdProvider> placements)
	{
		if (placements.Count != 0)
		{
			ReactiveProperty<bool> highestFloorHasAd = placements.First().AdsReady;
			ReadOnlyReactiveProperty<long> source = (from r in highestFloorHasAd
				where !r
				select r into _
				select ServerTimeService.NowTicks()).ToReadOnlyReactiveProperty();
			UniRx.IObservable<long> source2 = (from _ in source
				select Observable.Return(0L).Concat(Observable.Interval(TimeSpan.FromSeconds(preloadNotAvailableWaitingPeriod))).TakeUntil(from r in highestFloorHasAd
					where r
					select r)).Switch();
			FlooredAdProvider backFiller = placements.Last();
			(from _ in source2
				where !backFiller.AdsReady.Value
				select _).Subscribe(delegate
			{
				preload(backFiller);
			}).AddTo(_disposable);
			FlooredAdProvider[] flooredPlacements = (from p in placements
				where p != backFiller
				select p).ToArray();
			if (flooredPlacements.Length > 0)
			{
				(from _ in source2
					select (from p in flooredPlacements
						select UniRx.Tuple.Create(p, flooredPlacements.TakeWhile((FlooredAdProvider pr) => pr != p))).ToObservable().SelectMany((UniRx.Tuple<FlooredAdProvider, IEnumerable<FlooredAdProvider>> pair) => Observable.Return(pair).Delay(TimeSpan.FromSeconds(waitTimeForTheNextProvider * pair.Item2.Count())).SelectMany((UniRx.Tuple<FlooredAdProvider, IEnumerable<FlooredAdProvider>> p) => Observable.Return(p).TakeUntil(previousOrMeGotReady(pair.Item1, pair.Item2))))).Switch().Subscribe(delegate(UniRx.Tuple<FlooredAdProvider, IEnumerable<FlooredAdProvider>> p)
				{
					preload(p.Item1);
				}).AddTo(_disposable);
			}
		}
	}

	protected UniRx.IObservable<long> beginningsOfLoadingCycle(UniRx.IObservable<bool> adReadies)
	{
		ReadOnlyReactiveProperty<long> source = (from r in adReadies
			where !r
			select r into _
			select ServerTimeService.NowTicks()).ToReadOnlyReactiveProperty();
		return (from _ in source
			select Observable.Return(0L).Concat(Observable.Interval(TimeSpan.FromSeconds(preloadNotAvailableWaitingPeriod))).TakeUntil(from r in adReadies
				where r
				select r)).Switch();
	}

	private UniRx.IObservable<bool> previousOrMeGotReady<PROVIDER>(PROVIDER provider, IEnumerable<PROVIDER> previousProviders) where PROVIDER : AdProvider
	{
		return from r in (from p in previousProviders
				select p.AdsReady.AsObservable()).ToObservable().Merge().Merge(provider.AdsReady)
			where r
			select r;
	}

	protected void preload(AdProvider provider)
	{
		if (provider.AdsReady.Value)
		{
			this.debug("Got inventory from " + provider.ProviderInfo() + ", not going to preload. ");
			return;
		}
		this.debug("Preloading for " + provider.ProviderInfo());
		provider.TryToCache();
	}

	public abstract void Init();
}
