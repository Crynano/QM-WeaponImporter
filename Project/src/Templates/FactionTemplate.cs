using System;
using System.Collections.Generic;

namespace QM_WeaponImporter
{
    [Serializable]
    public class FactionTemplate
    {
        public string FactionName;
        public List<Items> Items;
        public List<Items> TradeItems;
        public List<Items> RewardItems;
        public List<Items> Units;

        public FactionTemplate()
        {
            // Empty! To Serialize!
        }

        public static FactionTemplate GetExample()
        {
            FactionTemplate retFaction = new FactionTemplate()
            {
                FactionName = "AnCom",
                Items = new List<Items>()
            {
                new Items()
                {
                    difficulty = 1,
                    contentRecords = new List<MGSC.ContentDropRecord>()
                    {
                        new MGSC.ContentDropRecord()
                        {
                            ContentIds = new List<string>()
                            {
                                "weapon_id"
                            },
                            Points = 20,
                            RewardWeight = 4,
                            TechLevel = 1,
                            Weight = 4
                        }
                    }
                }
            },
                TradeItems = new List<Items>()
            {
                new Items()
                {
                    difficulty = 1,
                    contentRecords = new List<MGSC.ContentDropRecord>()
                    {
                        new MGSC.ContentDropRecord()
                        {
                            ContentIds = new List<string>()
                            {
                                "weapon_id"
                            },
                            Points = 20,
                            RewardWeight = 4,
                            TechLevel = 1,
                            Weight = 4
                        }
                    }
                }
            },
                RewardItems = new List<Items>()
            {
                new Items()
                {
                    difficulty = 1,
                    contentRecords = new List<MGSC.ContentDropRecord>()
                    {
                        new MGSC.ContentDropRecord()
                        {
                            ContentIds = new List<string>()
                            {
                                "weapon_id"
                            },
                            Points = 20,
                            RewardWeight = 4,
                            TechLevel = 1,
                            Weight = 4
                        }
                    }
                }
            },
                Units = new List<Items>()
                {
                    new Items()
                {
                    difficulty = 1,
                    contentRecords = new List<MGSC.ContentDropRecord>()
                    {
                        new MGSC.ContentDropRecord()
                        {
                            ContentIds = new List<string>()
                            {
                                "weapon_id"
                            },
                            Points = 20,
                            RewardWeight = 4,
                            TechLevel = 1,
                            Weight = 4
                        }
                    }
                }
                }
            };

            return retFaction;
        }
    }

    [Serializable]
    public class Items
    {
        public int difficulty;
        public List<MGSC.ContentDropRecord> contentRecords;
    }

    public enum ContentDropTableType
    {
        Units,
        Items,
        TradeItems,
        RewardItems
    }
}