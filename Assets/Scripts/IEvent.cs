using UnityEngine;

public interface IEvent
{
	string GetName();

	GameObject GetParentGameObject();

	int GetAudioEventAction();

	string GetAudioEventActionString();

	float GetAudioFloatParameter();
}
