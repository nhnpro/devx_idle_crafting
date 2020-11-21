using UniRx;

public class SkillState
{
	public ReactiveProperty<int> Amount = new ReactiveProperty<int>(3);

	public ReactiveProperty<long> CooldownTimeStamp = new ReactiveProperty<long>(0L);

	public ReactiveProperty<long> ElapsedTime = new ReactiveProperty<long>(85536000000000L);

	public ReactiveProperty<int> LifetimeUsed = new ReactiveProperty<int>(0);
}
