using System;

namespace UnityEngine.XR.iOS
{
	[Flags]
	public enum ARHitTestResultType : long
	{
		ARHitTestResultTypeFeaturePoint = 0x1,
		ARHitTestResultTypeHorizontalPlane = 0x2,
		ARHitTestResultTypeVerticalPlane = 0x4,
		ARHitTestResultTypeExistingPlane = 0x8,
		ARHitTestResultTypeExistingPlaneUsingExtent = 0x10
	}
}
