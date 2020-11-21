using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct AnimSpeed
{
	public float Min
	{
		get;
		private set;
	}

	public float Max
	{
		get;
		private set;
	}

	public AnimSpeed(float min, float max)
	{
		Min = 1f;
		Max = 1f;
	}

	public void Set(float min, float max)
	{
		Min = min;
		Max = max;
	}
}
