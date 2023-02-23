using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClassSelect : MonoBehaviour
{
    public AdminPanelManager adminPM;

    public void MouseHover()
    {
        adminPM.PanelDataHover(gameObject.name, true);
    }

    public void MouseExit()
    {
        adminPM.PanelDataHover(gameObject.name, false);
    }

    public void MouseClick()
    {
        print("Select Data Set : " + gameObject.name);
        adminPM.PanelDataClicked(gameObject.name);
    }
}
