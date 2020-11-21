using System;
using UnityEngine.XR.iOS;

namespace Utils
{
	[Serializable]
	public class serializableARKitInit
	{
		public serializableARSessionConfiguration config;

		public UnityARSessionRunOption runOption;

		public serializableARKitInit(serializableARSessionConfiguration cfg, UnityARSessionRunOption option)
		{
			config = cfg;
			runOption = option;
		}
	}
}
