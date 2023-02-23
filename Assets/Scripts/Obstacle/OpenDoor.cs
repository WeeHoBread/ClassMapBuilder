using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private GameManager gm;
    private Vector3 close;
    private Vector3 open;
    public bool contact;
    private bool opening;
    private float openDoor;
    public float doorSpeed;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (transform.parent.parent.parent.parent != null)
        {
            this.GetComponent<OpenDoor>().enabled = false;
        }
        close = transform.parent.transform.position;
        open = new Vector3(close.x, -9.3f, close.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(!gm.isPlaying)
        {
            openDoor = 0.0f;
            opening = false;
            transform.parent.position = close;
        }

        if(openDoor == doorSpeed)
        {
            return;
        }

        if (opening)
        {
            openDoor += Time.deltaTime;
            if (openDoor >= doorSpeed)
            {
                openDoor = doorSpeed;
            }
            float doorProgress = openDoor / doorSpeed;
            transform.parent.position = Vector3.Lerp(close, open, doorProgress);
            return;
        }

        if (!contact)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            opening = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            contact = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            contact = false;
        }
    }
}
