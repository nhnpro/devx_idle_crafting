using Big;
using System;
using System.Collections;
using UniRx;

public class FundRunner : Singleton<FundRunner>
{
	private BigDouble m_addMoney = BigDouble.ZERO;

	private long m_addTap;

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigeInit
			select order).Subscribe(delegate
		{
			PrestigeFunds();
		}).AddTo(instance);
		instance.StartCoroutine(UpdateFrequentProperties());
	}

	public void AddCoins(BigDouble amount)
	{
		m_addMoney += amount;
	}

	public void CountBlockDestroy(BlockType block)
	{
		if (block < BlockType.NumCraftingTypes)
		{
			if (block != BlockType.Gold)
			{
				AddResource(block, 1L);
			}
			PlayerData.Instance.LifetimeBlocksDestroyed[(int)block].Value++;
		}
	}

	public void CountBlockTap()
	{
		m_addTap++;
	}

	private IEnumerator UpdateFrequentProperties()
	{
		while (true)
		{
			if (m_addMoney != BigDouble.ZERO)
			{
				PlayerData.Instance.LifetimeCoins.Value += m_addMoney;
				PlayerData.Instance.Coins.Value += m_addMoney;
				m_addMoney = BigDouble.ZERO;
			}
			if (m_addTap != 0)
			{
				PlayerData.Instance.LifetimeBlocksTaps.Value += m_addTap;
				m_addTap = 0L;
			}
			yield return null;
		}
	}

	public void RemoveCoins(BigDouble amount)
	{
		PlayerData.Instance.Coins.Value -= amount;
	}

	public void AddResource(BlockType block, long amount)
	{
		PlayerData.Instance.BlocksInBackpack[(int)block].Value += amount;
	}

	public void AddGems(int amount, string reason, string type)
	{
		PlayerData.Instance.LifetimeGems.Value += amount;
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.GemTransactions.Value = new GemTransaction(amount, "addFunds", reason, type, PlayerData.Instance.MainChunk.Value);
		}
		PlayerData.Instance.Gems.Value += amount;
	}

	public void RemoveGems(int amount, string reason, string type)
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.GemTransactions.Value = new GemTransaction(amount, "spendFunds", reason, type, PlayerData.Instance.MainChunk.Value);
		}
		PlayerData.Instance.Gems.Value -= amount;
	}

	public void AddMedals(int amount)
	{
		PlayerData.Instance.LifetimeMedals.Value += amount;
		PlayerData.Instance.Medals.Value += amount;
	}

	public void RemoveMedals(int amount)
	{
		PlayerData.Instance.Medals.Value -= amount;
	}

	public void AddRelicsToBackpak(BigDouble amount)
	{
		PlayerData.Instance.BlocksInBackpack[6].Value += amount.ToLong();
	}

	public void RemoveRelicsFromBackpack(BigDouble amount)
	{
		PlayerData.Instance.BlocksInBackpack[6].Value -= amount.ToLong();
	}

	public void AddRelicsToFunds(BigDouble amount)
	{
		PlayerData.Instance.LifetimeRelics.Value += amount.ToLong();
		PlayerData.Instance.BlocksCollected[6].Value += amount.ToLong();
	}

	public void RemoveFromFunds(CraftingRequirement req)
	{
		for (int i = 0; i < 7; i++)
		{
			PlayerData.Instance.BlocksCollected[i].Value -= req.Resources[i];
		}
	}

	public void RemoveFromFundsOrZero(CraftingRequirement req)
	{
		for (int i = 0; i < 7; i++)
		{
			PlayerData.Instance.BlocksCollected[i].Value = Math.Max(0L, PlayerData.Instance.BlocksCollected[i].Value - req.Resources[i]);
		}
	}

	public void AddNormalChests(int amount, string reason)
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ChestTransactions.Value = new ChestTransaction(amount, "addBronzeChest", reason);
		}
		PlayerData.Instance.NormalChests.Value += amount;
	}

	public void RemoveNormalChests(int amount)
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ChestTransactions.Value = new ChestTransaction(amount, "openBronzeChest", "open");
		}
		PlayerData.Instance.NormalChests.Value -= amount;
	}

	public void AddSilverChests(int amount, string reason)
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ChestTransactions.Value = new ChestTransaction(amount, "addSilverChest", reason);
		}
		PlayerData.Instance.SilverChests.Value += amount;
	}

	public void RemoveSilverChests(int amount)
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ChestTransactions.Value = new ChestTransaction(amount, "openSilverChest", "open");
		}
		PlayerData.Instance.SilverChests.Value -= amount;
	}

	public void AddGoldChests(int amount, string reason)
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ChestTransactions.Value = new ChestTransaction(amount, "addGoldChest", reason);
		}
		PlayerData.Instance.GoldChests.Value += amount;
	}

	public void RemoveGoldChests(int amount)
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ChestTransactions.Value = new ChestTransaction(amount, "openGoldChest", "open");
		}
		PlayerData.Instance.GoldChests.Value -= amount;
	}

	public void AddBerry(int amount, int heroIndex, string reason)
	{
		PlayerData.Instance.LifetimeBerries.Value += amount;
		HeroState orCreateHeroState = HeroStateFactory.GetOrCreateHeroState(heroIndex);
		orCreateHeroState.UnusedBerries.Value += amount;
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.BerryTransaction.Value = new BerryTransaction(amount, heroIndex, reason);
		}
	}

	public void AddKeys(int amount, string reason)
	{
		PlayerData.Instance.LifetimeKeys.Value += amount;
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.KeyTransactions.Value = new KeyTransaction(amount, reason);
		}
		PlayerData.Instance.Keys.Value += amount;
	}

	public void RemoveKeys(int amount)
	{
		if (amount == PersistentSingleton<GameSettings>.Instance.NormalChestKeyCost && PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ChestTransactions.Value = new ChestTransaction(1, "openKeyChest", "open");
		}
		PlayerData.Instance.Keys.Value -= amount;
	}

	public void OpenChests(int amount)
	{
		PlayerData.Instance.LifetimeAllOpenedChests.Value += amount;
	}

	public void PrestigeFunds()
	{
		PlayerData.Instance.Coins.Value = BigDouble.ZERO;
		AddRelicsToFunds((double)PlayerData.Instance.BlocksInBackpack[6].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[19].Value.ToDouble());
		PlayerData.Instance.BlocksInBackpack[6].Value = 0L;
		for (int i = 0; i < 6; i++)
		{
			long num = (long)((double)PlayerData.Instance.BlocksInBackpack[i].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[13 + i].Value.ToDouble());
			PlayerData.Instance.BlocksCollected[i].Value += num;
			PlayerData.Instance.BlocksInBackpack[i].Value = 0L;
		}
	}
}
