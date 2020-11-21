using System;
using System.Collections;
using UniRx;
using UnityEngine;

public static class TickerService
{
	public static Subject<long> MasterTicks = new Subject<long>();

	public static Subject<long> MasterTicksSlow = new Subject<long>();

	public static Subject<long> MasterTicksFast = new Subject<long>();

	public static void StartTickers(MonoBehaviour mono)
	{
		mono.StartCoroutine(TickRoutine(MasterTicks, 0.1f, 1f));
		mono.StartCoroutine(TickRoutine(MasterTicksSlow, 1f, 2f));
		mono.StartCoroutine(TickRoutine(MasterTicksFast, 0f, 1f));
	}

	private static IEnumerator TickRoutine(Subject<long> subject, float interval, float maxInterval)
	{
		long time = ServerTimeService.NowTicks();
		while (true)
		{
			yield return new WaitForSeconds(interval);
			long delta = ServerTimeService.NowTicks() - time;
			long t = Math.Max(0L, Math.Min(delta, (long)maxInterval * 10000000));
			subject.OnNext(t);
			time = ServerTimeService.NowTicks();
		}
	}
}
