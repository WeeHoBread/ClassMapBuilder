using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//
public class SaveMenu : MonoBehaviour
{
    [SerializeField] private Transform saveScrollContent;

    [SerializeField] private List<string> saveFileNameList;

    [SerializeField] private GameObject newFileNamePrefab;

    public GameObject overwrite;
    public GameObject load;
    public GameObject delete;

    private void Awake()
    {
        AquireReferences();
        GetSavesFromFolder();
    }

    private void AquireReferences()
    {
        saveScrollContent = GameObject.Find("SavesContent").transform; //Required, will use to dynamically adjust Height to amount of save files
    }

    private void GetSavesFromFolder()
    {
        var filePath = Application.dataPath + "/GridSaves";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        DirectoryInfo dir = new DirectoryInfo(filePath);
        FileInfo[] info = dir.GetFiles("*.txt");

        string txtExtension = ".txt";

        int i = 0;

        while (saveScrollContent.childCount < info.Length)
        {
            string fileName = info[i].Name;
            fileName = fileName.Replace(txtExtension, "");
            saveFileNameList.Add(fileName);

            GameObject newFileName = Instantiate(newFileNamePrefab);
            newFileName.transform.SetParent(saveScrollContent, false);
            newFileName.GetComponentInChildren<TextMeshProUGUI>().text = fileName;

            i++;
        }
    }

    public void UpdateSaveFile(string fileName)
    {
        saveFileNameList.Add(fileName);
        GameObject newFileName = Instantiate(newFileNamePrefab);
        newFileName.transform.SetParent(saveScrollContent, false);
        newFileName.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
    }

    public void UpdateDeletedFile (string fileName)
    {
        for (int i = 0; i < saveScrollContent.childCount; i++)
        {
            if(fileName == saveScrollContent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text)
            {
                Destroy(saveScrollContent.GetChild(i).gameObject);
                saveFileNameList.Remove(fileName);
                break;
            }
        }
    }

    private void OnEnable()
    {
        GridBuilder.Instance.DisableBuild();
    }

    private void OnDisable()
    {
        GridBuilder.Instance.EnableBuild();
        CloseMenu();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        overwrite.SetActive(false);
        delete.SetActive(false);
        load.SetActive(false);
    }    
}
