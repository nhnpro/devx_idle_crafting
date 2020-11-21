using System.Collections.Generic;

public static class SingletonManager
{
	private static List<IReleasable> m_release = new List<IReleasable>();

	public static void Register(IReleasable rel)
	{
		m_release.Add(rel);
	}

	public static void ReleaseAll()
	{
		for (int num = m_release.Count - 1; num >= 0; num--)
		{
			m_release[num].Release();
		}
		m_release.Clear();
	}
}
