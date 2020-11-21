using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamteck.Splines
{
	[Serializable]
	public class SplineAction
	{
		[SerializeField]
		public UnityEngine.Object target;

		public int intValue;

		public float floatValue;

		public double doubleValue;

		public string stringValue;

		public bool boolValue;

		public GameObject goValue;

		public Transform transformValue;

		private UnityAction action;

		private UnityAction<int> intAction;

		private UnityAction<float> floatAction;

		private UnityAction<double> doubleAction;

		private UnityAction<string> stringAction;

		private UnityAction<bool> boolAction;

		private UnityAction<GameObject> goAction;

		private UnityAction<Transform> transformAction;

		private MethodInfo methodInfo;

		[SerializeField]
		private string methodName = string.Empty;

		[SerializeField]
		private int paramType;

		public SplineAction()
		{
		}

		public SplineAction(UnityAction call)
		{
			action = call;
			paramType = 0;
		}

		public SplineAction(UnityAction<int> call, int value)
		{
			intAction = call;
			intValue = value;
			paramType = 1;
		}

		public SplineAction(UnityAction<float> call, float value)
		{
			floatAction = call;
			floatValue = value;
			paramType = 2;
		}

		public SplineAction(UnityAction<double> call, double value)
		{
			doubleAction = call;
			doubleValue = value;
			paramType = 3;
		}

		public SplineAction(UnityAction<string> call, string value)
		{
			stringAction = call;
			stringValue = value;
			paramType = 4;
		}

		public SplineAction(UnityAction<bool> call, bool value)
		{
			boolAction = call;
			boolValue = value;
			paramType = 5;
		}

		public SplineAction(UnityAction<GameObject> call, GameObject value)
		{
			goAction = call;
			goValue = value;
			paramType = 6;
		}

		public SplineAction(UnityAction<Transform> call, Transform value)
		{
			transformAction = call;
			transformValue = value;
			paramType = 7;
		}

		public void SetMethod(MethodInfo newMethod)
		{
			ParameterInfo[] parameters = newMethod.GetParameters();
			if (parameters.Length > 1)
			{
				UnityEngine.Debug.LogError("Cannot add method with more than one argument");
				return;
			}
			methodInfo = newMethod;
			methodName = methodInfo.Name;
			if (parameters.Length == 0)
			{
				paramType = 0;
			}
			else if (parameters[0].ParameterType == typeof(int))
			{
				paramType = 1;
			}
			else if (parameters[0].ParameterType == typeof(float))
			{
				paramType = 2;
			}
			else if (parameters[0].ParameterType == typeof(double))
			{
				paramType = 3;
			}
			else if (parameters[0].ParameterType == typeof(string))
			{
				paramType = 4;
			}
			else if (parameters[0].ParameterType == typeof(bool))
			{
				paramType = 5;
			}
			else if (parameters[0].ParameterType == typeof(GameObject))
			{
				paramType = 6;
			}
			else if (parameters[0].ParameterType == typeof(Transform))
			{
				paramType = 7;
			}
			ConstructUnityAction();
		}

		private Type GetParamType()
		{
			switch (paramType)
			{
			case 0:
				return null;
			case 1:
				return typeof(int);
			case 2:
				return typeof(float);
			case 3:
				return typeof(double);
			case 4:
				return typeof(string);
			case 5:
				return typeof(bool);
			case 6:
				return typeof(GameObject);
			case 7:
				return typeof(Transform);
			default:
				return null;
			}
		}

		public MethodInfo GetMethod()
		{
			if (methodInfo == null && target != null && methodName != string.Empty)
			{
				Type type = GetParamType();
				if (type == null)
				{
					methodInfo = target.GetType().GetMethod(methodName, new Type[0]);
				}
				else
				{
					methodInfo = target.GetType().GetMethod(methodName, new Type[1]
					{
						type
					});
				}
			}
			return methodInfo;
		}

		private void ConstructUnityAction()
		{
			action = null;
			intAction = null;
			floatAction = null;
			doubleAction = null;
			stringAction = null;
			boolAction = null;
			goAction = null;
			transformAction = null;
			methodInfo = GetMethod();
			switch (paramType)
			{
			case 0:
				action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, methodInfo);
				break;
			case 1:
				intAction = (UnityAction<int>)Delegate.CreateDelegate(typeof(UnityAction<int>), target, methodInfo);
				break;
			case 2:
				floatAction = (UnityAction<float>)Delegate.CreateDelegate(typeof(UnityAction<float>), target, methodInfo);
				break;
			case 3:
				doubleAction = (UnityAction<double>)Delegate.CreateDelegate(typeof(UnityAction<double>), target, methodInfo);
				break;
			case 4:
				stringAction = (UnityAction<string>)Delegate.CreateDelegate(typeof(UnityAction<string>), target, methodInfo);
				break;
			case 5:
				boolAction = (UnityAction<bool>)Delegate.CreateDelegate(typeof(UnityAction<bool>), target, methodInfo);
				break;
			case 6:
				goAction = (UnityAction<GameObject>)Delegate.CreateDelegate(typeof(UnityAction<GameObject>), target, methodInfo);
				break;
			case 7:
				transformAction = (UnityAction<Transform>)Delegate.CreateDelegate(typeof(UnityAction<Transform>), target, methodInfo);
				break;
			}
		}

		public void Invoke()
		{
			switch (paramType)
			{
			case 0:
				if (action == null)
				{
					ConstructUnityAction();
				}
				action();
				break;
			case 1:
				if (intAction == null)
				{
					ConstructUnityAction();
				}
				intAction(intValue);
				break;
			case 2:
				if (floatAction == null)
				{
					ConstructUnityAction();
				}
				floatAction(floatValue);
				break;
			case 3:
				if (doubleAction == null)
				{
					ConstructUnityAction();
				}
				doubleAction(doubleValue);
				break;
			case 4:
				if (stringAction == null)
				{
					ConstructUnityAction();
				}
				stringAction(stringValue);
				break;
			case 5:
				if (boolAction == null)
				{
					ConstructUnityAction();
				}
				boolAction(boolValue);
				break;
			case 6:
				if (goAction == null)
				{
					ConstructUnityAction();
				}
				goAction(goValue);
				break;
			case 7:
				if (transformAction == null)
				{
					ConstructUnityAction();
				}
				transformAction(transformValue);
				break;
			}
		}
	}
}
