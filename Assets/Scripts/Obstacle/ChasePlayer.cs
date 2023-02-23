using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
    private aiState currentState;
    private GameManager gm;
    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 playerDir;
    private Rigidbody rb;
    private float wander;

    public float adjust;
    public float changeDirection;
    public float detectRange;
    public float speed;

    enum aiState
    {
        Idle,
        Wander,
        Chase,
        Falling
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.parent.parent != null)
        {
            GetComponent<ChasePlayer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            return;
        }
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        
        startPos = this.transform.position;
        startRot = this.transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.isPlaying)
        {
            currentState = aiState.Idle;
            transform.position = startPos;
            transform.rotation = startRot;
            return;
        }
        playerDir = GameObject.FindGameObjectWithTag("Player").transform.position;

        //Debug.Log(currentState);
        switch (currentState)
        {
            case aiState.Idle:
                if (!CheckGround())
                {
                    currentState = aiState.Falling;
                    break;
                }
                if (PlayerSpotted())
                {
                    currentState = aiState.Chase;
                    break;
                }
                rb.velocity = Vector3.zero;
                wander += Time.deltaTime;

                if (wander > changeDirection / 2)
                {
                    do
                    {
                        int angle = UnityEngine.Random.Range(0, 4);
                        if (angle == 0)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (angle == 1)
                        {
                            transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (angle == 2)
                        {
                            transform.rotation = Quaternion.Euler(0, 180, 0);
                        }
                        else if (angle == 3)
                        {
                            transform.rotation = Quaternion.Euler(0, 270, 0);
                        }
                    } while (CheckObstacle());
                    wander = 0;
                    currentState = aiState.Wander;
                }
                break;
            case aiState.Wander:
                if (!CheckGround())
                {
                    currentState = aiState.Falling;
                    break;
                }
                if (PlayerSpotted())
                {
                    currentState = aiState.Chase;
                    break;
                }
                if (CheckObstacle())
                {
                    do
                    {
                        int angle = UnityEngine.Random.Range(0, 4);
                        if (angle == 0)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (angle == 1)
                        {
                            transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (angle == 2)
                        {
                            transform.rotation = Quaternion.Euler(0, 180, 0);
                        }
                        else if (angle == 3)
                        {
                            transform.rotation = Quaternion.Euler(0, 270, 0);
                        }
                    } while (CheckObstacle());
                }
                rb.velocity = transform.forward * speed;
                wander += Time.deltaTime;

                if (wander > changeDirection)
                {
                    wander = 0;
                    currentState = aiState.Idle;
                }
                break;

            case aiState.Chase:
                if (!CheckGround())
                {
                    currentState = aiState.Falling;
                    break;
                }
                if (!PlayerSpotted())
                {
                    if(CheckObstacle())
                    {
                        currentState = aiState.Idle;
                        break;
                    }
                    rb.velocity = transform.forward * speed * 3;
                    wander += Time.deltaTime;
                    if(wander > 4.0f)
                    {
                        currentState = aiState.Idle;
                        break;
                    }
                    break;
                }
                wander = 0;
                rb.velocity = transform.forward * speed * 3;
                transform.LookAt(new Vector3(playerDir.x, 0.0f, playerDir.z), Vector3.up);
                break;
            case aiState.Falling:
                if (transform.position.y < -1000.0f)
                {
                    rb.velocity = Vector3.zero;
                    return;
                }
                rb.velocity = Vector3.down * 50;
                break;
        }


    }

    bool PlayerSpotted()
    {
        float dist = Vector3.Distance(playerDir, transform.position);
        if ( dist > detectRange)
        {
            return false;
        }
        float fov = 45f;
        int rayCount = 4;
        float startAngle = transform.rotation.eulerAngles.y + (fov / 2);
        float angle = startAngle;
        float increase = fov / rayCount;
        float evalate = adjust * (Mathf.PI / 180f);
        //Debug.Log(Mathf.Sin(evalate));

        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit hit;
            float angleRad = angle * (Mathf.PI / 180f);
            Vector3 currentAngle = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), currentAngle * detectRange, Color.green);
            if (i ==2)
            {
                Vector3 evalateAngle = new Vector3(Mathf.Sin(angleRad), Mathf.Sin(evalate), Mathf.Cos(angleRad));
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), evalateAngle * detectRange, Color.green);
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), evalateAngle, out hit, detectRange))
                {
                    if (hit.transform.tag == "Player")
                    {
                        return true;
                    }
                }

            }    
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), currentAngle, out hit, detectRange))
            {
                if (hit.transform.tag == "Player")
                {
                    return true;
                }
            }
            angle -= increase;
        }
        return false;
    }

    bool CheckObstacle()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.forward * 3, Color.yellow);
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform.tag == "Pit")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    bool CheckGround()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position + transform.forward * -1.2f, Vector3.down, Color.yellow);

        if (Physics.Raycast(transform.position + transform.forward * -1.2f, Vector3.down, out hit, 3f))
        {
            return true;
        }

        return false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerMovement>().isGameOver = true;
        }
    }
}
