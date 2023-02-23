using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public struct SaveData
{
    public string Name;

}


public class ServerSave : MonoBehaviour
{
    //private GridBuilder gb;
    //public string UserID = "User01";
    //public string saveFileName;
    //public string dummyTxtFile;


    //[SerializeField] private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdlCDFg0ZQzMNsAHocHXK47PP7n1V1E8rd25JqaZLquRvSJjQ/formResponse";


    //private void Awake()
    //{
    //    gb = GameObject.Find("BuildGrid").GetComponent<GridBuilder>();
    //}


    //public void SendData()
    //{
    //    saveFileName = gb.saveName;
    //    dummyTxtFile = gb.GetJson();
    //    StartCoroutine(InsertData());
    //    //StartCoroutine(GetData(BASE_URL));
    //}


    //private IEnumerator InsertData()
    //{
    //    WWWForm dataForm = new WWWForm();

    //    dataForm.AddField("entry.23850225", UserID);
    //    dataForm.AddField("entry.130601508", saveFileName);
    //    dataForm.AddField("entry.1265869126", dummyTxtFile);
        

    //    byte[] rawData = dataForm.data;

    //    WWW www = new WWW(BASE_URL, rawData); // there is a replacement for www, still figuring out how to not use obsolete code
    //    //UnityWebRequest webRequest = UnityWebRequest.Post(BASE_URL, rawData);

    //    yield return www;
    //}

    //[System.Obsolete]
    //IEnumerator GetData(string url)
    //{
    //    UnityWebRequest webRequest = UnityWebRequest.Get(url);

    //    yield return webRequest.Send();

    //    if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
    //    {

    //    }
    //    else // Success
    //    {
    //        SaveData data = JsonUtility.FromJson<SaveData>(webRequest.downloadHandler.text);

    //        //Print data into the UI
    //        UserID = data.Name;
    //    }

    //    webRequest.Dispose(); //Clean web request after it is no longer required
    //}


}
