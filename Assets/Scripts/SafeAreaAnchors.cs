using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaAnchors : SafeAreaSpecificBehaviour
{
	public Vector2 MinInset;

	public Vector2 MaxInset;

	private Vector2 _minInset;

	private Vector2 _maxInset;

	protected override void SetupOnce(bool isEnabled)
	{
		RectTransform rectTransform = (RectTransform)base.transform;
		_minInset = rectTransform.anchorMin;
		_maxInset = rectTransform.anchorMax;
	}

	protected override void Setup(bool isEnabled)
	{
		RectTransform rectTransform = (RectTransform)base.transform;
		Vector2 anchorMin = (!isEnabled) ? _minInset : MinInset;
		Vector2 anchorMax = (!isEnabled) ? _maxInset : MaxInset;
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
	}
}
