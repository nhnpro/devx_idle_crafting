using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class VegetationController : Swipeable
{
	private Material m_mtl;

	private float m_amplitude;

	private const float Duration = 2f;

	protected void Start()
	{
		m_mtl = GetComponent<Renderer>().material;
	}

	protected void Update()
	{
		m_mtl.SetFloat("_Amplitude", m_amplitude);
		if (m_amplitude > 0f)
		{
			m_amplitude -= Time.deltaTime * 0.5f;
		}
	}

	protected override void OnSwipe()
	{
		m_amplitude = 1f;
	}
}
