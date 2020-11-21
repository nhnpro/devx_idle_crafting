using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamteck.Splines
{
	[Serializable]
	public class Trigger
	{
		public enum Type
		{
			Double,
			Forward,
			Backward
		}

		public string name = "Trigger";

		[SerializeField]
		public Type type;

		public bool workOnce;

		private bool worked;

		[Range(0f, 1f)]
		public double position = 0.5;

		[SerializeField]
		public bool enabled = true;

		[SerializeField]
		public Color color = Color.white;

		[SerializeField]
		[HideInInspector]
		public SplineAction[] actions = new SplineAction[0];

		public GameObject[] gameObjects;

		public void Create(Type t, UnityAction call)
		{
			Create(t);
			AddAction(new SplineAction(call));
		}

		public void Create(Type t, UnityAction<int> call, int value)
		{
			AddAction(new SplineAction(call, value));
		}

		public void Create(Type t, UnityAction<float> call, float value)
		{
			AddAction(new SplineAction(call, value));
		}

		public void Create(Type t, UnityAction<double> call, double value)
		{
			AddAction(new SplineAction(call, value));
		}

		public void Create(Type t, UnityAction<string> call, string value)
		{
			AddAction(new SplineAction(call, value));
		}

		public void Create(Type t, UnityAction<bool> call, bool value)
		{
			AddAction(new SplineAction(call, value));
		}

		public void Create(Type t, UnityAction<Transform> call, Transform value)
		{
			AddAction(new SplineAction(call, value));
		}

		public void Create(Type t, UnityAction<GameObject> call, GameObject value)
		{
			AddAction(new SplineAction(call, value));
		}

		public void Create(Type t)
		{
			type = t;
			switch (t)
			{
			case Type.Double:
				color = Color.yellow;
				break;
			case Type.Forward:
				color = Color.green;
				break;
			case Type.Backward:
				color = Color.red;
				break;
			}
			enabled = true;
		}

		public void ResetWorkOnce()
		{
			worked = false;
		}

		public bool Check(double previousPercent, double currentPercent)
		{
			if (!enabled)
			{
				return false;
			}
			if (workOnce && worked)
			{
				return false;
			}
			bool flag = false;
			switch (type)
			{
			case Type.Double:
				flag = ((previousPercent <= position && currentPercent >= position) || (currentPercent <= position && previousPercent >= position));
				break;
			case Type.Forward:
				flag = (previousPercent <= position && currentPercent >= position);
				break;
			case Type.Backward:
				flag = (currentPercent <= position && previousPercent >= position);
				break;
			}
			if (flag)
			{
				worked = true;
			}
			return flag;
		}

		public void Invoke()
		{
			for (int i = 0; i < actions.Length; i++)
			{
				actions[i].Invoke();
			}
		}

		private void AddAction()
		{
			SplineAction[] array = new SplineAction[actions.Length + 1];
			actions.CopyTo(array, 0);
			array[array.Length - 1] = new SplineAction();
			actions = array;
		}

		public void AddListener(MonoBehaviour behavior, string method, object arg)
		{
			System.Type type = null;
			if (arg.GetType() == typeof(int))
			{
				type = typeof(int);
			}
			else if (arg.GetType() == typeof(float))
			{
				type = typeof(float);
			}
			else if (arg.GetType() == typeof(double))
			{
				type = typeof(double);
			}
			else if (arg.GetType() == typeof(string))
			{
				type = typeof(string);
			}
			else if (arg.GetType() == typeof(bool))
			{
				type = typeof(bool);
			}
			else if (arg.GetType() == typeof(GameObject))
			{
				type = typeof(GameObject);
			}
			else if (arg.GetType() == typeof(Transform))
			{
				type = typeof(Transform);
			}
			MethodInfo methodInfo = (type != null) ? behavior.GetType().GetMethod(method, new System.Type[1]
			{
				type
			}) : behavior.GetType().GetMethod(method, new System.Type[0]);
			if (methodInfo == null)
			{
				UnityEngine.Debug.LogError("There is no overload of the method " + method + " that uses a " + type + " parameter");
				return;
			}
			AddAction();
			actions[actions.Length - 1].target = behavior;
			actions[actions.Length - 1].SetMethod(methodInfo);
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length == 1)
			{
				if (parameters[0].ParameterType == typeof(int))
				{
					actions[actions.Length - 1].intValue = (int)arg;
				}
				else if (parameters[0].ParameterType == typeof(float))
				{
					actions[actions.Length - 1].floatValue = (float)arg;
				}
				else if (parameters[0].ParameterType == typeof(double))
				{
					actions[actions.Length - 1].doubleValue = (double)arg;
				}
				else if (parameters[0].ParameterType == typeof(string))
				{
					actions[actions.Length - 1].stringValue = (string)arg;
				}
				else if (parameters[0].ParameterType == typeof(bool))
				{
					actions[actions.Length - 1].boolValue = (bool)arg;
				}
				else if (parameters[0].ParameterType == typeof(GameObject))
				{
					actions[actions.Length - 1].goValue = (GameObject)arg;
				}
				else if (parameters[0].ParameterType == typeof(Transform))
				{
					actions[actions.Length - 1].transformValue = (Transform)arg;
				}
			}
		}

		public void AddAction(SplineAction action)
		{
			AddAction();
			actions[actions.Length - 1] = action;
		}
	}
}
