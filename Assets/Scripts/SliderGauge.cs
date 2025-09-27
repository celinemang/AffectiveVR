// Refactored version of SliderGauge with reliable nudge + interaction logic
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderGauge : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;
    public Image fillImage;
    public TextMeshProUGUI valueText;
    public CanvasGroup canvasGroup;

    [Header("Input")]
    public float unitsPerSecond = 60f;
    [Range(0f, 1f)] public float deadzone = 0.05f;

    [Header("Auto Hide")]
    public float idleHideDelay = 2f;
    public float fadeSpeed = 6f;
    public float shownAlpha = 1f;
    public float hiddenAlpha = 0f;

    [Header("Nudge")]
    public bool enableNudge = true;
    public float hapticDuration = 0.08f;
    [Range(0f, 1f)] public float hapticAmplitude = 0.5f;
    [Range(0f, 1f)] public float hapticFrequency = 0.8f;
    public float nudgeCooldown = 30f; //30sec
    public float postNudgeInterval = 15f; //15sec

    private float lastInputTime;
    private float inactivityTimer = 0f;
    private float secondCounter = 0f;

    private float nudgeIntensity = 10f;
    private float baseNudgeIntensity = 20f;

    private bool isInPostNudgePhase = false;

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>(true);
        if (!canvasGroup) canvasGroup = GetComponentInChildren<CanvasGroup>(true);
        if (!fillImage && slider && slider.fillRect)
            fillImage = slider.fillRect.GetComponent<Image>();

        if (slider) slider.direction = Slider.Direction.LeftToRight;
        if (fillImage)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        canvasGroup.alpha = hiddenAlpha;
    }

    void Update()
    {
        float stickX = ReadStickX();
        bool interacted = false;

        if (Mathf.Abs(stickX) > deadzone)
        {
            float delta = stickX * unitsPerSecond * Time.deltaTime;
            slider.value = Mathf.Clamp(slider.value + delta, slider.minValue, slider.maxValue);
            interacted = true;
        }

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("ResetByButton - Source: RightController_A");
            interacted = true;
        }

        if (interacted)
        {
            OnUserInteraction();
        }

        UpdateVisibility();
        UpdateGaugeVisuals();

        secondCounter += Time.deltaTime;
        if (secondCounter >= 1f)
        {
            inactivityTimer += 1f;
            secondCounter = 0f;
            Debug.Log($"[SliderGauge] inactivity Timer: {inactivityTimer} sec");
        }

        if (Time.time - lastInputTime > idleHideDelay)
        {
            if (!isInPostNudgePhase && inactivityTimer >= nudgeCooldown)
            {
                TriggerNudge();
                isInPostNudgePhase = true;
                inactivityTimer = 0f;
            }
            else if (isInPostNudgePhase && inactivityTimer >= postNudgeInterval)
            {
                nudgeIntensity += 10f;
                TriggerNudge();
                inactivityTimer = 0f;
            }
        }
        if (slider.value != 0f)
        {
            TotalLog.GaugeValue = slider.value;
        }
        else
        {
            TotalLog.GaugeValue = 0f;
        }
    }

    void TriggerNudge()
    {
        Debug.Log($"[Nudge] Triggered at intensity {nudgeIntensity} | Timer: {inactivityTimer}");
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        float dynamicAmplitude = Mathf.Clamp01(nudgeIntensity / 100f);
        OVRInput.SetControllerVibration(hapticFrequency, dynamicAmplitude, OVRInput.Controller.RTouch);
        Invoke(nameof(StopHaptic), hapticDuration);
#endif
    }

    void StopHaptic()
    {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
#endif
    }

    void OnUserInteraction()
    {
		//Debug.Log($"[SliderGauge] Interaction Detected at {Time.time:F2}");
        lastInputTime = Time.time;
        ResetNudgeState();
        canvasGroup.alpha = shownAlpha;
    }

    void ResetNudgeState()
    {
        inactivityTimer = 0f;
        nudgeIntensity = baseNudgeIntensity;
        isInPostNudgePhase = false;
    }

    void UpdateVisibility()
    {
        float targetAlpha = (Time.time - lastInputTime < idleHideDelay) ? shownAlpha : hiddenAlpha;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    void UpdateGaugeVisuals()
    {
        if (slider && fillImage && slider.maxValue > 0f)
        {
            float t = slider.value / slider.maxValue;
            fillImage.color = t < 0.5f ? Color.Lerp(Color.green, Color.yellow, t / 0.5f)
                                       : Color.Lerp(Color.yellow, Color.red, (t - 0.5f) / 0.5f);
        }
        if (valueText)
            valueText.text = Mathf.RoundToInt(slider.value).ToString();
    }

    float ReadStickX()
    {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        Vector2 ovr = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        return ovr.x;
#else
        return 0f;
#endif
    }
}