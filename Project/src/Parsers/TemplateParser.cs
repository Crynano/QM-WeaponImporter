using Newtonsoft.Json;
using System;

namespace QM_WeaponImporter;
internal class TemplateParser<T> : IConfigParser where T : class, new()
{
    string identifier;
    Action<T> OnParsed;
    public string Identifier { get => identifier; set => identifier = value; }

    public TemplateParser(string identifier, Action<T> OnParsed)
    {
        this.identifier = identifier;
        this.OnParsed = OnParsed;
    }

    public void Parse(string data)
    {
        Logger.LogInfo($"Parsing {typeof(T).ToString()}");
        T instance = JsonConvert.DeserializeObject<T>(data);
        OnParsed?.Invoke(instance);
    }
}