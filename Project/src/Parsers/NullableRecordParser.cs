using Newtonsoft.Json;
using System;
using MGSC;

namespace QM_WeaponImporter;
public class NullableRecordParser<T> : IConfigParser where T : ConfigTableRecord
{
    string identifier;
    Action<T> OnParsed;
    public string Identifier { get => identifier; set => identifier = value; }

    public NullableRecordParser(string identifier, Action<T> OnParsed)
    {
        this.identifier = identifier;
        this.OnParsed = OnParsed;
    }

    public void Parse(string data)
    {
        Logger.WriteToLog($"Data passed is {data}");
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        T instance = JsonConvert.DeserializeObject<T>(data, settings);
        Logger.WriteToLog($"Data passed is {instance}");
        OnParsed?.Invoke(instance);
    }
}