using Big;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIDps : MonoBehaviour
{
	protected void Start()
	{
		Text textField = GetComponent<Text>();
		BigDouble[] previousHealth = new BigDouble[5]
		{
			BigDouble.ZERO,
			BigDouble.ZERO,
			BigDouble.ZERO,
			BigDouble.ZERO,
			BigDouble.ZERO
		};
		int current = 0;
		(from moving in Singleton<CameraMoveRunner>.Instance.IsCameraMoving
			where !moving
			select moving).DelayFrame(1).Subscribe(delegate
		{
			for (int i = 0; i < previousHealth.Length; i++)
			{
				previousHealth[i] = Singleton<ChunkRunner>.Instance.CurrentChunkHealth.Value;
			}
		}).AddTo(this);
		(from _ in TickerService.MasterTicks
			select BigDouble.Max(BigDouble.ZERO, previousHealth[current] - Singleton<ChunkRunner>.Instance.CurrentChunkHealth.Value) * 2L).Subscribe(delegate(BigDouble dps)
		{
			textField.text = BigString.ToString(dps);
			previousHealth[current] = Singleton<ChunkRunner>.Instance.CurrentChunkHealth.Value;
			if (current == previousHealth.Length - 1)
			{
				current = 0;
			}
			else
			{
				current++;
			}
		}).AddTo(this);
	}
}
