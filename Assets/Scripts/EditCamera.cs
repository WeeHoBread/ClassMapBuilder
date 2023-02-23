using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCamera : MonoBehaviour
{
    public float cameraSpeed;
    public float zoomSpeed;
    public float scrollSpeed;
    private float verticalInput;
    private float horizontalInput;
    private float mouseAxisInput;
    private bool cameraLocked;

    private AudioManager audioManager;
    private AudioSource cameraRotateAS;
    private AudioSource cameraZoomAS;
    public GameObject objectGhost;
    //private AudioSource cameraPanAS;

    #region Cam movement state for mouse control only
    private CameraMovementState camMoveState;
    public enum CameraMovementState
    {
        Neutral,
        ZoomIn,
        ZoomOut,
        RotatingClock,
        RotatingAntiClock,
        PanningHoz,
        PanningVert
    }
    #endregion


    // Start is called before the first frame update
    private void Start()
    {
        camMoveState = CameraMovementState.Neutral;
        audioManager = FindObjectOfType<AudioManager>();
        cameraRotateAS = audioManager.FindAudioSource(audioManager.soundsEditMode, "Camera Turn");
        cameraZoomAS = audioManager.FindAudioSource(audioManager.soundsEditMode, "Camera Zoom");
        //cameraPanAS = audioManager.FindAudioSource(audioManager.soundsEditMode, "Camera Pan");
    }

    // Update is called once per frame
    void Update()
    {   
        if (objectGhost == null)
        {
            objectGhost = GameObject.Find("GhostPointer");
        }

        if (cameraLocked)
        {
            return;
        }

        #region Keyboard Input
        //Camera Movement
        #region Camera Movement
        //Horizontal movement
        if(camMoveState != CameraMovementState.PanningHoz)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal") * cameraSpeed * Time.deltaTime;
        }
        else
        {
            horizontalInput = mouseAxisInput * cameraSpeed * Time.deltaTime;
        }

        //Vertical Movement
        if (camMoveState != CameraMovementState.PanningVert)
        {
            verticalInput = Input.GetAxisRaw("Vertical") * cameraSpeed * Time.deltaTime;
        }        
        else
        {
            verticalInput = mouseAxisInput * cameraSpeed * Time.deltaTime;
        }
        Vector3 movDir = new Vector3(horizontalInput + verticalInput, 0, verticalInput - horizontalInput);
        movDir.Normalize();
        transform.parent.transform.parent.transform.Translate(movDir, Space.Self);        
        #endregion

        #region Rotate Camera Left/Right
        if (Input.GetKey(KeyCode.Q) || camMoveState == CameraMovementState.RotatingAntiClock)
        {
            this.transform.parent.transform.parent.Rotate(0, cameraSpeed * Time.deltaTime, 0, Space.World);
            //If sound is not being played, play it
            //if (cameraRotateAS.isPlaying == false)
            //{
            //    cameraRotateAS.Play();
            //}
        }
        if (Input.GetKey(KeyCode.E) || camMoveState == CameraMovementState.RotatingClock)
        {
            this.transform.parent.transform.parent.Rotate(0, -1 * cameraSpeed * Time.deltaTime, 0, Space.World);

            //If sound is not being played, play it
            //if (cameraRotateAS.isPlaying == false)
            //{
            //    cameraRotateAS.Play();
            //}
        }

        //Stop playing sound the moment player is no longer holding the button
        //if (Input.GetKey(KeyCode.Q) == false && Input.GetKey(KeyCode.E) == false && camMoveState == CameraMovementState.Neutral)
        //{
        //    cameraRotateAS.Stop();
        //}

        #endregion

        #region Camera Zoom
        if (Input.GetKey(KeyCode.F) || camMoveState == CameraMovementState.ZoomIn)
        {
            CameraZoomIn();
        }

        if (Input.GetKey(KeyCode.G) || camMoveState == CameraMovementState.ZoomOut)
        {
            CameraZoomOut();
        }

        //Stop playing sound the moment player is no longer holding button
        //if (Input.GetKey(KeyCode.F) == false && Input.GetKey(KeyCode.G) == false && camMoveState == CameraMovementState.Neutral)
        //{
        //    cameraZoomAS.Stop();
        //}
        #endregion

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ResetCamera();
        }
        #endregion

        #region Mouse Input

        #region Mouse Drag Movement

        //if (Input.GetMouseButton(2)) //Camera Panning with middle mouse
        //{
        //   Vector3 input = new Vector3((Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Mouse Y")) * Time.deltaTime * cameraSpeed * -10f, 0, (Input.GetAxisRaw("Mouse Y") - Input.GetAxisRaw("Mouse X")) * Time.deltaTime * cameraSpeed * -10f);
        //    this.transform.parent.transform.parent.Translate(input);
        //}
        #endregion

        #region Camera zoom using mouse scroll wheel
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f && Camera.main.orthographicSize > 26.0f)
        {
            Camera.main.orthographicSize -= scrollSpeed;

            FindObjectOfType<AudioManager>().EditModeSound("Camera Scroll Zoom");
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f && Camera.main.orthographicSize < 150.0f)
        {
            Camera.main.orthographicSize += scrollSpeed;

            FindObjectOfType<AudioManager>().EditModeSound("Camera Scroll Zoom");
        }
        #endregion

        #region Mouse movement using L-Ctrl or L-Mouse
        if(!Input.GetKey(KeyCode.LeftControl)) //Not pressing left control
        {
            return;            
        }
        objectGhost.SetActive(false); //Set ghost to inactive if player is attempting to drag camera

        #region Mouse Drag Zoom
        if (Input.GetMouseButton(1))
        {
            if (Input.GetAxis("Mouse X") < 0 && Camera.main.orthographicSize < 150.0f)
            {
                Camera.main.orthographicSize += 0.5f;
            }

            if (Input.GetAxis("Mouse X") > 0 && Camera.main.orthographicSize > 26.0f)
            {
                Camera.main.orthographicSize -= 0.5f;
            }
        }
        #endregion 

        if (Input.GetMouseButton(0))
        {
            transform.parent.transform.parent.Rotate(0, Input.GetAxis("Mouse X"), 0, Space.Self);
        }
        #endregion
        #endregion
    }

    public void ResetCamera()
    {
        this.transform.parent.transform.parent.position = Vector3.zero;
        this.transform.parent.transform.parent.rotation = Quaternion.Euler(Vector3.zero);
        this.transform.parent.rotation = Quaternion.Euler(30, 45, 0);
    }

    #region Camera control methods
    private void CameraZoomIn()
    {
        if (Camera.main.orthographicSize > 26.0f)
        {
            Camera.main.orthographicSize -= zoomSpeed * Time.deltaTime;
        }
    }

    private void CameraZoomOut()
    {
        if (Camera.main.orthographicSize < 150.0f)
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
        }
    }

    public void CameraLock()
    {
        cameraLocked = true;
        camMoveState = CameraMovementState.Neutral;
    }
    
    public void CameraUnlocked()
    {
        cameraLocked = false;
    }

    #endregion

    #region Camera Click public methods
    public void SetNewCamState(int newStateIndex)
    {
        if (newStateIndex == 0)
        {
            camMoveState = CameraMovementState.Neutral;
            mouseAxisInput = 0;
        }
        else if (newStateIndex == 1)
        {
            camMoveState = CameraMovementState.ZoomIn;
        }
        else if (newStateIndex == 2)
        {
            camMoveState = CameraMovementState.ZoomOut;
        }
        else if (newStateIndex == 3)
        {
            camMoveState = CameraMovementState.RotatingClock;
        }
        else if (newStateIndex == 4)
        {
            camMoveState = CameraMovementState.RotatingAntiClock;
        }
    }

    public void HorizontalMove(float hozAxis)
    {
        hozAxis = Mathf.Clamp(hozAxis, -1, 1);
        mouseAxisInput = Mathf.Lerp(mouseAxisInput, hozAxis, 0.9f);
        camMoveState = CameraMovementState.PanningHoz;
    }

    public void VerticalMove(float vertAxis)
    {
        vertAxis = Mathf.Clamp(vertAxis, -1, 1);
        mouseAxisInput = Mathf.Lerp(mouseAxisInput, vertAxis, 0.9f);
        camMoveState = CameraMovementState.PanningVert;
    }


    #endregion

}
