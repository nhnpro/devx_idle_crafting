using UnityEngine;

public class SlerpTarget : MonoBehaviour
{
	[SerializeField]
	private Animator m_targetAnimator;

	[SerializeField]
	private SimplePoolManager m_pool;

	[SerializeField]
	private float m_time = 1f;

	[SerializeField]
	private float m_randomOffsetTime = 0.5f;

	[SerializeField]
	private string m_trigger = "Coinhit";

	public void SlerpFromHud(Vector3 fromWhere)
	{
		m_pool.GetPooledComponent<SlerpParticle>().SlerpParticlesToUI(fromWhere, base.transform.position, m_time + UnityEngine.Random.Range(0f, m_randomOffsetTime), m_targetAnimator, m_trigger, pooled: true);
	}

	public void SlerpFromWorld(Vector3 fromWhere)
	{
		m_pool.GetPooledComponent<SlerpParticle>().SlerpParticlesToUI(fromWhere, base.transform.position, m_time + UnityEngine.Random.Range(0f, m_randomOffsetTime), m_targetAnimator, m_trigger, BindingManager.Instance.UICamera, pooled: true);
	}

	public void SlerpFromWorldToWorld(Vector3 fromWhere)
	{
		m_pool.GetPooledComponent<SlerpParticle>().SlerpParticlesToWorld(fromWhere, base.transform.position, m_time + UnityEngine.Random.Range(0f, m_randomOffsetTime), m_targetAnimator, m_trigger, BindingManager.Instance.UICamera, pooled: true);
	}

	public GameObject SlerpFromWorldAndReturnParticle(Vector3 fromWhere)
	{
		SlerpParticle pooledComponent = m_pool.GetPooledComponent<SlerpParticle>();
		pooledComponent.SlerpParticlesToUI(fromWhere, base.transform.position, m_time + UnityEngine.Random.Range(0f, m_randomOffsetTime), m_targetAnimator, m_trigger, BindingManager.Instance.UICamera, pooled: true);
		return pooledComponent.gameObject;
	}
}
