using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class TooltipFollower : MonoBehaviour
{
    //Reference to canvas transform
    [SerializeField] private RectTransform canvasRectTransform;

    //Reference to background and text component
    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform textMeshProTransform;

    //Reference for self
    private RectTransform rectTransform;

    private void Awake()
    {
        backgroundRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        textMeshPro = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        textMeshProTransform = transform.GetChild(1).GetComponent<RectTransform>();

        rectTransform = transform.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        AnchorToolTipPositionToMouse();
    }

    private void AnchorToolTipPositionToMouse()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        //Check if tooltip has move too far to the right
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        //Check if tooltip has move too high up
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
        {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height - (rectTransform.anchorMax.y * 2000);
        }

        //Check and reference to check if it has moved too far to the left or too far down
        Rect screenRect = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
        if (anchoredPosition.x < screenRect.x + (rectTransform.anchorMax.x * -2000))
        {
            anchoredPosition.x = screenRect.x + (rectTransform.anchorMax.x * -3000);
        }

        if (anchoredPosition.y < screenRect.y)
        {
            anchoredPosition.y = screenRect.y;
        }

        rectTransform.anchoredPosition = anchoredPosition;
    }

    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText); //set text
        textMeshPro.ForceMeshUpdate(); //Get latest mesh visual

        Vector2 textsize = textMeshPro.GetRenderedValues(false); //get text size

        Vector2 paddingSize = new Vector2(textMeshProTransform.anchoredPosition.x * 2, textMeshProTransform.anchoredPosition.y * 2);

        backgroundRectTransform.sizeDelta = textsize + paddingSize; //adjust background size according to text size
    }


}
