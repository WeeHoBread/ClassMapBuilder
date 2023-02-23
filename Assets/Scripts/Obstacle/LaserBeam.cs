using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private LineRenderer lr;
    private int layerMask = 7; //Layer belonging to Empty grid

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 1000f, layerMask))
        {
            if (hit.collider && !hit.collider.CompareTag("Empty"))
            {
                lr.SetPosition(1, hit.point);
            }

            if(hit.collider.CompareTag("Player"))
            {
                hit.transform.GetComponent<IsometricMovement>().isGameOver = true;
            }

        }
        else
        {
            lr.SetPosition(1, transform.forward * 1000); //Set finite distance for laser
        }

    }
}
