using UniRx;

[PropertyClass]
public class NotEnoughGemsRunner : Singleton<NotEnoughGemsRunner>
{
	[PropertyInt]
	private ReactiveProperty<int> MissingGems = new ReactiveProperty<int>(0);

	public ReactiveProperty<IAPConfig> SmallestGemPurchase;

	public ReactiveProperty<IAPConfig> BiggestGemPurchase = new ReactiveProperty<IAPConfig>(PersistentSingleton<Economies>.Instance.IAPs[5]);

	public NotEnoughGemsRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		SmallestGemPurchase = (from gems in MissingGems
			select Singleton<EconomyHelpers>.Instance.GetSmallestGemPurchaseNeeded(gems)).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	public void NotEnoughGems(int missingGems)
	{
		MissingGems.SetValueAndForceNotify(missingGems);
	}
}
