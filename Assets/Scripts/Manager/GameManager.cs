using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static bool gamePaused = false;

    private gameState myState;
    public bool isPlaying = false;
    public bool isChallenge;
    public bool challengeClear;
    private Vector3 spawnPoint;
    private bool menuOpened;

    #region Reference to other objects and components
    private GameObject spawn;
    private GameObject end;
    private Text transition;
    private GameObject[] breakable;
    private GameObject[] laserTraps;
    private GameObject[] floorTraps;
    private GameObject[] keys;
    private PlayerMovement controls;
    public GameObject builder;
    public GameObject ghostGen;
    public GameObject buildCamera;
    public GameObject playCamera;
    public GameObject player;
    public GameObject blockageChecker;
    public GameObject clear;
    public GameObject pauseMenu;
    [SerializeField] private GameObject helpToolTip;

    //Grid references
    private GridBuilder gridBuilder;
    [SerializeField] private GameObject gridVisualHolder;
    [SerializeField] private GameObject gridWalls;
    private Transform gridTiles;

    //UI references
    private GameObject[] EditModeUI;
    private GameObject[] PlayModeUI;

    //Audio references
    private AudioManager audioManager;
    private AudioSource audioSource;
    #endregion

    //Time
    private float currentTime;
    private float totalTime = -1;
    private string timePassed;
    public string record;
    [SerializeField] private TextMeshProUGUI bestRecord;
    [SerializeField] private TextMeshProUGUI timeLapsed;
    private PlayfabManager pFM;
    enum gameState
    {
        Edit,
        Play,
        Clear,
        Fall
    }

    private void Awake()
    {
        #region Get References        
        audioManager = FindObjectOfType<AudioManager>();
        audioSource = audioManager.GetComponent<AudioSource>();
        EditModeUI = GameObject.FindGameObjectsWithTag("EditModeUI");
        PlayModeUI = GameObject.FindGameObjectsWithTag("PlayModeUI");
        transition = clear.GetComponentInChildren<Text>();
        controls = player.GetComponent<PlayerMovement>();
        gridBuilder = builder.GetComponent<GridBuilder>();
        pFM = FindObjectOfType<PlayfabManager>();
        gridTiles = gridVisualHolder.transform.GetChild(0);
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {   
        #region Get References
        gridVisualHolder.GetComponent<GridVisual>().GenerateGridVisual();
        #endregion
        GoToEditMode();

        SetInactiveEditModeUI();
        gridBuilder.enabled = false;
        ghostGen.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        //StartCoroutine(buttonPressCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        //if (bestTime ==-1)
        //{
        //    bestTime = 5999;

        //    record = "--:--";

        //    bestRecord.text = record;
        //}
        #region State Machine
        switch (myState)
        {
            case gameState.Edit:

                break;

            case gameState.Play:
                if (!gamePaused && currentTime < 5998)
                {
                    currentTime += Time.deltaTime;
                    int min = (int)currentTime / 60;
                    int sec = (int)currentTime % 60;
                    if (sec < 10)
                    {
                        timePassed = min + ":0" + sec;
                    }
                    else if (sec >= 10)
                    {
                        timePassed = min + ":" + sec;
                    }
                    timeLapsed.text = timePassed;
                }
                #region Check if Player has fell off
                if (player.transform.position.y < -50f || controls.isGameOver == true) //Check if player fell off
                {
                    PlayerDeath();
                    break;
                }
                #endregion

                #region Check if Player has cleared the stage
                if (controls.isClear == true) //Check if player cleared the stage
                {
                    transition.text = "CLEAR! \n Press 'Space' to go to edit mode";
                    FindObjectOfType<AudioManager>().PlayModeSound("Clear Game");
                    clear.SetActive(true);
                    Time.timeScale = 0;
                    foreach (GameObject pMU in PlayModeUI)
                    {
                        pMU.SetActive(false);
                    }
                    //isPlaying = false;
                    //if (currentTime < bestTime)
                    //{
                    //    bestTime = currentTime;
                    //    int min = (int)bestTime / 60;
                    //    int sec = (int)bestTime % 60;
                    //    if (sec < 10)
                    //    {
                    //        record = min + ":0" + sec;
                    //    }
                    //    else if (sec >= 10)
                    //    {
                    //        record = min + ":" + sec;
                    //    }
                    //    bestRecord.text = record;
                        if (isChallenge)
                        {
                        float add = pFM.GetRecord() + currentTime;
                            pFM.SaveGridSequence(pFM.targetFileName, GridBuilder.Instance.GetJson(), add);
                        }

                        //else if (!isChallenge)
                        //{
                        //    pFM.SaveGridSequence(pFM.GetUserID(), GridBuilder.Instance.GetJson(), record);
                        //}
                    //}
                    myState = gameState.Clear;
                    audioSource.Stop(); //stop music on clear
                    break;
                }
                #endregion

                #region Pause Game logics
                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (gamePaused)
                    {
                        ResumeGame();
                        FindObjectOfType<PlayerMovement>().EnableMovement();
                    }
                    else
                    {
                        PauseGame();
                        FindObjectOfType<PlayerMovement>().DisableMovement();
                    }
                }
                #endregion

                break;
            case gameState.Clear:
                {
                    if (isChallenge)
                    {
                        challengeClear = true;
                    }

                    #region Check input for continuing game after clear
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Time.timeScale = 1;
                        clear.SetActive(false);
                        controls.isClear = false;
                        spawn.SetActive(true);
                        GoToEditMode();
                        break;
                    }
                    break;
                    #endregion
                }
            case gameState.Fall:
                {
                    #region Check Input for returning to edit mode after fall
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Time.timeScale = 1;
                        clear.SetActive(false);
                        spawn.SetActive(true);
                        controls.isGameOver = false;
                        GoToEditMode();
                        break;
                    }
                    #endregion

                    #region Check input for continuing game after fall
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        ResetGame();
                        player.SetActive(false);
                        Time.timeScale = 1;
                        clear.SetActive(false);
                        controls.isGameOver = false;
                        spawn.SetActive(true);
                        StartGame();
                        break;
                    }
                    break;
                    #endregion
                }
        }
        #endregion


        //if (Input.GetKeyDown(KeyCode.P) && myState == gameState.Edit)
        //{
        //    StartGame();
        //}

        if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F4))
        {
            ExitGame();
        }

    }

    #region Switch States
    public void StartGame()
    {
        spawn = GameObject.FindGameObjectWithTag("Respawn");
        end = GameObject.FindGameObjectWithTag("Finish");

        if (menuOpened)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                DisplayToolTipText("Close Menu!");
                Timer.Create(HideToolTip, 2f, "Close Menu Timer"); //Hide warning after 2 seconds
            }
            else
            {
                DisplayToolTipText("Close Menu!");
                Timer.ResetTimer(HideToolTip, 2f, "Close Menu Timer"); //Reset Timer
            }
            return;
        }

        if (spawn != null && end != null)
        {
            //Debug.Log("end point is found?");
            //Debug.Log(end);
            breakable = GameObject.FindGameObjectsWithTag("Breakable");
            floorTraps = GameObject.FindGameObjectsWithTag("Pit");
            keys = GameObject.FindGameObjectsWithTag("Key");
            spawnPoint = spawn.transform.position;
            spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 1.0f, spawnPoint.z);
            end.transform.parent.transform.GetChild(0).gameObject.SetActive(true);

            player.transform.rotation = Quaternion.Euler(0, 180, 0);
            player.transform.position = spawnPoint;

            ghostGen.SetActive(false);
            buildCamera.SetActive(false);
            spawn.SetActive(false);

            //Find child of all grid walls and specifically turn OFF the renderer
            for (int x = 0; x < gridWalls.transform.childCount; x++)
            {
                gridWalls.transform.GetChild(x).transform.GetComponentInChildren<MeshRenderer>().enabled = false;
            }

            //Edit Mode UI stored in list, cycle through list to turn them ON
            foreach (GameObject eMU in EditModeUI)
            {
                eMU.SetActive(false);
            }

            //Play Mode UI stored in list, cycle through list to turn them ON
            foreach (GameObject pMU in PlayModeUI)
            {
                pMU.SetActive(true);
            }

            builder.GetComponent<GridBuilder>().InsertColliderAtEmptyGrid(); //Prevent Falling Off Empty Space
            builder.SetActive(false);
            gridTiles.gameObject.SetActive(false); //Do not turn off grid yet, need to be able to place platforms to prevent player from dropping
            player.SetActive(true);
            //player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            playCamera.transform.rotation = Quaternion.Euler(Vector3.zero);
            playCamera.SetActive(true);
            Time.timeScale = 1.0f;
            //player.GetComponent<CharacterController>().Move(new Vector3(0, 0, 0));
            myState = gameState.Play;
            isPlaying = true;

            laserTraps = GameObject.FindGameObjectsWithTag("Trap");
            for (int x = 0; x < laserTraps.Length; x++)
            {
                laserTraps[x].GetComponent<TrapRotation>().SetTrapActive();
            }

            helpToolTip.SetActive(false);

            PlayNextBGM(audioManager.playModeBGM);

        }
        else if (spawn == null)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                DisplayToolTipText("Build a Spawn Point!");
                Timer.Create(HideToolTip, 2f, "Build Spawn Timer"); //Hide warning after 2 seconds
            }
            else
            {
                DisplayToolTipText("Build a Spawn Point!");
                Timer.ResetTimer(HideToolTip, 2f, "Build Spawn Timer"); //Reset Timer
            }
            FindObjectOfType<AudioManager>().EditModeSound("Build Error");
        }
        else if (end == null)
        {
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                DisplayToolTipText("Build an End Point!");
                Timer.Create(HideToolTip, 2f, "Build End Timer"); //Hide warning after 2 seconds
            }
            else
            {
                DisplayToolTipText("Build an End Point!"); //Replace text if one already exist
                Timer.ResetTimer(HideToolTip, 2f, "Build End Timer"); //Reset Timer
            }
            FindObjectOfType<AudioManager>().EditModeSound("Build Error");
        }
    }

    public void GoToEditMode()
    {
        ResetGame();
        if (laserTraps != null)
        {
            for (int x = 0; x < laserTraps.Length; x++)
            {
                laserTraps[x].GetComponent<TrapRotation>().SetTrapInactive();
            }
        }

        if (spawn != null)
        {
            spawn.SetActive(true);
        }
        buildCamera.GetComponent<EditCamera>().ResetCamera();
        playCamera.SetActive(false);
        //player.GetComponent<IsometricMovement>().isGrounded = false;
        player.SetActive(false);

        //Edit Mode UI stored in list, cycle through list to turn them ON
        foreach (GameObject eMU in EditModeUI)
        {
            eMU.SetActive(true);
        }

        //Play Mode UI stored in list, cycle through list to turn them OFF
        foreach (GameObject pMU in PlayModeUI)
        {
            pMU.SetActive(false);
        }

        //Find child of all grid walls and specifically turn ON the renderer
        for (int x = 0; x < gridWalls.transform.childCount; x++)
        {
            gridWalls.transform.GetChild(x).transform.GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        builder.SetActive(true);
        ghostGen.SetActive(true);
        gridTiles.gameObject.SetActive(true);
        buildCamera.SetActive(true);
        isPlaying = false;
        breakable = null;

        gamePaused = false;
        pauseMenu.SetActive(false);

        myState = gameState.Edit;
        if (isChallenge)
        {
            //bestTime = 5999;

            record = "--:--";

            bestRecord.text = record;
            GridBuilder.Instance.ResetGrid();
            isChallenge = false;
        }
        PlayNextBGM(audioManager.editModeBGM);
    }

    private void SwitchOffAllMiscMenu()
    {
        EditModeUI = GameObject.FindGameObjectsWithTag("EditModeUI");
    }

    private void ResetGame()
    {
        if (laserTraps != null)
        {
            for (int x = 0; x < laserTraps.Length; x++)
            {
                laserTraps[x].GetComponent<TrapRotation>().SetTrapActive();
            }
        }

        if (breakable != null)
        {
            for (int i = 0; i < breakable.Length; i++)
            {
                breakable[i].gameObject.SetActive(true);
            }
        }
        if (floorTraps != null)
        {
            for (int x = 0; x < floorTraps.Length; x++)
            {
                floorTraps[x].GetComponent<FloorTrap>().Reset();
            }
        }
        if (keys != null)
        {
            for (int x = 0; x < keys.Length; x++)
            {
                keys[x].GetComponent<Key>().Reset();
            }
        }
        currentTime = 0.0f;
    }

    public void PlayerDeath()
    {
        transition.text = "YOU HAVE FALLEN! \n Press 'Space' to go to edit mode \n OR \n Press 'R' to try again";
        FindObjectOfType<AudioManager>().PlayModeSound("Fall Off");
        clear.SetActive(true);
        Time.timeScale = 0;
        foreach (GameObject pMU in PlayModeUI)
        {
            pMU.SetActive(false);
        }
        isPlaying = false;
        myState = gameState.Fall;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void ReturnToMainMenu()
    {
        //Remove if decided final product will not have main menu
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion

    public void LockInState()
    {
        menuOpened = true;
    }

    public void UnlockState()
    {
        menuOpened = false;
    }

    private void PlayNextBGM(AudioClip nextAudio)
    {
        audioSource.Stop(); //Stop current music
        audioSource.clip = nextAudio; //swap out for next audio
        audioSource.Play(); //Play next audio
    }

    public void SetInactiveEditModeUI()
    {
        foreach (GameObject eMU in EditModeUI)
        {
            eMU.SetActive(false);
        }
    }

    public void SetActiveEditModeUI()
    {
        foreach (GameObject eMU in EditModeUI)
        {
            eMU.SetActive(true);
        }
    }

    public void SetRecord(int tries, float time)
    {

        totalTime = time;
        if (tries == 0)
        {
            record = "--:--";

            bestRecord.text = record;
            return;
        }
        float average = totalTime / tries;

        int min = (int)average / 60;
        int sec = (int)average % 60;
        if (sec < 10)
        {
            record = min + ":0" + sec;
        }
        else if (sec >= 10)
        {
            record = min + ":" + sec;
        }
        bestRecord.text = record;
    }
    #region Tooltip display
    public void DisplayToolTipText(string newText)
    {
        //tool tip test
        System.Func<string> getTooltipNextFunc = () =>
        {
            return newText;
        };
        Tooltip.ShowTooltip_Static(getTooltipNextFunc);
    }

    public void HideToolTip()
    {
        Tooltip.HideTooltip_Static();
    }
    #endregion

}
