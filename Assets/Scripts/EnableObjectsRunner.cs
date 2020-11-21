using UniRx;

[PropertyClass]
public class EnableObjectsRunner : Singleton<EnableObjectsRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> SmallCurrencyHeader = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> DelayedCurrencyHeader = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> Popup = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> MapCloseButton = new ReactiveProperty<bool>(initialValue: true);

	[PropertyBool]
	public ReactiveProperty<bool> GameView = new ReactiveProperty<bool>(initialValue: true);

	public EnableObjectsRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
	}
}
