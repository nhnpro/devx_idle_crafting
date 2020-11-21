using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Spline Follower")]
	public class SplineFollower : SplineTracer
	{
		public enum FollowMode
		{
			Uniform,
			Time
		}

		public enum Wrap
		{
			Default,
			Loop,
			PingPong
		}

		[HideInInspector]
		public Wrap wrapMode;

		[HideInInspector]
		public FollowMode followMode;

		[HideInInspector]
		[FormerlySerializedAs("findStartPoint")]
		public bool autoStartPosition;

		[HideInInspector]
		public bool autoFollow = true;

		[SerializeField]
		[HideInInspector]
		private float _followSpeed = 1f;

		[SerializeField]
		[HideInInspector]
		private float _followDuration = 1f;

		private double lastClippedPercent = -1.0;

		public float followSpeed
		{
			get
			{
				return _followSpeed;
			}
			set
			{
				if (_followSpeed != value)
				{
					if (value < 0f)
					{
						value = 0f;
					}
					_followSpeed = value;
				}
			}
		}

		public float followDuration
		{
			get
			{
				return _followDuration;
			}
			set
			{
				if (_followDuration != value)
				{
					if (value < 0f)
					{
						value = 0f;
					}
					_followDuration = value;
				}
			}
		}

		[Obsolete("Deprecated in 1.0.8. Use result instead")]
		public SplineResult followResult => _result;

		[Obsolete("Deprecated in 1.0.8. Use offsettedResult instead")]
		public SplineResult offsettedFollowResult => base.offsettedResult;

		public event SplineReachHandler onEndReached;

		public event SplineReachHandler onBeginningReached;

		protected override void Start()
		{
			base.Start();
			if (autoStartPosition)
			{
				SetPercent(Project(GetTransform().position).percent);
			}
		}

		protected override void LateRun()
		{
			base.LateRun();
			if (autoFollow)
			{
				AutoFollow();
			}
		}

		protected override void PostBuild()
		{
			base.PostBuild();
			if (base.samples.Length != 0)
			{
				EvaluateClipped(_result, ClipPercent(_result.percent));
				if (autoFollow && !autoStartPosition)
				{
					ApplyMotion();
				}
			}
		}

		private void AutoFollow()
		{
			switch (followMode)
			{
			case FollowMode.Uniform:
				Move(Time.deltaTime * _followSpeed);
				break;
			case FollowMode.Time:
				if ((double)_followDuration == 0.0)
				{
					Move(0.0);
				}
				else
				{
					Move((double)Time.deltaTime / (double)_followDuration);
				}
				break;
			}
		}

		public void Restart(double startPosition = 0.0)
		{
			ResetTriggers();
			SetPercent(startPosition);
		}

		public override void SetPercent(double percent, bool checkTriggers = false)
		{
			if (base.clippedSamples.Length == 0)
			{
				GetClippedSamplesImmediate();
			}
			base.SetPercent(percent, checkTriggers);
			lastClippedPercent = percent;
		}

		public override void SetDistance(float distance, bool checkTriggers = false)
		{
			base.SetDistance(distance, checkTriggers);
			lastClippedPercent = ClipPercent(_result.percent);
			if (base.samplesAreLooped && base.clipFrom == base.clipTo && distance > 0f && lastClippedPercent == 0.0)
			{
				lastClippedPercent = 1.0;
			}
		}

		public void Move(double percent)
		{
			if (percent == 0.0)
			{
				return;
			}
			if (base.clippedSamples.Length <= 1)
			{
				if (base.clippedSamples.Length == 1)
				{
					_result.CopyFrom(base.clippedSamples[0]);
					ApplyMotion();
				}
				return;
			}
			EvaluateClipped(_result, ClipPercent(_result.percent));
			double num = ClipPercent(_result.percent);
			if (wrapMode == Wrap.Default && lastClippedPercent >= 1.0 && num == 0.0)
			{
				num = 1.0;
			}
			double num2 = num + ((_direction != Spline.Direction.Forward) ? (0.0 - percent) : percent);
			bool flag = false;
			bool flag2 = false;
			lastClippedPercent = num2;
			if (_direction == Spline.Direction.Forward && num2 >= 1.0)
			{
				if (this.onEndReached != null)
				{
					flag = true;
				}
				switch (wrapMode)
				{
				case Wrap.Default:
					num2 = 1.0;
					break;
				case Wrap.Loop:
					CheckTriggersClipped(UnclipPercent(num), UnclipPercent(1.0));
					while (num2 > 1.0)
					{
						num2 -= 1.0;
					}
					num = 0.0;
					break;
				case Wrap.PingPong:
					num2 = DMath.Clamp01(1.0 - (num2 - 1.0));
					num = 1.0;
					_direction = Spline.Direction.Backward;
					break;
				}
			}
			else if (_direction == Spline.Direction.Backward && num2 <= 0.0)
			{
				if (this.onBeginningReached != null)
				{
					flag2 = true;
				}
				switch (wrapMode)
				{
				case Wrap.Default:
					num2 = 0.0;
					break;
				case Wrap.Loop:
					CheckTriggersClipped(UnclipPercent(num), UnclipPercent(0.0));
					for (; num2 < 0.0; num2 += 1.0)
					{
					}
					num = 1.0;
					break;
				case Wrap.PingPong:
					num2 = DMath.Clamp01(0.0 - num2);
					num = 0.0;
					_direction = Spline.Direction.Forward;
					break;
				}
			}
			CheckTriggersClipped(UnclipPercent(num), UnclipPercent(num2));
			Evaluate(_result, UnclipPercent(num2));
			ApplyMotion();
			if (flag)
			{
				this.onEndReached();
			}
			else if (flag2)
			{
				this.onBeginningReached();
			}
			InvokeTriggers();
		}

		public void Move(float distance)
		{
			if (distance < 0f)
			{
				distance = 0f;
			}
			if (distance == 0f)
			{
				return;
			}
			if (base.samples.Length <= 1)
			{
				if (base.samples.Length == 1)
				{
					_result.CopyFrom(base.samples[0]);
					ApplyMotion();
				}
				return;
			}
			bool flag = false;
			bool flag2 = false;
			float num = 0f;
			int num2 = 0;
			double num3 = ClipPercent(_result.percent);
			if (num3 == 0.0 && lastClippedPercent == 1.0 && base.clipFrom == base.clipTo)
			{
				num3 = 1.0;
			}
			SplineResult splineResult = new SplineResult(_result);
			SplineResult splineResult2 = new SplineResult(_result);
			if (_direction == Spline.Direction.Forward)
			{
				num2 = DMath.FloorInt(num3 * (double)(base.clippedSamples.Length - 1)) - 1;
				if (num2 < 0)
				{
					num2 = 0;
				}
				for (int i = num2; i < base.clippedSamples.Length - 1 && !(ClipPercent(base.clippedSamples[i].percent) > num3); i++)
				{
					num2 = i;
				}
			}
			else
			{
				num2 = DMath.CeilInt(num3 * (double)(base.clippedSamples.Length - 1)) + 1;
				if (num2 >= base.clippedSamples.Length)
				{
					num2 = base.clippedSamples.Length - 1;
				}
				for (int num4 = num2; num4 >= 1; num4--)
				{
					double num5 = ClipPercent(base.clippedSamples[num4].percent);
					if (num4 == base.clippedSamples.Length - 1)
					{
						num5 = 1.0;
					}
					if (num5 < num3)
					{
						break;
					}
					num2 = num4;
				}
			}
			while (num < distance)
			{
				splineResult.CopyFrom(splineResult2);
				if (_direction == Spline.Direction.Forward)
				{
					num2++;
					if ((base.samplesAreLooped && _result.percent >= base.clippedSamples[base.clippedSamples.Length - 1].percent && _result.percent < base.clippedSamples[0].percent) || (!base.samplesAreLooped && _result.percent >= base.clippedSamples[base.clippedSamples.Length - 1].percent) || num2 >= base.clippedSamples.Length)
					{
						if (this.onEndReached != null)
						{
							flag = true;
						}
						if (wrapMode == Wrap.Default)
						{
							lastClippedPercent = 1.0;
							_result.CopyFrom(base.clippedSamples[base.clippedSamples.Length - 1]);
							CheckTriggersClipped(splineResult.percent, _result.percent);
							break;
						}
						if (wrapMode == Wrap.Loop)
						{
							CheckTriggersClipped(splineResult.percent, base.clippedSamples[base.clippedSamples.Length - 1].percent);
							splineResult.CopyFrom(base.clippedSamples[0]);
							num2 = 1;
						}
						else if (wrapMode == Wrap.PingPong)
						{
							_direction = Spline.Direction.Backward;
							CheckTriggersClipped(splineResult.percent, base.clippedSamples[base.clippedSamples.Length - 1].percent);
							splineResult.CopyFrom(base.clippedSamples[base.clippedSamples.Length - 1]);
							num2 = base.clippedSamples.Length - 2;
						}
					}
				}
				else
				{
					num2--;
					if ((base.samplesAreLooped && _result.percent <= base.clippedSamples[0].percent && _result.percent > base.clippedSamples[base.clippedSamples.Length - 1].percent) || (!base.samplesAreLooped && _result.percent <= base.clippedSamples[0].percent) || num2 < 0)
					{
						if (this.onBeginningReached != null)
						{
							flag2 = true;
						}
						if (wrapMode == Wrap.Default)
						{
							lastClippedPercent = 0.0;
							_result.CopyFrom(base.clippedSamples[0]);
							CheckTriggersClipped(splineResult.percent, _result.percent);
							break;
						}
						if (wrapMode == Wrap.Loop)
						{
							CheckTriggersClipped(splineResult.percent, base.clippedSamples[0].percent);
							splineResult.CopyFrom(base.clippedSamples[base.clippedSamples.Length - 1]);
							num2 = base.clippedSamples.Length - 2;
						}
						else if (wrapMode == Wrap.PingPong)
						{
							_direction = Spline.Direction.Forward;
							CheckTriggersClipped(splineResult.percent, base.clippedSamples[0].percent);
							splineResult.CopyFrom(base.clippedSamples[0]);
							num2 = 1;
						}
					}
				}
				lastClippedPercent = ClipPercent(_result.percent);
				splineResult2.CopyFrom(base.clippedSamples[num2]);
				float num6 = Vector3.Distance(splineResult2.position, splineResult.position);
				num += num6;
				if (num >= distance)
				{
					float num7 = num - distance;
					double t = 1.0 - (double)(num7 / num6);
					SplineResult.Lerp(splineResult, splineResult2, t, _result);
					if (base.samplesAreLooped)
					{
						if (_direction == Spline.Direction.Forward && splineResult.percent > splineResult2.percent)
						{
							_result.percent = DMath.Lerp(splineResult.percent, 1.0, t);
						}
						else if (_direction == Spline.Direction.Backward && splineResult.percent < splineResult2.percent)
						{
							_result.percent = DMath.Lerp(1.0, splineResult2.percent, t);
						}
					}
					CheckTriggersClipped(splineResult.percent, _result.percent);
					break;
				}
				CheckTriggersClipped(splineResult.percent, splineResult2.percent);
			}
			ApplyMotion();
			if (flag)
			{
				this.onEndReached();
			}
			else if (flag2)
			{
				this.onBeginningReached();
			}
			InvokeTriggers();
		}
	}
}
