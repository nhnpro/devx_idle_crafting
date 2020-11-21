using UniRx;

public class TweakableInt : Tweakable
{
	public ReactiveProperty<int> Int = new ReactiveProperty<int>();

	public int Min
	{
		get;
		private set;
	}

	public int Max
	{
		get;
		private set;
	}

	public TweakableInt(string name, int val, int min, int max)
	{
		base.TweakableName = name;
		Min = min;
		Max = max;
		Int.Value = val;
		TweakableManagerUI.RegisterTweakable(this);
	}
}
