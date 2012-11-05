using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class RemoveCastShadow : EditorWindow {

    [MenuItem("TorchLight/Editor/RemoveCastShadow")]
    static void ExecuteRemoveCastShadow()
    {

        RemoveCastShadowWithName("rubble");
        RemoveCastShadowWithName("dust");

        Debug.Log("Remove Cast Shadow Finished");
    }

    static void RemoveCastShadowWithName(string Name)
    {
        MeshRenderer[] Objs = UnityEngine.Object.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
        foreach (MeshRenderer Render in Objs)
        {
            if (Render.gameObject.name.IndexOf(Name, StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                Render.castShadows = false;
            }
        }
    }

    [MenuItem("TorchLight/Editor/ComputeSceneTrangles")]
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
