using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayHitLogger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Transform rightHand;
    Ray ray;

    void Start()
    {
        rightHand = GameObject.Find("OVRVameraRig/TrackingSpace/RightHandAnchor").transform;
        ray = new Ray(rightHand.position, rightHand.forward);

        RaycastHit[] hits = Physics.RaycastAll(ray, 10f);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Ray is now over the icon: " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Ray exited from: " + gameObject.name);
    }
}