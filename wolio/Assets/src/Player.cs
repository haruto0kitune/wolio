using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Linq;

public class Player : MonoBehaviour
{
    [InspectorDisplay]
    public IntReactiveProperty Hp;

    public ReactiveProperty<bool> IsDead;
    public ReactiveProperty<bool> IsGrounded;
    public ReactiveProperty<bool> IsDashing;
    public ReactiveProperty<bool> IsTouchingWall;
    public ReactiveProperty<bool> FacingRight;

    [SerializeField]
    private GameObject ShotPrefab;
    private GameObject Shot;

    private Transform GroundCheck;    // A position marking where to check if the player is grounded.
    private Rigidbody2D Rigidbody2D;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    
    private int shotwait = 0;

    [SerializeField]
    private float MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float KnockBackSpeed = 3f;
    [SerializeField]
    private float DashSpeed = 10f;                    
    [SerializeField]
    private float JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField]
    private bool AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField]
    private LayerMask WhatIsGround;                  // A mask determining what is ground to the character

    private void Awake()
    {
        GroundCheck = transform.Find("GroundCheck");
        Rigidbody2D = GetComponent<Rigidbody2D>();
        ShotPrefab = Resources.Load("Prefab/fireball") as GameObject;

        IsDead = new ReactiveProperty<bool>(false);
        IsGrounded = new ReactiveProperty<bool>(false);
        IsDashing = new ReactiveProperty<bool>(false);
        IsTouchingWall = new ReactiveProperty<bool>(false);
        FacingRight = new ReactiveProperty<bool>(true);

        IsDead = this.Hp.Select(x => transform.position.y <= -5 || x <= 0).ToReactiveProperty();
    }

    private void Start()
    {
        UpdateAsObservables();
        FixedUpdateAsObservables();
        ObserveEveryValueChangeds();
    }

    void FixedUpdateAsObservables ()
    {
        /*this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                IsGrounded.Value = false;

                // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
                // This can be done using layers instead but Sample Assets will not overwrite your project settings.
                Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, k_GroundedRadius, WhatIsGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                        IsGrounded.Value = true;
                }
            });*/
        this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                if ((bool)Physics2D.Linecast(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y - 0.539f), WhatIsGround))
                {
                    IsGrounded.Value = true;
                }
                else
                {
                    IsGrounded.Value = false;
                }
            });
    }

    void UpdateAsObservables ()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => Die());

/*        this.UpdateAsObservable()
            .Subscribe(_ => Gizmos.DrawWireSphere(transform.position, 1.0f));*/
    }

    void ObserveEveryValueChangeds ()
    {
        this.ObserveEveryValueChanged(x => x.Hp.Value)
            .Subscribe(_ => StartCoroutine(BecomeInvincible()));

        this.ObserveEveryValueChanged(x => x.Hp.Value)
            .Where(x => Hp.Value == 4)
            .Subscribe(_ => Destroy(GameObject.Find("hp5")));

        this.ObserveEveryValueChanged(x => x.Hp.Value)
            .Where(x => Hp.Value == 3)
            .Subscribe(_ => Destroy(GameObject.Find("hp4")));

        this.ObserveEveryValueChanged(x => x.Hp.Value)
            .Where(x => Hp.Value == 2)
            .Subscribe(_ => Destroy(GameObject.Find("hp3")));

        this.ObserveEveryValueChanged(x => x.Hp.Value)
            .Where(x => Hp.Value == 1)
            .Subscribe(_ => Destroy(GameObject.Find("hp2")));

        this.ObserveEveryValueChanged(x => x.Hp.Value)
            .Where(x => Hp.Value == 0)
            .Subscribe(_ => Destroy(GameObject.Find("hp1")));
    }

    public void Dash(float direction, bool shift)
    {
        if ((shift && ((direction < 0) || (direction > 0))) && !IsDashing.Value)
        {
            StartCoroutine(DashCoroutine(direction, shift));
        }
    }

    public IEnumerator DashCoroutine(float direction, bool shift)
    {
        IsDashing.Value = true;

        //right backdash
        if (FacingRight.Value && ((direction < 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                Rigidbody2D.velocity = new Vector2(direction * (DashSpeed - i * 2), Rigidbody2D.velocity.y);
                yield return null;
            }
        }
        //right frontdash
        else if (FacingRight.Value && ((direction > 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                Rigidbody2D.velocity = new Vector2(direction * (DashSpeed - i * 2), Rigidbody2D.velocity.y);
                yield return null;
            }
        }
        //left frontdash
        else if (!FacingRight.Value && ((direction < 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                Rigidbody2D.velocity = new Vector2(direction * (DashSpeed - i * 2), Rigidbody2D.velocity.y);
                yield return null;
            }
        }
        //left backdash
        else if (!FacingRight.Value && ((direction > 0) && shift))
        {
            for (int i = 0; i < 5; i++)
            {
                Rigidbody2D.velocity = new Vector2(direction * (DashSpeed - i * 2), Rigidbody2D.velocity.y);
                yield return null;
            }
        }

        // wait for 5 frames.
        for (int i = 0; i < 23; i++)
        {
            yield return null;
        }

        IsDashing.Value = false;
        yield return null;
    }

    public void TurnAround(float direction)
    {
        if ((direction > 0 && !FacingRight.Value) || (direction < 0 && FacingRight.Value))
        {
            Flip();
        }
    }

    public void Run(float direction)
    {
        //only control the player if grounded or airControl is turned on
        if (IsGrounded.Value || AirControl)
        {
            // Move the character
            Rigidbody2D.velocity = new Vector2(direction * MaxSpeed, Rigidbody2D.velocity.y);
        }
    }

    public void Jump(bool jump)
    {
        // If the player should jump...
        if ((bool)Physics2D.Linecast(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y - 0.539f), /*ground*/WhatIsGround) && jump)
        {
            // Add a vertical force to the player.
            Rigidbody2D.AddForce(new Vector2(0f, JumpForce));
        }
    }

    public void FireBall()
    {
        if (shotwait == 8)
        {
            Shot = Instantiate(ShotPrefab, transform.position, transform.rotation) as GameObject;
            shotwait = 0;
        }

        shotwait++;
    }

    public void Die()
    {
        if((transform.position.y <= -5 || Hp.Value <= 0))
        {
            Debug.Log(this.IsDead.Value);
        }
    }

    public void Guard()
    {
        int left = -1;
        int right = 1;

        if ( FacingRight.Value ) 
        {
            Rigidbody2D.velocity = new Vector2(left * KnockBackSpeed, Rigidbody2D.velocity.y);
        }
        else if ( !FacingRight.Value )
        {
            Rigidbody2D.velocity = new Vector2(right * KnockBackSpeed, Rigidbody2D.velocity.y);
        }
        
    }

    public IEnumerator WallKickJump()
    {
        
        Flip();

        Rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
        Rigidbody2D.AddForce(new Vector2(500.0f, JumpForce));

        for (int i = 0; i < 15; i++)
        {
            Rigidbody2D.AddForce(new Vector2(500.0f, 0.0f));
            yield return null;
        }
    }

    public IEnumerator BecomeInvincible()
    {
        // Player become invincible.
        gameObject.layer = LayerMask.NameToLayer("Invincible");

        // Invincible frames.
        for(int i = 0;i < 36;i++)
        {
            for (int j = 0; j < 5; j++)
            {
                yield return null;
            }

            GetComponent<SpriteRenderer>().enabled = !(GetComponent<SpriteRenderer>().enabled);
        }

        // Player become normal state.
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        FacingRight.Value = !FacingRight.Value;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}