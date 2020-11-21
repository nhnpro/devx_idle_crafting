using UniRx;

public interface FlooredAdProvider : AdProvider
{
	ReadOnlyReactiveProperty<int> FloorValue
	{
		get;
	}

	ReadOnlyReactiveProperty<string> Zone
	{
		get;
	}
}
