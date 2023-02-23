using System.Collections;
using UnityEngine;
using TMPro;

public class SetID : MonoBehaviour
{
    [SerializeField] private GameObject userIDSetPanel;
    [SerializeField] private GameObject classIDSetPanel;
    [SerializeField] private GameObject help;
    [SerializeField] private GameObject helpControls;
    [SerializeField] private GameObject helpPanels;
    [SerializeField] private GameObject bestClearTimePanel;

    private GameManager gm;

    private PlayfabManager pFM;
    private GridBuilder gB;
    private SetInfoText sIT;

    public bool isStart;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gB = FindObjectOfType<GridBuilder>();
        pFM = FindObjectOfType<PlayfabManager>();
        sIT = FindObjectOfType<SetInfoText>();

        FindObjectOfType<EditCamera>().CameraLock();
        gm.LockInState();
    }

    #region Mid-Game Data set
    public void SendUserID(TMP_InputField userInputField)
    {
        if (userInputField.text.Length == 0)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.Create(gm.HideToolTip, 2f, "UserInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.ResetTimer(gm.HideToolTip, 2f, "UserInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }

            return;
        }

        if(userInputField.text.Length < 8)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.Create(gm.HideToolTip, 2f, "NotEnoughCharacters1"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.ResetTimer(gm.HideToolTip, 2f, "NotEnoughCharacters1"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }

            return;
        }

        string newUserID = userInputField.text.ToUpper();
        pFM.SetUserID(newUserID);
        pFM.ClearLocalData();
        sIT.SetUserIDInfo(newUserID);
        pFM.ConvertAndSplitFileNames();
        userInputField.text = ""; //Empty field once change is made
    }

    public void SendClassID(TMP_InputField classInputField)
    {
        if (classInputField.text.Length == 0)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.Create(gm.HideToolTip, 2f, "ClassInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.ResetTimer(gm.HideToolTip, 2f, "ClassInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }

            return;
        }

        if (classInputField.text.Length < 3)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.Create(gm.HideToolTip, 2f, "NotEnoughCharacters2"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.ResetTimer(gm.HideToolTip, 2f, "NotEnoughCharacters2"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            return;

        }

        string newClassID = classInputField.text.ToUpper();
        pFM.SetClassID(newClassID);
        pFM.ClearLocalData();
        sIT.SetClassIDInfo(newClassID);
        pFM.ConvertAndSplitFileNames();
        classInputField.text = ""; //Empty field once change is made
    }
    #endregion

    #region Start Game Data Set
    public void StartSendUserID(TMP_InputField userInputField)
    {
        if (userInputField.text.Length == 0)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.Create(gm.HideToolTip, 2f, "UserInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.ResetTimer(gm.HideToolTip, 2f, "UserInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }

            return;
        }

        if (userInputField.text.Length < 8)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.Create(gm.HideToolTip, 2f, "NotEnoughCharacters1"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.ResetTimer(gm.HideToolTip, 2f, "NotEnoughCharacters1"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }

            return;
        }

        string newUserID = userInputField.text.ToUpper();
        pFM.SetUserID(newUserID);
        pFM.ClearLocalData();
        userInputField.text = "";
        userIDSetPanel.SetActive(false);
        bestClearTimePanel.SetActive(true);
        EnableEditMode();
    }

    public void StartSendClassID(TMP_InputField classInputField)
    {
        if (classInputField.text.Length == 0)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.Create(gm.HideToolTip, 2f, "ClassInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is empty");
                Timer.ResetTimer(gm.HideToolTip, 2f, "ClassInputfieldEmpty"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }

            return;
        }

        if (classInputField.text.Length < 3)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.Create(gm.HideToolTip, 2f, "NotEnoughCharacters2"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            else
            {
                gm.DisplayToolTipText("Input field is not filled up");
                Timer.ResetTimer(gm.HideToolTip, 2f, "NotEnoughCharacters2"); //Hide warning after 2 seconds
                FindObjectOfType<AudioManager>().EditModeSound("Build Error");
            }
            return;

        }

        string newClassID = classInputField.text.ToUpper();
        pFM.SetClassID(newClassID);
        classInputField.text = "";
        classIDSetPanel.SetActive(false);
        userIDSetPanel.SetActive(true);
    }
    #endregion

    public void EnableEditMode()
    {
        //gm.UnlockState();
        //gm.SetActiveEditModeUI();
        //gB.enabled = true;
        //gm.ghostGen.SetActive(true);
        //FindObjectOfType<PlayfabManager>().ConvertAndSplitFileNames();
        StartCoroutine(GoEdit());
    }
    private IEnumerator GoEdit()
    {
        FindObjectOfType<PlayfabManager>().ConvertAndSplitFileNames();
        yield return new WaitForSeconds(Time.deltaTime);
        FindObjectOfType<SaveMenuPlus>().UpdateUserFileList();
      
        gameObject.SetActive(false);
        gm.SetActiveEditModeUI();
        gB.enabled = true;
        gm.ghostGen.SetActive(true);
        //bestClearTimePanel.SetActive(true);

        if (FindObjectOfType<SaveMenuPlus>().saveFileNameList == null)
        {
            help.SetActive(true);
            help.GetComponent<TabSwapper>().SetActiveButton(helpControls);
            help.GetComponent<TabSwapper>().SetActivePanel(helpPanels);
            yield break;

        }
        FindObjectOfType<EditCamera>().CameraUnlocked();
        gm.UnlockState();
    }
}
