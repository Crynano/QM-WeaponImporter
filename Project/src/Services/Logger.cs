using System;
using System.IO;
using UnityEngine;

namespace QM_WeaponImporter;
internal static class Logger
{
    private static string LogFileName => "Log.txt";
    private static string LogSignature = "QM_WeaponImporter";
    private static string Log = $"[{DateTime.Now.ToString()}][{LogSignature}] ----------- Log Start -----------\n";

    private static string GetPath => Path.Combine(ConfigManager.rootFolder, LogFileName);

    public static void LogInfo(string message)
    {
        WriteToLog(message, LogType.Info);
    }

    public static void LogWarning(string message)
    {
        WriteToLog(message, LogType.Warning);
    }

    public static void LogError(string message)
    {
        WriteToLog(message, LogType.Error);
    }

    private static void WriteToLog(string message, LogType logType = LogType.Info, bool writeToUnity = true)
    {
        string beautifiedMessage = $"[{DateTime.Now.ToString()}][{LogSignature}][{logType.ToString().ToUpper()}] {message}";

        // We can duplicate logs for Bepinex or custom console users.
        if (writeToUnity) Debug.Log(beautifiedMessage);
        Log += beautifiedMessage + "\n";
    }

    public static void Flush()
    {
        WriteToLog($"[{DateTime.Now.ToString()}][{LogSignature}] ----------- Log End -----------");
        File.WriteAllText(GetPath, Log);
        Log = string.Empty;
    }

    public static void FlushAdditive()
    {
        string existingLog = "";
        if (File.Exists(GetPath))
        {
            existingLog = File.ReadAllText(GetPath);
        }
        existingLog += Log;
        File.WriteAllText(GetPath, existingLog);
        Log = string.Empty;
    }

    private enum LogType
    {
        Info,
        Warning,
        Error
    }
}
