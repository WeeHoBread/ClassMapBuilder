using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSwapper : MonoBehaviour
{
    [SerializeField] protected GameObject[] tabButtons;
    [SerializeField] protected GameObject[] panels;
    [SerializeField] protected bool setActiveOnStart = false;
    [SerializeField] protected bool customButton = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!setActiveOnStart)
        {
            return;
        }

        if (customButton == false)
        {
            SetActiveButton(tabButtons[0]);
        }
        else
        {
            SetActiveButtonCustom(tabButtons[0]);
        }

        SetActivePanel(panels[0]);
    }

    public void SetActivePanel(GameObject newActiveTab)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false); //Turn all panels off
        }
        newActiveTab.SetActive(true);  //Turn selected panel on
    }

    public void SetActiveButton(GameObject newActiveButton)
    {
        for (int j = 0; j < tabButtons.Length; j++)
        {
            //Turn button gray to show that it is not active tab but allow it to be clicked for changing tab
            tabButtons[j].GetComponent<Button>().interactable = true;
            tabButtons[j].GetComponent<Image>().color = Color.gray;
        }

        //Turn button to default color to show it is the selected tab, but make it not interactable
        newActiveButton.GetComponent<Button>().interactable = false;
        newActiveButton.GetComponent<Image>().color = Color.white;
    }

    public void SetActiveButtonCustom(GameObject newActiveButton)
    {
        for (int j = 0; j < tabButtons.Length; j++)
        {
            //Turn button gray to show that it is not active tab but allow it to be clicked for changing tab
            tabButtons[j].GetComponent<Image>().color = Color.gray;
        }

        //Turn button to default color to show it is the selected tab, but make it not interactable
        newActiveButton.GetComponent<Image>().color = Color.white;
    }
}
