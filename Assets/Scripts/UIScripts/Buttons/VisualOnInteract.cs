using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class VisualOnInteract : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color pressedColor;
    private Color originalColor;

    [Header("Keys Required")]
    public KeyCode firstInput;

    [Header("Alternate Keys")]
    public KeyCode secondInput;
    [SerializeField] private KeyCode disablerKey;

    [Header("Additional Settings")]
    [SerializeField] private bool highLightOnPress;
    public bool toggleColor;
    private bool colorToggled = false;
    [SerializeField] private bool resetConnectedInactivity;
    [SerializeField] private bool showTooltip;
    [SerializeField] private bool autoPressOnStart;
    //[SerializeField] private bool linkedButton;

    //Tooltip variables
    [Header("Requires showTooltip")]
    [SerializeField] private string toolTipMessage;
    private float hoverWaitTime = 0.5f;
    Coroutine hoverRoutine = null;

    [Header("Additional Color")]
    [SerializeField] private Color highlightColor;
    public Color toggledColor;

    private IEnumerator buttonPressCoroutine;
    public bool inactive;
    public bool isMute;
    public bool isMenuButton;
    //[Header("Linked Buttons")]
    //[SerializeField] private VisualOnInteract[] linkedButtons;

    private void Start()
    {
        originalColor = GetComponent<Image>().color;
        buttonPressCoroutine = PseudoButtonPressCo();
        if (autoPressOnStart)
        {
            StartCoroutine(buttonPressCoroutine);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMenuButton)
        {
            GameObject[] menus = new GameObject[4];
            menus[0] = GameObject.Find("SaveMenu");
            menus[1] = GameObject.Find("HelpMenu");
            menus[2] = GameObject.Find("RevertSaveWarningPanel");
            menus[3] = GameObject.Find("ConfirmReset");
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i] != null && menus[i].activeSelf)
                {
                    if (!isMute)
                    {
                        ReturnToDefaultColor();
                    }
                    return;
                }
            }
        }
        if (inactive || Input.GetKey(disablerKey))
        {
            return;
        }

        if (Input.GetKeyDown(firstInput))
        {
            ButtonInteract();
        }
        else if (Input.GetKeyUp(firstInput) && !toggleColor)
        {
            ReturnToDefaultColor();
        }
    }

    #region Color Change
    public void FadeColor(Color color)
    {
        Graphic graphic = GetComponent<Graphic>();
        graphic.CrossFadeColor(color, 0.1f, true, true);
    }

    public void HighlightPress()
    {
        Image[] children;
        children = transform.parent.GetComponentsInChildren<Image>();

        Color defaultColor = new Color(1f, 1f, 1f, 1f);

        foreach (Image image in children)
        {
            image.color = defaultColor;
        }

        GetComponent<Image>().color = highlightColor;

    }

    public void ResetHighLight(Transform targetContent)
    {
        Image[] children;
        children = targetContent.GetComponentsInChildren<Image>();

        Color defaultColor = new Color(1f, 1f, 1f, 1f);

        foreach (Image image in children)
        {
            image.color = defaultColor;
        }
    }

    //public void ChangeToDefaultColor()
    //{
    //    Image image = GetComponent<Image>();
    //    Color tempColor = new Color(1f, 1f, 1f, 1f);
    //    image.color = tempColor;
    //}
    #endregion

    #region Sound emit
    //public void ClickSound()
    //{
    //    FindObjectOfType<AudioManager>().UISounds("Button Click");
    //}

    //public void HoverSound()
    //{
    //    FindObjectOfType<AudioManager>().UISounds("Button Hover");
    //}
    #endregion

    #region Methods
    private void ResetInactivity()
    {
        if (GetComponentInParent<UI_Inactivity>() != null)
        {
            GetComponentInParent<UI_Inactivity>().ResetTimer();
        }
    }

    public void ButtonInteract()
    {
        FadeColor(pressedColor);
        if (highLightOnPress)
        {
            HighlightPress();
        }

        if (toggleColor)
        {
            ToggleColor();
        }

        if (resetConnectedInactivity)
        {
            ResetInactivity();
        }
    }

    public void ToggleColor()
    {
        if (colorToggled == false)
        {
            FadeColor(toggledColor);
            colorToggled = true;
        }
        else
        {
            ReturnToDefaultColor();
            colorToggled = false;
        }
    }

    public void ReturnToDefaultColor()
    {
        FadeColor(originalColor);
    }

    #endregion

    #region Methods for hovering enter
    private void ShowMessage(string message)
    {
        if (!Tooltip.Instance2.isActiveAndEnabled && showTooltip)
        {
            //tool tip test
            System.Func<string> getTooltipNextFunc = () =>
            {
                return message;
            };
            Tooltip.ShowHelptip_Static(getTooltipNextFunc);
        }
    }

    private void HideTooltip()
    {
        Tooltip.HideHelptip_Static();
    }

    #endregion

    #region Pointer Interface
    //On click down
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ButtonInteract();
        }
    }

    //On click release
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!colorToggled)
            {
                FadeColor(originalColor);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverRoutine = StartCoroutine(HoverShowMessage());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(hoverRoutine);
        HideTooltip();
    }
    #endregion

    #region IEnumerators
    IEnumerator HoverShowMessage()
    {
        yield return new WaitForSeconds(hoverWaitTime);
        ShowMessage(toolTipMessage);
    }

    public void PseudoButtonPress()
    {
        StartCoroutine(buttonPressCoroutine);
    }

    public IEnumerator PseudoButtonPressCo()
    {
        ButtonInteract();
        yield return new WaitForSeconds(0.1f);
        ReturnToDefaultColor();
    }
    #endregion
    //private void FadeOtherButton(VisualOnInteract otherButtonComp)
    //{
    //    otherButtonComp.FadeColor(otherButtonComp.pressedColor);
    //}


}
