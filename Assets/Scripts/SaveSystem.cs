using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private const string Save_Extention = "txt";
    private static readonly string Grid_Save_Folder = Application.dataPath + "/GridSaves/";
    private static bool isInitialize = false;

    public static void Initialize()
    {
        if (!isInitialize)
        {
            isInitialize = true;
            //Check if folder exist
            if (!Directory.Exists(Grid_Save_Folder))
            {
                //If it does not, create it
                Directory.CreateDirectory(Grid_Save_Folder);
            }
        }
    }

    #region Code for Saving File
    public static void Save(string fileName, string saveString, bool overWrite)
    {
        Initialize();
        string saveFileName = fileName;

        if (!overWrite)
        {
            //Forces save number to be unique to prevent overwritting
            int saveNumber = 1;
            while (File.Exists(fileName + "_" + saveNumber + ".txt"))
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }
        }

        //Write (name of save file and where it is located, its contents)
        File.WriteAllText(Grid_Save_Folder + saveFileName + "." + Save_Extention, saveString); //Saving GridObjects Only
    }

    public static void SaveObject(string fileName, object saveObject, bool overWrite)
    {
        Initialize();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overWrite);
    }

    public static void SaveObject(object saveObject)
    {
        SaveObject("save", saveObject, true);
    }



    #endregion

    #region Loading File
    public static string Load(string fileName)
    {
        Initialize();
        if (File.Exists(Grid_Save_Folder + fileName + "." + Save_Extention))
        {
            string saveString = File.ReadAllText(Grid_Save_Folder + fileName + "." + Save_Extention);
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static string LoadMostRecent()
    {
        Initialize();

        
        DirectoryInfo directoryInfo = new DirectoryInfo(Grid_Save_Folder);
        //Get access to all save files in said folder
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + Save_Extention);
        
        //Reference to store and access most recent save file
        FileInfo mostRecentFile = null;

        //Cycle through each file to check which is most recent base on Last Write Time
        foreach (FileInfo fileInfo in saveFiles)
        {
            if(mostRecentFile == null)
            {
                mostRecentFile = fileInfo;
            }
            else if(fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
            {
                mostRecentFile = fileInfo;
            }
        }

        if (mostRecentFile != null)
        {
            string savedString = File.ReadAllText(mostRecentFile.FullName);
            return savedString;
        }
        else
        {
            return null;
        }
    }


    public static TSaveObject LoadMostRecentObject<TSaveObject>()
    {
        Initialize();
        string saveString = LoadMostRecent();
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }

    public static TSaveObject LoadObject<TSaveObject>(string fileName)
    {
        Initialize();
        string saveString = Load(fileName);
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            Debug.Log("File not found");
            return default(TSaveObject);
        }
    }

    #endregion

    #region Delete File
    public static void DeleteFile(string fileName)
    {
        if (File.Exists(Grid_Save_Folder + fileName + "." + Save_Extention))
        {
            File.Delete(Grid_Save_Folder + fileName + "." + Save_Extention);
            File.Delete(Grid_Save_Folder + fileName + "." + Save_Extention + ".meta");
        }
    }
    #endregion
}
