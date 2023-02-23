using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private GameManager checkGameState;
    private Rigidbody rb;
    private GameObject hit;
    public GameObject ignore;
    public float speed;
    private AudioSource aS;

    // Start is called before the first frame update
    void Start()
    {
        hit = transform.GetChild(0).gameObject;
        hit.transform.parent = null;
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        checkGameState = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = this.GetComponent<Rigidbody>();
        aS = hit.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!checkGameState.isPlaying)
        {
            gameObject.SetActive(false);
            return;
        }
        Vector3 movementDirection = Vector3.forward;
        movementDirection = Quaternion.AngleAxis(ignore.transform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();
        rb.velocity = movementDirection * -1 * speed;
    }

    private void OnTriggerEnter(Collider other)
    {        

        if (other.tag == "Player")
        {
            hit.transform.position = transform.position;
            aS.Play();
            other.GetComponent<PlayerMovement>().isGameOver = true;

        }

        if (other.tag != "Empty" && other.gameObject.name != this.name)
        {
            hit.transform.position = transform.position;
            aS.Play();
            this.gameObject.SetActive(false);            
        }
    }
}
