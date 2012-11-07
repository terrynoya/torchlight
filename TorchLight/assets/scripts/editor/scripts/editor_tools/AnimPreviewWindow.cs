using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class AnimPreviewWindow : EditorWindow {

    static Vector3 OritentPosition = new Vector3(10000, 10000, 10000);

    GameObject      PreviewObject = null;
    GameObject      PreviewLight = null;
    RenderTexture   PreviewTexture = null;

    float           ZoomFactor = 1.0f;
    float           RotateAngle = 0.0f;
    Vector3         CamOffset = new Vector3(10, 0, 10);
    Vector3         CamLookAt = Vector3.zero;

    List<string>    AnimationNames = new List<string>();
    int             AnimIndex = 0;
    bool            AnimLoop = false;
    string          LastPlayedAnim = "";
    float           CurAnimTime = 0.0f;
    float           TotalAnimLength = 0.0f;

    string          LogInfo = "";

    [MenuItem("TorchLight/Tools/AnimPreviewWindow")]
    static void ShowEditor()
    {
        AnimPreviewWindow EditorWindow          = CreateInstance<AnimPreviewWindow>();
        EditorWindow.title                      = "Anim Preview";
        EditorWindow.autoRepaintOnSceneChange   = true;
        EditorWindow.Show();
    }

    bool LoadResource(string Path)
    {
        GameObject Obj = AssetDatabase.LoadAssetAtPath(Path, typeof(GameObject)) as GameObject;
        return LoadResource(Obj);
    }

    List<Mesh> GetMeshList(GameObject Obj)
    {
        List<Mesh> MeshList = new List<Mesh>();
        {
            MeshFilter[] Filters = Obj.GetComponentsInChildren<MeshFilter>();
            if (Filters.Length != 0)
            {
                foreach (MeshFilter Filter in Filters)
                {
                    Mesh AMesh = null;
                    if (Filter.sharedMesh == null)
                        AMesh = Filter.mesh;
                    else
                        AMesh = Filter.sharedMesh;

                    AMesh.RecalculateBounds();
                    MeshList.Add(AMesh);
                }

                LogInfo = "GameObject " + Obj.name + " Is NOT a SkinedMesh";
            }
            else
            {
                Mesh AMesh = null;
                SkinnedMeshRenderer[] Renders = Obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (Renders.Length != 0)
                {
                    foreach (SkinnedMeshRenderer Render in Renders)
                    {
                        
                        AMesh = Render.sharedMesh;
                        AMesh.RecalculateBounds();
                        MeshList.Add(AMesh);
                    }
                }
                LogInfo = "SkinedMesh - " + Obj.name;
            }
        }
        return MeshList;
    }

    Bounds ComputeMeshBound(List<Mesh> MeshList)
    {
        Bounds Bound = new Bounds();
        foreach (Mesh AMesh in MeshList)
            Bound.Encapsulate(AMesh.bounds);
        return Bound;
    }

    List<string> GetAnimationNames()
    {
        List<string> Names = new List<string>();
        {
            if (PreviewObject.animation != null)
            {
                foreach (AnimationState Clip in PreviewObject.animation)
                    Names.Add(Clip.name);
            }
        }
        return Names;
    }

    bool LoadResource(GameObject Obj)
    {
        if (PreviewObject != null)
        {
            DestroyImmediate(PreviewObject);
            PreviewObject = null;
        }

        try
        {
            if (Obj != null)
            {
                if (PreviewLight == null)
                {
                    PreviewLight            = new GameObject("The Light"); ;
                    Light NewLight          = PreviewLight.AddComponent<Light>();
                    NewLight.type           = LightType.Point;
                    PreviewLight.hideFlags  = HideFlags.HideAndDontSave;
                }

                PreviewObject       = Instantiate(Obj) as GameObject;
                PreviewObject.name  = Obj.name;

                List<Mesh> MeshList = GetMeshList(PreviewObject);
                Bounds MeshBound    = ComputeMeshBound(MeshList);

                PreviewObject.transform.position = OritentPosition;

                CamLookAt   = OritentPosition;
                CamLookAt.y += MeshBound.extents.y;

                CamOffset.x = MeshBound.extents.y * 2.5f;
                CamOffset.z = MeshBound.extents.y * 2.5f;
                CamOffset.y = MeshBound.extents.y;

                PreviewLight.transform.position = OritentPosition + CamOffset * 0.5f;
                PreviewObject.hideFlags         = HideFlags.HideAndDontSave;

                AnimationNames = GetAnimationNames();
                CurAnimTime = 0.0f;
                TotalAnimLength = 0.0f;

                return true;
            }
        }
        catch (Exception)
        {
            OnDestroy();
        }

        return false;
    }

    int FrameCounter = 0;
    void Update()
    {
        FrameCounter++;
        if (FrameCounter % 5 != 0)
            return;

        if (PreviewTexture == null || (PreviewTexture.width != position.width || PreviewTexture.height != position.height))
        {
            PreviewTexture = new RenderTexture((int)position.width, (int)position.height, 32, RenderTextureFormat.ARGB32);
            PreviewTexture.filterMode   = FilterMode.Trilinear;
            PreviewTexture.hideFlags    = HideFlags.HideAndDontSave;
        }

        if (PreviewTexture != null)
        {
            Vector3 RTTCamPos       = OritentPosition + (2 - ZoomFactor) * CamOffset;
            Vector3 RTTCamLoolAt    = CamLookAt;
            UpdateRTTCamera(RTTCamPos, RTTCamLoolAt);
        }

        UpdateAnimationSample();
    }

    void UpdateRTTCamera(Vector3 RTTCamPos, Vector3 RTTCamLookAt)
    {
        if (PreviewObject != null)
            PreviewObject.transform.rotation = Quaternion.AngleAxis(RotateAngle, Vector3.up);

        Vector3 CamPosBackup                    = Camera.mainCamera.transform.position;
        Quaternion CamRotBackup                 = Camera.mainCamera.transform.rotation;

        Camera.mainCamera.transform.position    = RTTCamPos;
        Camera.mainCamera.transform.rotation    = Quaternion.identity;
        Camera.mainCamera.transform.LookAt(RTTCamLookAt);

        Camera.mainCamera.targetTexture = PreviewTexture;
        Camera.mainCamera.Render();
        Camera.mainCamera.targetTexture = null;

        Camera.mainCamera.transform.position    = CamPosBackup;
        Camera.mainCamera.transform.rotation    = CamRotBackup;
    }

    void UpdateAnimationSample()
    {
        if (AnimationNames.Count > 0)
            PlayAnimation(AnimationNames[AnimIndex]);
    }

    void PlayAnimation(string Anim)
    {
        if (PreviewObject != null && PreviewObject.animation != null)
        {
            AnimationClip Clip = PreviewObject.animation.GetClip(Anim);
            if (Clip != null)
            {
                AnimationState AnimState = PreviewObject.animation[Anim];
                WrapMode Mode = AnimLoop ? WrapMode.Loop : WrapMode.Default;
                if (AnimState.wrapMode != Mode)
                    AnimState.time = 0.0f;

                AnimState.wrapMode  = Mode;
                AnimState.time      += EditorGUIUtil.DeltaTime();

                if (AnimState.time < AnimState.length && Mode == WrapMode.Default || 
                    Mode == WrapMode.Loop)
                    PreviewObject.animation.Sample();

                if (LastPlayedAnim != Anim)
                {
                    AnimState.time = 0.0f;
                    LastPlayedAnim = Anim;
                    PreviewObject.animation.Play(Anim);
                }

                CurAnimTime     = AnimState.time;
                TotalAnimLength = AnimState.length;
                CurAnimTime     = CurAnimTime - (int)(CurAnimTime/TotalAnimLength) * TotalAnimLength;

                if (Mode == WrapMode.Default)
                    CurAnimTime = Math.Min(AnimState.time, AnimState.length);
            }
        }
    }

    void ResetAnimation(string Anim)
    {
        if (PreviewObject != null && PreviewObject.animation != null)
        {
            AnimationClip Clip = PreviewObject.animation.GetClip(Anim);
            if (Clip != null)
            {
                PreviewObject.animation[Anim].time = 0.0f;
            }
        }
    }

    void OnDestroy()
    {
        if (PreviewObject != null)
            DestroyImmediate(PreviewObject);
        if (PreviewTexture != null)
            DestroyImmediate(PreviewTexture);
        if (PreviewLight != null)
            DestroyImmediate(PreviewLight);

        AnimationUtility.StopAnimationMode();
    }

    void OnGUI()
    {
        if (PreviewTexture != null)
        {
            int WindowWidth     = (int)position.width, WindowHeight = (int)position.height;
            int WindowXCenter   = (int)(WindowWidth * 0.5f);

            bool ButtonPress = false;
            GUI.DrawTexture(new Rect(0.0f, 0.0f, position.width, position.height), PreviewTexture);

            GUILayout.BeginHorizontal();
            ButtonPress = GUILayout.Button("Apply Mesh");
            ProcessApplayMesh(ButtonPress);
            ButtonPress = GUILayout.Button("Play Animation");
            ProcessPlayAnimation(ButtonPress);
            AnimLoop = GUILayout.Toggle(AnimLoop, "Loop", GUILayout.Width(50));
            GUILayout.EndHorizontal();
            AnimIndex = EditorGUILayout.Popup(AnimIndex, AnimationNames.ToArray());
            GUILayout.Label("Info  : " + LogInfo);
            GUILayout.Label(string.Format("Time : {0:0.0} / {1:0.0}", CurAnimTime, TotalAnimLength));

            GUILayout.BeginArea(new Rect(WindowXCenter - 120, WindowHeight - 30, 250, 50));
            GUILayout.BeginHorizontal();
            float Factor = GUILayout.HorizontalSlider(ZoomFactor, 0.5f, 1.5f, GUILayout.Width(180));
            ProcessZoom(Factor);
            GUILayout.Label(string.Format("Zoom({0:0.0})", Factor));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            ProcessMouseEvent();

            //Repaint();
        }
    }

    void ProcessApplayMesh(bool Press)
    {
        if (Press)
        {
            GameObject SelectionObj = Selection.activeGameObject as GameObject;
            if (SelectionObj != null)
            {
                LoadResource(SelectionObj);
                AnimIndex       = 0;
                LastPlayedAnim  = "";
            }
        }
    }

    void ProcessPlayAnimation(bool Press)
    {
        if (Press)
        {
            if (PreviewObject != null && PreviewObject.animation != null)
            {
                if (AnimationNames.Count > 0)
                    ResetAnimation(AnimationNames[AnimIndex]);
            }
        }
    }

    void ProcessZoom(float NewFactor)
    {
        if (NewFactor != ZoomFactor)
        {
            ZoomFactor = NewFactor;
        }
    }

    float   RotateDirection = 1.0f;
    bool    bCanRotateModel = false;
    void ProcessMouseEvent()
    {
        MouseEvent Event = MouseEvent.GetMouseEvent();
        if (Event.IsLeftButton())
        {
            if (Event.Type == EventType.MouseDown)
            {
                bCanRotateModel = true;
                RotateDirection = Event.MousePos.x;
            }

            if (Event.Type == EventType.MouseUp)
            {
                bCanRotateModel = false;
            }
        }

        if (bCanRotateModel)
        {
            float Dir       = Event.MousePos.x - RotateDirection;
            RotateAngle     -= Dir * 0.5f;
            RotateDirection = Event.MousePos.x;
        }
    }
}
