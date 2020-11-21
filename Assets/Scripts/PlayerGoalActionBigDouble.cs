using Big;
using System;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;

public class PlayerGoalActionBigDouble : PlayerGoalAction
{
	[CompilerGenerated]
	private static Func<BigDouble, BigDouble, BigDouble> _003C_003Ef__mg_0024cache0;

	public PlayerGoalActionBigDouble(UniRx.IObservable<BigDouble> rxProperty, UniRx.IObservable<int> rxClaimed, BigDouble[] req)
	{
		SceneLoader instance = SceneLoader.Instance;
		UniRx.IObservable<int> left = (from val in rxProperty
			select GetStarLevel(val, req)).DistinctUntilChanged();
		ClaimedStars = rxClaimed;
		CompletedStars = left.CombineLatest(ClaimedStars, (int comp, int claim) => Mathf.Min(comp, claim + 1)).TakeUntilDestroy(instance).ToReactiveProperty();
		Progress = rxProperty.CombineLatest(ClaimedStars, (BigDouble val, int done) => GetProgress(val, done, req));
		ProgressMax = from done in ClaimedStars
			select GetReq(done, req);
		ProgressCurrent = rxProperty.CombineLatest(ProgressMax, BigDouble.Min).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	private int GetStarLevel(BigDouble val, BigDouble[] req)
	{
		for (int i = 0; i < req.Length; i++)
		{
			if (req[i] < 0L)
			{
				return i;
			}
			if (val < req[i])
			{
				return i;
			}
		}
		return req.Length;
	}

	private float GetProgress(BigDouble val, int current, BigDouble[] req)
	{
		BigDouble req2 = GetReq(current - 1, req);
		BigDouble req3 = GetReq(current, req);
		return Mathf.Clamp01(((val - req2) / (req3 - req2)).ToFloat());
	}

	public static BigDouble GetReq(int index, BigDouble[] req)
	{
		if (index < 0)
		{
			return BigDouble.ZERO;
		}
		if (index >= req.Length)
		{
			return new BigDouble(-1.0);
		}
		return req[index];
	}
}
