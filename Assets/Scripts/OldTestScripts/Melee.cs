using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Breakable")
        {
            if (Input.GetKey(KeyCode.E))
            {
                other.gameObject.transform.gameObject.SetActive(false);
            }
        }
    }
}
