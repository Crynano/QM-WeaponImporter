using System;
using System.Collections.Generic;
using MGSC;
using System.Linq;
using QM_WeaponImporter;

namespace QM_WeaponImporter.Commands;

[ConsoleCommand(new string[] { "reloadhook" })]
public class ReloadAfterConfigHook
{
    public static string Help(string command, bool verbose)
    {
        return "Forcefully executes in-game hook. Useful to reload game assemblies.\nUsage: reloadhook <hooktype>";
    }

    public string Execute(string[] tokens)
    {
        try
        {
            ModHookType hookType = (ModHookType)Enum.Parse(typeof(ModHookType), tokens[0]);
            UserModSystem.InvokeHook(Bootstrap._state, hookType);
            return "Executed correctly.";
        }
        catch (Exception e)
        {
            return $"<hooktype> parameter is invalid.";
        }
    }

    public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
    {
        string moodhookOption = tokens[0];
        return GetMatchingOptions(command, moodhookOption);
    }

    private static List<string> GetMatchingOptions(string command, string token)
    {
        var allHookOptions = new List<string>(Enum.GetNames(typeof(ModHookType)));
        // If the user just tabs, then we return all enums. Helpful.
        if (string.IsNullOrEmpty(token)) return allHookOptions.Select(x => $"{command} {x}").ToList(); ;

        return allHookOptions
            .Where(x => x.StartsWith(token, StringComparison.CurrentCultureIgnoreCase))
            .Select(x => $"{command} {x}")
            .ToList();
    }

    public static bool IsAvailable()
    {
        return true;
    }

    public static bool ShowInHelpAndAutocomplete()
    {
        return true;
    }
}