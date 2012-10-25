using UnityEngine;
using System.Collections.Generic;

public class SkinedMeshCombine : MonoBehaviour {

    bool bNeedCreate = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    GenerateBodyParts(gameObject);
	}

    void GenerateBodyParts(GameObject Root)
    {
        if (!bNeedCreate) return;

        List<Transform> Bones       = new List<Transform>();
        List<Material> Materials    = new List<Material>();
        List<BoneWeight> Weights    = new List<BoneWeight>();

        SkinnedMeshRenderer[] Renders = GetComponentsInChildren<SkinnedMeshRenderer>();

        List<CombineInstance> MatCombinedInstances = new List<CombineInstance>();
        foreach (SkinnedMeshRenderer Render in Renders)
        {
            Materials.Add(Render.sharedMaterial);
            Bones.AddRange(Render.bones);

            Weights.AddRange(Render.sharedMesh.boneWeights);

            Debug.Log(Render.bones.Length);

            CombineInstance CI = new CombineInstance();
            CI.mesh = Render.sharedMesh;
            CI.subMeshIndex = 0;

            MatCombinedInstances.Add(CI);

            Destroy(Render.gameObject);
        }

        SkinnedMeshRenderer CombinedRender  = Root.AddComponent<SkinnedMeshRenderer>();
        CombinedRender.sharedMesh           = new Mesh();
        CombinedRender.sharedMesh.CombineMeshes(MatCombinedInstances.ToArray(), false, false);
        CombinedRender.bones                = Bones.ToArray();
        CombinedRender.sharedMesh.boneWeights = Weights.ToArray();
        CombinedRender.sharedMaterials      = Materials.ToArray();

        bNeedCreate = false;
    }
}
