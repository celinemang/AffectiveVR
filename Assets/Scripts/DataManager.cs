using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public string participantID;
    public string folderPath;
    public int taskNum;
    public string fileName;

    private bool isLogging = false;

    private void Awake()
    {
        folderPath = Application.dataPath + "/Logs/";
        DirectoryInfo folder = Directory.CreateDirectory(folderPath);

        if (string.IsNullOrEmpty(fileName))
            fileName = SceneManager.GetActiveScene().name + "_" + participantID + "_Trial" + taskNum;
    }

    void Update()
    {
        // Button B to start logging
        if (!isLogging && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)|| Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("[DataManager] ▶ B button pressed — Logging started");
            TotalLog.Init(folderPath, fileName, participantID);
            isLogging = true;
        }

        // log every frame at fixed interval
        if (isLogging)
        {
            TotalLog.LogFrameEveryInterval();
        }
    }
}