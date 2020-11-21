using System;
using System.Collections.Generic;

public class GlobalAnalyticsParameters : PersistentSingleton<GlobalAnalyticsParameters>, IGameAnalyticsParameters
{
	public void InitializeGlobalAnalyticsParameters()
	{
		if (!base.Inited)
		{
			base.Inited = true;
		}
	}

	public void AddFacebookParameters(Dictionary<string, object> p)
	{
		PlayerData instance = PlayerData.Instance;
		p.Add("fb_level", instance.LifetimeChunk.Value.ToString());
	}

	public void AddFirebaseParameters(Dictionary<string, string> p)
	{
		PlayerData instance = PlayerData.Instance;
		SessionService instance2 = PersistentSingleton<SessionService>.Instance;
		p.Add("Ads_Lifetime", instance.AdsInLifetime.ToString());
		p.Add("Ads_Session", instance.AdsInSession.ToString());
		p.Add("Purchases", instance.PurchasesMade.ToString());
		p.Add("InvalidPurchases", instance.InvalidPurchasesMade.ToString());
		p.Add("Current_Gems", instance.Gems.Value.ToString());
		p.Add("Session_Number", instance.SessionNumber.Value.ToString());
		p.Add("Prestiges", instance.LifetimePrestiges.Value.ToString());
		p.Add("Lifetime_Gems", instance.LifetimeGems.Value.ToString());
		p.Add("Max_Chunk", instance.LifetimeChunk.Value.ToString());
		p.Add("Lifetime_Money_Exponent", instance.LifetimeCoins.Value.exponent.ToString());
		p.Add("Max_Companions_Unlocked", instance.LifetimeCreatures.Value.ToString());
		p.Add("Minutes_In_Lifetime", ((int)TimeSpan.FromTicks(instance2.TicksPlayedInLifetime()).TotalMinutes).ToString());
		p.Add("Minutes_In_Session", ((int)TimeSpan.FromTicks(instance2.TicksInCurrentSession()).TotalMinutes).ToString());
		p.Add("Max_Hero_Level", HeroStateFactory.GetOrCreateHeroState(0).LifetimeLevel.Value.ToString());
		p.Add("Lifetime_Gears", instance.LifetimeGears.Value.ToString());
		p.Add("Lifetime_Gear_Levels", instance.LifetimeGearLevels.Value.ToString());
	}
}
