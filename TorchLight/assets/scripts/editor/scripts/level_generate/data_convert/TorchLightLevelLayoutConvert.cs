using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TorchLightLevelLayoutConvert : MonoBehaviour {

    public static void ConvertLevelLayout()
    {
        List<string> AllLayouts = EditorTools.GetAllFileInFolderFullPath(TorchLightConfig.TorchLightOrignalLayoutFolder, ".layout");

        foreach (string Layout in AllLayouts)
        {
            string Folder = EditorTools.GetFullFolder(Layout).ToLower();
            Folder = EditorTools.ConvertBasePath(Folder, TorchLightConfig.TorchLightConvertedLayoutFolder, TorchLightConfig.FolderOrignalLevelLayout);
            
            EditorTools.CheckFolderExit(Folder);

            string FileName = EditorTools.GetNameWithoutSuffix(Layout) + ".layout.txt";
            string SavePath = Folder + "/" + FileName;
            StreamWriter Writer = new StreamWriter(SavePath.ToLower());

            List<TorchLightLevel.LevelItem> Items = ParseOriginalLevelLayout(Layout);
            foreach (TorchLightLevel.LevelItem Item in Items)
            {
                Writer.WriteLine(TorchLightLevel.ChunkBegin);
                {
                    Writer.WriteLine("TAG:" + Item.Tag);
                    Writer.WriteLine("NAME:" + Item.Name);
                    Writer.WriteLine("GUID:" + Item.GUID);
                    Writer.WriteLine("POSITION:" + Item.Position.x + "," + Item.Position.y + "," + Item.Position.z);
                    Writer.WriteLine("ROTARION:" + Item.Rotation.eulerAngles.x + "," + Item.Rotation.eulerAngles.y + "," + Item.Rotation.eulerAngles.z);
                    Writer.WriteLine("SCALE:" + Item.Scaling);
                    Writer.WriteLine("RESFILE:" + Item.ResFile);
                    Writer.WriteLine("EXTERNINFO:" + Item.ExternInfo);
                }
                Writer.WriteLine(TorchLightLevel.ChunkEnd);
            }

            Writer.Close();
        }
    }

    static Vector3 RIGHT_VECTOR = new Vector3(1.0f, 0.0f, 0.0f);
    static private List<TorchLightLevel.LevelItem> ParseOriginalLevelLayout(string LayoutPath)
    {
        List<TorchLightLevel.LevelItem> LevelItems = new List<TorchLightLevel.LevelItem>();
        {
            StreamReader Reader = EditorTools.GetStreamReaderFromFile(LayoutPath);

            while (!Reader.EndOfStream)
            {
                string Line = Reader.ReadLine().Trim();

                if (Line == TorchLightLevel.ChunkBegin)
                {
                    Line = Reader.ReadLine().Trim();

                    string Tag = ""; string Value = "";
                    EditorTools.ParseLine(Line, ref Tag, ref Value);

                    if (Value == TorchLightLevel.DESCREPTION_ROOM_PIECE ||
                        Value == TorchLightLevel.DESCREPTION_MONSTER ||
                        Value == TorchLightLevel.DESCREPTION_LIGHT ||
                        Value == TorchLightLevel.DESCREPTION_UNIT_TRIGGER ||
                        Value == TorchLightLevel.DESCREPTION_WARPER ||
                        Value == TorchLightLevel.DESCREPTION_PARTICLE ||
                        Value == TorchLightLevel.DESCREPTION_LAYOUT_LINK)
                    {
                        TorchLightLevel.LevelItem AItem = new TorchLightLevel.LevelItem();
                        AItem.Tag = Value;

                        Vector3 RightDirection = -RIGHT_VECTOR;

                        Line = Reader.ReadLine().Trim();
                        while (Line != TorchLightLevel.ChunkEnd)
                        {
                            EditorTools.ParseLine(Line, ref Tag, ref Value);

                            if (Tag == "NAME") AItem.Name = Value;
                            else if (Tag == "POSITIONX")                    AItem.Position.x = float.Parse(Value);
                            else if (Tag == "POSITIONY")                    AItem.Position.y = float.Parse(Value);
                            else if (Tag == "POSITIONZ")                    AItem.Position.z = -float.Parse(Value);
                            else if (Tag == "RIGHTX")                       RightDirection.x = -float.Parse(Value);
                            else if (Tag == "RIGHTY")                       RightDirection.y = -float.Parse(Value);
                            else if (Tag == "RIGHTZ")                       RightDirection.z = float.Parse(Value);
                            else if (Tag == "SCALE")                        AItem.Scaling = float.Parse(Value);
                            else if (Tag == "SCALE X")                      AItem.Scaling = float.Parse(Value);
                            else if (Tag == "GUID")                         AItem.GUID = Value;
                            else if (Tag == "FILE" || Tag == "LAYOUT FILE") AItem.ResFile = TLPathConvertToUnityPath(Value); // Mesh Path
                            else if (Tag == "DUNGEON NAME")                 AItem.ExternInfo = Value;

                            Line = Reader.ReadLine().Trim();
                        }

                        AItem.Rotation = Quaternion.FromToRotation(RIGHT_VECTOR, RightDirection);
                        LevelItems.Add(AItem);
                    }
                }
            }

            Reader.Close();
        }
        return LevelItems;
    }

    public static string TLPathConvertToUnityPath(string Path)
    {
        if (Path.IndexOf("models/", System.StringComparison.CurrentCultureIgnoreCase) != -1)
            return PathConvertToUnityPath("models/", Path, ".fbx");
        else if (Path.IndexOf("lights/", System.StringComparison.CurrentCultureIgnoreCase) != -1)
            return PathConvertToUnityPath("lights/", Path, ".fbx");
        else if (Path.IndexOf("levelsets/", System.StringComparison.CurrentCultureIgnoreCase) != -1)
            return PathConvertToUnityPath("levelsets/", Path, ".layout");
        else if (Path.IndexOf("particles/", System.StringComparison.CurrentCultureIgnoreCase) != -1)
            return PathConvertToUnityPath("particles/", Path, ".layout");

        return "Unknow";
    }

    public static string PathConvertToUnityPath(string Folder, string Path, string Suffix)
    {
        string NewPath = Path.ToLower();
        NewPath = NewPath.Substring(0, NewPath.IndexOf('.')) + Suffix;
        NewPath = NewPath.Substring(NewPath.IndexOf(Folder) + Folder.Length);
        return NewPath;
    }
}
