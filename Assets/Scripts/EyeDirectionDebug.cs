using UnityEngine;
using Oculus.Interaction.Input;


public class EyeDirectionDebug : MonoBehaviour
{
    public OVREyeGaze eyeGaze;
    void Update()
    {
        if (eyeGaze != null && eyeGaze.EyeTrackingEnabled)
        {
            Vector3 eyeOrigin = eyeGaze.transform.position;
            Vector3 eyeForward = eyeGaze.transform.forward;
            Debug.DrawRay(eyeOrigin, eyeForward * 5f, Color.cyan);
            Debug.Log($"Gaze Position: {eyeOrigin}, Gaze Direction: {eyeForward}");
        }
        else
        {
            Debug.Log("OVREyeGaze not available or eye tracking not enabled.");
        }
    }
}