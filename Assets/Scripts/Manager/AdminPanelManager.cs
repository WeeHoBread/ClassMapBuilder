using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class AdminPanelManager : MonoBehaviour
{
    [SerializeField]
    private string adminEmail;
    [SerializeField]
    private string adminPassword;
    [SerializeField]
    private KeyCode[] adminKeyBind;

    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;

    public GameObject loginPanel;
    public GameObject adminPanel;
    public GameObject classRowPanel;
    public GameObject classRowTab;

    public GameObject classRowPageTemplate;

    public PlayfabManager pfM;

    public Color defaultColor = new Color(1f,1f,1f,0f);
    public float hover = 25f;
    public float click = 200f;
    private string currentSelectedRow;

    //Page Manager
    float studentsPerPage = 6f;
    int currentPage = 1;
    int totalPages = 0;
    GameObject currentPageGameObject;

    public bool CompareValues(string _val1, string _val2)
    {
        return _val1 == _val2;
    }

    public void ConfirmButtonDown()
    {

        if (emailInputField.text.Length == 0 || passwordInputField.text.Length == 0)
        {
            return;
        }

        string lowerEmail = emailInputField.text;
        string lowerPassword = passwordInputField.text;

        print("Login Attempt : " + lowerEmail + "/" + lowerPassword);

        if (!(CompareValues(lowerEmail, adminEmail) && CompareValues(lowerPassword, adminPassword)))
        {
            return;
        }

        print("Login Successful!");
        ToggleAdminPanel(true);
    }

    public void ConfirmedDeletion()
    {
        //pfM.DeleteFile();
    }

    public void CreatePages(int _count)
    {
        totalPages = _count;
        for (int i=1; i <= _count; i++)
        {
            GameObject newPage = Instantiate(classRowPageTemplate, classRowPageTemplate.transform.position, Quaternion.identity);
            newPage.name = "Page-" + i;
            newPage.transform.SetParent(classRowPanel.transform);
            newPage.transform.localScale = new Vector3(1, 1, 1);

            if (i == 1)
            {
                newPage.SetActive(true);
                currentPageGameObject = newPage;
            }
        }
    }

    public void GetStudentsFromPlayfabManager()
    {
        string students = pfM.GetKeyList();

        print(students);
        string[] studentInfo = students.Split("|");

        int pageCount = Mathf.CeilToInt((studentInfo.Length - 1)/ studentsPerPage);
        CreatePages(pageCount);

        print(studentInfo.Length / studentsPerPage);
        print("#Students : " + (studentInfo.Length - 1) + "/" + pageCount + " Pages");

        float counter = 0f;
        foreach (string _student in studentInfo)
        {
            if (_student.Length > 0)
            {
                counter = counter + 1f;
                GameObject pageFound = classRowPanel.transform.GetChild(Mathf.CeilToInt(counter/studentsPerPage) - 1).gameObject;
                string _class = _student.Split("_")[0];
                string _id = _student.Split("_")[1];

                CreateClassTabs(_class, _id, pageFound);
            }

        }
    }

    public void ChangePage(int _direction)
    {
        print("Page Change Direction : " + _direction);
        int nextPage = currentPage + _direction;

        if (nextPage == 0)
        {
            nextPage = totalPages;
        }

        if (nextPage == totalPages + 1)
        {
            nextPage = 1;
        }

        currentPage = nextPage;
        print("Next Page : " + currentPage);

        for (int i=0; i < classRowPanel.transform.childCount; i++)
        {
            GameObject _page = classRowPanel.transform.GetChild(i).gameObject;
            if (_page.name == "Page-" + currentPage)
            {
                _page.SetActive(true);
                currentPageGameObject = _page;
            } else
            {
                _page.SetActive(false);
            }
        }
    }

    /*
    public void GetClassesFromPlayfabManager()
    {
        string classId = pfM.GetClassID();
        print(classId);
        Dictionary<string, List<string>> _data = pfM.GetClassData();

        foreach (var _dict in _data)
        {
            CreateClassTabs(_dict.Key, pfM.GetStudentsFromClass(_dict.Value));
        }
    }*/

    private void CreateClassTabs(string _key, string _count, GameObject _page)
    {
        GameObject newClassRow = Instantiate(classRowTab, classRowTab.transform.position, Quaternion.identity);
        newClassRow.transform.SetParent(_page.transform);
        newClassRow.transform.localScale = new Vector3(1, 1, 1);
        newClassRow.SetActive(true);

        newClassRow.name = _key + "_" + _count;
        newClassRow.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _key;
        newClassRow.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _count;
    }

    public void PanelDataHover(string _class, bool _state)
    {
        //print("Hovering");
        for (int i = 0; i < (currentPageGameObject.transform.childCount); i++)
        {
            GameObject dataRow = currentPageGameObject.transform.GetChild(i).gameObject;
            Image dataRowImage = dataRow.GetComponent<Image>();
            if (dataRow.name != currentSelectedRow)
            {
                if (dataRow.name == _class && _state)
                {
                    dataRowImage.color = new Color(dataRowImage.color.r, dataRowImage.color.g, dataRowImage.color.b, hover / 255);
                }
                else
                {
                    dataRowImage.color = defaultColor;
                }
            }

        }
    }

    public void DeleteSelectedPanel()
    {
        print("Deleting Class : " + currentSelectedRow);
        string[] splitData = currentSelectedRow.Split("_");
        print(splitData[0] + "/" + splitData[1]);
        pfM.DeleteFile(splitData[0],splitData[1]);

        Destroy(GameObject.Find(currentSelectedRow));
        //pfM.AttemptFix();
        //pfM.DeleteFile("P01", "2004290I"); //This is for deleting individual students
        //pfM.DeleteEntireClass(currentSelectedRow);

        /*
        GameObject deletedPanel = classRowPanel.transform.Find(currentSelectedRow).gameObject;

        if (deletedPanel)
        {
            print("Successfully Destroyed : " + currentSelectedRow);
            Destroy(deletedPanel);
            currentSelectedRow = "";
        }*/
    }

    public void PanelDataClicked(string _class)
    {
        for (int i=0; i < (currentPageGameObject.transform.childCount); i++)
        {
            GameObject dataRow = currentPageGameObject.transform.GetChild(i).gameObject;
            Image dataRowImage = dataRow.GetComponent<Image>();

            if (dataRow.name == _class)
            {
                dataRowImage.color = new Color(dataRowImage.color.r, dataRowImage.color.g, dataRowImage.color.b, click/255);
                currentSelectedRow = dataRow.name;
            } else
            {
                dataRowImage.color = defaultColor;
            }
        }
    }

    private void ToggleAdminPanel(bool _state)
    {
        adminPanel.SetActive(_state);
        loginPanel.SetActive(!_state);

        if (adminPanel.activeSelf)
        {
            OnAdminPanelOpen();
        }
    }

    private void OnAdminPanelOpen()
    {
        print("Launching Admin Panel...");
        //GetClassesFromPlayfabManager();
        GetStudentsFromPlayfabManager();
    }

    public void CloseAllAdminPanels()
    {
        print("Closing Panels...");
        adminPanel.SetActive(false);
        loginPanel.SetActive(false);
    }

    private bool GetKeysDown()
    {
        int checker = 0;
        for (int i = 0; i < adminKeyBind.Length; i++)
        {
            if (Input.GetKey(adminKeyBind[i]))
            {
                checker++;
            }
        }

        //print(checker);
        return checker == adminKeyBind.Length;
    }

    private void Update()
    {
        if (!GetKeysDown())
        {
            return;
        }

        if (!adminPanel.activeSelf)
        {
            ToggleAdminPanel(false);
        }
    }

}
