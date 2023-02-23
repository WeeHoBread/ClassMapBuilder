using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject playerCamera;

    private void Update()
    {
        if(FindObjectOfType<GameManager>().isPlaying == false)
        {
            StopAllCoroutines();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag  == "Player")
        {
            StartCoroutine(spinPlayer(other.transform));
        }
    }

    public IEnumerator spinPlayer(Transform spin)
    {
        FindObjectOfType<AudioManager>().PlayModeSound("Teleporter");
        spin.GetComponent<PlayerMovement>().DisableMovement();
        spin.GetComponent<Collider>().enabled = false;
        spin.GetComponent<PlayerMovement>().isTeleport = true;

        yield return new WaitForSeconds(1f);

        playerCamera = GameObject.Find("LookAt");
        int angle = UnityEngine.Random.Range(0, 4);
        if (angle == 1)
        {
            playerCamera.transform.Rotate(0, 90, 0);
        }
        else if (angle == 2)
        {
            playerCamera.transform.Rotate(0, 180, 0);
        }
        else if (angle == 3)
        {
            playerCamera.transform.Rotate(0, -90, 0);
        }
        Vector3 target = GridBuilder.Instance.GetRandomPlatform();
        spin.transform.position = target;
        spin.GetComponent<PlayerMovement>().DisableMovement();
        yield return new WaitForSeconds(0.5f);
        spin.GetComponent<PlayerMovement>().isTeleport = false;
        FindObjectOfType<AudioManager>().PlayModeSound("Teleporter");
        spin.GetComponent<Collider>().enabled = true;
        spin.GetComponent<PlayerMovement>().EnableMovement();
    }
}
