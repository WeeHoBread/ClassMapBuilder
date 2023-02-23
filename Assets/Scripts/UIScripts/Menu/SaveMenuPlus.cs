using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveMenuPlus : MonoBehaviour
{
    [SerializeField] public List<string> saveFileNameList;

    public GameObject overwriteMenu;
    public GameObject load;
    public float bestTime;
    public float currentTime = 0.0f;
    public string timeLapsed;

    private PlayfabManager pFM;
    private GameManager gm;

    private void Awake()
    {
        AquireReferences();
    }

    private void AquireReferences()
    {
        pFM = FindObjectOfType<PlayfabManager>();
        gm = FindObjectOfType<GameManager>();
    }
    public void UpdateUserFileList()
    {
        saveFileNameList = pFM.GetAllUserFileNames();
    }

    public void SaveGrid()
    {
        //Get grid json file here
        string jsonFile = GridBuilder.Instance.GetJson();

        string fileName = pFM.GetUserID();
        pFM.SaveGridSequence(fileName, jsonFile, 0);
        gm.SetRecord(0, 0);
        if (!Tooltip.Instance.isActiveAndEnabled)
        {
            gm.DisplayToolTipText("Saved");
            Timer.Create(gm.HideToolTip, 2f, "Saving"); //Hide warning after 2 seconds
        }
        else
        {
            gm.DisplayToolTipText("Saved");
            Timer.ResetTimer(gm.HideToolTip, 2f, "Saving"); //Hide warning after 2 seconds
        }
    }


    public void LoadData()
    {
        StartCoroutine(loadMenu());
    }

    private IEnumerator loadMenu()
    {
        pFM.loading = true;
        pFM.GetSaveFileNames();

        if (!Tooltip.Instance.isActiveAndEnabled)
        {
            gm.DisplayToolTipText("Connecting to server...");
            Timer.Create(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }
        else
        {
            gm.DisplayToolTipText("Connecting to server...");
            Timer.ResetTimer(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }

        yield return new WaitUntil(() => pFM.loading == false);

        if (!Tooltip.Instance.isActiveAndEnabled)
        {
            gm.DisplayToolTipText("Connected");
            Timer.Create(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }
        else
        {
            gm.DisplayToolTipText("Connected");
            Timer.ResetTimer(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }

        pFM.ConvertAndSplitFileNames();
        UpdateUserFileList();

        if (CheckFile())
        {
            pFM.LoadSaveFile(pFM.GetUserID());
            load.SetActive(true);
        }
        else
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("No save data");
                Timer.Create(gm.HideToolTip, 2f, "LoadError"); //Hide warning after 2 seconds
            }
            else
            {
                gm.DisplayToolTipText("No save data");
                Timer.ResetTimer(gm.HideToolTip, 2f, "LoadError"); //Hide warning after 2 seconds
            }
        }
    }

    public void LoadGrid()
    {
        GridBuilder.Instance.LoadGrid(pFM.newlyReceivedFile);
        gm.SetRecord(pFM.GetAttempts(), pFM.GetRecord());
        if (!Tooltip.Instance.isActiveAndEnabled)
        {
            gm.DisplayToolTipText("Loaded");
            Timer.Create(gm.HideToolTip, 2f, "Loading"); //Hide warning after 2 seconds
        }
        else
        {
            gm.DisplayToolTipText("Loaded");
            Timer.ResetTimer(gm.HideToolTip, 2f, "Loading"); //Hide warning after 2 seconds
        }
    }

    public bool CheckFile()
    {
        if (saveFileNameList != null)
        {
            return true;
        }

        return false;
    }
    public void Menu_SaveOrOverWrite()
    {
        StartCoroutine(saveMenu());
    }

    private IEnumerator saveMenu()
    {
        string jsonFile = GridBuilder.Instance.GetJson();
        if (jsonFile == null)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("Spawn Point or End Point missing");
                Timer.Create(gm.HideToolTip, 2f, "saveWarning"); //Hide warning after 2 seconds
            }
            else
            {
                gm.DisplayToolTipText("Spawn Point or End Point missing");
                Timer.ResetTimer(gm.HideToolTip, 2f, "saveWarning"); //Hide warning after 2 seconds
            }
            yield break;
        }
        pFM.loading = true;
        pFM.GetSaveFileNames();

        if (!Tooltip.Instance.isActiveAndEnabled)
        {
            gm.DisplayToolTipText("Connecting to server...");
            Timer.Create(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }
        else
        {
            gm.DisplayToolTipText("Connecting to server...");
            Timer.ResetTimer(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }

        yield return new WaitUntil(() => pFM.loading == false);

        if (!Tooltip.Instance.isActiveAndEnabled)
        {
            gm.DisplayToolTipText("Connected");
            Timer.Create(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }
        else
        {
            gm.DisplayToolTipText("Connected");
            Timer.ResetTimer(gm.HideToolTip, 2f, "FetchData"); //Hide warning after 2 seconds
        }

        pFM.ConvertAndSplitFileNames();
        UpdateUserFileList();

        if (CheckFile())
        {
            overwriteMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Overwrite saved file" + " ?";
            overwriteMenu.SetActive(true);
        }
        else if (!CheckFile())
        {
            overwriteMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Save new file" + " ?";
            overwriteMenu.SetActive(true);
        }
    }
}
