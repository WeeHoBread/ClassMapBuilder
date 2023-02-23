using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnEnable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log("Object set active");
    }

    private void OnDisable()
    {
        Debug.Log("Object set inactive");
    }

}
