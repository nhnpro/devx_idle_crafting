using UnityEngine;

public class AndroidQuitButton : MonoBehaviour
{
	public void QuitApplicationFromAndroidQuitMenu()
	{
		PersistentSingleton<MainSaver>.Instance.QuitApplicationFromAndroidQuitMenu();
	}
}
