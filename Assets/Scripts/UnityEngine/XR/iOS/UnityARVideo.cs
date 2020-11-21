using System;
using UnityEngine.Rendering;

namespace UnityEngine.XR.iOS
{
	public class UnityARVideo : MonoBehaviour
	{
		public Material m_ClearMaterial;

		private CommandBuffer m_VideoCommandBuffer;

		private Texture2D _videoTextureY;

		private Texture2D _videoTextureCbCr;

		private Matrix4x4 _displayTransform;

		private bool bCommandBufferInitialized;

		public void Start()
		{
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateFrame;
			bCommandBufferInitialized = false;
		}

		private void UpdateFrame(UnityARCamera cam)
		{
			_displayTransform = default(Matrix4x4);
			_displayTransform.SetColumn(0, cam.displayTransform.column0);
			_displayTransform.SetColumn(1, cam.displayTransform.column1);
			_displayTransform.SetColumn(2, cam.displayTransform.column2);
			_displayTransform.SetColumn(3, cam.displayTransform.column3);
		}

		private void InitializeCommandBuffer()
		{
			m_VideoCommandBuffer = new CommandBuffer();
			m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, m_ClearMaterial);
			GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
			bCommandBufferInitialized = true;
		}

		private void OnDestroy()
		{
			if (m_VideoCommandBuffer != null)
			{
				GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
			}
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateFrame;
			bCommandBufferInitialized = false;
		}

		public void OnPreRender()
		{
			ARTextureHandles aRVideoTextureHandles = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetARVideoTextureHandles();
			if (!(aRVideoTextureHandles.textureY == IntPtr.Zero) && !(aRVideoTextureHandles.textureCbCr == IntPtr.Zero))
			{
				if (!bCommandBufferInitialized)
				{
					InitializeCommandBuffer();
				}
				Resolution currentResolution = Screen.currentResolution;
				if (_videoTextureY == null)
				{
					_videoTextureY = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height, TextureFormat.R8,  false, linear: false, aRVideoTextureHandles.textureY);
					_videoTextureY.filterMode = FilterMode.Bilinear;
					_videoTextureY.wrapMode = TextureWrapMode.Repeat;
					m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
				}
				if (_videoTextureCbCr == null)
				{
					_videoTextureCbCr = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height, TextureFormat.RG16,  false, linear: false, aRVideoTextureHandles.textureCbCr);
					_videoTextureCbCr.filterMode = FilterMode.Bilinear;
					_videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
					m_ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
				}
				_videoTextureY.UpdateExternalTexture(aRVideoTextureHandles.textureY);
				_videoTextureCbCr.UpdateExternalTexture(aRVideoTextureHandles.textureCbCr);
				m_ClearMaterial.SetMatrix("_DisplayTransform", _displayTransform);
			}
		}
	}
}
