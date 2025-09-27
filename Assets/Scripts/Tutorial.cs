using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Tutorial: MonoBehaviour
{
    [System.Serializable]
    public class Slide
    {
        public GameObject slideObject;       // One slide
        public GameObject[] highlights;      // Highlight cubes
        public GameObject[] popups;          // Associated popups
    }

    public Slide tutorialSlide;


    private Color baseColor = new Color(1f, 1f, 0f, 0.2f);    // transparent yellow
    private Color hoverColor = new Color(1f, 1f, 0f, 0.6f);   // hover
    private Color clickColor = new Color(1f, 0.5f, 0f, 0.8f); // clicked

    public Transform rayOriginOverride; // Optional
    public Transform SourceRay;         // Controller or gaze pointer

    private int currentPopupIndex = -1;

    void Start()
    {
        // Show the tutorial slide
        if (tutorialSlide.slideObject != null)
            tutorialSlide.slideObject.SetActive(true);

        // Hide all popups at the start
        foreach (var popup in tutorialSlide.popups)
            if (popup != null) popup.SetActive(false);
    }

    void Update()
{
    if (rayOriginOverride != null && SourceRay != null)
    {
        rayOriginOverride.position = SourceRay.position;
        rayOriginOverride.rotation = SourceRay.rotation;
    }

    // Hide everything on B press
    if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
    {
        HideAllPopups();

        foreach (var highlight in tutorialSlide.highlights)
        {
            if (highlight != null)
                highlight.SetActive(false);
        }
        if(tutorialSlide.slideObject != null)
            tutorialSlide.slideObject.SetActive(false);

        Debug.Log("B button pressed – all highlights and popups hidden.");
        return; // skip interaction checks if we just hid everything
    }

    CheckRayInteraction();
}

    void CheckRayInteraction()
    {
        Transform rayOrigin = rayOriginOverride != null ? rayOriginOverride : SourceRay;
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            for (int i = 0; i < tutorialSlide.highlights.Length; i++)
            {
                var highlight = tutorialSlide.highlights[i];
                if (highlight == null) continue;

                Renderer rend = highlight.GetComponent<Renderer>();

                if (hit.collider.gameObject == highlight)
                {
                    if (rend) rend.material.color = hoverColor;

                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                    {
                        if (rend) rend.material.color = clickColor;
                        ShowPopup(i);
                    }
                }
                else
                {
                    if (rend) rend.material.color = baseColor;
                }
            }
        }
        else
        {
            // Ray hits nothing – reset all highlight colors
            foreach (var highlight in tutorialSlide.highlights)
            {
                if (highlight == null) continue;
                Renderer rend = highlight.GetComponent<Renderer>();
                if (rend) rend.material.color = baseColor;
            }
        }
    }

    void ShowPopup(int index)
    {
        var popups = tutorialSlide.popups;

        if (index < 0 || index >= popups.Length) return;

        if (currentPopupIndex == index)
        {
            popups[index].SetActive(false);
            currentPopupIndex = -1;
        }
        else
        {
            HideAllPopups();
            if (popups[index] != null)
            {
                popups[index].SetActive(true);
                var vp = popups[index].GetComponent<VideoPlayer>();
                if (vp != null)
                {
                    vp.Stop();
                    vp.Play();
                }
                currentPopupIndex = index;
            }
        }
    }

    void HideAllPopups()
    {
        foreach (var popup in tutorialSlide.popups)
        {
            if (popup != null) popup.SetActive(false);
        }
        currentPopupIndex = -1;
    }
}