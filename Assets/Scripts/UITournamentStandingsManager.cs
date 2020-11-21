using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UITournamentStandingsManager : MonoBehaviour
{
	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private Text m_nextRank;

	[SerializeField]
	private Text m_nextDisplayName;

	[SerializeField]
	private Image m_nextRegion;

	[SerializeField]
	private Text m_nextScore;

	[SerializeField]
	private Text m_yourRank;

	[SerializeField]
	private Text m_yourDisplayName;

	[SerializeField]
	private Text m_yourScore;

	[SerializeField]
	private Text m_prevRank;

	[SerializeField]
	private Text m_prevDisplayName;

	[SerializeField]
	private Image m_prevRegion;

	[SerializeField]
	private Text m_prevScore;

	private LeaderboardEntry[] m_closePlayers = new LeaderboardEntry[5];

	private LeaderboardEntry m_playerEntry;

	private void Start()
	{
		(from rank in Singleton<TournamentRunner>.Instance.PlayerRank.DelayFrame(1)
			where rank >= 0
			select rank).Subscribe(delegate(int rank)
		{
			UpdateClosePlayers(rank);
		}).AddTo(this);
		(from rank in Singleton<TournamentRunner>.Instance.PlayerRank.DelayFrame(2).Pairwise()
			where rank.Current < rank.Previous
			select rank).Subscribe(delegate(Pair<int> rank)
		{
			ShowStandingsAnimation(rank);
		}).AddTo(this);
	}

	private void UpdateClosePlayers(int rank)
	{
		List<LeaderboardEntry> value = Singleton<LeaderboardRunner>.Instance.TournamentLeaderboard.Value;
		if (value == null || value.Count < 3)
		{
			return;
		}
		if (m_closePlayers[0] != null)
		{
			m_closePlayers[2] = CopyLeaderboardEntryToStaticOne(m_closePlayers[0]);
		}
		if (m_closePlayers[1] != null)
		{
			m_closePlayers[3] = CopyLeaderboardEntryToStaticOne(m_closePlayers[1]);
		}
		if (rank <= value.Count)
		{
			if (rank > 1)
			{
				m_closePlayers[0] = value[rank - 2];
			}
			if (rank < value.Count)
			{
				m_closePlayers[1] = value[rank];
			}
			if (rank < value.Count - 1)
			{
				m_closePlayers[4] = CopyLeaderboardEntryToStaticOne(value[rank + 1]);
			}
			m_playerEntry = value[rank - 1];
		}
	}

	private LeaderboardEntry CopyLeaderboardEntryToStaticOne(LeaderboardEntry lbe)
	{
		LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.DisplayName = lbe.DisplayName;
		leaderboardEntry.PicturePath = lbe.PicturePath;
		leaderboardEntry.StatValue = lbe.StatValue;
		return lbe;
	}

	private void ShowStandingsAnimation(Pair<int> rank)
	{
		int num = 0;
		if (Singleton<LeaderboardRunner>.Instance.TournamentLeaderboard.Value != null)
		{
			num = Singleton<LeaderboardRunner>.Instance.TournamentLeaderboard.Value.Count;
		}
		if (num < 3)
		{
			return;
		}
		if (rank.Current == rank.Previous - 1)
		{
			if (rank.Current < num - 1 && m_closePlayers[4] != null)
			{
				SetYourValues(m_playerEntry, rank.Current);
				SetNextPlayersValues(m_closePlayers[1], rank.Current + 1);
				SetPreviousPlayersValues(m_closePlayers[4], rank.Current + 2);
				m_animator.SetTrigger("MoveOneUp");
				AudioController.Instance.QueueEvent(new AudioEvent("TournamentRankUpOne", AUDIOEVENTACTION.Play));
			}
			else
			{
				SetYourValues(m_playerEntry, rank.Current);
				SetNextPlayersValues(m_closePlayers[1], rank.Current + 1);
				m_animator.SetTrigger("MoveOneUpLastPos");
				AudioController.Instance.QueueEvent(new AudioEvent("TournamentRankUpOne", AUDIOEVENTACTION.Play));
			}
		}
		else if (rank.Current > 1 && rank.Previous < num && m_closePlayers[3] != null)
		{
			SetYourValues(m_playerEntry, rank.Current);
			SetNextPlayersValues(m_closePlayers[2], rank.Previous + 1);
			SetPreviousPlayersValues(m_closePlayers[3], rank.Previous + 2);
			m_animator.SetTrigger("MoveMultipleUp");
			StartCoroutine(ChangeSetPlayerValues(m_closePlayers[0], m_closePlayers[1], rank.Current));
			AudioController.Instance.QueueEvent(new AudioEvent("TournamentRankUpMultiple", AUDIOEVENTACTION.Play));
		}
		else if (rank.Current > 1 && rank.Previous == num)
		{
			SetYourValues(m_playerEntry, rank.Current);
			SetNextPlayersValues(m_closePlayers[2], rank.Previous + 1);
			m_animator.SetTrigger("MoveMultipleUpLastPos");
			StartCoroutine(ChangeSetPlayerValues(m_closePlayers[0], m_closePlayers[1], rank.Current));
			AudioController.Instance.QueueEvent(new AudioEvent("TournamentRankUpMultiple", AUDIOEVENTACTION.Play));
		}
		else if (rank.Current == 1 && rank.Previous < num && m_closePlayers[3] != null)
		{
			SetYourValues(m_playerEntry, rank.Current);
			SetNextPlayersValues(m_closePlayers[2], rank.Previous + 1);
			SetPreviousPlayersValues(m_closePlayers[3], rank.Previous + 2);
			m_animator.SetTrigger("MoveMultipleUpFirstPos");
			StartCoroutine(ChangeSetPlayerValues(m_closePlayers[1], m_closePlayers[1], rank.Current));
			AudioController.Instance.QueueEvent(new AudioEvent("TournamentRankUpMultiple", AUDIOEVENTACTION.Play));
		}
	}

	private void SetYourValues(LeaderboardEntry lbe, int rank)
	{
		m_yourRank.text = rank.ToString();
		if (lbe != null)
		{
			m_yourDisplayName.text = lbe.DisplayName.Value;
			m_yourScore.text = lbe.StatValue.Value.ToString();
		}
	}

	private void SetNextPlayersValues(LeaderboardEntry lbe, int rank)
	{
		m_nextRank.text = rank.ToString();
		if (lbe != null)
		{
			m_nextDisplayName.text = lbe.DisplayName.Value;
			m_nextRegion.sprite = Resources.Load<Sprite>("Sprites/" + lbe.PicturePath.Value);
			m_nextScore.text = lbe.StatValue.Value.ToString();
		}
	}

	private void SetPreviousPlayersValues(LeaderboardEntry lbe, int rank)
	{
		m_prevRank.text = rank.ToString();
		if (lbe != null)
		{
			m_prevDisplayName.text = lbe.DisplayName.Value;
			m_prevRegion.sprite = Resources.Load<Sprite>("Sprites/" + lbe.PicturePath.Value);
			m_prevScore.text = lbe.StatValue.Value.ToString();
		}
	}

	private IEnumerator ChangeSetPlayerValues(LeaderboardEntry next, LeaderboardEntry prev, int rank)
	{
		yield return new WaitForSeconds(1.5f);
		SetNextPlayersValues(next, rank - 1);
		SetPreviousPlayersValues(prev, rank + 1);
	}
}
