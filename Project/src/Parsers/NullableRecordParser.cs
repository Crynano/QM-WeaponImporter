using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Serialization;

namespace QM_WeaponImporter;
public class NullableRecordParser<T> : IConfigParser where T : class, new()
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
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        var resolver = new DefaultContractResolver();
        resolver.DefaultMembersSearchFlags = resolver.DefaultMembersSearchFlags | System.Reflection.BindingFlags.NonPublic;
        settings.ContractResolver = resolver;
        T instance = JsonConvert.DeserializeObject<T>(data, settings) as T;
        OnParsed?.Invoke(instance);
    }
}