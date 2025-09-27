using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // for Slider

public class EmotionLog : MonoBehaviour
{
    [SerializeField] DataManager dataManager; 
    [SerializeField] Slider gaugeSlider;       

    string Path;
    string FileName;

    double logTime;
    float lastValue;

    // Start is called before the first frame update
    void Start()
    {
        // Setup file path + name from DataManager
        Path = dataManager.folderPath;
        FileName = dataManager.fileName;

        // Write CSV header
        RecordData.SaveData(Path, "EmotionLog",
            "LogTime(ms)" + ","
            + "LogTime(DateTime)" + ","
            + "GaugeValue"
            + '\n');
    }

    // Update is called once per frame
    void Update()
    {
        if (gaugeSlider == null) return;

        float currentValue = gaugeSlider.value;

        // Only log when the value changes
        if (!Mathf.Approximately(currentValue, lastValue))
        {
            logTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            RecordData.SaveData(Path, "EmotionLog",
                                logTime.ToString() + ","
                                + DateTime.Now.ToString() + ","
                                + currentValue.ToString("F2")
                                + '\n');

            lastValue = currentValue;
        }
    }
}
