using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.Audio;

public class AdaptiveLectureUI : MonoBehaviour
{
    // Reference to the InteractionLog component or class
    public InteractionLog interactionLog;

    [System.Serializable]
    public class Slide
    {
        public GameObject slideObject; // Reference to slide GameObject (e.g., slide_1, slide_2)
        public GameObject[] icons;
        public GameObject[] popups;
    }

    [Header("Slides")]
    public List<Slide> slides = new List<Slide>();
    public int currentSlideIndex = 0;

    [Header("Adaptation Control")]
    public bool adaptationActive = false;

    [Header("Boredom Tracking")]
    public Slider boredomSlider;
    [SerializeField] private float boredomLevel = 0f;
    public float boredomThreshold = 75f;
    public float dwellTime = 15f;

    // 🔹 Highlight colors
    private Color baseColor = new Color(1f, 1f, 0f, 0.2f);  // transparent yellow
    private Color hoverColor = new Color(1f, 1f, 0f, 0.6f);  // brighter yellow
    private Color clickColor = new Color(1f, 0.5f, 0f, 0.8f); // orange

    private float boredomTimer = 0f;
    private bool iconsVisible = false;
    private int currentPopupIndex = -1;

    [Header("Auto Slide Control")]
    public List<float> autoSlideTimes = new List<float>();
    private int nextAutoSlideIndex = 0;
    private float slideTimer = 0f;
    private float startTime;

    [Header("Audio")]
    public AudioSource narrationAudio;

    public Transform rayOriginOverride;
    public Transform SourceRay;

    void Start()
    {
        ShowOnlyCurrentSlide();

    }

    void Update()
    {
        if (rayOriginOverride != null && SourceRay != null)
        {
            rayOriginOverride.position = SourceRay.position;
            rayOriginOverride.rotation = SourceRay.rotation;
        }
        if (boredomSlider != null)
            boredomLevel = boredomSlider.value;

        if (Input.GetKeyDown(KeyCode.S) || OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            adaptationActive = true;
            boredomTimer = 0f;
            Debug.Log("Adaptation Started");
            if (narrationAudio != null && !narrationAudio.isPlaying)
            {
                narrationAudio.Play();
                startTime = Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            adaptationActive = false;
            if (interactionLog)
            {
                interactionLog.LogIconClosed(currentSlideIndex + 1, "SYSTEM");
            }
            HideAllIcons();
            HideAllPopups();
            Debug.Log("Adaptation Stopped");
        }

        // Slide navigation (example: N for next, P for previous)
        if (Input.GetKeyDown(KeyCode.N))
            ChangeSlide(currentSlideIndex + 1);
        if (Input.GetKeyDown(KeyCode.P))
            ChangeSlide(currentSlideIndex - 1);

        if (!adaptationActive) return;

        // boredom → show/hide icons
        if (boredomLevel >= boredomThreshold)
        {
            boredomTimer += Time.deltaTime;
            if (!iconsVisible && boredomTimer >= dwellTime)
                ShowAllIcons();
        }
        else
        {
            boredomTimer += Time.deltaTime;
            // if (iconsVisible && boredomTimer >= dwellTime)
            // {
            //     if (interactionLog)
            //     {
            //         interactionLog.LogIconClosed(currentSlideIndex + 1, "SYSTEM");
            //     }
            //     HideAllIcons();
            //     HideAllPopups();
            // }
        }

        if ((boredomLevel >= boredomThreshold && iconsVisible) ||
            (boredomLevel < boredomThreshold && !iconsVisible))
        {
            boredomTimer = 0f;
        }

        CheckRayInteraction();
        if (nextAutoSlideIndex < autoSlideTimes.Count)
        {
            slideTimer = Time.time - startTime;
            if (slideTimer >= autoSlideTimes[nextAutoSlideIndex])
            {
                ChangeSlide(currentSlideIndex + 1);
                nextAutoSlideIndex++;
            }
        }
    }

    void ChangeSlide(int newIndex)
    {
        if (newIndex < 0 || newIndex >= slides.Count) return;
        if (interactionLog && iconsVisible)
        {
            interactionLog.LogIconClosed(currentSlideIndex + 1, "SYSTEM");
        }
        HideAllIcons();
        HideAllPopups();
        currentSlideIndex = newIndex;
        ShowOnlyCurrentSlide();

        //start dwell timer.
        boredomTimer = 0.0f;

        // Show icons on new slide if adaptation + boredom threshold met
        if (adaptationActive && boredomLevel >= boredomThreshold)
        {
            //ShowAllIcons(); 
            
        }
        Debug.Log("Changed to slide: " + currentSlideIndex);
    }

    void ShowOnlyCurrentSlide()
    {
        for (int i = 0; i < slides.Count; i++)
        {
            if (slides[i].slideObject != null)
                slides[i].slideObject.SetActive(i == currentSlideIndex);
        }
    }
    

    void ShowAllIcons()
    {
        foreach (var icon in slides[currentSlideIndex].icons)
            if (icon != null) icon.SetActive(true);
        iconsVisible = true;
    }

    void HideAllIcons()
    {
        foreach (var icon in slides[currentSlideIndex].icons)
            if (icon != null) icon.SetActive(false);
        iconsVisible = false;
    }

    
    public void ShowPopup(int index)
    {
        var popups = slides[currentSlideIndex].popups;
        if (!iconsVisible || index < 0 || index >= popups.Length)
        {
            Debug.LogWarning("Cannot show popup: icon not visible or index out of range");
            return;
        }

        if (currentPopupIndex == index)
        {
            if (interactionLog)
            {
                interactionLog.LogIconClosed(currentSlideIndex + 1, "USER");
            }
            HideAllPopups();
            Debug.Log("Hiding popup: " + index);
            return;
        }

        HideAllPopups();
        if (popups[index] != null)
        {
            popups[index].SetActive(true);
            currentPopupIndex = index;
            if (interactionLog)
            {
                interactionLog.LogIconOpened(slides[currentSlideIndex].icons[index].name, currentSlideIndex + 1, "USER");
            }
            var vp = popups[index].GetComponent<VideoPlayer>();
            if (vp != null)
            {
                vp.Stop();
                vp.Play();
                Debug.Log("Playing video in popup: " + index);
            }
            Debug.Log("Showing popup: " + index);
        }
    }

    void HideAllPopups()
    {
        foreach (var popup in slides[currentSlideIndex].popups)
            if (popup != null) popup.SetActive(false);
        currentPopupIndex = -1;
    }


    void CheckRayInteraction()
    {
        Transform rayOrigin = rayOriginOverride != null ? rayOriginOverride : GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform;
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            var highlights = slides[currentSlideIndex].icons; // this will now store Highlights directly
            for (int i = 0; i < highlights.Length; i++)
            {
                GameObject highlight = highlights[i];
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
            // Reset color if ray hits nothing
            foreach (var highlight in slides[currentSlideIndex].icons)
            {
                if (highlight == null) continue;
                Renderer rend = highlight.GetComponent<Renderer>();
                if (rend) rend.material.color = baseColor;
            }
        }
    }
}