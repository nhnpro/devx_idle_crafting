using Big;
using UniRx;

[PropertyClass]
public class FakeFundRunner : Singleton<FakeFundRunner>
{
	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> Coins = new ReactiveProperty<BigDouble>(0L);

	[PropertyInt]
	public ReactiveProperty<int> Gems = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> Keys = new ReactiveProperty<int>(0);

	public FakeFundRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
	}

	public void CopyFunds()
	{
		Coins.Value = PlayerData.Instance.Coins.Value;
		Gems.Value = PlayerData.Instance.Gems.Value;
		Keys.Value = PlayerData.Instance.Keys.Value;
	}

	public void AddCoins(BigDouble coins)
	{
		Coins.Value += coins;
	}

	public void AddGems(int gems)
	{
		Gems.Value += gems;
	}

	public void AddKeys(int keys)
	{
		Keys.Value += keys;
	}
}
