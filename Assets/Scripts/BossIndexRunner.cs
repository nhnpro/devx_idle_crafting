using UniRx;

[PropertyClass]
public class BossIndexRunner : Singleton<BossIndexRunner>
{
	public ReactiveProperty<int> CurrentBossIndex;

	[PropertyBool]
	public ReactiveProperty<bool> IsCurrentBossNew;

	public BossIndexRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		CurrentBossIndex = (from chk in Singleton<WorldRunner>.Instance.CurrentChunk
			select GetBossIndex(chk.Index)).TakeUntilDestroy(instance).ToReactiveProperty();
		IsCurrentBossNew = (from chk in Singleton<WorldRunner>.Instance.CurrentChunk
			select GetMegaBossIndexOrNone(chk.Index) != -1).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	private int GetMegaBossIndexOrNone(int chunk)
	{
		return Singleton<EconomyHelpers>.Instance.GetNewCreatureInChunkOrNone(chunk);
	}

	private int GetBossIndex(int chunk)
	{
		return GetMegaBossIndexOrNone(chunk);
	}
}
