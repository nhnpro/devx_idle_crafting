using System.Collections;
using UnityEngine;

public class UIMapMoveJelly : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve m_movementCurve;

	[SerializeField]
	private float m_movementTime;

	private void Start()
	{
		base.gameObject.SetActive(PersistentSingleton<GameSettings>.Instance.MapJellyOn);
	}

	public void Move(Vector3 pos)
	{
		Transform transform = base.transform;
		Vector3 localPosition = base.transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = base.transform.localPosition;
		transform.localPosition = new Vector3(x, localPosition2.y, pos.z);
	}

	public void DecreaseJelly(Vector3 pos)
	{
		StartCoroutine(Movement(pos));
	}

	private IEnumerator Movement(Vector3 pos)
	{
		float movedFor = 0f;
		Vector3 origin = base.transform.localPosition;
		Vector3 localPosition = base.transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = base.transform.localPosition;
		Vector3 target = new Vector3(x, localPosition2.y, pos.z);
		while (movedFor <= m_movementTime)
		{
			movedFor += Time.deltaTime;
			float percent = Mathf.Clamp01(movedFor / m_movementTime);
			float curvePercent = m_movementCurve.Evaluate(percent);
			base.transform.localPosition = Vector3.Lerp(origin, target, curvePercent);
			yield return false;
		}
	}
}
