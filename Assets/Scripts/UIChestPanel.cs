using UnityEngine;

public class UIChestPanel : MonoBehaviour
{
	public void OnOpenNormalChest()
	{
		Singleton<ChestRunner>.Instance.OpenNormalChest();
		BindingManager.Instance.BronzeChestAnimObj.SetActive(value: true);
	}

	public void OnOpenSilverChest()
	{
		Singleton<ChestRunner>.Instance.OpenSilverChest();
		BindingManager.Instance.SilverChestAnimObj.SetActive(value: true);
	}

	public void OnOpenGoldChest()
	{
		Singleton<ChestRunner>.Instance.OpenGoldChest();
		BindingManager.Instance.GoldChestAnimObj.SetActive(value: true);
	}

	public void OnBuyNormalChestWithKeys()
	{
		Singleton<ChestRunner>.Instance.BuyNormalChestWithKeys(doubleReward: false);
		BindingManager.Instance.KeyChestAnimObj.SetActive(value: true);
	}

	public void OnBuyNormalChestWithGems()
	{
		Singleton<ChestRunner>.Instance.BuyNormalChestWithGems();
		BindingManager.Instance.BronzeChestAnimObj.SetActive(value: true);
	}

	public void OnNotEnoughGemsForNormalChest()
	{
		Singleton<ChestRunner>.Instance.NotEnoughGemsForChest(PersistentSingleton<GameSettings>.Instance.NormalChestGemCost);
		BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
	}

	public void OnBuySilverChestWithGems()
	{
		Singleton<ChestRunner>.Instance.BuySilverChestWithGems();
		BindingManager.Instance.SilverChestAnimObj.SetActive(value: true);
	}

	public void OnNotEnoughGemsForSilverChest()
	{
		Singleton<ChestRunner>.Instance.NotEnoughGemsForChest(PersistentSingleton<GameSettings>.Instance.SilverChestGemCost);
		BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
	}

	public void OnBuyGoldChestWithGems()
	{
		Singleton<ChestRunner>.Instance.BuyGoldChestWithGems();
		BindingManager.Instance.GoldChestAnimObj.SetActive(value: true);
	}

	public void OnNotEnoughGemsForGoldChest()
	{
		Singleton<ChestRunner>.Instance.NotEnoughGemsForChest(PersistentSingleton<GameSettings>.Instance.GoldChestGemCost);
		BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
	}
}
