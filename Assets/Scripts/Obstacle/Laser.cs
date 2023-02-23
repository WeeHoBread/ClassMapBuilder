using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float upTime;
    public float duration;
    public bool isTimeBased;
    private bool on;
    private GameManager checkGameState;
    // Start is called before the first frame update
    void Start()
    {
        checkGameState = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimeBased || !checkGameState.isPlaying)
        {
            upTime = 0.0f;
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider>().enabled = true;
            on = true;
            return;
        }
        if (!on)
        {
            if (upTime < duration)
            { 
                upTime += Time.deltaTime;
                return;
            }
            upTime = 0.0f;
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider>().enabled = true;
            on = true;
        }
        else if (on)
        {
            
            if (upTime < duration)
            {
                upTime += Time.deltaTime;
                return;
            }
            upTime = 0.0f;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            on = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerMovement>().isGameOver = true;
        }

        //if (other.tag == "Enemy")
        //{
        //    other.gameObject.SetActive(false);
        //}
    }
}
