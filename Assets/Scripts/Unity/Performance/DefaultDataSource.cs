using UnityEngine;
using UnityEngine.Profiling;

namespace Unity.Performance
{
	public class DefaultDataSource : IDataSource
	{
		public float unscaledDeltaTimeSeconds => Time.unscaledDeltaTime;

		public float realtimeSinceStartup => Time.realtimeSinceStartup;

		public int frameCount => Time.frameCount;

		public uint memoryAllocated => (uint)Profiler.GetTotalAllocatedMemoryLong();
	}
}
