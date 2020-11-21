using System;
using System.Collections.Generic;
using UnityEngine;

public class TweakableManagerUI : MonoBehaviour
{
	[SerializeField]
	private TweakableLabelUI m_prefabLabel;

	[SerializeField]
	private TweakableInputFieldUI m_prefabInputField;

	[SerializeField]
	private TweakableIntUI m_prefabInt;

	[SerializeField]
	private TweakableFloatUI m_prefabFloat;

	[SerializeField]
	private TweakableBoolUI m_prefabBool;

	[SerializeField]
	private TweakableTriggerUI m_prefabTrigger;

	[SerializeField]
	private TweakableTabUI m_prefabTab;

	[SerializeField]
	private RectTransform m_tabs;

	[SerializeField]
	private RectTransform m_content;

	private static List<Tweakable> m_listTodo = new List<Tweakable>();

	private static List<TweakableUI> m_listDone = new List<TweakableUI>();

	private string m_activeCategory;

	protected void Update()
	{
		if (m_listTodo.Count > 0)
		{
			Tweakable tweakable = m_listTodo[m_listTodo.Count - 1];
			m_listTodo.RemoveAt(m_listTodo.Count - 1);
			if (tweakable.GetType() == typeof(TweakableLabel))
			{
				CreateLabel((TweakableLabel)tweakable);
			}
			if (tweakable.GetType() == typeof(TweakableInputField))
			{
				CreateInputField((TweakableInputField)tweakable);
			}
			else if (tweakable.GetType() == typeof(TweakableBool))
			{
				CreateBool((TweakableBool)tweakable);
			}
			else if (tweakable.GetType() == typeof(TweakableInt))
			{
				CreateInt((TweakableInt)tweakable);
			}
			else if (tweakable.GetType() == typeof(TweakableFloat))
			{
				CreateFloat((TweakableFloat)tweakable);
			}
			else if (tweakable.GetType() == typeof(TweakableTrigger))
			{
				CreateTrigger((TweakableTrigger)tweakable);
			}
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void UpdateKeyShortCuts()
	{
		foreach (Tweakable item in m_listTodo)
		{
			UpdateKeyShortCut(item);
		}
		foreach (TweakableUI item2 in m_listDone)
		{
			UpdateKeyShortCut(item2.TweakableObject);
		}
	}

	private void UpdateKeyShortCut(Tweakable tw)
	{
		(tw as TweakableTrigger)?.UpdateKeyShortCut();
	}

	public void OnDestroy()
	{
		foreach (TweakableUI item in m_listDone)
		{
			m_listTodo.Add(item.TweakableObject);
		}
		m_listDone.Clear();
	}

	public static void RegisterTweakable(Tweakable tweakable)
	{
		m_listTodo.Add(tweakable);
	}

	private void CreateLabel(TweakableLabel tweakable)
	{
		TweakableLabelUI tweakableLabelUI = UnityEngine.Object.Instantiate(m_prefabLabel);
		tweakableLabelUI.transform.SetParent(m_content, worldPositionStays: false);
		TweakableLabelUI component = tweakableLabelUI.GetComponent<TweakableLabelUI>();
		component.Init(tweakable);
		PostCreate(component);
	}

	private void CreateInputField(TweakableInputField tweakable)
	{
		TweakableInputFieldUI tweakableInputFieldUI = UnityEngine.Object.Instantiate(m_prefabInputField);
		tweakableInputFieldUI.transform.SetParent(m_content, worldPositionStays: false);
		TweakableInputFieldUI component = tweakableInputFieldUI.GetComponent<TweakableInputFieldUI>();
		component.Init(tweakable);
		PostCreate(component);
	}

	private void CreateBool(TweakableBool tweakable)
	{
		TweakableBoolUI tweakableBoolUI = UnityEngine.Object.Instantiate(m_prefabBool);
		tweakableBoolUI.transform.SetParent(m_content, worldPositionStays: false);
		TweakableBoolUI component = tweakableBoolUI.GetComponent<TweakableBoolUI>();
		component.Init(tweakable);
		PostCreate(component);
	}

	private void CreateFloat(TweakableFloat tweakable)
	{
		TweakableFloatUI tweakableFloatUI = UnityEngine.Object.Instantiate(m_prefabFloat);
		tweakableFloatUI.transform.SetParent(m_content, worldPositionStays: false);
		TweakableFloatUI component = tweakableFloatUI.GetComponent<TweakableFloatUI>();
		component.Init(tweakable);
		PostCreate(component);
	}

	private void CreateInt(TweakableInt tweakable)
	{
		TweakableIntUI tweakableIntUI = UnityEngine.Object.Instantiate(m_prefabInt);
		tweakableIntUI.transform.SetParent(m_content, worldPositionStays: false);
		TweakableIntUI component = tweakableIntUI.GetComponent<TweakableIntUI>();
		component.Init(tweakable);
		PostCreate(component);
	}

	private void CreateTrigger(TweakableTrigger tweakable)
	{
		TweakableTriggerUI tweakableTriggerUI = UnityEngine.Object.Instantiate(m_prefabTrigger);
		tweakableTriggerUI.transform.SetParent(m_content, worldPositionStays: false);
		TweakableTriggerUI component = tweakableTriggerUI.GetComponent<TweakableTriggerUI>();
		component.Init(tweakable);
		PostCreate(component);
	}

	private void PostCreate(TweakableUI tweakable)
	{
		AddCategory(tweakable.GetCategory());
		m_listDone.Add(tweakable);
		tweakable.gameObject.SetActive(tweakable.GetCategory() == m_activeCategory);
	}

	public void ChangeTab(string category)
	{
		m_activeCategory = category;
		TweakableTabUI[] componentsInChildren = m_tabs.GetComponentsInChildren<TweakableTabUI>();
		TweakableTabUI[] array = componentsInChildren;
		foreach (TweakableTabUI tweakableTabUI in array)
		{
			tweakableTabUI.SetSelected(tweakableTabUI.TabName == category);
		}
		foreach (TweakableUI item in m_listDone)
		{
			item.gameObject.SetActive(item.GetCategory() == category);
		}
	}

	public void AddCategory(string category)
	{
		TweakableTabUI[] componentsInChildren = m_tabs.GetComponentsInChildren<TweakableTabUI>();
		if (componentsInChildren.Length == 0)
		{
			m_activeCategory = category;
		}
		if (Array.Find(componentsInChildren, (TweakableTabUI tab) => tab.TabName == category) == null)
		{
			TweakableTabUI tweakableTabUI = UnityEngine.Object.Instantiate(m_prefabTab);
			tweakableTabUI.GetComponent<TweakableTabUI>().Init(category);
			tweakableTabUI.transform.SetParent(m_tabs, worldPositionStays: false);
			tweakableTabUI.SetSelected(componentsInChildren.Length == 0);
		}
	}
}
