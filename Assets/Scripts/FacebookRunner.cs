using Facebook.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[PropertyClass]
public class FacebookRunner : Singleton<FacebookRunner>
{
	public ReactiveProperty<bool> UpdateLeaderboards = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> IsLoggedToFacebook;

	public ReactiveProperty<string> PlayerID = Observable.Never<string>().ToReactiveProperty();

	[PropertyString]
	public ReactiveProperty<string> PlayerName = new ReactiveProperty<string>(string.Empty);

	[PropertyBool]
	public ReactiveProperty<bool> FirstTimeRewardAvailable = new ReactiveProperty<bool>(initialValue: false);

	public UniRx.IObservable<UniRx.Tuple<FacebookEvent, object>> FacebookEvents;

	private Subject<UniRx.Tuple<FacebookEvent, object>> m_fbEventsInternal = new Subject<UniRx.Tuple<FacebookEvent, object>>();

	public ReadOnlyReactiveProperty<bool> GiftOpenBlocked;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> GiftOpenAvailable;

	public ReadOnlyReactiveProperty<bool> GiftSendBlocked;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> GiftSendAvailable;

	[PropertyInt]
	public ReactiveProperty<int> NumUnopenedGifts = new ReactiveProperty<int>(0);

	public Subject<bool> GiftsFetched = new Subject<bool>();

	public ReactiveProperty<int> GiftOpenInProgress = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> GiftSendInProgress = new ReactiveProperty<int>(0);

	public Subject<FacebookGift> GiftConsuming = new Subject<FacebookGift>();

	public Subject<FacebookGift> GiftConsumeFailed = new Subject<FacebookGift>();

	public Subject<FacebookGift> GiftConsumeSuccess = new Subject<FacebookGift>();

	public Subject<string> GiftSending = new Subject<string>();

	public Subject<string> GiftSendingFailed = new Subject<string>();

	public Subject<string> GiftSendingSuccess = new Subject<string>();

	private List<string> m_consumedFrom = new List<string>();

	private List<FacebookGift> m_allGifts = new List<FacebookGift>();

