using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLock : MonoBehaviour
{
    private GameObject[] keys;
    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.isPlaying)
        {
            return;
        }
        
        else if (gm.isPlaying)
        {
            keys = GameObject.FindGameObjectsWithTag("Key");
        }

        for (int i =0; i < keys.Length; i++)
        {
            if (keys[i].activeSelf)
            {
                return;
            }
        }
        transform.GetChild(0).gameObject.SetActive(false);

    }
}
