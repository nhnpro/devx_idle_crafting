using Big;
using UniRx;

public class PlayerGoalAction
{
	public UniRx.IObservable<int> CompletedStars;

	public UniRx.IObservable<int> ClaimedStars;

	public UniRx.IObservable<float> Progress;

	public UniRx.IObservable<BigDouble> ProgressCurrent;

	public UniRx.IObservable<BigDouble> ProgressMax;

	public virtual void Complete()
	{
	}
}
