using UniRx;

public class TweakableFloat : Tweakable
{
	public ReactiveProperty<float> Float = new ReactiveProperty<float>();

	public float Min
	{
		get;
		private set;
	}

	public float Max
	{
		get;
		private set;
	}

	public TweakableFloat(string name, float val, float min, float max)
	{
		base.TweakableName = name;
		Min = min;
		Max = max;
		Float.Value = val;
		TweakableManagerUI.RegisterTweakable(this);
	}
}
