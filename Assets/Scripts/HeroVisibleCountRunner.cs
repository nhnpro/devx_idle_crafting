using UniRx;

[PropertyClass]
public class HeroVisibleCountRunner : Singleton<HeroVisibleCountRunner>
{
	[PropertyInt]
	public ReactiveProperty<int> HeroVisibleCount;

	public HeroVisibleCountRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		HeroVisibleCount = (from chunk in PlayerData.Instance.MainChunk
			select GetVisibleCount(chunk)).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	private int GetVisibleCount(int chunk)
	{
		return Singleton<EconomyHelpers>.Instance.GetNumHeroes();
	}
}
