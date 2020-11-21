using System;
using UnityEngine;

namespace UniRx
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class InspectorDisplayAttribute : PropertyAttribute
	{
		public string FieldName
		{
			get;
			private set;
		}

		public bool NotifyPropertyChanged
		{
			get;
			private set;
		}

		public InspectorDisplayAttribute(string fieldName = "value", bool notifyPropertyChanged = true)
		{
			FieldName = fieldName;
			NotifyPropertyChanged = notifyPropertyChanged;
		}
	}
}
