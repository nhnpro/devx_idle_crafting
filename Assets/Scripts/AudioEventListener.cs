using UnityEngine;

public class AudioEventListener : MonoBehaviour, IEventListener
{
	public string m_audioEventName = string.Empty;

	public bool m_broadcastToChildren;

	public bool m_overrideAudioEventAction;

	public AUDIOEVENTACTION m_newAudioEventAction = AUDIOEVENTACTION.Play;

	private AudioComponent[] m_audioComponents;

	private AudioRandomComponent[] m_audioRandomComponents;

	private AudioRandomSimpleComponent[] m_audioRandomSimpleComponents;

	private AudioAssetLoader[] m_audioAssetLoaders;

	private void OnEnable()
	{
		AudioController.Instance.RegisterListener(this, m_audioEventName);
		m_audioComponents = GetComponents<AudioComponent>();
		m_audioRandomComponents = GetComponents<AudioRandomComponent>();
		m_audioRandomSimpleComponents = GetComponents<AudioRandomSimpleComponent>();
		m_audioAssetLoaders = GetComponents<AudioAssetLoader>();
	}

	private void OnDisable()
	{
		AudioController.Instance.UnregisterListener(this, m_audioEventName);
	}

	bool IEventListener.HandleEvent(IEvent evt)
	{
		int audioEventAction = evt.GetAudioEventAction();
		AUDIOEVENTACTION aUDIOEVENTACTION = (AUDIOEVENTACTION)audioEventAction;
		GameObject parentGameObject = evt.GetParentGameObject();
		if (m_overrideAudioEventAction)
		{
			aUDIOEVENTACTION = m_newAudioEventAction;
		}
		switch (aUDIOEVENTACTION)
		{
		case AUDIOEVENTACTION.Play:
			Play(parentGameObject);
			break;
		case AUDIOEVENTACTION.Stop:
			Stop(parentGameObject);
			break;
		case AUDIOEVENTACTION.Pause:
			Pause(parentGameObject);
			break;
		case AUDIOEVENTACTION.LoadAsset:
			LoadAssets();
			break;
		case AUDIOEVENTACTION.UnLoadAsset:
			UnLoadAssets();
			break;
		case AUDIOEVENTACTION.StopAll:
			StopAll();
			break;
		case AUDIOEVENTACTION.PauseAll:
			PauseAll();
			break;
		}
		return true;
	}

	private void SetVolume(float volume, GameObject _parentGameObject)
	{
		if (m_audioComponents != null)
		{
			AudioComponent[] audioComponents = m_audioComponents;
			foreach (AudioComponent audioComponent in audioComponents)
			{
				audioComponent.SetVolume(volume, _parentGameObject);
			}
		}
		if (m_audioRandomComponents != null)
		{
			AudioRandomComponent[] audioRandomComponents = m_audioRandomComponents;
			foreach (AudioRandomComponent audioRandomComponent in audioRandomComponents)
			{
				audioRandomComponent.SetVolume(volume);
			}
		}
		if (m_audioRandomSimpleComponents != null)
		{
			AudioRandomSimpleComponent[] audioRandomSimpleComponents = m_audioRandomSimpleComponents;
			foreach (AudioRandomSimpleComponent audioRandomSimpleComponent in audioRandomSimpleComponents)
			{
				audioRandomSimpleComponent.SetVolume(volume);
			}
		}
	}

	private void SetPitch(float pitch, GameObject _parentGameObject)
	{
		if (m_audioComponents != null)
		{
			AudioComponent[] audioComponents = m_audioComponents;
			foreach (AudioComponent audioComponent in audioComponents)
			{
				audioComponent.SetPitch(pitch, _parentGameObject);
			}
		}
		if (m_audioRandomComponents != null)
		{
			AudioRandomComponent[] audioRandomComponents = m_audioRandomComponents;
			foreach (AudioRandomComponent audioRandomComponent in audioRandomComponents)
			{
				audioRandomComponent.SetPitch(pitch);
			}
		}
		if (m_audioRandomSimpleComponents != null)
		{
			AudioRandomSimpleComponent[] audioRandomSimpleComponents = m_audioRandomSimpleComponents;
			foreach (AudioRandomSimpleComponent audioRandomSimpleComponent in audioRandomSimpleComponents)
			{
				audioRandomSimpleComponent.SetPitch(pitch);
			}
		}
	}

