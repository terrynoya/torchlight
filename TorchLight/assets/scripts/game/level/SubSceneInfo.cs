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
	
	int 						CurIndex = 0;
	AsyncOperation 	AsyncOp = null;

	Camera MainCamera = null;
	Light DirectionalLight = null;
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
		}
		
		if (CurIndex == AllSubScenes.Count)
		{
			RemoveUselessLightAndCamera();
		}
	}
	
	void AsyncLoadNextSubScene()
	{
		CurIndex++;
		if (CurIndex < AllSubScenes.Count)
		{
			Debug.Log(AllSubScenes[CurIndex]);
			AsyncOp = Application.LoadLevelAdditiveAsync(AllSubScenes[CurIndex]);
		}
	}
	
	void RemoveUselessLightAndCamera()
	{
		
	}
}
