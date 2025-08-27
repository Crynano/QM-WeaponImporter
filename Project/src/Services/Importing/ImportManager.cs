using System.Collections.Generic;
using MGSC;
using QM_WeaponImporter.ErrorManagement;
using QM_WeaponImporter.Templates;
using UnityEngine;

namespace QM_WeaponImporter
{
    internal static class ImportManager
    {
        private static DataParser ParserInstance = new DataParser();
        
        private static List<ResultInfo> LoadedMods = new List<ResultInfo>();
        
        /// <summary>
        /// Uses the default ConfigPath
        /// </summary>
        /// <param name="configPath">The path where Global config json is stored.</param>
        /// <param name="useDefault">Use default ConfigTemplate or the one provided by the modder</param>
        /// <returns></returns>
        internal static ResultInfo ImportConfig(string configPath, bool useDefault = false)
        {
            ResultInfo result = new ResultInfo();
            Logger.LinkResult(result);
            
            if (!useDefault && string.IsNullOrEmpty(configPath))
            {
                Logger.LogError($"Null config path at ImportConfig entry point.");
                result.Result = false;
                return result;
            }
            
            ParserInstance.ImportConfig(useDefault ? ConfigTemplate.GetDefault() : Importer.GetGlobalConfig(configPath), configPath, ref result);
            
            Logger.ClearResultInfo();
            Logger.LogInfo(result.Print());
            Logger.Flush();
            
            LoadedMods.Add(result);
            
            return result; 
        }
        
        internal static ResultInfo ImportConfig(IModContext modContext, bool useDefault = false)
        {
            ResultInfo result = new ResultInfo();
            Logger.LinkResult(result);
            ParserInstance.ImportConfig(useDefault ? ConfigTemplate.GetDefault() : Importer.GetGlobalConfig(modContext.ModContentPath), modContext.ModContentPath, ref result);
            
            Logger.ClearResultInfo();
            Logger.LogInfo(result.Print());
            Logger.Flush();
            
            LoadedMods.Add(result);
            
            return result; 
        }
        
        internal static ResultInfo ImportConfig(string configPath, ConfigTemplate config)
        {
            ResultInfo? result = new ResultInfo();
            Logger.LinkResult(result);
            ParserInstance.ImportConfig(config, configPath, ref result);
            
            Logger.ClearResultInfo();
            Logger.LogInfo(result.Print());
            Logger.Flush();
            
            LoadedMods.Add(result);
            
            return result; 
        }
    }
}