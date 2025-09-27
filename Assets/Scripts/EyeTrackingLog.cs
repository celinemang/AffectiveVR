using UnityEngine;

/// Logs gaze each frame into TotalLog.* (no file writing here)
/// Attach this to a GameObject and assign eye Transforms and camera.
public class EyeTrackingLog : MonoBehaviour
{
    [Header("XR Camera (CenterEyeAnchor)")]
    public Camera xrCamera; // eye camera

    [Header("Eye Tracking References")]
    public Transform leftEye;
    public Transform rightEye;

    [Header("Raycast Settings")]
    public LayerMask hudMask;
    public LayerMask worldMask;
    public float rayRadius = 0.01f;
    public float maxRayDistance = 50f;

    void Update()
    {
        if (leftEye == null || rightEye == null || xrCamera == null) return;

        // Get gaze origin and direction
        Vector3 gazeOrigin = 0.5f * (leftEye.position + rightEye.position);
        Vector3 gazeDirection = (leftEye.forward + rightEye.forward).normalized;

        // Save to TotalLog
        TotalLog.LeftEyePos = leftEye.position;
        TotalLog.RightEyePos = rightEye.position;
        //TotalLog.GazePos = gazeOrigin;
        //TotalLog.GazeDirection = gazeDirection;

        RaycastHit hit;
        bool hitFound = false;

        // Try HUD layer first
        if (Physics.SphereCast(gazeOrigin, rayRadius, gazeDirection, out hit, maxRayDistance, hudMask))
        {
            TotalLog.AOIName = hit.collider.name;
            hitFound = true;
        }
        // Try world layer second
        // else if (Physics.SphereCast(gazeOrigin, rayRadius, gazeDirection, out hit, maxRayDistance, worldMask))
        // {
        //     TotalLog.AOIName = hit.collider.name;
        //     hitFound = true;
        // }
        else
        {
            TotalLog.AOIName = "None";
        }

        
        // Debug info
        Debug.Log($"[Gaze Debug]");
        Debug.Log($"LeftEye  Pos: {leftEye.position:F3}  | Forward: {leftEye.forward:F3}");
        Debug.Log($"RightEye Pos: {rightEye.position:F3}  | Forward: {rightEye.forward:F3}");
        Debug.Log($"GazeOrigin: {gazeOrigin:F3} | GazeDir: {gazeDirection:F3}");

        if (hitFound)
        {
            Debug.Log($"HIT: {hit.collider.name} | HitPos: {hit.point:F3} | Distance: {hit.distance:F2} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
        }
        else
        {
            Debug.Log("MISS: No object hit.");
        }
    }
}