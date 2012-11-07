using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ProjectBuilder : EditorWindow {

    [MenuItem("TorchLight/Tools/Scenes Builder")]
    static void Execute()
    {
        ProjectBuilder Builder = CreateInstance<ProjectBuilder>();
        Builder.title = "Project Builder";
        Builder.Init();
        Builder.Show();
    }

    class SceneBuildInfo
    {
        public string   Path    = "";
        public bool     bBuild  = false;
        public bool     bChecked = false;
    }

    List<SceneBuildInfo> ScenesList = new List<SceneBuildInfo>();

    void Init()
    {
        ReloadScenesFromLocal();
    }

    Vector3 CurAddSceneScrollPosition = Vector3.zero;
    Vector3 CurProjectSceneScrollPosition = Vector3.zero;
    void OnGUI()
    {
        int Offset = 10;
        GUILayout.BeginArea(new Rect(Offset, Offset, position.width - Offset, position.height - Offset));
        {
            float ScrollHeight = (position.height - Offset - 90) * 0.5f;
            GUILayout.Label("Scenes To Build");
            CurAddSceneScrollPosition = GUILayout.BeginScrollView(CurAddSceneScrollPosition, "Box", GUILayout.Width(position.width - 2 * Offset), GUILayout.Height(ScrollHeight));
            {
                GUILayout.BeginVertical();
                {
                    EditorBuildSettingsScene[] ScenesToBuild = EditorBuildSettings.scenes;
                    foreach (EditorBuildSettingsScene Scene in ScenesToBuild)
                        GUILayout.Label(Scene.path);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.Label("All Scenes of Project");
            CurProjectSceneScrollPosition = GUILayout.BeginScrollView(CurProjectSceneScrollPosition, "Box", GUILayout.Width(position.width - 2 * Offset), GUILayout.Height(ScrollHeight));
            {
                GUILayout.BeginVertical();
                {
                    foreach (SceneBuildInfo Scene in ScenesList)
                    {
                        if (Scene.bBuild)
                            GUI.color = Color.green;
                        Scene.bBuild = GUILayout.Toggle(Scene.bBuild, Scene.Path);
                        GUI.color = Color.white;
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.BeginArea(new Rect(Offset, position.height - Offset - 30, position.width - Offset, position.height - Offset));
            {
                GUILayout.BeginHorizontal();
                {
                    bool Press = false;
                    Press = GUILayout.Button("Refresh", GUILayout.Width(100));
                    ProcessRefresh(Press);

                    Press = GUILayout.Button("Add To Build", GUILayout.Width(150));
                    ProcessAddToBuild(Press);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
    }

    void ReloadScenesFromLocal()
    {
        ScenesList.Clear();
        List<string> Scenes = EditorTools.GetAllFileInFolderFullPath(TorchLightConfig.TorchLightSceneFolder, ".unity");

        foreach (string Scene in Scenes)
        {
            SceneBuildInfo Info = new SceneBuildInfo();
            Info.Path   = Scene.ToLower();
            Info.bBuild = CheckSceneInBuildList(Info.Path);
            ScenesList.Add(Info);
        }
    }

    void ProcessRefresh(bool Press)
    {
        if (Press)
            ReloadScenesFromLocal();
    }

    void ClearAllBuilds()
    {
        EditorBuildSettingsScene[] S = new EditorBuildSettingsScene[1];
        S[0] = new EditorBuildSettingsScene("Clear", false);
        EditorBuildSettings.scenes = S;

        foreach (SceneBuildInfo Info in ScenesList)
            Info.bBuild = false;
    }

    void ProcessAddToBuild(bool Press)
    {
        if (Press)
        {
            List<EditorBuildSettingsScene> BuildScene = new List<EditorBuildSettingsScene>();
            foreach(SceneBuildInfo Info in ScenesList)
            {
                if (Info.bBuild)
                {
                    BuildScene.Add(new EditorBuildSettingsScene(Info.Path, true));
                }
            }

            if (BuildScene.Count > 0)
                EditorBuildSettings.scenes = BuildScene.ToArray();
            else
                ClearAllBuilds();
        }
    }

    bool CheckSceneInBuildList(string Path)
    {
         EditorBuildSettingsScene[] ScenesToBuild = EditorBuildSettings.scenes;
         foreach (EditorBuildSettingsScene Scene in ScenesToBuild)
         {
             if (Scene.path.ToLower() == Path)
                 return true;
         }
         return false;
    }

    void OnProjectChange()
    {
        ReloadScenesFromLocal();
    }
}
