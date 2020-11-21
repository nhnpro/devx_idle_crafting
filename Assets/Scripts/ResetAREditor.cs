using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetAREditor : MonoBehaviour
{
	public void OnResetGame()
	{
		SceneManager.LoadScene("AR");
	}
}
