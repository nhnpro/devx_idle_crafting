using UniRx;
using UnityEngine;

public class MainSaver : PersistentSingleton<MainSaver>
{
	public Subject<bool> Saving = new Subject<bool>();

	private long m_lastCallTime;

	public void StartUp()
	{
		if (!base.Inited)
		{
			(from p in Observable.EveryApplicationPause()
				where p
				select p).Subscribe(delegate
			{
				PleaseSave("app_pause", cloud: false);
			});
			base.Inited = true;
		}
	}

	public void QuitApplicationFromAndroidQuitMenu()
	{
		PleaseSave("app_quit", cloud: false);
		Application.Quit();
	}

	public void PleaseSave(string reason, bool cloud = true)
	{
		Saving.OnNext(value: true);
		PersistentSingleton<SaveLoad>.Instance.Save(PlayerData.Instance, reason);
		if (ServerTimeService.NowTicks() - m_lastCallTime >= 20000000)
		{
			m_lastCallTime = ServerTimeService.NowTicks();
			if (Singleton<CloudSyncRunner>.Instance != null)
			{
				Singleton<CloudSyncRunner>.Instance.CloudSync(reason);
			}
		}
	}
}
