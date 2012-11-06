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
		MainCamera = Camera.mainCamera;
		Light[] Lights = FindObjectsOfType(typeof(Light)) as Light[];
		foreach(Light L in Lights)
		{
			if (L.type == LightType.Directional)
			{
				DirectionalLight = L;
				break;
			}
		}
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
		Camera[] Cameras = FindObjectsOfType(typeof(Camera)) as Camera[];
		foreach(Camera C in Cameras)
		{
			if (C != MainCamera)
				Destroy(C.gameObject);
		}
		
		Light[] Lights = FindObjectsOfType(typeof(Light)) as Light[];
		foreach(Light L in Lights)
		{
			if (L != DirectionalLight)
				Destroy(L.gameObject);
		}
	}
	
	void OnGUI()
	{
		if (CurIndex > AllSubScenes.Count - 1)
			GUILayout.Label("Level Load Finished");
	}
}
