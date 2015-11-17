using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    public int m_hp = 1;
    public string m_Direction = "right";
    public LayerMask m_ground; 

    private int OnTriggerEnter2DOverlap = 0;
    private PlayerControl m_playercontrol;
    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    private Transform m_transform;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_Grounded;            // Whether or not the player is grounded.
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up

    [SerializeField]
    private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField]
    private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField]
    private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private void Awake()
    {
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_playercontrol = GetComponent<PlayerControl>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_transform = GetComponent<Transform>();
    }

    private void Start()
    {
        UpdateAsObservables();
        OnTriggerEnter2DAsObservables();
        OnTriggerStay2DAsObservables();
        OnTriggerExit2DAsObservables();
        ObserveEveryValueChangeds();
    }

    void UpdateAsObservables ()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => m_Grounded = Physics2D.Linecast(this.transform.position, m_GroundCheck.transform.position));
    }

    void OnTriggerEnter2DAsObservables ()
    {
        this.OnTriggerEnter2DAsObservable()
            .Where(x => transform.parent == null && x.gameObject.tag == "MovingFloor" && !(CrossPlatformInputManager.GetButton("Left")) && !(CrossPlatformInputManager.GetButton("Right")))
            .Subscribe(_ => transform.parent = _.gameObject.transform);

        this.OnTriggerEnter2DAsObservable()
            .Where(x => x.gameObject.tag == "Enemy")
            .Subscribe(_ => m_hp--);
    }

    void OnTriggerStay2DAsObservables ()
    {
        this.OnTriggerStay2DAsObservable()
            .Where(x => x.gameObject.tag == "MovingFloor")
            .Subscribe(_ =>
            {
                if (!(CrossPlatformInputManager.GetButton("Left")) && !(CrossPlatformInputManager.GetButton("Right")))
                {
                    transform.parent = _.gameObject.transform;
                }
                else
                {
                    transform.parent = null;
                }
            });
    }

    void OnTriggerExit2DAsObservables ()
    {
        this.OnTriggerExit2DAsObservable()
            .Where(x => transform.parent != null && x.gameObject.tag == "MovingFloor" && (CrossPlatformInputManager.GetButton("Left")) || (CrossPlatformInputManager.GetButton("Right")))
            .Subscribe(_ => transform.parent = null);

        this.OnTriggerExit2DAsObservable()
            .Where(x => transform.parent != null && x.gameObject.tag == "MovingFloor")
            .Subscribe(_ => transform.parent = null);
    }
            
    void ObserveEveryValueChangeds ()
    {
        this.ObserveEveryValueChanged(x => x.m_hp)
            .Where(x => m_hp == 4)
            .Subscribe(_ => Destroy(GameObject.Find("hp5")));

        this.ObserveEveryValueChanged(x => x.m_hp)
            .Where(x => m_hp == 3)
            .Subscribe(_ => Destroy(GameObject.Find("hp4")));

        this.ObserveEveryValueChanged(x => x.m_hp)
            .Where(x => m_hp == 2)
            .Subscribe(_ => Destroy(GameObject.Find("hp3")));

        this.ObserveEveryValueChanged(x => x.m_hp)
            .Where(x => m_hp == 1)
            .Subscribe(_ => Destroy(GameObject.Find("hp2")));

        this.ObserveEveryValueChanged(x => x.m_hp)
            .Where(x => m_hp == 0)
            .Subscribe(_ => Destroy(GameObject.Find("hp1")));
    }

    public IEnumerator Backstep(bool left, bool right, bool shift)
    {
        if (m_Direction == "left")
        {
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
        {
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
        for (int i = 0; i < 5; i++)
        {
            yield return null;
        }
    }

    public void Move(float move, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // Reduce the speed if crouching by the crouchSpeed multiplier
            if (move > 0) m_Direction = "right";
            if (move < 0) m_Direction = "left";

            // Move the character
            m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (CrossPlatformInputManager.GetAxisRaw("Horizontal") > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (CrossPlatformInputManager.GetAxisRaw("Horizontal") < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if ((bool)Physics2D.Linecast(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y - 0.539f), m_ground) && jump)
        {
            // Add a vertical force to the player.
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
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
