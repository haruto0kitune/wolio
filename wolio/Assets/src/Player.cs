using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [InspectorDisplay]
    public IntReactiveProperty Hp;
    public ReactiveProperty<bool> IsDead;
    public ReactiveProperty<bool> IsGrounded;
    public ReactiveProperty<bool> IsDashing;
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

        this.IsDead = new ReactiveProperty<bool>(false);
        this.IsGrounded = new ReactiveProperty<bool>(false);
        this.IsDashing = new ReactiveProperty<bool>(false);
        this.FacingRight = new ReactiveProperty<bool>(true);

        this.IsDead = this.Hp.Select(x => transform.position.y <= -5 || x <= 0).ToReactiveProperty();
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

    void FixedUpdateAsObservables ()
    {
        this.FixedUpdateAsObservable()
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
            .Where(x => x.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            .Subscribe(_ => Hp.Value--);

        this.OnTriggerEnter2DAsObservable()
            .Where(x => x.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
            .Subscribe(_ => Hp.Value--);
    }

    void OnTriggerStay2DAsObservables ()
    {
        this.OnTriggerStay2DAsObservable()
            .Where(x => x.gameObject.tag == "MovingFloor")
            .Where(x => !CrossPlatformInputManager.GetButton("Left") && !CrossPlatformInputManager.GetButton("Right"))
            .Subscribe(_ => transform.parent = _.gameObject.transform);

        this.OnTriggerStay2DAsObservable()
            .Where(x => x.gameObject.tag == "MovingFloor")
            .Where(x => CrossPlatformInputManager.GetButton("Left") || CrossPlatformInputManager.GetButton("Right"))
            .Subscribe(_ => transform.parent = null);
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
