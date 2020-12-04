using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using TMPro;
using System.Timers;

public class GameManager : MonoBehaviour
{
    //Player Triggered GameObjects
    public GameObject doorOne; //North
    public GameObject doorOneB;
    public GameObject levelOnePortal;
    public GameObject level0;

    //Text
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject controlText;
    public GameObject levelTwoText;

    //Hud Variables
    public float playerLife;
    private int lifeDisplay;
    private int count;
    private int pickup;
    public int winCount;
    public int levelGoal;

    //Movement
    public CharacterController controller;
    private float turnTime = 0.1f;
    private float turnVelocity = 3;
    public Transform cam;
    public float speed = 20f;
    public float force = 10f;
    public float recoilRecovery = 1;
    public float damageRecoil = -10;
    public float gravity = -20f;
    public Transform groundCheck;
    private float groundDistance = 0.5f;
    public LayerMask groundMask;
    Vector3 velocity;

    //Jump
    public int jumpStrength = 10;
    public int doubleJumpStrength = 5;
    public bool  doubleJumpCheck = false;
    private bool grounded = true;
    public float jumpCooldown;

    //Dash
    public int dashSpeed = 5;
    public float dashDelay = 1;
    public float dashTime = 0.15f;
    private int dDashCheck = 0;
    private int aDashCheck = 0;
    private int wDashCheck = 0;
    private int sDashCheck = 0;
    int DashLimit = 1;

    //Damage
    int enemyDamage = 20;
    int healthPack = 25;

    //Initialize
    void Start()
    {
        Cursor.visible = false;
        playerLife = 100;
        count = 0;
        SetLifeText();
        SetCountText();
        doorOne.SetActive(true);
        winTextObject.SetActive(false);


    }

    void Update()
    {
        
            if (Input.GetKey(KeyCode.D))
            {
              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (dDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    dDashCheck = dDashCheck + 1;
                }
              }
            }

            if (Input.GetKey(KeyCode.A))
            {
              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (aDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    aDashCheck = aDashCheck + 1;
                }
              }
            }

            if (Input.GetKey(KeyCode.S))
            {
              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (sDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    sDashCheck = sDashCheck + 1;
                }
              }
            }

            if (Input.GetKey(KeyCode.W))
            {

              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (wDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    wDashCheck = wDashCheck + 1;
                }
              }

            }
       
        //Set Life Range
        if (playerLife > 100)
        {
            playerLife = 100;
        }
        if (playerLife < 0)
        {
            playerLife = 0;
        }

        //groundcheck
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(grounded && velocity.y < 0)
        {
            velocity.y = -3f;
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;

        //Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf. Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        //Convert Life Float to Int & Display
        lifeDisplay = (int) playerLife;
        SetLifeText();

        //Remove Tutorial Text
        if (Input.GetKeyDown("x"))
        {
            controlText.SetActive(false);
        }

        //If Grounded
        if (grounded == true)
        {
            doubleJumpCheck = false;
            jumpCooldown = 0;

            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = velocity.y + jumpStrength;


            }
            //Jump Limit
            doubleJumpCheck = false;

            //Airdash Limit
            dDashCheck = 0;
            aDashCheck = 0;
            wDashCheck = 0;
            sDashCheck = 0;
        }
        else
        {
            jumpCooldown = jumpCooldown + (1 * Time.deltaTime);

            if (doubleJumpCheck == false && Input.GetKeyDown(KeyCode.Space))
            {

                if (jumpCooldown >= 0.3f)
                {
                    if (doubleJumpCheck == false && Input.GetKeyDown(KeyCode.Space))
                    {
                        velocity.y = 0;
                        velocity.y = velocity.y + doubleJumpStrength;
                        doubleJumpCheck = true;
                    }
                }

            }
        }
            controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator Dash()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            controller.Move(moveDir * dashSpeed * Time.deltaTime);

            yield return null;
        }
        
    }
    void SetLifeText()
    {
        lifeText.text = "Life: " + lifeDisplay.ToString();

        //Respawn
        if(playerLife <= 0)
        {   
            gameObject.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            playerLife = 100;
            lifeText.text = "Life: " + lifeDisplay.ToString();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        //Count Triggered Events
        if (count >= 4)
        {
            doorOne.SetActive(false);
        }
        if (count >= 8)
        {
            doorOneB.SetActive(false);
        }
        if (count == levelGoal)
        {
            levelTwoText.SetActive(true);
            levelOnePortal.SetActive(true);
        }
        else
        {
            levelTwoText.SetActive(false);
        }
        //if (count >= 10)
        //{
        //    doorTwo.SetActive(false);
        //}
        if (count >= winCount)
        {
            winTextObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //GameObject Interaction
        if (other.gameObject.CompareTag("Killbox"))
        {
           transform.position = new Vector3(0.0f, 1.6f, 0.0f);
        }

        if (other.gameObject.CompareTag("PortalOne"))
        {
            Application.LoadLevelAdditive(1);
            level0.SetActive(false);
        }

        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = (count + 1);
            SetCountText();
        }

        if (other.gameObject.CompareTag("Hp"))
        {
            if (playerLife < 100)
            {
                other.gameObject.SetActive(false);
                playerLife = (playerLife + healthPack);
                SetLifeText();
            }

        }

        if (other.gameObject.name == "HurtyBit")
        {   
            playerLife = playerLife - enemyDamage;
            SetLifeText();
            //Vector3 dir = transform.position - other.transform.position;
            //dir.Normalize();
            //controller.Move(dir * force);
        }

        if (other.gameObject.CompareTag("Home"))
        {
            levelTwoText.SetActive(false);
        }

        if (other.gameObject.CompareTag("Killbox"))
        {
            gameObject.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            playerLife = 100;
            lifeText.text = "Life: " + lifeDisplay.ToString();
        }

        if (other.gameObject.CompareTag("Lava"))
        {
            playerLife = playerLife - 10;
            SetLifeText();
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Lava"))
        { 
            playerLife = playerLife - 20 * Time.deltaTime;
            SetLifeText();
            controller.Move(Vector3.up * force * Time.deltaTime);
        }

        if (other.gameObject.CompareTag("Spike"))
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            controller.Move(dir * force);
        }
    }

}

