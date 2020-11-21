using UnityEngine;

public static class InGameProfiler
{
	private static ProfileStruct[] m_profiles = new ProfileStruct[2];

	private static ProfileStamp[] m_stamps = new ProfileStamp[1024];

	private static int m_stackCounter = 0;

	public static void BeginSample(ProfileEnum profile)
	{
		m_stamps[m_stackCounter].Profile = profile;
		m_stamps[m_stackCounter].Stamp = Time.realtimeSinceStartup;
		m_stackCounter++;
	}

	public static void EndSample(ProfileEnum profile)
	{
		m_stackCounter--;
		float num = Time.realtimeSinceStartup - m_stamps[m_stackCounter].Stamp;
		m_profiles[(int)profile].Time += num;
		m_profiles[(int)profile].Count++;
	}

	public static void Reset()
	{
		m_stackCounter = 0;
		for (int i = 0; i < 2; i++)
		{
			m_profiles[i].Count = 0;
			m_profiles[i].Time = 0f;
		}
	}

	public static void DumpLog()
	{
		for (int i = 0; i < 2; i++)
		{
			if (m_profiles[i].Count != 0)
			{
			}
		}
	}
}
