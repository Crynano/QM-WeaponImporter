using System;
using System.IO;
using UnityEngine;

namespace QM_WeaponImporter.Services;
internal class TextureImporter
{
    public Texture2D Import(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            try
            {
                FileData = File.ReadAllBytes(FilePath);
                // Linear must be set to true.
                Tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
                Tex2D.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(Tex2D, FileData);
                return Tex2D;
            }
            catch (Exception e)
            {
                Logger.LogError($"Could not load image from {FilePath}. Error: {e.Message}");
            }
        }
        return null;
    }
}