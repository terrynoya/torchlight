using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FTextureAtlas
{
    public string Name = "";
    public Texture2D    Texture         = null;
    public Rect         PixelAtlasRect  = new Rect(0, 0, 1, 1); // Pixel Rect [0~TextureWidth] [0~TexureHeight]

    public Rect AtlasRect   //[0~1.0]
    {
        get
        {
            Rect RelatedRect = new Rect(0, 0, 1, 1);
            if (Texture != null)
            {
                RelatedRect.x       = PixelAtlasRect.x / Texture.width;
                RelatedRect.y       = PixelAtlasRect.y / Texture.height;
                RelatedRect.width   = PixelAtlasRect.width / Texture.width;
                RelatedRect.height  = PixelAtlasRect.height / Texture.height;
            }
            return RelatedRect;
        }
    }
}

public class FSkillLevelInfo
{
    public string  Description = "Skill Level Description";
    public int LevelNeed = 0;
    public float DamageValue = 1.0f;
    public float ManaValue = 1.0f;

    Dictionary<string, float> ExternPower = new Dictionary<string, float>();
}

public class FSkillInfo
{
    public string Name = "Skill";
    public FTextureAtlas ImageIcon        = new FTextureAtlas();
    public FTextureAtlas DisableImageIcon = new FTextureAtlas();
    public List<FSkillLevelInfo> Levels = new List<FSkillLevelInfo>();

    public int NormalIndex = -1, DisableIndex = -1;
    public int CurSkillIndex = -1;

    public int AddLevel(FSkillLevelInfo Info)
    {
        Levels.Add(Info);
        return Levels.Count - 1;
    }

    public void RemoveLevel(int Index)
    {
        Levels.RemoveAt(Index);
    }

    public bool SetTextureAtlas(FTextureAtlas Texture, bool isDisbale)
    {
        if (isDisbale)
        {
            if (DisableImageIcon != Texture)
            {
                DisableImageIcon = Texture;
                return true;
            }
        }
        else
        {
            if (ImageIcon != Texture)
            {
                ImageIcon = Texture;
                return true;
            }
        }

        return false;
    }

    public bool HasIcon()
    {
        return ImageIcon.Texture != null || DisableImageIcon.Texture != null;
    }
}

public class FSkillManager {

    static FSkillManager GSkillManager = null;
    public static FSkillManager Instance()
    {
        if (GSkillManager == null)
            GSkillManager = new FSkillManager();
        return GSkillManager;
    }

    public List<FImageSet> ImageSets = new List<FImageSet>();
    public bool ContantImageSet(string Path)
    {
        for (int i = 0; i < ImageSets.Count; i++)
            if (ImageSets[i].ImagePath == Path)
                return true;
        return false;
    }

    public int AddImageSet(FImageSet ImageSet)
    {
        ImageSets.Add(ImageSet);
        return ImageSets.Count - 1;
    }

    public void RemoveImageSet(int Index)
    {
        ImageSets.RemoveAt(Index);
    }

    public FImageSet GetImageSet(int Index)
    {
        if (Index > - 1 && Index < ImageSets.Count)
            return ImageSets[Index];
        return null;
    }

    public List<string> GetAllImageAltasName()
    {
        List<string> Names = new List<string>();
        foreach (FImageSet ImageSet in ImageSets)
        {
            foreach (FTextureAtlas Texture in ImageSet.Icons)
            {
                //if (!Texture.Name.Contains("_gray"))
                    Names.Add(Texture.Name);
            }
        }
        return Names;
    }

    public FTextureAtlas GetTextureAtlas(string TextureName)
    {
        foreach (FImageSet ImageSet in ImageSets)
        {
            foreach (FTextureAtlas Texture in ImageSet.Icons)
            {
                if (Texture.Name == TextureName)
                    return Texture;
            }
        }
        return null;
    }

    public List<string> GetImageSetNameList()
    {
        List<string> ImageSetNames = new List<string>();
        foreach (FImageSet ImageSet in ImageSets)
            ImageSetNames.Add(ImageSet.Name);
        return ImageSetNames;
    }
}
