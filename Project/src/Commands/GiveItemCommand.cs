using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MGSC;

[ConsoleCommand(new string[] { "give" })]
public class GiveItemCommand
{
    [Inject(false)]
    private readonly MagnumCargo _magnumCargo;

    [Inject(false, AllowNull = true)]
    private readonly MapGrid _mapGrid;

    [Inject(false, AllowNull = true)]
    private readonly Creatures _creatures;

    [Inject(false, AllowNull = true)]
    private readonly ItemsOnFloor _itemsOnFloor;

    public static string Help(string command, bool verbose)
    {
        return "Spawn item on the floor or in ship cargo. Syntax: give <itemId>. Press TAB to autocomplete.";
    }

    public string Execute(string[] tokens)
    {
        try
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
                return "Added item";
            }
            Player player = _creatures.Player;
            ItemOnFloorSystem.SpawnItem(_itemsOnFloor, basePickupItem, player.CreatureData.Position);
            return "Added item";
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
            list2.Add(command + " " + item);
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
