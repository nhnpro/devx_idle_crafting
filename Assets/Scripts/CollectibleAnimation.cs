using DG.Tweening;
using UnityEngine;

public class CollectibleAnimation : MonoBehaviour
{
	public Transform target;

	private void OnEnable()
	{
		Sequence s = DOTween.Sequence();
		s.Append(target.DOScaleX(1.6f, 0.2f));
		s.Join(target.DOLocalJump(new Vector3(UnityEngine.Random.Range(0f, 25f), 0f), 100f, 2, 0.35f));
		s.Join(target.DOScaleY(1.6f, 0.2f));
		s.Join(target.DOScaleZ(1.6f, 0.2f));
		s.Append(target.DOScaleX(1f, 0.15f));
		s.Join(target.DOScaleY(1f, 0.15f));
		s.Join(target.DOScaleZ(1f, 0.15f));
	}
}
