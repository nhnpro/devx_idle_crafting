using System;

public class Message
{
	public TimeSpan Delay
	{
		get;
		private set;
	}

	public string Text
	{
		get;
		private set;
	}

	public Message(TimeSpan delay, string text)
	{
		Delay = delay;
		Text = text;
	}
}
