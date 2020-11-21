using Big;
using Facebook.Unity;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Analytics;

public class GameAnalytics : PersistentSingleton<GameAnalytics>
{
	public ReactiveProperty<PopupDecision> PopupDecisions = Observable.Never<PopupDecision>().ToReactiveProperty();

	public ReactiveProperty<string> ObjectTapped = Observable.Never<string>().ToReactiveProperty();

	public ReactiveProperty<GemTransaction> GemTransactions = Observable.Never<GemTransaction>().ToReactiveProperty();

	public ReactiveProperty<ChestTransaction> ChestTransactions = Observable.Never<ChestTransaction>().ToReactiveProperty();

	public ReactiveProperty<KeyTransaction> KeyTransactions = Observable.Never<KeyTransaction>().ToReactiveProperty();

	public ReactiveProperty<BerryTransaction> BerryTransaction = Observable.Never<BerryTransaction>().ToReactiveProperty();

	public ReactiveProperty<PrestigeOrder> PrestigeTriggered = Observable.Never<PrestigeOrder>().ToReactiveProperty();

	public ReactiveProperty<PlayerGoalRunner> GoalCompleted = Observable.Never<PlayerGoalRunner>().ToReactiveProperty();

	public ReactiveProperty<IAPProductEnum> NewBundle = Observable.Never<IAPProductEnum>().ToReactiveProperty();

	public ReactiveProperty<GearRunner> GearUpgraded = Observable.Never<GearRunner>().ToReactiveProperty();

	public ReactiveProperty<SkillsEnum> SkillUsed = Observable.Never<SkillsEnum>().ToReactiveProperty();

	public ReactiveProperty<bool> TournamentEvent = Observable.Never<bool>().ToReactiveProperty();

	public ReactiveProperty<bool> BossBattleResult = new ReactiveProperty<bool>();

	public ReactiveProperty<float> MusicVolume = new ReactiveProperty<float>();

	public ReactiveProperty<float> SFXVolume = new ReactiveProperty<float>();

	public ReactiveProperty<bool> ARSupported = new ReactiveProperty<bool>(initialValue: false);

