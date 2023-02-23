using UnityEngine;

public class MenuOpenChecker : MonoBehaviour
{
    private void OnEnable()
    {
        FindObjectOfType<EditCamera>().CameraLock();
        FindObjectOfType<GameManager>().LockInState();
    }

    private void OnDisable()
    {


        if(FindObjectOfType<EditCamera>() != null)
        {
            FindObjectOfType<EditCamera>().CameraUnlocked();
        }     
        
        if(FindObjectOfType<GameManager>() != null)
        {
            FindObjectOfType<GameManager>().UnlockState();
        }
        
    }

}
