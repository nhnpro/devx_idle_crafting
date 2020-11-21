using Big;
using UniRx;

public class PlayerGoalActionTriggered : PlayerGoalAction
{
	public PlayerGoalActionTriggered(ReactiveProperty<int> rxCompleted, ReactiveProperty<int> rxClaimed, BigDouble[] req)
	{
		CompletedStars = rxCompleted;
		ClaimedStars = rxClaimed;
		Progress = new ReactiveProperty<float>(0f);
		ProgressCurrent = new ReactiveProperty<BigDouble>(0L);
		ProgressMax = from done in ClaimedStars
			select PlayerGoalActionBigDouble.GetReq(done, req);
	}

	public override void Complete()
	{
		((ReactiveProperty<int>)CompletedStars).Value = ((ReactiveProperty<int>)CompletedStars).Value + 1;
		((ReactiveProperty<float>)Progress).Value = 1f;
		((ReactiveProperty<BigDouble>)ProgressCurrent).Value = 1L;
	}
}
