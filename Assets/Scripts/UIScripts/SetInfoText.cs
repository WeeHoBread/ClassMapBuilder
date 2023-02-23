using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetInfoText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userIDText;
    [SerializeField] private TextMeshProUGUI classIDText;
    [SerializeField] private TextMeshProUGUI gridLengthText;
    [SerializeField] private TextMeshProUGUI gridWidthText;

    [SerializeField] private GridBuilder gB;
    private PlayfabManager pFB;

    private void Start()
    {
        gB = FindObjectOfType<GridBuilder>();
        pFB = FindObjectOfType<PlayfabManager>();
        //SetAllInfo();
    }

    public void SetAllInfo()
    {
        if (gB == null)
        {
            gB = FindObjectOfType<GridBuilder>();
        }

        if(pFB == null)
        {
            pFB = FindObjectOfType<PlayfabManager>();
        }
       
        string gL = gB.gridHeight.ToString();
        string gW = gB.gridWidth.ToString();

        string userID = pFB.GetUserID();
        string classID = pFB.GetClassID();

        SetGridSizeInfo(gL, gW);
        SetUserIDInfo(userID);
        SetClassIDInfo(classID);
    }

    public void SetUserIDInfo(string newUserID)
    {
        userIDText.text = "Admin No: " + newUserID;
    }

    public void SetClassIDInfo(string newClassID)
    {
        classIDText.text = "Class: " + newClassID;
    }

    public void SetGridSizeInfo(string newGridLength, string newGridWidth)
    {
        gridLengthText.text = "Grid Length: " + newGridLength;
        gridWidthText.text = "Grid Width: " + newGridWidth;
    }

}
