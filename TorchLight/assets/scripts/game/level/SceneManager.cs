using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {
	
	static private bool LoadAsync = false;
	static private string SceneToLoad = "Unknow";
	static private AsyncOperation AsyncOp = null;
	
	// Use this for initialization
	void Start () {
		if (guiTexture == null)
		{
			Debug.LogError("SceneManager MUST Attact to a GUITexture");
			return;
		}
		
		guiTexture.transform.position = Vector3.zero;
		guiTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
		
		LoadScene();
	}
	
	// Update is called once per frame
	void Update () {
		if (AsyncOp != null && AsyncOp.isDone)
		{			
			AsyncOp = null;
			guiTexture.gameObject.SetActiveRecursively(false);
		}
	}
	
	void OnGUI()
	{
		if (AsyncOp != null)
			GUILayout.Label("Loading ..." + AsyncOp.progress);
	}
	
	void OnLevelWasLoaded(int level)
	{
		Debug.Log("Level Load Finished @" + level);
	}
	
	static public void LoadScene(string SceneName, bool Async)
	{
		SceneToLoad = SceneName;
		LoadAsync 	= Async;
		
		Application.LoadLevel("Loading");
	}
	
	static void LoadScene()
	{
		if (LoadAsync)
			AsyncOp = Application.LoadLevelAsync(SceneToLoad);
		else
			Application.LoadLevel(SceneToLoad);
	}

}
