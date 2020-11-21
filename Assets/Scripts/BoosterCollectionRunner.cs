using System.Collections.Generic;
using UniRx;

public class BoosterCollectionRunner : Singleton<BoosterCollectionRunner>
{
	private List<BoosterRunner> m_boosterRunners = new List<BoosterRunner>();

	public ReactiveProperty<int> ActiveBooster = new ReactiveProperty<int>(-1);

	public BoosterRunner GetOrCreateBoosterRunner(int booster)
	{
		m_boosterRunners.EnsureSize(booster, (int count) => new BoosterRunner(count));
		return m_boosterRunners[booster];
	}

	public void SetActiveBooster(int booster)
	{
		ActiveBooster.SetValueAndForceNotify(booster);
		Singleton<GoldBoosterCollectionRunner>.Instance.ActiveBooster.SetValueAndForceNotify(-1);
	}

	public void GiveBooster(BoosterEnum booster, float reward)
	{
		PlayerData.Instance.BoostersEffect[(int)booster].Value += reward;
	}

	public static string GetBoosterBonusString(BoosterEnum booster)
	{
		string empty = string.Empty;
		if (IsBoosterMultiplier(booster))
		{
			if (PlayerData.Instance.BoostersEffect[(int)booster].Value % 1f != 0f)
			{
				return PlayerData.Instance.BoostersEffect[(int)booster].Value.ToString("0.0") + "X";
			}
			return PlayerData.Instance.BoostersEffect[(int)booster].Value.ToString() + "X";
		}
		if (booster == BoosterEnum.HammerDuration)
		{
			int duration = (int)PlayerData.Instance.BoostersEffect[(int)booster].Value + PersistentSingleton<GameSettings>.Instance.GoldenHammerInitialDuration;
			return FormatSecondsForBoosters(duration);
		}
		int duration2 = (int)PlayerData.Instance.BoostersEffect[(int)booster].Value;
		return FormatSecondsForBoosters(duration2);
	}

	public static string GetBoosterNextUpgradeString(BoosterEnum booster)
	{
		string empty = string.Empty;
		if (IsBoosterMultiplier(booster))
		{
			return "+" + PersistentSingleton<Economies>.Instance.Boosters[(int)booster].RewardAmount.ToString() + "X";
		}
		int duration = (int)PersistentSingleton<Economies>.Instance.Boosters[(int)booster].RewardAmount;
		return "+" + FormatSecondsForBoosters(duration);
	}

	public static string FormatSecondsForBoosters(int duration)
	{
		string text = string.Empty;
		int num = duration % 60;
		int num2 = (duration - num) / 60 % 60;
		int num3 = (duration - num - num2 * 60) / 3600;
		if (num3 >= 1)
		{
			text = num3 + "h ";
		}
		if (num2 >= 1)
		{
			text = text + num2 + "m";
		}
		if (num >= 1)
		{
			text = text + " " + num + "s";
		}
		return text;
	}

	public static bool IsBoosterMultiplier(BoosterEnum booster)
	{
		switch (booster)
		{
		case BoosterEnum.DamageMultiplier:
		case BoosterEnum.GoldMultiplier:
		case BoosterEnum.ShardMultiplier:
		case BoosterEnum.DailyDoubleBoost:
		case BoosterEnum.KeyDropChance:
		case BoosterEnum.BerryDropChance:
			return true;
		default:
			return false;
		}
	}
}
