public class EnabledOnSafeAreaDevice : SafeAreaSpecificBehaviour
{
	protected override void Setup(bool isEnabled)
	{
		base.gameObject.SetActive(isEnabled);
	}
}
