using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Performance
{
	public class PerfRecorder : MonoBehaviour
	{
		public string pluginVersion => Benchmarking.pluginVersion;

		public string buildVersion
		{
			get
			{
				return Benchmarking.buildVersion;
			}
			set
			{
				Benchmarking.buildVersion = value;
			}
		}

		public void SetBuildVersion(string buildVersion)
		{
			Benchmarking.buildVersion = buildVersion;
		}

		public void BeginExperiment(string experimentName)
		{
			Benchmarking.BeginExperiment(experimentName);
		}

		public void EndExperiment()
		{
			Benchmarking.EndExperiment();
			Benchmarking.Finished();
		}

		public void LoadScene(int sceneBuildIndex)
		{
			Benchmarking.LoadScene(sceneBuildIndex);
		}

		public void LoadScene(int sceneBuildIndex, LoadSceneMode mode)
		{
			Benchmarking.LoadScene(sceneBuildIndex);
		}

		public void LoadScene(string sceneName)
		{
			Benchmarking.LoadScene(sceneName);
		}

		public void LoadScene(string sceneName, LoadSceneMode mode)
		{
			Benchmarking.LoadScene(sceneName, mode);
		}

		public AsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			return Benchmarking.LoadSceneAsync(sceneBuildIndex);
		}

		public AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode)
		{
			return Benchmarking.LoadSceneAsync(sceneBuildIndex, mode);
		}

		public AsyncOperation LoadSceneAsync(string sceneName)
		{
			return Benchmarking.LoadSceneAsync(sceneName);
		}

		public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode)
		{
			return Benchmarking.LoadSceneAsync(sceneName, mode);
		}

		public void StartTimer(string context)
		{
			Benchmarking.StartTimer(context);
		}

		public float StopTimer()
		{
			return Benchmarking.StopTimer();
		}
	}
}
