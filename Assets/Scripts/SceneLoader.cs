using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	private static bool m_loaded;

	public static string InitialScene = string.Empty;

	public static SceneLoader Instance
	{
		get;
		private set;
	}

	protected void Awake()
	{
		Instance = this;
		m_loaded = (m_loaded || SceneManager.GetActiveScene().name == "Initialize");
		if (!m_loaded)
		{
			InitialScene = SceneManager.GetActiveScene().name;
			SceneLoadHelper.LoadInitSceneNow();
			return;
		}
		if (SceneManager.GetActiveScene().name == "Main")
		{
			MainInstaller.ReleaseAll();
			MainInstaller.DoAwake();
		}
		if (SceneManager.GetActiveScene().name == "TimeMachine")
		{
			GamblingInstaller.ReleaseAll();
			GamblingInstaller.DoAwake();
		}
	}

	protected void Start()
	{
		if (SceneManager.GetActiveScene().name == "Main")
		{
			MainInstaller.DoStart();
			Observable.NextFrame().Take(1).Subscribe(delegate
			{
				MainInstaller.DoSceneStarted();
			})
				.AddTo(this);
		}
		if (SceneManager.GetActiveScene().name == "TimeMachine")
		{
			GamblingInstaller.DoStart();
		}
	}

	protected void OnDestroy()
	{
		Instance = null;
	}
}
