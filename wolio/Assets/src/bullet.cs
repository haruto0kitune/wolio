using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private GameObject m_Camera;
    private Player m_Player;
    private bool m_FacingRight;
    
    // Use this for initialization
    void Start()
    {
        m_Camera = GameObject.Find("Main Camera");
        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_FacingRight = m_Player.GetFacingRight;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Release();
    }

    void Move()
    {
        if (m_FacingRight)
        {
            transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if((other.gameObject.tag == "Enemy") || (other.gameObject.tag == "Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    void Release()
    {
        if((m_Camera.transform.position.x + 8.3) <= transform.position.x || (m_Camera.transform.position.x - 8.3) >= transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
