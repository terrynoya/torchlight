using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class FStrata
{
    public string RuleSet = "";
    public int MonsterLevelMin = 0;
    public int MonsterLevelMax = 0;

    public string RuleSetName = "Unkown";
    public string BackgroundMusic = "";
    public Vector3 LightDir = Vector3.zero;
    public Color LightColor = Color.white;
    public Color AmbientColor = Color.white;
    public Color FogColor = Color.white;
    public float FogStart = 0.0f;
    public float FogEnd = 1.0f;

    public FStrata(string InRuleSet)
    {
        RuleSet = TorchLightConfig.TorchLightConvertedLayoutFolder + InRuleSet + ".txt";
        LoadRuleSet();
    }

    void LoadRuleSet()
    {
        StreamReader Reader = EditorTools.GetStreamReaderFromAsset(RuleSet);
        while (!Reader.EndOfStream)
        {
            string Line = Reader.ReadLine();
            string Tag = "", Value = "";

            EditorTools.ParseTag(Line, ref Tag, ref Value);
            if (Tag == "LEVELNAME")
                RuleSetName = Value;
            else if (Tag == "AMBIENT")
                AmbientColor = EditorTools.ParseColor(Value);
            else if (Tag == "BGMUSIC")
                BackgroundMusic = Value;
            else if (Tag == "DIRECTION_COLOR")
                LightColor = EditorTools.ParseColor(Value);
            else if (Tag == "DIRECTION_DIR")
                LightDir = EditorTools.ParseVector3(Value);
            else if (Tag == "FOG_COLOR")
                FogColor = EditorTools.ParseColor(Value);
            else if (Tag == "FOG_BEGIN")
                FogStart = float.Parse(Value);
            else if (Tag == "FOG_END")
                FogEnd = float.Parse(Value);
        }
        Reader.Close();
    }
}

public class FDungeon
{
    public string FilePath = "";
    public string ShortPath = "";

    public string Name = "Default Dungeon";
    public string DisplayName = "";
    public string ParentDungeon = "";
    public int PlayerLevelMin = 0;
    public int PlayerLevelMax = 0;

    public List<FStrata> Startas = new List<FStrata>();
}

public class TorchLightLevel {

    /// <summary>
    /// 
    /// </summary>

    public static string CHUNK_PIECE_BEGIN = "[PIECE]";
    public static string CHUNK_PIECE_END   = "[/PIECE]";

    public class PirceItem
    {
        public string Name;
        public string GUID;
        public List<string> Meshes = new List<string>();
        public List<string> CollisionMeshes = new List<string>();
    }

    static public Dictionary<string, PirceItem> GetAllPieceItems()
    {
        string LayoutSetItemPath = TorchLightConfig.TorchLightConvertedLevelSetPath;

        Dictionary<string, PirceItem> PieceItems = new Dictionary<string, PirceItem>();
        {
            StreamReader Reader = EditorTools.GetStreamReaderFromAsset(LayoutSetItemPath);

            while (!Reader.EndOfStream)
            {
                string Line = Reader.ReadLine().Trim();

                if (Line == CHUNK_PIECE_BEGIN)
                {
                    PirceItem AItem = new PirceItem();

                    Line = Reader.ReadLine().Trim();
                    while (Line != CHUNK_PIECE_END)
                    {
                        string Tag = ""; string Value = "";

                        EditorTools.ParseTag(Line, ref Tag, ref Value);

                        if (Tag == "NAME")              AItem.Name = Value;
                        else if (Tag == "GUID")         AItem.GUID = Value;
                        else if (Tag == "MESH")         AItem.Meshes.Add(TorchLightConfig.TorchLightLevelSetFolder + Value);
                        else if (Tag == "COLLSIONMESH") AItem.CollisionMeshes.Add(TorchLightConfig.TorchLightLevelSetFolder + Value);

                        Line = Reader.ReadLine().Trim();
                    }

                    if (AItem.GUID != null)
                        PieceItems.Add(AItem.GUID, AItem);
                }
            }
            Reader.Close();
        }
        return PieceItems;
    }

    public static string DESCREPTION_MONSTER        = "Monster";
    public static string DESCREPTION_ROOM_PIECE     = "Room Piece";
    public static string DESCREPTION_PARTICLE       = "Layout Link Particle";
    public static string DESCREPTION_LIGHT          = "Light";
    public static string DESCREPTION_LAYOUT_LINK    = "Layout Link";

    public static string DESCREPTION_UNIT_TRIGGER   = "Unit Trigger";
    public static string DESCREPTION_WARPER         = "Warper";
    public static string DESCREPTION_MUSIC          = "Music";

    public static string ChunkBegin    = "[PROPERTIES]";
    public static string ChunkEnd      = "[/PROPERTIES]";

    public class LevelItem
    {
        public string Name;
        public string Tag;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public float Scaling = 1.0f;
        public string GUID = null;
        public string ResFile = null;

        public string ExternInfo;

        public LevelItem() { }
        public LevelItem(LevelItem Clone)
        {
            Name        = Clone.Name;
            Tag         = Clone.Tag;
            Position    = Clone.Position;
            Rotation    = Clone.Rotation;
            Scaling     = Clone.Scaling;
            GUID        = Clone.GUID;
            ResFile     = Clone.ResFile;
            ExternInfo  = Clone.ExternInfo;
        }
    }

    static Dictionary<string, List<LevelItem>> GLevelLayoutsCache = new Dictionary<string, List<LevelItem>>();
    static public List<LevelItem> ParseLevelLayout(string LayoutPath)
    {
        if (GLevelLayoutsCache.ContainsKey(LayoutPath))
            return GLevelLayoutsCache[LayoutPath];

        List<LevelItem> LevelItems = new List<LevelItem>();
        {
            StreamReader Reader = EditorTools.GetStreamReaderFromAsset(LayoutPath + ".txt");
            if (Reader == null) return null;

            while (!Reader.EndOfStream)
            {
                string Line = Reader.ReadLine().Trim();

                if (Line == ChunkBegin)
                {
                    Line = Reader.ReadLine().Trim();
                    LevelItem AItem = new LevelItem();

                    while (Line != ChunkEnd)
                    {
                        string Tag = "", Value = "";
                        EditorTools.ParseTag(Line, ref Tag, ref Value);
                        if      (Tag == "TAG")          AItem.Tag           = Value;
                        else if (Tag == "NAME")         AItem.Name          = Value;
                        else if (Tag == "GUID")         AItem.GUID          = Value;
                        else if (Tag == "POSITION")     AItem.Position      = EditorTools.ParseVector3(Value);
                        else if (Tag == "ROTARION")     AItem.Rotation      = Quaternion.Euler(EditorTools.ParseVector3(Value));
                        else if (Tag == "SCALE")        AItem.Scaling       = float.Parse(Value);
                        else if (Tag == "RESFILE")      AItem.ResFile       = Value;
                        else if (Tag == "EXTERNINFO")   AItem.ExternInfo    = Value;

                        Line = Reader.ReadLine().Trim();
                    }

                    LevelItems.Add(AItem);
                }
            }

            Reader.Close();
        }

        GLevelLayoutsCache.Add(LayoutPath, LevelItems);
        return LevelItems;
    }
}
