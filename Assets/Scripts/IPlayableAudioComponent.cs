using UnityEngine;

internal interface IPlayableAudioComponent
{
	void Play(GameObject _parentGameObject);

	void Pause(GameObject _parentGameObject);

	void PauseAll();

	void Stop(GameObject _parentGameObject);

	void StopAll();
}
