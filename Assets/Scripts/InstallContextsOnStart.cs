using UnityEngine;

public class InstallContextsOnStart : MonoBehaviour
{
	private void Start()
	{
		Singleton<PropertyManager>.Instance.InstallContexts(base.transform, null);
	}
}
