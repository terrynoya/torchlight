using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ESkillInfoInspector {

    public static void DrawTexture(FSkillInfo SkillInfo, Rect InRect)
    {
        if (SkillInfo != null && SkillInfo.ImageIcon.Texture != null)
        {
            GUI.DrawTextureWithTexCoords(InRect, SkillInfo.ImageIcon.Texture, SkillInfo.ImageIcon.AtlasRect);
        }
    }

    static int IMAGE_ICON_WIDTH = 64;
    public static bool DoDrawPropertyWindow(FSkillInfo SkillInfo)
    {
        try
        {
            bool ButtonPress = false;
            bool NeedReflash = false;
            List<string> Textures = FSkillManager.Instance().GetAllImageAltasName();
            string[] TextureName = Textures.ToArray();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Normal Icon");
            SkillInfo.NormalIndex = EditorGUILayout.Popup(SkillInfo.NormalIndex, TextureName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Disable Icon");
            SkillInfo.DisableIndex = EditorGUILayout.Popup(SkillInfo.DisableIndex, TextureName);
            GUILayout.EndHorizontal();

            int TextureBoxHeight = 70;
            GUILayout.Box("", GUILayout.Width(195), GUILayout.Height(TextureBoxHeight));
            Rect LastRect = GUILayoutUtility.GetLastRect();

            if (SkillInfo.NormalIndex > -1 && SkillInfo.NormalIndex < TextureName.Length)
            {
                FTextureAtlas Texture = FSkillManager.Instance().GetTextureAtlas(TextureName[SkillInfo.NormalIndex]);

                Rect LeftRect       = LastRect;
                LeftRect.x          = LeftRect.width / 2 - IMAGE_ICON_WIDTH - 5;
                LeftRect.y          = LastRect.y + LastRect.height / 2 - Texture.PixelAtlasRect.height / 2;
                LeftRect.width      = IMAGE_ICON_WIDTH;
                LeftRect.height     = IMAGE_ICON_WIDTH;
                DrawImageChunk(Texture, LeftRect);

                NeedReflash = SkillInfo.SetTextureAtlas(Texture, false);
            }

            if (SkillInfo.DisableIndex > -1 && SkillInfo.DisableIndex < TextureName.Length)
            {
                FTextureAtlas Texture = FSkillManager.Instance().GetTextureAtlas(TextureName[SkillInfo.DisableIndex]);

                Rect RightRect      = LastRect;
                RightRect.x         = RightRect.width / 2 + 5;
                RightRect.y         = LastRect.y + LastRect.height / 2 - Texture.PixelAtlasRect.height / 2;
                RightRect.width     = IMAGE_ICON_WIDTH;
                RightRect.height    = IMAGE_ICON_WIDTH;
                DrawImageChunk(Texture, RightRect);

                NeedReflash = SkillInfo.SetTextureAtlas(Texture, true);
            }

            GUILayout.BeginHorizontal();
            ButtonPress     = GUILayout.Button("Add Level");
            ProcessAddSkillLevel(ButtonPress, SkillInfo);
            ButtonPress     = GUILayout.Button("Del Level");
            ProcessDelSkillLevel(ButtonPress, SkillInfo);
            GUILayout.EndHorizontal();
            List<string> SkillLevels = new List<string>();
            for (int i = 0; i < SkillInfo.Levels.Count; i++)
                SkillLevels.Add(i.ToString());

            SkillInfo.CurSkillIndex = EditorGUILayout.Popup(SkillInfo.CurSkillIndex, SkillLevels.ToArray());
            if (SkillInfo.CurSkillIndex != -1 && SkillInfo.CurSkillIndex < SkillInfo.Levels.Count)
            {
                FSkillLevelInfo Info    = SkillInfo.Levels[SkillInfo.CurSkillIndex];
                Info.LevelNeed          = EditorGUILayout.IntField("Level Need", Info.LevelNeed);
                Info.DamageValue        = EditorGUILayout.FloatField("Damage Value", Info.DamageValue);
                Info.ManaValue          = EditorGUILayout.FloatField("Mana Value", Info.ManaValue);
                Info.Description        = GUILayout.TextArea(Info.Description, GUILayout.Width(195), GUILayout.Height((100)));
            }

            return NeedReflash;
        }
        catch (System.Exception) { }

        return false;
    }

    static void ProcessAddSkillLevel(bool Press, FSkillInfo Info)
    {
        if (Press)
        {
            Info.CurSkillIndex = Info.AddLevel(new FSkillLevelInfo());
        }
    }

    static void ProcessDelSkillLevel(bool Press, FSkillInfo Info)
    {
        if (Press)
        {
            Info.RemoveLevel(Info.CurSkillIndex);
            Info.CurSkillIndex = -1;
        }
    }

    static bool FoldState = false;
    static int CurSelectedImageSetIndex = 0;
    static int CurSelectedImageIndex = 0;
    public static float IMAGE_DRAW_HEIGHT = 195.0f;
    public static bool DoDrawImageSet(float WindowWidth, float WindowHeight)
    {
        bool ButtonPress = false;
        FoldState = EditorGUILayout.Foldout(FoldState, "Skill Icon Sets");
        if (FoldState)
        {
            ButtonPress = GUILayout.Button("Add ImageSet");
            ProcessLoadImageSet(ButtonPress);
            ButtonPress = GUILayout.Button("Remove ImageSet");
            ProcessRemoveImageSet(ButtonPress);

            List<string> ImageSetNames = FSkillManager.Instance().GetImageSetNameList();

            GUILayout.Label("ImageSets");
            CurSelectedImageSetIndex = EditorGUILayout.Popup(CurSelectedImageSetIndex, ImageSetNames.ToArray());
            FImageSet CurImageSet = FSkillManager.Instance().GetImageSet(CurSelectedImageSetIndex);
            if (CurImageSet != null)
            {
                List<string> ImageNames = CurImageSet.GetIconNameList();

                GUILayout.Label("Image Chunks");
                CurSelectedImageIndex = EditorGUILayout.Popup(CurSelectedImageIndex, ImageNames.ToArray());

                if (CurSelectedImageIndex < CurImageSet.Icons.Count)
                {
                    FTextureAtlas Texture   = CurImageSet.Icons[CurSelectedImageIndex];
                    Rect LastRect           = GUILayoutUtility.GetLastRect();
                    LastRect.y              += LastRect.height;
                    LastRect.width          = Texture.PixelAtlasRect.width;
                    LastRect.height         = Texture.PixelAtlasRect.height;
                    LastRect.x              = LastRect.x + WindowWidth / 2 - LastRect.width / 2;

                    DrawImageChunk(Texture, LastRect);
                }
            }
            DrawPreviewImageSet(CurImageSet, WindowWidth, WindowHeight);
            GUILayout.Label("", GUILayout.Width(IMAGE_DRAW_HEIGHT));

            return true;
        }
        GUILayout.Label("", GUILayout.Width(IMAGE_DRAW_HEIGHT));

        return false;
    }

    static void ProcessRemoveImageSet(bool Press)
    {
        if (Press && CurSelectedImageSetIndex > -1)
        {
            FSkillManager.Instance().RemoveImageSet(CurSelectedImageSetIndex);
            CurSelectedImageSetIndex = 0;
        }
    }

    static void DrawImageChunk(FTextureAtlas Texture, Rect InRect)
    {
        if (Texture != null)
        {
            GUI.DrawTextureWithTexCoords(InRect, Texture.Texture, Texture.AtlasRect);
        }
    }

    static void DrawPreviewImageSet(FImageSet ImageSet, float WindowWidth, float WindowHeight)
    {
        if (ImageSet != null)
        {
            GUI.DrawTexture(new Rect(2.5f, WindowHeight - IMAGE_DRAW_HEIGHT - 15.0f, IMAGE_DRAW_HEIGHT, IMAGE_DRAW_HEIGHT), ImageSet.Texture);
            GUILayout.BeginArea(new Rect(2.5f, WindowHeight - 16.0f, WindowWidth, 20));
            GUILayout.Label("Preview (" + ImageSet.Texture.width + "x" + ImageSet.Texture.height + ")");
            GUILayout.EndArea();
        }
    }

    public static void ProcessLoadImageSet(bool Press)
    {
        if (Press)
        {
            string ImageSetPath = "";
            if (EditorGUIUtil.OpenFile("Open Image Set", "imageset", ref ImageSetPath))
            {
                if (!FSkillManager.Instance().ContantImageSet(ImageSetPath))
                {
                    FImageSet ImageSet = EditorUtil.LoadImageSet(ImageSetPath);
                    if (ImageSet != null)
                    {
                        CurSelectedImageSetIndex = FSkillManager.Instance().AddImageSet(ImageSet);
                        CurSelectedImageIndex = 0;
                    }
                }
            }
        }
    }
}
