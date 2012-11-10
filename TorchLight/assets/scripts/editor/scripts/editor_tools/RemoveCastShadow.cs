using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class RemoveCastShadow : EditorWindow {

    [MenuItem("TorchLight/Commands/RemoveAlphaCastShadow")]
    static void ExecuteRemoveCastShadow()
    {
        MeshRenderer[] Objs = UnityEngine.Object.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
        foreach (MeshRenderer Render in Objs)
        {
            Material Mat = Render.sharedMaterial != null ? Render.sharedMaterial : Render.material;
            if (Mat.shader.name == "TorchLight/Alpha/Alpha" ||
                Mat.shader.name == "TorchLight/Alpha/AlphaShadowed" ||
                Mat.shader.name == "TorchLight/Alpha/AlphaTest")
            {
                Render.castShadows = false;
            }
        }

        Debug.Log("Remove Cast Shadow Finished");
    }

    [MenuItem("TorchLight/Commands/ComputeSceneTrangles")]
    static void ExecuteComputeSceneTrangles()
    {
        int TotalTranglesNum = 0;
        int TotalVertexNum = 0;
        MeshFilter[] Objs = UnityEngine.Object.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];
        foreach (MeshFilter Filter in Objs)
        {
            TotalTranglesNum += Filter.sharedMesh.triangles.Length / 3;
            TotalVertexNum += Filter.sharedMesh.vertexCount;
        }

        Debug.Log("----------------------------------------");
        Debug.Log("Total Trangle Num : " + TotalTranglesNum);
        Debug.Log("Total Vertex Num : " + TotalVertexNum);
        Debug.Log("----------------------------------------");
    }
}
