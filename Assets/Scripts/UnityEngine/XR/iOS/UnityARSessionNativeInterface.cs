using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public class UnityARSessionNativeInterface
	{
		public delegate void ARFrameUpdate(UnityARCamera camera);

		public delegate void ARAnchorAdded(ARPlaneAnchor anchorData);

		public delegate void ARAnchorUpdated(ARPlaneAnchor anchorData);

		public delegate void ARAnchorRemoved(ARPlaneAnchor anchorData);

		public delegate void ARUserAnchorAdded(ARUserAnchor anchorData);

		public delegate void ARUserAnchorUpdated(ARUserAnchor anchorData);

		public delegate void ARUserAnchorRemoved(ARUserAnchor anchorData);

		public delegate void ARFaceAnchorAdded(ARFaceAnchor anchorData);

		public delegate void ARFaceAnchorUpdated(ARFaceAnchor anchorData);

		public delegate void ARFaceAnchorRemoved(ARFaceAnchor anchorData);

		public delegate void ARSessionFailed(string error);

		public delegate void ARSessionCallback();

		public delegate void ARSessionTrackingChanged(UnityARCamera camera);

		private delegate void internal_ARFrameUpdate(internal_UnityARCamera camera);

		public delegate void internal_ARAnchorAdded(UnityARAnchorData anchorData);

		public delegate void internal_ARAnchorUpdated(UnityARAnchorData anchorData);

		public delegate void internal_ARAnchorRemoved(UnityARAnchorData anchorData);

		public delegate void internal_ARUserAnchorAdded(UnityARUserAnchorData anchorData);

		public delegate void internal_ARUserAnchorUpdated(UnityARUserAnchorData anchorData);

		public delegate void internal_ARUserAnchorRemoved(UnityARUserAnchorData anchorData);

		public delegate void internal_ARFaceAnchorAdded(UnityARFaceAnchorData anchorData);

		public delegate void internal_ARFaceAnchorUpdated(UnityARFaceAnchorData anchorData);

		public delegate void internal_ARFaceAnchorRemoved(UnityARFaceAnchorData anchorData);

		private delegate void internal_ARSessionTrackingChanged(internal_UnityARCamera camera);

		private IntPtr m_NativeARSession;

		private static UnityARCamera s_Camera;

		private static UnityARSessionNativeInterface s_UnityARSessionNativeInterface;

		[CompilerGenerated]
		private static internal_ARFrameUpdate _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static ARSessionFailed _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static ARSessionCallback _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static ARSessionCallback _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static internal_ARSessionTrackingChanged _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static internal_ARAnchorAdded _003C_003Ef__mg_0024cache5;

		[CompilerGenerated]
		private static internal_ARAnchorUpdated _003C_003Ef__mg_0024cache6;

		[CompilerGenerated]
		private static internal_ARAnchorRemoved _003C_003Ef__mg_0024cache7;

		[CompilerGenerated]
		private static internal_ARUserAnchorAdded _003C_003Ef__mg_0024cache8;

		[CompilerGenerated]
		private static internal_ARUserAnchorUpdated _003C_003Ef__mg_0024cache9;

		[CompilerGenerated]
		private static internal_ARUserAnchorRemoved _003C_003Ef__mg_0024cacheA;

		[CompilerGenerated]
		private static internal_ARFaceAnchorAdded _003C_003Ef__mg_0024cacheB;

		[CompilerGenerated]
		private static internal_ARFaceAnchorUpdated _003C_003Ef__mg_0024cacheC;

		[CompilerGenerated]
		private static internal_ARFaceAnchorRemoved _003C_003Ef__mg_0024cacheD;

		public static event ARFrameUpdate ARFrameUpdatedEvent;

		public static event ARAnchorAdded ARAnchorAddedEvent;

		public static event ARAnchorUpdated ARAnchorUpdatedEvent;

		public static event ARAnchorRemoved ARAnchorRemovedEvent;

		public static event ARUserAnchorAdded ARUserAnchorAddedEvent;

		public static event ARUserAnchorUpdated ARUserAnchorUpdatedEvent;

		public static event ARUserAnchorRemoved ARUserAnchorRemovedEvent;

		public static event ARFaceAnchorAdded ARFaceAnchorAddedEvent;

		public static event ARFaceAnchorUpdated ARFaceAnchorUpdatedEvent;

		public static event ARFaceAnchorRemoved ARFaceAnchorRemovedEvent;

		public static event ARSessionFailed ARSessionFailedEvent;

		public static event ARSessionCallback ARSessionInterruptedEvent;

		public static event ARSessionCallback ARSessioninterruptionEndedEvent;

		public static event ARSessionTrackingChanged ARSessionTrackingChangedEvent;

		public UnityARSessionNativeInterface()
		{
			m_NativeARSession = unity_CreateNativeARSession();
			session_SetSessionCallbacks(m_NativeARSession, _frame_update, _ar_session_failed, _ar_session_interrupted, _ar_session_interruption_ended, _ar_tracking_changed);
			session_SetPlaneAnchorCallbacks(m_NativeARSession, _anchor_added, _anchor_updated, _anchor_removed);
			session_SetUserAnchorCallbacks(m_NativeARSession, _user_anchor_added, _user_anchor_updated, _user_anchor_removed);
			session_SetFaceAnchorCallbacks(m_NativeARSession, _face_anchor_added, _face_anchor_updated, _face_anchor_removed);
		}

		[DllImport("__Internal")]
		private static extern IntPtr unity_CreateNativeARSession();

		[DllImport("__Internal")]
		private static extern void session_SetSessionCallbacks(IntPtr nativeSession, internal_ARFrameUpdate frameCallback, ARSessionFailed sessionFailed, ARSessionCallback sessionInterrupted, ARSessionCallback sessionInterruptionEnded, internal_ARSessionTrackingChanged trackingChanged);

		[DllImport("__Internal")]
		private static extern void session_SetPlaneAnchorCallbacks(IntPtr nativeSession, internal_ARAnchorAdded anchorAddedCallback, internal_ARAnchorUpdated anchorUpdatedCallback, internal_ARAnchorRemoved anchorRemovedCallback);

		[DllImport("__Internal")]
		private static extern void session_SetUserAnchorCallbacks(IntPtr nativeSession, internal_ARUserAnchorAdded userAnchorAddedCallback, internal_ARUserAnchorUpdated userAnchorUpdatedCallback, internal_ARUserAnchorRemoved userAnchorRemovedCallback);

		[DllImport("__Internal")]
		private static extern void session_SetFaceAnchorCallbacks(IntPtr nativeSession, internal_ARFaceAnchorAdded faceAnchorAddedCallback, internal_ARFaceAnchorUpdated faceAnchorUpdatedCallback, internal_ARFaceAnchorRemoved faceAnchorRemovedCallback);

		[DllImport("__Internal")]
		private static extern void StartWorldTrackingSession(IntPtr nativeSession, ARKitWorldTrackingSessionConfiguration configuration);

		[DllImport("__Internal")]
		private static extern void StartWorldTrackingSessionWithOptions(IntPtr nativeSession, ARKitWorldTrackingSessionConfiguration configuration, UnityARSessionRunOption runOptions);

		[DllImport("__Internal")]
		private static extern void StartSession(IntPtr nativeSession, ARKitSessionConfiguration configuration);

		[DllImport("__Internal")]
		private static extern void StartSessionWithOptions(IntPtr nativeSession, ARKitSessionConfiguration configuration, UnityARSessionRunOption runOptions);

		[DllImport("__Internal")]
		private static extern void StartFaceTrackingSession(IntPtr nativeSession, ARKitFaceTrackingConfiguration configuration);

		[DllImport("__Internal")]
		private static extern void StartFaceTrackingSessionWithOptions(IntPtr nativeSession, ARKitFaceTrackingConfiguration configuration, UnityARSessionRunOption runOptions);

		[DllImport("__Internal")]
		private static extern void PauseSession(IntPtr nativeSession);

		[DllImport("__Internal")]
		private static extern int HitTest(IntPtr nativeSession, ARPoint point, ARHitTestResultType types);

		[DllImport("__Internal")]
		private static extern UnityARHitTestResult GetLastHitTestResult(int index);

		[DllImport("__Internal")]
		private static extern ARTextureHandles GetVideoTextureHandles();

		[DllImport("__Internal")]
		private static extern float GetAmbientIntensity();

		[DllImport("__Internal")]
		private static extern int GetTrackingQuality();

		[DllImport("__Internal")]
		private static extern bool GetARPointCloud(ref IntPtr verts, ref uint vertLength);

		[DllImport("__Internal")]
		private static extern void SetCameraNearFar(float nearZ, float farZ);

		[DllImport("__Internal")]
		private static extern void CapturePixelData(int enable, IntPtr pYPixelBytes, IntPtr pUVPixelBytes);

		[DllImport("__Internal")]
		private static extern UnityARUserAnchorData SessionAddUserAnchor(IntPtr nativeSession, UnityARUserAnchorData anchorData);

		[DllImport("__Internal")]
		private static extern void SessionRemoveUserAnchor(IntPtr nativeSession, [MarshalAs(UnmanagedType.LPStr)] string anchorIdentifier);

		public static UnityARSessionNativeInterface GetARSessionNativeInterface()
		{
			if (s_UnityARSessionNativeInterface == null)
			{
				s_UnityARSessionNativeInterface = new UnityARSessionNativeInterface();
			}
			return s_UnityARSessionNativeInterface;
		}

		public Matrix4x4 GetCameraPose()
		{
			Matrix4x4 result = default(Matrix4x4);
			result.SetColumn(0, s_Camera.worldTransform.column0);
			result.SetColumn(1, s_Camera.worldTransform.column1);
			result.SetColumn(2, s_Camera.worldTransform.column2);
			result.SetColumn(3, s_Camera.worldTransform.column3);
			return result;
		}

		public Matrix4x4 GetCameraProjection()
		{
			Matrix4x4 result = default(Matrix4x4);
			result.SetColumn(0, s_Camera.projectionMatrix.column0);
			result.SetColumn(1, s_Camera.projectionMatrix.column1);
			result.SetColumn(2, s_Camera.projectionMatrix.column2);
			result.SetColumn(3, s_Camera.projectionMatrix.column3);
			return result;
		}

		public void SetCameraClipPlanes(float nearZ, float farZ)
		{
			SetCameraNearFar(nearZ, farZ);
		}

		public void SetCapturePixelData(bool enable, IntPtr pYByteArray, IntPtr pUVByteArray)
		{
			int enable2 = enable ? 1 : 0;
			CapturePixelData(enable2, pYByteArray, pUVByteArray);
		}

		[MonoPInvokeCallback(typeof(internal_ARFrameUpdate))]
		private static void _frame_update(internal_UnityARCamera camera)
		{
			UnityARCamera unityARCamera = default(UnityARCamera);
			unityARCamera.projectionMatrix = camera.projectionMatrix;
			unityARCamera.worldTransform = camera.worldTransform;
			unityARCamera.trackingState = camera.trackingState;
			unityARCamera.trackingReason = camera.trackingReason;
			unityARCamera.videoParams = camera.videoParams;
			unityARCamera.lightData = camera.lightData;
			unityARCamera.displayTransform = camera.displayTransform;
			s_Camera = unityARCamera;
			if (camera.getPointCloudData == 1)
			{
				UpdatePointCloudData(ref s_Camera);
			}
			if (UnityARSessionNativeInterface.ARFrameUpdatedEvent != null)
			{
				UnityARSessionNativeInterface.ARFrameUpdatedEvent(s_Camera);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARSessionTrackingChanged))]
		private static void _ar_tracking_changed(internal_UnityARCamera camera)
		{
			s_Camera.trackingState = camera.trackingState;
			s_Camera.trackingReason = camera.trackingReason;
			if (UnityARSessionNativeInterface.ARSessionTrackingChangedEvent != null)
			{
				UnityARSessionNativeInterface.ARSessionTrackingChangedEvent(s_Camera);
			}
		}

		private static void UpdatePointCloudData(ref UnityARCamera camera)
		{
			IntPtr verts = IntPtr.Zero;
			uint vertLength = 0u;
			bool aRPointCloud = GetARPointCloud(ref verts, ref vertLength);
			float[] array = null;
			if (aRPointCloud)
			{
				array = new float[vertLength];
				Marshal.Copy(verts, array, 0, (int)vertLength);
				Vector3[] array2 = new Vector3[vertLength / 4u];
				for (int i = 0; i < vertLength; i++)
				{
					array2[i / 4].x = array[i++];
					array2[i / 4].y = array[i++];
					array2[i / 4].z = 0f - array[i++];
				}
				camera.pointCloudData = array2;
			}
		}

		private static ARPlaneAnchor GetPlaneAnchorFromAnchorData(UnityARAnchorData anchor)
		{
			ARPlaneAnchor result = default(ARPlaneAnchor);
			result.identifier = Marshal.PtrToStringAuto(anchor.ptrIdentifier);
			Matrix4x4 transform = default(Matrix4x4);
			transform.SetColumn(0, anchor.transform.column0);
			transform.SetColumn(1, anchor.transform.column1);
			transform.SetColumn(2, anchor.transform.column2);
			transform.SetColumn(3, anchor.transform.column3);
			result.transform = transform;
			result.alignment = anchor.alignment;
			result.center = new Vector3(anchor.center.x, anchor.center.y, anchor.center.z);
			result.extent = new Vector3(anchor.extent.x, anchor.extent.y, anchor.extent.z);
			return result;
		}

		private static ARUserAnchor GetUserAnchorFromAnchorData(UnityARUserAnchorData anchor)
		{
			ARUserAnchor result = default(ARUserAnchor);
			result.identifier = Marshal.PtrToStringAuto(anchor.ptrIdentifier);
			Matrix4x4 transform = default(Matrix4x4);
			transform.SetColumn(0, anchor.transform.column0);
			transform.SetColumn(1, anchor.transform.column1);
			transform.SetColumn(2, anchor.transform.column2);
			transform.SetColumn(3, anchor.transform.column3);
			result.transform = transform;
			return result;
		}

		private static ARHitTestResult GetHitTestResultFromResultData(UnityARHitTestResult resultData)
		{
			ARHitTestResult result = default(ARHitTestResult);
			result.type = resultData.type;
			result.distance = resultData.distance;
			result.localTransform = resultData.localTransform;
			result.worldTransform = resultData.worldTransform;
			result.isValid = resultData.isValid;
			if (resultData.anchor != IntPtr.Zero)
			{
				result.anchorIdentifier = Marshal.PtrToStringAuto(resultData.anchor);
			}
			return result;
		}

		[MonoPInvokeCallback(typeof(internal_ARAnchorAdded))]
		private static void _anchor_added(UnityARAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARAnchorAddedEvent != null)
			{
				ARPlaneAnchor planeAnchorFromAnchorData = GetPlaneAnchorFromAnchorData(anchor);
				UnityARSessionNativeInterface.ARAnchorAddedEvent(planeAnchorFromAnchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARAnchorUpdated))]
		private static void _anchor_updated(UnityARAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARAnchorUpdatedEvent != null)
			{
				ARPlaneAnchor planeAnchorFromAnchorData = GetPlaneAnchorFromAnchorData(anchor);
				UnityARSessionNativeInterface.ARAnchorUpdatedEvent(planeAnchorFromAnchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARAnchorRemoved))]
		private static void _anchor_removed(UnityARAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARAnchorRemovedEvent != null)
			{
				ARPlaneAnchor planeAnchorFromAnchorData = GetPlaneAnchorFromAnchorData(anchor);
				UnityARSessionNativeInterface.ARAnchorRemovedEvent(planeAnchorFromAnchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARUserAnchorAdded))]
		private static void _user_anchor_added(UnityARUserAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARUserAnchorAddedEvent != null)
			{
				ARUserAnchor userAnchorFromAnchorData = GetUserAnchorFromAnchorData(anchor);
				UnityARSessionNativeInterface.ARUserAnchorAddedEvent(userAnchorFromAnchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARUserAnchorUpdated))]
		private static void _user_anchor_updated(UnityARUserAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent != null)
			{
				ARUserAnchor userAnchorFromAnchorData = GetUserAnchorFromAnchorData(anchor);
				UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent(userAnchorFromAnchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARUserAnchorRemoved))]
		private static void _user_anchor_removed(UnityARUserAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARUserAnchorRemovedEvent != null)
			{
				ARUserAnchor userAnchorFromAnchorData = GetUserAnchorFromAnchorData(anchor);
				UnityARSessionNativeInterface.ARUserAnchorRemovedEvent(userAnchorFromAnchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARFaceAnchorAdded))]
		private static void _face_anchor_added(UnityARFaceAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARFaceAnchorAddedEvent != null)
			{
				ARFaceAnchor anchorData = new ARFaceAnchor(anchor);
				UnityARSessionNativeInterface.ARFaceAnchorAddedEvent(anchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARFaceAnchorUpdated))]
		private static void _face_anchor_updated(UnityARFaceAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent != null)
			{
				ARFaceAnchor anchorData = new ARFaceAnchor(anchor);
				UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent(anchorData);
			}
		}

		[MonoPInvokeCallback(typeof(internal_ARFaceAnchorRemoved))]
		private static void _face_anchor_removed(UnityARFaceAnchorData anchor)
		{
			if (UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent != null)
			{
				ARFaceAnchor anchorData = new ARFaceAnchor(anchor);
				UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent(anchorData);
			}
		}

		[MonoPInvokeCallback(typeof(ARSessionFailed))]
		private static void _ar_session_failed(string error)
		{
			if (UnityARSessionNativeInterface.ARSessionFailedEvent != null)
			{
				UnityARSessionNativeInterface.ARSessionFailedEvent(error);
			}
		}

		[MonoPInvokeCallback(typeof(ARSessionCallback))]
		private static void _ar_session_interrupted()
		{
			UnityEngine.Debug.Log("ar_session_interrupted");
			if (UnityARSessionNativeInterface.ARSessionInterruptedEvent != null)
			{
				UnityARSessionNativeInterface.ARSessionInterruptedEvent();
			}
		}

		[MonoPInvokeCallback(typeof(ARSessionCallback))]
		private static void _ar_session_interruption_ended()
		{
			UnityEngine.Debug.Log("ar_session_interruption_ended");
			if (UnityARSessionNativeInterface.ARSessioninterruptionEndedEvent != null)
			{
				UnityARSessionNativeInterface.ARSessioninterruptionEndedEvent();
			}
		}

		public void RunWithConfigAndOptions(ARKitWorldTrackingSessionConfiguration config, UnityARSessionRunOption runOptions)
		{
			StartWorldTrackingSessionWithOptions(m_NativeARSession, config, runOptions);
		}

		public void RunWithConfig(ARKitWorldTrackingSessionConfiguration config)
		{
			StartWorldTrackingSession(m_NativeARSession, config);
		}

		public void Run()
		{
			RunWithConfig(new ARKitWorldTrackingSessionConfiguration(UnityARAlignment.UnityARAlignmentGravity, UnityARPlaneDetection.Horizontal, getPointCloudData: false, enableLightEstimation: false));
		}

		public void RunWithConfigAndOptions(ARKitSessionConfiguration config, UnityARSessionRunOption runOptions)
		{
			StartSessionWithOptions(m_NativeARSession, config, runOptions);
		}

		public void RunWithConfig(ARKitSessionConfiguration config)
		{
			StartSession(m_NativeARSession, config);
		}

		public void RunWithConfigAndOptions(ARKitFaceTrackingConfiguration config, UnityARSessionRunOption runOptions)
		{
			StartFaceTrackingSessionWithOptions(m_NativeARSession, config, runOptions);
		}

		public void RunWithConfig(ARKitFaceTrackingConfiguration config)
		{
			StartFaceTrackingSession(m_NativeARSession, config);
		}

		public void Pause()
		{
			PauseSession(m_NativeARSession);
		}

		public List<ARHitTestResult> HitTest(ARPoint point, ARHitTestResultType types)
		{
			int num = HitTest(m_NativeARSession, point, types);
			List<ARHitTestResult> list = new List<ARHitTestResult>();
			for (int i = 0; i < num; i++)
			{
				UnityARHitTestResult lastHitTestResult = GetLastHitTestResult(i);
				list.Add(GetHitTestResultFromResultData(lastHitTestResult));
			}
			return list;
		}

		public ARTextureHandles GetARVideoTextureHandles()
		{
			return GetVideoTextureHandles();
		}

		[Obsolete("Hook ARFrameUpdatedEvent instead and get UnityARCamera.ambientIntensity")]
		public float GetARAmbientIntensity()
		{
			return GetAmbientIntensity();
		}

		[Obsolete("Hook ARFrameUpdatedEvent instead and get UnityARCamera.trackingState")]
		public int GetARTrackingQuality()
		{
			return GetTrackingQuality();
		}

		public UnityARUserAnchorData AddUserAnchor(UnityARUserAnchorData anchorData)
		{
			return SessionAddUserAnchor(m_NativeARSession, anchorData);
		}

		public UnityARUserAnchorData AddUserAnchorFromGameObject(GameObject go)
		{
			return AddUserAnchor(UnityARUserAnchorData.UnityARUserAnchorDataFromGameObject(go));
		}

		public void RemoveUserAnchor(string anchorIdentifier)
		{
			SessionRemoveUserAnchor(m_NativeARSession, anchorIdentifier);
		}
	}
}
