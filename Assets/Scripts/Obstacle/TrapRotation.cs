using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private bool trapActive = false;
    private Vector3 start;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (trapActive)
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }

    public void SetTrapActive()
    {
        transform.rotation = Quaternion.Euler(start);
        trapActive = true;
    }

    public void SetTrapInactive()
    {
        trapActive = false;
        transform.rotation = Quaternion.Euler(start);
    }

}
