using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class EditorTools {

    // AAA/layouts/CCC  params  DDD layouts/
    // DDD/layouts/CCC
    public static string ConvertBasePath(string OrignalPath, string BaseFolder, string CutFolder)
    {
        OrignalPath = OrignalPath.Replace('\\', '/');
        OrignalPath = OrignalPath.ToLower();
        int Index = OrignalPath.IndexOf(CutFolder);
        if (Index != -1) return BaseFolder + OrignalPath.Substring(Index + CutFolder.Length);
        return OrignalPath;
    }

    // get all file name with suffixs
    public static List<string> GetAllFileInFolder(string Folder, string Suffix)
    {
        List<string> Files = new List<string>();
        DirectoryInfo Dir = new DirectoryInfo(Folder);
        if (Dir.Exists)
        {
            foreach (FileInfo File in Dir.GetFiles())
            {
                if (File.Name.EndsWith(Suffix, StringComparison.CurrentCultureIgnoreCase))
                    Files.Add(GetNameWithoutSuffix(File.Name));
            }
        }
        return Files;
    }

    public static string GetUnityRelativePath(string Path)
    {
        Path = Path.Replace('\\', '/');
        int Index = Path.IndexOf(TorchLightConfig.TorchLightAssetFolder, StringComparison.CurrentCultureIgnoreCase);
        if (Index != -1) return Path.Substring(Index);
        return Path;
    }

    public static List<string> GetAllFileInFolderFullPath(string Folder, string Suffix)
    {
        List<string> Files = new List<string>();
        DirectoryInfo Dir = new DirectoryInfo(Folder);
        if (Dir.Exists)
        {
            foreach (FileInfo File in Dir.GetFiles())
            {
                if (File.FullName.EndsWith(Suffix, StringComparison.CurrentCultureIgnoreCase))
                    Files.Add(GetUnityRelativePath(File.FullName));
            }

            foreach (DirectoryInfo D in Dir.GetDirectories())
            {
                Files.AddRange(GetAllFileInFolderFullPath(D.FullName, Suffix));
            }
        }
        return Files;
    }

    public static void CheckFolderExit(string Folder)
    {
        string FullPath = Application.dataPath + "/../" + Folder;
        DirectoryInfo Dir = new DirectoryInfo(FullPath);
        if (!Dir.Exists)
            Dir.CreateSubdirectory(FullPath);
    }

    // c:/111/111/111/1.txt
    // return c:/111/111/111/
    public static string GetFullFolder(string Path)
    {
        string APath = Path.Replace('\\', '/');
        return APath.Substring(0, APath.LastIndexOf('/') + 1);
    }

    // c:/111/222/333.layout.txt
    // return 333
    public static string GetNameWithoutSuffix(string FileName)
    {
        FileName = FileName.Replace('\\', '/');

        int StartIndex = 0;
        if (FileName.IndexOf('/') != -1)
            StartIndex = FileName.LastIndexOf('/') + 1;

        //return FileName.Substring(StartIndex, FileName.LastIndexOf('.') - StartIndex);
        return FileName.Substring(StartIndex, FileName.IndexOf('.', StartIndex + 1) - StartIndex);
    }

    public static StreamReader GetStreamReaderFromFile(string Path)
    {
        try
        {
            StreamReader Reader = new StreamReader(Path);
            return Reader;
        }
        catch (Exception)
        {
            Debug.LogError(Path + " Load Failed.");
            return null;
        }
    }

    public static StreamReader GetStreamReaderFromAsset(string Path)
    {
        TextAsset Asset = AssetDatabase.LoadAssetAtPath(Path, typeof(TextAsset)) as TextAsset;
        if (Asset != null)
        {
            Stream BinaryStream = new MemoryStream(Asset.bytes);
            StreamReader Reader = new StreamReader(BinaryStream);

            return Reader;
        }
        else
        {
            Debug.LogError(Path + " Load Failed.");
            return null;
        }
    }

    // C:/AAA/VVV/CCC/AA.TXT
    // -->CCC
    public static string GetUpFolderName(string Path)
    {
        Path = Path.Replace('\\', '/');
        int IndexA = Path.LastIndexOf('/');
        int IndexB = Path.LastIndexOf('/', IndexA - 1);
        return Path.Substring(IndexB + 1, IndexA - IndexB - 1);
    }

    // Parse Line
    // <INT>NAME:10
    // Tag = Name, Value = 10
    public static void ParseLine(string Line, ref string Tag, ref string Value)
    {
        if (Line.IndexOf('>') == -1) return;

        int Index = Line.IndexOf('>');
        int Index1 = Line.IndexOf(':');

        Tag = Line.Substring(Index + 1, Index1 - Index - 1);
        Value = Line.Substring(Index1 + 1);
    }

    public static void ParseTag(string Line, ref string Tag, ref string Value)
    {
        // 1111:2222
        int Index = Line.IndexOf(':');
        if (Index == -1) return;

        Tag = Line.Substring(0, Index);
        Value = Line.Substring(Index + 1);
    }

    public static Color ParseColor(string Line)
    {
        Color OutColor = new Color();
        string[] Value = Line.Split(',');
        OutColor.r = float.Parse(Value[0]) / 255.0f;
        OutColor.g = float.Parse(Value[1]) / 255.0f;
        OutColor.b = float.Parse(Value[2]) / 255.0f;
        return OutColor;
    }

    public static Vector3 ParseVector3(string Line)
    {
        Vector3 Vec = new Vector3();
        string[] Value = Line.Split(',');
        Vec.x = float.Parse(Value[0]);
        Vec.y = float.Parse(Value[1]);
        Vec.z = float.Parse(Value[2]);
        return Vec;
    }
}
