using Big;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public static class BonusTypeHelper
{
	public static Func<BigDouble, float, BigDouble> CreateBigDoubleFunction(BonusTypeEnum type)
	{
		if (IsTimeType(type))
		{
			return (BigDouble a, float b) => a + b;
		}
		return (BigDouble a, float b) => a * b;
	}

	public static Func<float, float, float> CreateFunction(BonusTypeEnum type)
	{
		if (IsTimeType(type))
		{
			return (float a, float b) => a + b;
		}
		return (float a, float b) => a * b;
	}

	public static Func<float, float, float> CreateFunctionStacking(BonusTypeEnum type)
	{
		if (IsTimeType(type))
		{
			return (float a, float b) => a + b;
		}
		return (float a, float b) => a + b - 1f;
	}

	public static float GetOrigin(BonusTypeEnum type)
	{
		if (IsTimeType(type))
		{
			return 0f;
		}
		return 1f;
	}

	public static bool IsTimeType(BonusTypeEnum type)
	{
		switch (type)
		{
		case BonusTypeEnum.BossTime:
		case BonusTypeEnum.SkillAutoMineDuration:
		case BonusTypeEnum.SkillTeamBoostDuration:
		case BonusTypeEnum.SkillTapBoostDuration:
		case BonusTypeEnum.SkillGoldfingerDuration:
			return true;
		default:
			return false;
		}
	}

	public static UniRx.IObservable<float> CreateOrigin(BonusTypeEnum type)
	{
		return Observable.Return(GetOrigin(type));
	}

	public static Func<float, float> CreateIdentityFunc(BonusTypeEnum type)
	{
		return (float a) => GetOrigin(type);
	}

	public static UniRx.IObservable<float> CreateCombine(BonusTypeEnum type, List<UniRx.IObservable<float>> multipliers)
	{
		Func<float, float, float> func = CreateFunction(type);
		UniRx.IObservable<float> observable = CreateOrigin(type);
		foreach (UniRx.IObservable<float> multiplier in multipliers)
		{
			observable = observable.CombineLatest(multiplier, (float left, float right) => func(left, right));
		}
		return observable;
	}

	public static UniRx.IObservable<float> CreateCombine(BonusTypeEnum type, List<UniRx.IObservable<float>> mult1, List<UniRx.IObservable<float>> mult2)
	{
		Func<float, float, float> func = CreateFunction(type);
		UniRx.IObservable<float> observable = CreateOrigin(type);
		foreach (UniRx.IObservable<float> item in mult1)
		{
			observable = observable.CombineLatest(item, (float left, float right) => func(left, right));
		}
		foreach (UniRx.IObservable<float> item2 in mult2)
		{
			observable = observable.CombineLatest(item2, (float left, float right) => func(left, right));
		}
		return observable;
	}

	public static UniRx.IObservable<float> CreateCombineStacking(BonusTypeEnum type, List<UniRx.IObservable<float>> mult1, List<UniRx.IObservable<float>> mult2)
	{
		Func<float, float, float> func = CreateFunctionStacking(type);
		UniRx.IObservable<float> observable = CreateOrigin(type);
		foreach (UniRx.IObservable<float> item in mult1)
		{
			observable = observable.CombineLatest(item, (float left, float right) => func(left, right));
		}
		foreach (UniRx.IObservable<float> item2 in mult2)
		{
			observable = observable.CombineLatest(item2, (float left, float right) => func(left, right));
		}
		return observable;
	}

	public static string FormatToBonus(BonusTypeEnum type, float val)
	{
		if (IsTimeType(type))
		{
			return val.ToString("0.##");
		}
		return Mathf.Abs(val * 100f - 100f).ToString("0.##");
	}

	public static string GetStepText(BonusTypeEnum type, float val)
	{
		if (IsTimeType(type))
		{
			return PersistentSingleton<LocalizationService>.Instance.Text("Attribute.StepDuration", ((!(val > BigDouble.ZERO)) ? string.Empty : "+") + val.ToString("0.##"));
		}
		return PersistentSingleton<LocalizationService>.Instance.Text("Attribute.StepMultiplier", "+" + Mathf.Abs(val * 100f).ToString("0.##"));
	}

	public static string GetAttributeText(BonusTypeEnum type, float val)
	{
		string text = FormatToBonus(type, val);
		return PersistentSingleton<LocalizationService>.Instance.Text("Attribute." + type, text);
	}
}
