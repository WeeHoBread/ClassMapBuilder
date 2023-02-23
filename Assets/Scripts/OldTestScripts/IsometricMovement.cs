using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricMovement : MonoBehaviour
{
    [Header("Character Attributes")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5;
    private float horInput;
    private float verInput;
    private float dropSpeed;
    //[SerializeField] private float _turnspeed = 300;
    private Vector3 input;
    private float cameraAngle;
    private Vector3 turn;
    public float jumpTime;
    public float jumpLimit;
    //public bool notStraight;

    [Header("Character Status")]
    public bool isGameOver;
    public bool onSlope;
    public bool isJumping;
    public bool isClear;
    private Animator animator;
    [SerializeField]  private Transform cameraTransform;

    //Variables requires


    [Header("Character Sounds")]
    [SerializeField] private AudioClip[] footSteps;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (isClear || isGameOver)
        {
            return;
        }
        cameraAngle = cameraTransform.parent.parent.rotation.eulerAngles.y;

        dropSpeed = rb.velocity.y;

        if (dropSpeed > 0.0f)
        {
            dropSpeed = 0.0f;
        }
        //if ((isJumping == true || onSlope == true) && dropSpeed < -10.0f)
        //{
        //    dropSpeed = -10.0f;
        //}
        //else if ((isJumping == false || onSlope == false) && dropSpeed > 0.0f)
        //{
        //    dropSpeed = 0.0f;
        //}

        //if (Input.GetKey(KeyCode.Space) && jumpTime < jumpLimit)
        //{
        //    dropSpeed = 50.0f;
        //    jumpTime += 10 * Time.deltaTime;
        //}
        //if (rb.velocity.y == 0)
        //{
        //    jumpTime = 0.0f;
        //}
        GatherInput();
        Look();
        if (horInput !=0 || verInput !=0)
        {
            animator.SetInteger("Speed", 1);
        }
        else
        {
            animator.SetInteger("Speed", 0);
        }
    }

    void FixedUpdate()
    {
        Move();
    }
    void GatherInput()
    {

        //if (notStraight)
        //{
        //Diagonal();
        //}
        //else if(notStraight ==false)
        //{
        Straight();
        //}
        input = new Vector3(horInput, dropSpeed, verInput);
        input = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * input;
        input.Normalize();
        turn = new Vector3(horInput, 0.0f, verInput);
    }

    void Move()
    {
        //rb.MovePosition(transform.position + (transform.forward * input.normalized.magnitude) * _speed * Time.deltaTime);        
        //transform.Translate(input * _speed * Time.deltaTime);
        rb.velocity = input * speed;
        //rb.AddForce(input * speed);
    }

    void Look()
    {
        if (input == Vector3.zero)
        {
            return;            
        }

        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraAngle + 90 * Input.GetAxisRaw("Horizontal"), 0);
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraAngle + 180, 0);
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraAngle, 0);
        }

        if (Input.GetAxisRaw("Horizontal") > 0 && Input.GetAxisRaw("Vertical") > 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraAngle + 45, 0);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && Input.GetAxisRaw("Vertical") > 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraAngle - 45, 0);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && Input.GetAxisRaw("Vertical") < 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraAngle - 135, 0);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0 && Input.GetAxisRaw("Vertical") < 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraAngle + 135, 0);
        }
    }

    void Straight()
    {
        horInput = Input.GetAxisRaw("Horizontal") - Input.GetAxisRaw("Vertical");
        verInput = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Horizontal");
    }

    //void Diagonal()
    //{
    //    horInput = Input.GetAxisRaw("Horizontal");
    //    verInput = Input.GetAxisRaw("Vertical");
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            isClear = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            isGameOver = true;
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{

    //        isGrounded = false;

    //}



    //A function that is called whenever play takes a step
    public void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return footSteps[UnityEngine.Random.Range(0, footSteps.Length)];
    }

}
