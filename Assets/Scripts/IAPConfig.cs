using UnityEngine.Purchasing;

public class IAPConfig
{
	public string AppleID;

	public string GoogleID;

	public IAPType Type;

	public IAPProductEnum ProductEnum;

	public RewardData Reward;

	public RewardData[] ExtraRewards = new RewardData[2];

	public IAPProductEnum SaleEnum;

	public ProductMetadata StoreProduct;

	public string ProductID => AppleID;

	public string GetLocalizedPriceStringOrDefault()
	{
		if (StoreProduct == null)
		{
			return PersistentSingleton<LocalizationService>.Instance.Text("IAPItem.DefaultButtonText");
		}
		return StoreProduct.localizedPriceString;
	}

	public string GetLocalizedPriceStringOrEmpty()
	{
		if (StoreProduct == null)
		{
			return string.Empty;
		}
		return StoreProduct.localizedPriceString;
	}
}
