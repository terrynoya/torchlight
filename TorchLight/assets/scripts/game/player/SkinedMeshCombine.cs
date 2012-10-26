using UnityEngine;
using System.Collections.Generic;

public class SkinedMeshCombine : MonoBehaviour {

    void Start()
    {
        GenerateBodyParts(gameObject);
    }

    void GenerateBodyParts(GameObject Root)
    {
        List<Transform> Bones = new List<Transform>();
        List<Material> Materials = new List<Material>();

        SkinnedMeshRenderer[] Renders = GetComponentsInChildren<SkinnedMeshRenderer>();

        List<CombineInstance> CombinedInstances = new List<CombineInstance>();
        foreach (SkinnedMeshRenderer Render in Renders)
        {
            Materials.Add(Render.sharedMaterial);
            Bones.AddRange(Render.bones);

            CombineInstance CI = new CombineInstance();
            CI.mesh = Render.sharedMesh;
            CI.subMeshIndex = 0;

            CombinedInstances.Add(CI);

            Destroy(Render.gameObject);
        }

        SkinnedMeshRenderer CombinedRender = Root.AddComponent<SkinnedMeshRenderer>();
        CombinedRender.sharedMesh = new Mesh();
        CombinedRender.bones = Bones.ToArray();
        CombinedRender.sharedMaterials = Materials.ToArray();
        CombinedRender.sharedMesh.CombineMeshes(CombinedInstances.ToArray(), false, false);

        CombinedRender.sharedMesh.RecalculateNormals();

        Debug.Log("SkinedMeshCombine");
    }

    
}
