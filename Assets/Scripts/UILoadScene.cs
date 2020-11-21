using System.Collections;
using UnityEngine;

public class UILoadScene : MonoBehaviour
{
	public void OnLoadMainFromAR()
	{
		ARBindingManager.Instance.SceneTransition.SetActive(value: true);
		ARBindingManager.Instance.StartCoroutine(LoadMainSceneWithDelay());
	}

	public void OnLoadARFromMain()
	{
		BindingManager.Instance.SceneTransition.SetActive(value: true);
		BindingManager.Instance.StartCoroutine(LoadARSceneWithDelay());
	}

	public void OnLoadMainFromSimpleEditor()
	{
		ARBindingManager.Instance.SceneTransition.SetActive(value: true);
		ARBindingManager.Instance.StartCoroutine(LoadMainSceneWithDelay());
	}

	public void OnLoadSimpleEditorFromMain()
	{
		BindingManager.Instance.SceneTransition.SetActive(value: true);
		BindingManager.Instance.StartCoroutine(LoadSimpleEditorSceneWithDelay());
	}

	public void OnLoadLevelEditorFromMain()
	{
		if (PlayerData.Instance.AREditorChosen.Value)
		{
			OnLoadARFromMain();
		}
		else
		{
			OnLoadSimpleEditorFromMain();
		}
	}

	public void OnLoadARFromSimpleEditor()
	{
		PlayerData.Instance.AREditorChosen.Value = true;
		ARBindingManager.Instance.SceneTransition.SetActive(value: true);
		ARBindingManager.Instance.StartCoroutine(LoadARSceneWithDelay());
	}

	public void OnLoadSimpleEditorFromAR()
	{
		PlayerData.Instance.AREditorChosen.Value = false;
		ARBindingManager.Instance.SceneTransition.SetActive(value: true);
		ARBindingManager.Instance.StartCoroutine(LoadSimpleEditorSceneWithDelay());
	}

	public void OnLoadMainFromGambling()
	{
		Singleton<GamblingRunner>.Instance.EmptyGamblingState();
		GamblingBindingManager.Instance.SceneTransition.SetActive(value: true);
		GamblingBindingManager.Instance.StartCoroutine(LoadMainSceneWithDelay());
	}

	public void OnLoadGamblingFromMain()
	{
		PlayerData.Instance.GamblingTimeStamp.Value = ServerTimeService.NowTicks();
		BindingManager.Instance.SceneTransition.SetActive(value: true);
		BindingManager.Instance.StartCoroutine(LoadGamblingSceneWithDelay());
	}

	private IEnumerator LoadMainSceneWithDelay()
	{
		yield return new WaitForSeconds(1f);
		PlayerData.Instance.WelcomebackTimeStamp.Value = ServerTimeService.NowTicks();
		SceneLoadHelper.LoadMainScene();
	}

	private IEnumerator LoadARSceneWithDelay()
	{
		yield return new WaitForSeconds(1f);
		SceneLoadHelper.LoadARScene();
	}

	private IEnumerator LoadSimpleEditorSceneWithDelay()
	{
		yield return new WaitForSeconds(1f);
		SceneLoadHelper.LoadSimpleEditorScene();
	}

	private IEnumerator LoadGamblingSceneWithDelay()
	{
		yield return new WaitForSeconds(1f);
		SceneLoadHelper.LoadGamblingScene();
	}
}
