using System;
using UnityEngine;

namespace UniRx.Diagnostics
{
	public class UnityDebugSink : IObserver<LogEntry>
	{
		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
		}

		public void OnNext(LogEntry value)
		{
			object context = value.Context;
			switch (value.LogType)
			{
			case LogType.Assert:
				break;
			case LogType.Error:
				if (context == null)
				{
					UnityEngine.Debug.LogError(value.Message);
				}
				else
				{
					UnityEngine.Debug.LogError(value.Message, value.Context);
				}
				break;
			case LogType.Exception:
				if (context == null)
				{
					UnityEngine.Debug.LogException(value.Exception);
				}
				else
				{
					UnityEngine.Debug.LogException(value.Exception, value.Context);
				}
				break;
			case LogType.Log:
				if (context == null)
				{
					UnityEngine.Debug.Log(value.Message);
				}
				else
				{
					UnityEngine.Debug.Log(value.Message, value.Context);
				}
				break;
			case LogType.Warning:
				if (context == null)
				{
					UnityEngine.Debug.LogWarning(value.Message);
				}
				else
				{
					UnityEngine.Debug.LogWarning(value.Message, value.Context);
				}
				break;
			}
		}
	}
}
