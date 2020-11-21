using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProjectileController : MonoBehaviour
{
	private int m_heroIndex;

	private IBlock m_block;

	private Transform m_target;

	private float m_offset;

	[SerializeField]
	private GameObject m_spawn;

	[SerializeField]
	private GameObject m_move;

	[SerializeField]
	private GameObject m_death;

	public void Init(int heroIndex, IBlock block, float targetOffset)
	{
		m_heroIndex = heroIndex;
		m_block = block;
		m_offset = targetOffset;
		m_spawn.SetActive(value: true);
		m_move.SetActive(value: true);
		StartCoroutine(UpdateMove());
	}

	private IEnumerator UpdateMove()
	{
		while (true)
		{
			Vector3 target = m_block.Position() - base.transform.position + Vector3.back * m_offset * 0.5f;
			Vector3 dir = target.normalized;
			float distance = 10f * Time.deltaTime;
			Vector3 move = dir * distance;
			base.transform.position += move;
			if (distance * distance > target.sqrMagnitude)
			{
				break;
			}
			yield return null;
		}
		GetComponent<Animator>().SetTrigger("Hit");
		Singleton<DamageRunner>.Instance.HeroHit(m_heroIndex, m_block);
		m_move.SetActive(value: false);
		m_death.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(value: false);
	}
}
