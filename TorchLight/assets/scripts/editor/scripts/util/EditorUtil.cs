using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEditor;

public class EditorUtil 
{
    public static string LoadTextFromFile(string Path)
    {
        string Str = "";
        StreamReader Reader = TorchLightTools.GetStreamReaderFromAsset(Path);
        Str = Reader.ReadToEnd();
        Reader.Close();

        return Str;
    }

    public static Texture2D LoadTexture(string Path)
    {
        Texture2D Texture = AssetDatabase.LoadAssetAtPath(TorchLightConfig.TorchLigthRunTimeResourceFolder + Path, typeof(Texture2D)) as Texture2D;
        if (Texture == null)
            Debug.LogError(TorchLightConfig.TorchLigthRunTimeResourceFolder + Path + " Load Failed");
        return Texture;
    }

    public static FImageSet LoadImageSet(string Path)
    {
        FImageSet ImageSet = new FImageSet();

        string Str = LoadTextFromFile(Path);
        if (Str.Length > 0)
        {
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(Str);

            XmlNode BaseNode    = Doc.SelectSingleNode("Imageset");
            string ImageSetName = BaseNode.Attributes["Name"].InnerText;
            string ImagePath    = BaseNode.Attributes["Imagefile"].InnerText;

            Texture2D Texture   = LoadTexture(ImagePath.Replace(".dds", ".png"));
            ImageSet.Name       = ImageSetName;
            ImageSet.Texture    = Texture;

            XmlNodeList Nodes = Doc.SelectNodes("Imageset/Image");
            foreach (XmlNode Node in Nodes)
            {
                string Name    = Node.Attributes["Name"].InnerText;
                float XPos     = float.Parse(Node.Attributes["XPos"].InnerText);
                float YPos     = float.Parse(Node.Attributes["YPos"].InnerText);
                float Width    = float.Parse(Node.Attributes["Width"].InnerText);
                float Height   = float.Parse(Node.Attributes["Height"].InnerText);

                FTextureAtlas TexAtlas = new FTextureAtlas();
                TexAtlas.Name           = Name;
                TexAtlas.Texture        = Texture;
                TexAtlas.PixelAtlasRect = new Rect(XPos, Texture.height - YPos - Height, Width, Height);

                ImageSet.Icons.Add(TexAtlas);
            }
        }

        return ImageSet;
    }
}
