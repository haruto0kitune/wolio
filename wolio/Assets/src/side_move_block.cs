using UnityEngine;
using System.Collections;

public class side_move_block : MonoBehaviour
{

    Transform m_transform;
    Rigidbody2D m_rigidbody2d;

    [SerializeField]
    private float m_speed = 0.01f;
    [SerializeField]
    private float m_volume = 100;

    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_rigidbody2d = GetComponent<Rigidbody2D>();
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (true)
        {
            // move left
            for (int i = 0; i < m_volume; i++)
            {
                m_transform.Translate(Vector3.left * m_speed);
                yield return null;
            }

            // move right
            for (int i = 0; i < m_volume; i++)
            {
                m_transform.Translate(Vector3.right * m_speed);
                yield return null;
            }
        }
    }
}
