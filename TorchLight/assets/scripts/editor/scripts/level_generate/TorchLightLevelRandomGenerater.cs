using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TorchLightLevelRandomGenerater
{
    static int DIRECTION_N = 0x0001;
    static int DIRECTION_E = 0x0010;
    static int DIRECTION_S = 0x0100;
    static int DIRECTION_W = 0x1000;

    int GetChunkDirection(LevelChunk Chunk)
    {
        if (Chunk.ChunkName.EndsWith("_N")) return DIRECTION_N;
        if (Chunk.ChunkName.EndsWith("_S")) return DIRECTION_S;
        if (Chunk.ChunkName.EndsWith("_W")) return DIRECTION_W;
        if (Chunk.ChunkName.EndsWith("_E")) return DIRECTION_E;
        if (Chunk.ChunkName.EndsWith("EW")) return (DIRECTION_E | DIRECTION_W);
        if (Chunk.ChunkName.EndsWith("NS")) return (DIRECTION_N | DIRECTION_S);
        if (Chunk.ChunkName.EndsWith("NE")) return (DIRECTION_N | DIRECTION_E);
        if (Chunk.ChunkName.EndsWith("NW")) return (DIRECTION_N | DIRECTION_W);
        if (Chunk.ChunkName.EndsWith("SE")) return (DIRECTION_S | DIRECTION_E);
        if (Chunk.ChunkName.EndsWith("SW")) return (DIRECTION_S | DIRECTION_W);
        Debug.LogError("Chunk Direction Error : " + Chunk.ChunkName);
        return 0;
    }

    int GetOppsiteDirection(int Dir)
    {
        if (Dir == DIRECTION_E) return DIRECTION_W;
        if (Dir == DIRECTION_W) return DIRECTION_E;
        if (Dir == DIRECTION_N) return DIRECTION_S;
        if (Dir == DIRECTION_S) return DIRECTION_N;
        return 0;
    }

    public Vector3 GetDirectionOffset(int Dir)
    {
        Vector3 Offset = Vector3.zero;

        if ((Dir & DIRECTION_N) != 0) Offset.z = -1.0f;
        if ((Dir & DIRECTION_S) != 0) Offset.z = 1.0f;
        if ((Dir & DIRECTION_W) != 0) Offset.x = 1.0f;
        if ((Dir & DIRECTION_E) != 0) Offset.x = -1.0f;

        return Offset;
    }

    public Vector3 GetDirectionOffset(LevelChunk Chunk)
    {
        return GetDirectionOffset(GetChunkDirection(Chunk));
    }

    LevelBuildInfo CurLevelInfo = null;

    public class LevelChunk
    {
        public string       ChunkName;
        public Vector3      Offset = Vector3.zero;
        public List<string> SceneNames = new List<string>();
    }

    public class LevelBuildInfo
    {
        public string LevelName;
        public string DisplayName;

        public int ChunkWidthOffset = 0;
        public int ChunkHeightOffset = 0;

        public string BackgroundMusic;
        public Color AmbientColor = Color.white;
        public Color DirectionLightColor = Color.white;
        public Color FogColor = Color.white;
        public float FogBegin = 0.0f;
        public float FogEnd = 100.0f;

        public Vector3 DirectionLightDir = new Vector3(-1.0f, -1.0f, 1.0f);
        public int MinChunkNum = 0;
        public int MaxChunkNum = 1;

        public List<LevelChunk> LevelChunks = new List<LevelChunk>();
    }

    public bool LoadLevelRuleFile(string LevelRuleFile)
    {
        StreamReader Reader = TorchLightTools.GetStreamReaderFromAsset(LevelRuleFile);

        CurLevelInfo = new LevelBuildInfo();
        while (!Reader.EndOfStream)
        {
            string Line = Reader.ReadLine().Trim();

            string Tag = "", Value = "";
            TorchLightTools.ParseTag(Line, ref Tag, ref Value);

            if (Tag == "LEVELNAME")         CurLevelInfo.LevelName              = Value;
            if (Tag == "BGMUSIC")           CurLevelInfo.BackgroundMusic        = Value;
            if (Tag == "AMBIENT")           CurLevelInfo.AmbientColor           = TorchLightTools.ParseColor(Value);
            if (Tag == "DIRECTION_COLOR")   CurLevelInfo.DirectionLightColor    = TorchLightTools.ParseColor(Value);
            if (Tag == "DIRECTION_DIR")     CurLevelInfo.DirectionLightDir      = TorchLightTools.ParseVector3(Value);
            if (Tag == "FOG_COLOR")         CurLevelInfo.FogColor               = TorchLightTools.ParseColor(Value);
            if (Tag == "FOG_BEGIN")         CurLevelInfo.FogBegin               = float.Parse(Value);
            if (Tag == "FOG_END")           CurLevelInfo.FogEnd                 = float.Parse(Value);
            if (Tag == "MINCHUNK")          CurLevelInfo.MinChunkNum            = int.Parse(Value);
            if (Tag == "MAXCHUNK")          CurLevelInfo.MaxChunkNum            = int.Parse(Value);

            if (Line == "[CHUNKTYPE]")
            {
                LevelChunk Chunk = new LevelChunk();
                while (Line != "[/CHUNKTYPE]")
                {
                    TorchLightTools.ParseTag(Line, ref Tag, ref Value);
                    if (Tag == "CHUNK_NAME") Chunk.ChunkName = Value;
                    if (Tag == "CHUNK_FILE") Chunk.SceneNames.Add(Value);

                    Line = Reader.ReadLine().Trim();
                }

                CurLevelInfo.LevelChunks.Add(Chunk);
            }
        }

        Reader.Close();
        return true;
    }

    public List<LevelChunk> BuildLevel()
    {
        if (CurLevelInfo == null) return new List<LevelChunk>();

        List<LevelChunk>    ChunkLists  = new List<LevelChunk>();
        if (CurLevelInfo.LevelChunks.Count == 1)
        {
            ChunkLists.Add(CurLevelInfo.LevelChunks[0]);
        }
        else
        {
            int ChunkNum = Random.Range(CurLevelInfo.MinChunkNum, CurLevelInfo.MaxChunkNum + 1);
            List<LevelChunk> EnteranceChunks    = new List<LevelChunk>();
            List<LevelChunk> ExitChunks         = new List<LevelChunk>();
            List<LevelChunk> LinkChunks         = new List<LevelChunk>();

            foreach (LevelChunk Chunk in CurLevelInfo.LevelChunks)
            {
                if (Chunk.ChunkName.IndexOf("ENTRANCE") != -1)
                    EnteranceChunks.Add(Chunk);
                else if (Chunk.ChunkName.IndexOf("EXIT") != -1)
                    ExitChunks.Add(Chunk);
                else if (Chunk.ChunkName.IndexOf("ROOM") == -1)
                    LinkChunks.Add(Chunk);
            }

            LevelChunk EnteranceChunk = EnteranceChunks[Random.Range(0, EnteranceChunks.Count)];
            ChunkLists.Add(EnteranceChunk);

            if (ChunkNum == 0)
            {
                LevelChunk ExitChunk = null;
                foreach (LevelChunk Chunk in ExitChunks)
                {
                    int ExitDir = GetOppsiteDirection(GetChunkDirection(EnteranceChunk));
                    if (ExitDir == GetChunkDirection(Chunk))
                    {
                        ExitChunk = Chunk;
                        break;
                    }
                }
                ExitChunk.Offset = GetDirectionOffset(ChunkLists[0]);
                ChunkLists.Add(ExitChunk);
            }
            else
            {
                Vector3 Offset = Vector3.zero;

                HashSet<LevelChunk> UsedChunks = new HashSet<LevelChunk>();
                int CurNeedDirection = GetOppsiteDirection(GetChunkDirection(EnteranceChunk));
                while(ChunkNum != 0)
                {
                    int NextChunkDir = 0;
                    foreach (LevelChunk Chunk in LinkChunks)
                    {
                        NextChunkDir = GetChunkDirection(Chunk);
                        if ((NextChunkDir & CurNeedDirection) != 0 && !UsedChunks.Contains(Chunk))
                        {
                            Offset += GetDirectionOffset(GetOppsiteDirection(CurNeedDirection));
                            Chunk.Offset = Offset;

                            UsedChunks.Add(Chunk);
                            ChunkLists.Add(Chunk);
                            CurNeedDirection = GetOppsiteDirection(NextChunkDir & (~CurNeedDirection));
                            break;
                        }
                    }
                    ChunkNum--;
                }

                foreach(LevelChunk Chunk in ExitChunks)
                {
                    int NextChunkDir = GetChunkDirection(Chunk);
                    if ((CurNeedDirection & NextChunkDir) != 0)
                    {
                        Offset += GetDirectionOffset(GetOppsiteDirection(CurNeedDirection));
                        Chunk.Offset = Offset;
                        ChunkLists.Add(Chunk);
                        break;
                    }
                }
            }
        }

        return ChunkLists;
    }

    public void LoadLevelRuleFileToScene(string RuleFilePath)
    {
        if (LoadLevelRuleFile(RuleFilePath))
        {
            List<TorchLightLevelRandomGenerater.LevelChunk> LevelChunks = BuildLevel();
            foreach (TorchLightLevelRandomGenerater.LevelChunk Chunk in LevelChunks)
            {
                string Path = TorchLightConfig.TorchLightConvertedLayoutFolder + Chunk.SceneNames[0];
                GameObject Level = TorchLightLevelBuilder.LoadLevelLayoutToScene(Path);
                Level.transform.position = Chunk.Offset * 100.0f;
            }
        }
    }
}
