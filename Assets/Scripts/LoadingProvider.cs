using UniRx;

public interface LoadingProvider : AdProvider
{
	ReactiveProperty<bool> Loading
	{
		get;
	}
}
