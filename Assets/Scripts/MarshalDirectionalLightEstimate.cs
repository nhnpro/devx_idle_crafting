using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public struct MarshalDirectionalLightEstimate
	{
		public Vector4 primaryDirAndIntensity;

		public IntPtr sphericalHarmonicCoefficientsPtr;

		public float[] SphericalHarmonicCoefficients => MarshalCoefficients(sphericalHarmonicCoefficientsPtr);

		private float[] MarshalCoefficients(IntPtr ptrFloats)
		{
			int num = 27;
			float[] shc = new float[num];
			Marshal.Copy(ptrFloats, shc, 0, num);
			RotateForUnity(ref shc, 0);
			RotateForUnity(ref shc, 9);
			RotateForUnity(ref shc, 18);
			return shc;
		}

		private void RotateForUnity(ref float[] shc, int startIndex)
		{
			shc[startIndex + 1] = 0f - shc[startIndex + 1];
			shc[startIndex + 3] = 0f - shc[startIndex + 3];
			shc[startIndex + 5] = 0f - shc[startIndex + 5];
			shc[startIndex + 7] = 0f - shc[startIndex + 7];
		}
	}
}
