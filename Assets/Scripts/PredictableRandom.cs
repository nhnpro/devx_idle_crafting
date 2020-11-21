using UnityEngine;

public class PredictableRandom
{
	private static uint m_z = 13u;

	private static uint m_w = 131u;

	public static void SetSeed(uint z, uint w)
	{
		m_z = z + 1;
		m_w = w + 1;
	}

	public static uint GetNextUint()
	{
		m_z = 36969 * (m_z & 0xFFFF) + (m_z >> 16);
		m_w = 18000 * (m_w & 0xFFFF) + (m_w >> 16);
		return (m_z << 16) + m_w;
	}

	public static float GetNextFloat01()
	{
		uint nextUint = GetNextUint();
		return ((float)(double)nextUint + 1f) * 2.32830644E-10f;
	}

	public static float GetNextRangeFloat(float min, float max)
	{
		return GetNextFloat01() * (max - min) + min;
	}

	public static int GetNextRangeInt(int min, int maxExclusive)
	{
		if (min >= maxExclusive)
		{
			return min;
		}
		float num = GetNextFloat01() * (float)(maxExclusive - min) + (float)min;
		return Mathf.Clamp((int)num, min, maxExclusive - 1);
	}

	public static int GetNextRangeIntTryAvoidDuplicate(int min, int maxExclusive, int prev)
	{
		int nextRangeInt = GetNextRangeInt(min, maxExclusive);
		if (nextRangeInt != prev)
		{
			return nextRangeInt;
		}
		return GetNextRangeInt(min, maxExclusive);
	}
}
