﻿using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace QM_WeaponImporter;
internal static class Logger
{
    private enum LogType
    {
        Info,
        Warning,
        Error,
        Debug
    }

    private static string LogFileName => $"Log_{DateTime.Now.ToString(@"dd_MM_yyyy")}.log";
    private static string LogSignature { get; set; } = "QM_WeaponImporter";

    private static string LogStart => $"[{DateTime.Now.ToString()}][{LogSignature}][START] ----------------- Log Start -----------------\nGame Version Report: {Application.version}\nWeapon Importer Version Report: {Assembly.GetExecutingAssembly().GetName().Version}\n";
    private static string LogEnd => $"[{DateTime.Now.ToString()}][{LogSignature}][#END#] |---------------- Log #End# ----------------|\n";

    private static string Context = "";
    private static string Log = "";

    private static string LogPath = Path.Combine(ConfigManager.AllModsConfigFolder, LogFileName);

    public static void SetConfig(string modName)
    {
        LogSignature = modName;
        LogPath = Path.Combine(ConfigManager.AllModsConfigFolder, modName, "Logs", LogFileName);
    }

    public static void LogDebug(string message)
    {
        // Only will log if debug mode.
#if DEBUG
        WriteToLog(message, LogType.Debug);
#endif
    }

    public static void LogInfo(string message)
    {
        //if (!ConfigManager.CurrentModConfig.ShowInfo) return;
        WriteToLog(message, LogType.Info);
    }

    public static void LogWarning(string message)
    {
        //if (!ConfigManager.CurrentModConfig.ShowWarnings) return;
        WriteToLog(message, LogType.Warning);
    }

    public static void LogError(string message)
    {
        //if (!ConfigManager.CurrentModConfig.ShowErrors) return;
        WriteToLog(message, LogType.Error, true);
    }

    public static void SetContext(string context)
    {
        Context = context;
    }

    public static void ClearContext()
    {
        Context = "";
    }

    private static void WriteToLog(string message, LogType logType, bool writeToUnity = false)
    {
        string beautifiedMessage =
            $"[{DateTime.Now.ToString()}][{LogSignature}][{logType.ToString().ToUpper()}]" +
            (string.IsNullOrEmpty(Context) ? "" : $"[{Context}]") +
            $": {message}";

        // We can duplicate logs for Bepinex or custom console users.
        if (writeToUnity) Debug.Log(beautifiedMessage);
        Log += $"{beautifiedMessage}\n";
    }

    public static void Flush()
    {
        string finalLog = LogStart + Log + LogEnd;
        File.WriteAllText(LogPath, finalLog);
        ResetLog();
    }

    public static void FlushAdditive()
    {
        string existingLog = "";
        if (File.Exists(LogPath))
        {
            existingLog = File.ReadAllText(LogPath);
        }
        string finalLog = LogStart + Log + LogEnd;
        existingLog += finalLog;
        File.WriteAllText(LogPath, existingLog);
        ResetLog();
    }

    private static void ResetLog()
    {
        Log = "";
    }
}
