using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace QM_WeaponImporter;
internal static class Logger
{
    internal static string AssemblyFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private const string LogFileName = "QM_WeaponsImporter_Log.txt";
    private const string LogSignature = "QM_WeaponImporter";
    private static string Log = "";

    private static string GetPath => Path.Combine(AssemblyFolder, LogFileName);
    public static void WriteToLog(string message, LogType logType = LogType.Info, bool writeToUnity = true)
    {
        if (Log.Equals(string.Empty))
        {
            Log = $"[{DateTime.Now.ToString()}][{LogSignature}] NEW LOG. \n";
        }

        string beautifiedMessage = $"[{DateTime.Now.ToString()}][{LogSignature}][{logType.ToString().ToUpper()}] {message}";

        // We can duplicate logs for Bepinex or custom console users.
        if (writeToUnity) Debug.Log(beautifiedMessage);
        Log += beautifiedMessage + "\n";
    }

    public static void Flush()
    {
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
