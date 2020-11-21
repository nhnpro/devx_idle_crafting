using Big;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;

[PropertyClass]
public class TournamentRunner : Singleton<TournamentRunner>
{
	public const string TOURNAMENT_FILE = "/saveHelp.idl";

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> LoggedInPlayfab;

	public ReadOnlyReactiveProperty<long> TimeTillTournament;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> TimeTillTournamentClock;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> TournamentAccessable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CurrentlyInTournament;

	public ReactiveProperty<bool> TournamentFetched = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> TournamentActive = new ReactiveProperty<bool>(initialValue: false);

	public ReadOnlyReactiveProperty<long> TimeTillTournamentEnd;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> TournamentEndClock;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> TournamentEnded;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> TournamentWorldReached;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DisplayNameGiven;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> TournamentsUnlocked;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> InternetConnectionAvailable;

	[PropertyInt]
	public ReactiveProperty<int> PlayerRank = new ReactiveProperty<int>(-1);

	[PropertyInt]
	public ReactiveProperty<int> PlayerHighestWorld = new ReactiveProperty<int>(0);

	public List<string> PFIDsUsed = new List<string>();

	public ReactiveProperty<List<TournamentRunStruct>> TournamentRuns = new ReactiveProperty<List<TournamentRunStruct>>();

	private ReactiveProperty<int> FetchAttempt = new ReactiveProperty<int>(0);

	private ReactiveProperty<int> TotalDaysSinceStart = new ReactiveProperty<int>(0);

	public ReactiveProperty<BigDouble> TrophyHeroDamageMultiplier = new ReactiveProperty<BigDouble>(1L);

	public ReactiveProperty<BigDouble> TrophyCompanionDamageMultiplier = new ReactiveProperty<BigDouble>(1L);

	public ReactiveProperty<BigDouble> TrophyAllDamageMultiplier = new ReactiveProperty<BigDouble>(1L);

	private int m_closestTournamentDay = int.MaxValue;

	private List<string> m_countries = new List<string>
	{
		"ad",
		"ae",
		"af",
		"ag",
		"al",
		"am",
		"ao",
		"ar",
		"at",
		"au",
		"az",
		"ba",
		"bb",
		"bd",
		"be",
		"bf",
		"bg",
		"bh",
		"bi",
		"bj",
		"bn",
		"bo",
		"br",
		"bs",
		"bt",
		"bw",
		"by",
		"bz",
		"ca",
		"cd",
		"cf",
		"cg",
		"ch",
		"ci",
		"cl",
		"cm",
		"cn",
		"co",
		"cr",
		"cu",
		"cv",
		"cy",
		"cz",
		"de",
		"dj",
		"dk",
		"dm",
		"do",
		"dz",
		"ec",
		"ee",
		"eg",
		"eh",
		"er",
		"es",
		"et",
		"fi",
		"fj",
		"fm",
		"fr",
		"ga",
		"gb",
		"gd",
		"ge",
		"gh",
		"gm",
		"gn",
		"gq",
		"gr",
		"gt",
		"gw",
		"gy",
		"hn",
		"hr",
		"ht",
		"hu",
		"id",
		"ie",
		"il",
		"in",
		"iq",
		"ir",
		"is",
		"it",
		"jm",
		"jo",
		"ke",
		"kg",
		"kh",
		"ki",
		"km",
		"kn",
		"kp",
		"kr",
		"ks",
		"kw",
		"kz",
		"la",
		"lb",
		"lc",
		"li",
		"lk",
		"lr",
		"ls",
		"lt",
		"lu",
		"lv",
		"ly",
		"ma",
		"mc",
		"md",
		"me",
		"mg",
		"mh",
		"mk",
		"ml",
		"mm",
		"mn",
		"mr",
		"mt",
		"mu",
		"mv",
		"mw",
		"mx",
		"my",
		"mz",
		"na",
		"ne",
		"ng",
		"ni",
		"nl",
		"no",
		"np",
		"nr",
		"nz",
		"om",
		"pa",
		"pe",
		"pg",
		"ph",
		"pk",
		"pl",
		"pt",
		"pw",
		"py",
		"qa",
		"ro",
		"rs",
		"ru",
		"rw",
		"sa",
		"sb",
		"sc",
		"sd",
		"se",
		"sg",
		"si",
		"sk",
		"sl",
		"sm",
		"sn",
		"so",
		"sr",
		"st",
		"sv",
		"sy",
		"sz",
		"td",
		"tg",
		"th",
		"tj",
		"tl",
		"tm",
		"tn",
		"to",
		"tr",
		"tt",
		"tv",
		"tw",
		"tz",
		"ua",
		"ug",
		"us",
		"uy",
		"uz",
		"va",
		"vc",
		"ve",
		"vn",
		"vu",
		"ws",
		"ye",
		"za",
		"zm",
		"zw"
	};