	public FacebookRunner()
	{
		SceneLoader root = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		IsLoggedToFacebook = PersistentSingleton<FacebookAPIService>.Instance.IsLoggedToFB.ToReactiveProperty();
		FacebookEvents = m_fbEventsInternal.AsObservable();
		FacebookEvents.Subscribe(delegate(UniRx.Tuple<FacebookEvent, object> e)
		{
			switch (e.Item1)
			{
			case FacebookEvent.LOGIN_CANCELLED:
			case FacebookEvent.LOGIN_FAILED:
				break;
			case FacebookEvent.NEW_FRIENDS_FETCHED:
				break;
			case FacebookEvent.ADD_FRIEND_REQUEST_ERROR:
				break;
			case FacebookEvent.LOGIN_ATTEMPT:
				HandleLoginAttempt((ILoginResult)e.Item2);
				break;
			case FacebookEvent.LOGIN:
				RequestFBData();
				break;
			case FacebookEvent.MY_DATA:
				HandleMyFBData((MyFacebookData)e.Item2);
				GetAllRequests();
				break;
			case FacebookEvent.FRIEND_DATA:
				HandleFriendFBData((FBPlayer)e.Item2);
				GiftsFetched.OnNext(value: true);
				break;
			case FacebookEvent.ALL_REQUESTS:
				HandleAllRequests((IGraphResult)e.Item2);
				break;
			case FacebookEvent.APP_REQUEST_RESULT:
				HandleAppRequestResult((IAppRequestResult)e.Item2);
				break;
			case FacebookEvent.FRIEND_REQUEST_RESULT:
				HandleFBFriendAppRequest((FBPlayer)e.Item2);
				GiftsFetched.OnNext(value: true);
				break;
			}
		}).AddTo(root);
		GiftConsuming.Subscribe(delegate
		{
			GiftOpenInProgress.Value++;
		}).AddTo(root);
		GiftConsumeFailed.Subscribe(delegate
		{
			GiftOpenInProgress.Value--;
		}).AddTo(root);
		GiftConsumeSuccess.Subscribe(delegate
		{
			GiftOpenInProgress.Value--;
		}).AddTo(root);
		GiftSending.Subscribe(delegate
		{
			GiftSendInProgress.Value++;
		}).AddTo(root);
		GiftSendingFailed.Subscribe(delegate
		{
			GiftSendInProgress.Value--;
		}).AddTo(root);
		GiftSendingSuccess.Subscribe(delegate
		{
			GiftSendInProgress.Value--;
		}).AddTo(root);
		GiftOpenBlocked = (from count in GiftOpenInProgress
			select count != 0).TakeUntilDestroy(root).ToReadOnlyReactiveProperty();
		GiftSendBlocked = (from count in GiftSendInProgress
			select count != 0).TakeUntilDestroy(root).ToReadOnlyReactiveProperty();
		GiftOpenAvailable = (from _ in (from _ in GiftOpenInProgress
				select true).Merge(GiftsFetched)
			select UniqueGifts().Count > 0).TakeUntilDestroy(root).ToReadOnlyReactiveProperty();
		GiftSendAvailable = (from _ in (from _ in GiftSendInProgress
				select true).Merge(GiftsFetched)
			select !AreAllPlayersGifted()).TakeUntilDestroy(root).ToReadOnlyReactiveProperty();
		GiftConsumeSuccess.Subscribe(delegate(FacebookGift gift)
		{
			MaybeSendBackGift(m_consumedFrom, gift.FromId);
		}).AddTo(root);
		(from pause in Observable.EveryApplicationPause().StartWith(value: false)
			where !pause && IsLoggedToFacebook.Value
			select pause).Subscribe(delegate
		{
			GetAllRequests();
		}).AddTo(root);
		(from tuple in PlayerData.Instance.LifetimeChunk.CombineLatest(PlayerData.Instance.FBId, (int highscore, string player) => new
			{
				highscore,
				player
			})
			where tuple.player != string.Empty && PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(tuple.player)
			where tuple.highscore > PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[tuple.player].Highscore
			select tuple).Subscribe(tuple =>
		{
			PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[tuple.player].Highscore = tuple.highscore;
		}).AddTo(root);
		(from e in FacebookEvents
			where e.Item1 == FacebookEvent.LOGIN
			select (AccessToken)e.Item2).Subscribe(delegate(AccessToken access)
		{
			PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Take(1).Subscribe(delegate
			{
				JSONObject customInfo = PersistentSingleton<PlayFabService>.Instance.GetCustomInfo();
				SyncToFacebook(access, customInfo, delegate(JSONObject json)
				{
					string a = json.asString("PlayFabId", () => string.Empty);
					if (a != PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value)
					{
						PersistentSingleton<PlayFabService>.Instance.ClearCmdQueue();
						PlayerData.Instance.LastSavedToCloud.Value = 0L;
						PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.UnpublishValue();
						PersistentSingleton<PlayFabService>.Instance.ShouldLoginImmediately = true;
						SceneLoadHelper.LoadInitScene();
					}
				}, null);
			}).AddTo(root);
		}).AddTo(root);
		(from e in FacebookEvents
			where e.Item1 == FacebookEvent.MY_DATA
			select (from f in ((MyFacebookData)e.Item2).Friends
				where ((Dictionary<string, object>)f).ContainsKey("id")
				select (string)((Dictionary<string, object>)f)["id"]).ToList()).Subscribe(delegate(List<string> list)
		{
			PersistentSingleton<PlayFabService>.Instance.GetPlayFabToFacebookIdMapping(list, delegate(Dictionary<string, string> fbToPf)
			{
				PlayerData.Instance.PlayFabToFBIds.Value = fbToPf;
				UpdateLeaderboards.SetValueAndForceNotify(value: true);
			}, null);
		}).AddTo(root);
		(from logged in IsLoggedToFacebook
			where logged
			select logged).Subscribe(delegate
		{
			OnLoggedIn();
		}).AddTo(root);
	}

