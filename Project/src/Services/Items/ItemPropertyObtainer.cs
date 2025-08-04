using System;
using System.Linq;
using System.Reflection;
using MGSC;
using UnityEngine;

namespace QM_WeaponImporter.Services.Items
{
    public static class ItemPropertyObtainer
    {
        public static object GetPropertyFromItem<T>(string id, string propertyName) where T : ScriptableObject
        {
            T descriptor = GetExistingItem<BasePickupItemRecord, T>(id, MGSC.Data.Items);
            if (descriptor == null)
            {
                Logger.LogWarning($"Couldn't get {propertyName} from {id}. Item does not exist in-game.");
                return null;
            }

            Logger.LogDebug($"Getting the \"{propertyName}\" from \"{id}\" from \"{descriptor.GetType()}\"");
            Type type = descriptor.GetType();
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.FlattenHierarchy;
            var a = type.GetProperties(bindingAttr);
            if (a.ToList().Find(x => x.Name.Equals(propertyName)) == null)
            {
                Logger.LogError($"Property \"{propertyName}\" is not found!");
                return null;
            }

            var b = a.First(x => x.Name.Equals(propertyName));
            var retVal = b.GetValue(descriptor, null);
            Logger.LogDebug($"Successfully obtained the \"{propertyName}\" from \"{id}\"");
            return retVal;
        }
        
        public static object GetPropertyFromList<T, T2>(string id, string propertyName, ConfigRecordCollection<T> list)
            where T : ConfigTableRecord where T2 : ScriptableObject
        {
            T2 descriptor = GetExistingItem<T, T2>(id, list);
            if (descriptor == null)
            {
                Logger.LogError($"Couldn't get property from {id}. Item does not exist in-game.");
                return null;
            }

            Logger.LogDebug($"Getting the \"{propertyName}\" from \"{id}\" from \"{descriptor.GetType()}\"");
            Type type = descriptor.GetType();
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase;
            var a = type.GetProperties(bindingAttr);
            foreach (var item in a)
            {
                Logger.LogDebug($"Listing property {item} for {id}");
            }

            if (a.ToList().Find(x => x.Name.Equals(propertyName)) == null)
            {
                Logger.LogError($"Property \"{propertyName}\" is not found!");
                return null;
            }

            var b = a.First(x => x.Name.Equals(propertyName));
            var retVal = b.GetValue(descriptor, null);
            Logger.LogDebug($"Successfully obtained the \"{propertyName}\" from \"{id}\"");
            return retVal;
        }
        
        public static T2 GetExistingItem<T, T2>(string id, ConfigRecordCollection<T> list)
            where T : ConfigTableRecord where T2 : ScriptableObject
        {
            if (!string.IsNullOrEmpty(id) && list._records.ContainsKey(id))
            {
                Logger.LogDebug($"GetExistingItem({id}) result? {list._records.ContainsKey(id)}");
                var returnVal = ((T)list.GetRecord(id)).ContentDescriptor as T2;
                if (returnVal != null)
                    Logger.LogDebug($"Type of ReturnVal {returnVal.GetType()}");
                return returnVal;
                //T2 returnResult = record.ContentDescriptor as T2;
                //return returnResult;
            }

            return null;
        }
    }
}