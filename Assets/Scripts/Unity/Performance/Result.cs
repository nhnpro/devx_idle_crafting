using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.Performance
{
	[Serializable]
	public class Result
	{
		public string name;

		public ValueBin[] deltaTimeMSBins;

		public float deltaTimeMSMax;

		public float deltaTimeMSMin;

		public uint memoryUsageAtStart;

		public uint memoryUsageAtEnd;

		public uint memoryUsageAtPeak;

		public long memoryTotal;

		public double memoryAverage;

		public long memorySystem;

		public int firstFrameNumber;

		public int lastFrameNumber;

		public float realtimeAtStart;

		public float realtimeAtStop;

		public int totalSamples;

		public int refreshRate;

		private float expectedFrames;

		private float droppedFrames;

		private float targetFrameTime;

		private int deviceRefreshRate;

		private int appRefreshRate;

		private float[] percList;

		public Stopwatch pauseTimer = new Stopwatch();

		private string[] upperboundList;

		public int memoryUsageDelta => (int)((long)memoryUsageAtEnd - (long)memoryUsageAtStart);

		public float totalTime => realtimeAtStop - realtimeAtStart - (float)pauseTimer.ElapsedMilliseconds / 1000f;

		public Result(string experimentName)
		{
			name = experimentName;
			totalSamples = 0;
			firstFrameNumber = Benchmarking.DataSource.frameCount;
			memoryUsageAtStart = Benchmarking.DataSource.memoryAllocated;
			realtimeAtStart = Benchmarking.DataSource.realtimeSinceStartup;
			deltaTimeMSMin = float.MaxValue;
			deltaTimeMSMax = 0f;
			populatePercList();
			populateUpperboundList();
			memoryUsageAtPeak = 0u;
			memoryTotal = 0L;
			memorySystem = (long)SystemInfo.systemMemorySize * 1048576L;
			deviceRefreshRate = Screen.currentResolution.refreshRate;
			appRefreshRate = Application.targetFrameRate;
			if (QualitySettings.vSyncCount == 2)
			{
				deviceRefreshRate /= 2;
			}
			if (deviceRefreshRate <= 0 && appRefreshRate <= 0)
			{
				refreshRate = 60;
				setDefaultRefreshRate();
			}
			else if (deviceRefreshRate <= 0)
			{
				refreshRate = appRefreshRate;
			}
			else if (appRefreshRate <= 0)
			{
				refreshRate = deviceRefreshRate;
				setDefaultRefreshRate();
			}
			else if (deviceRefreshRate < appRefreshRate)
			{
				refreshRate = deviceRefreshRate;
			}
			else
			{
				refreshRate = appRefreshRate;
			}
			targetFrameTime = 1f / (float)refreshRate * 1000f;
			ValueBinBuilder valueBinBuilder = new ValueBinBuilder();
			valueBinBuilder.AddBin(percList[0] * targetFrameTime);
			for (int i = 0; i < percList.Length - 1; i++)
			{
				float size = (percList[i + 1] - percList[i]) * targetFrameTime;
				valueBinBuilder.AddBin(size);
			}
			deltaTimeMSBins = valueBinBuilder.Result;
		}

		public void setDefaultRefreshRate()
		{
		}

		public void printHist()
		{
			ValueBin[] array = deltaTimeMSBins;
			for (int i = 0; i < array.Length; i++)
			{
				ValueBin valueBin = array[i];
				UnityEngine.Debug.Log("upperbound: " + valueBin.v + ", \n\tamount: " + valueBin.f);
			}
		}

		public void sendFrameData()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("experiment_name", name);
			dictionary.Add("build_version", Benchmarking.buildVersion);
			dictionary.Add("plugin_version", Benchmarking.pluginVersion);
			dictionary.Add("refresh_rate", refreshRate);
			dictionary.Add("experiment_time", totalTime);
			for (int i = 0; i < percList.Length; i++)
			{
				dictionary.Add(upperboundList[i], deltaTimeMSBins[i].f);
			}
			float num = totalTime - (float)pauseTimer.ElapsedMilliseconds;
			expectedFrames = (float)refreshRate * num;
			droppedFrames = expectedFrames - (float)totalSamples;
			float num2 = 60f / num;
			if (droppedFrames < 0f)
			{
				UnityEngine.Debug.Log("Dropped frames is neg");
				droppedFrames = 0f;
			}
			else
			{
				droppedFrames *= num2;
			}
			dictionary.Add("dropped_frames_per_min", droppedFrames);
			Analytics.CustomEvent("perfFrameData", dictionary);
		}

		public void populatePercList()
		{
			percList = new float[27]
			{
				0.5f,
				0.75f,
				0.85f,
				0.9f,
				0.92f,
				0.94f,
				0.96f,
				0.98f,
				1f,
				1.01f,
				1.02f,
				1.03f,
				1.04f,
				1.05f,
				1.06f,
				1.07f,
				1.08f,
				1.09f,
				1.1f,
				1.15f,
				1.2f,
				2f,
				4f,
				8f,
				16f,
				32f,
				float.PositiveInfinity
			};
		}

		public void populateUpperboundList()
		{
			upperboundList = new string[27]
			{
				"upper_bound: 0.5",
				"upper_bound: 0.75",
				"upper_bound: 0.85",
				"upper_bound: 0.90",
				"upper_bound: 0.92",
				"upper_bound: 0.94",
				"upper_bound: 0.96",
				"upper_bound: 0.98",
				"upper_bound: 1",
				"upper_bound: 1.01",
				"upper_bound: 1.02",
				"upper_bound: 1.03",
				"upper_bound: 1.04",
				"upper_bound: 1.05",
				"upper_bound: 1.06",
				"upper_bound: 1.07",
				"upper_bound: 1.08",
				"upper_bound: 1.09",
				"upper_bound: 1.1",
				"upper_bound: 1.15",
				"upper_bound: 1.2",
				"upper_bound: 2",
				"upper_bound: 4",
				"upper_bound: 8",
				"upper_bound: 16",
				"upper_bound: 32",
				"upper_bound: Infinity"
			};
		}
	}
}
