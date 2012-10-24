using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class EditorTools : MonoBehaviour {

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
                    Files.Add(GetName(File.Name));
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
    public static string GetFolder(string Path)
    {
        string APath = Path.Replace('\\', '/');
        return APath.Substring(0, APath.LastIndexOf('/') + 1);
    }

    // c:/111/111/111.txt
    // return 111
    public static string GetName(string FileName)
    {
        FileName = FileName.Replace('\\', '/');

        int StartIndex = 0;
        if (FileName.IndexOf('/') != -1)
            StartIndex = FileName.LastIndexOf('/') + 1;

        return FileName.Substring(StartIndex, FileName.LastIndexOf('.') - StartIndex);
    }
}
