using System.Collections.Generic;

public class MyFacebookData
{
	public FBPlayer Player
	{
		get;
		private set;
	}

	public List<object> Friends
	{
		get;
		private set;
	}

	public MyFacebookData(FBPlayer player, List<object> friends)
	{
		Player = player;
		Friends = friends;
	}
}
