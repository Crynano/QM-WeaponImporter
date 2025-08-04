using MGSC;

namespace QM_WeaponImporter.Templates
{
    public class FireModeRecordTemplate : ConfigTableRecordTemplate
    {
        public int AmmoPerShot { get; set; } = 1;

        public int WeaponCastsCount { get; set; } = 1;

        public bool RequiredAllAmmoToShot { get; set; } = false;

        public float Accuracy { get; set; } = 1f;

        public float DamageMult { get; set; } = 1f;

        public float ScatterAngle { get; set; } = 1f;

        public float DelayInSecsBetweenShots { get; set; } = 0.05f;

        public static FireModeRecordTemplate GetExample()
        {
            return new FireModeRecordTemplate()
            {
                Id = "example_id",
                Accuracy = 1.0f,
                DamageMult = 1.0f,
                AmmoPerShot = 0,
                ScatterAngle = 0.1f,
                WeaponCastsCount = 0,
                RequiredAllAmmoToShot = false,
                DelayInSecsBetweenShots = 0.1f,
            };
        }

        public FireModeRecord GetOriginal()
        {
            return new FireModeRecord()
            {
                Id = this.Id,
                Accuracy = this.Accuracy,
                DamageMult= this.DamageMult,
                AmmoPerShot = this.AmmoPerShot,
                ScatterAngle= this.ScatterAngle,
                WeaponCastsCount = this.WeaponCastsCount,
                RequiredAllAmmoToShot= this.RequiredAllAmmoToShot,
                DelayInSecsBetweenShots= this.DelayInSecsBetweenShots,
            };
        }
    }
}