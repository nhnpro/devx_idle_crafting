using UnityEngine;

public abstract class SafeAreaSpecificBehaviour : MonoBehaviour
{
	public bool EnabledOnDevice = true;

	private void Awake()
	{
		bool isEnabled = EnabledOnDevice && isSafeAreaDevice();
		SetupOnce(isEnabled);
		Setup(isEnabled);
	}

	private bool isSafeAreaDevice()
	{
		return new Rect(0f, 0f, Screen.width, Screen.height) != Screen.safeArea;
	}

	protected virtual void SetupOnce(bool isEnabled)
	{
	}

	protected abstract void Setup(bool isEnabled);
}
