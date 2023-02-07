using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

using UnityEngine;
using mKit;


[UnityEditor.AssetImporters.ScriptedImporter(1, "vm2")]
public class Vm2ImportExportVolume : UnityEditor.AssetImporters.ScriptedImporter
{
    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        Texture3D texture = ImportVm2(ctx.assetPath, generateMips: false);

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
    public static Texture3D ImportVm2(string filepath, bool generateMips = false)
    {
        Texture3D texture = null;

        Vm2Header header = Vm2Header.FromFile(filepath);

        if (header != null)
        {
            Vm2 vm2 = Vm2.LoadFile(filepath, null, null);

            if (vm2.RawDataByteArray == null || vm2.RawDataByteArray.Length == 0)
                Debug.LogError("vm2.RawDataByteArray is null or empty");

            GraphicsFormat textureFormat = GraphicsFormat.None;
            TextureCreationFlags flags = TextureCreationFlags.None;

            switch (header.FormatNumber)
            {
                case Vm2.FormatEnum.RGB_555:
                    textureFormat = GraphicsFormat.R5G6B5_UNormPack16;
                    break;
                case Vm2.FormatEnum.PHOTO_RGBA32:
                    textureFormat = GraphicsFormat.R8G8B8A8_UNorm;
                    break;
                case Vm2.FormatEnum.CT8:
                case Vm2.FormatEnum.MR8:
                    textureFormat = GraphicsFormat.R8_UNorm;
                    break;
            }

            if (textureFormat != GraphicsFormat.None)
            {
                uint pixelSize = GraphicsFormatUtility.GetBlockSize(textureFormat);
                Debug.Log("ReadIntoTexture3D: Pixel size = " + pixelSize);

                //// POT requirement in shader
                //var texWidth = Mathf.NextPowerOfTwo(header.Vx_x);
                //var texHeight = Mathf.NextPowerOfTwo(header.Vx_y);
                //var texDepth = Mathf.NextPowerOfTwo(header.Vx_z);
                //texture = new Texture3D(texWidth, texHeight, texDepth, textureFormat, flags);

                texture = new Texture3D(header.Vx_x, header.Vx_y, header.Vx_z, textureFormat, flags);

                if (texture != null)
                {
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.SetPixelData(vm2.RawDataByteArray, 0);
                    texture.Apply();
                }
            }
            else
            {
                Debug.LogError("ImportExportVolume.ImportVm2 ("+header.FormatNumber+"): No suitable import format (" + filepath + ")");
            }
        }
        else
        {
            Debug.LogError("ImportExportVolume.ImportVm2: Not a VM2(" + filepath + ")");
        }

        return texture;
    }
}

