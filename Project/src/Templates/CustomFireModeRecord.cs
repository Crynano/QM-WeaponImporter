using MGSC;

namespace QM_WeaponImporter.Templates
{
    public class CustomFireModeRecord : MGSC.FireModeRecord
    {
        public string FireModeSpritePath { get; set; } = string.Empty;

        public static CustomFireModeRecord GetExample()
        {
            return new CustomFireModeRecord()
            {
                AmmoPerShot = 0,
                WeaponCastsCount = 0,
                RequiredAllAmmoToShot = false,
                Accuracy = 1.0f,
                DamageMult = 1.0f,
                ScatterAngle = 0.1f,
                DelayInSecsBetweenShots = 0.1f,
                FireModeSpritePath = "Assets/Sprites/sprite.png"
            };
        }
    }
}