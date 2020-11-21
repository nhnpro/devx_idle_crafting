using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Dreamteck.Splines
{
	public class SplineUser : MonoBehaviour
	{
		public enum UpdateMethod
		{
			Update,
			FixedUpdate,
			LateUpdate
		}

		[HideInInspector]
		public SplineAddress _address;

		[SerializeField]
		[HideInInspector]
		private SplineUser[] subscribers = new SplineUser[0];

		[HideInInspector]
		public UpdateMethod updateMethod;

		[HideInInspector]
		[SerializeField]
		private SplineUser _user;

		[SerializeField]
		[HideInInspector]
		private double _resolution = 1.0;

		[SerializeField]
		[HideInInspector]
		private double _clipTo = 1.0;

		[SerializeField]
		[HideInInspector]
		private double _clipFrom;

		[SerializeField]
		[HideInInspector]
		private bool _autoUpdate = true;

		[SerializeField]
		[HideInInspector]
		private bool _loopSamples;

		[SerializeField]
		[HideInInspector]
		private bool _averageResultVectors = true;

		[SerializeField]
		[HideInInspector]
		private bool _uniformSample;

		[SerializeField]
		[HideInInspector]
		private bool _uniformPreserveClipRange;

		[SerializeField]
		[HideInInspector]
		private SplineResult[] _samples = new SplineResult[0];

		[SerializeField]
		[HideInInspector]
		private SplineResult[] _clippedSamples = new SplineResult[0];

		[SerializeField]
		[HideInInspector]
		private float animClipFrom;

		[SerializeField]
		[HideInInspector]
		private float animClipTo = 1f;

		[SerializeField]
		[HideInInspector]
		private double animResolution = 1.0;

		[SerializeField]
		[HideInInspector]
		protected bool sampleUser;

		private bool rebuild;

		private bool sample;

		private volatile bool getClippedSamples;

		[HideInInspector]
		public volatile bool multithreaded;

		[HideInInspector]
		public bool buildOnAwake;

		private Thread buildThread;

		private volatile bool postThread;

		private volatile bool threadSample;

		private volatile bool threadWork;

		private bool _threadWorking;

		private object locker = new object();

		public SplineUser user
		{
			get
			{
				return _user;
			}
			set
			{
				if ((!Application.isPlaying || !(value != null) || !(value.rootUser == this)) && value != _user)
				{
					if (value != null && computer != null)
					{
						computer.Unsubscribe(this);
						computer = null;
					}
					if (_user != null)
					{
						_user.Unsubscribe(this);
					}
					_user = value;
					if (_user != null)
					{
						_user.Subscribe(this);
						sampleUser = true;
					}
					if (computer == null)
					{
						_samples = new SplineResult[0];
						_clippedSamples = new SplineResult[0];
					}
					Rebuild(sampleComputer: false);
				}
			}
		}

		public SplineUser rootUser
		{
			get
			{
				SplineUser splineUser = _user;
				while (splineUser != null && !(splineUser._user == null))
				{
					splineUser = splineUser._user;
					if (splineUser == this)
					{
						break;
					}
				}
				if (splineUser == null)
				{
					splineUser = this;
				}
				return splineUser;
			}
		}

		public SplineComputer computer
		{
			get
			{
				return address.root;
			}
			set
			{
				if (_address == null)
				{
					_address = new SplineAddress(value);
					value.Subscribe(this);
					if (value != null)
					{
						RebuildImmediate(sampleComputer: true);
					}
				}
				else if (value != _address.root)
				{
					if (value != null && sampleUser)
					{
						_user.Unsubscribe(this);
						_user = null;
					}
					if (_address.root != null)
					{
						_address.root.Unsubscribe(this);
					}
					_address.root = value;
					if (value != null)
					{
						value.Subscribe(this);
						sampleUser = false;
					}
					if (_address.root != null)
					{
						RebuildImmediate(sampleComputer: true);
					}
				}
			}
		}

		public double resolution
		{
			get
			{
				return _resolution;
			}
			set
			{
				if (value != _resolution)
				{
					animResolution = (float)_resolution;
					_resolution = value;
					if (!sampleUser)
					{
						Rebuild(sampleComputer: true);
					}
				}
			}
		}

		public double clipFrom
		{
			get
			{
				return _clipFrom;
			}
			set
			{
				if (value != _clipFrom)
				{
					animClipFrom = (float)_clipFrom;
					_clipFrom = DMath.Clamp01(value);
					if (_clipFrom > _clipTo && !rootUser.computer.isClosed)
					{
						_clipTo = _clipFrom;
					}
					getClippedSamples = true;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public double clipTo
		{
			get
			{
				return _clipTo;
			}
			set
			{
				if (value != _clipTo)
				{
					animClipTo = (float)_clipTo;
					_clipTo = DMath.Clamp01(value);
					if (_clipTo < _clipFrom && !rootUser.computer.isClosed)
					{
						_clipFrom = _clipTo;
					}
					getClippedSamples = true;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool averageResultVectors
		{
			get
			{
				return _averageResultVectors;
			}
			set
			{
				if (value != _averageResultVectors)
				{
					_averageResultVectors = value;
					if (!sampleUser)
					{
						Rebuild(sampleComputer: true);
					}
				}
			}
		}

		public bool autoUpdate
		{
			get
			{
				return _autoUpdate;
			}
			set
			{
				if (value != _autoUpdate)
				{
					_autoUpdate = value;
					if (value)
					{
						Rebuild(sampleComputer: true);
					}
				}
			}
		}

		public bool loopSamples
		{
			get
			{
				return _loopSamples;
			}
			set
			{
				if (value != _loopSamples)
				{
					_loopSamples = value;
					if (value)
					{
						Rebuild(sampleComputer: true);
					}
				}
			}
		}

		public bool uniformSample
		{
			get
			{
				return _uniformSample;
			}
			set
			{
				if (value != _uniformSample)
				{
					_uniformSample = value;
					Rebuild(sampleComputer: true);
				}
			}
		}

		public bool uniformPreserveClipRange
		{
			get
			{
				return _uniformPreserveClipRange;
			}
			set
			{
				if (value != _uniformPreserveClipRange)
				{
					_uniformPreserveClipRange = value;
					Rebuild(sampleComputer: true);
				}
			}
		}

		public double span
		{
			get
			{
				if (samplesAreLooped)
				{
					return 1.0 - _clipFrom + _clipTo;
				}
				return _clipTo - _clipFrom;
			}
		}

		public SplineAddress address
		{
			get
			{
				if (_address == null)
				{
					_address = new SplineAddress((SplineComputer)null);
				}
				return _address;
			}
		}

		public bool samplesAreLooped
		{
			get
			{
				if (rootUser.computer == null)
				{
					return false;
				}
				return rootUser.computer.isClosed && _loopSamples && clipFrom >= clipTo;
			}
		}

		public SplineResult[] samples
		{
			get
			{
				if (sampleUser)
				{
					return _user.samples;
				}
				return _samples;
			}
		}

		public SplineResult[] clippedSamples
		{
			get
			{
				if (_clippedSamples.Length == 0 && _samples.Length > 0)
				{
					GetClippedSamples();
				}
				return _clippedSamples;
			}
		}

		protected bool willRebuild => rebuild;

		public bool threadWorking => _threadWorking;

		protected virtual void Awake()
		{
			if (sampleUser)
			{
				if (!user.IsSubscribed(this))
				{
					user.Subscribe(this);
				}
			}
			else if (computer == null)
			{
				computer = GetComponent<SplineComputer>();
			}
			else if (!computer.IsSubscribed(this))
			{
				computer.Subscribe(this);
			}
			if (buildOnAwake)
			{
				RebuildImmediate(sampleComputer: true);
			}
		}

		protected virtual void Reset()
		{
		}

		protected virtual void OnEnable()
		{
			if (computer != null)
			{
				computer.Subscribe(this);
			}
		}

		protected virtual void OnDisable()
		{
			if (computer != null)
			{
				computer.Unsubscribe(this);
			}
			threadWork = false;
		}

		protected virtual void OnDestroy()
		{
			if (buildThread != null)
			{
				threadWork = false;
				buildThread.Abort();
				buildThread = null;
				_threadWorking = false;
			}
		}

		protected virtual void OnApplicationQuit()
		{
			if (buildThread != null)
			{
				threadWork = false;
				buildThread.Abort();
				buildThread = null;
				_threadWorking = false;
			}
		}

		protected virtual void OnDidApplyAnimationProperties()
		{
			bool flag = false;
			if (_clipFrom != (double)animClipFrom || _clipTo != (double)animClipTo)
			{
				flag = true;
			}
			bool flag2 = false;
			if (_resolution != animResolution)
			{
				flag2 = true;
			}
			_clipFrom = animClipFrom;
			_clipTo = animClipTo;
			_resolution = animResolution;
			Rebuild(flag2);
			if (!flag2 && flag)
			{
				GetClippedSamples();
			}
		}

		public virtual void Rebuild(bool sampleComputer)
		{
			if (sampleUser)
			{
				sampleComputer = false;
				getClippedSamples = true;
			}
			if (!autoUpdate)
			{
				return;
			}
			rebuild = true;
			if (sampleComputer)
			{
				sample = true;
				if (threadWorking)
				{
					StartCoroutine(UpdateSubscribersRoutine());
				}
			}
		}

		private IEnumerator UpdateSubscribersRoutine()
		{
			while (rebuild)
			{
				yield return null;
			}
			UpdateSubscribers();
		}

		public virtual void RebuildImmediate(bool sampleComputer)
		{
			if (sampleUser)
			{
				sampleComputer = false;
				GetClippedSamples();
			}
			if (threadWork)
			{
				if (sampleComputer)
				{
					threadSample = true;
				}
				buildThread.Interrupt();
				StartCoroutine(UpdateSubscribersRoutine());
			}
			else
			{
				if (sampleComputer)
				{
					SampleComputer();
				}
				else if (getClippedSamples)
				{
					GetClippedSamples();
				}
				UpdateSubscribers();
				Build();
				PostBuild();
			}
			rebuild = false;
			sampleComputer = false;
			getClippedSamples = false;
		}

		public void GetClippedSamplesImmediate()
		{
			GetClippedSamples();
			if (sample)
			{
				getClippedSamples = true;
			}
		}

		public virtual void EnterAddress(Node node, int connectionIndex, Spline.Direction direction = Spline.Direction.Forward)
		{
			if (!sampleUser)
			{
				int depth = _address.depth;
				address.AddSpline(node, connectionIndex, direction);
				if (_address.depth != depth)
				{
					Rebuild(sampleComputer: true);
				}
			}
		}

		public virtual void AddComputer(SplineComputer computer, int connectionIndex, int connectedIndex, Spline.Direction direction = Spline.Direction.Forward)
		{
			if (!sampleUser)
			{
				int depth = _address.depth;
				address.AddSpline(computer, connectionIndex, connectedIndex, direction);
				if (_address.depth != depth)
				{
					Rebuild(sampleComputer: true);
				}
			}
		}

		public virtual void CollapseAddress()
		{
			if (!sampleUser)
			{
				address.Collapse();
				Rebuild(sampleComputer: true);
			}
		}

		public virtual void ClearAddress()
		{
			if (!sampleUser)
			{
				int depth = _address.depth;
				_address.Clear();
				if (_address.depth != depth)
				{
					Rebuild(sampleComputer: true);
				}
			}
		}

		public virtual void ExitAddress(int depth)
		{
			if (!sampleUser)
			{
				int depth2 = _address.depth;
				_address.Exit(depth);
				if (_address.depth != depth2)
				{
					Rebuild(sampleComputer: true);
				}
			}
		}

		private void Update()
		{
			if (updateMethod == UpdateMethod.Update)
			{
				RunMain();
			}
		}

		private void LateUpdate()
		{
			if (updateMethod == UpdateMethod.LateUpdate)
			{
				RunMain();
			}
		}

		private void FixedUpdate()
		{
			if (updateMethod == UpdateMethod.FixedUpdate)
			{
				RunMain();
			}
		}

		private void UpdateSubscribers()
		{
			for (int num = subscribers.Length - 1; num >= 0; num--)
			{
				if (subscribers[num] == null)
				{
					RemoveSubscriber(num);
				}
				else
				{
					subscribers[num].RebuildImmediate(sampleComputer: false);
				}
			}
		}

		private void RunMain()
		{
			Run();
			if (multithreaded)
			{
				threadWork = (Environment.ProcessorCount > 1);
			}
			else
			{
				threadWork = (postThread = false);
			}
			if (threadWork)
			{
				if (postThread)
				{
					PostBuild();
					postThread = false;
				}
				if (buildThread == null)
				{
					buildThread = new Thread(RunThread);
					buildThread.Start();
				}
				else if (!buildThread.IsAlive)
				{
					buildThread = new Thread(RunThread);
					buildThread.Start();
				}
			}
			else if (_threadWorking)
			{
				buildThread.Abort();
				buildThread = null;
				_threadWorking = false;
			}
			if (rebuild && base.enabled)
			{
				if (_threadWorking)
				{
					threadSample = sample;
					buildThread.Interrupt();
					sample = false;
				}
				else
				{
					if (sample)
					{
						SampleComputer();
						sample = false;
						UpdateSubscribers();
					}
					else if (getClippedSamples)
					{
						GetClippedSamples();
						UpdateSubscribers();
					}
					Build();
					PostBuild();
				}
				rebuild = false;
			}
			LateRun();
		}

		private void RunThread()
		{
			lock (locker)
			{
				_threadWorking = true;
			}
			while (true)
			{
				try
				{
					Thread.Sleep(-1);
				}
				catch (ThreadInterruptedException)
				{
					lock (locker)
					{
						if (threadSample)
						{
							SampleComputer();
							threadSample = false;
						}
						else if (getClippedSamples)
						{
							GetClippedSamples();
						}
						Build();
						postThread = true;
					}
				}
				catch (ThreadAbortException)
				{
					return;
				}
			}
		}

		protected virtual void Run()
		{
		}

		protected virtual void LateRun()
		{
		}

		protected virtual void Build()
		{
		}

		protected virtual void PostBuild()
		{
		}

		public void SetClipRange(double from, double to)
		{
			if (!rootUser.computer.isClosed && to < from)
			{
				to = from;
			}
			_clipFrom = DMath.Clamp01(from);
			_clipTo = DMath.Clamp01(to);
			GetClippedSamples();
			Rebuild(sampleComputer: false);
		}

		private void SampleComputer()
		{
			if (computer == null || computer.pointCount == 0)
			{
				return;
			}
			if (computer.pointCount == 1)
			{
				_samples = new SplineResult[1];
				_samples[0] = _address.Evaluate(0.0);
				return;
			}
			if (_resolution == 0.0)
			{
				_samples = new SplineResult[0];
				_clippedSamples = new SplineResult[0];
				return;
			}
			double num = _address.moveStep / _resolution;
			int num2 = DMath.CeilInt(1.0 / num) + 1;
			if (_samples.Length != num2)
			{
				_samples = new SplineResult[num2];
			}
			if (uniformSample)
			{
				float distance = computer.CalculateLength() / (float)(num2 - 1);
				_samples[0] = _address.Evaluate(0.0);
				_samples[0].percent = 0.0;
				for (int i = 1; i < num2 - 1; i++)
				{
					_samples[i] = _address.Evaluate(_address.Travel(_samples[i - 1].percent, distance, Spline.Direction.Forward, num2));
				}
				if (computer.isClosed)
				{
					_samples[samples.Length - 1] = new SplineResult(_samples[0]);
				}
				else
				{
					_samples[_samples.Length - 1] = _address.Evaluate(1.0);
				}
				_samples[_samples.Length - 1].percent = 1.0;
			}
			else
			{
				for (int j = 0; j < num2; j++)
				{
					double percent = (double)j / (double)(num2 - 1);
					if (computer.isClosed && j == num2 - 1)
					{
						percent = 0.0;
					}
					_samples[j] = _address.Evaluate(percent);
					_samples[j].percent = percent;
				}
			}
			if (_samples.Length == 0)
			{
				_clippedSamples = new SplineResult[0];
				GetClippedSamples();
				return;
			}
			if (_samples.Length > 1 && _averageResultVectors)
			{
				Vector3 vector = _samples[1].position - _samples[0].position;
				for (int k = 0; k < _samples.Length - 1; k++)
				{
					Vector3 normalized = (_samples[k + 1].position - _samples[k].position).normalized;
					_samples[k].direction = (vector + normalized).normalized;
					_samples[k].normal = (_samples[k].normal + _samples[k + 1].normal).normalized;
					vector = normalized;
				}
				if (computer.isClosed)
				{
					_samples[_samples.Length - 1].direction = (_samples[0].direction = Vector3.Slerp(_samples[0].direction, vector, 0.5f));
				}
				else
				{
					_samples[_samples.Length - 1].direction = vector;
				}
			}
			if (computer.isClosed && clipTo == 1.0)
			{
				_samples[_samples.Length - 1] = new SplineResult(_samples[0]);
			}
			_samples[_samples.Length - 1].percent = 1.0;
			GetClippedSamples();
		}

		private void GetClippedSamples()
		{
			getClippedSamples = false;
			if (span == 1.0 && !samplesAreLooped)
			{
				_clippedSamples = samples;
				return;
			}
			double num = clipFrom * (double)(samples.Length - 1);
			double num2 = clipTo * (double)(samples.Length - 1);
			int num3 = DMath.FloorInt(num);
			int num4 = DMath.CeilInt(num2);
			if (samplesAreLooped)
			{
				if (_uniformSample)
				{
					int num5 = 0;
					int num6 = 0;
					int num7 = -1;
					for (int i = 0; i < samples.Length - 1; i++)
					{
						if (samples[i].percent > clipFrom)
						{
							num5++;
							if (num7 < 0)
							{
								num7 = i - 1;
							}
						}
						else if (samples[i].percent < clipTo)
						{
							num6++;
						}
					}
					num5++;
					if (_clippedSamples.Length != num5 + num6 || _clippedSamples == samples)
					{
						_clippedSamples = new SplineResult[num5 + num6];
					}
					for (int j = 1; j <= num5; j++)
					{
						_clippedSamples[j] = _samples[num7 + j];
					}
					for (int k = 0; k < num6 - 1; k++)
					{
						_clippedSamples[k + num5 + 1] = _samples[k];
					}
					_clippedSamples[0] = Evaluate(clipFrom);
					_clippedSamples[_clippedSamples.Length - 1] = Evaluate(clipTo);
				}
				else
				{
					int num8 = DMath.CeilInt(num2) + 1;
					int num9 = samples.Length - DMath.FloorInt(num) - 1;
					if (_clippedSamples.Length != num8 + num9)
					{
						_clippedSamples = new SplineResult[num8 + num9];
					}
					_clippedSamples[0] = Evaluate(_clipFrom);
					for (int l = 1; l < num9; l++)
					{
						_clippedSamples[l] = samples[samples.Length - num9 + l - 1];
					}
					for (int m = 0; m < num8 - 1; m++)
					{
						_clippedSamples[num9 + m] = new SplineResult(samples[m]);
					}
					_clippedSamples[_clippedSamples.Length - 1] = Evaluate(_clipTo);
				}
			}
			else if (_uniformSample)
			{
				int num10 = 0;
				int num11 = -1;
				for (int n = 0; n < samples.Length; n++)
				{
					if (samples[n].percent > clipFrom && samples[n].percent < clipTo)
					{
						num10++;
						if (num11 < 0)
						{
							num11 = n - 1;
						}
					}
				}
				num10 += 2;
				if (_clippedSamples.Length != num10 || _clippedSamples == samples)
				{
					_clippedSamples = new SplineResult[num10];
				}
				for (int num12 = 1; num12 < _clippedSamples.Length - 1; num12++)
				{
					_clippedSamples[num12] = samples[num11 + num12];
				}
				_clippedSamples[0] = Evaluate(clipFrom);
				_clippedSamples[_clippedSamples.Length - 1] = Evaluate(clipTo);
			}
			else
			{
				int num13 = DMath.CeilInt(num2) - DMath.FloorInt(num) + 1;
				if (_clippedSamples.Length != num13 || _clippedSamples == samples)
				{
					_clippedSamples = new SplineResult[num13];
				}
				if (num3 + 1 < samples.Length)
				{
					_clippedSamples[0] = SplineResult.Lerp(samples[num3], samples[num3 + 1], num - (double)num3);
				}
				for (int num14 = 1; num14 < _clippedSamples.Length - 1; num14++)
				{
					_clippedSamples[num14] = samples[num3 + num14];
				}
				if (num4 - 1 >= 0)
				{
					_clippedSamples[_clippedSamples.Length - 1] = SplineResult.Lerp(samples[num4], samples[num4 - 1], (double)num4 - num2);
				}
			}
		}

		public virtual SplineResult Evaluate(double percent)
		{
			if (samples.Length == 0)
			{
				return new SplineResult();
			}
			if (samples.Length == 1)
			{
				return samples[0];
			}
			if (_uniformSample && _uniformPreserveClipRange)
			{
				double num = 1.0;
				int num2 = 0;
				for (int i = 0; i < samples.Length; i++)
				{
					double num3 = DMath.Abs(percent - samples[i].percent);
					if (num3 < num)
					{
						num = num3;
						num2 = i;
					}
				}
				if (percent > samples[num2].percent)
				{
					return SplineResult.Lerp(samples[num2], samples[num2 + 1], Mathf.InverseLerp((float)samples[num2].percent, (float)samples[num2 + 1].percent, (float)percent));
				}
				if (percent < samples[num2].percent)
				{
					return SplineResult.Lerp(samples[num2 - 1], samples[num2], Mathf.InverseLerp((float)samples[num2 - 1].percent, (float)samples[num2].percent, (float)percent));
				}
				return new SplineResult(samples[num2]);
			}
			percent = DMath.Clamp01(percent);
			int sampleIndex = GetSampleIndex(percent);
			double num4 = (double)(samples.Length - 1) * percent - (double)sampleIndex;
			if (num4 > 0.0 && sampleIndex < samples.Length - 1)
			{
				return SplineResult.Lerp(samples[sampleIndex], samples[sampleIndex + 1], num4);
			}
			return new SplineResult(samples[sampleIndex]);
		}

		public virtual void Evaluate(SplineResult result, double percent)
		{
			if (samples.Length == 0)
			{
				result = new SplineResult();
			}
			else if (samples.Length == 1)
			{
				result.CopyFrom(samples[0]);
			}
			else if (_uniformSample && _uniformPreserveClipRange)
			{
				double num = 1.0;
				int num2 = 0;
				for (int i = 0; i < samples.Length; i++)
				{
					double num3 = DMath.Abs(percent - samples[i].percent);
					if (num3 < num)
					{
						num = num3;
						num2 = i;
					}
				}
				if (percent > samples[num2].percent)
				{
					SplineResult.Lerp(samples[num2], samples[num2 + 1], Mathf.InverseLerp((float)samples[num2].percent, (float)samples[num2 + 1].percent, (float)percent), result);
				}
				else if (percent < samples[num2].percent)
				{
					SplineResult.Lerp(samples[num2 - 1], samples[num2], Mathf.InverseLerp((float)samples[num2 - 1].percent, (float)samples[num2].percent, (float)percent), result);
				}
				else
				{
					result.CopyFrom(samples[num2]);
				}
			}
			else
			{
				percent = DMath.Clamp01(percent);
				int sampleIndex = GetSampleIndex(percent);
				double num4 = (double)(samples.Length - 1) * percent - (double)sampleIndex;
				if (num4 > 0.0 && sampleIndex < samples.Length - 1)
				{
					SplineResult.Lerp(samples[sampleIndex], samples[sampleIndex + 1], num4, result);
				}
				else
				{
					result.CopyFrom(samples[sampleIndex]);
				}
			}
		}

		public virtual Vector3 EvaluatePosition(double percent, bool overrideUniformClipRange = false)
		{
			if (samples.Length == 0)
			{
				return Vector3.zero;
			}
			if (samples.Length == 1)
			{
				return samples[0].position;
			}
			percent = DMath.Clamp01(percent);
			if (_uniformSample && overrideUniformClipRange)
			{
				double num = 1.0;
				int num2 = 0;
				for (int i = 0; i < samples.Length; i++)
				{
					double num3 = DMath.Abs(percent - samples[i].percent);
					if (num3 < num)
					{
						num = num3;
						num2 = i;
					}
				}
				if (percent > samples[num2].percent)
				{
					return Vector3.Lerp(samples[num2].position, samples[num2 + 1].position, Mathf.InverseLerp((float)samples[num2].percent, (float)samples[num2 + 1].percent, (float)percent));
				}
				if (percent < samples[num2].percent)
				{
					return Vector3.Lerp(samples[num2 - 1].position, samples[num2].position, Mathf.InverseLerp((float)samples[num2 - 1].percent, (float)samples[num2].percent, (float)percent));
				}
				return samples[num2].position;
			}
			int sampleIndex = GetSampleIndex(percent);
			double num4 = (double)(samples.Length - 1) * percent - (double)sampleIndex;
			if (num4 > 0.0 && sampleIndex < samples.Length - 1)
			{
				return Vector3.Lerp(samples[sampleIndex].position, samples[sampleIndex + 1].position, (float)num4);
			}
			return samples[sampleIndex].position;
		}

		public double ClipPercent(double percent)
		{
			ClipPercent(ref percent);
			return percent;
		}

		public void ClipPercent(ref double percent)
		{
			if (_clippedSamples.Length == 0)
			{
				percent = 0.0;
				return;
			}
			double percent2 = _clippedSamples[0].percent;
			double percent3 = _clippedSamples[_clippedSamples.Length - 1].percent;
			if (samplesAreLooped)
			{
				if (percent >= percent2 && percent <= 1.0)
				{
					percent = DMath.InverseLerp(percent2, percent2 + span, percent);
				}
				else if (percent <= percent3)
				{
					percent = DMath.InverseLerp(percent3 - span, percent3, percent);
				}
				else if (DMath.InverseLerp(percent3, percent2, percent) < 0.5)
				{
					percent = 1.0;
				}
				else
				{
					percent = 0.0;
				}
			}
			else
			{
				percent = DMath.InverseLerp(percent2, percent3, percent);
			}
		}

		public double UnclipPercent(double percent)
		{
			UnclipPercent(ref percent);
			return percent;
		}

		public void UnclipPercent(ref double percent)
		{
			double percent2 = _clippedSamples[0].percent;
			double percent3 = _clippedSamples[_clippedSamples.Length - 1].percent;
			if (samplesAreLooped)
			{
				double num = (1.0 - percent2) / span;
				if (num == 0.0)
				{
					percent = 0.0;
					return;
				}
				if (percent < num)
				{
					percent = DMath.Lerp(percent2, 1.0, percent / num);
				}
				else
				{
					if (percent3 == 0.0)
					{
						percent = 0.0;
						return;
					}
					percent = DMath.Lerp(0.0, percent3, (percent - num) / (percent3 / span));
				}
			}
			else
			{
				percent = DMath.Lerp(percent2, percent3, percent);
			}
			percent = DMath.Clamp01(percent);
		}

		public int GetSampleIndex(double percent)
		{
			return DMath.FloorInt(percent * (double)(samples.Length - 1));
		}

		public int GetClippedSampleIndex(double percent)
		{
			return DMath.FloorInt(percent * (double)(clippedSamples.Length - 1));
		}

		public virtual SplineResult Project(Vector3 point, double from = 0.0, double to = 1.0)
		{
			SplineResult result = new SplineResult();
			Project(result, point, from, to);
			return result;
		}

		public virtual void Project(SplineResult result, Vector3 point, double from = 0.0, double to = 1.0)
		{
			if (samples.Length == 0)
			{
				return;
			}
			if (samples.Length == 1)
			{
				if (result == null)
				{
					result = new SplineResult(samples[0]);
				}
				else
				{
					result.CopyFrom(samples[0]);
				}
				return;
			}
			if (computer == null)
			{
				result = new SplineResult();
				return;
			}
			int num = (computer.pointCount - 1) * 6;
			int num2 = samples.Length / num;
			if (num2 < 1)
			{
				num2 = 1;
			}
			float num3 = (point - samples[0].position).sqrMagnitude;
			int num4 = 0;
			int num5 = samples.Length - 1;
			if (from != 0.0)
			{
				num4 = GetSampleIndex(from);
			}
			if (to != 1.0)
			{
				num5 = Mathf.CeilToInt((float)to * (float)(samples.Length - 1));
			}
			int num6 = num4;
			int num7 = num5;
			for (int i = num4; i <= num5; i += num2)
			{
				if (i > num5)
				{
					i = num5;
				}
				float sqrMagnitude = (point - samples[i].position).sqrMagnitude;
				if (sqrMagnitude < num3)
				{
					num3 = sqrMagnitude;
					num6 = Mathf.Max(i - num2, 0);
					num7 = Mathf.Min(i + num2, samples.Length - 1);
				}
				if (i == num5)
				{
					break;
				}
			}
			num3 = (point - samples[num6].position).sqrMagnitude;
			int num8 = num6;
			for (int j = num6 + 1; j <= num7; j++)
			{
				float sqrMagnitude2 = (point - samples[j].position).sqrMagnitude;
				if (sqrMagnitude2 < num3)
				{
					num3 = sqrMagnitude2;
					num8 = j;
				}
			}
			int num9 = num8 - 1;
			if (num9 < 0)
			{
				num9 = 0;
			}
			int num10 = num8 + 1;
			if (num10 > samples.Length - 1)
			{
				num10 = samples.Length - 1;
			}
			Vector3 vector = LinearAlgebraUtility.ProjectOnLine(samples[num9].position, samples[num8].position, point);
			Vector3 vector2 = LinearAlgebraUtility.ProjectOnLine(samples[num8].position, samples[num10].position, point);
			float magnitude = (samples[num8].position - samples[num9].position).magnitude;
			float magnitude2 = (samples[num8].position - samples[num10].position).magnitude;
			float magnitude3 = (vector - samples[num9].position).magnitude;
			float magnitude4 = (vector2 - samples[num10].position).magnitude;
			if (num9 < num8 && num8 < num10)
			{
				if ((point - vector).sqrMagnitude < (point - vector2).sqrMagnitude)
				{
					SplineResult.Lerp(samples[num9], samples[num8], magnitude3 / magnitude, result);
				}
				else
				{
					SplineResult.Lerp(samples[num10], samples[num8], magnitude4 / magnitude2, result);
				}
			}
			else if (num9 < num8)
			{
				SplineResult.Lerp(samples[num9], samples[num8], magnitude3 / magnitude, result);
			}
			else
			{
				SplineResult.Lerp(samples[num10], samples[num8], magnitude4 / magnitude2, result);
			}
			if (from == 0.0 && to == 1.0 && result.percent < _address.moveStep / _resolution)
			{
				Vector3 vector3 = LinearAlgebraUtility.ProjectOnLine(samples[samples.Length - 1].position, samples[samples.Length - 2].position, point);
				if ((point - vector3).sqrMagnitude < (point - result.position).sqrMagnitude)
				{
					SplineResult.Lerp(samples[samples.Length - 1], samples[samples.Length - 2], LinearAlgebraUtility.InverseLerp(samples[samples.Length - 1].position, samples[samples.Length - 2].position, vector3), result);
				}
			}
		}

		public virtual double Travel(double start, float distance, Spline.Direction direction)
		{
			if (samples.Length <= 1)
			{
				return 0.0;
			}
			if (direction == Spline.Direction.Forward && start >= 1.0)
			{
				return 1.0;
			}
			if (direction == Spline.Direction.Backward && start <= 0.0)
			{
				return 0.0;
			}
			if (distance == 0f)
			{
				return DMath.Clamp01(start);
			}
			float num = 0f;
			Vector3 b = EvaluatePosition(start);
			double a = start;
			int num2 = (direction != Spline.Direction.Forward) ? DMath.FloorInt(start * (double)(samples.Length - 1)) : DMath.CeilInt(start * (double)(samples.Length - 1));
			float num3 = 0f;
			while (true)
			{
				num3 = Vector3.Distance(samples[num2].position, b);
				b = samples[num2].position;
				num += num3;
				if (num >= distance)
				{
					break;
				}
				a = samples[num2].percent;
				if (direction == Spline.Direction.Forward)
				{
					if (num2 == samples.Length - 1)
					{
						break;
					}
					num2++;
				}
				else
				{
					if (num2 == 0)
					{
						break;
					}
					num2--;
				}
			}
			return DMath.Lerp(a, samples[num2].percent, 1f - (num - distance) / num3);
		}

		private void Subscribe(SplineUser input)
		{
			if (input == this)
			{
				return;
			}
			int num = -1;
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (subscribers[i] == input)
				{
					return;
				}
				if (subscribers[i] == null && num < 0)
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				subscribers[num] = input;
				return;
			}
			SplineUser[] array = new SplineUser[subscribers.Length + 1];
			subscribers.CopyTo(array, 0);
			array[subscribers.Length] = input;
			subscribers = array;
		}

		private void Unsubscribe(SplineUser input)
		{
			int num = -1;
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (subscribers[i] == input)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				return;
			}
			SplineUser[] array = new SplineUser[subscribers.Length - 1];
			int num2 = subscribers.Length - 1;
			for (int j = 0; j < subscribers.Length; j++)
			{
				if (num2 != num)
				{
					if (j < num2)
					{
						array[j] = subscribers[j];
					}
					else
					{
						array[j - 1] = subscribers[j - 1];
					}
				}
			}
			subscribers = array;
		}

		public virtual float CalculateLength(double from = 0.0, double to = 1.0)
		{
			float num = 0f;
			Vector3 b = EvaluatePosition(from);
			int num2 = DMath.CeilInt(from * (double)(samples.Length - 1));
			int sampleIndex = GetSampleIndex(to);
			for (int i = num2; i < sampleIndex; i++)
			{
				num += Vector3.Distance(samples[i].position, b);
				b = samples[i].position;
			}
			return num + Vector3.Distance(EvaluatePosition(to), b);
		}

		private void RemoveSubscriber(int index)
		{
			SplineUser[] array = new SplineUser[subscribers.Length - 1];
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (i != index)
				{
					if (i < index)
					{
						array[i] = subscribers[i];
					}
					else
					{
						array[i - 1] = subscribers[i];
					}
				}
			}
			subscribers = array;
		}

		private bool IsSubscribed(SplineUser user)
		{
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (subscribers[i] == user)
				{
					return true;
				}
			}
			return false;
		}
	}
}
