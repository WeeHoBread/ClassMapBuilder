using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class UI_Interact : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    private KeyCode existingKeycode;
    private bool hasButtonPress;

    [Header("Additional Settings")]
    [SerializeField] private bool pressAndHold;

    private bool buttonHeld = false;

    [Serializable]
    public class ClickEvent : UnityEvent
    {

    }

    public ClickEvent OnEvent;
    public ClickEvent SoundEvent;
    public bool isMenuButton;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<VisualOnInteract>() != null)
        {
            existingKeycode = GetComponent<VisualOnInteract>().firstInput;
            hasButtonPress = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pressAndHold)
        {
            return;
        }

        OnEvent.Invoke();
        SoundEvent.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!pressAndHold)
        {
            return;
        }

        SoundEvent.Invoke();
        buttonHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonHeld = false;
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
                    return;
                }
            }
        }
        if (buttonHeld && pressAndHold)
        {
            OnEvent.Invoke();
            return;
        }
    

        if (hasButtonPress)
        {
            if (Input.GetKeyUp(existingKeycode))
            {
                OnEvent.Invoke();
                SoundEvent.Invoke();
            }
        }
    }

    public void ToggleToActive(GameObject gameObject) //Currently used for: Save Menu button
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
    }

}
