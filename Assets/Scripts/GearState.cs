using UniRx;

public class GearState
{
	public const int LockedLevel = 0;

	public const int UnlockedLevel = 1;

	public ReactiveProperty<int> Level = new ReactiveProperty<int>(0);
}
