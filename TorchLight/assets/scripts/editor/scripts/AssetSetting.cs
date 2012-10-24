using UnityEditor;
using UnityEngine;
using System.Collections;

public class TrochLightAssetSetting : AssetPostprocessor 
{
    void OnPreprocessModel()		
    {	
        ModelImporter AModelImporter = assetImporter as ModelImporter;
        if(assetImporter.assetPath.ToLower().Contains(".fbx"))
        {
            AModelImporter.globalScale          = 1.0f;
            AModelImporter.optimizeMesh         = true;
            AModelImporter.meshCompression      = ModelImporterMeshCompression.Medium;
            AModelImporter.tangentImportMode    = ModelImporterTangentSpaceMode.None;
			
			AModelImporter.swapUVChannels	= false;
            if (assetImporter.assetPath.ToLower().Contains("levelsets") && !assetImporter.assetPath.ToLower().Contains("collision"))
                AModelImporter.generateSecondaryUV  = true;
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