	public void PostInit()
	{
		if (string.IsNullOrEmpty(PlayerData.Instance.FBId.Value))
		{
			FirstTimeRewardAvailable.Value = true;
			(from id in PlayerData.Instance.FBId.Pairwise()
				where id.Previous == string.Empty && id.Current != string.Empty
				select id).Take(1).Delay(TimeSpan.FromSeconds(3.0)).Subscribe(delegate
			{
				GiveFirstTimeLoginBonus();
				BindingManager.Instance.FacebookConnectionSuccessParent.ShowInfo();
			})
				.AddTo(SceneLoader.Instance);
		}
	}

	public List<FacebookGift> UniqueGifts()
	{
		List<FacebookGift> list = new List<FacebookGift>();
		foreach (FacebookGift gift in m_allGifts)
		{
			if (list.Find((FacebookGift g) => g.FromId == gift.FromId) == null)
			{
				list.Add(gift);
			}
		}
		return list;
	}

	private bool AreAllPlayersGifted()
	{
		foreach (KeyValuePair<string, FBPlayer> fBPlayer in PersistentSingleton<FacebookAPIService>.Instance.FBPlayers)
		{
			FBPlayer value = fBPlayer.Value;
			if (!(value.Id == PlayerData.Instance.FBId.Value) && value.GiftTimeStamp + TimeSpan.FromSeconds(PersistentSingleton<GameSettings>.Instance.GiftSendCooldown).Ticks <= ServerTimeService.NowTicks())
			{
				return false;
			}
		}
		return true;
	}

	private void GiveFirstTimeLoginBonus()
	{
		Singleton<FundRunner>.Instance.AddGems(200, "facebookLogin", "rewards");
		FirstTimeRewardAvailable.Value = false;
	}

	public void Login()
	{
		if (!PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.HasValue)
		{
			PersistentSingleton<PlayFabService>.Instance.Login();
		}
		PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Take(1).Subscribe(delegate
		{
			LoginToFB();
		}).AddTo(SceneLoader.Instance);
	}

