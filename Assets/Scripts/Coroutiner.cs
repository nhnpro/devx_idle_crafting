using System.Collections;
using UnityEngine;

public class Coroutiner
{
	public static Coroutine StartCoroutine(IEnumerator iterationResult)
	{
		GameObject gameObject = new GameObject("Coroutiner");
		CoroutinerInstance coroutinerInstance = gameObject.AddComponent(typeof(CoroutinerInstance)) as CoroutinerInstance;
		return coroutinerInstance.ProcessWork(iterationResult);
	}
}
