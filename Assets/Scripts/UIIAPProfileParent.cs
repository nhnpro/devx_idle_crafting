using System;
using System.Collections;
using UnityEngine;

public class UIIAPProfileParent : MonoBehaviour
{
	protected void Start()
	{
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		Debug.LogError($"GetPrefabPath {GetPrefabPath()}");
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		IAPItemRunner iAPItemRunner = (IAPItemRunner)Singleton<PropertyManager>.Instance.GetContext("IAPItemRunner", base.transform);
		return "UI/IAPItemProfiles/IAPItem." + iAPItemRunner.ProductID;
	}
}
