using UnityEngine;
using System.Collections.Generic;

public class SubSceneInfo : MonoBehaviour {

    // AllSubScenes include all the subscenes to load, normally we
    // load subscene from index of 1 becouse 0 had been loaded!
    public List<string> AllSubScenes = new List<string>();
	
	public string 	Name = "";
	public string 	DisplayName = "";
	public int 		PlayerLevelMin = 0;
	public int 		PlayerLevelMax = 999;
	
	int 			CurIndex = 0;
	AsyncOperation 	AsyncOp = null;

	void Start()
	{
	}
	
	void Update()
	{
		if (CurIndex > AllSubScenes.Count)
			return;
		
		if (AsyncOp == null || AsyncOp.isDone)
		{
			AsyncLoadNextSubScene();

            if (CurIndex == 2)
            {
                SplashManager Splash = FindObjectOfType(typeof(SplashManager)) as SplashManager;
                if (Splash != null)
                    Splash.HideSplash();

                Debug.Log("Hide Splash");
            }
		}
	}
	
	void AsyncLoadNextSubScene()
	{
		if (CurIndex < AllSubScenes.Count)
		{
			Debug.Log(AllSubScenes[CurIndex]);
			AsyncOp = Application.LoadLevelAdditiveAsync(AllSubScenes[CurIndex]);
		}
        CurIndex++;
	}
	
	void OnGUI()
	{
		if (CurIndex > AllSubScenes.Count - 1)
			GUILayout.Label("Level Load Finished");
	}
}
