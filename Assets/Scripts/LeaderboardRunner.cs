using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class LeaderboardRunner : Singleton<LeaderboardRunner>
{
	public const int RefreshInterval = 300;

	public const float ThrottleSeconds = 30f;

	public const string LeaderboardName = "Highscore";

	public const int NumEntries = 100;

	public ReactiveProperty<List<LeaderboardEntry>> FriendLeaderboard = Observable.Never<List<LeaderboardEntry>>().ToReactiveProperty();

	public ReactiveProperty<bool> LeaderboardUpdateTrigger = Observable.Never<bool>().ToReactiveProperty();

	public ReactiveProperty<List<LeaderboardEntry>> TournamentLeaderboard = new ReactiveProperty<List<LeaderboardEntry>>();

	private float m_lastFetch = -30f;

	public LeaderboardRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		FriendLeaderboard.Subscribe(delegate(List<LeaderboardEntry> fs)
		{
			foreach (LeaderboardEntry f in fs)
			{
				if (PlayerData.Instance.PlayFabToFBIds.HasValue && PlayerData.Instance.PlayFabToFBIds.Value.ContainsKey(f.PlayerId.Value) && PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(PlayerData.Instance.PlayFabToFBIds.Value[f.PlayerId.Value]))
				{
					PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[PlayerData.Instance.PlayFabToFBIds.Value[f.PlayerId.Value]].Highscore = f.StatValue.Value;
				}
			}
		}).AddTo(instance);
		(from _ in PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId
			select true).Merge(LeaderboardUpdateTrigger).Subscribe(delegate
		{
			TryUpdateLeaderboards();
		}).AddTo(instance);
		LeaderboardUpdateTrigger.Subscribe(delegate
		{
			TryUpdateLeaderboards();
		}).AddTo(instance);
		instance.StartCoroutine(RefreshRoutine());
	}

	private IEnumerator RefreshRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(300f);
			TryUpdateLeaderboards();
		}
	}

	private void TryUpdateLeaderboards()
	{
		if (!string.IsNullOrEmpty(PersistentSingleton<PlayFabService>.Instance.LinkedFacebookId.Value) && ConnectivityService.InternetConnectionAvailable.Value && !(m_lastFetch + 30f > Time.realtimeSinceStartup))
		{
			m_lastFetch = Time.realtimeSinceStartup;
			PersistentSingleton<PlayFabService>.Instance.GetFriendLeaderboard("Highscore", delegate(Leaderboard lb)
			{
				FriendLeaderboard.Value = lb.Entries;
			}, delegate
			{
			}, 100);
		}
	}
}
