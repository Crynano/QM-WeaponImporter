using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace QM_WeaponImporter;
internal static class Logger
{
    private static string LogFileName => "Log.txt";
    private static string LogSignature = "QM_WeaponImporter";
    private static string Log = $"[{DateTime.Now.ToString()}][{LogSignature}] ----------- Log Start -----------\n";

    private static string GetPath => Path.Combine(ConfigManager.rootFolder, LogFileName);
    public static void WriteToLog(string message, LogType logType = LogType.Info, bool writeToUnity = true)
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

    public enum LogType
    {
        Info,
        Warning,
        Error
    }
}
