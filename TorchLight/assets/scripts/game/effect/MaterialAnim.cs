using UnityEngine;
using System.Collections.Generic;

public class MaterialAnim : MonoBehaviour {

    public float USpeed = 0.0f;
    public float VSpeed = 0.0f;

    protected Material[] Materials = null;

	// Use this for initialization
	public void Start () {
        MeshRenderer Render = GetComponent<MeshRenderer>();
        if (Render == null)
            Render = GetComponentInChildren<MeshRenderer>();

        if (Render != null)
        {
            Materials = Render.materials != null ? Render.materials : Render.sharedMaterials;
        }
	}

    protected Vector2 Offset = Vector2.zero;
	// Update is called once per frame
	public void Update () 
    {
        if (Materials != null)
        {
            for (int i = 0; i < Materials.Length; i++)
            {
                Offset.x += USpeed * Time.deltaTime;
                Offset.y += VSpeed * Time.deltaTime;
                Materials[i].mainTextureOffset = Offset;
            }
        }
	}
}
