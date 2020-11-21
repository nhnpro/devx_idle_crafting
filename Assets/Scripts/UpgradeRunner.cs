using Big;
using System.Collections.Generic;
using UniRx;

public class UpgradeRunner : Singleton<UpgradeRunner>
{
	public ReactiveProperty<int> GearUnlockTriggered = Observable.Never<int>().ToReactiveProperty();

	public ReactiveProperty<UpgradeEnum> UpgradeStep = new ReactiveProperty<UpgradeEnum>(UpgradeEnum.One);

	public UpgradeRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigeInit
			select order).Subscribe(delegate
		{
			ResetUpgrades();
		}).AddTo(instance);
	}

	public void UpgradeHero(int hero)
	{
		HeroCostRunner orCreateHeroCostRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroCostRunner(hero);
		if (!(PlayerData.Instance.Coins.Value < orCreateHeroCostRunner.Cost.Value))
		{
			BigDouble value = orCreateHeroCostRunner.Cost.Value;
			int value2 = orCreateHeroCostRunner.LevelsToUpgrade.Value;
			Singleton<FundRunner>.Instance.RemoveCoins(value);
			Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(hero).Level.Value += value2;
		}
	}

	public void UpgradeGear(int gear)
	{
		GearRunner orCreateGearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gear);
		if (orCreateGearRunner.UpgradeAvailable.Value && !orCreateGearRunner.MaxLevelReached.Value)
		{
			if (orCreateGearRunner.Level.Value == 0)
			{
				GearUnlockTriggered.SetValueAndForceNotify(orCreateGearRunner.GearIndex);
			}
			CraftingRequirement gearUpgradeCost = Singleton<EconomyHelpers>.Instance.GetGearUpgradeCost(orCreateGearRunner.GearIndex, orCreateGearRunner.Level.Value);
			Singleton<FundRunner>.Instance.RemoveFromFunds(gearUpgradeCost);
			orCreateGearRunner.Level.Value++;
		}
	}

	public void UpgradeGearWithoutLimit(int gear, int levels)
	{
		GearRunner orCreateGearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gear);
		orCreateGearRunner.Level.Value += levels;
	}

	public void UpgradeGearWithGems(int gear)
	{
		GearRunner orCreateGearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gear);
		CraftingRequirement gearUpgradeCost = Singleton<EconomyHelpers>.Instance.GetGearUpgradeCost(orCreateGearRunner.GearIndex, orCreateGearRunner.Level.Value);
		if (!orCreateGearRunner.MaxLevelReached.Value)
		{
			if (orCreateGearRunner.Level.Value == 0)
			{
				GearUnlockTriggered.SetValueAndForceNotify(orCreateGearRunner.GearIndex);
			}
			int craftingGemCost = Singleton<EconomyHelpers>.Instance.GetCraftingGemCost(AsCraftingRequirement(PlayerData.Instance.BlocksCollected), gearUpgradeCost);
			if (craftingGemCost > PlayerData.Instance.Gems.Value)
			{
				NotEnoughGemsForUpgrade(gear);
				BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
			}
			else
			{
				Singleton<FundRunner>.Instance.RemoveFromFundsOrZero(gearUpgradeCost);
				Singleton<FundRunner>.Instance.RemoveGems(craftingGemCost, "gear_" + gear.ToString(), "gears");
				orCreateGearRunner.Level.Value++;
			}
		}
	}

	public void NotEnoughGemsForUpgrade(int gear)
	{
		GearRunner orCreateGearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gear);
		CraftingRequirement gearUpgradeCost = Singleton<EconomyHelpers>.Instance.GetGearUpgradeCost(orCreateGearRunner.GearIndex, orCreateGearRunner.Level.Value);
		int craftingGemCost = Singleton<EconomyHelpers>.Instance.GetCraftingGemCost(AsCraftingRequirement(PlayerData.Instance.BlocksCollected), gearUpgradeCost);
		int missingGems = craftingGemCost - PlayerData.Instance.Gems.Value;
		Singleton<NotEnoughGemsRunner>.Instance.NotEnoughGems(missingGems);
	}

	private static CraftingRequirement AsCraftingRequirement(List<ReactiveProperty<long>> blocks)
	{
		CraftingRequirement craftingRequirement = new CraftingRequirement();
		for (int i = 0; i < 7; i++)
		{
			craftingRequirement.Resources[i] = blocks[i].Value;
		}
		return craftingRequirement;
	}

	private void ResetUpgrades()
	{
		Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(0).Level.Value = 1;
	}

	public void SetUpgradeStep(UpgradeEnum step)
	{
		UpgradeStep.Value = step;
	}
}