	public TournamentRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		PFIDsUsed.Add("D1E872B9C9DA0648");
		LoggedInPlayfab = (from id in PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.StartWith(string.Empty)
			select id != string.Empty).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TimeTillTournament = TickerService.MasterTicksSlow.CombineLatest(ServerTimeService.IsSynced, (long tick, bool sync) => (!sync) ? (-1) : GetTimeTillTournament()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TimeTillTournamentClock = (from time in TimeTillTournament.DistinctUntilChanged()
			select (time <= 0) ? string.Empty : TextUtils.FormatSecondsShortWithDays(time)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from till in TimeTillTournament
			where till == 0
			select till).Subscribe(delegate
		{
			ResetIfDifferentDevice();
		}).AddTo(instance);
		TournamentAccessable = TimeTillTournament.CombineLatest(PlayerData.Instance.TournamentIdCurrent, TournamentFetched, ConnectivityService.InternetConnectionAvailable, (long time, int id, bool fetched, bool connected) => connected && time == 0 && id < 0 && fetched).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CurrentlyInTournament = (from id in PlayerData.Instance.TournamentIdCurrent
			select id >= 0 && CheckIfSameDevice()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from should in TournamentFetched.CombineLatest(TimeTillTournament, FetchAttempt, LoggedInPlayfab, (bool fetched, long till, int attempt, bool loggedIn) => !fetched && attempt == 0 && till == 0)
			where should
			select should).Subscribe(delegate
		{
			TryToFetchTournament();
		}).AddTo(instance);
		(from timestamp in PlayerData.Instance.TournamentTimeStamp
			where timestamp + 10000000L * (long)PersistentSingleton<GameSettings>.Instance.TournamentDurationSeconds >= ServerTimeService.NowTicks() && ServerTimeService.NowTicks() >= timestamp && CheckIfSameDevice()
			select timestamp).Subscribe(delegate
		{
			TournamentRuns.SetValueAndForceNotify(LoadTournamentRuns());
			TournamentActive.SetValueAndForceNotify(value: true);
		}).AddTo(instance);
		TimeTillTournamentEnd = (from should in TickerService.MasterTicks.CombineLatest(ServerTimeService.IsSynced, PlayerData.Instance.TournamentIdCurrent, (long tick, bool sync, int id) => sync && id >= 0 && CheckIfSameDevice())
			where should
			select should into _
			select GetTimeTillTournamentEnd()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TournamentEndClock = (from time in TimeTillTournamentEnd.DistinctUntilChanged()
			select (time <= 0) ? string.Empty : TextUtils.FormatSecondsShort(time)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TournamentEnded = TimeTillTournamentEnd.CombineLatest(PlayerData.Instance.TournamentIdCurrent, (long time, int id) => time < 0 && id >= 0).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from ended in TournamentEnded
			where ended
			select ended).Subscribe(delegate
		{
			LoadTournamentEndedValues();
		}).AddTo(instance);
		TournamentWorldReached = (from world in PlayerData.Instance.MainChunk
			select world >= 70).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DisplayNameGiven = (from name in PlayerData.Instance.DisplayName
			select name != string.Empty).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TournamentsUnlocked = PlayerData.Instance.LifetimePrestiges.CombineLatest(PlayerData.Instance.LifetimeChunk, (int prest, int chunk) => prest > 0 || chunk >= 45).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from unlocked in TournamentsUnlocked.Pairwise()
			where !unlocked.Previous && unlocked.Current
			select unlocked).Subscribe(delegate
		{
			BindingManager.Instance.TournamentUnlockedParent.ShowInfo();
		}).AddTo(instance);
		PlayerData.Instance.Medals.Take(1).Subscribe(delegate
		{
			TryToBuyTrophy(showUnlocking: false);
		}).AddTo(instance);
		PlayerData.Instance.Trophies.Subscribe(delegate(int trophies)
		{
			CalculateTrophiesMultiplier(trophies);
		}).AddTo(instance);
		InternetConnectionAvailable = (from avail in ConnectivityService.InternetConnectionAvailable
			select (avail)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from should in Singleton<LeaderboardRunner>.Instance.LeaderboardUpdateTrigger.CombineLatest(TournamentEnded, ConnectivityService.InternetConnectionAvailable, (bool lb, bool ended, bool connected) => !ended && connected)
			where should
			select should).Subscribe(delegate
		{
			UpdateTournamentRun();
		}).AddTo(instance);
		(from should in TournamentEnded.CombineLatest(ConnectivityService.InternetConnectionAvailable, PlayerData.Instance.MainChunk, TournamentActive, Singleton<LevelSkipRunner>.Instance.SkippingLevels, (bool ended, bool connected, int chunk, bool active, bool skipping) => !ended && connected && active && !skipping)
			where should
			select should).Subscribe(delegate
		{
			UpdateTournamentRun();
		}).AddTo(instance);
	}

