using Newtonsoft.Json;
using System;

namespace QM_WeaponImporter;
internal class TemplateParser<T> : IConfigParser where T : class, new()
{
    string identifier;
    Action<T> OnParsed;
    public string Identifier { get => identifier; set => identifier = value; }

    public int Priority { get; set; } = 100;

    public TemplateParser(string identifier, Action<T> OnParsed, int Priority = 100)
    {
        this.identifier = identifier;
        this.OnParsed = OnParsed;
        this.Priority = Priority;
    }

    public void Parse(string data)
    {
        T instance = JsonConvert.DeserializeObject<T>(data);
        OnParsed?.Invoke(instance);
    }
}