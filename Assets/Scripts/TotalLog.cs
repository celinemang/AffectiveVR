using System;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class TotalLog
{
    private static bool _initialized;
    private static string _folder;
    private static string _recordBaseName;
    private static bool _headerWritten;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    // Logged externally
    public static float GaugeValue = 0f;
    public static Transform HeadTransform;
    public static Vector3 LeftEyePos = Vector3.zero;
    public static Vector3 LeftEyeEuler = Vector3.zero;
    public static Vector3 RightEyePos = Vector3.zero;
    public static Vector3 RightEyeEuler = Vector3.zero;
    public static string AOIName = "";

    private static float _logTimer = 0f;
    private static readonly float _logInterval = 0.1f; // Log every 100ms

    public static void Init(string folderPath,string fileBase, string participantID)
    {
        if (_initialized) return;
        _initialized = true;

        _folder = folderPath;
        Directory.CreateDirectory(_folder);

        string core = string.IsNullOrEmpty(participantID)
            ? fileBase
            : $"{fileBase}_{participantID}_";

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _recordBaseName = core + timestamp;
        _headerWritten = false;

        WriteHeader();
    }


    private static void WriteHeader()
    {
        if (_headerWritten) return;

        string header =
            "Timestamp(ms)," +
            "GaugeValue," +
            "HeadPos.x,HeadPos.y,HeadPos.z," +
            "HeadForward.x,HeadForward.y,HeadForward.z," +
            "GazePos.x,GazePos.y,GazePos.z," +
            "GazeDirection.x,GazeDirection.y,GazeDirection.z," +
            "ObjLookingAt\n";

        RecordData.SaveData(_folder, _recordBaseName, header);
        _headerWritten = true;
    }

    public static void LogFrame()
    {
        long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        Vector3 headPos = HeadTransform ? HeadTransform.position : Vector3.zero;
        Vector3 headFwd = HeadTransform ? HeadTransform.forward : Vector3.forward;

        // Average eye position
        Vector3 gazePos = 0.5f * (LeftEyePos + RightEyePos);

        // Average direction from eye rotation (normalized)
        Vector3 gazeDir = ((Quaternion.Euler(LeftEyeEuler) * Vector3.forward) +
                           (Quaternion.Euler(RightEyeEuler) * Vector3.forward)).normalized;

        string line =
            timestamp.ToString(Inv) + "," +
            GaugeValue.ToString("0.###", Inv) + "," +
            headPos.x.ToString("0.###", Inv) + "," + headPos.y.ToString("0.###", Inv) + "," + headPos.z.ToString("0.###", Inv) + "," +
            headFwd.x.ToString("0.###", Inv) + "," + headFwd.y.ToString("0.###", Inv) + "," + headFwd.z.ToString("0.###", Inv) + "," +
            gazePos.x.ToString("0.###", Inv) + "," + gazePos.y.ToString("0.###", Inv) + "," + gazePos.z.ToString("0.###", Inv) + "," +
            gazeDir.x.ToString("0.###", Inv) + "," + gazeDir.y.ToString("0.###", Inv) + "," + gazeDir.z.ToString("0.###", Inv) + "," +
            AOIName + "\n";

        RecordData.SaveData(_folder, _recordBaseName, line);
    }

    public static void LogFrameEveryInterval()
    {
        _logTimer += Time.deltaTime;
        if (_logTimer >= _logInterval)
        {
            LogFrame();
            _logTimer = 0f;
        }
    }
}