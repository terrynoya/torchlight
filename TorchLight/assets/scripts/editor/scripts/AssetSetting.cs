using UnityEditor;
using UnityEngine;
using System.Collections;

public class TrochLightAssetSetting : AssetPostprocessor 
{
    void OnPreprocessModel()		
    {
        string AssetPath = assetImporter.assetPath.ToLower();
        ModelImporter AModelImporter = assetImporter as ModelImporter;
        if (AssetPath.EndsWith(".fbx"))
        {
            AModelImporter.globalScale          = 1.0f;
            AModelImporter.optimizeMesh         = true;
            AModelImporter.meshCompression      = ModelImporterMeshCompression.Medium;
            AModelImporter.tangentImportMode    = ModelImporterTangentSpaceMode.None;
			
			AModelImporter.swapUVChannels	= false;

            if (AssetPath.Contains("levelsets"))
            {
                // collision mesh don't need normal
                if (AssetPath.Contains("collision"))
                    AModelImporter.normalImportMode = ModelImporterTangentSpaceMode.None;
                else
                    // for static mesh, we need seconderyUV for lightmapping
                    AModelImporter.generateSecondaryUV = true;
            }
        }
    }
	
    //void OnPreprocessTexture()
    //{
        //TextureImporter texture_importer = assetImporter as TextureImporter;
        //if(texture_importer != null)			
        //{
        //    if(assetImporter.assetPath.ToLower().Contains("_n"))
        //        texture_importer.textureType = TextureImporterType.Bump;
        //    else
        //        texture_importer.textureType = TextureImporterType.Image;
        //}
    //}
}
