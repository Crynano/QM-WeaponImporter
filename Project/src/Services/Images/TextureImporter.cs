using System;
using System.IO;
using UnityEngine;

namespace QM_WeaponImporter.Services;
internal class TextureImporter
{
    private static string defaultTexture = "iVBORw0KGgoAAAANSUhEUgAAACwAAAAsCAIAAACR5s1WAAAAaklEQVRYCe3WsRFAQABFQac7PVAdPWhPtA04geBJnuSMWT8wzu1a5q7j3ucesKyT5z853ktgTCIJAtomkiCgbSIJAtomkiCgbYLEcPO+8//rfQ76SSRBQNtEEgS0TSRBQNtEEgS0TfxK4gGfNATn17aOOAAAAABJRU5ErkJggg==";
    public Texture2D Import(string FilePath)
    {
        Texture2D Tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
        Tex2D.filterMode = FilterMode.Point;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            try
            {
                FileData = File.ReadAllBytes(FilePath);
                ImageConversion.LoadImage(Tex2D, FileData);
                return Tex2D;
            }
            catch (Exception e)
            {
                Logger.LogWarning($"Could not load image from {FilePath}. Error: {e.Message}");
                ImageConversion.LoadImage(Tex2D, Convert.FromBase64String(defaultTexture));
            }
        }
        else
        {
            ImageConversion.LoadImage(Tex2D, Convert.FromBase64String(defaultTexture));
        }
        return Tex2D;
    }
}