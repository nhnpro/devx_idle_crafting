using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadHelper
{
	public static void LoadInitSceneNow()
	{
		MainInstaller.ReleaseAll();
		PersistentSingleton<IAPService>.Instance.UnpublishPurchases();
		SceneManager.LoadScene("Initialize");
	}

	public static void LoadInitScene()
	{
		MainThreadDispatcher.StartCoroutine(InitSceneRoutine());
	}

	public static void LoadMainScene()
	{
		MainThreadDispatcher.StartCoroutine(MainSceneRoutine());
	}

	public static void LoadARScene()
	{
		MainThreadDispatcher.StartCoroutine(ARSceneRoutine());
	}

	public static void LoadSimpleEditorScene()
	{
		MainThreadDispatcher.StartCoroutine(SimpleEditorSceneRoutine());
	}

	public static void LoadGamblingScene()
	{
		MainThreadDispatcher.StartCoroutine(GamblingSceneRoutine());
	}

	public static void ResetGame()
	{
		MainThreadDispatcher.StartCoroutine(ResetGameRoutine());
	}

	private static IEnumerator MainSceneRoutine()
	{
		SceneManager.LoadScene("Empty");
		yield return null;
		PersistentSingleton<IAPService>.Instance.UnpublishPurchases();
		MainInstaller.ReleaseAll();
		ARInstaller.ReleaseAll();
		GamblingInstaller.ReleaseAll();
		SceneManager.LoadScene("Main");
	}

	private static IEnumerator ARSceneRoutine()
	{
		SceneManager.LoadScene("Empty");
		yield return null;
		MainInstaller.ReleaseAll();
		ARInstaller.ReleaseAll();
		SceneManager.LoadScene("AR");
	}

	private static IEnumerator SimpleEditorSceneRoutine()
	{
		SceneManager.LoadScene("Empty");
		yield return null;
		MainInstaller.ReleaseAll();
		ARInstaller.ReleaseAll();
		SceneManager.LoadScene("LevelEditor");
	}

	private static IEnumerator GamblingSceneRoutine()
	{
		SceneManager.LoadScene("Empty");
		yield return null;
		PersistentSingleton<IAPService>.Instance.UnpublishPurchases();
		MainInstaller.ReleaseAll();
		GamblingInstaller.ReleaseAll();
		SceneManager.LoadScene("TimeMachine");
	}

	private static IEnumerator InitSceneRoutine()
	{
		SceneManager.LoadScene("Empty");
		yield return null;
		LoadInitSceneNow();
	}

	private static IEnumerator ResetGameRoutine()
	{
		SceneManager.LoadScene("Empty");
		yield return null;
		PersistentSingleton<SaveLoad>.Instance.DebugResetData(PlayerData.Instance);
		ServerTimeService.ResetDebug();
		MainInstaller.ReleaseAll();
		PersistentSingleton<StringCache>.Instance.Clear();
		yield return null;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		yield return null;
		LoadInitSceneNow();
	}
}
