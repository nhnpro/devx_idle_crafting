using UniRx;
using UnityEngine;

public class HighDetailSetActive : MonoBehaviour
{
	public void Start()
	{
		(from q in Singleton<QualitySettingsRunner>.Instance.GraphicsDetail
			select q > 0).SubscribeToActive(base.gameObject).AddTo(this);
	}
}
