using UnityEngine.Profiling;

namespace Unity.Performance
{
	internal class DefaultProfilerControl : IProfilerControl
	{
		public bool supported => Profiler.supported;

		public bool recording => Profiler.supported && Profiler.enabled;

		public void StartRecording(string filePath)
		{
			Profiler.logFile = filePath;
			Profiler.enableBinaryLog = true;
			Profiler.enabled = true;
		}

		public void StopRecording()
		{
			Profiler.enabled = false;
		}
	}
}
