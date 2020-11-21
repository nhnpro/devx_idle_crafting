using UnityEngine;

public class UIAdjustEditorWorld : MonoBehaviour
{
	public void OnScale()
	{
		float value = ARBindingManager.Instance.ScaleSlider.value;
		ARBindingManager.Instance.World.transform.localScale = new Vector3(value, value, value);
	}

	public void OnRotate()
	{
		ARBindingManager.Instance.World.transform.rotation = Quaternion.identity;
		ARBindingManager.Instance.World.transform.Rotate(new Vector3(0f, ARBindingManager.Instance.RotationSlider.value * 360f, 0f));
	}
}
