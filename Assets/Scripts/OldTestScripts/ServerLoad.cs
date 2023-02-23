using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;


public struct Data
{
    public string Name; //Correspond 
    public string ImageURL;
}

public class ServerLoad : MonoBehaviour
{
    //[SerializeField] Text uiNameText;
    //[SerializeField] RawImage uiRawImage;

    //private string jsonURL = "https://drive.google.com/uc?export=download&id=1IRY3XpSWA25_lHKM-Uy0P1IlJGLKZQjo";

    //// Start is called before the first frame update
    //void Start()
    //{
    //    StartCoroutine(GetData(jsonURL));
    //}

    //IEnumerator GetData(string url)
    //{
    //    UnityWebRequest webRequest = UnityWebRequest.Get(url);

    //    yield return webRequest.Send();

    //    if (webRequest.isError)
    //    {

    //    }
    //    else // Success
    //    {
    //        Data data = JsonUtility.FromJson<Data>(webRequest.downloadHandler.text);

    //        //Print data into the UI
    //        uiNameText.text = data.Name;

    //        //Load Image
    //        StartCoroutine(GetImage(data.ImageURL));
    //    }

    //    webRequest.Dispose(); //Clean web request after it is no longer required
    //}

    //IEnumerator GetImage(string url)
    //{
    //    UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

    //    yield return webRequest.Send();

    //    if (webRequest.isError)
    //    {

    //    }
    //    else // Success
    //    {
    //        uiRawImage.texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
    //    }

    //    webRequest.Dispose(); //Clean web request after it is no longer required

    //}


    //IEnumerator SetData(string url)
    //{
    //    UnityWebRequest webRequest = UnityWebRequest.Get(url);

    //    yield return webRequest.Send();

    //    if (webRequest.isError)
    //    {

    //    }
    //    else // Success
    //    {
    //        Data data = new Data();
    //        data.Name = "Save test";
    //        //JsonUtility.FromJson<Data>(webRequest.uploadHandler.data) = data;
    //        //Data data = JsonUtility.FromJson<Data>(webRequest.downloadHandler.text);
    //    }

    //    webRequest.Dispose(); //Clean web request after it is no longer required
    //}

}
