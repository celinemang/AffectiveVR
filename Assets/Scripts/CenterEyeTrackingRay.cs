using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CenterEyeTrackingRay : MonoBehaviour
{
    [SerializeField] private float rayDistance = 10.0f;
    [SerializeField] private float rayWidth = 0.01f;
    [SerializeField] private LayerMask layersToInclude;
    [SerializeField] private Color rayColorDefault = Color.yellow;
    [SerializeField] private Color rayColorHoverState = Color.red;

    private LineRenderer lineRenderer;
    private List<EyeInteractable> eyeInteractables = new();

    public Transform leftEye;
    public Transform rightEye;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.startColor = rayColorDefault;
        lineRenderer.endColor = rayColorDefault;
    }

    void FixedUpdate()
    {
        if (leftEye == null || rightEye == null) return;

        Vector3 gazePos = (leftEye.position + rightEye.position) * 0.5f;
        Vector3 gazeDir = ((leftEye.forward + rightEye.forward) * 0.5f).normalized;

        Ray ray = new Ray(gazePos, gazeDir);
        lineRenderer.SetPosition(0, gazePos);
        lineRenderer.SetPosition(1, gazePos + gazeDir * rayDistance);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance, layersToInclude))
        {
            UnSelect();
            lineRenderer.startColor = rayColorHoverState;
            lineRenderer.endColor = rayColorHoverState;

            var interactable = hit.transform.GetComponent<EyeInteractable>();
            if (interactable != null)
            {
                eyeInteractables.Add(interactable);
                interactable.IsHovered = true;
                TotalLog.AOIName = hit.collider.name;
            }
            else
            {
                TotalLog.AOIName = hit.collider.name;
            }
        }
        else
        {
            lineRenderer.startColor = rayColorDefault;
            lineRenderer.endColor = rayColorDefault;
            UnSelect(true);
            TotalLog.AOIName = "None";
        }

        // Log gaze info
        TotalLog.LeftEyePos = leftEye.position;
        TotalLog.RightEyePos = rightEye.position;
        TotalLog.LeftEyeEuler = leftEye.rotation.eulerAngles;
        TotalLog.RightEyeEuler = rightEye.rotation.eulerAngles;
        TotalLog.LogFrameEveryInterval();
    }

    void UnSelect(bool clear = false)
    {
        foreach (var interactable in eyeInteractables)
        {
            if (interactable != null) interactable.IsHovered = false;
        }
        if (clear) eyeInteractables.Clear();
    }
}