using UnityEngine;

namespace QM_WeaponImporter.Services;
internal class SpriteImporter
{
    public Sprite Import(string fullPath, Vector2 size, float imagePixelScaling, bool returnDefault = true)
    {
        Texture2D importedTexture = new TextureImporter().Import(fullPath, returnDefault);
        Rect oldRect = new Rect(0, 0, importedTexture.width, importedTexture.height);
        Sprite NewSprite = Sprite.Create(importedTexture, oldRect, size, imagePixelScaling);
        return NewSprite;
    }
}