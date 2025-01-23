using System;
using System.Collections.Generic;
using MGSC;

namespace QM_WeaponImporter
{
    [Serializable]
    public class FactionTemplate
    {
        public List<FactionReward> FactionRewardList = new List<FactionReward>();
        public FactionTemplate()
        {
            // Empty! To Serialize!
        }
    }

    [Serializable]
    public class FactionReward
    {
        public string TableName => $"{FactionName}_{RewardType}";
        public string FactionName = string.Empty;
        public string RewardType = ContentDropTableType.rewardEquipment.ToString();
        public List<ContentDropRecord> contentRecords = new List<ContentDropRecord>();
    }

    public enum ContentDropTableType
    {
        rewardEquipment,
        rewardChips,
        rewardConsumables
    }
}