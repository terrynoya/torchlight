using UnityEngine;
using System.Collections;

public class MaterialAnim : MonoBehaviour {

    public bool bCreateNewMaterial = false;
    public float USpeed = 0.0f;
    public float VSpeed = 0.0f;

    protected Material CurMaterial = null;

	// Use this for initialization
	public void Start () {
        MeshRenderer Render = GetComponent<MeshRenderer>();
        if (Render == null)
            Render = GetComponentInChildren<MeshRenderer>();

        if (Render != null)
        {
            CurMaterial = Render.material != null ? Render.material : Render.sharedMaterial;
            if (CurMaterial != null && bCreateNewMaterial)
            {
                CurMaterial = Instantiate(CurMaterial) as Material;
                Render.material = CurMaterial;
                Render.sharedMaterial = null;
            }
        }
	}

    protected Vector2 Offset = Vector2.zero;
	// Update is called once per frame
	public void Update () 
    {
        if (CurMaterial != null)
        {
            Offset.x += USpeed * Time.deltaTime;
            Offset.y += VSpeed * Time.deltaTime;
            CurMaterial.mainTextureOffset = Offset;
        }
	}
}
