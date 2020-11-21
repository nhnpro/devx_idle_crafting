using UniRx;

public class TweakableBool : Tweakable
{
	public ReactiveProperty<bool> Bool = new ReactiveProperty<bool>();

	public TweakableBool(string name, bool b)
	{
		base.TweakableName = name;
		Bool.Value = b;
		TweakableManagerUI.RegisterTweakable(this);
	}
}
