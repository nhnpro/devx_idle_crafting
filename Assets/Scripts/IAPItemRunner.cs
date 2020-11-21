using UniRx;

[PropertyClass]
public class IAPItemRunner
{
	[PropertyInt]
	public ReactiveProperty<int> RewardAmount = new ReactiveProperty<int>();

	[PropertyString]
	public ReactiveProperty<string> PriceString;

	[PropertyString]
	public ReactiveProperty<string> Description = new ReactiveProperty<string>();

	private IAPConfig m_iapConfig;

	public IAPProductEnum ProductID
	{
		get;
		private set;
	}

	public IAPItemRunner(IAPProductEnum productID)
	{
		SceneLoader instance = SceneLoader.Instance;
		ProductID = productID;
		m_iapConfig = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig s) => s.ProductEnum == ProductID);
		RewardAmount.Value = m_iapConfig.Reward.Amount.ToInt();
		Description.Value = PersistentSingleton<LocalizationService>.Instance.Text("IAPItem." + ProductID);
		PriceString = (from fetched in PersistentSingleton<IAPService>.Instance.DataFetched
			select (!fetched) ? PersistentSingleton<LocalizationService>.Instance.Text("IAPItem.DefaultButtonText") : m_iapConfig.GetLocalizedPriceStringOrDefault()).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	public void BuyIAPProduct(string placement)
	{
		Singleton<IAPRunner>.Instance.BuyIAP(m_iapConfig, placement);
	}
}
