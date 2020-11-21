using UniRx;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	[SerializeField]
	private float m_intensity = 0.2f;

	[SerializeField]
	private float m_frequency = 10f;

	private float m_shakeTimer;

	private float WeightToDuration = 0.1f;

	protected void Start()
	{
		(from wave in Singleton<ChunkRunner>.Instance.BlastWaveTriggered
			where wave.Weight > 1.5f
			select wave).Subscribe(delegate(BlastWave wave)
		{
			DoShake(wave.Weight * WeightToDuration);
		}).AddTo(this);
	}

	protected void Update()
	{
		if (m_shakeTimer > 0f)
		{
			m_shakeTimer -= Time.deltaTime;
			base.transform.localPosition = new Vector3((Mathf.PerlinNoise(Time.time * m_frequency, 0f) * 2f - 1f) * m_intensity, (Mathf.PerlinNoise(0f, Time.time * m_frequency) * 2f - 1f) * m_intensity, 0f);
		}
	}

	public void DoShake(float duration)
	{
		m_shakeTimer = duration;
	}
}
