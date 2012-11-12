using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LevelChunkShower : MonoBehaviour {

    public float BoxSize = 100;

    Transform[] Chunks = null;

	// Use this for initialization
	void Start () {
        Chunks = gameObject.GetComponentsInChildren<Transform>() as Transform[];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        foreach (Transform T in Chunks)
        {
            Gizmos.DrawWireCube(new Vector3(T.position.x, T.position.y, T.position.z - BoxSize * 0.5f), new Vector3(BoxSize, BoxSize, BoxSize));
        }
    }
}
