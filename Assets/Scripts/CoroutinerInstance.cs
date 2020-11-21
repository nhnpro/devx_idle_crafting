using System.Collections;
using UnityEngine;

public class CoroutinerInstance : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public Coroutine ProcessWork(IEnumerator iterationResult)
	{
		return StartCoroutine(DestroyWhenComplete(iterationResult));
	}

	public IEnumerator DestroyWhenComplete(IEnumerator iterationResult)
	{
		yield return StartCoroutine(iterationResult);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