	private void Play(GameObject _parentGameObject)
	{
		if (m_audioComponents != null)
		{
			AudioComponent[] audioComponents = m_audioComponents;
			foreach (AudioComponent audioComponent in audioComponents)
			{
				audioComponent.Play(_parentGameObject);
			}
		}
		if (m_audioRandomComponents != null)
		{
			AudioRandomComponent[] audioRandomComponents = m_audioRandomComponents;
			foreach (AudioRandomComponent audioRandomComponent in audioRandomComponents)
			{
				audioRandomComponent.Play(_parentGameObject);
			}
		}
		if (m_audioRandomSimpleComponents != null)
		{
			AudioRandomSimpleComponent[] audioRandomSimpleComponents = m_audioRandomSimpleComponents;
			foreach (AudioRandomSimpleComponent audioRandomSimpleComponent in audioRandomSimpleComponents)
			{
				audioRandomSimpleComponent.Play(_parentGameObject);
			}
		}
		if (m_broadcastToChildren)
		{
			BroadcastMessage("Play", _parentGameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Stop(GameObject _parentGameObject)
	{
		if (m_audioComponents != null)
		{
			AudioComponent[] audioComponents = m_audioComponents;
			foreach (AudioComponent audioComponent in audioComponents)
			{
				audioComponent.Stop(_parentGameObject);
			}
		}
		if (m_audioComponents != null)
		{
			AudioRandomComponent[] audioRandomComponents = m_audioRandomComponents;
			foreach (AudioRandomComponent audioRandomComponent in audioRandomComponents)
			{
				audioRandomComponent.Stop(_parentGameObject);
			}
		}
		if (m_audioRandomSimpleComponents != null)
		{
			AudioRandomSimpleComponent[] audioRandomSimpleComponents = m_audioRandomSimpleComponents;
			foreach (AudioRandomSimpleComponent audioRandomSimpleComponent in audioRandomSimpleComponents)
			{
				audioRandomSimpleComponent.Stop(_parentGameObject);
			}
		}
		if (m_broadcastToChildren)
		{
			BroadcastMessage("Stop", _parentGameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void StopAll()
	{
		if (m_audioComponents != null)
		{
			AudioComponent[] audioComponents = m_audioComponents;
			foreach (AudioComponent audioComponent in audioComponents)
			{
				audioComponent.StopAll();
			}
		}
		if (m_audioRandomSimpleComponents != null)
		{
			AudioRandomSimpleComponent[] audioRandomSimpleComponents = m_audioRandomSimpleComponents;
			foreach (AudioRandomSimpleComponent audioRandomSimpleComponent in audioRandomSimpleComponents)
			{
				audioRandomSimpleComponent.StopAll();
			}
		}
		if (m_broadcastToChildren)
		{
			BroadcastMessage("StopAll", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Pause(GameObject _parentGameObject)
	{
		if (m_audioComponents != null)
		{
			AudioComponent[] audioComponents = m_audioComponents;
			foreach (AudioComponent audioComponent in audioComponents)
			{
				audioComponent.Pause(_parentGameObject);
			}
		}
		if (m_audioRandomSimpleComponents != null)
		{
			AudioRandomSimpleComponent[] audioRandomSimpleComponents = m_audioRandomSimpleComponents;
			foreach (AudioRandomSimpleComponent audioRandomSimpleComponent in audioRandomSimpleComponents)
			{
				audioRandomSimpleComponent.Pause(_parentGameObject);
			}
		}
		if (m_broadcastToChildren)
		{
			BroadcastMessage("Pause", _parentGameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void PauseAll()
	{
		if (m_audioComponents != null)
		{
			AudioComponent[] audioComponents = m_audioComponents;
			foreach (AudioComponent audioComponent in audioComponents)
			{
				audioComponent.PauseAll();
			}
		}
		if (m_audioRandomSimpleComponents != null)
		{
			AudioRandomSimpleComponent[] audioRandomSimpleComponents = m_audioRandomSimpleComponents;
			foreach (AudioRandomSimpleComponent audioRandomSimpleComponent in audioRandomSimpleComponents)
			{
				audioRandomSimpleComponent.PauseAll();
			}
		}
		if (m_broadcastToChildren)
		{
			BroadcastMessage("PauseAll", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void LoadAssets()
	{
		if (m_audioAssetLoaders != null)
		{
			AudioAssetLoader[] audioAssetLoaders = m_audioAssetLoaders;
			foreach (AudioAssetLoader audioAssetLoader in audioAssetLoaders)
			{
				audioAssetLoader.LoadAssets();
			}
		}
		if (m_broadcastToChildren)
		{
			BroadcastMessage("LoadAssets", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void UnLoadAssets()
	{
		if (m_audioAssetLoaders != null)
		{
			AudioAssetLoader[] audioAssetLoaders = m_audioAssetLoaders;
			foreach (AudioAssetLoader audioAssetLoader in audioAssetLoaders)
			{
				audioAssetLoader.UnLoadAssets();
			}
		}
		if (m_broadcastToChildren)
		{
			BroadcastMessage("UnLoadAssets", SendMessageOptions.DontRequireReceiver);
		}
	}
}
