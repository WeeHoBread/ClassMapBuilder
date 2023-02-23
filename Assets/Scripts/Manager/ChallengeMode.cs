using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeMode : MonoBehaviour
{
    [SerializeField] private List<string> challengeList;
    [SerializeField] private GameObject challengeMenu;
    private bool clearAllChallenge = false;
    private int challengeIndex;
    private int lastChallenge;
    private List<int> cleared;
    private PlayfabManager pFM;
    private GameManager gm;
    //private bool isInit;

    public void GetChallenge()
    {
        StartCoroutine(FindChallenge());
    }

    private IEnumerator FindChallenge()
    {
        //if (!isInit)
        //{
        //    GetChallenge();
        //    if (!isInit)
        //    {
        //        return;
        //    }
        //}
        int oldCount = 0;
        pFM.loading = true;
        pFM.GetSaveFileNames();

        yield return new WaitUntil(() => pFM.loading == false);
        if (challengeList != null)
        {
            oldCount = challengeList.Count;
        }
        pFM.ConvertAndSplitFileNames();
        challengeList = pFM.GetAllNonUserFileNames();

        if (challengeList == null)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                gm.DisplayToolTipText("No challenge available at the moment");
                Timer.Create(gm.HideToolTip, 2f, "EmptyChallenge"); //Hide warning after 2 seconds
            }
            else
            {
                gm.DisplayToolTipText("No challenge available at the moment");
                Timer.ResetTimer(gm.HideToolTip, 2f, "EmptyChallenge"); //Hide warning after 2 seconds
            }
            yield break;
        }

        if (!clearAllChallenge && gm.challengeClear)
        {
            cleared.Add(challengeIndex);
            gm.challengeClear = false;

            if (challengeList.Count <= cleared.Count)
            {
                clearAllChallenge = true;
            }
        }
        if (challengeList.Count > oldCount)
        {
            clearAllChallenge = false;
        }

        //if (challengeList.Count <= 0 && !clearAllChallenge)
        //{
        //        if (!Tooltip.Instance.isActiveAndEnabled)
        //        {
        //            gm.DisplayToolTipText("No challenge available at the moment");
        //            Timer.Create(gm.HideToolTip, 2f, "EmptyChallenge"); //Hide warning after 2 seconds
        //        }
        //        else
        //        {
        //            gm.DisplayToolTipText("No challenge available at the moment");
        //            Timer.ResetTimer(gm.HideToolTip, 2f, "EmptyChallenge"); //Hide warning after 2 seconds
        //        }
        //        return;
        //}


        if (challengeList.Count > cleared.Count)
        {
            do
            {
                challengeIndex = UnityEngine.Random.Range(0, challengeList.Count);
            } while (CheckRepeat(challengeIndex));
        }
        else if (challengeList.Count == 1)
        {
            challengeIndex = 0;
        }
        else if (clearAllChallenge && challengeList.Count > 1 && challengeList.Count <= cleared.Count)
        {
            do
            {
                challengeIndex = UnityEngine.Random.Range(0, challengeList.Count);
            } while (challengeIndex == lastChallenge);
        }
        pFM.LoadSaveFile(challengeList[challengeIndex]);
        challengeMenu.SetActive(true);
    }

    public void StartChallenge()
    {
        StartCoroutine(LoadChallenge());
    }

    private IEnumerator LoadChallenge()
    {
        GridBuilder.Instance.LoadGrid(pFM.newlyReceivedFile);
        gm.SetRecord(pFM.GetAttempts(), pFM.GetRecord());
        yield return new WaitForSeconds(Time.deltaTime);
        lastChallenge = challengeIndex;
        gm.isChallenge = true;
        gm.StartGame();

    }

    private void Awake()
    {
        pFM = FindObjectOfType<PlayfabManager>();
        gm = FindObjectOfType<GameManager>();
        cleared = new List<int>();
    }

    private bool CheckRepeat(int next)
    {
        for (int i = 0; i < cleared.Count; i++)
        {
            if(next == cleared[i])
            {
                return true;
            }
        }
        return false;
    }
}
