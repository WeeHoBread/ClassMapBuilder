using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableObjectCycler : TabSwapper
{
    public static PlacableObjectCycler Instance { get; private set; }

    public int[] CommandBarDefaultIndexes;
    public int[] CommandBarDefaultBuildStates;
    public VisualOnInteract[] CommandbarBarDefaultButtons;
    
    private int activeTabIndex = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            SwapToNextButton();
        }
    }

    public void SwapToNextButton()
    {
        int newIndex;

        CommandbarBarDefaultButtons[activeTabIndex].ResetHighLight(CommandbarBarDefaultButtons[activeTabIndex].transform);

        if (activeTabIndex >= tabButtons.Length - 1)
        {
            newIndex = 0;
        }
        else
        {
            newIndex = activeTabIndex + 1;
        }

        SetActiveButtonCustom(tabButtons[newIndex]);
        activeTabIndex = newIndex;
        CommandbarBarDefaultButtons[newIndex].HighlightPress();

        //Swap to new active command bar
        SetActivePanel(panels[newIndex]); //Set correct active command bar
        GridBuilder.Instance.ChangeSelectedObject(CommandBarDefaultBuildStates[newIndex]); //Change placable object
        panels[newIndex].GetComponentInChildren<ScrollRectMovement>().ScrollToPosition(0); //Scroll command bar to the left
    }

    public void SetIndex(int newIndex)
    {
        activeTabIndex = newIndex;
    }

}
