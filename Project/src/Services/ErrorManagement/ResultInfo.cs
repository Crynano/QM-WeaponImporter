using System.Collections.Generic;
using UnityEngine;

namespace QM_WeaponImporter.ErrorManagement
{
    /// <summary>
    /// A class that will contain the process information of the import execution
    /// </summary>
    public class ResultInfo
    {
        public bool Result { get; set; }
        public string ResultMessage { get; set; }
        public double ExecutionTime { get; set; }
        public List<string> ErrorMessages { get; set; }
        public List<string> ContentList { get; set; }

        public ResultInfo()
        {
            Result = true;
            ResultMessage = string.Empty;
            ExecutionTime = 0f;
            ErrorMessages = new List<string>();
            ContentList = new List<string>();
        }
        
        public ResultInfo(bool result, string resultMessage, float executionTime, List<string> errorMessages, List<string> contentList)
        {
            Result = result;
            ResultMessage = resultMessage;
            ExecutionTime = executionTime;
            ErrorMessages = errorMessages;
            ContentList = contentList;
        }

        /// <summary>
        /// Returns a stringified version of the ResultInfo, ready to log.
        /// </summary>
        /// <returns>A formatted string</returns>
        public string Print()
        {
            string msg = "";

            msg += $"\tExecution Time: {this.ExecutionTime}\n";
            msg += $"\tResult: {this.Result}\n";
            msg += $"\tResult Message: {this.ResultMessage}\n";
            msg += $"\tError Messages:\n";
            foreach (var errorMessage in this.ErrorMessages)
            {
                msg += $"\t\t- {errorMessage}\n";
            }
            
            msg += $"\tContent List:\n";
            foreach (var content in this.ContentList)
            {
                msg += $"\t\t- {content}\n";
            }
            
            return msg;
        }

        public void AddItem(string itemId)
        {
            ContentList.Add(itemId);
        }
    }
}