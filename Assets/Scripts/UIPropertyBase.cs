using System;
using System.Reflection;
using UniRx;
using UnityEngine;

public class UIPropertyBase : MonoBehaviour
{
	[SerializeField]
	private string m_class;

	[SerializeField]
	private string m_field;

	protected IReadOnlyReactiveProperty<T> GetProperty<T>()
	{
		Type type = Type.GetType(m_class);
		object context = Singleton<PropertyManager>.Instance.GetContext(m_class, base.transform);
		FieldInfo field = type.GetField(m_field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		return field.GetValue(context) as IReadOnlyReactiveProperty<T>;
	}
}
