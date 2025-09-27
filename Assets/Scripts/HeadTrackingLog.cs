using UnityEngine;

public class HeadTrackingLog : MonoBehaviour
{
    public Transform head;

    void Update()
    {
        if (head != null)
        {
            TotalLog.HeadTransform = head;
        }
    }
}