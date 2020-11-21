using UniRx;
using UnityEngine;

public class SceneStarted : MonoBehaviour
{
	protected void Start()
	{
		Observable.NextFrame().Subscribe(delegate
		{
		}).AddTo(this);
	}
}
