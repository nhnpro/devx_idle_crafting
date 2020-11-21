using System;
using System.Collections;
using UniRx;
using UnityEngine;

[Obsolete("Use class TreeSwinger")]
public class SpringConstraint : MonoBehaviour
{
	[SerializeField]
	private float m_weight = 1f;

	private Vector3 m_speed;

	public Vector3 Origin
	{
		get;
		private set;
	}

	public Vector3 Pos
	{
		get;
		private set;
	}

	protected void Start()
	{
		Singleton<ChunkRunner>.Instance.BlastWaveTriggered.Subscribe(delegate(BlastWave blast)
		{
			AddBlastWave(blast.Pos, blast.Weight);
		}).AddTo(this);
		Origin = base.transform.position;
		Pos = base.transform.position;
		m_speed = Vector3.zero;
	}

	protected void Update()
	{
		m_speed += 1f * (Origin - Pos);
		m_speed *= 0.95f;
		Pos += m_speed * Time.deltaTime * 1f;
		base.transform.position = Pos;
	}

	private void AddBlastWave(Vector3 pos, float weight)
	{
		StartCoroutine(DelayedBlastWave(pos, weight));
	}

	private IEnumerator DelayedBlastWave(Vector3 pos, float weight)
	{
		Vector3 dir = Origin - pos;
		float len = dir.magnitude;
		yield return new WaitForSeconds(len * 0.05f);
		float len_ = 1f / Mathf.Max(1f, len);
		dir.Normalize();
		float force = Mathf.Min(2f, len_ * weight * 5f / m_weight);
		m_speed += dir * force;
	}
}
