using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace QM_WeaponImporter;
public class ImportParser<T> : IConfigParser where T : class, new()
{
    string identifier;
    Action<T> OnParsed;
    public string Identifier { get => identifier; set => identifier = value; }

    public ImportParser(string identifier, Action<T> OnParsed)
    {
        this.identifier = identifier;
        this.OnParsed = OnParsed;
    }

    public void Parse(string data)
    {
        // TODO -- tidy this up into a reusable settings?
        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        var resolver = new DefaultContractResolver();
        resolver.DefaultMembersSearchFlags = resolver.DefaultMembersSearchFlags | BindingFlags.NonPublic;
        jsonSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        jsonSettings.ContractResolver = resolver;

        Logger.WriteToLog($"Parsing {typeof(T).ToString()}");
        T instance = JsonConvert.DeserializeObject<T>(data, jsonSettings);
        OnParsed?.Invoke(instance);
    }
}