	private void LoginToFB()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.LOGIN_FAILED, new object()));
		}
		else
		{
			PersistentSingleton<FacebookAPIService>.Instance.Login(delegate(ILoginResult result)
			{
				m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.LOGIN_ATTEMPT, (object)result));
			});
		}
	}

	public void Logout()
	{
		if (PersistentSingleton<FacebookAPIService>.Instance.LogOut())
		{
			PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.Clear();
		}
		m_allGifts.Clear();
		NumUnopenedGifts.Value = 0;
		GiftsFetched.OnNext(value: true);
	}

	private void HandleLoginAttempt(ILoginResult result)
	{
		if (result != null && !string.IsNullOrEmpty(result.RawResult))
		{
			IDictionary<string, object> resultDictionary = result.ResultDictionary;
			if (resultDictionary.ContainsKey("cancelled"))
			{
				m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.LOGIN_CANCELLED, new object()));
			}
			else if (resultDictionary.ContainsKey("error"))
			{
				m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.LOGIN_FAILED, new object()));
			}
		}
	}

	private void OnLoggedIn()
	{
		if (PlayerData.Instance.FBId.Value != AccessToken.CurrentAccessToken.UserId)
		{
			if (PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.Count > 0)
			{
				PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.Clear();
			}
			PlayerData.Instance.FBId.Value = AccessToken.CurrentAccessToken.UserId;
		}
		m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.LOGIN, (object)AccessToken.CurrentAccessToken));
	}

	private void RequestFBData()
	{
		(from available in ConnectivityService.InternetConnectionAvailable
			where available
			select available).Take(1).Subscribe(delegate
		{
			PersistentSingleton<FacebookAPIService>.Instance.RequestMyFBData(delegate(FBPlayer f, List<object> friends)
			{
				m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.MY_DATA, (object)new MyFacebookData(f, friends)));
			});
		}).AddTo(SceneLoader.Instance);
	}

	private void HandleMyFBData(MyFacebookData fbData)
	{
		FBPlayer player = fbData.Player;
		List<object> friends = fbData.Friends;
		if (!PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(player.Id))
		{
			PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[fbData.Player.Id] = player;
		}
		SaveProfilePictureToFile(player.ProfilePicture, player.Id);
		UpdateLeaderboards.SetValueAndForceNotify(value: true);
		PlayerID.Value = player.Id;
		PlayerName.Value = player.Name;
		List<string> list = new List<string>();
		for (int i = 0; i < friends.Count; i++)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)friends[i];
			if (dictionary.ContainsKey("id"))
			{
				string text = (string)dictionary["id"];
				if (PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(text))
				{
					FBPlayer fBPlayer = PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[(string)dictionary["id"]];
					fBPlayer.Playing = true;
				}
				else
				{
					list.Add(text);
				}
			}
		}
		if (list.Count > 0)
		{
			FBPlayersFromIDs(list);
		}
	}

	private void FBPlayersFromIDs(List<string> fbIDs)
	{
		int counter = fbIDs.Count;
		UniRx.IObservable<string> left = fbIDs.ToObservable();
		Subject<bool> queries = new Subject<bool>();
		left.Zip(queries.StartWith(value: true), (string id, bool b) => id).Subscribe(delegate(string id)
		{
			PersistentSingleton<FacebookAPIService>.Instance.LoadFriendAPI(id, delegate(FBPlayer f)
			{
				m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.FRIEND_DATA, (object)f));
				counter--;
				queries.OnNext(value: true);
			});
		}).AddTo(SceneLoader.Instance);
	}

	private void HandleFriendFBData(FBPlayer friend)
	{
		SaveProfilePictureToFile(friend.ProfilePicture, friend.Id);
		UpdateLeaderboards.SetValueAndForceNotify(value: true);
		if (!PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(friend.Id))
		{
			PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.Add(friend.Id, friend);
		}
	}

	private void SaveProfilePictureToFile(Texture2D tex, string FBID)
	{
		try
		{
			PngTexture.SaveTexture2DAsPNG(tex, FBID + ".png");
			UnityEngine.Object.Destroy(tex);
		}
		catch (Exception)
		{
		}
	}

	public void SendAppRequestFriendSelector(string msg, string title)
	{
		PersistentSingleton<MainSaver>.Instance.PleaseSave("fb_add_friend");
		if (!PersistentSingleton<FacebookAPIService>.Instance.CallAppRequestAsFriendSelector(msg, title, delegate(IAppRequestResult result)
		{
			m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.APP_REQUEST_RESULT, (object)result));
		}))
		{
			m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.ADD_FRIEND_REQUEST_ERROR, new object()));
		}
	}

	public void HandleAppRequestResult(IAppRequestResult result)
	{
		if (result != null && result.To != null)
		{
			List<string> list = new List<string>(result.To);
			int count = PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.Count;
			foreach (string item in list)
			{
				if (!PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(item))
				{
					PersistentSingleton<FacebookAPIService>.Instance.LoadFriendAPI(item, delegate(FBPlayer player)
					{
						m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.FRIEND_REQUEST_RESULT, (object)player));
					});
				}
				else
				{
					PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[item].Invited = true;
				}
			}
		}
	}

	private void HandleFBFriendAppRequest(FBPlayer f)
	{
		f.Invited = true;
		BindingManager.Instance.FacebookInviteSuccessParent.ShowInfo();
		HandleFriendFBData(f);
	}

	private void SyncToFacebook(AccessToken accessToken, JSONObject customInfo, Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject fakeJson = new JSONObject(JSONObject.Type.OBJECT);
		if (accessToken.UserId == PersistentSingleton<PlayFabService>.Instance.LinkedFacebookId.Value)
		{
			fakeJson.AddField("PlayFabId", PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value);
			PersistentSingleton<PlayFabService>.Instance.LoginWithFacebook(accessToken, createAccount: false, delegate
			{
				callback(fakeJson);
			}, errorCallback);
		}
		else
		{
			bool hasValue = PersistentSingleton<PlayFabService>.Instance.LinkedFacebookId.HasValue;
			PersistentSingleton<PlayFabService>.Instance.LoginWithFacebook(accessToken, hasValue, delegate(JSONObject fbLogin)
			{
				string text = customInfo.asString("CustomId", () => Guid.NewGuid().ToString());
				string sessionTicket = fbLogin.asString("SessionTicket", () => string.Empty);
				customInfo.SetField("CustomId", text);
				PersistentSingleton<PlayFabService>.Instance.LinkCustomId(text, force: true, sessionTicket, delegate
				{
					PersistentSingleton<PlayFabService>.Instance.WriteCustomInfo(customInfo);
					callback(fakeJson);
				}, null);
			}, delegate(string error)
			{
				if (error == "AccountNotLinked")
				{
					fakeJson.AddField("PlayFabId", PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value);
					PersistentSingleton<PlayFabService>.Instance.LinkFacebookAccount(accessToken, delegate
					{
						callback(fakeJson);
					}, errorCallback);
				}
			});
		}
	}

	private void GetAllRequests()
	{
		PersistentSingleton<FacebookAPIService>.Instance.GetAllRequests(delegate(IGraphResult result)
		{
			m_fbEventsInternal.OnNext(UniRx.Tuple.Create(FacebookEvent.ALL_REQUESTS, (object)result));
		});
	}

	private FacebookGift ParseGiftData(Dictionary<string, object> data)
	{
		string empty = string.Empty;
		string fromId = string.Empty;
		string giftId = (string)data["id"];
		if (data.TryGetValue("from", out object value))
		{
			empty = (string)((Dictionary<string, object>)value)["name"];
			fromId = (string)((Dictionary<string, object>)value)["id"];
		}
		return new FacebookGift(fromId, giftId);
	}

	private void HandleAllRequests(IGraphResult result)
	{
		m_allGifts.Clear();
		NumUnopenedGifts.Value = 0;
		if (!string.IsNullOrEmpty(result.RawResult))
		{
			IDictionary<string, object> resultDictionary = result.ResultDictionary;
			if (resultDictionary.TryGetValue("data", out List<object> value))
			{
				foreach (Dictionary<string, object> item in value)
				{
					if (item.TryGetValue("data", out object value2) && (string)value2 == "DefaultGiftData")
					{
						m_allGifts.Add(ParseGiftData(item));
					}
				}
			}
		}
		NumUnopenedGifts.Value = UniqueGifts().Count;
		GiftsFetched.OnNext(value: true);
	}

	public static bool CanGiftPlayer(FBPlayer fbPlayer)
	{
		if (fbPlayer.Id == PlayerData.Instance.FBId.Value)
		{
			return false;
		}
		if (fbPlayer.GiftTimeStamp + TimeSpan.FromSeconds(PersistentSingleton<GameSettings>.Instance.GiftSendCooldown).Ticks > ServerTimeService.NowTicks())
		{
			return false;
		}
		return true;
	}

	private bool WasGiftSent(IAppRequestResult result)
	{
		if (result == null)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(result.Error))
		{
			return false;
		}
		if (result.Cancelled)
		{
			return false;
		}
		return true;
	}

	private void GiftSentCallback(List<string> list, IAppRequestResult result)
	{
		if (!WasGiftSent(result))
		{
			foreach (string item in list)
			{
				GiftSendingFailed.OnNext(item);
			}
		}
		else
		{
			foreach (string item2 in list)
			{
				if (PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(item2))
				{
					PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[item2].SetGiftTimeStamp(ServerTimeService.NowTicks());
					GiftSendingSuccess.OnNext(item2);
				}
			}
		}
	}

	private void MaybeSendBackGift(List<string> list, string from)
	{
		if (!list.Contains(from) && PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(from) && CanGiftPlayer(PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[from]))
		{
			list.Add(from);
		}
	}

	public void GiftBackPlayers(string message, string title)
	{
		if (m_consumedFrom.Count > 0)
		{
			GiftPlayers(m_consumedFrom.ToList(), message, title);
			m_consumedFrom.Clear();
		}
	}

	public void GiftAllPlayers(string message, string title)
	{
		if (!GiftSendBlocked.Value)
		{
			List<string> list = (from keyValue in (from keyValue in PersistentSingleton<FacebookAPIService>.Instance.FBPlayers
					where CanGiftPlayer(keyValue.Value)
					select keyValue).Take(25)
				select keyValue.Value.Id).ToList();
			GiftPlayers(list, message, title);
		}
	}

	private void GiftPlayers(List<string> list, string message, string title)
	{
		foreach (string item in list)
		{
			GiftSending.OnNext(item);
		}
		PersistentSingleton<FacebookAPIService>.Instance.GiftWithDirectRequest(list, message, title, delegate(IAppRequestResult result)
		{
			GiftSentCallback(list, result);
		});
	}

	public void GiftPlayer(string to, string message, string title)
	{
		if (!GiftSendBlocked.Value)
		{
			List<string> list = new List<string>();
			list.Add(to);
			GiftSending.OnNext(to);
			PersistentSingleton<FacebookAPIService>.Instance.GiftWithDirectRequest(list, message, title, delegate(IAppRequestResult result)
			{
				GiftSentCallback(list, result);
			});
		}
	}

	public void OpenAllGifts()
	{
		if (!GiftOpenBlocked.Value)
		{
			List<FacebookGift> list = UniqueGifts();
			foreach (FacebookGift item in list)
			{
				OpenGift(item);
			}
		}
	}

	public void OpenOneGift(FacebookGift gift)
	{
		if (!GiftOpenBlocked.Value)
		{
			OpenGift(gift);
		}
	}

	private void OpenGift(FacebookGift gift)
	{
		GiftConsuming.OnNext(gift);
		ConsumeGiftsForPlayer(gift);
	}

	private void ConsumeGiftsForPlayer(FacebookGift oneGift)
	{
		int counter = 0;
		bool consumeOk = true;
		List<FacebookGift> list = m_allGifts.FindAll((FacebookGift g) => g.FromId == oneGift.FromId);
		foreach (FacebookGift item in list)
		{
			counter++;
			PersistentSingleton<FacebookAPIService>.Instance.ConsumeGift(item.GiftId, delegate(IGraphResult res)
			{
				counter--;
				consumeOk &= WasGiftConsumed(res);
				if (counter == 0)
				{
					OpenGiftCallback(oneGift, consumeOk);
				}
			});
		}
	}

	private void OpenGiftCallback(FacebookGift gift, bool ok)
	{
		if (ok)
		{
			m_allGifts.RemoveAll((FacebookGift g) => g.FromId == gift.FromId);
			NumUnopenedGifts.Value = UniqueGifts().Count;
			GiftConsumeSuccess.OnNext(gift);
			if (m_allGifts.Count == 0)
			{
				GetAllRequests();
			}
		}
		else
		{
			GiftConsumeFailed.OnNext(gift);
		}
	}

	private bool WasGiftConsumed(IGraphResult result)
	{
		if (result == null && !result.Cancelled)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(result.Error))
		{
			return false;
		}
		if (string.IsNullOrEmpty(result.RawResult))
		{
			return false;
		}
		return (bool)result.ResultDictionary["success"];
	}
}
