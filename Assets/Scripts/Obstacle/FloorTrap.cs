using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : MonoBehaviour
{
    private Vector3 startPos;
    private float timer;
    private GameManager gm;
    public bool triggered;
    public float limit;
    private bool soundPlayed = false;

    [SerializeField] private Material green;
    [SerializeField] private Material red;
    // Start is called before the first frame update
    void Start()
    {
        //SetMaterial(green);
        startPos = this.transform.position;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        SetMaterial(green);
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggered)
        {
            return;
        }

        if(!soundPlayed)
        {
            FindObjectOfType<AudioManager>().PlayModeSound("TrapFloor");
            soundPlayed = true;
        }

        if (transform.position.y < -50.0f)
        {
            gameObject.SetActive(false);
            return;
        }
        if (timer > limit)
        {
            transform.Translate(0.0f , -1.0f, 0.0f);
            return;
        }
        timer += Time.deltaTime;
    }

    public void Reset()
    {
        transform.position = startPos;
        triggered = false;
        soundPlayed = false;
        SetMaterial(green);
        timer = 0.0f;
        gameObject.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(!gm.isPlaying)
        {
            return;
        }
        if (other.GetComponent<Rigidbody>().mass >= 20.0f)
        {
            SetMaterial(red);
            triggered = true;
        }
    }
    private void SetMaterial(Material nextMat)
    {
        Renderer[] children;

        children = transform.GetChild(0).transform.GetChild(0).GetComponents<Renderer>();
        foreach (Renderer rend in children)
        {
            var mat = new Material[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                mat[i] = nextMat;
            }
            rend.materials = mat;
        }


    }
}
