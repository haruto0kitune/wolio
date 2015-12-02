using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    public int m_hp = 1;

    GameObject m_Gameover;
    GameObject m_GameoverInstance;
    
    public GameObject shot;
    public GameObject prefav;
    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    private Transform m_transform;
    private Rigidbody2D m_Rigidbody2D;
    public bool m_Grounded = false;            // Whether or not the player is grounded.
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.

    public bool GetFacingRight
    {
        get { return this.m_FacingRight; }
    }

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    public bool m_isDashing = false;
    private int m_shotwait = 0;

    [SerializeField]
    private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float m_DashSpeed = 10f;                    
    [SerializeField]
    private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField]
    private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField]
    private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private void Awake()
    {
        m_GroundCheck = transform.Find("GroundCheck");
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_transform = GetComponent<Transform>();
        m_Gameover = (GameObject)Resources.Load("Prefab/gameover");
    }

    private void Start()
    {
        UpdateAsObservables();
        FixedUpdateAsObservables();
        OnTriggerEnter2DAsObservables();
        OnTriggerStay2DAsObservables();
        OnTriggerExit2DAsObservables();
        ObserveEveryValueChangeds();
    }

    void FixedUpdateAsObservables()
    {
        this.FixedUpdateAsObservable()
            .Subscribe(_ =>
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
            });
    }

    void UpdateAsObservables ()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => Die());
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

    public void Dash(float direction, bool shift)
    {
        if ((shift && ((direction < 0) || (direction > 0))) && !m_isDashing)
        {
            StartCoroutine(DashCoroutine(direction, shift));
        }
    }

    public IEnumerator DashCoroutine(float direction, bool shift)
    {
        m_isDashing = true;

        //right backdash
        if (m_FacingRight && ((direction < 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                m_Rigidbody2D.velocity = new Vector2(direction * (m_DashSpeed - i * 2), m_Rigidbody2D.velocity.y);
                yield return null;
            }
        }
        //right frontdash
        else if (m_FacingRight && ((direction > 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                m_Rigidbody2D.velocity = new Vector2(direction * (m_DashSpeed - i * 2), m_Rigidbody2D.velocity.y);
                yield return null;
            }
        }
        //left frontdash
        else if (!m_FacingRight && ((direction < 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                m_Rigidbody2D.velocity = new Vector2(direction * (m_DashSpeed - i), m_Rigidbody2D.velocity.y);
                yield return null;
            }
        }
        //left backdash
        else if (!m_FacingRight && ((direction > 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                m_Rigidbody2D.velocity = new Vector2(direction * (m_DashSpeed - i), m_Rigidbody2D.velocity.y);
                yield return null;
            }
        }

        // wait for 5 frames.
        for (int i = 0; i < 23; i++)
        {
            yield return null;
        }

        m_isDashing = false;
        yield return null;
    }

    public void TurnAround(float direction)
    {
        if ((direction > 0 && !m_FacingRight) || (direction < 0 && m_FacingRight))
        {
            Flip();
        }
    }

    public void Run(float direction)
    {
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // Move the character
            m_Rigidbody2D.velocity = new Vector2(direction * m_MaxSpeed, m_Rigidbody2D.velocity.y);
        }
    }

    public void Jump(bool jump)
    {
        // If the player should jump...
        if ((bool)Physics2D.Linecast(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y - 0.539f), /*m_ground*/m_WhatIsGround) && jump)
        {
            // Add a vertical force to the player.
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    public void FireBall()
    {
        if (m_shotwait % 8 == 0)
        {
            prefav = Instantiate(shot, transform.position, transform.rotation) as GameObject;
            m_shotwait = 0;
        }

        m_shotwait++;
    }

    public void Die()
    {
        if((transform.position.y <= -5 || m_hp <= 0) && m_GameoverInstance == null)
        {
            m_GameoverInstance = Instantiate(m_Gameover) as GameObject;
            Destroy(gameObject);
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
