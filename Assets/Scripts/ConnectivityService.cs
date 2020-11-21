using System;
using UniRx;
using UnityEngine;

public static class ConnectivityService
{
	public static ReactiveProperty<bool> InternetConnectionAvailable;

	public static bool Inited
	{
		get;
		private set;
	}

	public static void StartUp()
	{
		if (!Inited)
		{
			InternetConnectionAvailable = (from _ in Observable.Interval(TimeSpan.FromSeconds(1.0))
				select Application.internetReachability != NetworkReachability.NotReachable).DistinctUntilChanged().ToReactiveProperty();
			Inited = true;
		}
	}
}
