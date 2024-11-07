using System;
using UnityEngine;
using UnityEngine.Networking;

namespace QM_WeaponImporter.Services
{
    public class UnityFileAudioImporter : IAudioImporter<AudioClip>
    {
        public AudioClip Import(string path)
        {
            AudioType audioType = AnalyzeAudioType(path, out string fileName);
            if (audioType == AudioType.UNKNOWN)
            {
                Logger.WriteToLog($"Audio: {path}\nERROR: AudioType was not identified correctly.");
                return null;
            }
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType);
            request.SendWebRequest();
            do
            {
                Console.WriteLine($"Iterating in console waiting for the request!");
            }
            while (!request.isDone);

            if (request.isNetworkError)
            {
                Logger.WriteToLog($"Got a network error in AudioImporting!", Logger.LogType.Error);
            }
            else
            {
                AudioClip returnClip = DownloadHandlerAudioClip.GetContent(request);
                returnClip.name = fileName;
                return returnClip;
            }
            return null;
        }

        public AudioType AnalyzeAudioType(string path, out string fileName)
        {
            string[] pathSplit = path.Split('/');
            string completeFile = pathSplit[pathSplit.Length - 1];
            fileName = completeFile.Split('.')[0];
            string fileExtension = completeFile.Split('.')[1];
            Enum.TryParse(fileExtension.ToUpper(), out AudioType audioType);
            Logger.WriteToLog($"Audio Analysis: {path}\n{fileName}\n{fileExtension}\n{audioType}");
            return audioType;
        }
    }
}