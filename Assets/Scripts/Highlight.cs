using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Collider))]
public class Highlight : MonoBehaviour
{
    private Renderer rend;
    private Color baseColor = new Color(1f, 1f, 0f, 0.45f);   // transparent yellow
    private Color hoverColor = new Color(1f, 1f, 0f, 0.6f);  // brighter yellow
    private Color clickColor = new Color(1f, 0.5f, 0f, 0.8f); // orange
    private bool isHovered = false;
    

    void Start()
    {
        rend = GetComponent<Renderer>();
        SetColor(baseColor);
    }

    void OnMouseEnter()   // when ray hovers (works with OVR/OVRRaycaster)
    {
        SetColor(hoverColor);
        isHovered = true;

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && isHovered)
        {

            SetColor(clickColor);
            Debug.Log("[Highlight] Clicked on " + gameObject.name);
        }
    }

    void OnMouseExit()
    {
        if (isHovered)
        {
            isHovered = false;
            SetColor(baseColor);
        }
    }

    void OnMouseDown()    // trigger (click) on cube
    {
        SetColor(clickColor);
    }

    void OnMouseUp()
    {
        SetColor(hoverColor); // go back to hover color
    }

    private void SetColor(Color c)
    {
        if (rend != null)
        {
            rend.material.color = c;
        }
    }
}