using UnityEngine;
using System.Collections;

public class Piyo : MonoBehaviour
{
    [SerializeField]
    private int m_hp;
    [SerializeField]
    private float m_JumpForce;
    [SerializeField]
    private LayerMask m_WhatIsGround;
    [SerializeField]
    private float m_MaxSpeed = 10f;

    private Rigidbody2D m_Rigidbody2D;
    private bool m_isRunning;

    // Use this for initialization
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_hp <= 0)
        {
            Destroy(gameObject);
        }

        Run();
        Jump();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            m_hp--;
        }
    }

    public void Run()
    {
        if (!m_isRunning)
        {
            StartCoroutine(RunCoroutine());
        }
    }

    public IEnumerator RunCoroutine()
    {
        m_isRunning = true;

        for(int i = 0;i < 10;i++)
        {
            m_Rigidbody2D.velocity = new Vector2(1 * m_MaxSpeed, m_Rigidbody2D.velocity.y);
            yield return null;
        }

        for (int i = 0; i < 10; i++)
        {
            m_Rigidbody2D.velocity = new Vector2(-1 * m_MaxSpeed, m_Rigidbody2D.velocity.y);
            yield return null;
        }

        m_isRunning = false;
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
