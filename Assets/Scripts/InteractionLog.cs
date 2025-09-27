using System;
using System.IO;
using UnityEngine;

public class InteractionLog : MonoBehaviour
{
    [SerializeField] DataManager dataManager;

    private string folderPath;
    private string fileName;

    private float popupOpenStartTime = -1f;
    private string currentIcon = "";

    void Start()
    {
        folderPath = dataManager.folderPath;
        string participantID = dataManager.participantID;

        // Unique timestamped filename per run
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        fileName = $"InteractionLog_{participantID}_{timestamp}";

        // Header line
        string header = "Timestamp,Target,SlideNumber,Type,Source\n";
        RecordData.SaveData(folderPath, fileName, header);

        Debug.Log($"[InteractionLog] Logging to: {Path.Combine(folderPath, fileName)}.csv");
    }

    public void LogIconOpened(string iconName, int slideNumber, string source)
    {
        popupOpenStartTime = Time.time;
        currentIcon = iconName;

        string timestamp = DateTime.Now.ToString("O");
        string line = $"{timestamp},{iconName},{slideNumber},OPEN,{source}\n";
        RecordData.SaveData(folderPath, fileName, line);
    }

    public void LogIconClosed(int slideNumber, string source)
    {
        if (popupOpenStartTime < 0f) return;

        string timestamp = DateTime.Now.ToString("O");
        string line = $"{timestamp},{currentIcon},{slideNumber},CLOSE,{source}\n";
        RecordData.SaveData(folderPath, fileName, line);

        popupOpenStartTime = -1f;
        currentIcon = "";
    }
}