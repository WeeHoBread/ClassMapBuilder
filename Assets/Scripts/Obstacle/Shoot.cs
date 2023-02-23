using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private int lastFired;
    private GameObject[] ammo;
    private GameObject[] hit;
    public int ammoLimit;
    private float nextShot;
    public float interval;
    public GameObject bullet;
    public Transform fireFromHere;
    private GameManager checkGameState;
    private bool reset = true;
    private AudioSource aS;

    // Start is called before the first frame update
    void Start()
    {
        checkGameState = GameObject.Find("GameManager").GetComponent<GameManager>();
        aS = GetComponent<AudioSource>();

        ammo = new GameObject[ammoLimit];
        hit = new GameObject[ammoLimit];
        //for (int i = 0; i < ammo.Length; i++)
        //{
        //    Instantiate(bullet, fireFromHere);
        //    ammo[i] = transform.GetChild(i).gameObject;
        //    ammo[i].SetActive(false);
        //}
        //for (int i = 0; i < ammo.Length; i++)
        //{
        //    Physics.IgnoreCollision(ammo[i].GetComponent<Collider>(), transform.parent.GetChild(0).GetComponent<Collider>());
        //    ammo[i].GetComponent<Bullet>().ignore = this.transform.parent.gameObject;
        //    ammo[i].transform.parent = null;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkGameState.isPlaying)
        {
            nextShot = interval - 1.0f;
            for (int i = 0; i < ammo.Length; i++)
            {
                Destroy(ammo[i]);
                Destroy(hit[i]);
            }
            reset = true;
            return;
        }

        if (reset)
        {
            for (int i = 0; i < ammo.Length; i++)
            {
                Instantiate(bullet, fireFromHere);
                ammo[i] = transform.GetChild(i).gameObject;
                hit[i] = ammo[i].transform.GetChild(0).gameObject;
                ammo[i].SetActive(false);
            }
            for (int i = 0; i < ammo.Length; i++)
            {
                Physics.IgnoreCollision(ammo[i].GetComponent<Collider>(), transform.parent.GetChild(0).GetComponent<Collider>());
                ammo[i].GetComponent<Bullet>().ignore = this.transform.parent.gameObject;
                ammo[i].transform.parent = null;
            }
            reset = false;
        }

        if (lastFired == ammoLimit)
        {
            lastFired = 0;
        }

        nextShot += Time.deltaTime;

        if (nextShot > interval)
        {
            ammo[lastFired].transform.position = new Vector3(fireFromHere.position.x, fireFromHere.position.y, fireFromHere.position.z);
            ammo[lastFired].transform.rotation = fireFromHere.rotation;
            ammo[lastFired].SetActive(true);
            lastFired++;

            aS.Play();

            //FindObjectOfType<AudioManager>().PlayModeSound("Turret Shoot");

            nextShot = 0.0f;
        }

    }
}
