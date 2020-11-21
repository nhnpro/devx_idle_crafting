using UnityEngine;

public class UIReview : MonoBehaviour
{
	public void OnShowReview()
	{
		XPromoConfig cfg = PersistentSingleton<Economies>.Instance.XPromo.Find((XPromoConfig c) => c.ID == "CA");
		// XPromoPlugin.OpenReview(cfg);
	}

	public void OnNeedsImprovement()
	{
		Singleton<ReviewAppRunner>.Instance.GameNeedsImprovement();
	}

	public void OnLoveIt()
	{
		Singleton<ReviewAppRunner>.Instance.GameWasLoved();
	}
}
