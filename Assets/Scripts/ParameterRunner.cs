using Big;
using UniRx;

[PropertyClass]
public class ParameterRunner
{
	[PropertyInt]
	public ReactiveProperty<int> IntValue = new ReactiveProperty<int>();

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> BigDoubleValue = new ReactiveProperty<BigDouble>();

	[PropertyString]
	public ReactiveProperty<string> StringValue = new ReactiveProperty<string>();

	[PropertyBool]
	public ReactiveProperty<bool> BoolValue = new ReactiveProperty<bool>();
}
