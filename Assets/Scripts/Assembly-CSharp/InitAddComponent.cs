using System.Diagnostics;
using Localize.Translate;
using Manager;
using UnityEngine;

public static class InitAddComponent
{
	public static void AddComponents(GameObject go)
	{
		Localize.Translate.Manager.Initialize();
		Localize.Translate.Manager.SCENE_ID sceneID = Localize.Translate.Manager.SCENE_ID.SET_UP;
		Localize.Translate.Manager.LoadScene(sceneID, null);
		go.AddComponent<Manager.Sound>();
		go.AddComponent<Voice>();
		go.AddComponent<Manager.Config>();
		go.AddComponent<Scene>();
		go.AddComponent<Game>();
		go.AddComponent<Character>();
		go.AddComponent<Communication>();
		go.AddComponent<PlayerAction>();
		Localize.Translate.Manager.initializeSolution.Execute();
		Localize.Translate.Manager.DisposeScene(sceneID, true);
	}

	[Conditional("__DEBUG_PROC__")]
	private static void DebugAddComponents(GameObject go)
	{
		go.AddComponent<DebugShow>();
		go.AddComponent<AllocMem>();
		go.AddComponent<QualityDebug>();
		go.AddComponent<DebugExample>();
		go.AddComponent<DebugRenderLog>();
	}
}
