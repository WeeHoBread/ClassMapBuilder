using UnityEngine;

public class Key : MonoBehaviour
{
    public void Reset()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            FindObjectOfType<AudioManager>().EditModeSound("Object Pickup");
            gameObject.SetActive(false);
        }
    }
}
