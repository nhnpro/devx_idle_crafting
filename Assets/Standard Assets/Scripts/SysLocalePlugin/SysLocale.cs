using UnityEngine;

namespace SysLocalePlugin
{
	public class SysLocale
	{
		public static string getSystemLocale()
		{
			string empty = string.Empty;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]))
				{
					return androidJavaObject.Call<string>("toString", new object[0]);
				}
			}
		}
	}
}
