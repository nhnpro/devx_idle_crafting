using System.Collections.Generic;
using UnityEngine;

public abstract class PropertyContext : MonoBehaviour
{
	private Dictionary<string, object> m_providers;

	public abstract void Install(Dictionary<string, object> parameters);

	public void PerformInstall(Dictionary<string, object> parameters)
	{
		m_providers = new Dictionary<string, object>();
		Install(parameters);
	}

	protected void Add(string name, object obj)
	{
		m_providers.Add(name, obj);
	}

	public object GetProvider(string name)
	{
		object value = null;
		m_providers.TryGetValue(name, out value);
		return value;
	}
}
