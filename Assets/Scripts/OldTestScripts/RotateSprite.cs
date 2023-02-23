using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSprite : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationAngle;

    private bool rotating;
    float oldRotation;
    float newRotation;
    private Quaternion quat;

    // Start is called before the first frame update
    void Start()
    {
        oldRotation = transform.rotation.z;
        newRotation = oldRotation + rotationAngle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rotating)
        {
            SpriteRotation();
        }
    }



    public void InitiateRotation()
    {
        if (rotating == false)
        {
            Debug.Log(oldRotation);
            
            Debug.Log(newRotation);
            rotating = true;
        }
    }

    private void SpriteRotation()
    {        
        transform.Rotate(Vector3.forward, rotationSpeed);

        if (transform.rotation.eulerAngles.z >= newRotation)
        {
            oldRotation += rotationAngle;
            newRotation = oldRotation + rotationAngle;
            rotating = false;
        }
    }

}
