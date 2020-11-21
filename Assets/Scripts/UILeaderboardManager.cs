using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UILeaderboardManager : MonoBehaviour
{
	private enum LeaderboardEnum
	{
		Friends,
		Tournament
	}

	[SerializeField]
	private LeaderboardEnum m_lbenum;

	private List<LeaderboardEntry> m_lb = new List<LeaderboardEntry>();

	protected void Start()
	{
		if (m_lbenum == LeaderboardEnum.Friends)
		{
			(from upd in Singleton<FacebookRunner>.Instance.UpdateLeaderboards
				where upd
				select upd).CombineLatest(Singleton<LeaderboardRunner>.Instance.FriendLeaderboard, (bool upd, List<LeaderboardEntry> lb) => lb).Subscribe(delegate(List<LeaderboardEntry> lb)
			{
				m_lb = lb;
				SetupLeaderboard();
			}).AddTo(this);
		}
		else if (m_lbenum == LeaderboardEnum.Tournament)
		{
			Singleton<LeaderboardRunner>.Instance.TournamentLeaderboard.Subscribe(delegate(List<LeaderboardEntry> lb)
			{
				m_lb = lb;
				SetupTournamentLeaderboard();
			}).AddTo(this);
		}
	}

	public void SetupLeaderboard()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string str = "UI/LeaderboardItems/";
		string str2 = "_ListItem";
		base.transform.DestroyChildrenImmediate();
		foreach (LeaderboardEntry item in m_lb)
		{
			if (PlayerData.Instance.PlayFabToFBIds.HasValue && PlayerData.Instance.PlayFabToFBIds.Value.ContainsKey(item.PlayerId.Value))
			{
				item.PicturePath.Value = PlayerData.Instance.PlayFabToFBIds.Value[item.PlayerId.Value] + ".png";
			}
			else if (item.PlayerId.Value == PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value)
			{
				item.PicturePath.Value = PersistentSingleton<PlayFabService>.Instance.LinkedFacebookId.Value + ".png";
			}
			else
			{
				item.PicturePath.Value = null;
			}
			dictionary.Clear();
			dictionary.Add("LeaderboardEntry", item);
			string str3 = "Normal";
			if (item.Position.Value == 1)
			{
				str3 = "First";
			}
			else if (item.Position.Value == 2)
			{
				str3 = "Second";
			}
			else if (item.Position.Value == 3)
			{
				str3 = "Third";
			}
			else if (item.PlayerId.Value == PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value)
			{
				str3 = "Player";
			}
			GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources(str + str3 + str2, Vector3.zero, Quaternion.identity, dictionary);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}
	}

	public void SetupTournamentLeaderboard()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string str = "UI/LeaderboardItems/";
		string str2 = "TournamentLeaderboardItem";
		base.transform.DestroyChildrenImmediate();
		if (m_lb != null)
		{
			foreach (LeaderboardEntry item in m_lb)
			{
				dictionary.Clear();
				dictionary.Add("LeaderboardEntry", item);
				string str3 = string.Empty;
				if (item.Position.Value == 1)
				{
					str3 = "1st";
				}
				else if (item.Position.Value == 2)
				{
					str3 = "2nd";
				}
				else if (item.Position.Value == 3)
				{
					str3 = "3rd";
				}
				if (item.PlayerId.Value == PlayerData.Instance.PFId.Value)
				{
					str3 = "You";
				}
				GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources(str + str2 + str3, Vector3.zero, Quaternion.identity, dictionary);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}
}
