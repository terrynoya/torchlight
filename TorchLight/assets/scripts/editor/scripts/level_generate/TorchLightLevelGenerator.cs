using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class TorchLightLevelGenerator : EditorWindow {

    [MenuItem("TorchLight/Editor/LevelGenerator")]
    static void Execute()
    {
        TorchLightLevelGenerator LevelGenerator = CreateInstance<TorchLightLevelGenerator>();
        LevelGenerator.title = "Level Generator";
        LevelGenerator.Init();
        LevelGenerator.Show();
    }

    public static List<FDungeon> LoadAllDungeons()
    {
        List<FDungeon> Dungeons = new List<FDungeon>();
        {
            List<string> Files = EditorTools.GetAllFileInFolderFullPath(TorchLightConfig.TorchLightDungeonFolder, "dat.txt");
            foreach (string AFile in Files)
            {
                try
                {
                    FStrata CurStrata   = null;
                    FDungeon Dungeon    = new FDungeon();
                    Dungeon.FilePath    = AFile;

                    StreamReader Reader = EditorTools.GetStreamReaderFromAsset(AFile);

                    while (!Reader.EndOfStream)
                    {
                        string Line = Reader.ReadLine();
                        string Tag = "", Value = "";
                        EditorTools.ParseLine(Line, ref Tag, ref Value);

                        if (Tag == "NAME")
                            Dungeon.Name = Value;
                        else if (Tag == "DISPLAYNAME")
                            Dungeon.DisplayName = Value;
                        else if (Tag == "PARENT_DUNGEON")
                            Dungeon.ParentDungeon = Value;
                        else if (Tag == "PLAYER_LVL_MATCH_MIN")
                            Dungeon.PlayerLevelMin = int.Parse("0" + Value);
                        else if (Tag == "PLAYER_LVL_MATCH_MAX")
                            Dungeon.PlayerLevelMax = int.Parse("0" + Value);
                        else if (Tag == "RULESET")
                        {
                            CurStrata = new FStrata(Value);
                            Dungeon.Startas.Add(CurStrata);
                        }
                        else if (Tag == "MONSTER_LVL_MIN")
                        {
                            if (CurStrata != null) CurStrata.MonsterLevelMin = int.Parse("0" + Value);
                        }
                        else if (Tag == "MONSTER_LVL_MAX")
                        {
                            if (CurStrata != null) CurStrata.MonsterLevelMax = int.Parse("0" + Value);
                        }
                    }
                    Reader.Close();

                    Dungeons.Add(Dungeon);
                }
                catch (System.Exception)
                {
                    Debug.LogError("Dungeon Load Failed : " + AFile);
                }
            }
        }
        return Dungeons;
    }

    List<FDungeon> Dungeons = null;
    void Init()
    {
        Dungeons = LoadAllDungeons();
        GetBackgroundTexture();
    }

    static Texture2D BackgroundTexture = null;
    static Texture2D GetBackgroundTexture()
    {
        if (BackgroundTexture == null)
            BackgroundTexture = AssetDatabase.LoadAssetAtPath(TorchLightConfig.TorchLightLogoIconPath, typeof(Texture2D)) as Texture2D;
        return BackgroundTexture;
    }

    Rect PropertyWindowRect;
    void OnGUI()
    {
        if (Event.current == null) { EditorGUIUtility.ExitGUI(); }

        float DungeonWindowWidth    = 340.0f;
        float StrataWindowWidth     = 230.0f;
        float WindowY               = 24.0f;
        float WindowWidth = position.width, windowHeight = position.height - WindowY;

        bool ButtonPress = false;
        GUILayout.BeginHorizontal();
        {
            ButtonPress = GUILayout.Button("Convert TorchLight Resource", GUILayout.Width(200));
            ProcessConvertTorchLightResource(ButtonPress);
            ButtonPress = GUILayout.Button("Reload Dungeons", GUILayout.Width(200));
            ProcessReloadDungeons(ButtonPress);
            GUILayout.Space(20);
            ButtonPress = GUILayout.Button("(Test) Load A Level Layout", GUILayout.Width(200));
            ProcessLoadALevelLayout(ButtonPress);
        }
        GUILayout.EndHorizontal();
        
		try
		{
        	BeginWindows();
        	GUI.Window(0, new Rect(0, WindowY, DungeonWindowWidth, windowHeight), DoDrawDungeonListWindow, "", GUI.skin.box);
        	GUI.Window(1, new Rect(DungeonWindowWidth +1, WindowY, StrataWindowWidth, windowHeight), DoDrawRuleListWindow, "", GUI.skin.box);
        	PropertyWindowRect = GUI.Window(2, new Rect(StrataWindowWidth + DungeonWindowWidth + 2, WindowY, WindowWidth - StrataWindowWidth - DungeonWindowWidth - 3, windowHeight), DoDrawPropertyWindow, "", GUI.skin.box);
        	EndWindows();
		}catch(Exception){}
    }

    void ProcessLoadALevelLayout(bool ButtonPress)
    {
        if (ButtonPress)
        {
            string Path = "";
            if (EditorGUIUtil.OpenFile("Select Level Layout to Scene", "txt", ref Path))
            {
                Path = EditorTools.GetUnityRelativePath(Path);
                Path = EditorTools.GetFullFolder(Path) + EditorTools.GetNameWithoutSuffix(Path);
                TorchLightLevelBuilder.LoadLevelLayoutToScene(Path + ".layout");
            }
        }
    }

    void ProcessConvertTorchLightResource(bool ButtonPress)
    {
        if (ButtonPress)
        {
            // Convert TL LevelSet Items
            TorchLightLevelLayoutSetConvert.ParseLevelSet();

            // Convert TL levellayout
            TorchLightLevelLayoutConvert.ConvertLevelLayout();

            // Convert TL LevelLayout Rule Files
            TorchLightLevelLayoutRuleConvert.ConvertLevelRuleFile();

            Debug.Log("TorchLight Level Resource Convert Finished.");
        }
    }

    void ProcessReloadDungeons(bool ButtonPress)
    {
        if (ButtonPress)
        {
            Init();
        }
    }

    FDungeon CurSelectDungeon = null;
    Vector2 DungeonScrollPosition = Vector2.zero;
    void DoDrawDungeonListWindow(int WindowID)
    {
        if (Dungeons == null)
            return;

        DungeonScrollPosition = GUILayout.BeginScrollView(DungeonScrollPosition);
        {
            GUILayout.BeginVertical();
            foreach(FDungeon Dungeon in Dungeons)
            {
                if (Dungeon == CurSelectDungeon)
                    GUI.color = Color.green;

                bool ButtonPress = GUILayout.Button(Dungeon.FilePath, GUI.skin.label);
                ProcessSelectDungeon(ButtonPress, Dungeon);
                GUI.color = Color.white;
            }
            GUILayout.EndVertical();
        }      
        GUILayout.EndScrollView();
    }

    void ProcessSelectDungeon(bool ButtonPress, FDungeon CurDungeon)
    {
        if (ButtonPress)
        {
            if (CurSelectDungeon != CurDungeon)
                CurSelectStrata     = null;

            CurSelectDungeon    = CurDungeon;
        }
    }

    int CurStrataNum = 0;
    FStrata CurSelectStrata = null;
    Vector2 RuleListScrollPosition = Vector2.zero;
    void DoDrawRuleListWindow(int WindowID)
    {
        if (CurSelectDungeon == null)
            return;

        GUILayout.BeginVertical();
        {
            GUILayout.Label("Name", GUILayout.Width(100));
            CurSelectDungeon.Name = GUILayout.TextField(CurSelectDungeon.Name);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        {
            GUILayout.Label("Display Name", GUILayout.Width(100));
            CurSelectDungeon.DisplayName = GUILayout.TextField(CurSelectDungeon.DisplayName);
        }
        GUILayout.EndVertical();

        GUILayout.Space(5);
        RuleListScrollPosition = GUILayout.BeginScrollView(RuleListScrollPosition);
        {
            if (CurSelectDungeon != null)
            {
                int Num = 0;
                foreach(FStrata Strata in CurSelectDungeon.Startas)
                {
                    if (Strata == CurSelectStrata)
                        GUI.color = Color.green;

                    bool ButtonPress = GUILayout.Button("Starta" + Num + "  (" + Strata.RuleSetName + ")", GUI.skin.label);
                    ProcessSelectStrata(ButtonPress, Strata, Num);
                    Num++;

                    GUI.color = Color.white;
                }
            }           
        }
        GUILayout.EndScrollView();
        GUILayout.Space(5);
    }

    void ProcessSelectStrata(bool PressButton, FStrata Strata, int Num)
    {
        if (PressButton)
        {
            CurSelectStrata = Strata;
            CurStrataNum = Num;
        }
    }

    void DoDrawPropertyWindow(int WindowID)
    {
        float WindowWidth = PropertyWindowRect.width, windowHeight = PropertyWindowRect.height;
        if (BackgroundTexture != null)
            GUI.DrawTexture(new Rect(WindowWidth - 125, windowHeight - 140, 128, 128), BackgroundTexture);

        if (CurSelectStrata == null)
            return;

        EditorGUIUtility.LookLikeControls();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Parent Dungeon", GUILayout.Width(150));
            CurSelectDungeon.ParentDungeon = GUILayout.TextField(CurSelectDungeon.ParentDungeon, GUILayout.Width(100));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Player Level Match", GUILayout.Width(150));
            CurSelectDungeon.PlayerLevelMin = EditorGUILayout.IntField(CurSelectDungeon.PlayerLevelMin, GUILayout.Width(100));
            GUILayout.Label("--", GUILayout.Width(10));
            CurSelectDungeon.PlayerLevelMax = EditorGUILayout.IntField(CurSelectDungeon.PlayerLevelMax, GUILayout.Width(100));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Monster Level Match", GUILayout.Width(150));
            CurSelectStrata.MonsterLevelMin = EditorGUILayout.IntField(CurSelectStrata.MonsterLevelMin, GUILayout.Width(100));
            GUILayout.Label("--", GUILayout.Width(10));
            CurSelectStrata.MonsterLevelMax = EditorGUILayout.IntField(CurSelectStrata.MonsterLevelMax, GUILayout.Width(100));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            //string [] Rules = new string []{"11111", "22222"};
            GUILayout.Label("Rule Set", GUILayout.Width(150));
            CurSelectStrata.RuleSet = GUILayout.TextField(CurSelectStrata.RuleSet);
            //EditorGUILayout.Popup(0, Rules);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        GUILayout.BeginVertical("box");
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            {
                CurSelectStrata.LightColor = EditorGUILayout.ColorField("DirLight Color", CurSelectStrata.LightColor, GUILayout.Width(300));
                GUILayout.Space(10);
                CurSelectStrata.LightDir = EditorGUILayout.Vector3Field("Light Direction", CurSelectStrata.LightDir, GUILayout.Width(335));
            }
            GUILayout.EndHorizontal();

            CurSelectStrata.AmbientColor = EditorGUILayout.ColorField("Ambient Color", CurSelectStrata.AmbientColor, GUILayout.Width(300));

            GUILayout.BeginHorizontal();
            {
                CurSelectStrata.FogColor = EditorGUILayout.ColorField("Fog Color", CurSelectStrata.FogColor, GUILayout.Width(300));
                GUILayout.Space(10);
                GUILayout.Label("Fog : Start", GUILayout.Width(75));
                CurSelectStrata.FogStart = EditorGUILayout.FloatField("", CurSelectStrata.FogStart, GUILayout.Width(100));
                GUILayout.Label(" - End", GUILayout.Width(50));
                CurSelectStrata.FogEnd = EditorGUILayout.FloatField("", CurSelectStrata.FogEnd, GUILayout.Width(100));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        GUILayout.EndVertical();

        EditorGUIUtility.LookLikeInspector();

        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        {
            CreateNewScene  = GUILayout.Toggle(CreateNewScene, "Create New Scene", GUILayout.Width(200));
            SaveAfterCreate = GUILayout.Toggle(SaveAfterCreate, "Save After Create", GUILayout.Width(200));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            SplitToSubScene = GUILayout.Toggle(SplitToSubScene, "Split Scene Into SubScenes", GUILayout.Width(200));
            GUILayout.Toggle(SplitToSubScene, "Output A Full Scene for Navmesh", GUILayout.Width(200));
        }
        GUILayout.EndHorizontal();

        bool ButtonPress = false;
        ButtonPress = GUILayout.Button("Generate Level", GUILayout.Width(150));
        ProcessGenerateLevel(ButtonPress);
    }

    bool CreateNewScene  = true;
    bool SaveAfterCreate = true;
	bool SplitToSubScene = false;
    void ProcessGenerateLevel(bool ButtonPress)
    {
        if (ButtonPress)
        {
            if (CreateNewScene && !SplitToSubScene)
                EditorApplication.NewScene();
			
            string RelativePath = GetSceneRelatePath();
			string Prefix = RelativePath.Replace('/', '-');
			
            EditorTools.CheckFolderExit(TorchLightConfig.TorchLightSceneFolder + RelativePath);

            TorchLightLevelRandomGenerater Loader = new TorchLightLevelRandomGenerater();
            Loader.LoadLevelRuleFileToScene(CurSelectStrata, SplitToSubScene, RelativePath);

            if (SaveAfterCreate && !SplitToSubScene)
                EditorApplication.SaveScene(TorchLightConfig.TorchLightSceneFolder + RelativePath + Prefix + "Scene-Full.unity");

            Debug.Log("Generate Level Finished!");
        }
    }

    string GetSceneRelatePath()
    {
        string DungeonName = EditorTools.GetNameWithoutSuffix(CurSelectDungeon.FilePath);
        string StrataName = "Starta" + CurStrataNum;
        return DungeonName + "/" + StrataName + "/";
    }
}
