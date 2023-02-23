using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputfieldStringToInt : MonoBehaviour
{
    [SerializeField] private TMP_InputField lengthInput;
    [SerializeField] private TMP_InputField widthInput;

    [SerializeField] private int newGridLength;
    [SerializeField] private int newGridWidth;

    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void test()
    {
        newGridLength = 10;
    }

    public void Convert()
    {
        if (lengthInput.text == "")
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.Create(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
            }
            else
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.ResetTimer(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
            }
            return;
        }

        if(widthInput.text == "")
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.Create(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
            }
            else
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.ResetTimer(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
            }
            return;
        }

        string newLength = lengthInput.text;
        newGridLength = int.Parse(newLength);
        Debug.Log(newGridLength);

        string newWidth = widthInput.text;
        newGridWidth = int.Parse(newWidth);
        Debug.Log(newGridWidth);
    }

}
