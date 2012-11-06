using UnityEngine;
using System.Collections;

public class TestLoad : MonoBehaviour {
	
	public string SceneToLoad = "main-Starta0-Scene-Full";
	
	// Use this for initialization
	void Start () {
		SceneManager.LoadScene(SceneToLoad, true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
