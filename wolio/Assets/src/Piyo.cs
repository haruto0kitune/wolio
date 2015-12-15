using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class Piyo : MonoBehaviour
{
    [SerializeField]
    private float JumpForce;
    [SerializeField]
    private LayerMask WhatIsGround;
    [SerializeField]
    private float MaxSpeed = 10f;

    private GameObject Shot;
    [SerializeField]
    private GameObject ShotPrefab;

    private int shotwait;
    [SerializeField]
    private bool CanAttack;

    [InspectorDisplay]
    public IntReactiveProperty Hp;
    public ReactiveProperty<bool> IsGrounded;
    public ReactiveProperty<bool> FacingRight;
    
    private Rigidbody2D Rigidbody2D;

    void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        IsGrounded = new ReactiveProperty<bool>(false);
        FacingRight = new ReactiveProperty<bool>(false);
    }

    // Use this for initialization
    void Start()
    {
        this.Hp
            .Where(x => x <= 0)
            .Subscribe(_ => Destroy(gameObject));

        this.UpdateAsObservable()
            .Where(x => IsGrounded.Value)
            .Subscribe(_ => Jump());

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                this.IsGrounded.Value = (bool)Physics2D.Linecast(
                    this.transform.position,
                    new Vector2(this.transform.position.x, this.transform.position.y - 0.539f),
                    WhatIsGround);
            });

        this.StartCoroutine(Run());

        this.OnTriggerEnter2DAsObservable()
            .Where(x => x.gameObject.tag == "Bullet")
            .Subscribe(_ => this.Hp.Value--);

        this.UpdateAsObservable()
            .Where(x => CanAttack)
            .Subscribe(_ => FireBall());
    }

    public IEnumerator Run()
    {
        while (true)
        {
            // Move left
            for (int i = 0; i < 10; i++)
            {
                Rigidbody2D.velocity = new Vector2(1 * MaxSpeed, Rigidbody2D.velocity.y);
                yield return null;
            }

            // Move right
            for (int i = 0; i < 10; i++)
            {
                Rigidbody2D.velocity = new Vector2(-1 * MaxSpeed, Rigidbody2D.velocity.y);
                yield return null;
            }
        }
    }

    public void Jump()
    {
        Rigidbody2D.AddForce(new Vector2(0f, JumpForce));
    }

    public void FireBall()
    {
        if (shotwait == 60)
        {
            Shot = Instantiate(ShotPrefab, transform.position, transform.rotation) as GameObject;
            shotwait = 0;
        }

        shotwait++;
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
