using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeLoad : MonoBehaviour
{
    private PlayfabManager pfm;
    [SerializeField] private List<string> nonUsersavefilenameList;

    // Start is called before the first frame update
    void Start()
    {
        pfm = FindObjectOfType<PlayfabManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadChallengeLevel()
    {
        pfm.RefreshFileList(); //Prob need to run this all the way in pfm
        nonUsersavefilenameList = pfm.GetAllNonUserFileNames();
    }

}
