using Spine.Unity;
using System.Linq;
using UnityEngine;

public abstract class AbstractCreatureController : MonoBehaviour
{
	public const float MinAnimSpeed = 0.9f;

	public const float MaxAnimSpeed = 1.1f;

	protected SkeletonAnimation m_animation;

	protected int m_heroIndex;

	protected int m_indexInCatetory;

	public HeroConfig Config
	{
		get;
		private set;
	}

	public abstract void Init(int heroIndex, CreatureStateEnum defaultState);

	protected void Init(int heroIndex)
	{
		m_animation = GetComponentInChildren<SkeletonAnimation>();
		m_heroIndex = heroIndex;
		Config = PersistentSingleton<Economies>.Instance.Heroes[heroIndex];
		m_indexInCatetory = (from h in PersistentSingleton<Economies>.Instance.Heroes
			where h.Category == Config.Category
			select h).ToList().FindIndex((HeroConfig h) => h.HeroIndex == Config.HeroIndex);
	}

	public Vector3 GetTargetPosition()
	{
		return GetBlockFinder().Get()?.Position().x0z() ?? GetEnterPosition();
	}

	public Vector3 GetEnterPosition()
	{
		int num = 9;
		float num2 = ((m_indexInCatetory & 1) != 0) ? (-0.6f) : 0.6f;
		int num3 = Mathf.FloorToInt(m_indexInCatetory / num);
		switch (Config.Category)
		{
		case HeroCategory.GroundRanged:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 0f, -8f - (float)num3 * 1.5f);
		case HeroCategory.AirMelee:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 4f, -6f - (float)num3 * 1.5f);
		case HeroCategory.AirRanged:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 4f, -8f - (float)num3 * 1.5f);
		default:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 0f, -6f - (float)num3 * 1.5f);
		}
	}

	public Vector3 GetBossPosition()
	{
		int num = 9;
		float num2 = ((m_indexInCatetory & 1) != 0) ? (-0.6f) : 0.6f;
		int num3 = Mathf.FloorToInt(m_indexInCatetory / num);
		switch (Config.Category)
		{
		case HeroCategory.GroundRanged:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 0f, -5f - (float)num3 * 1.5f);
		case HeroCategory.AirMelee:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 4f, -2f - (float)num3 * 1.5f);
		case HeroCategory.AirRanged:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 4f, -5f - (float)num3 * 1.5f);
		default:
			return BindingManager.Instance.CameraCtrl.transform.position + new Vector3((float)(m_indexInCatetory - num * num3) * num2, 0f, -2f - (float)num3 * 1.5f);
		}
	}

	public bool Collides(Vector3 pos)
	{
		return (pos - GetWantedPosition()).sqrMagnitude < 1f;
	}

	protected abstract Vector3 GetWantedPosition();

	protected abstract IBlockFinder GetBlockFinder();

	public abstract void Freeze();

	public abstract void UnFreeze();
}
