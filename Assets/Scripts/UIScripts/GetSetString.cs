using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetSetString : MonoBehaviour
{
    //private Text textComp;
    //private TextMeshPro TMP;
    private TextMeshProUGUI tmpComp;


    // Start is called before the first frame update
    void Start()
    {
        //if (GetComponent<Text>() != null)
        //{
        //    textComp = GetComponent<Text>();
        //}

        if (GetComponent<TextMeshProUGUI>() != null)
        {
            tmpComp = GetComponent<TextMeshProUGUI>();
        }
    }

    //public void GiveStringTxtToTxt(string targetObjectName)
    //{
    //    Transform targetReceiver = GameObject.Find(targetObjectName).transform;
    //    targetReceiver.GetComponent<Text>().text = textComp.text;
    //}

    //public void GiveStringTxtToTMP(string targetObjectName)
    //{
    //    Debug.Log("test");
    //    GameObject targetReceiver = GameObject.Find(targetObjectName);
    //    targetReceiver.GetComponent<TextMeshProUGUI>().text = textComp.text;
    //}

    //public void GiveStringTMPtoTxt(string targetObjectName)
    //{        
    //    Transform targetReceiver = GameObject.Find(targetObjectName).transform;
    //    string newText = tmpComp.text;
    //    targetReceiver.GetComponent<Text>().text = newText;
    //}

    public void GiveStringTMPtoInputField(string targetObjectName)
    {
        Transform targetReceiver = GameObject.Find(targetObjectName).transform;
        targetReceiver.GetComponent<InputField>().text = tmpComp.text;
    }

}
