using System;
using UniRx;

public class CameraMoveRunner : Singleton<CameraMoveRunner>
{
	public const float MoveDuration = 1.5f;

	public ReactiveProperty<bool> IsCameraMoving;

	public CameraMoveRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		UniRx.IObservable<bool> observable = from _ in Singleton<ChunkRunner>.Instance.MoveForward.Delay(TimeSpan.FromSeconds(1.5)).TakeUntilDestroy(instance)
			select false;
		IsCameraMoving = Singleton<ChunkRunner>.Instance.MoveForward.Merge(observable).TakeUntilDestroy(instance).StartWith(value: false)
			.ToReactiveProperty();
		(from enter in Singleton<ChunkRunner>.Instance.BaseCampTrigger
			where enter
			select enter into _
			select Observable.Return<bool>(value: true).Delay(TimeSpan.FromSeconds(Singleton<PrestigeRunner>.Instance.PrestigeBasecampCameraDelay)).Take(1)).Switch().Subscribe(delegate
		{
			bind.CameraAnimator.SetTrigger("EnterBasecamp");
		}).AddTo(instance);
		(from pair in Singleton<ChunkRunner>.Instance.BaseCampTrigger.Pairwise()
			where !pair.Current && pair.Previous
			select pair).Subscribe(delegate
		{
			bind.CameraAnimator.SetTrigger("EnterAdventure");
		}).AddTo(instance);
		(from seq in Singleton<WorldRunner>.Instance.MapSequence.Pairwise()
			where !seq.Current && seq.Previous
			select seq).Subscribe(delegate
		{
			bind.CameraAnimator.SetTrigger("EnterAdventure");
			bind.EntryClouds.SetActive(value: true);
		}).AddTo(instance);
		(from active in Singleton<BossBattleRunner>.Instance.BossBattleActive.Pairwise()
			where active.Current && !active.Previous
			select active).Subscribe(delegate
		{
			bind.CameraAnimator.SetTrigger("BossEnter");
		}).AddTo(instance);
		Singleton<BossBattleRunner>.Instance.BossBattleResult.Skip(1).Subscribe(delegate
		{
			bind.CameraAnimator.SetTrigger("BossExit");
		}).AddTo(instance);
	}
}
