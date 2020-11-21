using System;
using UnityEngine;

namespace notif
{
	public class LocalNotification
	{
		public enum Importance
		{
			Default = 3,
			High = 4,
			Low = 2,
			Max = 5,
			Min = 1,
			None = 0
		}

		public class Action
		{
			public string Identifier;

			public string Title;

			public string Icon;

			public bool Foreground = true;

			public string GameObject;

			public string HandlerMethod;

			public Action(string identifier, string title, MonoBehaviour handler)
			{
				Identifier = identifier;
				Title = title;
				if (handler != null)
				{
					GameObject = handler.gameObject.name;
					HandlerMethod = "OnAction";
				}
			}
		}

		private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";

		private static string actionClassName = "net.agasper.unitynotification.NotificationAction";

		private static string bundleIdentifier => Application.identifier;

		public static int SendNotification(TimeSpan delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string soundName = null, string channel = "default", params Action[] actions)
		{
			int id = new System.Random().Next();
			return SendNotification(id, (long)delay.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon, soundName, channel, actions);
		}

		public static int SendNotification(int id, TimeSpan delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string soundName = null, string channel = "default", params Action[] actions)
		{
			return SendNotification(id, (long)delay.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon, soundName, channel, actions);
		}

		public static int SendNotification(int id, long delayMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string soundName = null, string channel = "default", params Action[] actions)
		{
			new AndroidJavaClass(fullClassName)?.CallStatic("SetNotification", id, delayMs, title, message, message, sound ? 1 : 0, soundName, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", ToInt(bgColor), bundleIdentifier, channel, PopulateActions(actions));
			return id;
		}

		public static int SendRepeatingNotification(TimeSpan delay, TimeSpan timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string soundName = null, string channel = "default", params Action[] actions)
		{
			int id = new System.Random().Next();
			return SendRepeatingNotification(id, (long)delay.TotalSeconds * 1000, (int)timeout.TotalSeconds, title, message, bgColor, sound, vibrate, lights, bigIcon, soundName, channel, actions);
		}

		public static int SendRepeatingNotification(int id, TimeSpan delay, TimeSpan timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string soundName = null, string channel = "default", params Action[] actions)
		{
			return SendRepeatingNotification(id, (long)delay.TotalSeconds * 1000, (int)timeout.TotalSeconds, title, message, bgColor, sound, vibrate, lights, bigIcon, soundName, channel, actions);
		}

		public static int SendRepeatingNotification(int id, long delayMs, long timeoutMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", string soundName = null, string channel = "default", params Action[] actions)
		{
			return id;
			new AndroidJavaClass(fullClassName)?.CallStatic("SetRepeatingNotification", id, delayMs, title, message, message, timeoutMs, sound ? 1 : 0, soundName, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", ToInt(bgColor), bundleIdentifier, channel, PopulateActions(actions));
			return id;
		}

		public static void CancelNotification(int id)
		{
			return;
			new AndroidJavaClass(fullClassName)?.CallStatic("CancelPendingNotification", id);
		}

		public static void ClearNotifications()
		{
			return;
			new AndroidJavaClass(fullClassName)?.CallStatic("ClearShowingNotifications");
		}

		public static void CreateChannel(string identifier, string name, string description, Color32 lightColor, bool enableLights = true, string soundName = null, Importance importance = Importance.Default, bool vibrate = true, long[] vibrationPattern = null)
		{
			return;
			new AndroidJavaClass(fullClassName)?.CallStatic("CreateChannel", identifier, name, description, (int)importance, soundName, enableLights ? 1 : 0, ToInt(lightColor), vibrate ? 1 : 0, vibrationPattern, bundleIdentifier);
		}

		private static int ToInt(Color32 color)
		{
			return color.r * 65536 + color.g * 256 + color.b;
		}

		private static AndroidJavaObject PopulateActions(Action[] actions)
		{
			return null;
			AndroidJavaObject androidJavaObject = null;
			if (actions.Length > 0)
			{
				androidJavaObject = new AndroidJavaObject("java.util.ArrayList");
				foreach (Action action in actions)
				{
					using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject(actionClassName))
					{
						androidJavaObject2.Call("setIdentifier", action.Identifier);
						androidJavaObject2.Call("setTitle", action.Title);
						androidJavaObject2.Call("setIcon", action.Icon);
						androidJavaObject2.Call("setForeground", action.Foreground);
						androidJavaObject2.Call("setGameObject", action.GameObject);
						androidJavaObject2.Call("setHandlerMethod", action.HandlerMethod);
						androidJavaObject.Call<bool>("add", new object[1]
						{
							androidJavaObject2
						});
					}
				}
			}
			return androidJavaObject;
		}
	}
}
