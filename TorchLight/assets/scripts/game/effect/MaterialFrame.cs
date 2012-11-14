using UnityEngine;
using System.Collections.Generic;

public class MaterialFrame : MonoBehaviour
{

    public List<Texture2D> TextureFrames = new List<Texture2D>();
    public float DeltaTime = 0.1f;
    public bool bCreateNewMaterial = false;

    private int CurIndex = 0;
    private float TimeLast = 0.0f;
    private Material CurMaterial = null;

	// Use this for initialization
	void Start () {
        TimeLast = DeltaTime;

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
	
	// Update is called once per frame
	void Update () 
    {
        if (TextureFrames.Count == 0)
            return;

        TimeLast -= Time.deltaTime;
        if (TimeLast < 0.0f && CurMaterial != null)
        {
            CurMaterial.mainTexture = TextureFrames[CurIndex];
            TimeLast = DeltaTime;

            CurIndex = (CurIndex + 1) % TextureFrames.Count;
        }
	}
}
