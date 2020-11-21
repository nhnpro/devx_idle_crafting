using Big;
using UniRx;

public class PlayerGoalActionSkip : PlayerGoalAction
{
	public PlayerGoalActionSkip()
	{
		CompletedStars = Observable.Return(5);
		ClaimedStars = Observable.Return(5);
		Progress = new ReactiveProperty<float>(1f);
		ProgressCurrent = new ReactiveProperty<BigDouble>(1L);
		ProgressMax = new ReactiveProperty<BigDouble>(1L);
	}
}
