using UnityEngine;

namespace QM_WeaponImporter.Services;
internal class SpriteImporter
{
    public Sprite Import(string fullPath, Vector2 size, float imagePixelScaling)
    {
        Texture2D SpriteTexture = new TextureImporter().Import(fullPath);
        Rect oldRect = new Rect(0, 0, SpriteTexture.width, SpriteTexture.height);
        Sprite NewSprite = Sprite.Create(SpriteTexture, oldRect, size, imagePixelScaling);
        return NewSprite;
    }
}