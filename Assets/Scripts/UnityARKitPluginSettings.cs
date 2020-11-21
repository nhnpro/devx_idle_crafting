using UnityEngine;

[CreateAssetMenu(fileName = "ARKitSettings", menuName = "UnityARKitPlugin/Settings", order = 1)]
public class UnityARKitPluginSettings : ScriptableObject
{
	[Tooltip("Toggles whether Facetracking for iPhone X (and later) is used. If enabled, provide a Privacy Policy for submission to AppStore.")]
	public bool m_ARKitUsesFacetracking;

	[Tooltip("Toggles whether ARKit is required for this app: will make app only downloadable by devices with ARKit support if enabled.")]
	public bool AppRequiresARKit;
}
