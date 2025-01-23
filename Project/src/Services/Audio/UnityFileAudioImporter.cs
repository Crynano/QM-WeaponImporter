using System;
using UnityEngine;
using UnityEngine.Networking;

namespace QM_WeaponImporter.Services
{
    internal class UnityFileAudioImporter : IAudioImporter<AudioClip>
    {
        public AudioClip Import(string path)
        {
            AudioType audioType = AnalyzeAudioType(path, out string fileName);
            if (audioType == AudioType.UNKNOWN)
            {
                Logger.LogInfo($"Audio: {path}\nERROR: AudioType was not identified correctly.");
                return null;
            }
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType);
            request.SendWebRequest();
            do
            {
                //Console.WriteLine($"Iterating in console waiting for the request!");
            }
            while (!request.isDone);

            if (request.isNetworkError)
            {
                Logger.LogError($"Got a network error in AudioImporting!");
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
            //Logger.LogInfo($"Audio Analysis: {path} from {fileName} with {fileExtension}\n{audioType}");
            return audioType;
        }
    }
}