	public void InitializeGameAnalytics()
	{
		if (!base.Inited)
		{
			AnalyticsService analytics = PersistentSingleton<AnalyticsService>.Instance;
			IAPService instance = PersistentSingleton<IAPService>.Instance;
			AdService instance2 = PersistentSingleton<AdService>.Instance;
			SessionService sessionService = PersistentSingleton<SessionService>.Instance;
			PlayerData playerData = PlayerData.Instance;
			instance.IAPCompleted.Subscribe(delegate(IAPTransactionState iapCompleted)
			{
				analytics.TrackEvent("IAP_Complete", iapCompleted.asDictionary(), string.Empty);
			});
			instance.IAPNotCompleted.Subscribe(delegate(IAPNotCompleted iapNotCompleted)
			{
				analytics.TrackEvent("IAP_Not_Complete", iapNotCompleted.asDictionary(), string.Empty);
			});
			instance.IAPValidated.Subscribe(delegate(IAPTransactionState iapValidated)
			{
				analytics.TrackEvent("IAP_Validated", iapValidated.asDictionary(), string.Empty);
			});
			instance2.AdStarted.Subscribe(delegate(AdWatched adStarted)
			{
				analytics.TrackEvent("Ad_Started", adStarted.asDictionary(), string.Empty);
			});
			(from adResult in instance2.AdResults
				where adResult.result == AdService.V2PShowResult.Finished
				select adResult).Subscribe(delegate(AdWatched adWatched)
			{
				analytics.TrackEvent("Ad_Watched", adWatched.asDictionary(), "fb_mobile_content_view");
			});
			(from adResult in instance2.AdResults
				where adResult.result == AdService.V2PShowResult.Failed || adResult.result == AdService.V2PShowResult.Skipped
				select adResult).Subscribe(delegate(AdWatched adFailed)
			{
				analytics.TrackEvent("Ad_Failed", adFailed.asDictionary(), string.Empty);
			});
			(from a in instance2.AdLoadRequests
				select a.asDictionary(ServerTimeService.NowTicks())).Subscribe(delegate(Dictionary<string, string> adLoadRequest)
			{
				analytics.TrackEvent("Ad_Load", adLoadRequest, string.Empty);
			});
			(from isNewUser in sessionService.newUser
				where isNewUser
				select isNewUser).Subscribe(delegate
			{
				analytics.TrackEvent("New_User", new Dictionary<string, string>(), string.Empty);
			});
			playerData.SessionNumber.Subscribe(delegate
			{
				analytics.TrackEvent("Session_Started", new Dictionary<string, string>
				{
					{
						"LastSavedBy",
						playerData.LastSavedBy
					},
					{
						"HoursSinceLastSession",
						((int)TimeSpan.FromTicks(sessionService.TicksSinceLastSave()).TotalHours).ToString()
					},
					{
						"Lifetime_BlocksDestroyed",
						CountLifetimeBlocksDestroyed()
					}
				}, string.Empty);
			});
			playerData.SessionNumber.Subscribe(delegate
			{
				analytics.TrackEvent("Session_Balance", new Dictionary<string, string>
				{
					{
						"Material_Grass",
						playerData.BlocksCollected[0].Value.ToString()
					},
					{
						"Material_Dirt",
						playerData.BlocksCollected[1].Value.ToString()
					},
					{
						"Material_Wood",
						playerData.BlocksCollected[2].Value.ToString()
					},
					{
						"Material_Stone",
						playerData.BlocksCollected[3].Value.ToString()
					},
					{
						"Material_Metal",
						playerData.BlocksCollected[4].Value.ToString()
					},
					{
						"Material_Gold",
						playerData.BlocksCollected[5].Value.ToString()
					},
					{
						"Material_Jelly",
						playerData.BlocksCollected[6].Value.ToString()
					}
				}, string.Empty);
			});
			(from ord in PrestigeTriggered
				where ord == PrestigeOrder.PrestigeStart
				select ord).Subscribe(delegate
			{
				analytics.TrackEvent("Prestige", new Dictionary<string, string>
				{
					{
						"Material_Grass",
						((double)playerData.BlocksInBackpack[0].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[13].Value.ToDouble()).ToString()
					},
					{
						"Material_Dirt",
						((double)playerData.BlocksInBackpack[1].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[14].Value.ToDouble()).ToString()
					},
					{
						"Material_Wood",
						((double)playerData.BlocksInBackpack[2].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[15].Value.ToDouble()).ToString()
					},
					{
						"Material_Stone",
						((double)playerData.BlocksInBackpack[3].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[16].Value.ToDouble()).ToString()
					},
					{
						"Material_Metal",
						((double)playerData.BlocksInBackpack[4].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[17].Value.ToDouble()).ToString()
					},
					{
						"Material_Gold",
						((double)playerData.BlocksInBackpack[5].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[18].Value.ToDouble()).ToString()
					},
					{
						"Material_Jelly",
						((double)playerData.BlocksInBackpack[6].Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[19].Value.ToDouble()).ToString()
					}
				}, string.Empty);
			});
			playerData.TutorialStep.Skip(1).Subscribe(delegate(int step)
			{
				analytics.TrackEvent("FTUE_Stage_Done", new Dictionary<string, string>
				{
					{
						"FTUE_Stage",
						Singleton<TutorialGoalCollectionRunner>.Instance.GetOrCreatePlayerGoalRunner(step - 1).GoalConfig.ID
					},
					{
						"FTUE_Stage_ID",
						(step - 1).ToString()
					}
				}, (step < Singleton<EconomyHelpers>.Instance.GetTutorialGoalAmount() - 1) ? string.Empty : "fb_mobile_tutorial_completion");
			});
			playerData.TutorialStep.Skip(1).Subscribe(delegate(int step)
			{
				Analytics.CustomEvent("FTUE_Stage_Done", new Dictionary<string, object>
				{
					{
						"FTUE_Stage",
						Singleton<TutorialGoalCollectionRunner>.Instance.GetOrCreatePlayerGoalRunner(step - 1).GoalConfig.ID
					},
					{
						"FTUE_Stage_ID",
						(step - 1).ToString()
					}
				});
			});
			GemTransactions.Subscribe(delegate(GemTransaction transaction)
			{
				analytics.TrackEvent("Gem_Transaction", transaction.asDictionary(), (!(transaction.transaction == "spendFunds")) ? string.Empty : "fb_mobile_spent_credits", transaction.amount);
			});
			ChestTransactions.Subscribe(delegate(ChestTransaction transaction)
			{
				analytics.TrackEvent("Chest_Transaction", transaction.asDictionary(), string.Empty);
			});
			KeyTransactions.Subscribe(delegate(KeyTransaction transaction)
			{
				analytics.TrackEvent("Key_Added", transaction.asDictionary(), string.Empty);
			});
			BerryTransaction.Subscribe(delegate(BerryTransaction transaction)
			{
				analytics.TrackEvent("Berry_Added", transaction.asDictionary(), string.Empty);
			});
			PopupDecisions.Subscribe(delegate(PopupDecision decision)
			{
				analytics.TrackEvent("Popup_Decision", decision.asDictionary(), string.Empty);
			});
			(from coins in playerData.LifetimeCoins.Pairwise()
				where coins.Previous.exponent != coins.Current.exponent
				select coins).Subscribe(delegate
			{
				analytics.TrackEvent("New_LTE_Digit", new Dictionary<string, string>(), string.Empty);
			});
			playerData.LifetimeChunk.Skip(1).Subscribe(delegate
			{
				analytics.TrackEvent("New_Max_Chunk", new Dictionary<string, string>(), string.Empty);
			});
			playerData.LifetimeCreatures.Skip(1).Subscribe(delegate
			{
				analytics.TrackEvent("New_Max_Companions", new Dictionary<string, string>(), string.Empty);
			});
			HeroStateFactory.GetOrCreateHeroState(0).LifetimeLevel.Skip(1).Subscribe(delegate
			{
				analytics.TrackEvent("New_Max_Hero_Level", new Dictionary<string, string>(), string.Empty);
			});
			GoalCompleted.Subscribe(delegate(PlayerGoalRunner goal)
			{
				analytics.TrackEvent("Achievement_Completed", new Dictionary<string, string>
				{
					{
						"Goal_ID",
						goal.GoalConfig.ID
					}
				}, "fb_mobile_achievement_unlocked");
			});
			NewBundle.Subscribe(delegate(IAPProductEnum bundle)
			{
				analytics.TrackEvent("Offer_Given", new Dictionary<string, string>
				{
					{
						"Offer",
						bundle.ToString()
					}
				}, string.Empty);
			});
			GearUpgraded.Subscribe(delegate(GearRunner gear)
			{
				analytics.TrackEvent("Gear_Upgrade", new Dictionary<string, string>
				{
					{
						"GearLevel",
						gear.Level.ToString()
					},
					{
						"GearID",
						gear.GearIndex.ToString()
					}
				}, string.Empty);
			});
			SkillUsed.Subscribe(delegate(SkillsEnum skill)
			{
				analytics.TrackEvent("Skill_Used", new Dictionary<string, string>
				{
					{
						"Skill",
						skill.ToString()
					},
					{
						"Lifetime_Used",
						playerData.SkillStates[(int)skill].LifetimeUsed.Value.ToString()
					}
				}, string.Empty);
			});
			BossBattleResult.Skip(1).Subscribe(delegate(bool result)
			{
				analytics.TrackEvent("BossBattleResult", new Dictionary<string, string>
				{
					{
						"Result",
						result.ToString()
					},
					{
						"Current_Chunk",
						playerData.MainChunk.Value.ToString()
					}
				}, string.Empty);
			});
			ObjectTapped.Subscribe(delegate(string obj)
			{
				analytics.TrackEvent("Object_Tapped", new Dictionary<string, string>
				{
					{
						"Object_Tapped",
						obj
					}
				}, string.Empty);
			});
			/*XPromoPlugin.XPromoActions.Subscribe(delegate(XPromoAction XPromoAction)
			{
				analytics.TrackEvent("X_Promo_Action", XPromoAction.asDictionary(), string.Empty);
			});*/
			PersistentSingleton<ARService>.Instance.LevelEditorEvent.Subscribe(delegate(string editorEvent)
			{
				analytics.TrackEvent("Level_Editor", new Dictionary<string, string>
				{
					{
						"Event",
						editorEvent
					}
				}, string.Empty);
			});
			PersistentSingleton<AnalyticsService>.Instance.FortunePodsResult.Subscribe(delegate(FortunePodResult result)
			{
				analytics.TrackEvent("Fortune_Pod_Result", result.asDictionary(), string.Empty);
			});
			(from chunk in playerData.MainChunk.Skip(1)
				where chunk == 22 && PlayerData.Instance.LifetimePrestiges.Value == 0 && !Singleton<QualitySettingsRunner>.Instance.LowFPS.Value
				select chunk).Subscribe(delegate
			{
				SceneLoader.Instance.StartCoroutine(TrackFpsRuotine());
			});
			(from hr in playerData.HasReviewed.Skip(1)
				where hr
				select hr).Subscribe(delegate
			{
				analytics.TrackEvent("fb_mobile_rate", new Dictionary<string, string>
				{
					{
						"fb_max_rating_value",
						"5"
					}
				}, "fb_mobile_rate", 5f);
			});
			playerData.Trophies.Skip(1).Subscribe(delegate(int trophies)
			{
				AnalyticsService analyticsService = analytics;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				Dictionary<string, string> dictionary2 = dictionary;
				TournamentTier tournamentTier = (TournamentTier)trophies;
				dictionary2.Add("RewardType", tournamentTier.ToString());
				analyticsService.TrackEvent("TournamentReward", dictionary, string.Empty);
			});
			(from tour in TournamentEvent
				where tour
				select tour).Subscribe(delegate
			{
				analytics.TrackEvent("TournamentStarted", new Dictionary<string, string>
				{
					{
						"Current_Chunk",
						playerData.MainChunk.Value.ToString()
					}
				}, string.Empty);
			});
			(from tour in TournamentEvent
				where !tour
				select tour).Subscribe(delegate
			{
				analytics.TrackEvent("TournamentEnded", new Dictionary<string, string>
				{
					{
						"Current_Chunk",
						playerData.MainChunk.Value.ToString()
					},
					{
						"Highest_Chunk_Reached",
						Singleton<TournamentRunner>.Instance.PlayerHighestWorld.Value.ToString()
					},
					{
						"Position",
						Singleton<TournamentRunner>.Instance.PlayerRank.Value.ToString()
					}
				}, string.Empty);
			});
			(from p in Observable.EveryApplicationPause()
				where !p && FB.IsInitialized
				select p).Subscribe(delegate
			{
				FB.ActivateApp();
			});
			FirebaseAnalytics.SetUserId(playerData.PlayerId);
			FirebaseAnalytics.SetUserProperty("AppGenuine", (!Application.genuineCheckAvailable) ? "N/A" : Application.genuine.ToString());
			FirebaseAnalytics.SetUserProperty("Build_Number", Application.version);
			subscribeToUserProperty("Days_Retained", playerData.DaysRetained);
			// FirebaseAnalytics.SetUserProperty("Games_Installed", XPromoPlugin.InstalledApps());
			FirebaseAnalytics.SetUserProperty("Friends_Giftable", PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.Count.ToString());
			FirebaseAnalytics.SetUserProperty("Friends_Playing", PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.Values.Count((FBPlayer p) => p.Playing).ToString());
			subscribeToUserProperty("Has_Reviewed", playerData.HasReviewed);
			FirebaseAnalytics.SetUserProperty("Hours_In_Lifetime", ((int)TimeSpan.FromTicks(sessionService.TicksPlayedInLifetime()).TotalHours).ToString());
			FirebaseAnalytics.SetUserProperty("Segments", string.Join(",", PersistentSingleton<GameSettings>.Instance.Segments.ToArray()));
			subscribeToUserProperty("Music", MusicVolume);
			subscribeToUserProperty("Notifications_Decided", playerData.NotificationDecision);
			subscribeToUserProperty("Online", ConnectivityService.InternetConnectionAvailable);
			subscribeToUserProperty("Review_State", playerData.ReviewState);
			subscribeToUserProperty("Sound", SFXVolume);
			subscribeToUserProperty("AR_Editor_Supported", ARSupported);
			FirebaseAnalytics.SetUserProperty("Language", playerData.Language);
			FirebaseAnalytics.SetUserProperty("PFID", playerData.PFId.Value);
			FirebaseAnalytics.SetUserProperty("FBID", playerData.FBId.Value);
			base.Inited = true;
		}
	}

	private IEnumerator TrackFpsRuotine()
	{
		yield return new WaitForSeconds(5f);
		Dictionary<string, string> dict = new Dictionary<string, string>
		{
			{
				"GPU",
				SystemInfo.graphicsDeviceName
			},
			{
				"RAM",
				SystemInfo.systemMemorySize.ToString()
			},
			{
				"FPS",
				FrameRateCounter.AvgFps.ToString()
			},
			{
				"HighDetail",
				Singleton<QualitySettingsRunner>.Instance.HighDetails.Value.ToString()
			}
		};
		PersistentSingleton<AnalyticsService>.Instance.TrackEvent("GpuPerf", dict, string.Empty);
	}

	private void subscribeToUserProperty<A>(string key, UniRx.IObservable<A> dimension)
	{
		dimension.DistinctUntilChanged().Subscribe(delegate(A d)
		{
			FirebaseAnalytics.SetUserProperty(key, Convert.ToString(d));
		});
	}

	private string CountLifetimeBlocksDestroyed()
	{
		long num = 0L;
		foreach (ReactiveProperty<long> item in PlayerData.Instance.LifetimeBlocksDestroyed)
		{
			num += item.Value;
		}
		return num.ToString();
	}
}
