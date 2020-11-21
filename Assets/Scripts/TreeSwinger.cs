using System.Collections;
using UniRx;
using UnityEngine;

public class TreeSwinger : MonoBehaviour
{
	[SerializeField]
	private float m_weight = 1f;

	[SerializeField]
	private float m_height = 2f;

	private Vector3 m_particleOrigin;

	private Vector3 m_particlePos;

	private Vector3 m_speed;

	private Quaternion m_rot;

	private float m_invWeight;

	protected void Start()
	{
		Singleton<ChunkRunner>.Instance.BlastWaveTriggered.Subscribe(delegate(BlastWave blast)
		{
			AddBlastWave(blast.Pos, blast.Weight);
		}).AddTo(this);
		m_particleOrigin = base.transform.TransformPoint(new Vector3(0f, m_height, 0f));
		m_particlePos = m_particleOrigin;
		m_speed = Vector3.zero;
		m_rot = base.transform.rotation;
		m_invWeight = 1f / m_weight;
	}

	protected void Update()
	{
		m_speed += 1f * (m_particleOrigin - m_particlePos) * m_invWeight;
		m_speed *= 1f - 0.05f * m_invWeight;
		m_particlePos += m_speed * Time.deltaTime * 1f;
		Vector3 v = m_particleOrigin - base.transform.position;
		Vector3 v2 = m_particlePos - base.transform.position;
		AxisAngle axisAngle = MathExt.AngleBetween(v, v2);
		base.transform.rotation = Quaternion.AngleAxis(57.29578f * axisAngle.Angle, axisAngle.Axis) * m_rot;
	}

	private void AddBlastWave(Vector3 pos, float weight)
	{
		StartCoroutine(DelayedBlastWave(pos, weight));
	}

	private IEnumerator DelayedBlastWave(Vector3 pos, float weight)
	{
		Vector3 dir = m_particleOrigin - pos;
		float len = dir.magnitude;
		yield return new WaitForSeconds(len * 0.05f);
		float len_ = 1f / Mathf.Max(1f, len);
		dir.Normalize();
		float force = Mathf.Min(2f, len_ * 5f);
		m_speed += dir * force;
	}
}
