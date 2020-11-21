using UniRx;

public class PlayerGoalState
{
	public string ID;

	public ReactiveProperty<int> CompletedStars = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> ClaimedStars = new ReactiveProperty<int>(0);
}
