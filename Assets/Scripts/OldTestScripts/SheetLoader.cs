using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class SheetLoader : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public string userID;
        public string fileName;
        public string fileTxt;
    }

    [System.Serializable]
    public class SaveDataList
    {
        public SaveData[] values;
    }


    [HideInInspector]
    public string latestJsonFile;
    public string latestSaveData;


    public SaveDataList myDataList = new SaveDataList();
    private string[] splitData;

    [HideInInspector]
    public string[] thisUserDataName;
    [HideInInspector]
    public string[] thisUserDataFile;

    public string[] allDataName;
    [HideInInspector]
    public string[] allDataFile;
    
    [SerializeField] private Transform saveScrollContent;
    [SerializeField] private List<string> saveFileNameList;
    [SerializeField] private GameObject newFileNamePrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ObtainData());

    }

    private IEnumerator ObtainData()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/1Rp0yrzBuiaJr7xhdQAMaplyCaDcwReD4QP8zjd4Rvr4/values/Form%20Responses%201?key=AIzaSyDckiNiBU9bBU_oV7ofbcVfEEaG0mD3aKE");
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + webRequest.error);
        }
        else
        {
            string json = webRequest.downloadHandler.text;

            latestJsonFile = json;
            myDataList = JsonUtility.FromJson<SaveDataList>(json);
            RemoveElement(ref myDataList.values, 0);
            splitData = TrimJson(json);

            string[] targetSplitData;

            for (int i = 0; i < splitData.Length; i++)
            {
                targetSplitData = TrimData(splitData[i]);
                myDataList.values[i].userID = targetSplitData[0];
                myDataList.values[i].fileName = targetSplitData[1];
                myDataList.values[i].fileTxt = targetSplitData[2];
            }
        }

        GetAllDataFile();
        AquireReferences();
        GetSavesFromFolder();
    }

    public void GetLatestSaveData() //Run this when in a refresh button in save menu
    {
        StartCoroutine(ObtainData());
    }

    private void RemoveElement<T>(ref T[] arr, int index)
    {
        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }

        Array.Resize(ref arr, arr.Length - 1);

    }

    private string[] TrimJson(string jsonFile)
    {
        //jsonFile = Regex.Replace(jsonFile, @"\s+", String.Empty); //Remove Empty Space
        jsonFile = jsonFile.Remove(0, 95);
        jsonFile = jsonFile.Replace("],[", "");
        jsonFile = jsonFile.Replace("[[", "");
        jsonFile = jsonFile.Replace("]]", "");
        jsonFile = jsonFile.Replace("]]}", "");
        jsonFile = jsonFile.Replace("}", "");
        jsonFile = Regex.Replace(jsonFile, @"\s+", String.Empty); //Remove Empty Space
        jsonFile = jsonFile.Replace("],[", "");
        //jsonFile.Replace()
        char[] splitterWords = {' '};
        //Debug.Log(jsonFile.Split(splitterWords));
        string[] newData = jsonFile.Split(splitterWords);

        foreach (string x in newData)
        {
            
            Debug.Log(x);   
        }

        for (int i = 0; i < newData.Length; i++)
        {
            if (newData[i] == "," || newData[i] == "" || newData[i] == " ")
            {
                RemoveElement(ref newData, i);
            }
        }

        //Remove unrequired data and empty slots
        RemoveElement(ref newData, 0);
        RemoveElement(ref newData, 0);
        RemoveElement(ref newData, newData.Length);

        return newData;

    }

    private string[] TrimData(string text)
    {
        char[] splitterWords = { '"', '"', ',' };
        text = text.Replace('"', ' ').Trim();
        text = Regex.Replace(text, @"\s+", String.Empty);
        string[] newData = text.Split(splitterWords);

        for (int i = 0; i < newData.Length; i++)
        {
            if (newData[i] == "," || newData[i] == "" || newData[i] == " ")
            {
                RemoveElement(ref newData, i);
            }
        }

        RemoveElement(ref newData, 0); //Remove Time Stamp

        return newData;
    }

    private void GetAllDataFile()
    {
        List<string> newNameList = new List<string>();
        List<string> newFileList = new List<string>();

        for (int i = 0; i < myDataList.values.Length; i++)
        {
            newNameList.Add(myDataList.values[i].fileName);
            newFileList.Add(myDataList.values[i].fileTxt);

            allDataName = newNameList.ToArray();
            allDataFile = newFileList.ToArray();
        }
    }

    public void SearchForUserData(string userID)
    {
        List<string> newNameList = new List<string>();
        List<string> newFileList = new List<string>();

        for (int i = 0; i < myDataList.values.Length; i++)
        {
            if (userID == myDataList.values[i].userID)
            {
                newNameList.Add(myDataList.values[i].fileName);
                newFileList.Add(myDataList.values[i].fileTxt);

                thisUserDataName = newNameList.ToArray();
                thisUserDataFile = newFileList.ToArray();
                //This file belongs to him
            }
        }
    }

    public string GetRandomLevel(string userID)
    {
        int randomLevelIndex = UnityEngine.Random.Range(0, myDataList.values.Length);
        string dataFile;

        if (userID == myDataList.values[randomLevelIndex].userID)
        {
            //This file belongs to the user
            return null;
        }

        dataFile = myDataList.values[randomLevelIndex].fileTxt;


        return dataFile;
    }
    private void AquireReferences()
    {
        saveScrollContent = GameObject.Find("LoadContent").transform; //Required, will use to dynamically adjust Height to amount of save files
    }

    private void GetSavesFromFolder()
    {

    }
    

    public void GetUserID(string newUserId)
    {
        int i = 0;

        do
        {
            string fileName = allDataName[i];

            saveFileNameList.Add(fileName);

            GameObject newFileName = Instantiate(newFileNamePrefab);
            newFileName.transform.SetParent(saveScrollContent, false);
            newFileName.GetComponentInChildren<TextMeshProUGUI>().text = fileName;

            i++;
        }        while (saveScrollContent.childCount < allDataName.Length);
        Debug.Log(saveScrollContent.childCount);
    }
}
