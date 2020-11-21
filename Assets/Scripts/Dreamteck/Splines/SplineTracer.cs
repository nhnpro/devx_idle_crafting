using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	public class SplineTracer : SplineUser
	{
		public enum PhysicsMode
		{
			Transform,
			Rigidbody,
			Rigidbody2D
		}

		private Trigger[] triggerInvokeQueue = new Trigger[0];

		private int addTriggerIndex;

		[HideInInspector]
		public bool applyDirectionRotation = true;

		[SerializeField]
		[HideInInspector]
		protected Spline.Direction _direction = Spline.Direction.Forward;

		[SerializeField]
		[HideInInspector]
		protected PhysicsMode _physicsMode;

		[SerializeField]
		[HideInInspector]
		protected TransformModule _motion;

		[HideInInspector]
		[SerializeField]
		[FormerlySerializedAs("triggers")]
		protected SplineTrigger[] triggers_old = new SplineTrigger[0];

		[HideInInspector]
		public Trigger[] triggers = new Trigger[0];

		[SerializeField]
		[HideInInspector]
		protected CustomRotationModule _customRotations = new CustomRotationModule();

		[SerializeField]
		[HideInInspector]
		protected CustomOffsetModule _customOffsets = new CustomOffsetModule();

		[SerializeField]
		[HideInInspector]
		protected Rigidbody targetRigidbody;

		[SerializeField]
		[HideInInspector]
		protected Rigidbody2D targetRigidbody2D;

		[SerializeField]
		[HideInInspector]
		protected Transform targetTransform;

		[SerializeField]
		[HideInInspector]
		protected SplineResult _result = new SplineResult();

		private bool setPercentOnRebuild;

		private double targetPercentOnRebuild;

		public PhysicsMode physicsMode
		{
			get
			{
				return _physicsMode;
			}
			set
			{
				_physicsMode = value;
				RefreshTargets();
			}
		}

		public TransformModule motion
		{
			get
			{
				if (_motion == null)
				{
					_motion = new TransformModule();
				}
				return _motion;
			}
		}

		public CustomRotationModule customRotations
		{
			get
			{
				if (_customRotations == null)
				{
					_customRotations = new CustomRotationModule();
				}
				return _customRotations;
			}
		}

		public CustomOffsetModule customOffsets
		{
			get
			{
				if (_customOffsets == null)
				{
					_customOffsets = new CustomOffsetModule();
				}
				return _customOffsets;
			}
		}

		public SplineResult result => _result;

		public SplineResult offsettedResult
		{
			get
			{
				SplineResult splineResult = new SplineResult(_result);
				Vector2 vector = customOffsets.Evaluate(splineResult.percent);
				Vector3 direction = splineResult.direction;
				SplineResult splineResult2 = splineResult;
				Vector3 position = splineResult2.position;
				Vector3 right = splineResult.right;
				Vector2 offset = motion.offset;
				Vector3 a = right * (offset.x + vector.x);
				Vector3 normal = splineResult.normal;
				Vector2 offset2 = motion.offset;
				splineResult2.position = position + (a + normal * (offset2.y + vector.y));
				Quaternion rotation = customRotations.Evaluate(Quaternion.Euler(motion.rotationOffset) * Quaternion.LookRotation(splineResult.direction, splineResult.normal), splineResult.percent);
				splineResult.direction = rotation * Vector3.forward;
				rotation = customRotations.Evaluate(Quaternion.Euler(motion.rotationOffset) * Quaternion.LookRotation(splineResult.normal, direction), splineResult.percent);
				splineResult.normal = rotation * Vector3.forward;
				return splineResult;
			}
		}

		public Spline.Direction direction
		{
			get
			{
				return _direction;
			}
			set
			{
				if (value != _direction)
				{
					_direction = value;
					ApplyMotion();
				}
			}
		}

		public double clampedPercent => ClipPercent(_result.percent);

		[Obsolete("Deprecated in version 1.0.7. Use motion.applyPosition instead")]
		public bool applyPosition
		{
			get
			{
				return motion.applyPosition;
			}
			set
			{
				motion.applyPosition = value;
			}
		}

		[Obsolete("Deprecated in version 1.0.7. Use motion.applyRotation instead")]
		public bool applyRotation
		{
			get
			{
				return motion.applyRotation;
			}
			set
			{
				motion.applyRotation = value;
			}
		}

		[Obsolete("Deprecated in version 1.0.7. Use motion.applyScale instead")]
		public bool applyScale
		{
			get
			{
				return motion.applyScale;
			}
			set
			{
				motion.applyScale = value;
			}
		}

		[Obsolete("Deprecated in version 1.0.7. User motion.offset instead")]
		public Vector2 offset
		{
			get
			{
				return motion.offset;
			}
			set
			{
				motion.offset = value;
			}
		}

		[Obsolete("Deprecated in version 1.0.7. User motion.rotationOffset instead")]
		public Vector3 rotationOffset
		{
			get
			{
				return motion.rotationOffset;
			}
			set
			{
				motion.rotationOffset = value;
			}
		}

		protected virtual void Start()
		{
			RefreshTargets();
		}

		public Node GetNextNode()
		{
			double percent = 0.0;
			Spline.Direction direction = Spline.Direction.Forward;
			_address.GetEvaluationValues(_result.percent, out SplineComputer computer, out percent, out direction);
			if (_direction == Spline.Direction.Backward)
			{
				direction = ((direction != Spline.Direction.Forward) ? Spline.Direction.Forward : Spline.Direction.Backward);
			}
			int[] availableNodeLinksAtPosition = computer.GetAvailableNodeLinksAtPosition(percent, direction);
			if (availableNodeLinksAtPosition.Length == 0)
			{
				return null;
			}
			if (direction == Spline.Direction.Forward)
			{
				int num = computer.pointCount - 1;
				int num2 = 0;
				for (int i = 0; i < availableNodeLinksAtPosition.Length; i++)
				{
					if (computer.nodeLinks[availableNodeLinksAtPosition[i]].pointIndex < num)
					{
						num = computer.nodeLinks[availableNodeLinksAtPosition[i]].pointIndex;
						num2 = i;
					}
				}
				return computer.nodeLinks[availableNodeLinksAtPosition[num2]].node;
			}
			int num3 = 0;
			int num4 = 0;
			for (int j = 0; j < availableNodeLinksAtPosition.Length; j++)
			{
				if (computer.nodeLinks[availableNodeLinksAtPosition[j]].pointIndex > num3)
				{
					num3 = computer.nodeLinks[availableNodeLinksAtPosition[j]].pointIndex;
					num4 = j;
				}
			}
			return computer.nodeLinks[availableNodeLinksAtPosition[num4]].node;
		}

		public void GetCurrentComputer(out SplineComputer comp, out double percent, out Spline.Direction dir)
		{
			_address.GetEvaluationValues(_result.percent, out comp, out percent, out dir);
		}

		public void ResetTriggers()
		{
			for (int i = 0; i < triggers.Length; i++)
			{
				triggers[i].ResetWorkOnce();
			}
		}

		public virtual void SetPercent(double percent, bool checkTriggers = false)
		{
			if (base.samples.Length != 0)
			{
				double percent2 = _result.percent;
				UnclipPercent(ref percent);
				Evaluate(_result, percent);
				ApplyMotion();
				if (checkTriggers)
				{
					CheckTriggersClipped(percent2, percent);
					InvokeTriggers();
				}
			}
		}

		public virtual void SetDistance(float distance, bool checkTriggers = false)
		{
			if (base.clippedSamples.Length != 0)
			{
				double percent = _result.percent;
				EvaluateClipped(_result, TravelClipped(0.0, distance, Spline.Direction.Forward));
				ApplyMotion();
				if (checkTriggers)
				{
					CheckTriggersClipped(percent, _result.percent);
					InvokeTriggers();
				}
			}
		}

		protected override void PostBuild()
		{
			if (setPercentOnRebuild)
			{
				SetPercent(targetPercentOnRebuild);
				setPercentOnRebuild = false;
			}
		}

		public override void EnterAddress(Node node, int pointIndex, Spline.Direction direction = Spline.Direction.Forward)
		{
			int elementIndex = _address.GetElementIndex(_result.percent);
			double localPercent = _address.PathToLocalPercent(_result.percent, elementIndex);
			base.EnterAddress(node, pointIndex, direction);
			double num = _address.LocalToPathPercent(localPercent, elementIndex);
			setPercentOnRebuild = true;
			targetPercentOnRebuild = num;
		}

		public override void AddComputer(SplineComputer computer, int connectionIndex, int connectedIndex, Spline.Direction direction = Spline.Direction.Forward)
		{
			int elementIndex = _address.GetElementIndex(_result.percent);
			double localPercent = _address.PathToLocalPercent(_result.percent, elementIndex);
			base.AddComputer(computer, connectionIndex, connectedIndex, direction);
			double num = _address.LocalToPathPercent(localPercent, elementIndex);
			setPercentOnRebuild = true;
			targetPercentOnRebuild = num;
		}

		public override void ExitAddress(int depth)
		{
			int elementIndex = _address.GetElementIndex(_result.percent);
			double localPercent = _address.PathToLocalPercent(_result.percent, elementIndex);
			base.ExitAddress(depth);
			double num = _address.LocalToPathPercent(localPercent, elementIndex);
			setPercentOnRebuild = true;
			targetPercentOnRebuild = num;
		}

		protected virtual Rigidbody GetRigidbody()
		{
			return GetComponent<Rigidbody>();
		}

		protected virtual Rigidbody2D GetRigidbody2D()
		{
			return GetComponent<Rigidbody2D>();
		}

		protected virtual Transform GetTransform()
		{
			return base.transform;
		}

		protected void ApplyMotion()
		{
			motion.targetUser = this;
			motion.splineResult = _result;
			motion.customRotation = _customRotations;
			motion.customOffset = _customOffsets;
			if (applyDirectionRotation)
			{
				motion.direction = _direction;
			}
			else
			{
				motion.direction = Spline.Direction.Forward;
			}
			switch (_physicsMode)
			{
			case PhysicsMode.Transform:
				if (targetTransform == null)
				{
					RefreshTargets();
				}
				if (!(targetTransform == null))
				{
					motion.ApplyTransform(targetTransform);
				}
				break;
			case PhysicsMode.Rigidbody:
				if (targetRigidbody == null)
				{
					RefreshTargets();
					if (targetRigidbody == null)
					{
						throw new MissingComponentException("There is no Rigidbody attached to " + base.name + " but the Physics mode is set to use one.");
					}
				}
				motion.ApplyRigidbody(targetRigidbody);
				break;
			case PhysicsMode.Rigidbody2D:
				if (targetRigidbody2D == null)
				{
					RefreshTargets();
					if (targetRigidbody2D == null)
					{
						throw new MissingComponentException("There is no Rigidbody2D attached to " + base.name + " but the Physics mode is set to use one.");
					}
				}
				motion.ApplyRigidbody2D(targetRigidbody2D);
				break;
			}
		}

		protected void CheckTriggers(double from, double to)
		{
			for (int i = 0; i < triggers.Length; i++)
			{
				if (triggers[i] != null && triggers[i].Check(from, to))
				{
					AddTriggerToQueue(triggers[i]);
				}
			}
		}

		protected void CheckTriggersClipped(double from, double to)
		{
			if (_direction == Spline.Direction.Forward)
			{
				if (from <= to)
				{
					CheckTriggers(from, to);
					return;
				}
				CheckTriggers(from, 1.0);
				CheckTriggers(0.0, to);
			}
			else if (from >= to)
			{
				CheckTriggers(from, to);
			}
			else
			{
				CheckTriggers(from, 0.0);
				CheckTriggers(1.0, to);
			}
		}

		protected void InvokeTriggers()
		{
			for (int i = 0; i < addTriggerIndex; i++)
			{
				if (triggerInvokeQueue[i] != null)
				{
					triggerInvokeQueue[i].Invoke();
				}
			}
			addTriggerIndex = 0;
		}

		private bool MigrateTriggers()
		{
			if (triggers_old.Length > 0)
			{
				Trigger[] array = new Trigger[triggers_old.Length];
				for (int i = 0; i < triggers_old.Length; i++)
				{
					if (!(triggers_old[i] == null))
					{
						array[i] = new Trigger();
						array[i].name = triggers_old[i].name;
						array[i].color = triggers_old[i].color;
						array[i].position = triggers_old[i].position;
						array[i].type = (Trigger.Type)triggers_old[i].type;
						array[i].actions = triggers_old[i].actions;
						array[i].gameObjects = triggers_old[i].gameObjects;
					}
				}
				triggers_old = new SplineTrigger[0];
				triggers = array;
				return true;
			}
			return false;
		}

		protected void RefreshTargets()
		{
			switch (_physicsMode)
			{
			case PhysicsMode.Transform:
				targetTransform = GetTransform();
				break;
			case PhysicsMode.Rigidbody:
				targetRigidbody = GetRigidbody();
				break;
			case PhysicsMode.Rigidbody2D:
				targetRigidbody2D = GetRigidbody2D();
				break;
			}
		}

		private void AddTriggerToQueue(Trigger trigger)
		{
			if (addTriggerIndex >= triggerInvokeQueue.Length)
			{
				Trigger[] array = new Trigger[triggerInvokeQueue.Length + triggers.Length];
				triggerInvokeQueue.CopyTo(array, 0);
				triggerInvokeQueue = array;
			}
			triggerInvokeQueue[addTriggerIndex] = trigger;
			addTriggerIndex++;
		}

		private void AddTrigger(Trigger trigger)
		{
			Trigger[] array = new Trigger[triggers.Length + 1];
			triggers.CopyTo(array, 0);
			array[array.Length - 1] = trigger;
			triggers = array;
		}

		public void AddTrigger(Trigger.Type t, UnityAction call, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void AddTrigger(Trigger.Type t, UnityAction<int> call, int value, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call, value);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void AddTrigger(Trigger.Type t, UnityAction<float> call, float value, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call, value);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void AddTrigger(Trigger.Type t, UnityAction<double> call, double value, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call, value);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void AddTrigger(Trigger.Type t, UnityAction<string> call, string value, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call, value);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void AddTrigger(Trigger.Type t, UnityAction<bool> call, bool value, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call, value);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void AddTrigger(Trigger.Type t, UnityAction<GameObject> call, GameObject value, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call, value);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void AddTrigger(Trigger.Type t, UnityAction<Transform> call, Transform value, double position = 0.0, Trigger.Type type = Trigger.Type.Double)
		{
			Trigger trigger = new Trigger();
			trigger.Create(t, call, value);
			trigger.position = position;
			trigger.type = type;
			AddTrigger(trigger);
		}

		public void EvaluateClipped(SplineResult result, double clippedPercent)
		{
			Evaluate(result, UnclipPercent(clippedPercent));
		}

		public Vector3 EvaluatePositionClipped(double clippedPercent)
		{
			return EvaluatePosition(UnclipPercent(clippedPercent));
		}

		public double TravelClipped(double start, float distance, Spline.Direction direction)
		{
			if (base.clippedSamples.Length <= 1)
			{
				return 0.0;
			}
			if (distance == 0f)
			{
				return start;
			}
			float num = 0f;
			Vector3 b = EvaluatePositionClipped(start);
			double a = start;
			int num2 = (direction != Spline.Direction.Forward) ? DMath.FloorInt(start * (double)(base.clippedSamples.Length - 1)) : DMath.CeilInt(start * (double)(base.clippedSamples.Length - 1));
			float num3 = 0f;
			while (true)
			{
				num3 = Vector3.Distance(base.clippedSamples[num2].position, b);
				b = base.clippedSamples[num2].position;
				num += num3;
				if (num >= distance)
				{
					break;
				}
				a = ClipPercent(base.clippedSamples[num2].percent);
				if (direction == Spline.Direction.Forward)
				{
					if (num2 == base.clippedSamples.Length - 1)
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
			return DMath.Lerp(a, ClipPercent(base.clippedSamples[num2].percent), 1f - (num - distance) / num3);
		}

		public SplineResult ProjectClipped(Vector3 point)
		{
			return Project(point, base.clipFrom, base.clipTo);
		}
	}
}
