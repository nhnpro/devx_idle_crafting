using System;
using UnityEngine;

public class SlerpParticle : MonoBehaviour
{
	private enum MoveType
	{
		lerp,
		slerp
	}

	[SerializeField]
	private float RandomOffsetRadius = 0.3f;

	private Vector3 m_movefrom;

	private Vector3 m_moveto;

	private float m_invJourneyTime = 0.06451613f;

	private float m_startTime;

	private Animator m_hitAnimator;

	private string m_trigger;

	private MoveType moveType;

	private bool m_pooled;

	private Action m_hitAction;

	private static T GetRandomEnum<T>()
	{
		Array values = Enum.GetValues(typeof(T));
		return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
	}

	private void Update()
	{
		float num = (Time.time - m_startTime) * m_invJourneyTime;
		float t = num * num * num;
		switch (moveType)
		{
		case MoveType.lerp:
			base.transform.position = Vector3.Lerp(m_movefrom, m_moveto, t);
			break;
		case MoveType.slerp:
			base.transform.position = Vector3.Slerp(m_movefrom, m_moveto, t);
			break;
		default:
			base.transform.position = Vector3.Slerp(m_movefrom, m_moveto, t);
			break;
		}
		if (num >= 1f)
		{
			if (m_hitAnimator != null && m_hitAnimator.isInitialized)
			{
				m_hitAnimator.SetTrigger(m_trigger);
			}
			if (m_hitAction != null)
			{
				m_hitAction();
			}
			if (!m_pooled)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public void SlerpParticlesToUI(Vector3 f, Vector3 t, float time, Animator hitAnimator, string trigger, Camera uiCam, bool pooled = false, Action hitAction = null)
	{
		Vector3 f2 = uiCam.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(f));
		Vector3 t2 = t;
		f2.z = t2.z;
		SetupParticle(f2, t2, time, hitAnimator, trigger, pooled, hitAction);
	}

	public void SlerpParticlesToUI(Vector3 f, Vector3 t, float time, Animator hitAnimator, string trigger, bool pooled = false, Action hitAction = null)
	{
		Vector3 f2 = f;
		Vector3 t2 = t;
		f2.z = t2.z;
		SetupParticle(f2, t2, time, hitAnimator, trigger, pooled, hitAction);
	}

	public void SlerpParticlesToWorld(Vector3 f, Vector3 t, float time, Animator hitAnimator, string trigger, Camera uiCam, bool pooled = false, Action hitAction = null)
	{
		Vector3 f2 = uiCam.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(f));
		Vector3 t2 = uiCam.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(t));
		Vector3 position = uiCam.transform.position;
		f2.z = position.z;
		t2.z = f2.z;
		SetupParticle(f2, t2, time, hitAnimator, trigger, pooled, hitAction);
	}

	private void SetupParticle(Vector3 f, Vector3 t, float time, Animator hitAnimator, string trigger, bool pooled, Action hitAction)
	{
		m_movefrom = f;
		Vector2 vector = UnityEngine.Random.insideUnitCircle * RandomOffsetRadius;
		m_movefrom.x += vector.x;
		m_movefrom.y += vector.y;
		m_moveto = t;
		m_pooled = pooled;
		m_hitAnimator = hitAnimator;
		m_trigger = trigger;
		m_invJourneyTime = 1f / time;
		base.transform.position = m_movefrom;
		m_startTime = Time.time;
		moveType = GetRandomEnum<MoveType>();
		m_hitAction = hitAction;
		base.gameObject.SetActive(value: true);
	}
}
