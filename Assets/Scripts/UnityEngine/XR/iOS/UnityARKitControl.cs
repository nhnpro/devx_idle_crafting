namespace UnityEngine.XR.iOS
{
	public class UnityARKitControl : MonoBehaviour
	{
		private UnityARSessionRunOption[] runOptions = new UnityARSessionRunOption[4];

		private UnityARAlignment[] alignmentOptions = new UnityARAlignment[3];

		private UnityARPlaneDetection[] planeOptions = new UnityARPlaneDetection[4];

		private int currentOptionIndex;

		private int currentAlignmentIndex;

		private int currentPlaneIndex;

		private void Start()
		{
			runOptions[0] = (UnityARSessionRunOption)3;
			runOptions[1] = UnityARSessionRunOption.ARSessionRunOptionResetTracking;
			runOptions[2] = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors;
			runOptions[3] = (UnityARSessionRunOption)0;
			alignmentOptions[0] = UnityARAlignment.UnityARAlignmentCamera;
			alignmentOptions[1] = UnityARAlignment.UnityARAlignmentGravity;
			alignmentOptions[2] = UnityARAlignment.UnityARAlignmentGravityAndHeading;
			planeOptions[0] = UnityARPlaneDetection.Horizontal;
			planeOptions[1] = UnityARPlaneDetection.None;
		}

		private void Update()
		{
		}

		private void OnGUI()
		{
			if (GUI.Button(new Rect(100f, 100f, 200f, 50f), "Stop"))
			{
				UnityARSessionNativeInterface.GetARSessionNativeInterface().Pause();
			}
			if (GUI.Button(new Rect(300f, 100f, 200f, 50f), "Start"))
			{
				ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration(alignmentOptions[currentAlignmentIndex], planeOptions[currentPlaneIndex]);
				UnityARSessionNativeInterface.GetARSessionNativeInterface().RunWithConfigAndOptions(config, runOptions[currentOptionIndex]);
			}
			if (GUI.Button(new Rect(100f, 300f, 200f, 100f), "Start Non-WT Session"))
			{
				ARKitSessionConfiguration config2 = new ARKitSessionConfiguration(alignmentOptions[currentAlignmentIndex], getPointCloudData: true, enableLightEstimation: true);
				UnityARSessionNativeInterface.GetARSessionNativeInterface().RunWithConfig(config2);
			}
			string str = (currentOptionIndex == 0) ? "Full" : ((currentOptionIndex == 1) ? "Tracking" : ((currentOptionIndex != 2) ? "None" : "Anchors"));
			if (GUI.Button(new Rect(100f, 200f, 150f, 50f), "RunOption:" + str))
			{
				currentOptionIndex = (currentOptionIndex + 1) % 4;
			}
			string str2 = (currentAlignmentIndex == 0) ? "Camera" : ((currentAlignmentIndex != 1) ? "GravityAndHeading" : "Gravity");
			if (GUI.Button(new Rect(300f, 200f, 150f, 50f), "AlignOption:" + str2))
			{
				currentAlignmentIndex = (currentAlignmentIndex + 1) % 3;
			}
			string str3 = (currentPlaneIndex != 0) ? "None" : "Horizontal";
			if (GUI.Button(new Rect(500f, 200f, 150f, 50f), "PlaneOption:" + str3))
			{
				currentPlaneIndex = (currentPlaneIndex + 1) % 2;
			}
		}
	}
}
