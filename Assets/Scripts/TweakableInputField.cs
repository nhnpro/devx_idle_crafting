using UniRx;

public class TweakableInputField : Tweakable
{
	public ReactiveProperty<string> Input = new ReactiveProperty<string>();

	public TweakableInputField(string name, string text)
	{
		base.TweakableName = name;
		Input.Value = text;
		TweakableManagerUI.RegisterTweakable(this);
	}
}
