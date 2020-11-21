using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public class UnityRemoteVideo : MonoBehaviour
	{
		public ConnectToEditor connectToEditor;

		private UnityARSessionNativeInterface m_Session;

		private bool bTexturesInitialized;

		private int currentFrameIndex;

		private byte[] m_textureYBytes;

		private byte[] m_textureUVBytes;

		private byte[] m_textureYBytes2;

		private byte[] m_textureUVBytes2;

		private GCHandle m_pinnedYArray;

		private GCHandle m_pinnedUVArray;

		public void Start()
		{
			m_Session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateCamera;
			currentFrameIndex = 0;
			bTexturesInitialized = false;
		}

		private void UpdateCamera(UnityARCamera camera)
		{
			if (!bTexturesInitialized)
			{
				InitializeTextures(camera);
			}
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateCamera;
		}

		private void InitializeTextures(UnityARCamera camera)
		{
			int num = camera.videoParams.yWidth * camera.videoParams.yHeight;
			int num2 = camera.videoParams.yWidth * camera.videoParams.yHeight / 2;
			m_textureYBytes = new byte[num];
			m_textureUVBytes = new byte[num2];
			m_textureYBytes2 = new byte[num];
			m_textureUVBytes2 = new byte[num2];
			m_pinnedYArray = GCHandle.Alloc(m_textureYBytes);
			m_pinnedUVArray = GCHandle.Alloc(m_textureUVBytes);
			bTexturesInitialized = true;
		}

		private IntPtr PinByteArray(ref GCHandle handle, byte[] array)
		{
			handle.Free();
			handle = GCHandle.Alloc(array, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}

		private byte[] ByteArrayForFrame(int frame, byte[] array0, byte[] array1)
		{
			return (frame != 1) ? array0 : array1;
		}

		private byte[] YByteArrayForFrame(int frame)
		{
			return ByteArrayForFrame(frame, m_textureYBytes, m_textureYBytes2);
		}

		private byte[] UVByteArrayForFrame(int frame)
		{
			return ByteArrayForFrame(frame, m_textureUVBytes, m_textureUVBytes2);
		}

		private void OnDestroy()
		{
			m_Session.SetCapturePixelData(enable: false, IntPtr.Zero, IntPtr.Zero);
			m_pinnedYArray.Free();
			m_pinnedUVArray.Free();
		}

		public void OnPreRender()
		{
			ARTextureHandles aRVideoTextureHandles = m_Session.GetARVideoTextureHandles();
			if (!(aRVideoTextureHandles.textureY == IntPtr.Zero) && !(aRVideoTextureHandles.textureCbCr == IntPtr.Zero) && bTexturesInitialized)
			{
				currentFrameIndex = (currentFrameIndex + 1) % 2;
				Resolution currentResolution = Screen.currentResolution;
				m_Session.SetCapturePixelData(enable: true, PinByteArray(ref m_pinnedYArray, YByteArrayForFrame(currentFrameIndex)), PinByteArray(ref m_pinnedUVArray, UVByteArrayForFrame(currentFrameIndex)));
				connectToEditor.SendToEditor(ConnectionMessageIds.screenCaptureYMsgId, YByteArrayForFrame(1 - currentFrameIndex));
				connectToEditor.SendToEditor(ConnectionMessageIds.screenCaptureUVMsgId, UVByteArrayForFrame(1 - currentFrameIndex));
			}
		}
	}
}
