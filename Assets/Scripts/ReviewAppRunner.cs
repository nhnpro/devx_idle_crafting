using UniRx;
using UnityEngine;

[PropertyClass]
public class ReviewAppRunner : Singleton<ReviewAppRunner>
{
	public ReadOnlyReactiveProperty<bool> ReadyForReview;

	public UniRx.IObservable<bool> IsReviewPending;

	[PropertyBool]
	public ReactiveProperty<bool> ShowFeedbackPopup = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> ShowReviewPopup = new ReactiveProperty<bool>(initialValue: false);

	public ReviewAppRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		PlayerData instance2 = PlayerData.Instance;
		string version = Application.version;
		if (instance2.LastVersionReviewed.CompareTo(version) != 0)
		{
			if (instance2.ReviewState.Value != BaseData.ReviewStates.NeedsImprovement)
			{
				instance2.ReviewState.Value = BaseData.ReviewStates.Pending;
			}
			instance2.LastVersionReviewed = version;
		}
		IsReviewPending = from state in instance2.ReviewState
			select state == BaseData.ReviewStates.Pending;
		ReadyForReview = ConnectivityService.InternetConnectionAvailable.CombineLatest(IsReviewPending, (bool online, bool pending) => online && pending).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		(from show in (from pair in (from prestiges in PlayerData.Instance.LifetimePrestiges
					select (prestiges != 0) ? Observable.Never<int>() : PlayerData.Instance.MainChunk.AsObservable()).Switch().Pairwise()
				where pair.Previous == 35 && pair.Current == 36
				select pair).Take(1).CombineLatest(ReadyForReview, (Pair<int> unlock, bool ready) => ready)
			where show
			select show).Subscribe(delegate
		{
			ShowReivewPopup();
		}).AddTo(SceneLoader.Instance);
	}

	private void ShowReivewPopup()
	{
		PlayerData instance = PlayerData.Instance;
		instance.ReviewState.Value = BaseData.ReviewStates.Reviewed;
		instance.HasReviewed.Value = true;
		if (NativeReview.iOSNativeReviewSupported())
		{
			ShowFeedbackPopup.SetValueAndForceNotify(value: true);
			XPromoConfig cfg = PersistentSingleton<Economies>.Instance.XPromo.Find((XPromoConfig c) => c.ID == "CA");
			// XPromoPlugin.OpenReview(cfg);
		}
		else
		{
			ShowReviewPopup.SetValueAndForceNotify(value: true);
		}
	}

	public void GameNeedsImprovement()
	{
		PlayerData instance = PlayerData.Instance;
		instance.ReviewState.Value = BaseData.ReviewStates.NeedsImprovement;
		GotoFBPage();
	}

	public void GameWasLoved()
	{
		PlayerData instance = PlayerData.Instance;
		instance.ReviewState.Value = BaseData.ReviewStates.Reviewed;
		GotoFBPage();
	}

	public void GotoFAQPage()
	{
		// XPromoPlugin.OpenURL(PersistentSingleton<GameSettings>.Instance.FAQPage);
	}

	public void GoToPrivacyPolicyPage()
	{
		// XPromoPlugin.OpenURL(PersistentSingleton<GameSettings>.Instance.PrivacyPolicyPage);
	}

	private void GotoFBPage()
	{
		string url = "fb://page/" + PersistentSingleton<GameSettings>.Instance.FacebookPageId;
		if (FBAppStatus.isFBAppInstalled())
		{
			// XPromoPlugin.OpenURL(url);
		}
		else
		{
			// XPromoPlugin.OpenURL(PersistentSingleton<GameSettings>.Instance.FacebookPage);
		}
	}
}
