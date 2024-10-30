using System;
using System.Collections.Generic;
using MGSC;

namespace QM_WeaponImporter
{
    [Serializable]
    public class FactionTemplate
    {
        public List<FactionReward> Items = new List<FactionReward>();
        //public List<Items> TradeItems;
        //public List<Items> RewardItems;
        //public List<Items> Units;

        public FactionTemplate()
        {
            // Empty! To Serialize!
        }

        public static FactionTemplate GetExample()
        {
            try
            {
                FactionTemplate retFaction = new FactionTemplate()
                {
                    Items =
                    [
                        new FactionReward()
                        {
                            factionTags = ["AnCom_1", "SBN_3"],
                            contentRecords =
                            [
                                new ContentDropRecord()
                                {
                                    ContentIds = ["ItemID"], Points = 100, TechLevel = 1, Weight = 10
                                }
                            ]
                        }

                    ]
                };

                return retFaction;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Exception when generating the Example for FactionTemplate.\n{ex.Message}");
            }

            return null;
        }
    }

    [Serializable]
    public class FactionReward
    {
        // This faction tags define all components of where an item goes.
        // Want it to go to 7 different factions at level 1?
        // use AnCom_1, {FactionName}_{TechLevel}
        // Then we split it and add it to every table.
        public List<string> factionTags = new List<string>();
        public List<MGSC.ContentDropRecord> contentRecords = new List<MGSC.ContentDropRecord>();
    }

    public enum ContentDropTableType
    {
        Units,
        Items,
        RewardEquipment,
        RewardChips,
        RewardConsumables
    }
}