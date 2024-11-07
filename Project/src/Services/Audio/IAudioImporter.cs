namespace QM_WeaponImporter.Services
{
    internal interface IAudioImporter<T>
    {
        T Import(string path);
    }
}