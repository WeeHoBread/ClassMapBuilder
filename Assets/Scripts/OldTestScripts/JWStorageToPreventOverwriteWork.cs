using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JWStorageToPreventOverwriteWork : MonoBehaviour
{
    #region Challenge Script
    //[SerializeField] private List<string> challengeList;
    //private bool clearAllChallenge = false;
    //private int challengeIndex;
    //private PlayfabManager pFM;
    //private GameManager gm;
    //public void GetChallenge()
    //{
    //    challengeList = pFM.GetAllNonUserFileNames();
    //}

    //public void FindChallenge()
    //{
    //    if (challengeList.Count <= 0 && !clearAllChallenge)
    //    {
    //        Debug.Log("No challenge found");
    //        return;
    //    }

    //    else if (challengeList.Count <= 0 && clearAllChallenge)
    //    {
    //        challengeList = pFM.GetAllNonUserFileNames();
    //    }

    //    challengeIndex = UnityEngine.Random.Range(0, challengeList.Count);
    //    pFM.LoadSaveFile(challengeList[challengeIndex]);
    //}

    //public void StartChallenge()
    //{
    //    GridBuilder.Instance.ResetGrid();
    //    GridBuilder.Instance.LoadGrid(pFM.newlyReceivedFile);
    //    gm.isChallenge = true;
    //    gm.StartGame();
    //}
    #endregion

    #region Game Manager Changes for Challenge
    //public bool isChallenge;
    //void StateMachine()
    //{
    //    //At clear and fall
    //    if (isChallenge)
    //    {
    //        GridBuilder.Instance.ResetGrid();
    //        isChallenge = false;
    //    }
    //}
    #endregion
}
