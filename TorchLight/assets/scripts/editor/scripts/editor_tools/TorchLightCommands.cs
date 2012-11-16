using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class TorchLightCommands : EditorWindow {

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

    [MenuItem("TorchLight/Commands/OptimizeGameObjects")]
    static void ExecuteOptimizeGameObjects()
    {
        GameObject CurSelect = Selection.activeGameObject;
        if (CurSelect != null && CurSelect.name == "LevelObjects")
        {
            GameObject NewSelectObj = new GameObject(CurSelect.name);
            NewSelectObj.transform.parent = CurSelect.transform.parent;
            NewSelectObj.transform.localPosition = CurSelect.transform.localPosition;
            NewSelectObj.transform.localRotation = CurSelect.transform.localRotation;

            List<GameObject> DestoryObjs = new List<GameObject>();
            for (int i = 0; i < CurSelect.transform.GetChildCount(); i++)
            {
                GameObject Child = CurSelect.transform.GetChild(i).gameObject;
                MeshRenderer Render = Child.GetComponentInChildren<MeshRenderer>();
                if (Render != null)
                {
                    Render.gameObject.transform.parent = null;
                    Render.gameObject.name = Child.name;
                    Render.gameObject.transform.parent = NewSelectObj.transform;
                    Render.gameObject.transform.localPosition = Child.transform.localPosition;
                    Render.gameObject.transform.localRotation = Child.transform.localRotation;

                    DestoryObjs.Add(Child);
                }
            }

            Debug.Log("Remove [" + DestoryObjs.Count * 2 + "] Useless GameObjects");

            foreach (GameObject Obj in DestoryObjs)
                DestroyImmediate(Obj);

            DestroyImmediate(CurSelect);
            Selection.activeGameObject = NewSelectObj;
        }
        else
            Debug.LogError("You MUST Select LevelObjects GameObject Under Level Before Operation.");
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

    [MenuItem("TorchLight/Commands/CaptureScreen")]
    static void ExecuteCaptureScreen()
    {
        string Date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        Application.CaptureScreenshot(Date + ".png");
    }

    [MenuItem("GameObject/Create Other/Game Object", false, -1)]
    static void ExecuteCreateNewGameObject()
    {
        GameObject NewGameObj = new GameObject("GameObject");
        GameObject CurSelect = Selection.activeGameObject;
        if (CurSelect != null)
        {
            NewGameObj.transform.parent         = CurSelect.transform;
            NewGameObj.transform.localPosition  = Vector3.zero;
            NewGameObj.transform.localRotation  = Quaternion.identity;
        }
    }
}
