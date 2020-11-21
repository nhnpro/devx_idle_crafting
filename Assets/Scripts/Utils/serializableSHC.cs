using System;

namespace Utils
{
	[Serializable]
	public class serializableSHC
	{
		public byte[] shcData;

		public serializableSHC(byte[] inputSHCData)
		{
			shcData = inputSHCData;
		}

		public static implicit operator serializableSHC(float[] floatsSHC)
		{
			if (floatsSHC != null)
			{
				byte[] array = new byte[floatsSHC.Length * 4];
				for (int i = 0; i < floatsSHC.Length; i++)
				{
					Buffer.BlockCopy(BitConverter.GetBytes(floatsSHC[i]), 0, array, i * 4, 4);
				}
				return new serializableSHC(array);
			}
			return new serializableSHC(null);
		}

		public static implicit operator float[](serializableSHC spc)
		{
			if (spc.shcData != null)
			{
				int num = spc.shcData.Length / 4;
				float[] array = new float[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = BitConverter.ToSingle(spc.shcData, i * 4);
				}
				return array;
			}
			return null;
		}
	}
}
