using Facebook.Unity;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FacebookAPIService : PersistentSingleton<FacebookAPIService>
{
	public delegate void MyDataDelegate(FBPlayer f, List<object> friends);

	public delegate void FriendDataDelegate(FBPlayer f);

	public Dictionary<string, FBPlayer> FBPlayers;

	public const string DefaultGiftData = "DefaultGiftData";

	public ReactiveProperty<bool> IsLoggedToFB = Observable.Never<bool>().ToReactiveProperty();

	private string meQueryString = "/me?fields=id,first_name,picture.width(120).height(120),friends.limit(100).fields(first_name,id,picture.width(120).height(120))";

	public void InitializeFB()
	{
		if (!base.Inited)
		{
			FacebookDataHandler.Instance().Load(this);
			if (!FB.IsInitialized)
			{
				FB.Init(OnInitComplete, OnHideUnity);
			}
			else
			{
				FB.ActivateApp();
			}
			base.Inited = true;
		}
	}

	private void OnInitComplete()
	{
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
			IsLoggedToFB.Value = FB.IsLoggedIn;
		}
		else
		{
			UnityEngine.Debug.LogWarning("FacebookAPIService Failed to Initialize the Facebook SDK");
		}
	}

	public void SaveFBData()
	{
		if (FBPlayers != null)
		{
			FacebookDataHandler.Instance().Save(this);
		}
	}

	public void Login(FacebookDelegate<ILoginResult> callback = null)
	{
		List<string> list = new List<string>();
		list.Add("public_profile");
		list.Add("user_friends");
		FB.LogInWithReadPermissions(list, delegate(ILoginResult result)
		{
			IsLoggedToFB.Value = FB.IsLoggedIn;
			if (callback != null)
			{
				callback(result);
			}
		});
	}

	public bool LogOut()
	{
		IsLoggedToFB.Value = false;
		if (FB.IsLoggedIn)
		{
			FB.LogOut();
			return true;
		}
		return false;
	}

	private void OnHideUnity(bool isGameShown)
	{
	}

	public void RequestMyFBData(MyDataDelegate callback = null)
	{
		LoadMyDataAPI(meQueryString, callback);
	}

	private void LoadMyDataAPI(string url, MyDataDelegate callback = null)
	{
		FB.API(meQueryString, HttpMethod.GET, delegate(IGraphResult result)
		{
			if (result.Error == null)
			{
				Dictionary<string, string> dictionary = GraphUtil.DeserializeJSONProfile(result.RawResult);
				List<object> friends = GraphUtil.DeserializeJSONFriends(result.RawResult);
				string id = "me";
				if (dictionary.ContainsKey("id"))
				{
					id = dictionary["id"];
				}
				string name = dictionary["first_name"];
				int value = PlayerData.Instance.LifetimeChunk.Value;
				FBPlayer f = new FBPlayer(id, name, playing: false, invited: false, value, 0L);
				string url2 = GraphUtil.DeserializePictureURL(result.ResultDictionary);
				LoadMyPicture(url2, f, friends, callback);
			}
		});
	}

	private void LoadMyPicture(string url, FBPlayer f, List<object> friends, MyDataDelegate callback = null)
	{
		GraphUtil.LoadImgFromURL(url, delegate(Texture pictureTexture)
		{
			if (pictureTexture == null)
			{
				LoadMyPicture(url, f, friends, callback);
			}
			else
			{
				f.ProfilePicture = (pictureTexture as Texture2D);
				if (callback != null)
				{
					callback(f, friends);
				}
			}
		});
	}

	/*public bool SendAppInvite(Uri inviteUrl, Uri imgUrl, FacebookDelegate<IAppInviteResult> callback = null)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		FB.Mobile.AppInvite(inviteUrl, imgUrl, delegate(IAppInviteResult result)
		{
			if (callback != null)
			{
				callback(result);
			}
		});
		return true;
	}*/

	public bool CallAppRequestAsFriendSelector(string message, string title, FacebookDelegate<IAppRequestResult> callback = null)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		string empty = string.Empty;
		FB.AppRequest(message, null, null, (!string.IsNullOrEmpty(empty)) ? empty.Split(',') : null, 0, null, title, delegate(IAppRequestResult result)
		{
			if (callback != null)
			{
				callback(result);
			}
		});
		return true;
	}

	public void LoadFriendAPI(string id, FriendDataDelegate callback = null)
	{
		string query = "/" + id + "?fields=id,first_name,picture.width(120).height(120)";
		FB.API(query, HttpMethod.GET, delegate(IGraphResult result)
		{
			if (result.Error == null)
			{
				object value = string.Empty;
				result.ResultDictionary.TryGetValue("first_name", out value);
				FBPlayer f = new FBPlayer(id, (string)value, playing: false, invited: false, 0, 0L);
				string url = GraphUtil.DeserializePictureURL(result.ResultDictionary);
				LoadFriendPicture(url, f, callback);
			}
		});
	}

	private void LoadFriendPicture(string url, FBPlayer f, FriendDataDelegate callback = null)
	{
		GraphUtil.LoadImgFromURL(url, delegate(Texture pictureTexture)
		{
			if (pictureTexture == null)
			{
				LoadFriendPicture(url, f, callback);
			}
			else
			{
				f.ProfilePicture = (pictureTexture as Texture2D);
				if (callback != null)
				{
					callback(f);
				}
			}
		});
	}

	public void ResetFacebookLogin()
	{
		IsLoggedToFB.UnpublishValue();
	}

	public void GetAllRequests(FacebookDelegate<IGraphResult> callback)
	{
		string query = "/me/apprequests?access_token=" + AccessToken.CurrentAccessToken.TokenString;
		FB.API(query, HttpMethod.GET, delegate(IGraphResult result)
		{
			if (callback != null)
			{
				callback(result);
			}
		});
	}

	public void GiftWithDirectRequest(List<string> to, string message, string title, FacebookDelegate<IAppRequestResult> callback)
	{
		FB.AppRequest(message, OGActionType.SEND, PersistentSingleton<GameSettings>.Instance.FacebookOGObjectId, to, "DefaultGiftData", title, delegate(IAppRequestResult result)
		{
			if (callback != null)
			{
				callback(result);
			}
		});
	}

	public void ConsumeGift(string requestObjectID, FacebookDelegate<IGraphResult> callback)
	{
		string query = "/" + requestObjectID + "?access_token=" + AccessToken.CurrentAccessToken.TokenString;
		FB.API(query, HttpMethod.DELETE, delegate(IGraphResult result)
		{
			if (callback != null)
			{
				callback(result);
			}
		});
	}
}
