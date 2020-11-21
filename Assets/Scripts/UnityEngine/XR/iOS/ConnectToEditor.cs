using System;
using UnityEngine.Networking.PlayerConnection;
using Utils;

namespace UnityEngine.XR.iOS
{
	public class ConnectToEditor : MonoBehaviour
	{
		private PlayerConnection playerConnection;

		private UnityARSessionNativeInterface m_session;

		private int editorID;

		private Texture2D frameBufferTex;

		private void Start()
		{
			UnityEngine.Debug.Log("STARTING ConnectToEditor");
			editorID = -1;
			playerConnection = PlayerConnection.instance;
			playerConnection.RegisterConnection(EditorConnected);
			playerConnection.RegisterDisconnection(EditorDisconnected);
			playerConnection.Register(ConnectionMessageIds.fromEditorARKitSessionMsgId, HandleEditorMessage);
			m_session = null;
		}

		private void OnGUI()
		{
			if (m_session == null)
			{
				GUI.Box(new Rect(Screen.width / 2 - 200, Screen.height / 2, 400f, 50f), "Waiting for editor connection...");
			}
		}

		private void HandleEditorMessage(MessageEventArgs mea)
		{
			serializableFromEditorMessage serializableFromEditorMessage = mea.data.Deserialize<serializableFromEditorMessage>();
			if (serializableFromEditorMessage != null && serializableFromEditorMessage.subMessageId == SubMessageIds.editorInitARKit)
			{
				InitializeARKit(serializableFromEditorMessage.arkitConfigMsg);
			}
		}

		private void InitializeARKit(serializableARKitInit sai)
		{
			Application.targetFrameRate = 60;
			m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
			ARKitWorldTrackingSessionConfiguration config = sai.config;
			UnityARSessionRunOption runOption = sai.runOption;
			m_session.RunWithConfigAndOptions(config, runOption);
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
			UnityARSessionNativeInterface.ARAnchorAddedEvent += ARAnchorAdded;
			UnityARSessionNativeInterface.ARAnchorUpdatedEvent += ARAnchorUpdated;
			UnityARSessionNativeInterface.ARAnchorRemovedEvent += ARAnchorRemoved;
		}

		public void ARFrameUpdated(UnityARCamera camera)
		{
			serializableUnityARCamera serializableObject = camera;
			SendToEditor(ConnectionMessageIds.updateCameraFrameMsgId, serializableObject);
		}

		public void ARAnchorAdded(ARPlaneAnchor planeAnchor)
		{
			serializableUnityARPlaneAnchor serializableObject = planeAnchor;
			SendToEditor(ConnectionMessageIds.addPlaneAnchorMsgeId, serializableObject);
		}

		public void ARAnchorUpdated(ARPlaneAnchor planeAnchor)
		{
			serializableUnityARPlaneAnchor serializableObject = planeAnchor;
			SendToEditor(ConnectionMessageIds.updatePlaneAnchorMsgeId, serializableObject);
		}

		public void ARAnchorRemoved(ARPlaneAnchor planeAnchor)
		{
			serializableUnityARPlaneAnchor serializableObject = planeAnchor;
			SendToEditor(ConnectionMessageIds.removePlaneAnchorMsgeId, serializableObject);
		}

		private void EditorConnected(int playerID)
		{
			UnityEngine.Debug.Log("connected");
			editorID = playerID;
		}

		private void EditorDisconnected(int playerID)
		{
			if (editorID == playerID)
			{
				editorID = -1;
			}
			DisconnectFromEditor();
			if (m_session != null)
			{
				m_session.Pause();
				m_session = null;
			}
		}

		public void SendToEditor(Guid msgId, object serializableObject)
		{
			byte[] data = serializableObject.SerializeToByteArray();
			SendToEditor(msgId, data);
		}

		public void SendToEditor(Guid msgId, byte[] data)
		{
			if (playerConnection.isConnected)
			{
				playerConnection.Send(msgId, data);
			}
		}

		public void DisconnectFromEditor()
		{
			playerConnection.DisconnectAll();
		}
	}
}
