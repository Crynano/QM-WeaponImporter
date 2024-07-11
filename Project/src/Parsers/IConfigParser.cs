namespace QM_WeaponImporter;
public interface IConfigParser
{
    public string Identifier { get; set; }
    public void Parse(string data);
}