	public void LoginToPlayfab()
	{
		if (PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value == null)
		{
			PersistentSingleton<PlayFabService>.Instance.Login();
		}
	}

	public void JoinTournament()
	{
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.TournamentEvent.SetValueAndForceNotify(value: true);
		}
		ResetTournamentValues();
		Singleton<PrestigeRunner>.Instance.StartPrestigeSequence();
		(from seq in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where seq == PrestigeOrder.PrestigePost
			select seq).Take(1).Subscribe(delegate
		{
			SetTournamentValues();
		}).AddTo(SceneLoader.Instance);
	}

	private void SetTournamentValues()
	{
		PlayerData.Instance.TournamentEnteringDevice.Value = PlayFabService.GetSystemUniqueIdentifier();
		PlayerData.Instance.TournamentIdCurrent.Value = 1;
		PlayerData.Instance.TournamentTimeStamp.Value = ServerTimeService.NowTicks() - TimeSpan.FromSeconds(UnityEngine.Random.Range(0, PersistentSingleton<GameSettings>.Instance.TournamentVariationSeconds)).Ticks;
	}

	public void SaveFakeRuns()
	{
		List<LeaderboardEntry> list = new List<LeaderboardEntry>();
		for (int i = 0; i < 99; i++)
		{
			short[] array = new short[90];
			for (int j = 0; j < 90; j++)
			{
				array[j] = (short)(0.0625 * (double)(i + 1) * (double)(j + 1));
			}
			string str = DataHelper.ConvertInt16ArrayToString(array);
			string str2 = RandomizeCountry();
			list.Add(new LeaderboardEntry
			{
				PlayerId = new ReactiveProperty<string>("TournamentParticipantId"),
				DisplayName = new ReactiveProperty<string>("TestUser_" + (i + 1)),
				StatValue = new ReactiveProperty<int>(0),
				Position = new ReactiveProperty<int>(0),
				Tags = new ReactiveProperty<List<string>>(new List<string>
				{
					"title.8E58.T1" + str,
					"title.8E58.CC" + str2
				})
			});
		}
		SaveTournamentRuns(list, 1);
		TryToFetchTournament();
	}

	public string RandomizeCountry()
	{
		return m_countries[UnityEngine.Random.Range(0, m_countries.Count)];
	}

	private long GetTimeTillTournament()
	{
		GetClosestTournamentDay();
		if (m_closestTournamentDay == 0)
		{
			return 0L;
		}
		DateTime d = ServerTimeService.UtcNow();
		DateTime d2 = ServerTimeService.UtcNow().Date.AddDays(m_closestTournamentDay);
		long val = (long)(d2 - d).TotalSeconds;
		return Math.Max(0L, val);
	}

	private void GetClosestTournamentDay()
	{
		List<DayOfWeek> list = new List<DayOfWeek>();
		for (int i = 0; i < PersistentSingleton<GameSettings>.Instance.TournamentDays.Count; i++)
		{
			if (Enum.IsDefined(typeof(DayOfWeek), PersistentSingleton<GameSettings>.Instance.TournamentDays[i]))
			{
				list.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), PersistentSingleton<GameSettings>.Instance.TournamentDays[i], ignoreCase: true));
			}
		}
		m_closestTournamentDay = int.MaxValue;
		int num = m_closestTournamentDay;
		for (int j = 0; j < list.Count; j++)
		{
			int num2 = list[j] - ServerTimeService.UtcNow().DayOfWeek + ((ServerTimeService.UtcNow().DayOfWeek > list[j]) ? 7 : 0);
			if (num2 < m_closestTournamentDay)
			{
				num = ((num != m_closestTournamentDay) ? m_closestTournamentDay : (num2 + 7));
				m_closestTournamentDay = num2;
			}
		}
		if (CheckIfDifferentDevice() && PlayerData.Instance.TournamentTimeStamp.Value + 10000000L * (long)PersistentSingleton<GameSettings>.Instance.TournamentDurationSeconds > ServerTimeService.NowTicks())
		{
			m_closestTournamentDay = num;
		}
	}

	private bool CheckIfDifferentDevice()
	{
		return PlayerData.Instance.TournamentEnteringDevice.Value != string.Empty && PlayerData.Instance.TournamentEnteringDevice.Value != PlayFabService.GetSystemUniqueIdentifier();
	}

	private bool CheckIfSameDevice()
	{
		return PlayerData.Instance.TournamentEnteringDevice.Value == PlayFabService.GetSystemUniqueIdentifier();
	}

	private void ResetIfDifferentDevice()
	{
		if (CheckIfDifferentDevice())
		{
			ResetTournamentValues();
		}
	}

	public void ResetTournamentValues()
	{
		TournamentActive.SetValueAndForceNotify(value: false);
		PlayerData.Instance.TournamentTimeStamp.Value = 0L;
		PlayerData.Instance.TournamentIdCurrent.Value = -1;
		PlayerData.Instance.TournamentLastPointOnline.Value = -1L;
		PlayerData.Instance.TournamentRun.Value = new short[90];
		PlayerData.Instance.TournamentEnteringDevice.Value = string.Empty;
		TournamentFetched.SetValueAndForceNotify(value: false);
		FetchAttempt.SetValueAndForceNotify(0);
	}

	private void TryToFetchTournament()
	{
		List<TournamentRunStruct> list = LoadTournamentRuns();
		if (TotalDaysSinceStart.Value == (int)TimeSpan.FromTicks(ServerTimeService.NowTicks()).TotalDays && list != null)
		{
			TournamentFetched.SetValueAndForceNotify(value: true);
		}
		else if (LoggedInPlayfab.Value)
		{
			FetchTournamentRuns();
		}
	}

	public void FetchTournamentRuns()
	{
		if (!string.IsNullOrEmpty(PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value) && ConnectivityService.InternetConnectionAvailable.Value)
		{
			FetchAttempt.Value++;
			PersistentSingleton<PlayFabService>.Instance.GetTournamentLeaderboard("T" + 1, delegate(Leaderboard lb)
			{
				SaveTournamentRuns(lb.Entries, 1);
				TournamentFetched.SetValueAndForceNotify(value: true);
			}, delegate
			{
				if (FetchAttempt.Value < 10)
				{
					Observable.Return<bool>(value: true).Delay(TimeSpan.FromSeconds(60.0)).Subscribe(delegate
					{
						FetchTournamentRuns();
					})
						.AddTo(SceneLoader.Instance);
				}
			}, 99);
		}
	}

	private void UpdateTournamentRun()
	{
		PlayerData.Instance.TournamentLastPointOnline.Value = ServerTimeService.NowTicks() - PlayerData.Instance.TournamentTimeStamp.Value;
		if (PlayerData.Instance.TournamentLastPointOnline.Value < PlayerData.Instance.TournamentTimeStamp.Value + 10000000L * (long)PersistentSingleton<GameSettings>.Instance.TournamentDurationSeconds)
		{
			short[] value = PlayerData.Instance.TournamentRun.Value;
			for (int i = GetNextPoint(TimeSpan.FromTicks(PlayerData.Instance.TournamentLastPointOnline.Value).TotalSeconds); i < value.Length; i++)
			{
				if ((short)PlayerData.Instance.MainChunk.Value > value[i])
				{
					value[i] = (short)PlayerData.Instance.MainChunk.Value;
				}
			}
			PlayerData.Instance.TournamentRun.Value = value;
		}
		UpdateTournamentLeaderboard();
	}

	private int GetNextPoint(double currentTime)
	{
		int result = PersistentSingleton<GameSettings>.Instance.SavePointsPerTournament - 1;
		int num = PersistentSingleton<GameSettings>.Instance.TournamentDurationSeconds / PersistentSingleton<GameSettings>.Instance.SavePointsPerTournament;
		for (int i = 1; i < PersistentSingleton<GameSettings>.Instance.SavePointsPerTournament; i++)
		{
			if ((double)(num * i) > currentTime)
			{
				result = i - 1;
				break;
			}
		}
		return result;
	}

	public void UpdateTournamentLeaderboard()
	{
		int nextPoint = GetNextPoint(TimeSpan.FromTicks(PlayerData.Instance.TournamentLastPointOnline.Value).TotalSeconds);
		int num = PersistentSingleton<GameSettings>.Instance.TournamentDurationSeconds / PersistentSingleton<GameSettings>.Instance.SavePointsPerTournament;
		List<TournamentRunStruct> value = TournamentRuns.Value;
		if (value == null)
		{
			return;
		}
		string value2 = PlayerData.Instance.PFId.Value;
		if (!PFIDsUsed.Contains(value2))
		{
			PFIDsUsed.Add(value2);
		}
		int i;
		for (i = 0; i < PFIDsUsed.Count; i++)
		{
			List<TournamentRunStruct> list = value.FindAll((TournamentRunStruct run) => run.PlayerId == PFIDsUsed[i]);
			for (int j = 0; j < list.Count; j++)
			{
				value.Remove(list[j]);
			}
		}
		for (int k = 0; k < value.Count; k++)
		{
			double totalSeconds = TimeSpan.FromTicks(PlayerData.Instance.TournamentLastPointOnline.Value).TotalSeconds;
			TournamentRunStruct tournamentRunStruct = value[k];
			double num2 = totalSeconds - (double)tournamentRunStruct.Variation;
			TournamentRunStruct value3 = value[k];
			if (num2 < 0.0)
			{
				value3.CurrentScore = -1;
			}
			else
			{
				int nextPoint2 = GetNextPoint(num2);
				int num3 = 0;
				if (nextPoint2 > 0)
				{
					num3 = value3.Run[nextPoint2 - 1];
				}
				int num4 = value3.Run[nextPoint2] - num3;
				num4 = (int)((double)num4 * Math.Min(1.0, (num2 - (double)(nextPoint2 * num)) / (double)num));
				value3.CurrentScore = (short)(num3 + num4);
			}
			value[k] = value3;
		}
		value.Add(new TournamentRunStruct
		{
			PlayerId = value2,
			DisplayName = PlayerData.Instance.DisplayName.Value,
			RegionTag = "FI",
			Variation = 0,
			Run = PlayerData.Instance.TournamentRun.Value,
			CurrentScore = PlayerData.Instance.TournamentRun.Value[nextPoint]
		});
		List<TournamentRunStruct> list2 = (from run in value
			orderby run.CurrentScore descending
			select run).ToList();
		List<LeaderboardEntry> list3 = new List<LeaderboardEntry>();
		int valueAndForceNotify = PlayerRank.Value;
		for (int l = 0; l < list2.Count; l++)
		{
			TournamentRunStruct tournamentRunStruct2 = list2[l];
			if (tournamentRunStruct2.CurrentScore >= 0)
			{
				TournamentRunStruct tournamentRunStruct3 = list2[l];
				if (tournamentRunStruct3.PlayerId == value2)
				{
					valueAndForceNotify = l + 1;
				}
				string initialValue = string.Empty;
				TournamentRunStruct tournamentRunStruct4 = list2[l];
				if (tournamentRunStruct4.RegionTag != null)
				{
					TournamentRunStruct tournamentRunStruct5 = list2[l];
					initialValue = "Flags/" + tournamentRunStruct5.RegionTag.ToLowerInvariant();
				}
				List<LeaderboardEntry> list4 = list3;
				LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
				LeaderboardEntry leaderboardEntry2 = leaderboardEntry;
				TournamentRunStruct tournamentRunStruct6 = list2[l];
				leaderboardEntry2.PlayerId = new ReactiveProperty<string>(tournamentRunStruct6.PlayerId);
				LeaderboardEntry leaderboardEntry3 = leaderboardEntry;
				TournamentRunStruct tournamentRunStruct7 = list2[l];
				leaderboardEntry3.DisplayName = new ReactiveProperty<string>(tournamentRunStruct7.DisplayName);
				LeaderboardEntry leaderboardEntry4 = leaderboardEntry;
				TournamentRunStruct tournamentRunStruct8 = list2[l];
				leaderboardEntry4.StatValue = new ReactiveProperty<int>(tournamentRunStruct8.CurrentScore);
				leaderboardEntry.Position = new ReactiveProperty<int>(l + 1);
				leaderboardEntry.PicturePath = new ReactiveProperty<string>(initialValue);
				list4.Add(leaderboardEntry);
			}
		}
		Singleton<LeaderboardRunner>.Instance.TournamentLeaderboard.SetValueAndForceNotify(list3);
		PlayerRank.SetValueAndForceNotify(valueAndForceNotify);
	}

	private long GetTimeTillTournamentEnd()
	{
		return (long)TimeSpan.FromTicks(PlayerData.Instance.TournamentTimeStamp.Value + 10000000L * (long)PersistentSingleton<GameSettings>.Instance.TournamentDurationSeconds - ServerTimeService.NowTicks()).TotalSeconds;
	}

	private void LoadTournamentEndedValues()
	{
		if (TournamentRuns.Value == null)
		{
			TournamentRuns.SetValueAndForceNotify(LoadTournamentRuns());
		}
		PlayerData.Instance.TournamentLastPointOnline.Value = ServerTimeService.NowTicks() - PlayerData.Instance.TournamentTimeStamp.Value;
		UpdateTournamentLeaderboard();
	}

	public void ClaimTournamentPrice()
	{
		int tournamentID = PlayerData.Instance.TournamentIdCurrent.Value;
		PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Take(1).Subscribe(delegate
		{
			UploadTournamentRun(tournamentID);
		}).AddTo(SceneLoader.Instance);
		PlayerHighestWorld.Value = PlayerData.Instance.TournamentRun.Value[PlayerData.Instance.TournamentRun.Value.Length - 1];
		BindingManager.Instance.TournamentRewardsClaimingParent.ShowInfo();
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.TournamentEvent.SetValueAndForceNotify(value: false);
		}
		TournamentConfig tournamentPriceBracket = Singleton<EconomyHelpers>.Instance.GetTournamentPriceBracket(PlayerRank.Value);
		for (int i = 0; i < tournamentPriceBracket.Rewards.Length; i++)
		{
			if (tournamentPriceBracket.Rewards[i] == null)
			{
				continue;
			}
			if (tournamentPriceBracket.Rewards[i].Type == RewardEnum.AddToBerries)
			{
				for (int j = 0; j < tournamentPriceBracket.Rewards[i].Amount; j++)
				{
					RewardFactory.ToRewardAction(new RewardData(RewardEnum.AddToBerries, 1L), RarityEnum.Common, Singleton<EconomyHelpers>.Instance.GetNumUnlockedHeroes(PlayerData.Instance.LifetimeChunk.Value)).GiveReward();
				}
			}
			else
			{
				RewardFactory.ToRewardAction(tournamentPriceBracket.Rewards[i], RarityEnum.Common).GiveReward();
			}
		}
		ResetTournamentValues();
	}

	public void TryToBuyTrophy(bool showUnlocking = true)
	{
		TournamentTierConfig tournamentTierConfig = Singleton<EconomyHelpers>.Instance.GetTournamentTierConfig(PlayerData.Instance.Trophies.Value + 1);
		if (tournamentTierConfig != null && tournamentTierConfig.Requirement <= PlayerData.Instance.Medals.Value)
		{
			PlayerData.Instance.Trophies.Value = (int)tournamentTierConfig.Tier;
			Singleton<FundRunner>.Instance.RemoveMedals(tournamentTierConfig.Requirement);
			if (showUnlocking)
			{
				BindingManager.Instance.TournamentTrophyUnlockedParent.ShowInfo();
			}
		}
	}

	private void CalculateTrophiesMultiplier(int tier)
	{
		BigDouble bigDouble = new BigDouble(1.0);
		BigDouble bigDouble2 = new BigDouble(1.0);
		BigDouble bigDouble3 = new BigDouble(1.0);
		for (int i = 1; i <= tier; i++)
		{
			TournamentTierConfig tournamentTierConfig = Singleton<EconomyHelpers>.Instance.GetTournamentTierConfig(i);
			Func<BigDouble, float, BigDouble> func = BonusTypeHelper.CreateBigDoubleFunction(tournamentTierConfig.Bonus.BonusType);
			switch (tournamentTierConfig.Bonus.BonusType)
			{
			case BonusTypeEnum.TapDamage:
				bigDouble = func(bigDouble, tournamentTierConfig.Bonus.Amount);
				break;
			case BonusTypeEnum.AllCompanionDamage:
				bigDouble2 = func(bigDouble2, tournamentTierConfig.Bonus.Amount);
				break;
			case BonusTypeEnum.AllDamage:
				bigDouble3 = func(bigDouble3, tournamentTierConfig.Bonus.Amount);
				break;
			}
		}
		TrophyHeroDamageMultiplier.Value = bigDouble;
		TrophyCompanionDamageMultiplier.Value = bigDouble2;
		TrophyAllDamageMultiplier.Value = bigDouble3;
	}

	public void UploadTournamentRun(int tournamentID)
	{
		short[] value = PlayerData.Instance.TournamentRun.Value;
		if (value[value.Length - 1] <= PersistentSingleton<GameSettings>.Instance.TournamentBiggestLegitValue)
		{
			string val = DataHelper.ConvertInt16ArrayToString(value);
			JSONObject jSONObject = JSONObject.Create();
			jSONObject.AddField("Tag", "T" + tournamentID);
			jSONObject.AddField("Data", val);
			Action<JSONObject> callback = delegate
			{
				int val2 = tournamentID;
				JSONObject jSONObject2 = new JSONObject(JSONObject.Type.OBJECT);
				jSONObject2.AddField("StatisticName", "T" + tournamentID);
				jSONObject2.AddField("Value", val2);
				PersistentSingleton<PlayFabService>.Instance.UpdatePlayerStatistic(jSONObject2, null, null);
			};
			PersistentSingleton<PlayFabService>.Instance.ExecuteCloudScript("updatePlayerTag", callback, null, jSONObject);
		}
	}

	public void SaveTournamentRuns(List<LeaderboardEntry> entries, int tournamentID)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		JSONObject jSONObject2 = new JSONObject(JSONObject.Type.OBJECT);
		jSONObject2.AddField("Date", (int)TimeSpan.FromTicks(ServerTimeService.NowTicks()).TotalDays);
		jSONObject.Add(jSONObject2);
		for (int i = 0; i < entries.Count; i++)
		{
			JSONObject jSONObject3 = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject3.AddField("PlayerId", entries[i].PlayerId.Value);
			jSONObject3.AddField("DisplayName", entries[i].DisplayName.Value);
			jSONObject3.AddField("Variation", UnityEngine.Random.Range(0, PersistentSingleton<GameSettings>.Instance.TournamentVariationSeconds));
			for (int j = 0; j < entries[i].Tags.Value.Count; j++)
			{
				string a = entries[i].Tags.Value[j].Substring(7 + PersistentSingleton<GameSettings>.Instance.PlayFabTitleId.Length, 2);
				if (a == "CC")
				{
					jSONObject3.AddField("Region", entries[i].Tags.Value[j].Substring(9 + PersistentSingleton<GameSettings>.Instance.PlayFabTitleId.Length, entries[i].Tags.Value[j].Length - (9 + PersistentSingleton<GameSettings>.Instance.PlayFabTitleId.Length)));
				}
				else if (a == "T" + tournamentID)
				{
					jSONObject3.AddField("Run", entries[i].Tags.Value[j].Substring(9 + PersistentSingleton<GameSettings>.Instance.PlayFabTitleId.Length, entries[i].Tags.Value[j].Length - (9 + PersistentSingleton<GameSettings>.Instance.PlayFabTitleId.Length)));
				}
			}
			jSONObject.Add(jSONObject3);
		}
		SaveTournamentRuns(jSONObject);
	}

	private void SaveTournamentRuns(JSONObject json)
	{
		try
		{
			File.WriteAllText(PersistentDataPath.Get() + "/saveHelp.idl", Encryptor.Encrypt(json.ToString()));
		}
		catch (IOException)
		{
			SaveLoad.ShowDiskFullDialog();
		}
	}

	private List<TournamentRunStruct> LoadTournamentRuns()
	{
		string text = Application.persistentDataPath + "/saveHelp.idl";
		try
		{
			return (!File.Exists(text)) ? null : JSONToTournamentRun(LoadTournamentRunsToJSON(text));
		}
		catch (Exception)
		{
			return null;
		}
	}

	private JSONObject LoadTournamentRunsToJSON(string filePath)
	{
		string text = Encryptor.Decrypt(File.ReadAllText(filePath));
		if (text.Length > 0)
		{
			return JSONObject.Create(text);
		}
		return null;
	}

	private List<TournamentRunStruct> JSONToTournamentRun(JSONObject json)
	{
		if (json == null)
		{
			return null;
		}
		List<TournamentRunStruct> list = new List<TournamentRunStruct>();
		int num = json[0].asInt("Date", () => 0);
		if (num > 0)
		{
			TotalDaysSinceStart.Value = num;
		}
		for (int i = 1; i < json.Count; i++)
		{
			string run = json[i].asString("Run", () => string.Empty);
			list.Add(new TournamentRunStruct
			{
				PlayerId = json[i].asString("PlayerId", () => string.Empty),
				DisplayName = json[i].asString("DisplayName", () => string.Empty),
				RegionTag = json[i].asString("Region", () => string.Empty),
				Variation = json[i].asInt("Variation", () => 0),
				Run = DataHelper.ConvertStringToInt16Array(run),
				CurrentScore = 0
			});
		}
		return list;
	}
}
