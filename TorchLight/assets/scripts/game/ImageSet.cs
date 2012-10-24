using UnityEngine;
using System.Collections.Generic;

public class FImageSet 
{
    public string ImagePath;
    public string Name;
    public Texture2D Texture = null;
    public List<FTextureAtlas> Icons = new List<FTextureAtlas>();

    public List<string> GetIconNameList()
    {
        List<string> ImageNames = new List<string>();
        foreach (FTextureAtlas Atlas in Icons)
            ImageNames.Add(Atlas.Name);
        return ImageNames;
    }
}
