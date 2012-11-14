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

	int 			NextIndex = 0;
	AsyncOperation 	AsyncOp = null;
	
	void Update()
	{
		if (NextIndex > AllSubScenes.Count)
			return;

        if (AsyncOp == null || AsyncOp.isDone)
		{
			AsyncLoadNextSubScene();

            if (NextIndex == 2)
            {
                Debug.Log(NextIndex);
                SplashManager Splash = FindObjectOfType(typeof(SplashManager)) as SplashManager;
                if (Splash != null)
                    Splash.HideSplash();

                Debug.Log("Hide Splash");
            }
		}
	}
	
	void AsyncLoadNextSubScene()
	{
		if (NextIndex < AllSubScenes.Count)
		{
			Debug.Log(AllSubScenes[NextIndex]);
			AsyncOp = Application.LoadLevelAdditiveAsync(AllSubScenes[NextIndex]);
		}
        NextIndex++;
	}
}
