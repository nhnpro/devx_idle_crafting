using UniRx;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CameraController : MonoBehaviour
{
	public const int CameraMotion = 30;

	private Animator m_animator;

	protected void Start()
	{
		m_animator = GetComponent<Animator>();
		Singleton<ChunkRunner>.Instance.MoveForward.Subscribe(delegate
		{
			MoveForward();
		}).AddTo(this);
		(from pair in Singleton<CameraMoveRunner>.Instance.IsCameraMoving.Pairwise()
			where !pair.Current && pair.Previous
			select pair).Subscribe(delegate
		{
			Vector3 position = base.transform.position;
			base.transform.position = new Vector3(0f, 0f, Mathf.RoundToInt(position.z));
		});
		Singleton<ChunkRunner>.Instance.ChapterCompleted.Subscribe(delegate
		{
			Reset();
		}).AddTo(this);
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigeStart
			select order).Subscribe(delegate
		{
			Prestige();
		}).AddTo(this);
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigeInit
			select order).Subscribe(delegate
		{
			Reset();
		}).AddTo(this);
	}

	private void MoveForward()
	{
		m_animator.SetTrigger("Forward");
	}

	private void Prestige()
	{
		m_animator.SetTrigger("PrestigeCamera");
	}

	private void Reset()
	{
		base.transform.position = Vector3.zero;
	}
}
