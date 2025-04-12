using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MGSC;

namespace QM_WeaponImporter;

[ConsoleCommand(new string[] { "give" })]
public class GiveItemCommand
{
    [Inject(false)]
    private readonly MagnumCargo _magnumCargo;

    [Inject(false, AllowNull = true)]
    private readonly Creatures _creatures;

    [Inject(false, AllowNull = true)]
    private readonly ItemsOnFloor _itemsOnFloor;

    public static string Help(string command, bool verbose)
    {
        return "Spawn X items on the floor or in ship cargo. Default amount is 1.\nSyntax: give <itemId> <itemAmount>.\nPress TAB to autocomplete.";
    }

    public string Execute(string[] tokens)
    {
        try
        {
            int amountOfItems = 1;
            if (tokens.Length > 1)
            {
                // If user wants more than one, specify
                if (int.TryParse(tokens[1], out int newAmount))
                {
                    if (newAmount < 1) return "ERROR: Please introduce a valid amount of items.";
                    amountOfItems = newAmount;
                }
            }
            for (int i = 0; i < amountOfItems; i++)
            {
                BasePickupItem basePickupItem = SingletonMonoBehaviour<ItemFactory>.Instance.CreateForInventory(tokens[0]);
                if (basePickupItem.Is<DatadiskRecord>())
                {
                    DatadiskComponent datadiskComponent = basePickupItem.Comp<DatadiskComponent>();
                    DatadiskRecord datadiskRecord = basePickupItem.Record<DatadiskRecord>();
                    datadiskComponent.SetUnlockId(datadiskRecord.UnlockIds[UnityEngine.Random.Range(0, datadiskRecord.UnlockIds.Count)]);
                }
                if (SingletonMonoBehaviour<DungeonGameMode>.Instance == null)
                {
                    _magnumCargo.ShipCargo[0].AddItemAndReshuffleOptional(basePickupItem);
                }
                else
                {
                    Player player = _creatures.Player;
                    ItemOnFloorSystem.SpawnItem(_itemsOnFloor, basePickupItem, player.CreatureData.Position);
                }
            }
            return amountOfItems == 1 ? "Added item!" : $"Added {amountOfItems} items!" ;
        }
        catch (NullReferenceException exception)
        {
            string msg = $"ERROR: Item with ID \"{tokens[0]}\" not found";
            Debug.Log(msg);
            return msg;
        }
    }

    public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
    {
        string enteredText = ((tokens.Length != 0) ? tokens[0] : "");
        //UI.Get<DevConsole>().Daemon.CommandList.Where((string name) => name.StartsWith(enteredText)).ToList();
        List<string> list = Data.Items.Ids.Where((string name) => name.StartsWith(enteredText)).ToList();
        if (list == null || list.Count == 0)
        {
            return null;
        }
        List<string> list2 = new List<string>();
        foreach (string item in list)
        {
            // Also add its localization name.
            string locName = Localization.Get("item." + item + ".name").FirstLetterToUpperCase() ?? "missing name";
            list2.Add(command + " " + item + $" ({locName})");
        }
        return list2;
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
