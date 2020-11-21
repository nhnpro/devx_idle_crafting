using UnityEngine;

public class FBPlayer
{
	public string Id
	{
		get;
		private set;
	}

	public bool Playing
	{
		get;
		set;
	}

	public bool Invited
	{
		get;
		set;
	}

	public string Name
	{
		get;
		private set;
	}

	public int Highscore
	{
		get;
		set;
	}

	public Texture2D ProfilePicture
	{
		get;
		set;
	}

	public long GiftTimeStamp
	{
		get;
		private set;
	}

	public FBPlayer(string id, string name, bool playing, bool invited, int highscore, long gitTimeStamp)
	{
		Id = id;
		Name = name;
		Playing = playing;
		Invited = invited;
		Highscore = highscore;
		GiftTimeStamp = gitTimeStamp;
	}

	public void SetGiftTimeStamp(long ticks)
	{
		GiftTimeStamp = ticks;
	}
}
