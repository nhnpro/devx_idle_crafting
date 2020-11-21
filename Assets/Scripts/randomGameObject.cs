using UnityEngine;

public class randomGameObject : MonoBehaviour
{
	public GameObject[] objectPool;

	private int currentIndex;

	private void Awake()
	{
		int num = UnityEngine.Random.Range(0, objectPool.Length);
		objectPool[currentIndex].SetActive(value: false);
		currentIndex = num;
		objectPool[currentIndex].SetActive(value: true);
	}
}
