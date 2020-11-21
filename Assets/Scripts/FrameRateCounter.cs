using UnityEngine;

public class FrameRateCounter : MonoBehaviour
{
	private const int Size = 64;

	private const int Mask = 63;

	private float[] m_buffer = new float[64];

	private int m_counter;

	public static float HighFps
	{
		get;
		private set;
	}

	public static float LowFps
	{
		get;
		private set;
	}

	public static float AvgFps
	{
		get;
		private set;
	}

	protected void Update()
	{
		m_counter++;
		m_counter &= 63;
		m_buffer[m_counter] = Time.deltaTime;
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = 0f;
		for (int i = 0; i < 64; i++)
		{
			num = Mathf.Min(m_buffer[i], num);
			num2 = Mathf.Max(m_buffer[i], num2);
			num3 += m_buffer[i];
		}
		HighFps = 1f / Mathf.Max(num, 0.01f);
		LowFps = 1f / Mathf.Max(num2, 0.01f);
		AvgFps = 64f / num3;
	}
}
