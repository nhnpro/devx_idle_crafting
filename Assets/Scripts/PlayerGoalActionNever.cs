using Big;
using UniRx;

public class PlayerGoalActionNever : PlayerGoalAction
{
	public PlayerGoalActionNever()
	{
		CompletedStars = Observable.Never(0);
		ClaimedStars = Observable.Never(0);
		Progress = new ReactiveProperty<float>(0f);
		ProgressCurrent = new ReactiveProperty<BigDouble>(0L);
		ProgressMax = new ReactiveProperty<BigDouble>(0L);
	}
}
