using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

using UnityEngine;
using mKit;


[UnityEditor.AssetImporters.ScriptedImporter(1, "ess")]
public class ESSImportExportVolume : UnityEditor.AssetImporters.ScriptedImporter
{
    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        Texture3D texture = ImportESS(ctx.assetPath, generateMips: false);

        if (texture != null)
        {
            ctx.AddObjectToAsset("Texture", texture);
            ctx.SetMainObject(texture);
        }
    }

    /// <summary>
    /// Reads the given volume file into a Texture3D.
    /// </summary>
    /// <param name="filepath">Path of the file to read.</param>
    /// <returns>Unity Texture3D object filled with the contents of the passed volume file.</returns>
    public static Texture3D ImportESS(string filepath, bool generateMips = false)
    {
        Texture3D texture = null;

        Vm2Header header = Vm2Header.FromFile(filepath.Replace(Config.EssExtension, Config.Vm2Extension));

        if (header != null)
        {
            EssData ess = Vm2ESS.ReadData(new FileInfo(filepath));

            Debug.Log("ESSImportExportVolume: blockwidth=" + ess.blockWidth + " for " + filepath);

            if (ess.data == null || ess.data.Length == 0)
                Debug.LogError("EssData.data is null or empty");

            GraphicsFormat textureFormat = textureFormat = GraphicsFormat.R8_UNorm;
            TextureCreationFlags flags = TextureCreationFlags.None;

            uint pixelSize = GraphicsFormatUtility.GetBlockSize(textureFormat);
            //Debug.Log("ReadIntoTexture3D: Pixel size = " + pixelSize);

            var essWidth = header.Vx_x / ess.blockWidth + (header.Vx_x % ess.blockWidth > 0 ? 1 : 0);
            var essHeight = header.Vx_y / ess.blockWidth + (header.Vx_y % ess.blockWidth > 0 ? 1 : 0);
            var essDepth = header.Vx_z / ess.blockWidth + (header.Vx_z % ess.blockWidth > 0 ? 1 : 0);

            texture = new Texture3D(essWidth, essHeight, essDepth, textureFormat, flags);

            if (texture != null)
            {
                texture.filterMode = FilterMode.Trilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.SetPixelData(ess.data, 0);
                texture.Apply();
            }
        }
        else
        {
            Debug.LogError("ImportExportVolume.ImportVm2: Corresponding VM2 not found (ESS: " + filepath + ")");
        }

        return texture;
    }
}

