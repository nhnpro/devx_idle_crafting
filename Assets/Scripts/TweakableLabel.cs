using UniRx;

public class TweakableLabel : Tweakable
{
	public ReactiveProperty<string> Text = new ReactiveProperty<string>();

	public TweakableLabel(string name, string text)
	{
		base.TweakableName = name;
		Text.Value = text;
		TweakableManagerUI.RegisterTweakable(this);
	}
}
