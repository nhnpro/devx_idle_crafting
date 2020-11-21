using Big;
using UnityEngine;

public static class TextSlerpHelper
{
	public static void SlerpTextCrit(string text, Vector3 pos)
	{
		FloatingText pooledComponent = BindingManager.Instance.CritTextsPool.GetPooledComponent<FloatingText>();
		Vector3 position = BindingManager.Instance.UICamera.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pos));
		pooledComponent.transform.position = position;
		pooledComponent.FloatText(text);
	}

	public static void SlerpText(string text, Vector3 pos)
	{
		FloatingText pooledComponent = BindingManager.Instance.PlusTextsPool.GetPooledComponent<FloatingText>();
		Vector3 position = BindingManager.Instance.UICamera.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pos));
		pooledComponent.transform.position = position;
		pooledComponent.FloatText(text);
	}

	public static void SlerpTextBigText(BigDouble reward, Vector3 pos)
	{
		SlerpText("+" + BigString.ToString(reward), pos);
	}

	public static void SlerpUIText(string text, Vector3 pos)
	{
		FloatingText pooledComponent = BindingManager.Instance.UITextsPool.GetPooledComponent<FloatingText>();
		pooledComponent.transform.position = pos;
		pooledComponent.FloatText(text);
	}

	public static void SlerpUITextBigText(BigDouble reward, Vector3 pos)
	{
		SlerpUIText("+" + BigString.ToString(reward), pos);
	}
}
