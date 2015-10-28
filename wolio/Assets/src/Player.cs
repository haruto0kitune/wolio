using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
    public int m_hp = 1;

    private int OnTriggerEnter2DOverlap = 0;

    private PlayerControl m_playercontrol;

    [SerializeField]
    private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField]
    private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField]
    private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
    
    public string m_Direction = "right";

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    //private Animator m_Anim;            // Reference to the player's animator component.

    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.

    Transform m_transform;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TriggerEnter2D");
        if (transform.parent == null && other.gameObject.tag == "MovingFloor")
        {
            transform.parent = other.gameObject.transform;
            Debug.Log("parenting");
        }
        else if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("enemy");
            switch (m_hp)
            {
                case 1:
                    {
                        //Instantiate(Resources.Load("prefav/damage_hp"));
                        Destroy(GameObject.Find("hp1"));
                        m_hp--;
                        break;
                    }
                case 2:
                    {
                        //Instantiate(Resources.Load("prefav/damage_hp"));
                        Destroy(GameObject.Find("hp2"));
                        m_hp--;
                        break;
                    }
                case 3:
                    {
                        //Instantiate(Resources.Load("prefav/damage_hp"));
                        Destroy(GameObject.Find("hp3"));
                        m_hp--;
                        break;
                    }
                case 4:
                    {
                        //Instantiate(Resources.Load("prefav/damage_hp"));
                        Destroy(GameObject.Find("hp4"));
                        m_hp--;
                        break;
                    }
                case 5:
                    {
                        //Instantiate(Resources.Load("prefav/damage_hp"));
                        Destroy(GameObject.Find("hp5"));
                        m_hp--;
                        break;
                    }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("TriggerStay2D");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("TriggerExit2D");
        if (transform.parent != null && other.gameObject.tag == "MovingFloor")
        {
            transform.parent = null;
            Debug.Log("unparenting");
        }
    }

    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_playercontrol = GetComponent<PlayerControl>();
        //m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_transform = GetComponent<Transform>();
    }


    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_Grounded = true;
        }
        //m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        //m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
    }

    public IEnumerator Backstep(bool left, bool right, bool shift)
    {
        //m_playercontrol.m_Backstep = false;

        if (m_Direction == "left")
        //if (m_FacingRight)
        {
            Debug.Log("right backstep");
            if (shift && right)
            {
                for (int i = 0; i < 5; i++)
                {
                    m_transform.Translate(Vector3.right * 0.1f);
                    yield return null;
                }
            }
        }
        else if (m_Direction == "right")
        //else if (!m_FacingRight)
        {
            Debug.Log("left backstep");
            if (shift && left)
            {
                for (int i = 0; i < 5; i++)
                {
                    m_transform.Translate(Vector3.left * 0.1f);
                    yield return null;
                }
            }
        }

        // wait for 5 frames.
        for (int i = 0;i < 5;i++)
        {
            yield return null;
        }

        //m_playercontrol.m_Backstep = true;
    }

    public void Move(float move, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // Reduce the speed if crouching by the crouchSpeed multiplier
            // move = move;
            if (move > 0) m_Direction = "right";
            if (move < 0) m_Direction = "left";
            // The Speed animator parameter is set to the absolute value of the horizontal input.
            //m_Anim.SetFloat("Speed", Mathf.Abs(move));

            // Move the character
            //m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
            m_transform.Translate(move * m_MaxSpeed / 50, 0, 0);
            
            //m_transform.position = new Vector3((move + m_transform.position.x), m_transform.position.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                //m_Direction = "right";
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                //m_Direction = "left";
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump) /*m_Anim.GetBool("Ground")*/
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            //m_Anim.SetBool("Ground", false);
            //m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            m_transform.Translate(Vector3.up * 5f);
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
