using System.Collections.Generic;
using UniRx;

[PropertyClass]
public class LeaderboardEntry
{
	[PropertyString]
	public ReactiveProperty<string> PlayerId;

	[PropertyString]
	public ReactiveProperty<string> DisplayName;

	[PropertyInt]
	public ReactiveProperty<int> StatValue;

	[PropertyInt]
	public ReactiveProperty<int> Position;

	public ReactiveProperty<List<string>> Tags;

	[PropertyString]
	public ReactiveProperty<string> PicturePath = new ReactiveProperty<string>();
}
