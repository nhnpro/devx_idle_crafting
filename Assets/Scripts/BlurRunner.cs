public class BlurRunner : Singleton<BlurRunner>
{
	public void SetBlur(bool enable)
	{
		BindingManager.Instance.Blur.enabled = enable;
	}
}
