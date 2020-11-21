using System;

public class FrequencyCap
{
	public TimeSpan coolDown;

	public long capSize;

	public FrequencyCap(TimeSpan _coolDown, long _capSize)
	{
		coolDown = _coolDown;
		capSize = _capSize;
	}
}
