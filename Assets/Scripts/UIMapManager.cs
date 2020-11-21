using System.Collections.Generic;
using UnityEngine;

public class UIMapManager : MonoBehaviour
{
	private int m_lastNumChapters = -1;

	protected void OnEnable()
	{
		int num = Mathf.CeilToInt((float)PlayerData.Instance.MainChunk.Value / 40f) + 7;
		if (num != m_lastNumChapters)
		{
			base.transform.DestroyChildrenWithImmediate<MapRunnerContext>();
			PredictableRandom.SetSeed((uint)PlayerData.Instance.LifetimePrestiges.Value, 3737u);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				dictionary.Clear();
				dictionary.Add("Chapter", num2);
				GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateSetParent(GetChapterPrefab(num2), dictionary, base.transform);
				gameObject.GetComponent<Canvas>().worldCamera = BindingManager.Instance.MapCamera.GetComponent<Camera>();
			}
			m_lastNumChapters = num;
			Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("Map/MapBasecamp", Vector3.zero, Quaternion.identity, null, base.transform);
			Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("Map/Boundaries", Vector3.zero, Quaternion.identity, null, base.transform);
			GameObject gameObject2 = Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("Map/Boundaries", Vector3.zero, Quaternion.identity, null, base.transform);
			gameObject2.transform.SetAsFirstSibling();
		}
	}

	private GameObject GetChapterPrefab(int chapter)
	{
		if (chapter >= BindingManager.Instance.ChapterList.Chapters.Length)
		{
			chapter = BindingManager.Instance.ChapterList.Chapters.Length - 1;
		}
		ChapterVariations chapterVariations = BindingManager.Instance.ChapterList.Chapters[chapter];
		return chapterVariations.ChapterVariants[PredictableRandom.GetNextRangeInt(0, chapterVariations.ChapterVariants.Length)];
	}
}
