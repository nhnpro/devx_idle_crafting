using UnityEngine;
using UnityEngine.UI;

public class UIToggleSpriteSwap : MonoBehaviour
{
	public Toggle targetToggle;

	public Sprite selectedSprite;

	protected void Start()
	{
		targetToggle.toggleTransition = Toggle.ToggleTransition.None;
		targetToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);
		OnTargetToggleValueChanged(targetToggle.isOn);
	}

	private void OnTargetToggleValueChanged(bool newValue)
	{
		Image image = targetToggle.targetGraphic as Image;
		if (image != null)
		{
			if (newValue)
			{
				image.overrideSprite = selectedSprite;
			}
			else
			{
				image.overrideSprite = null;
			}
		}
	}
}
