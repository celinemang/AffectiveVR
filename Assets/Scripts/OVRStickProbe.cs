using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRStickProbe : MonoBehaviour
{
    void Update()
    {
        var l = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        var r = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.RTouch);
        var active = OVRInput.GetActiveController();
        Debug.Log($"OUT OF IF STATMENT 1");
        // Keep logs sparse
        if (l.sqrMagnitude > 0.001f || r.sqrMagnitude > 0.001f)
            Debug.Log($"L:{l}  R:{r}  Active:{active}  Focus:{OVRManager.hasInputFocus}");
        Debug.Log($"OUT OF IF STATMENT 2");
    }
}
