using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField] private string loginID;
    [SerializeField] private string password;
    public string newlyReceivedFile;

    private string allSaveFilesKey = "SaveFiles";
    //private string allSaveFilesKey = "AverageTimeTest";
    [SerializeField] public string targetFileName = "DummySaveFile";
    [SerializeField] private string allSaveFileNames; //Store all save files in a long line
    [SerializeField] private string userID = "User01";
    [SerializeField] private string classID = "Z99";

    [SerializeField] private string[] allUserFileNames; //Used to store all files belonging to this user
    [SerializeField] private string[] allUserBest; //Used to store all files belonging to this user
    [SerializeField] private string[] allNonUserFileNames; //Used to store all files not belonging to this user, to be used to for challenge mode
    [SerializeField] private string[] allNonUserBest; //Used to store all files not belonging to this user, to be used to for challenge mode
    public string[] testStringArray;

    [HideInInspector] public bool fileExist = false;
    [SerializeField] private string targetFile;
    private string targetJsonFile;
    private float totalTime;
    private int totalAttempts;
    private string cachedClass = "";

    private SaveMenuPlus sMP;
    private LoadingManager lm;
    public bool loading;

    #region Notes/idea on how to save/load
    /* 
     * Have a "Key" that stores all save file names in this format: UserID-SaveFileName
     * This key will be called allSaveFilesKey
     * When saving a new save, take in the save file name as multiple strings like so, "|" +UserID +  "-" + SaveFileName + "|";
     * At the same time, use the save file to create a new key with the actual json savefile as the value
     * -----------------
     * When loading a save file, split the save file using the char[] array '|'
     * This will split different save file names attached with their UserID in an string array
     * Go through length of the array, intake the userID and find it in each string.
     * If found, add it to a temporary list for all files belonging to the user, vice-versa if not found.
     * -----------------
     * Go through each temporary list and split using char[] array '-' in intake into new string array
     * index 0 being the userID, index 1 being the filename.
     * Index 1 will be added into two temporary list to files belonging and not belonging to the user.
     * Then convert list to array allUserFileNames and allNonUserFilesNames
     * -----------------
     * allFileNames will display the savefile
    */

    /*Notes on how to overwrite
     * Load all data by default to be able to detect an overwrite
     * detect using if statement, using && statement to find both userID and saveFileName together
     * If return true, open overwrite warning
     * If false, save as usual
     * -----------------
     * For overwriting, save as usual using the same key, this will replace existing data automatically 
     * 
     */
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        LoginUser();
        sMP = FindObjectOfType<SaveMenuPlus>();
        //Timer might need to be longer or initiate function at end of login where the debug.log is
        //Timer.Create(GetSaveFileNames, 3, "WaitForLoginCompletion");  //Get all save files


        //Split the files


    }

    #region Login to database
    private void RegisterUser()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = loginID,
            Password = password,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);

    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registered and logged in");
    }

    private void LoginUser()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = loginID,
            Password = password,
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }

    #endregion

    #region Action conversion for timer
    private void SendLeaderboardData()
    {
        SendDataToLeaderboard(10);
    }
    private void RetrieveLeaderboardData()
    {
        GetLeaderboardData();
    }

    public void SaveGridSequence(string newSaveFileName, string newGridjson, float best)
    {
        GetSaveFileNames(); //Load latest data
        SaveGridData(newSaveFileName, newGridjson, best);
        GetSaveFileNames(); //Load latest data
    }

    private void SaveGridData(string newFileName, string newGridSave, float best)
    {
        fileExist = false;
        targetFileName = newFileName;
        targetJsonFile = newGridSave;
        totalTime = best;
        SaveNewFile();

    }

    #endregion

    #region OnSuccess/Error Functions
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Success login/account creation");
        GetSaveFileNames();
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Error during login/account creation");
        Debug.Log(error.GenerateErrorReport());
    }

    public void OnLeaderboardSucessfulUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Sucessful leaderboard data sent");
    }

    public void OnLeaderboardSucessfulRetrieval(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            item.Position = 0;
            Debug.Log("Sucessful leaderboard data received: " + item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }

    #endregion

    #region Leaderboard
    public void SendDataToLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "BoardState",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardSucessfulUpdate, OnError);

    }

    public void GetLeaderboardData()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "BoardState",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSucessfulRetrieval, OnError);

    }

    #endregion

    #region Save/Load Data
    #region Loading
    #region Lookup file's existance


    Dictionary<string, List<string>> classesDict = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> GetClassData()
    {
        print("Attempting to Pull Class Data...");
        print(allSaveFileNames);
        string allData = allSaveFileNames;

        string[] iData = allData.Split("|"); //Change as needed
        print(iData.Length);

        for (int i = 1; i < iData.Length; i++)
        {
            string _data = iData[i];
            if (_data.Length > 0)
            {
                string[] _nData = _data.Split("_");
                string _class = _nData[0];
                string _student = _nData[1];

                print(_class + "::" + _student);

                AddToClassDictionary(_class, _student);
            }
        }

        DebugPrintClassDictionary();
        return classesDict;
    }

    public void AddToClassDictionary(string _key, string _student)
    {
        print("Adding to Class");
        if (!classesDict.ContainsKey(_key))
        {
            classesDict[_key] = new List<string>();
        }

        classesDict[_key].Add(_student);
    }

    public int GetStudentsFromClass(List<string> _list)
    {
        int _count = 0;

        foreach (string _str in _list)
        {
            _count++;
        }
        return _count;
    }

    public void DebugPrintClassDictionary()
    {
        //This is a debug print to show and list all classes and students
        print("///DEBUG///");
        foreach (var item in classesDict)
        {
            print(item.Key + " : " + GetStudentsFromClass(item.Value) + " Student(s)");
        }
        print("/////////");
    }

    public bool GetFileExist()
    {
        fileExist = false;
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), CheckFileExistance, OnError);
        return fileExist;
    }

    private void CheckFileExistance(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(targetFileName))
        {
            fileExist = true;
            Debug.Log("File exist");
        }
        else
        {
            Debug.Log("save file does not exist in data base");
        }
    }
    #endregion

    #region Get All File names
    public void GetSaveFileNames()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetAllSaveNames, OnError);
    }

    //Get all Save file names
    private void OnGetAllSaveNames(GetUserDataResult result)
    {
        print(result.Data);
        loading = true;
        if (result.Data != null && result.Data.ContainsKey(allSaveFilesKey))
        {
            allSaveFileNames = result.Data[allSaveFilesKey].Value;
            Debug.Log("File names retrieved " + allSaveFileNames);
        }
        else
        {
            Debug.Log("Savefile data not found/incomplete");
        }
        if (lm == null)
        {
            lm = FindObjectOfType<LoadingManager>();
        }
        loading = false;
        lm.isDataRetrieved = true;
    }
    #endregion

    #region Load TargetFile
    public void LoadSaveFile(string saveFileName)
    {
        targetFileName = saveFileName;
        GetFileFromServer();
    }

    public void GetFileFromServer()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), GetSpecificSaveFile, OnError);
    }

    private void GetSpecificSaveFile(GetUserDataResult result)
    {
        loading = true;
        if (result.Data != null && result.Data.ContainsKey(targetFileName))
        {
            newlyReceivedFile = SplitString(result.Data[targetFileName].Value)[0];
            totalTime = float.Parse(SplitString(result.Data[targetFileName].Value)[1]);
            totalAttempts = int.Parse(SplitString(result.Data[targetFileName].Value)[2]);
        }
        else
        {
            //Debug.Log("Savefile data not found/incomplete");
        }
        loading = false;
    }
    #endregion

    #endregion

    #region Saving
    #region Overwriting
    public void SaveNewFile()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), UploadNewFile, OnError);
    }

    private void UploadNewFile(GetUserDataResult result)
    {
        //-------------- Check for file overwrite -----------
        if (result.Data != null && result.Data.ContainsKey(targetFileName))
        {
            fileExist = true;
            Debug.Log("File exist");
        }
        else
        {
            Debug.Log("save file does not exist in data base");
        }

        //-------------- Base on overwrite, save file -------------------

        if (fileExist)
        {
            if (targetFileName == userID)
            {
                OverwriteSaveData(targetJsonFile, totalTime, 0);
            }
            else if (targetFileName != userID)
            {
                totalAttempts++;
                OverwriteSaveData(targetJsonFile, totalTime, totalAttempts);
            }
        }
        else
        {
            SaveNewData(targetJsonFile, totalTime, 0);
        }
    }
    #endregion

    public void SaveNewData(string saveFile, float best, int numOfTries)
    {
        print("New Data!");
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {allSaveFilesKey, allSaveFileNames + "|" + classID + "_" + userID},
                {userID, saveFile + "_" + best + "_" + numOfTries},
            }
        };
        //Debug.Log("saving new data");
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void OverwriteSaveData(string saveFile, float best, int numOfTries)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {targetFileName, saveFile  + "_" + best + "_" + numOfTries}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    #endregion

    #region Data Splincer
    public void ConvertAndSplitFileNames()
    {
        if (CheckIfFileEmpty())
        {
            return;
        }

        char[] DifferentSavesSplitterChar = { '|' };
        //allSaveFileNames = allSaveFileNames.Remove(0, 1);
        string allSaveFileNamesTemp = allSaveFileNames.Remove(0, 1);
        string[] splitSavedData = allSaveFileNamesTemp.Split(DifferentSavesSplitterChar);

        List<string> userSaveFileList = new List<string>();
        bool containUserFile = false;

        List<string> nonUserSaveFileList = new List<string>();
        bool containNonUserFile = false;

        foreach (string saveFile in splitSavedData)
        {

            if (saveFile.Contains(userID))
            {
                containUserFile = true;
                userSaveFileList.Add(saveFile);
                Debug.Log("User file found");

            }
            else if (saveFile.Contains(classID))
            {
                containNonUserFile = true;
                nonUserSaveFileList.Add(saveFile);
                Debug.Log("Classmate file found");
            }
        }

        if (containUserFile == true)
        {
            for (int i = 0; i < userSaveFileList.Count; i++)
            {
                userSaveFileList[i] = SplitString(userSaveFileList[i])[1];
            }
            allUserFileNames = userSaveFileList.ToArray();
        }

        if (containNonUserFile == true)
        {
            for (int i = 0; i < nonUserSaveFileList.Count; i++)
            {
                nonUserSaveFileList[i] = SplitString(nonUserSaveFileList[i])[1];
            }
            allNonUserFileNames = nonUserSaveFileList.ToArray();
        }
    }

    #region Data Clear
    public void ClearLocalData()
    {
        allUserFileNames = null;
        allNonUserFileNames = null;
    }
    #endregion
    private string[] SplitString(string combinedString)
    {
        //string newSaveData;
        char[] IDSaveSplitterChar = { '_' };
        string[] seperatedString = combinedString.Split(IDSaveSplitterChar);


        //newSaveData = seperatedString[0];

        return seperatedString;
    }
    //private string SplitSecondHalf(string combinedString)
    //{
    //    string newSaveData;
    //    char[] IDSaveSplitterChar = { '_' };
    //    string[] seperatedString = combinedString.Split(IDSaveSplitterChar);


    //    newSaveData = seperatedString[1];

    //    return newSaveData;
    //}


    #endregion

    private void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Sucessful user data sent");
    }

    #endregion

    #region Refresh
    public void RefreshFileList()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnFileRefresh, OnError);
    }

    private void OnFileRefresh(GetUserDataResult result)
    {
        OnGetAllSaveNames(result);
        StartCoroutine(FileRefreshRountine());
    }

    private IEnumerator FileRefreshRountine()
    {
        //Don't continue rountine while OnGetALlSaveNames is running
        while (loading)
        {
            yield return null;
        }

        //After getting all save names, split the names
        ConvertAndSplitFileNames();

        //Find save menu plus if not found yet
        if (sMP == null)
        {
            sMP = FindObjectOfType<SaveMenuPlus>();
        }

        //Clear the list
        //sMP.ClearSavesList();
        yield return new WaitForSeconds(0.5f);

        //Repopulate the list
        sMP.UpdateUserFileList();
        //sMP.GetSavesFromFolder();
    }

    #endregion

    #region Delete File

    public void AttemptFix()
    {
        //I noticed "adding back the 1111111111" back into the key, fixes it, but a manual reset might be better
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { allSaveFilesKey, "" },
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataDelete, OnError);
    }

    public void DeleteEntireClass(string _class)
    {
        cachedClass = _class;
        StartCoroutine(DeleteCoroutine());
    }

    IEnumerator DeleteCoroutine()
    {
        //This function deletes all class data under that class tag, e.g everything with the key "P01"
        string[] studentList = allSaveFileNames.Split("|");

        for (int i = 0; i < studentList.Length; i++)
        {
            string[] splitInfo = studentList[i].Split("_");
            if (studentList[i].Length > 1 && splitInfo[0] == cachedClass)
            {
                //print(_class + "_" + splitInfo[1]);
                string studentId = studentList[i].Split("_")[1];
                string studentToDelete = "|" + studentList[i];
                print(studentToDelete + " : " + studentId);
                DeleteFile(cachedClass, studentId);

                yield return new WaitForSeconds(2f);
            }
        }

        print("Class Deletion Complete!");
        cachedClass = "";

        /*
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { allSaveFilesKey, finalStudents },
                //{_student, null }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataDelete, OnError); */
    }
    public void DeleteFile(string _class, string _student)
    {
        string valueToRemove = "|" + _class + "_" + _student;
        print(valueToRemove);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { allSaveFilesKey, allSaveFileNames.Replace(valueToRemove, null) },
                {_student, null }
            }
        };

        print(allSaveFileNames);

        /*string saveFileToRemove = GameObject.Find("SaveMenuInputText").GetComponent<Text>().text;
        string valueToRemove = "|" + userID + "-" + saveFileToRemove;
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {allSaveFilesKey, allSaveFileNames.Replace(valueToRemove, null)},
                {saveFileToRemove, null }
            }
        };
        */
        PlayFabClientAPI.UpdateUserData(request, OnDataDelete, OnError);
    }

    private void OnDataDelete(UpdateUserDataResult result)
    {
        Debug.Log("Sucessful data deletion");
        RefreshFileList();
    }
    #endregion

    #region Return/Set file names
    public List<string> GetAllUserFileNames()
    {
        if (allUserFileNames == null)
        {
            return null;
        }
        List<string> saveFileList = new List<string>(allUserFileNames);
        return saveFileList;
    }

    public List<string> GetAllNonUserFileNames()
    {
        if (allNonUserFileNames == null)
        {
            return null;
        }
        List<string> saveFileList = new List<string>(allNonUserFileNames);
        return saveFileList;
    }

    public string GetUserID()
    {
        return userID;
    }

    public void SetUserID(string newUserID)
    {
        userID = newUserID;
    }

    public string GetKeyList()
    {
        return allSaveFileNames;
    }

    public string GetClassID()
    {
        return classID;
    }

    public void SetClassID(string newClassID)
    {
        classID = newClassID;
    }

    #endregion

    public float GetRecord()
    {
        return totalTime;
    }
    public int GetAttempts()
    {
        return totalAttempts;
    }
    //public float GetRecordFloat()
    //{
    //    float time = 0;
    //    char[] IDSaveSplitterChar = { ':' };
    //    string[] seperatedString = recordTime.Split(IDSaveSplitterChar);


    //    time = int.Parse(seperatedString[0]) * 60 + int.Parse(seperatedString[1]);
    //    Debug.Log(time);
    //    return time;
    //}
    private bool CheckIfFileEmpty()
    {
        bool fileEmpty = false;

        if(allSaveFileNames == "" || allSaveFileNames == null)
        {
            fileEmpty = true;
        }

        return fileEmpty;
    }

}
