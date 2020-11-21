using Big;
using UniRx;

[PropertyFormerlyAs("CraftingResourceRunner")]
[PropertyClass]
public class BlockResourceRunner
{
	[PropertyString]
	public ReadOnlyReactiveProperty<string> CollectedAmount;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> BackpackAmount;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> PrestigeBackpackAmount;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> PrestigeBackpackMult;

	public BlockResourceRunner(BlockType type)
	{
		SceneLoader instance = SceneLoader.Instance;
		ReactiveProperty<long> source = PlayerData.Instance.BlocksCollected[(int)type];
		CollectedAmount = (from amount in source
			select amount.ToString()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		ReactiveProperty<long> source2 = PlayerData.Instance.BlocksInBackpack[(int)type];
		BackpackAmount = (from amount in source2
			select amount.ToString()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		PrestigeBackpackMult = (from mult in Singleton<CumulativeBonusRunner>.Instance.BonusMult[(int)(13 + type)]
			select (!(mult == new BigDouble(1.0))) ? (BigString.ToString(mult) + "x") : string.Empty).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		PrestigeBackpackAmount = (from pair in source2.Pairwise()
			where pair.Current == 0
			select pair.Previous into amount
			select amount.ToString()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}
}
