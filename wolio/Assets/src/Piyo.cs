using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class Piyo : MonoBehaviour
{
    [SerializeField]
    private float m_JumpForce;
    [SerializeField]
    private LayerMask m_WhatIsGround;
    [SerializeField]
    private float m_MaxSpeed = 10f;

    [InspectorDisplay]
    public IntReactiveProperty Hp;
    
    private Rigidbody2D m_Rigidbody2D;

    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        this.Hp.Where(x => x <= 0).Subscribe(_ => Destroy(this.gameObject));
        this.StartCoroutine(Run());
        this.UpdateAsObservable().Subscribe(_ => Jump());
        this.OnTriggerEnter2DAsObservable().Where(x => x.gameObject.tag == "Bullet").Subscribe(_ => this.Hp.Value--);
    }

    public IEnumerator Run()
    {
        while (true)
        {
            // Move left
            for (int i = 0; i < 10; i++)
            {
                m_Rigidbody2D.velocity = new Vector2(1 * m_MaxSpeed, m_Rigidbody2D.velocity.y);
                yield return null;
            }

            // Move right
            for (int i = 0; i < 10; i++)
            {
                m_Rigidbody2D.velocity = new Vector2(-1 * m_MaxSpeed, m_Rigidbody2D.velocity.y);
                yield return null;
            }
        }
    }

    public void Jump()
    {
        // If the player should jump...
        if ((bool)Physics2D.Linecast(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y - 0.539f), m_WhatIsGround))
        {
            // Add a vertical force to the player.
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }
}
