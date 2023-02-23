using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GridBuilder : MonoBehaviour
{
    public static GridBuilder Instance { get; private set; }

    #region Placable Objects variables
    //For empty grid
    public GameObject emptySpace;

    #region Saving UI Component
    //For saving UI
    public GameObject resetWarning;
    public string json;
    #endregion

    //List of other objects that can be placed    
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    [SerializeField] private List<PlacedObjectTypeSO> placedEnemySOList;

    //Reference to the object selected for placement
    private PlacedObjectTypeSO placedObjectTypeSO;

    //To Store direction we are building in
    public PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;
    #endregion

    #region Grid/Placement References
    //Reference to the grid
    public GridCustom<GridObject> grid;

    //Reference to which object variant to be placed
    private int objectVariantNo;
    private int objectMaxVariant;
    #endregion

    #region Grid Attributes
    [Header("Grid Attributes")]
    public int gridWidth;
    public int gridHeight;
    private float cellSize = 10f;
    private int gridState = 2;
    private Vector3 gridOffset; // Use to keep center of grid in coordinate 0,0,0
    [SerializeField] private bool buildingDisabled; //SerializeField for debugging
    #endregion

    #region One Instance Objects variables    
    //Spawn Point Storage
    [HideInInspector]
    public bool spawnPointPlaced = false;
    [HideInInspector]
    public Vector3 spawnPointLocation;

    //End Point Storage
    [HideInInspector]
    public bool endPointPlaced = false;
    [HideInInspector]
    public Vector3 endPointLocation;
    #endregion

    #region Events
    //public event EventHandler OnLoaded;
    public event EventHandler OnSelectedChanged;
    public event EventHandler OnVariantChanged;
    public event EventHandler OnObjectPlaced;
    public event EventHandler OnBuildable;
    public event EventHandler OnBuildError;
    #endregion

    #region Other Componenent Reference
    private ScrollRectMovement scrollRectMove;
    private GameManager gm;
    private GameObject objectGhost;
    private GridVisual gv;
    //Reference to commandBar
    private GameObject commandBar01;
    private GameObject commandBar02;
    private int commandBarContent01No;
    [SerializeField] private GameObject changeSize;
    [SerializeField] private TMP_InputField lengthInput;
    [SerializeField] private TMP_InputField widthInput;
    private int newGridWidth;
    private int newGridLength;
    //private int commandBarContent02No;  //Not in use at the moment, but might be needed if there is going to be commandBar03  
    #endregion


    private void Awake()
    {
        Instance = this;

        #region Grid Set-up
        gridOffset = new Vector3((gridWidth * (int)cellSize) / 2, 0, (gridHeight * (int)cellSize) / 2);

        grid = new GridCustom<GridObject>(gridWidth, gridHeight, cellSize, -gridOffset,
            (GridCustom<GridObject> g, int x, int z) => new GridObject(g, x, z));

        //Setting grids to be empty
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                grid.GetGridObject(x, z).EmptyGrid();
            }
        }
        #endregion

        #region Array/List & Object Set-up 
        //Set default placement object
        placedObjectTypeSO = placedObjectTypeSOList[gridState];
        RefreshSelectedObjectType();
        #endregion

        AquireReferences();
    }


    private void Start()
    {
        objectGhost = GameObject.Find("GhostPointer");
        //GenerateGridVisual();
    }

    public class GridObject
    {
        private GridCustom<GridObject> grid;
        private int x;                          //Grid X
        private int z;                          //Grid Z
        private int gridState;                  //Grid Object
        private int objVar;                     //Object Variant
        private PlacedObjectTypeSO.Dir objDir;  //Rotation of place object
        private PlacableObjects placedObject;

        public GridObject(GridCustom<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        #region Set/Get Placable Object
        public void SetPlacableObject(PlacableObjects placedObject, int gridState, int objVar, PlacedObjectTypeSO.Dir dir)
        {
            this.placedObject = placedObject;
            this.gridState = gridState;
            this.objVar = objVar;
            this.objDir = dir;
            grid.TriggerGridObjectChanged(x, z);
        }

        //Get the object placed on specific grid
        public PlacableObjects GetPlacedObject()
        {
            return placedObject;
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            this.gridState = -1;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void EmptyGrid()
        {
            this.gridState = -1;
        }

        #endregion
        public bool CanBuildObject()
        {
            return placedObject == null; //return true if transform == null
        }

        public int GetGridState()
        {
            return gridState;
        }

        public override string ToString()
        {
            return x + ", " + z + "\n" + placedObject;
        }

        #region Saving/loading grid
        [System.Serializable]
        public class SaveGridObject
        {
            //Variables to save
            public int x;                       //Grid X
            public int z;                       //Grid Z
            public int gridState;               //Grid Object
            public int objVar;                  //Object Variant
            public PlacedObjectTypeSO.Dir dir;  //Rotation of place object
        }

        public SaveGridObject Save()
        {
            return new SaveGridObject
            {
                x = x,
                z = z,
                gridState = gridState,
                objVar = objVar,
                dir = objDir
            };

        }

        public void LoadGrid(SaveGridObject saveObject)
        {
            gridState = saveObject.gridState;
            objVar = saveObject.objVar;
            objDir = saveObject.dir;
        }
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        #region SaveLoad Input
            //if (Input.GetKeyDown(KeyCode.O)) //Leaving here just in case, remove at near the end of the project
            //{
            //    if (saving.activeSelf != true)
            //    {
            //        saving.SetActive(true);
            //    }
            //    else
            //    {
            //        saving.SetActive(false);
            //    }           
            //}

            //if (saving.activeSelf == true)
            //{
            //    objectGhost.SetActive(false);
            //    saveName = saveFile.text;
            //    if (Input.GetKeyDown(KeyCode.Escape))
            //    {
            //        saving.GetComponent<SaveMenuPlus>().CloseMenu();
            //    }
            //    return;
            //}
        if (!Input.GetKey(KeyCode.LeftControl) && !buildingDisabled && !objectGhost.activeSelf)
        {
            objectGhost.SetActive(true);
        }

        if (resetWarning.activeSelf == true)
        {
            objectGhost.SetActive(false);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                resetWarning.SetActive(false);
            }
            return;
        }
        else if (!Input.GetKey(KeyCode.LeftControl) && !buildingDisabled && !objectGhost.activeSelf)
        {
            objectGhost.SetActive(true);
        }

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    saveName = lastSave;
        //    revert.SetActive(true);
        //}
        #endregion

        //Disable building code when building is disabled, usually when menu is open
        if (buildingDisabled)
        {
            if (objectGhost.activeSelf)
            {
                objectGhost.SetActive(false);
                //Debug.Log("update setting inactive");
            }
            return;
        }

        #region Reset Grid
        if (Input.GetKeyDown(KeyCode.B)) //Resets grid to nothing
        {
            resetWarning.SetActive(true);
            //ResetGrid();
            //FindObjectOfType<AudioManager>().EditModeSound("Clear Grid");
        }
        #endregion

        #region Go through different inputs for placable objects   
        ////Input to change object variant
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    objectVariantNo++;
        //    if (objectVariantNo >= objectMaxVariant)
        //    {
        //        objectVariantNo = 0;
        //    }

        //    placedObjectTypeSO.SetNextActiveChild(objectVariantNo);
        //    RefreshVariantValues();
        //    FindObjectOfType<AudioManager>().EditModeSound("Variant Change");

        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    RotateSelectedObject();
        //}

        //Inputs to change object type
        //if (Input.GetKeyDown(KeyCode.Alpha1)) { gridState = 0; ChangeSelectedObject(gridState); }
        //if (Input.GetKeyDown(KeyCode.Alpha2)) { gridState = 1; ChangeSelectedObject(gridState); }
        //if (Input.GetKeyDown(KeyCode.Alpha3)) { gridState = 2; ChangeSelectedObject(gridState); } //Change to Straight Wall
        //if (Input.GetKeyDown(KeyCode.Alpha4)) { gridState = 3; ChangeSelectedObject(gridState); } //Change to Corner Wall
        //if (Input.GetKeyDown(KeyCode.Alpha5)) { gridState = 4; ChangeSelectedObject(gridState); } //Change to T-Wall
        //if (Input.GetKeyDown(KeyCode.Alpha6)) { gridState = 5; ChangeSelectedObject(gridState); } //Change to Junction Wall
        //if (Input.GetKeyDown(KeyCode.Alpha7)) { gridState = 6; ChangeSelectedObject(gridState); } //Change to Platform
        //if (Input.GetKeyDown(KeyCode.Alpha8)) { gridState = 7; ChangeSelectedObject(gridState); }
        #endregion

        //Code stops if pointer is over canvas and ctrl button is being held
        //When ctrl button is held, camera movement is active, disable build when moving camera with mouse
        if (EventSystem.current.IsPointerOverGameObject() || Input.GetKey(KeyCode.LeftControl))
        {
            return;
        }

        #region Functions available when unbuildable
        if (!CheckBuildable(MousePos3D.GetMousePositition3D())) //Runnable statement for when hovered position is not buildable
        {
            OnBuildError?.Invoke(this, EventArgs.Empty); //Call build error function for Objectghost to display correct visual

            //Display error if player try to build on unbuildable area
            if (Input.GetMouseButtonDown(0))
            {
                BuildError(true); //Build Error Play Sound
            }

            //Clear on right click
            if (Input.GetMouseButtonDown(1))
            {
                ClearObjectAtCoordinate(MousePos3D.GetMousePositition3D(), true);
            }
            return;
        }
        #endregion

        OnBuildable?.Invoke(this, EventArgs.Empty); //Call build error function for Objectghost to display correct visual
        //Placement on Left Click
        if (Input.GetMouseButtonDown(0))
        {
            BuildAtPosition(MousePos3D.GetMousePositition3D());
        }
    }
    private void FixedUpdate()
    {         
        //Check keys for fast placement
        if (EventSystem.current.IsPointerOverGameObject() || Input.GetKey(KeyCode.LeftControl)
            || !Input.GetKey(KeyCode.LeftShift) || buildingDisabled)
        {
            return;
        }

        #region Functions available when unbuildable
        //Check if Area is Buildable
        if (!CheckBuildable(MousePos3D.GetMousePositition3D()))
        {
            OnBuildError?.Invoke(this, EventArgs.Empty); //Call build error function for Objectghost to display correct visual

            //Fast Placement on left click + left shift
            if (Input.GetMouseButton(0))
            {
                BuildError(false); //Build Error don't play sound
            }

            //Clear on right click + left shift
            if (Input.GetMouseButton(1))
            {
                ClearObjectAtCoordinate(MousePos3D.GetMousePositition3D(), true);
            }
            return;
        }
        #endregion

        //Fast Placement on left click + left shift
        if (Input.GetMouseButton(0))
        {
            BuildAtPosition(MousePos3D.GetMousePositition3D());
        }
    }

    #region Build/Change Grid Items

    //NOTE -----------------------------------------------------------------------------------------------------------
    public void ChangeSelectedObject(int index) //Code required to change the selected object
    {
        gridState = index; //this line is a problem when swapping list
        placedObjectTypeSO = placedObjectTypeSOList[index];

        //Take into account the index so that it counts from 0 regardless of active command bar
        if (commandBar01.activeSelf)
        {
            scrollRectMove = GameObject.Find("ObjectCommandBar_Building").GetComponent<ScrollRectMovement>();
            scrollRectMove.ScrollToPosition(index);
        }
        else if(commandBar02.activeSelf)
        {
            scrollRectMove = GameObject.Find("ObjectCommandBar_Dangers").GetComponent<ScrollRectMovement>();
            scrollRectMove.ScrollToPosition(index - commandBarContent01No);
        }
           
        
        
        RefreshSelectedObjectType();
        dir = PlacedObjectTypeSO.Dir.Down;
        FindObjectOfType<AudioManager>().EditModeSound("Change Object");
    }

    public void RotateSelectedObject()
    {
        dir = PlacedObjectTypeSO.GetNextDir(dir);
        FindObjectOfType<AudioManager>().EditModeSound("Rotate");
    }

    private void BuildAtPosition(Vector3 position)
    {
        grid.GetXY(position, out int x, out int z); //Get coordinate of the grid base on mouse position clicked

        //The grid position that this object will occupy
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);
        Vector2Int placedObjectOrigin = new Vector2Int(x, z);
        placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);

        #region Placement Code
        Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
        Vector3 placedObjectWorldPos = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y)
                          + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

        PlacableObjects placedObject = PlacableObjects.Create(placedObjectWorldPos, new Vector2Int(x, z), dir, placedObjectTypeSO);

        foreach (Vector2Int gridPosition in gridPositionList)
        {
            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacableObject(placedObject, gridState, objectVariantNo, dir);
        }

        //Checked if placed object is a spawn point or end point, if check what object it is and delete its old position
        if (placedObject.placedObjectTypeSO.isOneInstanceObject())
        {
            ResetOneInstanceObject(placedObject, position);
        }

        //Events
        OnObjectPlaced?.Invoke(this, EventArgs.Empty);

        FindObjectOfType<AudioManager>().EditModeSound("Build");
        #endregion
    }

    private bool CheckBuildable(Vector3 position)
    {
        grid.GetXY(position, out int x, out int z); //Get coordinate of the grid base on mouse position clicked

        //The grid position that this object will occupy
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);

        bool canBuild = true;

        //Check through each grid that the object being place would take
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            //get reference to to grid coordinate
            GridObject tempGO = grid.GetGridObject(gridPosition.x, gridPosition.y);

            //if any grid returns false on CanBuild method, the object cannot be placed
            if (tempGO != null)
            {
                if (!tempGO.CanBuildObject())
                {
                    canBuild = false;
                    break;
                }
            }
            else
            {
                canBuild = false;
            }
        }

        return canBuild;
    }

    private void BuildError(bool playSound)
    {
        if (playSound == true)
        {
            FindObjectOfType<AudioManager>().EditModeSound("Build Error");

            //Check if warning is already displayed, if not display warning
            if (!Tooltip.Instance.isActiveAndEnabled)
            {
                DisplayWarning("You cannot build there");
                Timer.Create(HideWarning, 2f, "Build Warning Timer"); //Hide warning after 2 seconds
            }
            else //If warning is already displayed, reset its timer                
            {
                DisplayWarning("You cannot build there"); //Change text if there is a text already there
                Timer.ResetTimer(HideWarning, 2, "Build Warning Timer");
            }

        }
    }

    public int GetCurrentVariantIndex()
    {
        return objectVariantNo;
    }
    #endregion

    #region Resetting One Instance Object
    public void RemoveOneInstanceObject(PlacableObjects placedObject)
    {
        if (placedObject.placedObjectTypeSO.GetObjectType() == PlacedObjectTypeSO.ObjectType.SpawnPoint)
        {
            spawnPointPlaced = false;
            spawnPointLocation = Vector3.zero;
        }
        else if (placedObject.placedObjectTypeSO.GetObjectType() == PlacedObjectTypeSO.ObjectType.EndPoint)
        {
            endPointPlaced = false;
            endPointLocation = Vector3.zero;
        }
    }

    private void ResetOneInstanceObject(PlacableObjects placableObjects, Vector3 objectNewPosition)
    {
        if (placableObjects.placedObjectTypeSO.GetObjectType() == PlacedObjectTypeSO.ObjectType.SpawnPoint)
        {
            if (spawnPointPlaced == true)
            {
                ClearObjectAtCoordinate(spawnPointLocation, false);
            }

            spawnPointLocation = objectNewPosition;
            spawnPointPlaced = true;
        }
        else if (placableObjects.placedObjectTypeSO.GetObjectType() == PlacedObjectTypeSO.ObjectType.EndPoint)
        {
            if (endPointPlaced == true)
            {
                ClearObjectAtCoordinate(endPointLocation, false);
            }
            endPointLocation = objectNewPosition;
            endPointPlaced = true;
        }
    }

    #endregion

    #region Clear/ResetGridValues
    private void ClearObjectAtCoordinate(Vector3 targetCoordinate, bool playSound)
    {
        GridObject gridObject = grid.GetGridObject(targetCoordinate);
        if (gridObject != null)
        {
            PlacableObjects placedObject = gridObject.GetPlacedObject();

            //if there is an object there
            if (placedObject != null)
            {
                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }

                //Check for one instance object
                if (placedObject.placedObjectTypeSO.isOneInstanceObject())
                {
                    RemoveOneInstanceObject(placedObject);
                }
                placedObject.DestroySelf();

                if (playSound == true)
                {
                    FindObjectOfType<AudioManager>().EditModeSound("Remove");
                }

            }
        }
    }

    private void ClearObjectAtCoordinate(int xCor, int zCor)
    {
        GridObject gridObject = grid.GetGridObject(xCor, zCor);
        if (gridObject != null)
        {
            PlacableObjects placedObject = gridObject.GetPlacedObject();

            //if there is an object there
            if (placedObject != null)
            {
                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }

                //Check for one instance object
                if (placedObject.placedObjectTypeSO.isOneInstanceObject())
                {
                    RemoveOneInstanceObject(placedObject);
                }
                placedObject.DestroySelf();
            }
        }
    }

    public void InsertColliderAtEmptyGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GridObject gridObject = grid.GetGridObject(x, z);
                if (gridObject != null)
                {
                    PlacableObjects placedObject = gridObject.GetPlacedObject();

                    //Check if there is an object there
                    if (placedObject == null)
                    {
                        Instantiate(emptySpace, grid.GetWorldPosition(x, z), Quaternion.Euler(Vector3.zero));
                    }
                }
            }
        }

    }

    public void ResetGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    ClearObjectAtCoordinate(x, y);
                }
            }
        }
        FindObjectOfType<AudioManager>().EditModeSound("Clear Grid");
    }
    #endregion

    #region Saving Loading
    public string GetJson()
    {
        bool spawn = false;
        bool end = false;
        List<GridObject.SaveGridObject> gridSaveList = new List<GridObject.SaveGridObject>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GridObject gridObject = grid.GetGridObject(x, z);
                if(gridObject.GetGridState() == 0)
                {
                    spawn = true;
                }
                if (gridObject.GetGridState() == 1)
                {
                    end = true;
                }
                gridSaveList.Add(gridObject.Save());
            }
        }
        if(!spawn || !end)
        {
            return null;
        }
        SaveObject saveObject = new SaveObject { gridSaveArray = gridSaveList.ToArray() };
        json = JsonUtility.ToJson(saveObject);
        return json;
    }
    public void LoadGrid(string data)
    {
        ResetGrid(); //Empty all grid before loading
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(data);
        gridWidth = saveObject.gridSaveArray[saveObject.gridSaveArray.Length - 1].x + 1;
        gridHeight = saveObject.gridSaveArray[saveObject.gridSaveArray.Length - 1].z + 1;
        SetUpGrid();
        gv.RefreshGrid();

        foreach (GridObject.SaveGridObject saveGridObject in saveObject.gridSaveArray)
        {
            GridObject gridObject = grid.GetGridObject(saveGridObject.x, saveGridObject.z);

            if (saveGridObject.gridState != -1)
            {
                List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(saveGridObject.x, saveGridObject.z), saveGridObject.dir);
                Vector2Int placedObjectOrigin = new Vector2Int(saveGridObject.x, saveGridObject.z);
                placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(saveGridObject.dir);
                Vector3 placedObjectWorldPos = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y)
                                  + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                placedObjectTypeSO = placedObjectTypeSOList[saveGridObject.gridState];  //Load object
                placedObjectTypeSO.SetNextActiveChild(saveGridObject.objVar);           //Load object variant
                PlacableObjects placedObject =
                    PlacableObjects.Create(placedObjectWorldPos, new Vector2Int(saveGridObject.x, saveGridObject.z), saveGridObject.dir, placedObjectTypeSO);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacableObject(placedObject, saveGridObject.gridState, saveGridObject.objVar, saveGridObject.dir);
                }
                
                //Checked if placed object is a spawn point or end point, if check what object it is and delete its old position
                if (placedObject.placedObjectTypeSO.isOneInstanceObject())
                {
                    ResetOneInstanceObject(placedObject, placedObjectWorldPos);
                }

                //Check if spawn point loaded
                if (saveGridObject.gridState == 0)
                {
                    spawnPointPlaced = true;
                }

                //Check if end point loaded
                if (saveGridObject.gridState == 1)
                {
                    endPointPlaced = true;
                }

                //Events
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
            }
            gridObject.LoadGrid(saveGridObject);
        }
        ChangeSelectedObject(gridState); //Reset to current selected object
    }

    public class SaveObject
    {
        public GridObject.SaveGridObject[] gridSaveArray;
    }

    #endregion

    #region Event Callers
    private void RefreshSelectedObjectType()
    {
        objectVariantNo = 0;
        objectMaxVariant = placedObjectTypeSO.GetMaxVariantIndex();
        placedObjectTypeSO.SetNextActiveChild(objectVariantNo); //reset variant to 0
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshVariantValues()
    {
        OnVariantChanged?.Invoke(this, EventArgs.Empty);
    }

    //private void DeselectObjectType()
    //{
    //    placedObjectTypeSO = null; RefreshSelectedObjectType();
    //}
    #endregion

    #region For Object ghost
    //Return world position base on mouse position
    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePos = MousePos3D.GetMousePositition3D();
        grid.GetXY(mousePos, out int x, out int z);
        if (mousePos.y == 999)
        {
            mousePos = Vector3.zero;
            return mousePos;
        }
        else if (placedObjectTypeSO != null)
        {
            //Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) /*+ new Vector3(rotationOffset.x, 0, rotationOffset.y)*/ /** grid.GetCellSize()*/;
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePos;
        }
    }

    //Return current placed object's rotation
    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    //Return current object being placed
    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }


    #endregion

    #region Warning functions
    private void DisplayWarning(string warningText)
    {
        gm.DisplayToolTipText(warningText);
    }

    private void HideWarning()
    {
        gm.HideToolTip();
    }

    #endregion

    #region Check save file name exist
    //Checks if file exist
    //public bool CheckFile()
    //{
    //    var filePath = Application.dataPath + "/GridSaves";
    //    DirectoryInfo dir = new DirectoryInfo(filePath);
    //    FileInfo[] info = dir.GetFiles("*.txt");

    //    string txtExtension = ".txt";

    //    for (int i = 0; i < info.Length; i++)
    //    {
    //        string fileName = info[i].Name;
    //        fileName = fileName.Replace(txtExtension, "");
    //        if (saveName == fileName)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //Confirm Overwrite
    //public void CheckOverwrite()
    //{
    //    overwrite = false;
    //    CheckFile();
    //    if (CheckFile() == true)
    //    {
    //        overwrite = true;
    //        overwriteMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Overwrite " + saveName + " ?";
    //        overwriteMenu.SetActive(true);
    //    }

    //    if (!overwrite)
    //    {
    //        saveMenu.gameObject.SetActive(false);
    //        SaveGrid();
    //        //saveMenu.UpdateSaveFile(saveName);
    //    }
    //}

    //Confirm Load
    //public void ConfirmLoad()
    //{
    //    CheckFile();
    //    if (CheckFile() == true)
    //    {
    //        loadMenu.SetActive(true);
    //    }
    //}

    //Confirm Delete
    //public void ConfirmDelete()
    //{
    //    CheckFile();
    //    if (CheckFile() == true)
    //    {
    //        deleteMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Delete " + saveName + " ?";
    //        deleteMenu.SetActive(true);
    //    }
    //}
    #endregion

    //public void Delete()
    //{
    //    saveMenu.UpdateDeletedFile(saveName);
    //    SaveSystem.DeleteFile(saveName);
    //}

    public Vector3 GetRandomPlatform()
    {
        int target = 0;
        int x = 0;
        int z = 0;
        do
        {
            x = UnityEngine.Random.Range(0, gridWidth - 1);
            z = UnityEngine.Random.Range(0, gridHeight - 1);
            target = grid.GetGridObject(x, z).GetGridState();
            if(target == 0 || target == 14)
            {
                break;
            }
        } while (target != 2);
        Vector3 destination = grid.GetWorldPosition(x, z);
        destination.x += cellSize / 2;
        destination.y += 0.5f;
        destination.z += cellSize / 2;

        return destination;
    }

    #region External Reference
    private void AquireReferences()
    {
        scrollRectMove = GameObject.Find("ObjectCommandBar_Building").GetComponent<ScrollRectMovement>();

        gm = FindObjectOfType<GameManager>();
        gv = FindObjectOfType<GridVisual>(); 
        commandBar01 = GameObject.Find("CommandBar01");
        commandBar02 = GameObject.Find("CommandBar02");
        commandBarContent01No = GameObject.Find("ObjectCommandBar_Building").transform.GetChild(0).childCount;
        //commandBarContent02No = GameObject.Find("ObjectCommandBar_Dangers").transform.GetChild(0).childCount;  //Not needed until there is a commandBar03
    }

    public void EnableBuild()
    {
        buildingDisabled = false;
        if(objectGhost != null)
        {
            objectGhost.SetActive(true);
        }        
    }

    public void DisableBuild()
    {
        buildingDisabled = true;
        if(objectGhost != null)
        {
            objectGhost.SetActive(false);
        }        
    }

    #endregion

    #region Change Size
    public void SetUpGrid()
    {
        if(changeSize.activeSelf)
        {
            if (lengthInput.text == "")
            {
                if (!Tooltip.Instance.isActiveAndEnabled)
                {
                    gm.DisplayToolTipText("Input field is empty");
                    Timer.Create(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                else
                {
                    gm.DisplayToolTipText("Input field is empty");
                    Timer.ResetTimer(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                return;
            }

            if (widthInput.text == "")
            {
                if (!Tooltip.Instance.isActiveAndEnabled)
                {
                    gm.DisplayToolTipText("Input field is empty");
                    Timer.Create(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                else
                {
                    gm.DisplayToolTipText("Input field is empty");
                    Timer.ResetTimer(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                return;
            }
            
            string newLength = lengthInput.text;
            newGridLength = int.Parse(newLength);

            string newWidth = widthInput.text;
            newGridWidth = int.Parse(newWidth);

            if (newGridLength > 50)
            {
                if (!Tooltip.Instance.isActiveAndEnabled)
                {
                    gm.DisplayToolTipText("Length cannot be > 50");
                    Timer.Create(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                else
                {
                    gm.DisplayToolTipText("Length cannot be > 50");
                    Timer.ResetTimer(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                return;
            }

            if (newGridWidth > 50)
            {
                if (!Tooltip.Instance.isActiveAndEnabled)
                {
                    gm.DisplayToolTipText("Width cannot be > 50");
                    Timer.Create(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                else
                {
                    gm.DisplayToolTipText("Width cannot be > 50");
                    Timer.ResetTimer(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                return;
            }

            if (newGridLength <= 0)
            {
                if (!Tooltip.Instance.isActiveAndEnabled)
                {
                    gm.DisplayToolTipText("Length must be > 0");
                    Timer.Create(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                else
                {
                    gm.DisplayToolTipText("Length must be > 0");
                    Timer.ResetTimer(gm.HideToolTip, 2f, "LengthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                return;
            }

            if (newGridWidth <= 0)
            {
                if (!Tooltip.Instance.isActiveAndEnabled)
                {
                    gm.DisplayToolTipText("Width must be > 0");
                    Timer.Create(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                else
                {
                    gm.DisplayToolTipText("Width must be > 0");
                    Timer.ResetTimer(gm.HideToolTip, 2f, "WidthInputfieldEmpty"); //Hide warning after 2 seconds
                }
                return;
            }

            ResetGrid();
            gridWidth = newGridLength;
            gridHeight = newGridWidth;
            FindObjectOfType<SetInfoText>().SetGridSizeInfo(newLength, newWidth);
            lengthInput.text = null;
            widthInput.text = null;
        }
        #region Grid Set-up
        gridOffset = new Vector3((gridWidth * (int)cellSize) / 2, 0, (gridHeight * (int)cellSize) / 2);

        grid = new GridCustom<GridObject>(gridWidth, gridHeight, cellSize, -gridOffset,
            (GridCustom<GridObject> g, int x, int z) => new GridObject(g, x, z));

        //Setting grids to be empty
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                grid.GetGridObject(x, z).EmptyGrid();
            }
        }
        #endregion
    }
    #endregion
}
