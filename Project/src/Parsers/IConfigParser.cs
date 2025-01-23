namespace QM_WeaponImporter;
internal interface IConfigParser
{
    public string Identifier { get; set; }
    public void Parse(string data);
}
