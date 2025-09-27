using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayDebug : MonoBehaviour
{
    public Transform targetTransform; // Assign this to your controller in the Inspector
    public float maxDistance = 10f;

    void Update()
    {
        Transform rayOrigin = targetTransform != null ? targetTransform : transform;
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name);
        }
        else
        {
           // Debug.Log("Ray is not hitting anything");
        }
    }
}