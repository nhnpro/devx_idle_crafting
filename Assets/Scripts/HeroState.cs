using UniRx;

public class HeroState
{
	public const int LockedLevel = 0;

	public const int UnlockedLevel = 1;

	public ReactiveProperty<int> Level = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> LifetimeLevel = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> Tier = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> Berries = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> UnusedBerries = new ReactiveProperty<int>(0);
}
