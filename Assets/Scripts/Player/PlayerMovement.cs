using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float maximumSpeed;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float jumpButtonGracePeriod;

    [SerializeField]
    private Transform cameraTransform;

    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool grounded;
    private bool icy;
    private Vector3 lastMoved;
    private Vector3 velocity;
    private float verticalInput;
    private float horizontalInput;
    private float slipSpeed;
    private int teleDir;

    public float slipmax;
    public bool isStraight;
    public bool isGameOver;
    public bool isClear;
    public bool isTeleport;
    private bool canMove;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        animator = GetComponent<Animator>();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTeleport)
        {
            Spin();
        }

        if (isClear || isGameOver || !canMove)
        {
            if (isClear || isGameOver)
            {
                ySpeed = -0.5f;
                icy = false;
                GetComponent<Collider>().enabled = true;
                isTeleport = false;
            }
            return;
        }

        if (isStraight)
        {
            verticalInput = Input.GetAxisRaw("Vertical");
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        else if(!isStraight)
        {
            verticalInput = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Horizontal");
            horizontalInput = Input.GetAxisRaw("Horizontal") - Input.GetAxisRaw("Vertical");
        }
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        float speed = inputMagnitude * maximumSpeed;
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();



        if (horizontalInput != 0 || verticalInput != 0)
        {
            slipSpeed = slipmax;
            animator.SetInteger("Speed", 1);
        }
        else
        {
            animator.SetInteger("Speed", 0);
        }
        if (slipSpeed > 0.0f)
        {
            slipSpeed -= 2.0f * Time.deltaTime;
            if (slipSpeed < 0.0f)
            {
                slipSpeed = 0.0f;
            }
        }
        ySpeed += Physics.gravity.y * 30.0f * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
            if(grounded == false)
            {
                grounded = true;
                StepSound();
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
            if (characterController.isGrounded)
            {
                grounded = false;
                StepSound();
            }
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }
        if (icy)
        {
            if (horizontalInput != 0 || verticalInput != 0)
            {
                lastMoved = movementDirection;
            }
            velocity = movementDirection * speed + lastMoved * slipSpeed;
        }
        else if (!icy)
        {
            lastMoved = Vector3.zero;
            velocity = movementDirection * speed;
        }
        velocity.y = ySpeed;
        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = toRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            isClear = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Enemy")
        {
            isGameOver = true;
        }

        icy = hit.collider.CompareTag("Ice");
    }

    private void StepSound()
    {
        FindObjectOfType<IsometricMovement>().Step();
    }

    public void ToggleDirection()
    {
        if (isStraight)
        {
            isStraight = false;
        }
        else if (!isStraight)
        {
            isStraight = true;
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Enemy")
    //    {
    //        Debug.Log("HIT");
    //        isGameOver = true;
    //    }
    //}
    //private void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //    }
    //    else
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //    }
    //}

    private void OnEnable()
    {
        EnableMovement();
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void Spin()
    {
        teleDir = UnityEngine.Random.Range(0, 4);
        transform.Rotate(0, teleDir * 90 * Time.deltaTime * 10, 0);
    }
}
