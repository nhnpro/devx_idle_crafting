using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteRunner : MonoBehaviour
{
	[SerializeField]
	private GameObject m_explosion;

	[SerializeField]
	private int m_delay;

	private List<BlockController> m_blocksInArea = new List<BlockController>();

	private BossBlockController m_boss;

	protected void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Block")
		{
			BlockController component = other.gameObject.GetComponent<BlockController>();
			if (component != null)
			{
				m_blocksInArea.Add(component);
			}
			else
			{
				m_boss = other.gameObject.GetComponent<BossBlockController>();
			}
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		if (other.tag == "Block")
		{
			BlockController component = other.gameObject.GetComponent<BlockController>();
			if (component != null)
			{
				m_blocksInArea.Remove(component);
			}
		}
	}

	protected void OnEnable()
	{
		m_explosion.SetActive(value: false);
		StartCoroutine(ExplodeRoutine());
	}

	protected IEnumerator ExplodeRoutine()
	{
		yield return new WaitForSeconds(m_delay);
		foreach (BlockController item in m_blocksInArea)
		{
			item.CauseDamage(Singleton<TapBoostRunner>.Instance.DynamiteCurrentDamage.Value);
		}
		if (m_boss != null)
		{
			m_boss.CauseDamage(Singleton<TapBoostRunner>.Instance.DynamiteCurrentDamage.Value);
		}
		m_blocksInArea.Clear();
		Vector3 cam = BindingManager.Instance.CameraCtrl.transform.position;
		Singleton<ChunkRunner>.Instance.CauseBlastWave(cam.x0z(), 5f);
		base.gameObject.SetActive(value: false);
		m_explosion.SetActive(value: true);
	}
}
