using UnityEngine;
using System.Collections;

public class rotate_move_block : MonoBehaviour
{

    Transform m_transform;

    [SerializeField]
    private float m_radius = 1f;
    [SerializeField]
    private string m_direction = "left";

    float m_angle = 0f;
    float m_sin = 1 * Mathf.Sin(30 * Mathf.Deg2Rad);
    float m_cos = 1 * Mathf.Cos(180 * Mathf.Deg2Rad);

    void Start()
    {
        m_transform = GetComponent<Transform>();
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (true)
        {
            m_sin = m_radius * Mathf.Sin(m_angle * Mathf.Deg2Rad) / 100;
            m_cos = m_radius * Mathf.Cos(m_angle * Mathf.Deg2Rad) / 100;
            m_transform.Translate(m_sin, m_cos, 0);
            if (m_angle == 360 || m_angle == -360) m_angle = 0;
            if (m_direction == "left") m_angle--;
            if (m_direction == "right") m_angle++;

            yield return null;
        }
    }
}