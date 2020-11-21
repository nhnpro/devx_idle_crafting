using UnityEngine;
using UnityEngine.UI;

public class Touchable : Graphic, ICanvasRaycastFilter
{
	public new void OnEnable()
	{
		base.OnEnable();
		base.color = Color.clear;
	}

	public virtual bool IsInteractable()
	{
		return true;
	}

	public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		return true;
	}
}
